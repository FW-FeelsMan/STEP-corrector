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

                bool containsProduct = lines.Any(line => Regex.IsMatch(line, @"=\s*PRODUCT\s*\(", RegexOptions.IgnoreCase));

                if (!containsProduct)
                {
                    MessageBox.Show($"Файл {filePath} не содержит данных для обработки. Попробуйте пересохранить stp-файл в новых версиях КОМПАС-3D (v20+).",
                                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string autoCorrectFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AutoCorrect.txt");
                _autoCorrect.LoadAutoCorrectFile(autoCorrectFilePath);

                var processedLines = ProcessStepFile(new List<string>(lines));

                WriteFileWithEncoding(filePath, processedLines);
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
            var valueTracker = new Dictionary<string, int>(); 

            foreach (var line in lines)
            {
                buffer += (buffer.Length > 0 ? " " : "") + line.Trim();

                if (buffer.TrimEnd().EndsWith(";"))
                {
                    if (Regex.IsMatch(buffer, @"=\s*PRODUCT\s*\(", RegexOptions.IgnoreCase))
                    {
                        if (buffer.Contains(@"\X2\"))
                        {
                            buffer = EditProductHexLine(buffer);
                        }
                        else
                        {
                            buffer = EditProductCyrillicLine(buffer);
                        }
                        var regexQuotes = new Regex(@"'([^']*)'");
                        var matchesQuotes = regexQuotes.Matches(buffer);

                        if (matchesQuotes.Count >= 2)
                        {
                            var value1 = matchesQuotes[0].Groups[1].Value; 
                            var value2 = matchesQuotes[1].Groups[1].Value; 

                            string key = $"{value1},{value2}";

                            if (valueTracker.ContainsKey(key))
                            {
                                valueTracker[key]++;
                                value2 += $"(копия {valueTracker[key]})";
                            }
                            else
                            {
                                valueTracker[key] = 1;
                            }

                            buffer = buffer.Replace(matchesQuotes[1].Groups[1].Value, value2);
                        }
                    }

                    processedLines.Add(buffer);
                    buffer = string.Empty;
                }
                else if (!Regex.IsMatch(buffer, @"=\s*PRODUCT\s*\(", RegexOptions.IgnoreCase))
                {
                    processedLines.Add(buffer);
                    buffer = string.Empty;
                }
            }

            if (!string.IsNullOrEmpty(buffer))
            {
                processedLines.Add(buffer);
            }

            return processedLines;
        }
        private string ExtractHexValue(string input)
        {
            int startIndex = input.IndexOf(@"\X2\") + 4; 
            int endIndex = input.IndexOf(@"\X0\", startIndex); 

            if (startIndex < 4 || endIndex == -1)
            {
                throw new FormatException("Hex value not found in the input string.");
                
            }

            string hexValue = input.Substring(startIndex, endIndex - startIndex);
            return hexValue;
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
                try
                {
                    int charCode = int.Parse(hexChar, NumberStyles.HexNumber);
                    result.Append((char)charCode);
                }
                catch (FormatException ex)
                {
                    throw new FormatException($"Invalid hex code: {hexChar}");                    
                }
            }
            return result.ToString();
        }
        private string EditProductHexLine(string line)
        {
            line = _autoCorrect.ApplyReplacements(line);

            var regex = new Regex(@"\\X2\\(.*?)\\X0\\");
            var matches = regex.Matches(line);

            foreach (Match match in matches)
            {
                string hexValue = match.Groups[1].Value;

                string decodedValue = DecodeHexValue(hexValue);

                line = line.Replace(match.Value, decodedValue);
            }

            line = _autoCorrect.ApplyReplacements(line);

            var regexQuotes = new Regex(@"'([^']*)'");
            var matchesQuotes = regexQuotes.Matches(line);

            if (matchesQuotes.Count >= 2)
            {
                var value1 = matchesQuotes[0].Groups[1].Value; 
                var value2 = matchesQuotes[1].Groups[1].Value; 

                value1 = _autoCorrect.ApplyReplacements(value1);
                value2 = _autoCorrect.ApplyReplacements(value2);

                if (value1 != value2)
                {
                    value2 = value1 + " " + value2;
                    line = line.Replace($"'{matchesQuotes[1].Groups[1].Value}'", $"'{value2}'");
                }
            }

            return line;
        }
        private string EditProductCyrillicLine(string line)
        {
            line = _autoCorrect.ApplyReplacements(line);

            var regex = new Regex(@"'([^']*)'");
            var matches = regex.Matches(line);

            if (matches.Count < 2)
                return line;

            var value1 = matches[0].Groups[1].Value; 
            var value2 = matches[1].Groups[1].Value;

            value1 = _autoCorrect.ApplyReplacements(value1);
            value2 = _autoCorrect.ApplyReplacements(value2);

            if (value1 == value2)
            {
                return line;
            }
            value1 = _autoCorrect.ApplyReplacements(value1);
            value2 = _autoCorrect.ApplyReplacements(value2);

            if (value1.Contains(value2))
            {
                value1 = _autoCorrect.ApplyReplacements(value1);
                value2 = _autoCorrect.ApplyReplacements(value2);
                value2 = value1;
            }
            else
            {
                value2 = value1 + " " + value2;
            }

            return line.Replace($"'{matches[1].Groups[1].Value}'", $"'{value2}'");
        }
    }
}
