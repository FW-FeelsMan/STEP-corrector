using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace STEP_corrector
{
    public partial class EditorModel : Window
    {
        public void StartEditingModel(string modelPath)
        {
            string tempDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "STEP_corrector_Temp");
            string extractedDir = Path.Combine(tempDir, Path.GetFileNameWithoutExtension(modelPath));

            try
            {
                if (!Directory.Exists(tempDir))
                {
                    Directory.CreateDirectory(tempDir);
                }

                if (Directory.Exists(extractedDir))
                {
                    Directory.Delete(extractedDir, true);
                }

                Directory.CreateDirectory(extractedDir);
                using (ZipFile zip = ZipFile.Read(modelPath))
                {
                    zip.ExtractAll(extractedDir, ExtractExistingFileAction.OverwriteSilently);
                }

                string metaFilePath = Path.Combine(extractedDir, "MetaProductInfo");

                if (File.Exists(metaFilePath))
                {
                    var lines = ReadFileWithEncoding(metaFilePath, Encoding.BigEndianUnicode);
                    ProcessMetaProductInfo(lines);

                    SaveFileWithEncoding(metaFilePath, lines, Encoding.BigEndianUnicode);                   
                }
                else
                {
                    MessageBox.Show("Файл MetaProductInfo не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                string archivePath = RepackToOriginalFormat(extractedDir, modelPath);                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обработке файла: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (Directory.Exists(tempDir))
                {
                    Directory.Delete(tempDir, true);
                }
            }
        }

        private void ProcessMetaProductInfo(List<string> lines)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i];

                if (line.Contains("<property id=\"name\" value=\""))
                {
                    string nameValue = ExtractValue(line);

                    for (int j = i + 1; j < lines.Count; j++)
                    {
                        string baseLine = lines[j];

                        if (baseLine.Contains("<property id=\"base\" value=\""))
                        {
                            string baseValue = ExtractValue(baseLine);

                            if (!string.IsNullOrEmpty(baseValue) && nameValue.Contains(baseValue))
                            {
                                string updatedName = nameValue.Replace(baseValue, "").Trim();
                                lines[i] = ReplaceValue(lines[i], updatedName); 
                                break; 
                            }
                        }
                    }
                }
            }
        }

        private string ExtractValue(string line)
        {
            var match = Regex.Match(line, "value=\"(.*?)\"");
            return match.Success ? match.Groups[1].Value : string.Empty;
        }

        private string ReplaceValue(string line, string newValue)
        {
            return Regex.Replace(line, "value=\"(.*?)\"", $"value=\"{newValue}\"");
        }

        private List<string> ReadFileWithEncoding(string filePath, Encoding encoding)
        {
            string content = File.ReadAllText(filePath, encoding);
            return new List<string>(content.Split(new[] { '\n' }, StringSplitOptions.None));
        }

        private void SaveFileWithEncoding(string filePath, List<string> lines, Encoding encoding)
        {
            string unixFormattedContent = string.Join("\n", lines);
            File.WriteAllText(filePath, unixFormattedContent, encoding);
        }

        private string RepackToOriginalFormat(string folderPath, string originalFilePath)
        {
            string originalExtension = Path.GetExtension(originalFilePath);
            string newArchivePath = Path.ChangeExtension(originalFilePath, $"{originalExtension}");

            try
            {
                using (ZipFile zip = new ZipFile())
                {
                    zip.CompressionLevel = Ionic.Zlib.CompressionLevel.None; 
                    zip.UseZip64WhenSaving = Zip64Option.Never;

                    zip.AddDirectory(folderPath);
                    zip.Save(newArchivePath);
                }

                return newArchivePath;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Ошибка при упаковке в архив: {ex.Message}");
            }
        }
    }
}
