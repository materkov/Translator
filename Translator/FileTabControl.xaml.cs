using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
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
    /// Interaction logic for FileTabControl.xaml
    /// </summary>
    public partial class FileTabControl : UserControl, INotifyPropertyChanged
    {
        // Активный таб + фокус есть на редакторе
        public SolidColorBrush ActiveFocusTabBrushB { get; set; }
        public SolidColorBrush ActiveFocusTabBrushF { get; set; }

        // Активный таб + фокуса нет на редакторе
        public SolidColorBrush ActiveNotFocusTabBrush { get; set; }

        // Неактивный таб
        public SolidColorBrush NotActiveTabBrush { get; set; }
        
        // Таб, на который наведена мышь с фокусом
        public SolidColorBrush MouseHoverTabBrush { get; set; }

        // Таб, на который наведена мышь безфокуса
        public SolidColorBrush MouseHoverTabBrushNoFocus { get; set; }

        // нижняя строчка
        public SolidColorBrush BottomPanelBrush { get; set; }

        // На биндерах логика получается слишком сложной, проще по-старому

        public FileTabControl()
        {
            var state = (Application.Current.Resources["IDEState"]) as IDEState;

            ActiveFocusTabBrushB = state.FocusedHeaderBrushB;
            ActiveFocusTabBrushF = state.FocusedHeaderBrushF;
            ActiveNotFocusTabBrush = state.NotFocusedHeaderBrushB;
            NotActiveTabBrush = new SolidColorBrush(Colors.Transparent);
            MouseHoverTabBrush = IDEState.Get().EditorColor_MouseHoverTabFocusedBrush;
            MouseHoverTabBrushNoFocus = IDEState.Get().EditorColor_MouseHoverTabNotFocusedBrush;

            BottomPanelBrush = ActiveFocusTabBrushB;

            Elements = new ObservableCollection<CodeFile>();

            InitializeComponent();

            OnPropertyChanged("SelectedBrush");
        }

        public CodeFile CurrentCodeFile { get; set; }
        public CodeEditorView _CodeEditorView { get; set; }
        public ObservableCollection<CodeFile> Elements { get; private set; }

        public void Init(CodeEditorView codeEditorView)
        {
            _CodeEditorView = codeEditorView;
        }

        // Чтоб не выходили за поле зрения
        private void BringToView(CodeFile file)
        {
            object o = itemsControl.ItemContainerGenerator.ContainerFromItem(file);
            if (o != null)
            {
                ContentPresenter p = (o as ContentPresenter);
                p.BringIntoView();
            }
        }

        public void FocusOn(CodeFile file)
        {
            if (CurrentCodeFile != file)
            {
                CurrentCodeFile = file;
                OnPropertyChanged("CurrentCodeFile");   // Это только для биндингов
                OnSelectionChanged();
            }

            BringToView(file);
        }

        public void AddTab(CodeFile file)
        {
            Elements.Add(file);
            OnFileOpened(new FileArgs(file));

            //BringToView(file);
        }

        public void CloseTab(CodeFile file)
        {
            int idx = Elements.IndexOf(file);
            
            // Если закрываемый таб - последний, то так и остаемся на последнем
            if (idx == Elements.Count - 1)
                idx = idx - 1;
            else
                idx = idx % Elements.Count;

            Elements.Remove(file);
            OnFileClosed(new FileArgs(file));
            file.Save();

            if (Elements.Count == 0)
                FocusOn(null); // Файлов октрытых больше нет
            else
            {
                // Усли удаляемый - текущий, надо преместить "фокус"
                if (CurrentCodeFile == file)
                    FocusOn(Elements[idx]);
            }
        }

        public void CloseAllTabs()
        {
            while (Elements.Count > 0)
            {
                CodeFile file = Elements.First();
                Elements.RemoveAt(0);
                
                file.Save();
                OnFileClosed(new FileArgs(file));
            }

            FocusOn(null);
        }

        public void GoToNextTab()
        {
            if (CurrentCodeFile == null) return;

            int idx = (Elements.IndexOf(CurrentCodeFile) + 1) % Elements.Count;
            FocusOn(Elements[idx]);
        }

        // Скроллинг по табам
        private void scrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scrollviewer = sender as ScrollViewer;
            if (e.Delta > 0)
            {
                scrollviewer.LineLeft();
                scrollviewer.LineLeft();
            }
            else
            {
                scrollviewer.LineRight();
                scrollviewer.LineRight();
            }

            e.Handled = true;
        }

        private void ButtonLeft_Click(object sender, RoutedEventArgs e)
        {
            scrollViewer.LineLeft();
        }

        private void ButtonRight_Click(object sender, RoutedEventArgs e)
        {
            scrollViewer.LineRight();
        }

        // Тык на вкладку
        private void Tab_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CodeFile newFile = (sender as Grid).DataContext as CodeFile;
            FocusOn(newFile);
            //_CodeEditorView.ForceFocus();
        }

        // Твк в пустое место
        private void EmptySpace_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _CodeEditorView.ForceFocus();
        }

        // Тык на кнопку закрытия
        private void closeTextBlock_Click(object sender, MouseButtonEventArgs e)
        {
            CodeFile file = (sender as Image).DataContext as CodeFile;
            CloseTab(file);
            e.Handled = true;
        }

        // ----------------------------------------------------------------------------------------
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public event EventHandler SelectionChanged;
        public void OnSelectionChanged()
        {
            EventHandler handler = SelectionChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public event EventHandler<FileArgs> FileOpened;
        protected virtual void OnFileOpened(FileArgs e)
        {
            EventHandler<FileArgs> handler = FileOpened;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<FileArgs> FileClosed;
        private void OnFileClosed(FileArgs e)
        {
            EventHandler<FileArgs> handler = FileClosed;
            if (handler != null) handler(this, e);
        }
    }

    public class SelectionChangedArgs : EventArgs
    {
        public CodeFile File { get; private set; }

        public SelectionChangedArgs(CodeFile file)
        {
            File = file;
        }
    }

    class CurrentSelectionConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            CodeFile file1 = values[0] as CodeFile;
            CodeFile file2 = values[1] as CodeFile;

            return file1 == file2;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class Converter0 : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool isMouseOver = (bool) values[0];
            CodeFile currentCodeFile = values[1] as CodeFile;
            CodeFile tabFile = values[2] as CodeFile;
            bool focused = (bool) values[3];

            if (currentCodeFile == tabFile && focused)
                return 0;
            else
                return -1;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    class Converter1 : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool isMouseOver = (bool)values[0];
            CodeFile currentCodeFile = values[1] as CodeFile;
            CodeFile tabFile = values[2] as CodeFile;
            bool focused = (bool)values[3];

            if (currentCodeFile == tabFile && !focused)
                return 1;
            else
                return -1;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    class Converter2 : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool isMouseOver = (bool)values[0];
            CodeFile currentCodeFile = values[1] as CodeFile;
            CodeFile tabFile = values[2] as CodeFile;
            bool focused = (bool)values[3];

            if (currentCodeFile != tabFile && !isMouseOver)
                return 2;
            else
                return -1;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    class Converter3 : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool isMouseOver = (bool)values[0];
            CodeFile currentCodeFile = values[1] as CodeFile;
            CodeFile tabFile = values[2] as CodeFile;
            bool focused = (bool)values[3];

            if (currentCodeFile != tabFile && isMouseOver && focused)
                return 3;
            else
                return -1;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    class Converter4 : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool isMouseOver = (bool)values[0];
            CodeFile currentCodeFile = values[1] as CodeFile;
            CodeFile tabFile = values[2] as CodeFile;
            bool focused = (bool)values[3];

            if (currentCodeFile != tabFile && isMouseOver && !focused)
                return 4;
            else
                return -1;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
