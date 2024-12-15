using System;
using System.ComponentModel;
using System.Windows;

namespace STEP_corrector
{
    public partial class ProgressWindow : Window, INotifyPropertyChanged
    {
        private int _processedFiles;
        private int _totalFiles;
        private double _progressValue;
        private Visibility _continueButtonVisibility = Visibility.Collapsed;

        public string ProcessedFilesText => $"{_processedFiles} из {_totalFiles} файлов обработано";

        public double ProgressValue
        {
            get => _progressValue;
            set
            {
                if (_progressValue != value)
                {
                    _progressValue = value;
                    OnPropertyChanged(nameof(ProgressValue));
                }
            }
        }

        public Visibility ContinueButtonVisibility
        {
            get => _continueButtonVisibility;
            set
            {
                if (_continueButtonVisibility != value)
                {
                    _continueButtonVisibility = value;
                    OnPropertyChanged(nameof(ContinueButtonVisibility));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ProgressWindow(int totalFiles)
        {
            InitializeComponent();
            _totalFiles = totalFiles;
            DataContext = this; 
        }

        public void UpdateProgress(int processedFiles)
        {
            _processedFiles = processedFiles;
            ProgressValue = (double)processedFiles / _totalFiles * 100;
            OnPropertyChanged(nameof(ProcessedFilesText));

            if (processedFiles == _totalFiles)
            {
                ContinueButtonVisibility = Visibility.Visible; 
            }
        }

        private void Button_Continue(object sender, RoutedEventArgs e)
        {
            this.Close(); 
        }

    }
}
