using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml.Linq;

namespace STEP_corrector
{
    public class EditorModelProp
    {
        private MainWindow _mainWindow;

        private string _fileExtensionProp;
        public string FileNameProp { get; set; }
        public string FileExtensionProp
        {
            get => _fileExtensionProp;
            set
            {
                _fileExtensionProp = value;
                FileType = DetermineFileType();
            }
        }
        public string FileType { get; private set; }
        public string Nomination { get; set; }
        public string Designation { get; set; }

        public EditorModelProp(MainWindow mainWindow)
        {
            _mainWindow = mainWindow ?? throw new ArgumentNullException(nameof(mainWindow));
        }

        private string DetermineFileType()
        {
            switch (_fileExtensionProp.ToLower())
            {
                case ".m3d":
                    return "Деталь";
                case ".a3d":
                    return "Сборка";
                default:
                    return "Неизвестный тип";
            }
        }

        public void StartEditingModel(string modelPath)
        {
            FileExtensionProp = Path.GetExtension(modelPath);
            string tempDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "STEP_corrector_Temp_Prop");

            try
            {
                Directory.CreateDirectory(tempDir);

                string extractedDir = Path.Combine(tempDir, Path.GetFileNameWithoutExtension(modelPath));
                Directory.CreateDirectory(extractedDir);
                using (ZipFile zip = ZipFile.Read(modelPath))
                {
                    zip.ExtractAll(extractedDir, ExtractExistingFileAction.OverwriteSilently);
                }

                string metaFilePath = Path.Combine(extractedDir, "MetaProductInfo");
                if (File.Exists(metaFilePath))
                {
                    XDocument xDoc = XDocument.Load(metaFilePath);

                    string massValue = ExtractMassValue(xDoc);
                    string nameValue = ExtractNameValue(xDoc);
                    string baseValue = ExtractBaseValue(xDoc);
                    string positionValue = ExtractPositionValue(xDoc);
                    string sectionNameValue = ExtractSectionNameValue(xDoc);
                    string stampAuthorValue = ExtractStampAuthorValue(xDoc);
                    string checkedByValue = ExtractCheckedByValue(xDoc);
                    string approvedByValue = ExtractApprovedByValue(xDoc);
                    string mfgApprovedByValue = ExtractMfgApprovedByValue(xDoc);
                    string rateOfInspectionValue = ExtractRateOfInspectionValue(xDoc);
                    string noteValue = ExtractNoteValue(xDoc);
                    string materialValue = ExtractMaterialValue(xDoc);
                    string fileType = FileType;

                    var modelData = new ModelData
                    {
                        FileNameProp = Path.GetFileName(modelPath),
                        FileType = fileType,
                        Designation = baseValue,
                        NameValue = nameValue,
                        StampAuthorValue = stampAuthorValue,
                        CheckedByValue = checkedByValue,
                        MfgApprovedByValue = mfgApprovedByValue,
                        RateOfInspectionValue = rateOfInspectionValue,
                        ApprovedByValue = approvedByValue,
                        MassValue = massValue,
                        MaterialValue = materialValue,
                        SectionNameValue = sectionNameValue,
                        PositionValue = positionValue,
                        NoteValue = noteValue,
                        PathModel = modelPath
                    };

                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        _mainWindow.AddModelData(modelData);
                    }));

                }
                else
                {
                    MessageBox.Show("Файл MetaProductInfo не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string ExtractMassValue(XDocument xDoc)
        {
            var massElement = xDoc.Descendants("property")
                .FirstOrDefault(e => (string)e.Attribute("id") == "mass");
            return massElement?.Attribute("value")?.Value ?? "Не указано";
        }

        private string ExtractNameValue(XDocument xDoc)
        {
            var nameElement = xDoc.Descendants("property")
                .FirstOrDefault(e => (string)e.Attribute("id") == "name");
            return nameElement?.Attribute("value")?.Value ?? "Не указано";
        }

        private string ExtractBaseValue(XDocument xDoc)
        {
            var baseElement = xDoc.Descendants("property")
                .FirstOrDefault(e => (string)e.Attribute("id") == "base");
            return baseElement?.Attribute("value")?.Value ?? "Не указано";
        }

        private string ExtractMaterialValue(XDocument xDoc)
        {
            var materialElement = xDoc.Descendants("property")
                .FirstOrDefault(e => (string)e.Attribute("id") == "material");

            if (materialElement != null)
            {
                var materialName = materialElement.Descendants("property")
                    .FirstOrDefault(p => (string)p.Attribute("id") == "name")?.Attribute("value")?.Value ?? "Не указано";

                return $"{materialName}";
            }

            return "Материал не указан";
        }

        private string ExtractPositionValue(XDocument xDoc)
        {
            var positionElement = xDoc.Descendants("property")
                .FirstOrDefault(e => (string)e.Attribute("id") == "position");
            return positionElement?.Attribute("value")?.Value ?? "Не указано";
        }

        private string ExtractSectionNameValue(XDocument xDoc)
        {
            var sectionElement = xDoc.Descendants("property")
                .FirstOrDefault(e => (string)e.Attribute("id") == "SPCSection");
            return sectionElement?.Descendants("property")
                .FirstOrDefault(p => (string)p.Attribute("id") == "sectionName")?.Attribute("value")?.Value ?? "Не указано";
        }

        private string ExtractStampAuthorValue(XDocument xDoc)
        {
            var stampAuthorElement = xDoc.Descendants("property")
                .FirstOrDefault(e => (string)e.Attribute("id") == "stampAuthor");
            return stampAuthorElement?.Attribute("value")?.Value ?? "Не указано";
        }

        private string ExtractCheckedByValue(XDocument xDoc)
        {
            var checkedByElement = xDoc.Descendants("property")
                .FirstOrDefault(e => (string)e.Attribute("id") == "checkedBy");
            return checkedByElement?.Attribute("value")?.Value ?? "Не указано";
        }

        private string ExtractApprovedByValue(XDocument xDoc)
        {
            var approvedByElement = xDoc.Descendants("property")
                .FirstOrDefault(e => (string)e.Attribute("id") == "approvedBy");
            return approvedByElement?.Attribute("value")?.Value ?? "Не указано";
        }

        private string ExtractMfgApprovedByValue(XDocument xDoc)
        {
            var mfgApprovedByElement = xDoc.Descendants("property")
                .FirstOrDefault(e => (string)e.Attribute("id") == "mfgApprovedBy");
            return mfgApprovedByElement?.Attribute("value")?.Value ?? "Не указано";
        }

        private string ExtractRateOfInspectionValue(XDocument xDoc)
        {
            var rateOfInspectionElement = xDoc.Descendants("property")
                .FirstOrDefault(e => (string)e.Attribute("id") == "rateOfInspection");
            return rateOfInspectionElement?.Attribute("value")?.Value ?? "Не указано";
        }

        private string ExtractNoteValue(XDocument xDoc)
        {
            var noteElement = xDoc.Descendants("property")
                .FirstOrDefault(e => (string)e.Attribute("id") == "note");
            return noteElement?.Attribute("value")?.Value ?? "Не указано";
        }
    }
}
