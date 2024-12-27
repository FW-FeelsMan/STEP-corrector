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
        private ObservableCollection<StepFile> _filesSTP = new ObservableCollection<StepFile>();
        private ObservableCollection<KompasModel> _kompasFiles = new ObservableCollection<KompasModel>();
        public ObservableCollection<KompasModelProp> KompasFilesProp { get; } = new ObservableCollection<KompasModelProp>();

        private Editor editor = new Editor();
        private EditorModel editorModel = new EditorModel();

        #region header
        public MainWindow()
        {
            InitializeComponent();

            FilesSTPListBox.ItemsSource = _filesSTP;
            ModelListBox.ItemsSource = _kompasFiles;
            DataGrid.ItemsSource = KompasFilesProp;

            STEPkompas.Visibility = Visibility.Hidden;
            STEPgrid.Visibility = Visibility.Visible;
            STEPprop.Visibility = Visibility.Hidden;
            progressBarWaiting.Visibility = Visibility.Hidden;
        }
        private void KMPSbuttonSidebar_Click(object sender, RoutedEventArgs e)
        {
            STEPgrid.Visibility = Visibility.Hidden;
            STEPkompas.Visibility = Visibility.Visible;
            STEPprop.Visibility = Visibility.Hidden;
        }

        private void STPbuttonSidebar_Click(object sender, RoutedEventArgs e)
        {
            STEPkompas.Visibility = Visibility.Hidden;
            STEPgrid.Visibility = Visibility.Visible;
            STEPprop.Visibility = Visibility.Hidden;
        }
        private void KMPSbuttonSidebarProps_Click(object sender, RoutedEventArgs e)
        {
            STEPkompas.Visibility = Visibility.Hidden;
            STEPgrid.Visibility = Visibility.Hidden;
            STEPprop.Visibility = Visibility.Visible;
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
        #endregion header

        #region STP
        private void ButtonLoadSTP_Click(object sender, RoutedEventArgs e)
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

                    if (_filesSTP.Any(f => f.FileName == fileName && f.FileExtension == fileExtension))
                    {
                        MessageBox.Show($"Файл {fileName}{fileExtension} уже добавлен.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        _filesSTP.Add(new StepFile
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

        private void ButtonSelectAllSTP_Click(object sender, RoutedEventArgs e)
        {
            foreach (var file in _filesSTP)
            {
                file.IsChecked = true;
            }
        }

        private void ButtonUnSelectSTP_Click(object sender, RoutedEventArgs e)
        {
            foreach (var file in _filesSTP)
            {
                file.IsChecked = false;
            }
        }

        private void ButtonClearSTPlist_Click(object sender, RoutedEventArgs e)
        {
            if (_filesSTP.Count == 0)
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
                _filesSTP.Clear();
                MessageBox.Show("Список файлов успешно очищен.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ButtonFixSTP_Click(object sender, RoutedEventArgs e)
        {
            if (_filesSTP.Count == 0)
            {
                MessageBox.Show("Отсутствуют файлы для редактирования. Пожалуйста, добавьте файлы.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var checkedFiles = _filesSTP.Where(file => file.IsChecked).ToList();

            if (checkedFiles.Count == 0)
            {
                MessageBox.Show("Отсутствуют отмеченные файлы для редактирования. Пожалуйста, выберите файлы.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show("Сделать резервные копии перед обработкой?", "Резервные копии", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                progressBarWaiting.Visibility = Visibility.Visible;

                var worker = new BackgroundWorker();
                worker.DoWork += (s, args) =>
                {
                    for (int i = 0; i < checkedFiles.Count; i++)
                    {
                        var file = checkedFiles[i];
                        CreateBackup(file.FilePath);

                        Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            // Обновление UI может быть здесь, если необходимо
                        }));
                    }
                };

                worker.RunWorkerCompleted += (s, args) =>
                {
                    progressBarWaiting.Visibility = Visibility.Hidden;
                    StartEditingFiles(checkedFiles);
                };

                worker.RunWorkerAsync();
            }
            else
            {
                StartEditingFiles(checkedFiles);
            }
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
        private void FilesSTPListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void ButtonExcludeSTP_Click(object sender, RoutedEventArgs e)
        {
            var checkedFiles = _filesSTP.Where(file => file.IsChecked).ToList();

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
                    _filesSTP.Remove(file);
                }

                MessageBox.Show("Отмеченные файлы успешно удалены.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private void StartEditingFiles(List<StepFile> checkedFiles)
        {
            progressBarWaiting.Visibility = Visibility.Visible;

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
                progressBarWaiting.Visibility = Visibility.Hidden; // Скрываем progressBarWaiting
                MessageBox.Show("Обработка завершена!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            };

            worker.RunWorkerAsync();
        }
        private void FilesSTPListBox_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy; 
            }
            else
            {
                e.Effects = DragDropEffects.None; 
            }
        }

        private void FilesSTPListBox_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach (var filePath in files)
                {
                    var fileName = Path.GetFileNameWithoutExtension(filePath);
                    var fileExtension = Path.GetExtension(filePath).ToLower(); 

                    if (fileExtension != ".stp" && fileExtension != ".step")
                    {
                        MessageBox.Show($"Файл {filePath} имеет недопустимое расширение. Пожалуйста, перетащите файлы с расширениями .stp или .step.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        continue; 
                    }
                    if (_filesSTP.Any(f => f.FileName == fileName && f.FileExtension == fileExtension))
                    {
                        MessageBox.Show($"Файл {fileName}{fileExtension} уже добавлен.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        _filesSTP.Add(new StepFile
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
        #endregion STP
        #region 3Dmodel                     
        private void ModelListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void ButtonFixModel_Click(object sender, RoutedEventArgs e)
        {
            var checkedModels = _kompasFiles.Where(model => model.IsChecked).ToList();

            if (checkedModels.Count == 0)
            {
                MessageBox.Show("Нет отмеченных моделей для редактирования.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show("Сделать резервные копии перед обработкой?", "Резервные копии", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                progressBarWaiting.Visibility = Visibility.Visible; // Показываем progressBarWaiting

                var worker = new BackgroundWorker();
                worker.DoWork += (s, args) =>
                {
                    for (int i = 0; i < checkedModels.Count; i++)
                    {
                        var model = checkedModels[i];
                        CreateBackup(model.FilePath); // Создаем резервную копию

                        // Обновление UI может быть здесь, если необходимо
                        Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            // Можно обновить прогресс, если нужно
                        }));
                    }
                };

                worker.RunWorkerCompleted += (s, args) =>
                {
                    progressBarWaiting.Visibility = Visibility.Hidden; // Скрываем progressBarWaiting
                    StartEditingModel(checkedModels); // Начинаем редактирование моделей
                };

                worker.RunWorkerAsync();
            }
            else
            {
                StartEditingModel(checkedModels); // Начинаем редактирование моделей без резервного копирования
            }
        }       
        private void GetAllModelButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var kompasFile in _kompasFiles)
            {
                kompasFile.IsChecked = true;
            }
        }
        private void UnselectAllModelButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var kompasFile in _kompasFiles)
            {
                kompasFile.IsChecked = false;
            }
        }

        private void ButtonClearListModel_Click(object sender, RoutedEventArgs e)
        {
            if (_kompasFiles.Count == 0)
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
                _kompasFiles.Clear();
                MessageBox.Show("Список файлов успешно очищен.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ButtonExcludeModel_Click(object sender, RoutedEventArgs e)
        {
            var checkedModel = _kompasFiles.Where(kompasFile => kompasFile.IsChecked).ToList();

            if (checkedModel.Count == 0)
            {
                MessageBox.Show("Нет отмеченных файлов для удаления.", "Информация", 
                MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show($"Вы уверены, что хотите удалить {checkedModel.Count} отмеченных файлов?",
                                         "Удаление файлов",
                                         MessageBoxButton.YesNo,
                                         MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                foreach (var kompasFile in checkedModel)
                {
                    _kompasFiles.Remove(kompasFile);
                }

                MessageBox.Show("Отмеченные файлы успешно удалены.", "Информация", 
                MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ButtonLoadKompas_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Выберите файл 3D модели Компас",
                Filter = "3D Model Files (*.a3d;*.m3d)|*.a3d;*.m3d|All Files (*.*)|*.*",
                Multiselect = true
            };

            if (openFileDialog.ShowDialog() == true)
            {
                foreach (var filePath in openFileDialog.FileNames)
                {
                    var fileName = Path.GetFileNameWithoutExtension(filePath);
                    var fileExtension = Path.GetExtension(filePath);

                    if (_kompasFiles.Any(f => f.FileName == fileName && f.FileExtension == fileExtension))
                    {
                        MessageBox.Show($"Файл {fileName}{fileExtension} уже добавлен.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        _kompasFiles.Add(new KompasModel
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
        private void StartEditingModel(List<KompasModel> checkedModels)
        {
            progressBarWaiting.Visibility = Visibility.Visible; // Показываем progressBarWaiting

            var worker = new BackgroundWorker();
            worker.DoWork += (s, args) =>
            {
                foreach (var kompasModel in checkedModels)
                {
                    try
                    {
                        editorModel.StartEditingModel(kompasModel.FilePath);
                    }
                    catch (Exception ex)
                    {
                        Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            MessageBox.Show($"Ошибка при обработке файла {kompasModel.FileName}{kompasModel.FileExtension}: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }));
                    }
                }
            };

            worker.RunWorkerCompleted += (s, args) =>
            {
                progressBarWaiting.Visibility = Visibility.Hidden; // Скрываем progressBarWaiting
                MessageBox.Show("Обработка завершена!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            };

            worker.RunWorkerAsync();
        }
        private void ModelListBox_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach (var filePath in files)
                {
                    var fileName = Path.GetFileNameWithoutExtension(filePath);
                    var fileExtension = Path.GetExtension(filePath).ToLower();

                    if (fileExtension != ".a3d" && fileExtension != ".m3d")
                    {
                        MessageBox.Show($"Файл {filePath} имеет недопустимое расширение. Пожалуйста, перетащите файлы с расширениями .a3d или .m3d.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        continue;
                    }

                    if (_kompasFiles.Any(m => m.FileName == fileName && m.FileExtension == fileExtension))
                    {
                        MessageBox.Show($"Файл {fileName}{fileExtension} уже добавлен.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        _kompasFiles.Add(new KompasModel
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
        private void ModelListBox_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        #endregion 3Dmodel

        #region 3DmodelProperties

        private void ButtonLoadKompasModelProp_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Выберите файл 3D модели Компас",
                Filter = "3D Model Files (*.a3d;*.m3d)|*.a3d;*.m3d|All Files (*.*)|*.*",
                Multiselect = true
            };

            if (openFileDialog.ShowDialog() == true)
            {
                foreach (var filePathProp in openFileDialog.FileNames)
                {
                    var fileNameProp = Path.GetFileNameWithoutExtension(filePathProp);
                    var fileExtensionProp = Path.GetExtension(filePathProp);

                    if (KompasFilesProp.Any(f => f.FilePathProp.Equals(filePathProp, StringComparison.OrdinalIgnoreCase)))
                    {
                        MessageBox.Show($"Файл {filePathProp} уже добавлен.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        KompasFilesProp.Add(new KompasModelProp
                        {
                            FileNameProp = fileNameProp,
                            FileExtensionProp = fileExtensionProp,
                            FilePathProp = filePathProp,
                            IsCheckedProp = false
                        });
                    }
                }
            }
        }
        private void DataGrid_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

        }
        private void DataGrid_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void DataGrid_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] filesProp = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach (var filePathProp in filesProp)
                {
                    var fileNameProp = Path.GetFileNameWithoutExtension(filePathProp);
                    var fileExtensionProp = Path.GetExtension(filePathProp).ToLower();

                    if (fileExtensionProp != ".a3d" && fileExtensionProp != ".m3d")
                    {
                        MessageBox.Show($"Файл {filePathProp} имеет недопустимое расширение. Пожалуйста, перетащите файлы с расширениями .a3d или .m3d.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        continue;
                    }

                    if (KompasFilesProp.Any(m => m.FileNameProp == fileNameProp && m.FileExtensionProp == fileExtensionProp))
                    {
                        MessageBox.Show($"Файл {fileNameProp}{fileExtensionProp} уже добавлен.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        KompasFilesProp.Add(new KompasModelProp
                        {
                            FileNameProp = fileNameProp,
                            FileExtensionProp = fileExtensionProp,
                            FilePathProp = filePathProp,
                            IsCheckedProp = false
                        });
                    }
                }
            }
        }
        #endregion 3DmodelProperties
    }
}
