using Ionic.Zip;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Xml.Linq;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;

using Application = System.Windows.Application;
using System.Windows.Media;

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

            FilesSTPListBox.ItemsSource = _filesSTP;
            ModelListBox.ItemsSource = _kompasFiles;

            STEPkompas.Visibility = Visibility.Hidden;
            STEPgrid.Visibility = Visibility.Visible;
            STEPprop.Visibility = Visibility.Hidden;
            stpView.Visibility = Visibility.Hidden;
            progressBarWaiting.Visibility = Visibility.Hidden;

            LabelPropertiesAmmountElemValue.Content = "0";
            CheckAndCreateErrorLog();

            DataGrid.ItemsSource = _modelDataCollection;
            DataGrid.SelectionChanged += DataGrid_SelectionChanged;

            editorModelProp = new EditorModelProp(this);

            try
            {
                RemoveDirectory();
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
            }

            

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
                }
            };

            worker.RunWorkerCompleted += (s, args) =>
            {
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
            stpView.Visibility = Visibility.Hidden;
        }
        private void STPbuttonViewSidebar_Click(object sender, RoutedEventArgs e)
        {
            STEPkompas.Visibility = Visibility.Hidden;
            STEPgrid.Visibility = Visibility.Hidden;
            STEPprop.Visibility = Visibility.Hidden;
            stpView.Visibility = Visibility.Visible;
        }

        private void STPbuttonSidebar_Click(object sender, RoutedEventArgs e)
        {
            STEPkompas.Visibility = Visibility.Hidden;
            STEPgrid.Visibility = Visibility.Visible;
            STEPprop.Visibility = Visibility.Hidden;
            stpView.Visibility = Visibility.Hidden;
        }
        private void KMPSbuttonSidebarProps_Click(object sender, RoutedEventArgs e)
        {
            STEPkompas.Visibility = Visibility.Hidden;
            STEPgrid.Visibility = Visibility.Hidden;
            STEPprop.Visibility = Visibility.Visible;
            stpView.Visibility = Visibility.Hidden;
        }

        private void ButtonQuit_Click(object sender, RoutedEventArgs e)
        {
            RemoveDirectory();
            System.Windows.Application.Current.Shutdown();
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

            foreach (var file in checkedFiles)
            {
                if (FileChecker.IsFileLocked(file.FilePath))
                {
                    MessageBox.Show($"Файл {file.FileName}{file.FileExtension} занят другой программой или недоступен для записи.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return; // Прерываем выполнение, если файл занят
                }
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

                if (File.Exists(filePath))
                {
                    File.Copy(filePath, backupPath, overwrite: true);
                }
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
                    if (FileChecker.IsFileLocked(file.FilePath))
                    {
                        Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            MessageBox.Show($"Файл {file.FileName}{file.FileExtension} занят другой программой или недоступен для записи.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }));
                        continue; // Пропускаем файл, если он занят
                    }

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
                        if (FileChecker.IsFileLocked(filePath))
                        {
                            MessageBox.Show($"Файл {fileName}{fileExtension} занят другой программой или недоступен для записи.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            continue; // Пропускаем добавление, если файл занят
                        }

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

            foreach (var model in checkedModels)
            {
                if (FileChecker.IsFileLocked(model.FilePath))
                {
                    MessageBox.Show($"Файл {model.FileName}{model.FileExtension} занят другой программой или недоступен для записи.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return; // Прерываем выполнение, если файл занят
                }
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
                        if (FileChecker.IsFileLocked(filePath))
                        {
                            MessageBox.Show($"Файл {fileName}{fileExtension} занят другой программой или недоступен для записи.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            continue; // Пропускаем добавление, если файл занят
                        }

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

                var filePaths = openFileDialog.FileNames.ToList();

                foreach (var filePath in filePaths)
                {
                    var fileName = Path.GetFileNameWithoutExtension(filePath);
                    var fileExtension = Path.GetExtension(filePath);

                    if (_kompasFiles.Any(f => f.FileName == fileName && f.FileExtension == fileExtension))
                    {
                        MessageBox.Show($"Файл {fileName}{fileExtension} уже добавлен.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        try
                        {
                            editorModelProp.StartEditingModel(filePath);
                            _kompasFiles.Add(new KompasModel
                            {
                                FileName = fileName,
                                FileExtension = fileExtension,
                                FilePath = filePath,
                                IsChecked = false
                            });
                        }
                        catch (Exception ex)
                        {
                            LogError(ex.Message);
                        }
                    }
                }

                progressBarWaiting.Visibility = Visibility.Hidden;
            }
        }

        private void RemoveModelPropButton_Click(object sender, RoutedEventArgs e)
        {
            var itemsToRemove = _modelDataCollection.Where(item => item.IsSelected).ToList();

            if (itemsToRemove.Count == 0)
            {
                MessageBox.Show("Нет отмеченных элементов для удаления.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            foreach (var item in itemsToRemove)
            {
                _modelDataCollection.Remove(item);
            }

            UpdateLabelPropertiesCount();
        }

        private void UpdateModelPropButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateLabelPropertiesCount();
        }
        private void UpdateLabelPropertiesCount()
        {
            LabelPropertiesAmmountElemValue.Content = _modelDataCollection.Count.ToString();
        }
        private void InputFieldValueProp_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(InputFieldValueProp.Text))
            {
                LabelHint.Visibility = Visibility.Visible;
            }
            else
            {
                LabelHint.Visibility = Visibility.Collapsed;
            }
        }

        

        private void RemoveDataCellPropButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedCells = DataGrid.SelectedCells;

            if (selectedCells.Count == 0)
            {
                MessageBox.Show("Нет отмеченных ячеек для удаления.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            foreach (var cell in selectedCells)
            {
                var item = cell.Item as ModelData;

                if (item != null)
                {
                    var boundColumn = cell.Column as DataGridBoundColumn;
                    if (boundColumn != null)
                    {
                        var binding = boundColumn.Binding as Binding;
                        if (binding != null)
                        {
                            if (binding.Mode == BindingMode.OneTime)
                            {
                                continue;
                            }

                            var propertyName = binding.Path.Path; 
                            
                            var propertyInfo = typeof(ModelData).GetProperty(propertyName);
                            if (propertyInfo != null && propertyInfo.CanWrite)
                            {
                                propertyInfo.SetValue(item, null, null); 
                            }
                        }
                    }
                }
            }

            DataGrid.Items.Refresh();
            UpdateLabelPropertiesCount();
        }
        private void ClearGridModelPropButton_Click(object sender, RoutedEventArgs e)
        {
            if (_modelDataCollection.Count == 0)
            {
                MessageBox.Show("Таблица уже пуста.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show("Вы уверены, что хотите очистить таблицу?", "Очистка таблицы", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                _modelDataCollection.Clear(); 
                UpdateLabelPropertiesCount(); 
                RemoveDirectory();
            }
        }


        private void AddNewModelPropButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedCells = DataGrid.SelectedCells;

            if (selectedCells.Count == 0)
            {
                MessageBox.Show("Нет отмеченных ячеек для изменения.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var newValue = InputFieldValueProp.Text;

            foreach (var cell in selectedCells)
            {
                var item = cell.Item as ModelData;

                if (item != null)
                {
                    var boundColumn = cell.Column as DataGridBoundColumn;
                    if (boundColumn != null)
                    {
                        var binding = boundColumn.Binding as Binding;
                        if (binding != null)
                        {
                            if (binding.Mode == BindingMode.OneTime)
                            {
                                continue;
                            }

                            var propertyName = binding.Path.Path; 

                            var propertyInfo = typeof(ModelData).GetProperty(propertyName);
                            if (propertyInfo != null && propertyInfo.CanWrite)
                            {
                                var convertedValue = Convert.ChangeType(newValue, propertyInfo.PropertyType);
                                propertyInfo.SetValue(item, convertedValue, null); 
                            }
                        }
                    }
                }
            }

            DataGrid.Items.Refresh();
            UpdateLabelPropertiesCount();
        }

        private void ApplyNewModelPropButton_Click(object sender, RoutedEventArgs e)
        {
            var updatedModels = _modelDataCollection.Where(m => m.IsSelected).ToList();
            if (updatedModels.Count == 0)
            {
                MessageBox.Show("Нет выбранных моделей для обновления.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            MessageBoxResult result = MessageBox.Show("Хотите ли вы создать резервную копию файла перед применением изменений?",
                                                          "Создание резервной копии",
                                                          MessageBoxButton.YesNo,
                                                          MessageBoxImage.Question);

            bool createBackup = result == MessageBoxResult.Yes;
            bool allUpdatesSuccessful = true; 
            List<string> errorModels = new List<string>(); 

            foreach (var model in updatedModels)
            {
                try
                {
                    string tempDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "STEP_corrector_Temp_Prop");
                    string extractedDir = Path.Combine(tempDir, Path.GetFileNameWithoutExtension(model.PathModel));
                    string metaFilePath = Path.Combine(extractedDir, "MetaProductInfo");

                    if (createBackup)
                    {
                        CreateBackup(model.PathModel); // Резервная копия создается по исходному пути
                    }

                    if (File.Exists(metaFilePath))
                    {
                        var xDoc = XDocument.Load(metaFilePath);
                        UpdateMetaProductInfo(xDoc, model);
                        SaveXmlWithEncoding(metaFilePath, xDoc, Encoding.BigEndianUnicode);
                    }

                    string newArchivePath = Path.ChangeExtension(model.PathModel, Path.GetExtension(model.PathModel));
                    using (ZipFile zip = new ZipFile())
                    {
                        zip.CompressionLevel = Ionic.Zlib.CompressionLevel.None;
                        zip.AddDirectory(extractedDir);
                        zip.Save(newArchivePath);
                    }

                    Directory.Delete(extractedDir, true);
                }
                catch (Exception ex)
                {
                    allUpdatesSuccessful = false; // Устанавливаем флаг в false, если произошла ошибка
                    errorModels.Add(model.FileNameProp); // Добавляем модель с ошибкой в список
                    MessageBox.Show($"Ошибка при обновлении модели {model.FileNameProp}: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            
            if (allUpdatesSuccessful)
            {
                MessageBox.Show("Изменения успешно применены!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                string errorMessage = "Изменения применены, но возникли ошибки для следующих моделей: " + string.Join(", ", errorModels);
                MessageBox.Show(errorMessage, "Информация", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void SaveXmlWithEncoding(string filePath, XDocument xDoc, Encoding encoding)
        {
            using (var writer = new StreamWriter(filePath, false, encoding))
            {
                xDoc.Save(writer);
            }
        }
        private void UpdateMetaProductInfo(XDocument xDoc, ModelData model)
        {
            var infObjects = xDoc.Descendants("infObject").ToList();

            foreach (var infObject in infObjects)
            {
                var designationElement = infObject.Descendants("property").FirstOrDefault(e => (string)e.Attribute("id") == "base");
                if (designationElement != null && !string.IsNullOrEmpty(model.Designation) && model.Designation != "Не указано")
                {
                    designationElement.SetAttributeValue("value", model.Designation);
                }
                var nameElement = infObject.Descendants("property").FirstOrDefault(e => (string)e.Attribute("id") == "name");
                if (nameElement != null && !string.IsNullOrEmpty(model.NameValue) && model.NameValue != "Не указано")
                {
                    nameElement.SetAttributeValue("value", model.NameValue);
                }

                // Обновляем "Разработал"
                var stampAuthorElement = infObject.Descendants("property").FirstOrDefault(e => (string)e.Attribute("id") == "stampAuthor");
                if (stampAuthorElement != null && !string.IsNullOrEmpty(model.StampAuthorValue) && model.StampAuthorValue != "Не указано")
                {
                    stampAuthorElement.SetAttributeValue("value", model.StampAuthorValue);
                }

                // Обновляем "Проверил"
                var checkedByElement = infObject.Descendants("property").FirstOrDefault(e => (string)e.Attribute("id") == "checkedBy");
                if (checkedByElement != null && !string.IsNullOrEmpty(model.CheckedByValue) && model.CheckedByValue != "Не указано")
                {
                    checkedByElement.SetAttributeValue("value", model.CheckedByValue);
                }

                // Обновляем "Т.контр."
                var mfgApprovedByElement = infObject.Descendants("property").FirstOrDefault(e => (string)e.Attribute("id") == "mfgApprovedBy");
                if (mfgApprovedByElement != null && !string.IsNullOrEmpty(model.MfgApprovedByValue) && model.MfgApprovedByValue != "Не указано")
                {
                    mfgApprovedByElement.SetAttributeValue("value", model.MfgApprovedByValue);
                }

                // Обновляем "Н.контр."
                var rateOfInspectionElement = infObject.Descendants("property").FirstOrDefault(e => (string)e.Attribute("id") == "rateOfInspection");
                if (rateOfInspectionElement != null && !string.IsNullOrEmpty(model.RateOfInspectionValue) && model.RateOfInspectionValue != "Не указано")
                {
                    rateOfInspectionElement.SetAttributeValue("value", model.RateOfInspectionValue);
                }

                // Обновляем "Утвердил"
                var approvedByElement = infObject.Descendants("property").FirstOrDefault(e => (string)e.Attribute("id") == "approvedBy");
                if (approvedByElement != null && !string.IsNullOrEmpty(model.ApprovedByValue) && model.ApprovedByValue != "Не указано")
                {
                    approvedByElement.SetAttributeValue("value", model.ApprovedByValue);
                }

                // Обновляем материал (специальная обработка для вложенных элементов)
                var materialElement = infObject.Descendants("property").FirstOrDefault(e => (string)e.Attribute("id") == "material");
                if (materialElement != null)
                {
                    var materialNameElement = materialElement.Descendants("property").FirstOrDefault(e => (string)e.Attribute("id") == "name");
                    if (materialNameElement != null && !string.IsNullOrEmpty(model.MaterialValue) && model.MaterialValue != "Не указано")
                    {
                        materialNameElement.SetAttributeValue("value", model.MaterialValue);
                    }
                }

                // Обновляем раздел спецификации
                var sectionNameElement = infObject.Descendants("property").FirstOrDefault(e => (string)e.Attribute("id") == "SPCSection");
                if (sectionNameElement != null && !string.IsNullOrEmpty(model.SectionNameValue) && model.SectionNameValue != "Не указано")
                {
                    sectionNameElement.SetAttributeValue("value", model.SectionNameValue);
                }

                // Обновляем позицию
                var positionElement = infObject.Descendants("property").FirstOrDefault(e => (string)e.Attribute("id") == "position");
                if (positionElement != null && !string.IsNullOrEmpty(model.PositionValue) && model.PositionValue != "Не указано")
                {
                    positionElement.SetAttributeValue("value", model.PositionValue);
                }

                // Обновляем примечание
                var noteElement = infObject.Descendants("property").FirstOrDefault(e => (string)e.Attribute("id") == "note");
                if (noteElement != null && !string.IsNullOrEmpty(model.NoteValue) && model.NoteValue != "Не указано")
                {
                    noteElement.SetAttributeValue("value", model.NoteValue);
                }

                // Если какие-то из этих свойств не были найдены, добавляем их в нужное место
                AddElementIfNotExists(infObject, "stampAuthor", model.StampAuthorValue);
                AddElementIfNotExists(infObject, "checkedBy", model.CheckedByValue);
                AddElementIfNotExists(infObject, "mfgApprovedBy", model.MfgApprovedByValue);
                AddElementIfNotExists(infObject, "rateOfInspection", model.RateOfInspectionValue);
                AddElementIfNotExists(infObject, "approvedBy", model.ApprovedByValue);
                AddElementIfNotExists(infObject, "material", model.MaterialValue);
                AddElementIfNotExists(infObject, "SPCSection", model.SectionNameValue);
                AddElementIfNotExists(infObject, "position", model.PositionValue);
                AddElementIfNotExists(infObject, "note", model.NoteValue);
            }
        }

        private void AddElementIfNotExists(XElement infObject, string id, string value)
        {
            var existingElement = infObject.Descendants("property").FirstOrDefault(e => (string)e.Attribute("id") == id);

            if (existingElement == null && !string.IsNullOrEmpty(value) && value != "Не указано")
            {
                var newElement = new XElement("property",
                                              new XAttribute("id", id),
                                              new XAttribute("value", value));
                infObject.Add(newElement); 
            }
        }

        private void GetAllModelPropButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var model in _modelDataCollection)
            {
                model.IsSelected = true;
            }

            DataGrid.Items.Refresh();
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }
        private void DataGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var hit = e.OriginalSource as DependencyObject;
            while (hit != null && !(hit is DataGridCell))
            {
                hit = VisualTreeHelper.GetParent(hit);
            }

            if (hit is DataGridCell cell)
            {
                if (cell.Column is DataGridTextColumn textColumn && textColumn.Header.ToString() == "Имя файла")
                {
                    var row = DataGrid.ItemContainerGenerator.ContainerFromItem(cell.DataContext) as DataGridRow;
                    if (row != null)
                    {
                        var selectedItem = row.Item as ModelData;

                        if (selectedItem != null)
                        {
                            string filePath = selectedItem.PathModel; 
                            ShowPreview(filePath);
                        }
                    }
                }
            }
        }
        private void ShowPreview(string filePath)
        {
            try
            {
                var thumbnail = GetLargeThumbnail(filePath);
                if (thumbnail != null)
                {
                    PreviewImage.Source = thumbnail;
                }
                else
                {
                    MessageBox.Show("Не удалось получить превью для данного файла.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private BitmapSource GetLargeThumbnail(string filePath)
        {
            IntPtr hBitmap = IntPtr.Zero;
            try
            {
                var shellItem = SHCreateItemFromParsingName(filePath, IntPtr.Zero, typeof(IShellItemImageFactory).GUID, out var factory);
                if (shellItem == 0 && factory != null)
                {
                    var size = new SIZE { cx = 1024, cy = 1024 };
                    var options = SIIGBF.SIIGBF_BIGGERSIZEOK;
                    factory.GetImage(size, (int)options, out hBitmap);

                    if (hBitmap != IntPtr.Zero)
                    {
                        var bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                            hBitmap,
                            IntPtr.Zero,
                            Int32Rect.Empty,
                            BitmapSizeOptions.FromEmptyOptions());

                        return bitmapSource;
                    }
                }
            }
            finally
            {
                if (hBitmap != IntPtr.Zero)
                {
                    DeleteObject(hBitmap);
                }
            }
            return null;
        }

        [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int SHCreateItemFromParsingName(
            [MarshalAs(UnmanagedType.LPWStr)] string pszPath,
            IntPtr pbc,
            [MarshalAs(UnmanagedType.LPStruct)] Guid riid,
            [MarshalAs(UnmanagedType.Interface)] out IShellItemImageFactory ppv);

        [DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);

        [StructLayout(LayoutKind.Sequential)]
        private struct SIZE
        {
            public int cx;
            public int cy;
        }

        [ComImport]
        [Guid("bcc18b79-ba16-442f-80c4-8a59c30c463b")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IShellItemImageFactory
        {
            [PreserveSig]
            int GetImage(SIZE size, int flags, out IntPtr phbm);
        }

        private enum SIIGBF
        {
            SIIGBF_RESIZETOFIT = 0x00,
            SIIGBF_BIGGERSIZEOK = 0x01,
            SIIGBF_MEMORYONLY = 0x02,
            SIIGBF_ICONONLY = 0x04,
            SIIGBF_THUMBNAILONLY = 0x08,
            SIIGBF_INCACHEONLY = 0x10
        }
        #endregion 3DmodelProperties

        
    }
}
