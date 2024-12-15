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
            var valueTracker = new Dictionary<string, int>(); // Словарь для отслеживания значений

            foreach (var line in lines)
            {
                // Accumulate lines into the buffer
                buffer += (buffer.Length > 0 ? " " : "") + line.Trim();

                // Process the buffer when a complete statement is found (ends with ";")
                if (buffer.TrimEnd().EndsWith(";"))
                {
                    // Check if the buffer contains `=PRODUCT(` to decide whether to process it
                    if (Regex.IsMatch(buffer, @"=\s*PRODUCT\s*\(", RegexOptions.IgnoreCase))
                    {
                        // If the buffer contains `\X2\`, call the corresponding method
                        if (buffer.Contains(@"\X2\"))
                        {
                            buffer = EditProductHexLine(buffer);
                        }
                        else
                        {
                            buffer = EditProductCyrillicLine(buffer);
                        }

                        // Получаем значения из строки
                        var regexQuotes = new Regex(@"'([^']*)'");
                        var matchesQuotes = regexQuotes.Matches(buffer);

                        if (matchesQuotes.Count >= 2)
                        {
                            var value1 = matchesQuotes[0].Groups[1].Value; // Первое значение
                            var value2 = matchesQuotes[1].Groups[1].Value; // Второе значение

                            // Формируем ключ для отслеживания значений
                            string key = $"{value1},{value2}";

                            // Проверяем, есть ли уже такое значение в словаре
                            if (valueTracker.ContainsKey(key))
                            {
                                // Увеличиваем счетчик копий
                                valueTracker[key]++;
                                // Добавляем "(копия n)" к value2
                                value2 += $"(копия {valueTracker[key]})";
                            }
                            else
                            {
                                // Если это первое вхождение, добавляем в словарь
                                valueTracker[key] = 1;
                            }

                            // Заменяем значение в строке
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

            // Add any remaining buffer content (if the file ends without a semicolon)
            if (!string.IsNullOrEmpty(buffer))
            {
                processedLines.Add(buffer);
            }

            return processedLines;
        }
        private string ExtractHexValue(string input)
        {
            // Находим индексы управляющих символов
            int startIndex = input.IndexOf(@"\X2\") + 4; // Индекс сразу после \X2\
            int endIndex = input.IndexOf(@"\X0\", startIndex); // Индекс \X0\ после \X2\

            // Проверяем, что оба индекса найдены
            if (startIndex < 4 || endIndex == -1)
            {
                throw new FormatException("Hex value not found in the input string.");
            }

            // Извлекаем шестнадцатеричное значение
            string hexValue = input.Substring(startIndex, endIndex - startIndex);
            return hexValue;
        }

        private string DecodeHexValue(string hexValue)
        {
            var result = new StringBuilder();

            // Проверяем, что длина строки корректна
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
                catch (FormatException)
                {
                    throw new FormatException($"Invalid hex code: {hexChar}");
                }
            }
            return result.ToString();
        }
        private string EditProductHexLine(string line)
        {
            // Применяем автозамену для всей строки
            line = _autoCorrect.ApplyReplacements(line);

            // Используем регулярное выражение для поиска всех вхождений \X2\...\X0\
            var regex = new Regex(@"\\X2\\(.*?)\\X0\\");
            var matches = regex.Matches(line);

            // Для каждого найденного значения
            foreach (Match match in matches)
            {
                // Извлекаем шестнадцатеричное значение
                string hexValue = match.Groups[1].Value;

                // Декодируем шестнадцатеричное значение
                string decodedValue = DecodeHexValue(hexValue);

                // Заменяем в строке
                line = line.Replace(match.Value, decodedValue);
            }

            // Применяем автозамену к всей строке после декодирования
            line = _autoCorrect.ApplyReplacements(line);

            // Обработка значений в одинарных кавычках
            var regexQuotes = new Regex(@"'([^']*)'");
            var matchesQuotes = regexQuotes.Matches(line);

            if (matchesQuotes.Count >= 2)
            {
                var value1 = matchesQuotes[0].Groups[1].Value; // Первое значение
                var value2 = matchesQuotes[1].Groups[1].Value; // Второе значение

                // Применяем автозамену к значениям (включая value1 и value2)
                value1 = _autoCorrect.ApplyReplacements(value1);
                value2 = _autoCorrect.ApplyReplacements(value2);

                // Если строки одинаковые, не меняем
                if (value1 != value2)
                {
                    // Добавляем строку value1 перед value2 с пробелом, если она еще не добавлена
                    value2 = value1 + " " + value2;

                    // Заменяем второе значение в строке на объединенное значение
                    line = line.Replace($"'{matchesQuotes[1].Groups[1].Value}'", $"'{value2}'");
                }
            }

            return line;
        }
        private string EditProductCyrillicLine(string line)
        {
            // Применяем автозамену для всей строки
            line = _autoCorrect.ApplyReplacements(line);

            var regex = new Regex(@"'([^']*)'");
            var matches = regex.Matches(line);

            // Если не нашли два значения, возвращаем строку как есть
            if (matches.Count < 2)
                return line;

            var value1 = matches[0].Groups[1].Value; // Первое значение
            var value2 = matches[1].Groups[1].Value; // Второе значение

            // Применяем автозамену к значениям
            value1 = _autoCorrect.ApplyReplacements(value1);
            value2 = _autoCorrect.ApplyReplacements(value2);

            // Если значения равны, возвращаем строку без изменений
            if (value1 == value2)
            {
                return line;
            }
            value1 = _autoCorrect.ApplyReplacements(value1);
            value2 = _autoCorrect.ApplyReplacements(value2);

            // Если value1 полностью содержит value2, очищаем value2 и записываем в него value1
            if (value1.Contains(value2))
            {
                value1 = _autoCorrect.ApplyReplacements(value1);
                value2 = _autoCorrect.ApplyReplacements(value2);
                value2 = value1; // Заменяем value2 на value1
            }
            else
            {
                // Если значения не равны и не одно не содержит другое, добавляем value1 перед value2
                value2 = value1 + " " + value2;
            }

            // Заменяем второе значение в строке на обновленное value2
            return line.Replace($"'{matches[1].Groups[1].Value}'", $"'{value2}'");
        }
    }
}
