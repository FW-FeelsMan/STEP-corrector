using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace STEP_corrector
{
    public class ModelData
    {
        public string FileNameProp { get; set; } // Имя файла
        public string FileType { get; set; } // Тип
        public string Designation { get; set; } // Обозначение
        public string NameValue { get; set; } // Наименование
        public string StampAuthorValue { get; set; } // Разработал
        public string CheckedByValue { get; set; } // Проверил
        public string MfgApprovedByValue { get; set; } // Т.контр.
        public string RateOfInspectionValue { get; set; } // Н.контр.
        public string ApprovedByValue { get; set; } // Утвердил
        public string MassValue { get; set; } // Масса
        public string MaterialValue { get; set; } // Материал
        public string SectionNameValue { get; set; } // Раздел спецификации
        public string PositionValue { get; set; } // Позиция
        public string NoteValue { get; set; } // Примечание
        public string PathModel { get; set; } // Путь к файлу

        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
