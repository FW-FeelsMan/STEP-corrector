using System.ComponentModel;

namespace STEP_corrector
{
    public class KompasModelProp : INotifyPropertyChanged
    {
        private string _fileNameProp;
        private string _fileExtensionProp;
        private string _filePathProp;
        private bool _isCheckedProp;

        public string FullFileName => $"{_fileNameProp}{_fileExtensionProp}";

        public string FileNameProp
        {
            get { return _fileNameProp; }
            set
            {
                if (_fileNameProp != value)
                {
                    _fileNameProp = value;
                    OnPropertyChanged(nameof(FileNameProp));
                    OnPropertyChanged(nameof(FullFileName)); // Уведомляем об изменении
                }
            }
        }

        public string FileExtensionProp
        {
            get { return _fileExtensionProp; }
            set
            {
                if (_fileExtensionProp != value)
                {
                    _fileExtensionProp = value;
                    OnPropertyChanged(nameof(FileExtensionProp));
                    OnPropertyChanged(nameof(FullFileName)); // Уведомляем об изменении
                }
            }
        }

        public string FilePathProp
        {
            get { return _filePathProp; }
            set
            {
                if (_filePathProp != value)
                {
                    _filePathProp = value;
                    OnPropertyChanged(nameof(FilePathProp));
                }
            }
        }

        public bool IsCheckedProp
        {
            get { return _isCheckedProp; }
            set
            {
                if (_isCheckedProp != value)
                {
                    _isCheckedProp = value;
                    OnPropertyChanged(nameof(IsCheckedProp));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}