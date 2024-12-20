using System.ComponentModel;

namespace STEP_corrector
{
    public class StepFile : INotifyPropertyChanged
    {
        private string _fileName;
        private string _fileExtension;
        private string _filePath;  
        private bool _isChecked;

        public string FileName
        {
            get { return _fileName; }
            set
            {
                if (_fileName != value)
                {
                    _fileName = value;
                    OnPropertyChanged(nameof(FileName));
                }
            }
        }

        public string FileExtension
        {
            get { return _fileExtension; }
            set
            {
                if (_fileExtension != value)
                {
                    _fileExtension = value;
                    OnPropertyChanged(nameof(FileExtension));
                }
            }
        }

        public string FilePath  
        {
            get { return _filePath; }
            set
            {
                if (_filePath != value)
                {
                    _filePath = value;
                    OnPropertyChanged(nameof(FilePath)); 
                }
            }
        }

        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                if (_isChecked != value)
                {
                    _isChecked = value;
                    OnPropertyChanged(nameof(IsChecked)); 
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
