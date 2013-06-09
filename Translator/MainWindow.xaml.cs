using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Translator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, MainWindowView
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public void ActivateProjectExplorer()
        {
            projectExplorerControlPanel.Activate();
        }

        public void ActivateErrors()
        {
            errorsListControlPanel.Activate();
        }

        public void ActivateContents()
        {
            contentsControlPanel.Activate();
        }

        public void ActivateCode()
        {
            codeEditor.ForceFocus();
        }

        private ProjectExplorerControl projectExplorer;
        private ContentsControl contentsControl;
        private ErrorsListControl errorsListControl;

        private void Init()
        {
            projectExplorer = grid1.Resources["ProjectExplorerControl"] as ProjectExplorerControl;
            contentsControl = grid1.Resources["ContentsControl"] as ContentsControl;
            errorsListControl = grid1.Resources["ErrorsListControl"] as ErrorsListControl;

            codeEditor.Init(errorsListControl);
            projectExplorer.Init(codeEditor);
            errorsListControl.Init(codeEditor);
            contentsControl.Init(codeEditor);

            Logger.Init(StatusPanel);

            _hotkeyWindow.Owner = this;

            bookmarksDropDownControl.Init(codeEditor);
            colorsDropDownControl.Init(codeEditor);
            CodeProjectManager.Instance.Init(codeEditor, errorsListControl, this);

            // Обновление имени текущего файла в заголовке окна
            codeEditor.ActiveFileChanged += (sender, args) => UpdateTitle();
            CodeProjectManager.Instance.CurrentProjectChanged += (sender, args) => UpdateTitle();
        }

        private void UpdateTitle()
        {
            if (codeEditor.CurrentCodeFile != null)
            {
                MultiBinding multiBinding = new MultiBinding();
                multiBinding.Bindings.Add(new Binding { Source = codeEditor.CurrentCodeFile, Path = new PropertyPath("Name") });
                multiBinding.Bindings.Add(new Binding { Source = CodeProjectManager.Instance.Current, Path = new PropertyPath("Name") });
                multiBinding.Converter = new NameAndProjectConverter();

                SetBinding(TitleProperty, multiBinding);
            }
            else if (CodeProjectManager.Instance.Current != null)
            {
                Binding binding = new Binding
                {
                    Source = CodeProjectManager.Instance.Current,
                    Path = new PropertyPath("Name"),
                    Converter = new ProjectConverter()
                };

                SetBinding(TitleProperty, binding);
            }
            else
            {
                BindingOperations.ClearBinding(this, TitleProperty);
                Title = "Translator";
            }
        }

        private class NameAndProjectConverter : IMultiValueConverter
        {
            public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
            {
                return (string) values[0] + " - " + (string) values[1] + " - Translator";
            }

            public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            {
                throw new NotSupportedException();
            }
        }
        private class ProjectConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return (string)value + " - Translator";
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotSupportedException();
            }
        }

        private void Window_Loaded(object sender, EventArgs e)
        {
            Init();

            // Закрыть окна, какие не нужны
            if (!IDEState.Get().ProjectExplorer_Visible)
                projectExplorerControlPanel.Collapse();

            if (!IDEState.Get().Errors_visible)
                errorsListControlPanel.Collapse();

            if (!IDEState.Get().Contents_Visible)
                contentsControlPanel.Collapse();
            
            Logger.LogWithBinding("Status_Ready");

            // Стартовая подсказка
            if (IDEState.Get().ShowStartPage)
            {
                StartPageWindow window = new StartPageWindow { Owner = this };
                window.ShowDialog();
            }

            // Очистить былые ошибки, считаем все заново
            IDEState.Get().RecentErrors.Clear();

            // Поначалу ставим фокус в прожект эксплорер
            projectExplorerControlPanel.Activate();
        }

        private void Window_Closing(object sender, CancelEventArgs args)
        {
            // Сохранить состояние открытых/закрытых панелей
            IDEState.Get().ProjectExplorer_Visible = projectExplorerControlPanel.IsActive;
            IDEState.Get().Errors_visible = errorsListControlPanel.IsActive;
            IDEState.Get().Contents_Visible = contentsControlPanel.IsActive;

            // Закрыть дочерние окна
            _hotkeyWindow.RealClose();

            // Закрыть текущий проект
            codeEditor.CloseAllTabs();
            CodeProjectManager.Instance.CloseCurrent();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            
        }

        // ----------------------------------------------------------------------------------------
        // Commands
        // ----------------------------------------------------------------------------------------
        #region COMMANDS
        private void OnCompileCommand(object sender, ExecutedRoutedEventArgs e)
        {
            CodeProjectManager.Instance.CompileProject(CodeProjectManager.Instance.Current);
        }

        private void OnCompileCurrentFileCommand(object sender, ExecutedRoutedEventArgs e)
        {
            CodeProjectManager.Instance.CompileFile(CodeProjectManager.Instance.Current, codeEditor.CurrentCodeFile);
        }
        
        private void OnNewProjectCommand(object sender, ExecutedRoutedEventArgs e)
        {
            NewProjectWindow newProjectWindow = new NewProjectWindow() { Owner = this };
            newProjectWindow.ShowDialog();

            if (newProjectWindow.DialogResult.HasValue && newProjectWindow.DialogResult.Value)
            {
                // Закрыть предыдущий
                codeEditor.CloseAllTabs();
                CodeProjectManager.Instance.CloseCurrent();

                // Открыть новый
                string name = newProjectWindow.textBox1.Text;
                string path = newProjectWindow.comboBox2.Text;

                // Создаем директорию под него
                Directory.CreateDirectory(System.IO.Path.Combine(path, name));
                string fileName = System.IO.Path.Combine(path, name, name + CodeProject.DefaultExtension);


                // Создаем новый
                CodeProjectManager.Instance.Current = CodeProject.MakeNew(fileName);

                // Сразу добавим новый файл
                OnAddNewFileCommand(sender, e);
            }
        }
        
        private void OnOpenProjectCommand(object sender, ExecutedRoutedEventArgs e)
        {
            OpenProjectWindow window = new OpenProjectWindow(this);
            bool? showDialog = window.ShowDialog();

            if (showDialog.HasValue && showDialog.Value)
            {
                // Закрыть предыдущий
                codeEditor.CloseAllTabs();
                CodeProjectManager.Instance.CloseCurrent();

                CodeProjectManager.Instance.Load(window.Path);
            }
        }
        
        private void OnCloseCurrentTabCommand(object sender, ExecutedRoutedEventArgs e)
        {
            codeEditor.CloseCurrentTab();
        }
        
        private void OnExitCommand(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }

        private void OnBuildProjectCommand(object sender, ExecutedRoutedEventArgs e)
        {
            CodeProjectManager.Instance.CompileProject(CodeProjectManager.Instance.Current);
        }

        private void OnAddNewFileCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (CodeProjectManager.Instance.Current != null)
            {
                string filePath = CodeProjectManager.Instance.Current.GetDefaultNewFilePath();

                CodeFile file = new CodeFile(filePath, true);
                CodeProjectManager.Instance.Current.AddFile(file);

                // Открыть во вкладках только что созданный файл (и фокус засунуть на него)
                codeEditor.OpenTab(file, true);
            }
        }

        private void OnSaveCurrentFileCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (codeEditor.CurrentCodeFile != null)
                codeEditor.CurrentCodeFile.Save();
        }

        private void OnSaveAllFilesCommand(object sender, ExecutedRoutedEventArgs e)
        {
            foreach (CodeFile file in codeEditor.OpenedFiles)
            {
                file.Save();
            }
        }

        private void OnSaveProjectCommand(object sender, ExecutedRoutedEventArgs e)
        {
            CodeProjectManager.Instance.SaveCurrent();
        }

        private void OnShowSettingsCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var window = new SettingsWindow() { Owner = this };
            window.ShowDialog();
        }

        private void OnCloseProjectCommand(object sender, ExecutedRoutedEventArgs e)
        {
            // Закрыть все файлы, относящиеся к текущему проекту
            codeEditor.CloseAllTabs();

            // Закрываем сам проект
            CodeProjectManager.Instance.CloseCurrent();
        }

        private void OnAddExistingFileCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var window = new OpenFileWindow(this);
            bool? result = window.ShowDialog();

            if (result.HasValue && result.Value)
            {
                try
                {
                    CodeFile file = new CodeFile(window.Path, false);
                    CodeProjectManager.Instance.Current.AddFile(file);
                }
                catch (FileExistsException ex)
                {
                    // Если уже существует, просто выделяем его
                    projectExplorer.FocusOnFile(ex.File);
                }
            }
        }

        private void OnRenameCommand(object sender, ExecutedRoutedEventArgs e)
        {
            projectExplorer.Rename(codeEditor.CurrentCodeFile);
        }

        private void OnMoveCommand(object sender, ExecutedRoutedEventArgs e)
        {
            projectExplorer.Move(codeEditor.CurrentCodeFile);
        }

        private void OnNewCopyCommand(object sender, ExecutedRoutedEventArgs e)
        {
            projectExplorer.Copy(codeEditor.CurrentCodeFile);
        }

        private void OnCloseAllFilesCommand(object sender, ExecutedRoutedEventArgs e)
        {
            codeEditor.CloseAllTabs();
        }

        private void OnNewVersionCommand(object sender, ExecutedRoutedEventArgs e)
        {
            codeEditor.CurrentCodeFile.MakeVersion();
        }

        private void OnGoToVersionCommand(object sender, ExecutedRoutedEventArgs e)
        {
            VersionSelectWindow window = new VersionSelectWindow(codeEditor.CurrentCodeFile) { Owner = this };
            bool? result = window.ShowDialog();
            if (result.HasValue && result.Value)
            {
                KeyValuePair<DateTime, string>? pair = window.versionView.SelectedItem as KeyValuePair<DateTime, string>?;
                codeEditor.CurrentCodeFile.GoToVersion(pair.Value.Key);
            }
        }

        private void OnBookmarksCommand(object sender, ExecutedRoutedEventArgs e)
        {
            // Если уже есть закладка на этой строке, удаляем ее
            CodeFile file = codeEditor.CurrentCodeFile;
            int removed = file.Bookmarks.RemoveAll(b => b.Line == codeEditor.Line);

            if (removed == 0)
            {
                // Если ничего нету На текущел линии, то добавляем
                bool foundFirstEmptyKey = false;
                int key = 0;

                while (!foundFirstEmptyKey)
                {
                    foundFirstEmptyKey = (file.Bookmarks.Count(b => b.Key == key) == 0);
                    key++;
                }

                file.AddBookmark(codeEditor.Line, key - 1);
            }

            codeEditor.UpdateBookmarks();
        }

        //private ContentWindow _contentWindow;

        private void OnShowContentsCommand(object sender, ExecutedRoutedEventArgs e)
        {
            contentsControlPanel.Activate();
        }

        private void OnShowCodeCommand(object sender, ExecutedRoutedEventArgs e)
        {
            codeEditor.ForceFocus();
        }

        private void OnShowCodeCommentsCommand(object sender, ExecutedRoutedEventArgs e)
        {
            codeEditor.SetFocusInComments();
        }

        private void OnShowCodeCheckboxesCommand(object sender, ExecutedRoutedEventArgs e)
        {
            codeEditor.SetFocusInCheckboxes();
        }

        private void OnShowErrorsCommand(object sender, ExecutedRoutedEventArgs e)
        {
            errorsListControlPanel.Activate();
        }

        private void OnShowSolutionExplorerCommand(object sender, ExecutedRoutedEventArgs e)
        {
            projectExplorerControlPanel.Activate();
        }
        #endregion

        // ----------------------------------------------------------------------------------------
        private void HaveCurrentProject(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (CodeProjectManager.Instance.Current != null);
        }

        private void HaveCurrentFile(object sender, CanExecuteRoutedEventArgs e)
        {
            if (codeEditor == null)
                e.CanExecute = false;
            else 
                e.CanExecute = (codeEditor.CurrentCodeFile != null);
        }

        // ----------------------------------------------------------------------------------------
        private void MenuRecentPeoject_Clicked(object sender, RoutedEventArgs e)
        {
            // Закрыть все что было ранее!
            codeEditor.CloseAllTabs();

            string path = ((MenuItem) sender).Header as string;
            CodeProjectManager.Instance.Load(path);
        }

        private void MenuShowStartPage_OnClick(object sender, RoutedEventArgs e)
        {
            // Стартовая подсказка
            StartPageWindow window = new StartPageWindow() { Owner = this };
            window.ShowDialog();
        }

        private void MenuWindows_Click(object sender, RoutedEventArgs e)
        {
            WindowsWindow window = new WindowsWindow(codeEditor.OpenedFiles, codeEditor) { Owner = this };
            window.ShowDialog();
        }

        private void WindowItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            if (item.DataContext == null)
                return; // Это не пункт с файлом
            CodeFile file = item.DataContext as CodeFile;
            codeEditor.OpenTab(file, true);
        }

        // ----------------------------------------------------------------------------------------
        private void Splitter_OnDragCompleted(object sender, DragCompletedEventArgs e)
        {
            // Сохраняем значения для будущего запуска
            IDEState.Get().ErrorsRow_Height = ErrorsRow.ActualHeight;
            IDEState.Get().ProjectExplorerColumn_Width = ProjectExplorerColumn.ActualWidth;
            IDEState.Get().ContentsColumn_Width = ContentsColumn.ActualWidth;
        }

        private void Splitter_OnDragStarted(object sender, DragStartedEventArgs e)
        {
            // Установка максимальных границ
            ContentsColumn.MaxWidth = CodeColumn.ActualWidth + ContentsColumn.ActualWidth - CodeColumn.MinWidth;
            ProjectExplorerColumn.MaxWidth = CodeColumn.ActualWidth + ProjectExplorerColumn.ActualWidth - CodeColumn.MinWidth;
            ErrorsRow.MaxHeight = grid1.ActualHeight - CodeRow.MinHeight;
        }

        // ----------------------------------------------------------------------------------------
        private void ColorButton_Click(object sender, RoutedEventArgs e)
        {
            DoColorSelection();
        }

        private void ColorButton_MenuClosed(object sender, RoutedEventArgs e)
        {
            CurrentColor.Fill = colorsDropDownControl.Selected;
            DoColorSelection();
            codeEditor.ForceFocus();
        }

        private void DoColorSelection()
        {
            SolidColorBrush color = colorsDropDownControl.Selected;
            if (color != null)
            {
                if (color.Color == Colors.Transparent)
                    codeEditor.EraseColorSegment();
                else
                    codeEditor.AddColorSegment(color);
            }
        }

        private HotkeyWindow _hotkeyWindow = new HotkeyWindow();

        private void OnShowHotkeysCommand(object sender, ExecutedRoutedEventArgs e)
        {
            _hotkeyWindow.Show();
            _hotkeyWindow.Activate();
            
            //Activate();
        }

        private void OnHelpCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (IDEState.Get().CurrentLanguage == 0)
                Process.Start("ENG.chm");
            else
                Process.Start("RUS.chm");
        }
    } // class MainWindow

    // Преобразовать цвет в SlidColorBrush
    class ColorToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new SolidColorBrush((value as Color?).Value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    // Выделить чекбоксом текущий контрол
    class CurrentCheckConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            CodeFile file0 = values[0] as CodeFile;
            CodeFile file1 = values[1] as CodeFile;

            if (file0 == null || file1 == null)
                return false;
            else
                return file0 == file1;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}