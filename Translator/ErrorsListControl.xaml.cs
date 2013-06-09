using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
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
using System.Windows.Threading;
using Translator.Annotations;

namespace Translator
{
    /// <summary>
    /// Interaction logic for ErrorsListContainter.xaml
    /// </summary>
    public partial class ErrorsListControl : UserControl, ErrorsListView, INotifyPropertyChanged
    {
        public ErrorsListControl()
        {
            ErrorsListContainter = new ObservableCollection<CompileError>();
            Dict = new Pulser();

            InitializeComponent();
        }

        private IDEState _IDEState;

        private FrameworkElement _selectedItem;
        public FrameworkElement MainObject
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                OnPropertyChanged("MainObject");
            }
        }

        public void Init(CodeEditorView codeEditorView)
        {
            CodeEditorView = codeEditorView;
            _IDEState = Application.Current.Resources["IDEState"] as IDEState;
            listView1.DataContext = ErrorsListContainter; // Все манипуляции - через данные
            CheckboxConverter.CodeEditor = codeEditorView;

            // Если проект меняется, то все ошибки надо удалить
            // Иначе можно будет тыкать на ошибку, и переходитьвв неизвестно какой файл
            CodeProjectManager.Instance.CurrentProjectChanged += (sender, args) => ClearAllErrors();

            MainObject = listView1;
        }

        private CompileError _lastSelectedError;

        public CompileError LastSelectedError
        {
            get { return _lastSelectedError; }
            set
            {
                _lastSelectedError = value;
                OnPropertyChanged("LastSelectedError");
            }
        }

        public CodeEditorView CodeEditorView { get; private set; }
        public ObservableCollection<CompileError> ErrorsListContainter { get; private set; }

        public IEnumerable<CompileError> ErrorsList
        {
            get
            {
                return ErrorsListContainter;
            }
        }

        private bool _needSelectFirst = false;

        public void AddErrorsCollection(IEnumerable<CompileError> list)
        {
            //ErrorsListContainter.Add(new CollectionContainer {Collection = list});
            //ErrorsListContainter.AddRange(list);
            foreach(CompileError err in list)
                ErrorsListContainter.Add(err);

            listView1.UpdateLayout();
            ListViewItem item = listView1.ItemContainerGenerator.ContainerFromIndex(0) as ListViewItem;
            MainObject = item;
            _needSelectFirst = true;
        }

        public void ClearAllErrors()
        {
            ErrorsListContainter.Clear();
            MainObject = listView1;
        }

        /*
         * err.IsActive && err.File == CurrentCodeFile && err.BeginLine == Line
         * err.IsActive && err.File == CurrentCodeFile && err.BeginLine == Line
         * err.IsActive && err.File == CurrentCodeFile && err.BeginOffset == AvalonEditor.TextArea.Caret.Offset
         */

        public void SelectError(CompileError err, bool setFocus, bool setEditorFocus)
        {
            _needSetEditorFocus = setEditorFocus;
            _needSetFocus = setFocus;
            listView1.SelectedItem = err;
            listView1.ScrollIntoView(err);
            
            if (err != null)
                LastSelectedError = err;
        }

        public void ScrollToError(CompileError err)
        {
            listView1.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() => listView1.ScrollIntoView(err)));
            //listView1.ScrollIntoView(err);
        }

        private void ErrorItem_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListViewItem item = sender as ListViewItem;
            CompileError error = item.Content as CompileError;

            if (error.IsActive)
            {
                // Позиционируем на позицию курсор
                CodeEditorView.SetCaretTo(error.File, error.BeginOffset);
            }

            e.Handled = true;
        }

        private bool _needSetFocus = false;
        private bool _needSetEditorFocus = true;

        private void ErrorItem_Selected(object sender, RoutedEventArgs e)
        {
            ListViewItem item = sender as ListViewItem;
            if (_needSetFocus)
            {
                Dispatcher.BeginInvoke(new Action(() => item.Focus())); // official hack! BAD!
                _needSetFocus = false;
            }

            var error = item.Content as CompileError;

            // Обязательно надо как минимум открыть вкладку
            // Позиционироваться ли там куда-то, это уже другой вопрос
            //CodeEditorView.OpenTab(error.File, false);

            if (error != null && error.IsActive && _needSetEditorFocus)
            {
                // Позиционируем на позицию курсор
                CodeEditorView.ScrollTo(error.File, error.BeginOffset);
                CodeEditorView.HighlightError(error.File, error);
            }

            _needSetEditorFocus = true;
            MainObject = item;

            //e.Handled = true;
        }

        private void ErrorItem_UnSelected(object sender, RoutedEventArgs e)
        {
            ListViewItem item = sender as ListViewItem;
            CompileError error = item.Content as CompileError;

            if (error != null && error.IsActive && IsFocused)
            {
                CodeEditorView.UnHighlightError();
            }

            //e.Handled = true;
        }

        private void UserControl_GotFocus(object sender, RoutedEventArgs e)
        {
            //(listView1.Background as SolidColorBrush).Color = _IDEState.FocusedElementColor;

            //e.Handled = true;
            // Этот флажок устанавливаетя при лобавлении ошибок, однако
            // работать он будет только если SelectedIndex не установлен,
            // Потому что юзер может пройтись по тексту и там SelectedIndex уже установили
            // и спрыгивать на начало было бы нелогично
            if (_needSelectFirst && listView1.SelectedIndex == -1)
                listView1.SelectedIndex = 0;

            _needSelectFirst = false;
        }

        private void UserControl_LostFocus(object sender, RoutedEventArgs e)
        {
            //(listView1.Background as SolidColorBrush).Color = _IDEState.NotFocusedElementColor;
            CodeEditorView.UnHighlightError();

            //e.Handled = false;
        }

        private void ErrorList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ListViewItem item = sender as ListViewItem;
                CompileError error = item.Content as CompileError;

                if (error.IsActive)
                {
                    // Позиционируем на позицию курсор
                    CodeEditorView.SetCaretTo(error.File, error.BeginOffset);
                }

                e.Handled = true;
            }
            else if (e.Key == Key.Space)
            {
                ListViewItem item = sender as ListViewItem;
                CompileError obj = item.Content as CompileError;

                if (CodeEditorView.GetIfAnotherErrorColor(obj.ErrorType))
                    CodeEditorView.UnSetAnotherErrorColor(obj.ErrorType);
                else
                    CodeEditorView.SetAnotherErrorColor(obj.ErrorType);

                Dict.DoPulse();
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            var checkbox = sender as CheckBox;
            var err = checkbox.DataContext as CompileError;

            CodeEditorView.SetAnotherErrorColor(err.ErrorType);
            //UpdateCheckboxes();

            Dict.DoPulse();
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            e.Handled = true;

            var checkbox = sender as CheckBox;
            var err = checkbox.DataContext as CompileError;

            CodeEditorView.UnSetAnotherErrorColor(err.ErrorType);
            //UpdateCheckboxes();

            Dict.DoPulse();
        }

        private static T GetFrameworkElementByName<T>(FrameworkElement referenceElement) where T : FrameworkElement
        {
            FrameworkElement child = null;
            for (Int32 i = 0; i < VisualTreeHelper.GetChildrenCount(referenceElement); i++)
            {
                child = VisualTreeHelper.GetChild(referenceElement, i) as FrameworkElement;
                if (child != null && child.GetType() == typeof (T))
                {
                    break;
                }
                else if (child != null)
                {
                    child = GetFrameworkElementByName<T>(child);
                    if (child != null && child.GetType() == typeof (T))
                    {
                        break;
                    }
                }
            }
            return child as T;
        }

        private static FrameworkElement FindNameFromCellTemplate(ListView listView, Int32 cellColumn, Int32 cellRow, String name)
        {
            if (listView == null)
                throw new ArgumentNullException("listView");

            if (!listView.IsLoaded)
                throw new InvalidOperationException("ListView is not yet loaded");

            if (cellRow >= listView.Items.Count || cellRow < 0)
                throw new ArgumentOutOfRangeException("row");

            GridView gridView = listView.View as GridView;
            if (gridView == null)
                return null;

            if (cellColumn >= gridView.Columns.Count || cellColumn < 0)
                throw new ArgumentOutOfRangeException("column");

            ListViewItem item = listView.ItemContainerGenerator.ContainerFromItem(listView.Items[cellRow]) as ListViewItem;
            if (item != null)
            {
                if (!item.IsLoaded)
                    item.ApplyTemplate();

                GridViewRowPresenter rowPresenter = GetFrameworkElementByName<GridViewRowPresenter>(item);

                if (rowPresenter != null)
                {
                    ContentPresenter templatedParent = VisualTreeHelper.GetChild(rowPresenter, cellColumn) as ContentPresenter;
                    DataTemplate dataTemplate = gridView.Columns[cellColumn].CellTemplate;
                    if (dataTemplate != null && templatedParent != null)
                    {
                        return dataTemplate.FindName(name, templatedParent) as FrameworkElement;
                    }
                }
            }

            return null;
        }

        private void button_SetAllChecked(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i <listView1.Items.Count; i++)
            {
                var elem = FindNameFromCellTemplate(listView1, 0, i, "checkbox1") as CheckBox;
                if (!elem.IsChecked.Value)
                    elem.IsChecked = true;
            }
        }

        private void button_SetAllUnChecked(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < listView1.Items.Count; i++)
            {
                var elem = FindNameFromCellTemplate(listView1, 0, i, "checkbox1") as CheckBox;
                if (elem.IsChecked.Value)
                    elem.IsChecked = false;
            }
        }

        private void button_ClearSorting(object sender, RoutedEventArgs e)
        {
            listView1.ClearSorting();
            //UpdateLayout();
        }

        public Pulser Dict { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ListView1_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!MainObject.IsFocused)
                MainObject.Focus();
        }

        private void EmptySpace_Click(object sender, MouseButtonEventArgs e)
        {
            if (!MainObject.IsFocused)
                MainObject.Focus();
        }
    }

    class Pulsar : INotifyPropertyChanged
    {
        public void DoPulse()
        {
            OnPropertyChanged("Pulse");
        }

        public object Pulse { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class CurrentLineHihlight_Converter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            int line0 = (int)values[0]; // CodeEditorView.Line
            int line1 = (int)values[1]; // Line

            CodeFile file0 = values[2] as CodeFile;
            CodeFile file1 = values[3] as CodeFile;

            if (file0 == null || file1 == null) return false;

            bool isFocused = (bool)values[4];

            return isFocused && line0 == line1 && file0 == file1;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("This method should never be called");
        }
    }

    public class LastErrorHighlight_Converter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            CompileError error0 = values[0] as CompileError;    // Current selected error
            CompileError error1 = values[1] as CompileError;    // LastSelectedError

            bool isFocused = (bool)values[2];

            return isFocused && error0 == error1;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("This method should never be called");
        }
    }

    public class Conv_Type : IValueConverter
    {
        class MyDep : DependencyObject
        {
            private string text 
            {
                get { return (string)GetValue(TextProperty); }
                set { SetValue(TextProperty, value); }
            }

            public static readonly DependencyProperty TextProperty = DependencyProperty.Register("TextProperty", typeof(string), typeof(MyDep));

            public override string ToString()
            {
                return text;
            }
        }

        private static List<TextBlock> depList = null;
        static Conv_Type()
        {
            depList = new List<TextBlock>();
            
            for (int i = 0; i < 10; i++)
            {
                var binding = new Binding()
                    {
                        Source = Application.Current.Resources["LocalString"],
                        Path = new PropertyPath("Dict[Err_t" + i.ToString() + "]"),
                        Mode=BindingMode.TwoWay
                    };

                TextBlock dep = new TextBlock();
                BindingOperations.SetBinding(dep, TextBlock.TextProperty, binding);
                depList.Add(dep);
            }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return depList[(int)value];
            //return (int)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("This method should never be called");
        }
    }

    class DiagConverter : IMultiValueConverter
    {
        private LocalString _localString;
        public DiagConverter()
        {
            _localString = App.Current.Resources["LocalString"] as LocalString;
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            CompileError.Type err = (CompileError.Type)values[0];
            //int lang = (int) values[1];
            switch (err)
            {
                case CompileError.Type.Type0: 
                    return CompileError.NeedFullDiagnostic(CompileError.Type.Type0) ? _localString.Dict["Err_0_full"] : _localString.Dict["Err_0_short"];
                case CompileError.Type.Type1:
                    return CompileError.NeedFullDiagnostic(CompileError.Type.Type1) ? _localString.Dict["Err_1_full"] : _localString.Dict["Err_1_short"];
                case CompileError.Type.Type2:
                    return CompileError.NeedFullDiagnostic(CompileError.Type.Type2) ? _localString.Dict["Err_2_full"] : _localString.Dict["Err_2_short"];
                case CompileError.Type.Type3:
                    return CompileError.NeedFullDiagnostic(CompileError.Type.Type3) ? _localString.Dict["Err_3_full"] : _localString.Dict["Err_3_short"];
                case CompileError.Type.Type4:
                    return CompileError.NeedFullDiagnostic(CompileError.Type.Type4) ? _localString.Dict["Err_4_full"] : _localString.Dict["Err_4_short"];
                case CompileError.Type.Type5:
                    return CompileError.NeedFullDiagnostic(CompileError.Type.Type5) ? _localString.Dict["Err_5_full"] : _localString.Dict["Err_5_short"];
                case CompileError.Type.Type6:
                    return CompileError.NeedFullDiagnostic(CompileError.Type.Type6) ? _localString.Dict["Err_6_full"] : _localString.Dict["Err_6_short"];
                case CompileError.Type.Type7:
                    return CompileError.NeedFullDiagnostic(CompileError.Type.Type7) ? _localString.Dict["Err_7_full"] : _localString.Dict["Err_7_short"];
                case CompileError.Type.Type8:
                    return CompileError.NeedFullDiagnostic(CompileError.Type.Type8) ? _localString.Dict["Err_8_full"] : _localString.Dict["Err_8_short"];
                case CompileError.Type.Type9:
                    return CompileError.NeedFullDiagnostic(CompileError.Type.Type9) ? _localString.Dict["Err_9_full"] : _localString.Dict["Err_9_short"];
                default:
                    return "";
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    class ErrorTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int) value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    class BorderBrushConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(values[0] is bool)) return 0; // Эта строка для XAML DESIGNER!!!!!!!!!!!! иначе - он падает

            bool focused = (bool) values[0];
            SolidColorBrush FocusedHeaderBrush = values[1] as SolidColorBrush;
            SolidColorBrush NotFocusedHeaderBrush = values[2] as SolidColorBrush;

            return focused ? FocusedHeaderBrush : NotFocusedHeaderBrush;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    class SpecialErrorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            CompileError.Type? type = values[0] as CompileError.Type?;
            var specialErrors = values[1] as ObservableCollection<CompileError.Type>;

            return specialErrors.Count(e => e == type.Value) > 0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    class CheckboxConverter : IMultiValueConverter
    {
        public static CodeEditorView CodeEditor;

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            CompileError.Type? type = values[0] as CompileError.Type?;

            return CodeEditor.GetIfAnotherErrorColor(type.Value);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}