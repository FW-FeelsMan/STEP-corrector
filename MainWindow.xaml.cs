using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace STEP_corrector
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<StepFile> _files = new ObservableCollection<StepFile>();
        private Editor editor = new Editor();

        public MainWindow()
        {
            InitializeComponent();
            FilesListBox.ItemsSource = _files;
        }

        private void ButtonQuit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void ButtonCollaps_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        private void ButtonAutocorrecrList_Click(object sender, RoutedEventArgs e)
        {
            string filePath = "AutoCorrect.txt"; 

            if (System.IO.File.Exists(filePath))
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = filePath,
                    UseShellExecute = true
                });
            }
            else
            {
                MessageBox.Show("Файл не найден. Проверьте путь к файлу.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Выберите файл STEP",
                Filter = "STEP Files (*.stp;*.step)|*.stp;*.step|All Files (*.*)|*.*",
                Multiselect = true
            };

            if (openFileDialog.ShowDialog() == true)
            {
                foreach (var filePath in openFileDialog.FileNames)
                {
                    var fileName = Path.GetFileNameWithoutExtension(filePath);
                    var fileExtension = Path.GetExtension(filePath);

                    if (_files.Any(f => f.FileName == fileName && f.FileExtension == fileExtension))
                    {
                        MessageBox.Show($"Файл {fileName}{fileExtension} уже добавлен.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        _files.Add(new StepFile
                        {
                            FileName = fileName,
                            FileExtension = fileExtension,
                            FilePath = filePath,
                            IsChecked = false
                        });
                    }
                }
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            foreach (var file in _files)
            {
                file.IsChecked = true;
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            foreach (var file in _files)
            {
                file.IsChecked = false;
            }
        }

        private void Button_Clear(object sender, RoutedEventArgs e)
        {
            if (_files.Count == 0)
            {
                MessageBox.Show("Список уже пуст.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show("Вы уверены, что хотите полностью очистить список файлов?",
                                         "Очистка списка",
                                         MessageBoxButton.YesNo,
                                         MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                _files.Clear();
                MessageBox.Show("Список файлов успешно очищен.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if (_files.Count == 0)
            {
                MessageBox.Show("Отсутствуют файлы для редактирования. Пожалуйста, добавьте файлы.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var checkedFiles = _files.Where(file => file.IsChecked).ToList();

            if (checkedFiles.Count == 0)
            {
                MessageBox.Show("Отсутствуют отмеченные файлы для редактирования. Пожалуйста, выберите файлы.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show("Сделать резервные копии перед обработкой?", "Резервные копии", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var progressWindow = new ProgressWindow(checkedFiles.Count)
                {
                    Owner = this
                };

                progressWindow.Show();

                var worker = new BackgroundWorker();
                worker.DoWork += (s, args) =>
                {
                    for (int i = 0; i < checkedFiles.Count; i++)
                    {
                        var file = checkedFiles[i];
                        CreateBackup(file.FilePath);

                        Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            progressWindow.UpdateProgress(i + 1);
                        }));
                }
                };

                worker.RunWorkerCompleted += (s, args) =>
                {
                    progressWindow.Close();
                    StartEditingFiles(checkedFiles);
                };

                worker.RunWorkerAsync();
            }
            else
            {
                StartEditingFiles(checkedFiles);
            }
        }

        private void StartEditingFiles(List<StepFile> checkedFiles)
        {
            var editorProgress = new EditorProgress
            {
                Owner = this
            };

            editorProgress.Show();

            var worker = new BackgroundWorker();
            worker.DoWork += (s, args) =>
            {
                foreach (var file in checkedFiles)
                {
                    try
                    {
                        editor.StartEditing(file.FilePath);
                    }
                    catch (Exception ex)
                    {
                        Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            MessageBox.Show($"Ошибка при обработке файла {file.FileName}{file.FileExtension}: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }));
                    }
                }
            };

            worker.RunWorkerCompleted += (s, args) =>
            {
                editorProgress.Close();
                MessageBox.Show("Обработка завершена!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            };

            worker.RunWorkerAsync();
        }

        private void CreateBackup(string filePath)
        {
            try
            {
                string backupPath = $"{filePath}.bak";
                File.Copy(filePath, backupPath, overwrite: true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании резервной копии для файла {filePath}: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FilesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_Exclude(object sender, RoutedEventArgs e)
        {
            var checkedFiles = _files.Where(file => file.IsChecked).ToList();

            if (checkedFiles.Count == 0)
            {
                MessageBox.Show("Нет отмеченных файлов для удаления.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show($"Вы уверены, что хотите удалить {checkedFiles.Count} отмеченных файлов?",
                                         "Удаление файлов",
                                         MessageBoxButton.YesNo,
                                         MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                foreach (var file in checkedFiles)
                {
                    _files.Remove(file);
                }

                MessageBox.Show("Отмеченные файлы успешно удалены.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
