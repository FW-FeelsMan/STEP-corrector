using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace STEP_corrector
{
    public partial class Editor : Window
    {
        private AutoCorrect _autoCorrect = new AutoCorrect();

        public Editor()
        {
            InitializeComponent();
        }

        public void StartEditing(string filePath)
        {
            try
            {
                var lines = ReadFileWithEncoding(filePath);

                if (!lines.Any(line => Regex.IsMatch(line, @"=\s*PRODUCT\s*\(", RegexOptions.IgnoreCase)))
                {
                    MessageBox.Show($"Файл {filePath} не содержит данных для обработки. Попробуйте пересохранить stp-файл в новых версиях КОМПАС-3D (v20+).",
                                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string autoCorrectFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AutoCorrect.txt");
                _autoCorrect.LoadAutoCorrectFile(autoCorrectFilePath);

                var processedLines = ProcessStepFile(lines);

                WriteFileWithEncoding(filePath, processedLines);

                MessageBox.Show("Обработка завершена. Подробности ошибок (если есть) в errorLog.log.",
                                "Завершено", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось обработать файл {filePath}: {ex.Message}",
                                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                MainWindow.LogError(ex.Message);
            }
        }

        private List<string> ReadFileWithEncoding(string filePath)
        {
            var lines = new List<string>();
            using (var reader = new StreamReader(filePath, Encoding.GetEncoding("windows-1251")))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    lines.Add(line);
                }
            }
            return lines;
        }

        private void WriteFileWithEncoding(string filePath, List<string> lines)
        {
            using (var writer = new StreamWriter(filePath, false, Encoding.GetEncoding("windows-1251")))
            {
                foreach (var line in lines)
                {
                    writer.WriteLine(line);
                }
            }
        }

        private List<string> ProcessStepFile(List<string> lines)
        {
            var processedLines = new List<string>();
            var buffer = string.Empty;
            var insideProduct = false;
            var productDictionary = new Dictionary<string, int>();
            int lineNumber = 0;

            foreach (var line in lines)
            {
                lineNumber++;

                if (Regex.IsMatch(line, @"=\s*PRODUCT\s*\(", RegexOptions.IgnoreCase) || insideProduct)
                {
                    insideProduct = true;
                    buffer += line.Trim();

                    if (line.TrimEnd().EndsWith(";"))
                    {
                        try
                        {
                            if (Regex.IsMatch(buffer, @"\\X2\\(.*?)\\X0\\"))
                            {
                                buffer = EditProductHexLine(buffer, lineNumber);
                            }

                            buffer = HandleQuotes(buffer);
                            buffer = _autoCorrect.ApplyReplacements(buffer);
                            buffer = HandleDuplicateProducts(buffer, productDictionary);

                            processedLines.Add(buffer);
                        }
                        catch (Exception ex)
                        {
                            MainWindow.LogError($"Ошибка в строке {lineNumber}: {ex.Message}\nСтрока: {buffer}");
                            processedLines.Add(buffer);
                        }

                        buffer = string.Empty;
                        insideProduct = false;
                    }
                }
                else
                {
                    processedLines.Add(line);
                }
            }

            if (!string.IsNullOrEmpty(buffer))
            {
                try
                {
                    if (Regex.IsMatch(buffer, @"\\X2\\(.*?)\\X0\\"))
                    {
                        buffer = EditProductHexLine(buffer, lineNumber);
                    }
                    buffer = HandleQuotes(buffer);
                    buffer = _autoCorrect.ApplyReplacements(buffer);
                    buffer = HandleDuplicateProducts(buffer, productDictionary);
                    processedLines.Add(buffer);
                }
                catch (Exception ex)
                {
                    MainWindow.LogError($"Ошибка в строке {lineNumber}: {ex.Message}\nСтрока: {buffer}");
                }
            }

            return processedLines;
        }

        private string HandleDuplicateProducts(string line, Dictionary<string, int> productDictionary)
        {
            var regexQuotes = new Regex(@"'([^']*)'");
            var matchesQuotes = regexQuotes.Matches(line);

            if (matchesQuotes.Count >= 2)
            {
                var valueOboznachenie = matchesQuotes[0].Groups[1].Value;
                var valueNaimenovanie = matchesQuotes[1].Groups[1].Value;

                if (productDictionary.ContainsKey(valueOboznachenie))
                {
                    productDictionary[valueOboznachenie]++;
                    var copyIndex = productDictionary[valueOboznachenie];
                    valueNaimenovanie = $"{valueNaimenovanie} (Копия {copyIndex})";

                    line = line.Substring(0, matchesQuotes[1].Index)
                           + $"'{valueNaimenovanie}'"
                           + line.Substring(matchesQuotes[1].Index + matchesQuotes[1].Length);
                }
                else
                {
                    productDictionary[valueOboznachenie] = 0;
                }
            }

            return line;
        }

        private string EditProductHexLine(string line, int lineNumber)
        {
            line = _autoCorrect.ApplyReplacements(line);

            var regex = new Regex(@"\\X2\\(.*?)\\X0\\");
            var matches = regex.Matches(line);

            foreach (Match match in matches)
            {
                try
                {
                    string hexValue = match.Groups[1].Value;

                    if (hexValue.Length % 4 != 0)
                    {
                        MainWindow.LogError($"Ошибка в строке {lineNumber}: Hex value '{hexValue}' имеет некорректный формат.");
                        continue;
                    }

                    string decodedValue = DecodeHexValue(hexValue);
                    line = line.Replace(match.Value, decodedValue);
                }
                catch (FormatException ex)
                {
                    MainWindow.LogError($"Ошибка в строке {lineNumber}: {ex.Message}\nСтрока: {line}");
                }
            }

            return line;
        }

        private string DecodeHexValue(string hexValue)
        {
            var result = new StringBuilder();

            if (hexValue.Length % 4 != 0)
            {
                throw new FormatException("Hex value has an invalid format.");
            }

            for (int i = 0; i < hexValue.Length; i += 4)
            {
                string hexChar = hexValue.Substring(i, 4);
                int charCode = int.Parse(hexChar, NumberStyles.HexNumber);
                result.Append((char)charCode);
            }
            return result.ToString();
        }

        private string HandleQuotes(string line)
        {
            var regexQuotes = new Regex(@"'([^']*)'");
            var matchesQuotes = regexQuotes.Matches(line);

            if (matchesQuotes.Count >= 2)
            {
                var valueOboznachenie = matchesQuotes[0].Groups[1].Value;
                var valueNaimenovanie = matchesQuotes[1].Groups[1].Value;

                if (string.IsNullOrEmpty(valueOboznachenie))
                {
                    return line;
                }

                if (valueNaimenovanie.Contains(valueOboznachenie))
                {
                    return line;
                }

                var newValueNaimenovanie = $"{valueOboznachenie} {valueNaimenovanie}";

                line = line.Substring(0, matchesQuotes[1].Index)
                       + $"'{newValueNaimenovanie}'"
                       + line.Substring(matchesQuotes[1].Index + matchesQuotes[1].Length);
            }

            return line;
        }
    }
}
