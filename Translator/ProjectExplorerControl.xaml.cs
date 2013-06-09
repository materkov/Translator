using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
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
using Translator.Annotations;

namespace Translator
{
    /// <summary>
    /// Interaction logic for ProjectExplorerControl.xaml
    /// </summary>
    public partial class ProjectExplorerControl : UserControl, ProjectExplorerView, INotifyPropertyChanged
    {
        private CodeEditorView _codeEditorView;

        public ProjectExplorerControl()
        {
            InitializeComponent();

            // Дщфв templates
            ItemTemplate = FindResource("Item") as DataTemplate;
            EditableItemTemplate = FindResource("EditableItem") as DataTemplate;
            CurrentEditorItemTemplate = FindResource("CurrentEditorItemTemplate") as DataTemplate;
            OpenInEditorItemTemplate = FindResource("OpenInEditorItemTemplate") as DataTemplate;
        }

        // Для панельки 
        private FrameworkElement _mainObject;
        public FrameworkElement MainObject
        {
            get { return _mainObject; }
            set
            {
                _mainObject = value;
                OnPropertyChanged("MainObject");
            }
        }

        public void Init(CodeEditorView codeEditorView)
        {
            Background = new SolidColorBrush();

            _codeEditorView = codeEditorView;

            _codeEditorView.ActiveFileChanged += (sender, args) => UpdateSelections();
            _codeEditorView.FileOpened += (sender, args) => UpdateSelections();
            _codeEditorView.FileClosed += (sender, args) => UpdateSelections();
            
            CodeProjectManager.Instance.CurrentProjectChanged += CurrentProjectChanged;

            UpdateTree();

            // Для сортировки
            parent.Items.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
        }

        public void UpdateTree()
        {
            Debug.Assert(parent != null, "parent != null");

            if (CodeProjectManager.Instance.Current != null)
            {
                parent.Visibility = Visibility.Visible;
                parent.DataContext = CodeProjectManager.Instance.Current.Files;
                parent.GetBindingExpression(TreeViewItem.HeaderProperty).UpdateTarget();    // Update name

                TreeViewItem item = (TreeViewItem)(treeView1.ItemContainerGenerator.ContainerFromIndex(0));
                MainObject = item;
            }
            else
            {
                parent.Visibility = Visibility.Hidden;
                parent.DataContext = null;

                MainObject = treeView1;
            }

            parent.Items.Refresh(); // To enable sorting
            UpdateSelections();
        }

        public CodeProject Current { get; private set; } // Для обновления имени

        private void CurrentProjectChanged(object sender, CurrentProjectChangedArgs propertyChangedEventChangedArgs)
        {
            // Отписаться от предыдущего
            if (propertyChangedEventChangedArgs.Old != null)
                propertyChangedEventChangedArgs.Old.Files.CollectionChanged -= ProjectChanged;

            Current = propertyChangedEventChangedArgs.New;

            // Подписаться на новый
            if (propertyChangedEventChangedArgs.New != null)
                propertyChangedEventChangedArgs.New.Files.CollectionChanged += ProjectChanged;

            UpdateTree();
        }

        private void ProjectChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            UpdateTree();
        }

        // ----------------------------------------------------------------------------------------
        public void FocusOnFile(CodeFile file)
        {
            foreach (CodeFile treeFile in parent.Items.Cast<CodeFile>().Where(treeFile => treeFile == file))
            {
                TreeViewItem item = parent.ItemContainerGenerator.ContainerFromItem(file) as TreeViewItem;
                item.IsSelected = true;
                item.Focus();
            }
        }

        private void ApplyTemplate(CodeFile file, DataTemplate template)
        {
            TreeViewItem item = parent.ItemContainerGenerator.ContainerFromItem(file) as TreeViewItem;
            if (item != null)
            {
                item.HeaderTemplate = template;
                item.ApplyTemplate();
            }
        }

