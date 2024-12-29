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

        private ObservableCollection<ModelData> _modelDataCollection = new ObservableCollection<ModelData>();

        private Editor editor = new Editor();
        private EditorModel editorModel = new EditorModel();

        private const string ErrorLogFileName = "errorLog.log";

        private EditorModelProp editorModelProp;


        #region header
        public MainWindow()
        {
            InitializeComponent();
            RemoveDirectory();

            FilesSTPListBox.ItemsSource = _filesSTP;
            ModelListBox.ItemsSource = _kompasFiles;

            STEPkompas.Visibility = Visibility.Hidden;
            STEPgrid.Visibility = Visibility.Visible;
            STEPprop.Visibility = Visibility.Hidden;
            progressBarWaiting.Visibility = Visibility.Hidden;

            LabelPropertiesAmmountElemValue.Content = "0";
            CheckAndCreateErrorLog();

            DataGrid.ItemsSource = _modelDataCollection;

            editorModelProp = new EditorModelProp(this);
        }

        public void AddModelData(ModelData modelData)
        {
            if (modelData != null)
            {
                _modelDataCollection.Add(modelData);
                UpdateLabelPropertiesCount();
            }
        }

        private void CheckAndCreateErrorLog()
        {
            string logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ErrorLogFileName);
            
            if (!File.Exists(logFilePath))
            {
                File.Create(logFilePath).Dispose();
            }
        }
        public static void LogError(string message)
        {
            string logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ErrorLogFileName);
            
            using (StreamWriter writer = new StreamWriter(logFilePath, true))
            {
                writer.WriteLine($"{DateTime.Now}: {message}");
            }
        }

        private void RemoveDirectory()
        {
            string tempDirProp = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "STEP_corrector_Temp_Prop");
            string tempDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "STEP_corrector_Temp");

            progressBarWaiting.Visibility = Visibility.Visible;

            var worker = new BackgroundWorker();
            worker.DoWork += (s, args) =>
            {
                try
                {
                    if (Directory.Exists(tempDir))
                    {
                        Directory.Delete(tempDir, true);
                    }
                    if (Directory.Exists(tempDirProp))
                    {
                        Directory.Delete(tempDirProp, true);
                    }
                }
                catch (Exception ex)
                {
                    args.Result = ex;
                    LogError(ex.Message);
                }
            };

            worker.RunWorkerCompleted += (s, args) =>
            {
                progressBarWaiting.Visibility = Visibility.Collapsed;

                if (args.Result is Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении папок: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    LogError(ex.Message);
                }
            };

            worker.RunWorkerAsync();
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
                progressBarWaiting.Visibility = Visibility.Hidden;
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
                progressBarWaiting.Visibility = Visibility.Visible;

                var worker = new BackgroundWorker();
                worker.DoWork += (s, args) =>
                {
                    for (int i = 0; i < checkedModels.Count; i++)
                    {
                        var model = checkedModels[i];
                        CreateBackup(model.FilePath);

                        // Обновление UI может быть здесь, если необходимо
                        Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            // Можно обновить прогресс, если нужно
                        }));
                    }
                };

                worker.RunWorkerCompleted += (s, args) =>
                {
                    progressBarWaiting.Visibility = Visibility.Hidden;
                    StartEditingModel(checkedModels);
                };

                worker.RunWorkerAsync();
            }
            else
            {
                StartEditingModel(checkedModels);
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
                progressBarWaiting.Visibility = Visibility.Visible;

                foreach (var filePathProp in openFileDialog.FileNames)
                {
                    editorModelProp.StartEditingModel(filePathProp);
                }

                progressBarWaiting.Visibility = Visibility.Hidden;
            }
        }
        private void DataGrid_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

        }

        private void DataGridRemove_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                //RemoveModelPropButton_Click(sender, e);
                // Убедитесь, что вы не обновляете DataGrid здесь, если это не требуется
                // RefreshDataGrid(); // Удалите или закомментируйте эту строку
            }
        }

        private void RefreshDataGrid()
        {
            DataGrid.ItemsSource = null;
            //DataGrid.ItemsSource = KompasFilesProp;
        }
        private void DataGrid_Drop(object sender, DragEventArgs e)
        {
            // Проверяем, есть ли в перетаскиваемых данных файлы
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Получаем массив файлов из перетаскиваемых данных
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach (var filePath in files)
                {
                    // Проверяем, что файл существует
                    if (File.Exists(filePath))
                    {
                        // Создаем объект ModelData из файла
                        var modelData = CreateModelDataFromFile(filePath);

                        if (modelData != null)
                        {
                            // Добавляем созданный объект в коллекцию и обновляем счетчик
                            AddModelData(modelData);
                        }
                        else
                        {
                            MessageBox.Show($"Не удалось создать ModelData для файла: {filePath}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show($"Файл не найден: {filePath}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void RemoveModelPropButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItems = DataGrid.SelectedItems.Cast<ModelData>().ToList();
            foreach (var item in selectedItems)
            {
                _modelDataCollection.Remove(item);
            }
            UpdateLabelPropertiesCount(); // Обновление количества элементов
        }

        private void UpdateModelPropButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateLabelPropertiesCount();
        }
        private void UpdateLabelPropertiesCount()
        {
            LabelPropertiesAmmountElemValue.Content = _modelDataCollection.Count.ToString();
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

        #endregion 3DmodelProperties

    }
}
