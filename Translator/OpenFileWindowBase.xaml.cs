using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Translator
{
    /// <summary>
    /// Interaction logic for OpenFileWindowBase.xaml
    /// </summary>
    public abstract partial class OpenFileWindowBase : Window, INotifyPropertyChanged
    {
        protected OpenFileWindowBase(Window owner)
        {
            Owner = owner;
            

            Files = new ObservableCollection<FileDesc>();
            _upLabel = LocalString.Get("OpenFileWindowBase_Up");
            
            InitializeComponent();

            Loaded += (sender, args) =>
            {
                ShowPreview.IsChecked = false;

                LoadWorkDirs();

                // Начальная инициализация
                if (_lastPath != null)
                    _currentPath = _lastPath;
                else
                    _currentPath = ((workDirsItem != null) ? workDirsItem.Items[0] : FavouriteItem.Items[0]) as string;

                _parentPath = System.IO.Path.GetDirectoryName(_currentPath);

                DoOpenPath();

                // т.к. тут при инициализации установили переменные напрямую, надо оповестить всех об этом
                OnPropertyChanged("Path");  
            };
        }
        
        // Загрузить рабочие директории из проекта (если загружен)
        private void LoadWorkDirs()
        {
            if (CodeProjectManager.Instance.Current == null) return;

            // Load Work dirs from project
            workDirsItem = new TreeViewItem();

            workDirsItem.ItemTemplate = FavouriteItem.ItemTemplate;
            workDirsItem.ApplyTemplate();
            workDirsItem.Header = LocalString.Get("WorkDirs");
            workDirsItem.IsExpanded = true;
            workDirsItem.SetBinding(TreeViewItem.ItemsSourceProperty, 
                new Binding { Source = CodeProjectManager.Instance.Current.WorkDirs });

            treeView1.Items.Insert(0, workDirsItem);
        }

        protected abstract bool CheckTemplate(string filename);
        public abstract string ExtensionFilter { get; }
        protected virtual string GetPreview(string path)
        {
            return null;
        }

        protected virtual void OnSelectDirectory()
        {
            //_parentPath = Path;
        }

        protected virtual void OnSelectFile() { }

        public class FileDesc
        {
            public enum Type
            {
                File,
                Dir,
                UpDir,
                Drive
            };

            public string Name { get; set; }
            public string Path { get; set; }
            public Type FileType { get; set; }
            public DateTime? LastChanged { get; set; }
            public long? Size { get; set; }
        }

        public ObservableCollection<FileDesc> Files { get; private set; }
        private readonly string _upLabel;
        private string _parentPath = null;
        private static string _lastPath = null;
        private TreeViewItem workDirsItem = null;

        private string _currentPath;
        public string Path
        {
            get { return _currentPath; }
            set
            {
                _parentPath = _currentPath;
                _currentPath = value; 
                DoOpenPath();
                OnPropertyChanged("Path");
            }
        }

        private void DoOpenPath()
        {
            Files.Clear();

            if (File.Exists(Path))
            {
                // Если текущий путь - файл, то выбрали что хотели, закрываем
                OnSelectFile();
                DialogResult = true;
            }
            else if (Path == "")
            {
                // Страница с дисками
                DriveInfo[] drives = System.IO.DriveInfo.GetDrives();
                foreach (DriveInfo drive in drives)
                {
                    Files.Add(new FileDesc() 
                    { 
                        FileType = FileDesc.Type.Drive,
                        Name = (drive.IsReady && drive.VolumeLabel != null)? drive.VolumeLabel + " (" + drive.Name + ")" : drive.Name,
                        Path = drive.Name
                    });
                }
            }
            else if (!Directory.Exists(Path))
            {
                Path = _parentPath;
            }
            else
            {
                // Текущий путь - директории
                OnSelectDirectory();

                // Добавить вверх
                DirectoryInfo rootDir = new DirectoryInfo(Path);

                Files.Add(new FileDesc
                {
                    FileType = FileDesc.Type.UpDir,
                    Name = _upLabel,
                    Path = rootDir.Parent != null ? rootDir.Parent.FullName : ""
                });

                try
                {
                    // Открыть все в текущей папке
                    DirectoryInfo dr = new DirectoryInfo(Path);

                    // Директории
                    foreach (DirectoryInfo dir in dr.GetDirectories())
                        Files.Add(new FileDesc() { FileType = FileDesc.Type.Dir, 
                            Name = dir.Name, 
                            Path = dir.FullName,
                            LastChanged = dir.LastAccessTime});

                    // Файлы, соответствующие шаблону
                    foreach (FileInfo file in dr.GetFiles().Where(file => CheckTemplate(file.Name)))
                    {
                        Files.Add(new FileDesc 
                        {
                            FileType = FileDesc.Type.File, 
                            Name = file.Name, 
                            Path = file.FullName,
                            LastChanged = file.LastAccessTime,
                            Size = file.Length
                        });
                    }
                }
                catch
                {
                    // Если что-то поймали, скорее всего нет прав, просто ничего не добавляем и все
                }
            }
        }

        private void Item_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListViewItem s = sender as ListViewItem;
            FileDesc desc = s.Content as FileDesc;
            Path = desc.Path;
        }

        private void Item_ButtonDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ListViewItem s = sender as ListViewItem;
                FileDesc desc = s.Content as FileDesc;
                Path = desc.Path;

                e.Handled = true;   // Чтоб не отдавался ентер на дефолтную кнопку
            }
        }

        private void Item_Loaded(object sender, RoutedEventArgs e)
        {
            // Тут устанавливаем фокусы на только что созданные элементы
            var item = sender as ListViewItem;
            var file = item.Content as FileDesc;    // Только что созданный элемен

            // Фокус надо установить на тот файл, который по имени совпадает с предыдущим путем
            if (file.Path == _parentPath)
            {
                item.IsSelected = true;
                item.BringIntoView();
                Dispatcher.Invoke(new Action(() => item.Focus()));
            }
        }

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            FileDesc desc = listView1.SelectedItem as FileDesc;
            if (desc != null)
            {
                if (desc.FileType != FileDesc.Type.File)
                    //OpenPath(desc.Path);
                    Path = desc.Path;
                else
                {
                    OnSelectFile();
                    DialogResult = true;
                }
            }
        }

        private void FavoriteItem_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            string path = (string) treeView1.SelectedItem;
            Path = path;
        }

        private void FavoriteItem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string path = (string)treeView1.SelectedItem;
                Path = path;

                e.Handled = true; // Чтоб не отдавался ентер на дефолтную кнопку
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private void PathTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                (sender as TextBox).GetBindingExpression(TextBox.TextProperty).UpdateSource();
                e.Handled = true;   // Чтоб на кнопку ОК не ушел ентер
            }
        }

        private void Item_Selected(object sender, RoutedEventArgs e)
        {
            // Все это надо, только если стоит галочка показывать превью
            if (ShowPreview.IsChecked.HasValue && !ShowPreview.IsChecked.Value)
                return;

            ListViewItem item = sender as ListViewItem;
            FileDesc fileDesc = item.Content as FileDesc;

            if (fileDesc != null && fileDesc.FileType == FileDesc.Type.File)
            {
                string preview = GetPreview(fileDesc.Path);

                if (preview != null)
                {
                    PreviewTextBoxEmpty.Visibility = Visibility.Hidden;
                    PreviewTextBox.Visibility = Visibility.Visible;

                    PreviewTextBox.Text = preview;
                }
                else
                {
                    PreviewTextBoxEmpty.Visibility = Visibility.Visible;
                    PreviewTextBox.Visibility = Visibility.Hidden;
                }
            }
            else
            {
                PreviewTextBoxEmpty.Visibility = Visibility.Visible;
                PreviewTextBox.Visibility = Visibility.Hidden;
            }
        }

        private double? _saveWidth;
        private void ShowPreview_OnUnchecked(object sender, RoutedEventArgs e)
        {
            _saveWidth = PreviewColumn.ActualWidth;
            PreviewColumnSplitter.Width = new GridLength(0);
            PreviewColumn.Width = new GridLength(0);
            PreviewColumn.MinWidth = 0;
        }

        private void ShowPreview_OnChecked(object sender, RoutedEventArgs e)
        {
            if (_saveWidth.HasValue)
            {
                PreviewColumnSplitter.Width = new GridLength(5, GridUnitType.Pixel);
                PreviewColumn.Width = new GridLength(_saveWidth.Value, GridUnitType.Pixel);
                PreviewColumn.MinWidth = 100;
                
                _saveWidth = null;
            }
        }
    }
    
    class FileTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var type = (OpenFileWindowBase.FileDesc.Type)value;

            switch (type)
            {
                case OpenFileWindowBase.FileDesc.Type.File:
                    return LocalString.Get("OpenProjectWindow_ElementType_File");
                case OpenFileWindowBase.FileDesc.Type.Dir:
                    return LocalString.Get("OpenProjectWindow_ElementType_Dir");
                case OpenFileWindowBase.FileDesc.Type.Drive:
                    return LocalString.Get("OpenProjectWindow_ElementType_Drive");
                case OpenFileWindowBase.FileDesc.Type.UpDir:
                    return "";
                default:
                    return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    class DateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime? type = value as DateTime?;
            return type.HasValue ? type.Value.ToString("dd.MM.yyyy HH:ss") : "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    class SizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var size = value as long?;

            if (size.HasValue)
            {
                double _size = size.Value;
                if (_size <= 1024)
                    return _size.ToString() + " " + LocalString.Get("Byte_short");
                
                _size = _size / 1024;
                if (_size <= 1024)
                    return _size.ToString("F2") + " " + LocalString.Get("KB_short");

                _size = _size / 1024;
                if (_size <= 1024)
                    return _size.ToString("F2") + " " + LocalString.Get("MB_short");

                _size = _size / 1024;
                return _size.ToString("F2") + " " + LocalString.Get("GB_short");
            }
            else
                return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    class MinusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double v = (double) value;
            v -= 45.0;
            if (v < 1.0) v = 1.0;

            return v;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    class CheckedToVisibilittyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool? isChecked = value as bool?;

            if (isChecked.HasValue && isChecked.Value)
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