        // Обновляет выделения (текущий активный файл и открытые файлы)
        private void UpdateSelections()
        {
            if (Current == null) return;

            foreach (CodeFile file in Current.Files)
            {
                TreeViewItem item = parent.ItemContainerGenerator.ContainerFromItem(file) as TreeViewItem;
                if (item != null)
                {
                    if (_codeEditorView.CurrentCodeFile == file)
                        item.HeaderTemplate = CurrentEditorItemTemplate;
                    else if (_codeEditorView.OpenedFiles.Count(f => f == file) > 0)
                        item.HeaderTemplate = OpenInEditorItemTemplate;
                    else
                        item.HeaderTemplate = ItemTemplate;

                    item.ApplyTemplate();
                }
            }

            // Если нет ни одного активного файла, перетащить фокус на себя!
            if (_codeEditorView.OpenedFiles.Count() == 0)
            {
                if (treeView1.SelectedItem == null)
                    parent.Focus(); // Если нету выбранного ранее объекта, то установим на сам проект
                else
                    treeView1.Focus(); // Иначе установим на сам тривь, а он уже сам выберет ранее выбранный
            }
        }

        // ----------------------------------------------------------------------------------------
        public void Rename(CodeFile file)
        {
            BeginRenameVisual(file);
        }

        public void Copy(CodeFile file)
        {
            CodeFile newFile = file.MakeCopy(null);
            CodeProjectManager.Instance.Current.AddFile(newFile);
        }

        public void Move(CodeFile file)
        {
            var window = new OpenFolderWindow(System.IO.Path.GetDirectoryName(file.Path));
            bool? result = window.ShowDialog();

            if (result.HasValue && result.Value)
            {
                // Сохраняем исходный
                file.Save();

                try
                {
                    // Перемещение
                    file.Move(System.IO.Path.Combine(window.Path, file.Name));
                    CodeProjectManager.Instance.SaveCurrent();
                }
                catch (FileMoveException ex)
                {
                    ErrorWindow err = new ErrorWindow
                    {
                        ShortDesc = LocalString.Get("Error_MoveFile_short"),
                        FullDesc = LocalString.Get("Error_MoveFile_full"),
                        Exception = ex
                    };
                    err.ShowDialog();
                }
            }
        }

        // ----------------------------------------------------------------------------------------
        private void Item_Selected(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = sender as TreeViewItem;
            MainObject = item;

            e.Handled = true;   // Чтоб не полнималось событие вверх
        }

