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

        // Основной метод для начала редактирования файла
        public void StartEditing(string filePath)
        {
            try
            {
                var lines = ReadFileWithEncoding(filePath);

                // Проверка наличия данных для обработки
                if (!lines.Any(line => Regex.IsMatch(line, @"=\s*PRODUCT\s*\(", RegexOptions.IgnoreCase)))
                {
                    MessageBox.Show($"Файл {filePath} не содержит данных для обработки. Попробуйте пересохранить stp-файл в новых версиях КОМПАС-3D (v20+).",
                                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Загрузка файла автозамены
                string autoCorrectFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AutoCorrect.txt");
                _autoCorrect.LoadAutoCorrectFile(autoCorrectFilePath);

                // Обработка строк файла
                var processedLines = ProcessStepFile(lines);

                // Запись обработанных строк обратно в файл
                WriteFileWithEncoding(filePath, processedLines);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось обработать файл {filePath}: {ex.Message}",
                                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                MainWindow.LogError(ex.Message);
            }
        }

        // Чтение файла с указанным кодированием
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

        // Запись строк в файл с указанным кодированием
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

        // Обработка строк STEP файла
        // Обработка строк STEP файла с учетом дубликатов
        private List<string> ProcessStepFile(List<string> lines)
        {
            var processedLines = new List<string>();
            var buffer = string.Empty;
            var insideProduct = false; // Флаг, указывающий, что мы обрабатываем строку с =PRODUCT
            var productDictionary = new Dictionary<string, int>(); // Словарь для отслеживания дубликатов

            foreach (var line in lines)
            {
                // Если строка содержит начало =PRODUCT, активируем флаг
                if (Regex.IsMatch(line, @"=\s*PRODUCT\s*\(", RegexOptions.IgnoreCase) || insideProduct)
                {
                    insideProduct = true; // Мы внутри строки с =PRODUCT
                    buffer += line.Trim(); // Убираем лишние пробелы и добавляем строку в буфер

                    // Если строка заканчивается на ";", значит, это конец записи
                    if (line.TrimEnd().EndsWith(";"))
                    {
                        // Обрабатываем только строки с =PRODUCT
                        if (Regex.IsMatch(buffer, @"\\X2\\(.*?)\\X0\\"))
                        {
                            buffer = EditProductHexLine(buffer); // Декодируем шестнадцатиричные значения
                        }

                        buffer = HandleQuotes(buffer); // Обрабатываем кавычки
                        buffer = _autoCorrect.ApplyReplacements(buffer); // Пропускаем строку buffer через автозамену AutoCorrect

                        // Проверяем на дубликаты
                        buffer = HandleDuplicateProducts(buffer, productDictionary);

                        processedLines.Add(buffer); // Добавляем обработанную строку в результат

                        buffer = string.Empty; // Очищаем буфер
                        insideProduct = false; // Выходим из режима обработки =PRODUCT
                    }
                }
                else
                {
                    // Строки вне =PRODUCT не обрабатываются AutoCorrect
                    processedLines.Add(line); // Добавляем строку как есть
                }
            }

            // Если в буфере остались строки (например, файл не завершен корректно), добавляем их
            if (!string.IsNullOrEmpty(buffer))
            {
                if (Regex.IsMatch(buffer, @"\\X2\\(.*?)\\X0\\"))
                {
                    buffer = EditProductHexLine(buffer); // Декодируем оставшиеся значения
                }
                buffer = HandleQuotes(buffer); // Обрабатываем кавычки только в =PRODUCT
                buffer = _autoCorrect.ApplyReplacements(buffer); // Пропускаем через автозамену AutoCorrect
                buffer = HandleDuplicateProducts(buffer, productDictionary); // Проверяем на дубликаты
                processedLines.Add(buffer); // Добавляем обработанную строку
            }

            return processedLines;
        }

        // Метод для обработки дубликатов =PRODUCT
        private string HandleDuplicateProducts(string line, Dictionary<string, int> productDictionary)
        {
            var regexQuotes = new Regex(@"'([^']*)'"); // Ищем значения в кавычках
            var matchesQuotes = regexQuotes.Matches(line);

            if (matchesQuotes.Count >= 2) // Если найдено два или более значений в кавычках
            {
                var valueOboznachenie = matchesQuotes[0].Groups[1].Value; // Значение в первых кавычках
                var valueNaimenovanie = matchesQuotes[1].Groups[1].Value; // Значение во вторых кавычках

                if (productDictionary.ContainsKey(valueOboznachenie))
                {
                    // Увеличиваем индекс копии
                    productDictionary[valueOboznachenie]++;
                    var copyIndex = productDictionary[valueOboznachenie];

                    // Добавляем "Копия №" к valueNaimenovanie
                    valueNaimenovanie = $"{valueNaimenovanie} (Копия {copyIndex})";

                    // Формируем новую строку, изменяя только нужную часть
                    line = line.Substring(0, matchesQuotes[1].Index) // До начала вторых кавычек
                           + $"'{valueNaimenovanie}'" // Новое значение
                           + line.Substring(matchesQuotes[1].Index + matchesQuotes[1].Length); // После вторых кавычек
                }
                else
                {
                    // Добавляем новое значение в словарь
                    productDictionary[valueOboznachenie] = 0;
                }
            }

            return line; // Возвращаем строку
        }


        // Обработка строки с шестнадцатиричными значениями
        private string EditProductHexLine(string line)
        {
            line = _autoCorrect.ApplyReplacements(line);

            // Поиск и замена шестнадцатиричных значений
            var regex = new Regex(@"\\X2\\(.*?)\\X0\\");
            var matches = regex.Matches(line);

            foreach (Match match in matches)
            {
                string hexValue = match.Groups[1].Value;
                string decodedValue = DecodeHexValue(hexValue);
                line = line.Replace(match.Value, decodedValue);
            }

            line = _autoCorrect.ApplyReplacements(line);
            //HandleQuotes(line); // Обработка кавычек в строке
            return line;
        }
        private string HandleQuotes(string line)
        {
            var regexQuotes = new Regex(@"'([^']*)'"); // Ищем значения в кавычках
            var matchesQuotes = regexQuotes.Matches(line);

            if (matchesQuotes.Count >= 2) // Если найдено два или более значений в кавычках
            {
                var valueOboznachenie = matchesQuotes[0].Groups[1].Value; // Значение в первых кавычках
                var valueNaimenovanie = matchesQuotes[1].Groups[1].Value; // Значение во вторых кавычках

                if (string.IsNullOrEmpty(valueOboznachenie))
                {
                    return line; // Если valueOboznachenie пустое, возвращаем строку как есть
                }

                if (valueNaimenovanie.Contains(valueOboznachenie))
                {
                    return line; // Если valueOboznachenie уже содержится в valueNaimenovanie, возвращаем строку как есть
                }

                var newValueNaimenovanie = $"{valueOboznachenie} {valueNaimenovanie}"; // Новое значение для вторых кавычек

                // Формируем новую строку, изменяя только нужную часть
                line = line.Substring(0, matchesQuotes[1].Index) // До начала вторых кавычек
                       + $"'{newValueNaimenovanie}'" // Новое значение
                       + line.Substring(matchesQuotes[1].Index + matchesQuotes[1].Length); // После вторых кавычек
            }

            return line; // Возвращаем строку
        }


        // Извлечение шестнадцатиричного значения из строки
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

        // Декодирование шестнадцатиричного значения в строку
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
    }
}