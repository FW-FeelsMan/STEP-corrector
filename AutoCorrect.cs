using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;

namespace STEP_corrector
{
    public class AutoCorrect
    {
        private readonly Dictionary<string, string> _replacements = new Dictionary<string, string>();

        public void LoadAutoCorrectFile(string filePath)
        {
            try
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

                foreach (var line in lines)
                {
                    var parts = line.Split(':');
                    if (parts.Length == 2)
                    {
                        _replacements[parts[0].Trim()] = parts[1].Trim();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке файла автозамены: {ex.Message}",
                                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public string ApplyReplacements(string line)
        {
            foreach (var replacement in _replacements)
            {
                if (string.IsNullOrEmpty(replacement.Value))
                {
                    line = line.Replace(replacement.Key, string.Empty);
                }
                else
                {
                    // Заменяем значение
                    line = line.Replace(replacement.Key, replacement.Value);
                }
            }

            return line;
        }

        private string ConvertToStepFormat(string value)
        {
            var result = new StringBuilder();
            foreach (char c in value)
            {
                result.Append($"\\X2\\{((int)c).ToString("X4", CultureInfo.InvariantCulture)}\\X0\\");
            }
            return result.ToString();
        }
    }
}