        // ----------------------------------------------------------------------------------------
        private void Parent_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var file = treeView1.SelectedItem as CodeFile;
            if (file != null)
            {
                _codeEditorView.OpenTab(file, true);
            }
        }

        private void Parent_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var file = treeView1.SelectedItem as CodeFile;
                if (file != null)
                {
                    _codeEditorView.OpenTab(file, true);
                }

                e.Handled = true;   // Чтоб ентер не уходил в редактор
            }
        }

        private void OnPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem treeViewItem = VisualUpwardSearch(e.OriginalSource as DependencyObject);

            if (treeViewItem != null)
            {
                treeViewItem.Focus();
                e.Handled = true;
            }
        }

        static TreeViewItem VisualUpwardSearch(DependencyObject source)
        {
            while (source != null && !(source is TreeViewItem))
                source = VisualTreeHelper.GetParent(source);

            return source as TreeViewItem;
        }

        // ----------------------------------------------------------------------------------------
        private CodeFile _currentEdit;

        private readonly DataTemplate EditableItemTemplate;
        private readonly DataTemplate ItemTemplate;
        private readonly DataTemplate CurrentEditorItemTemplate;
        private readonly DataTemplate OpenInEditorItemTemplate;

        // ----------------------------------------------------------------------------------------
        private void BeginRenameVisual(CodeFile file)
        {
            _currentEdit = file;

            // Применяем шаблон "изменяемый"
            ApplyTemplate(file, EditableItemTemplate);
        }

        private void EndRenameVisual()
        {
            ApplyTemplate(_currentEdit, ItemTemplate);
        }

        // ----------------------------------------------------------------------------------------
        private void TextBoxEditName_LostFocus(object sender, RoutedEventArgs e)
        {
            if (_currentEdit != null)
            {
                EndRenameVisual();

                _currentEdit = null;
            }
        }

        private void TextBoxEditName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;

                // Уводим фокус от текстбока, тем самым обновляя биндинг
                TreeViewItem item = parent.ItemContainerGenerator.ContainerFromItem(_currentEdit) as TreeViewItem;
                item.Focus();
            }
            else if (e.Key == Key.Escape)
            {
                e.Handled = true;

                // Возвращаемся к старому значению
                TextBox box = sender as TextBox;
                box.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
                
                TreeViewItem item = parent.ItemContainerGenerator.ContainerFromItem(_currentEdit) as TreeViewItem;
                item.Focus();
            }
        }

        private void TextBoxEditName_Loaded(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            //textBox.SelectAll();
            textBox.Focus();
        }

        // ----------------------------------------------------------------------------------------
        private void OnRenameFileInProjectExplorerCommand(object sender, ExecutedRoutedEventArgs e)
        {
            BeginRenameVisual(treeView1.SelectedItem as CodeFile);
        }

        private void HaveSelectedItem(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (treeView1.SelectedItem != null);
        }

        // ----------------------------------------------------------------------------------------
        private void MenuItem_Open_Click(object sender, RoutedEventArgs e)
        {
            var file = treeView1.SelectedItem as CodeFile;
            if (file != null)
            {
                _codeEditorView.OpenTab(file, true);
            }

            e.Handled = true;   // Чтоб тык не уходил в редактор
        }

        private void MenuItem_Rename_Click(object sender, RoutedEventArgs e)
        {
            BeginRenameVisual(treeView1.SelectedItem as CodeFile);
        }

        private void MenuItem_Move_Click(object sender, RoutedEventArgs e)
        {
            Move(treeView1.SelectedItem as CodeFile);
        }

        private void MenuItem_Copy_Click(object sender, RoutedEventArgs e)
        {
            Copy(treeView1.SelectedItem as CodeFile);
        }

        private void MenuItem_Exclude_Click(object sender, RoutedEventArgs e)
        {
            var file = treeView1.SelectedItem as CodeFile;
            if (file != null)
            {
                _codeEditorView.RemoveFromTabs(file);
                CodeProjectManager.Instance.Current.RemoveFile(file);
            }
        }

        private void MenuItem_NewVer_Click(object sender, RoutedEventArgs e)
        {
            var file = treeView1.SelectedItem as CodeFile;
            if (file != null)
            {
                //CodeFileVersionManager.Instance.MakeVersion(file);
                file.MakeVersion();
            }
        }

        private void MenuItem_GoToVer_Click(object sender, RoutedEventArgs e)
        {
            var file = treeView1.SelectedItem as CodeFile;
            if (file != null)
            {
                VersionSelectWindow window = new VersionSelectWindow(file) { Owner = Window.GetWindow(this) };
                bool? result = window.ShowDialog();
                if (result.HasValue && result.Value)
                {
                    object o = window.versionView.SelectedItem;
                    KeyValuePair<DateTime, string>? pair = window.versionView.SelectedItem as KeyValuePair<DateTime, string>?;
                    file.GoToVersion(pair.Value.Key);
                }
            }
        }

        // ----------------------------------------------------------------------------------------
        private void MenuItem_Click_Settings(object sender, RoutedEventArgs e)
        {
            ProjectSettingsWindow window = new ProjectSettingsWindow() { Owner = Window.GetWindow(this) };
            window.ShowDialog();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnDeleteFileCommand(object sender, ExecutedRoutedEventArgs e)
        {
            //throw new NotImplementedException();
            var file = treeView1.SelectedItem as CodeFile;
            int idx = parent.Items.IndexOf(file);
            if (file != null)
            {
                _codeEditorView.RemoveFromTabs(file);
                CodeProjectManager.Instance.Current.RemoveFile(file);
            }

            if (parent.Items.Count > 0)
            {
                if (idx == parent.Items.Count) idx--; // Чтоб не выходить за последний
                TreeViewItem item = parent.ItemContainerGenerator.ContainerFromItem(parent.Items[idx]) as TreeViewItem;
                item.IsSelected = true;
                item.Focus();
            }
            else
            {
                // Фокус на сам проект
                parent.IsSelected = true;
                parent.Focus();
            }

            // Переставить фокус на следующий файл
            //treeView1.Se
        }

        private void TreeView1_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!MainObject.IsFocused)
                MainObject.Focus();
        }
    }
}
