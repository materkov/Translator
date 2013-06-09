using System;
using System.Collections.Generic;
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
    /// Interaction logic for CloseablePanel.xaml
    /// </summary>
    public partial class CloseablePanel : UserControl
    {
        // CONTENT
        public static readonly new DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof (UserControl), typeof (CloseablePanel), new PropertyMetadata(default(UserControl), ContentChangedCallback));

        private static void ContentChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            CloseablePanel panel = dependencyObject as CloseablePanel;
            panel.Presenter.Content = dependencyPropertyChangedEventArgs.NewValue;
        }

        public new UserControl Content
        {
            get { return (UserControl) GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        // HEADER
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof (string), typeof (CloseablePanel), new PropertyMetadata(default(string), HeaderChangedCallback));

        private static void HeaderChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            CloseablePanel panel = dependencyObject as CloseablePanel;
            panel.HeadetTextBlock.Text = dependencyPropertyChangedEventArgs.NewValue as string;
        }

        public string Header
        {
            get { return (string) GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        // COLUMN
        public static readonly DependencyProperty ColumnProperty =
            DependencyProperty.Register("Column", typeof (ColumnDefinition), typeof (CloseablePanel), new PropertyMetadata(default(ColumnDefinition)));

        public ColumnDefinition Column
        {
            get { return (ColumnDefinition) GetValue(ColumnProperty); }
            set { SetValue(ColumnProperty, value); }
        }

        // ROW
        public static readonly DependencyProperty RowProperty =
            DependencyProperty.Register("Row", typeof (RowDefinition), typeof (CloseablePanel), new PropertyMetadata(default(RowDefinition)));

        public RowDefinition Row
        {
            get { return (RowDefinition) GetValue(RowProperty); }
            set { SetValue(RowProperty, value); }
        }

        // MAIN OBJECT
        public static readonly DependencyProperty MainObjectProperty =
            DependencyProperty.Register("MainObject", typeof (FrameworkElement), typeof (CloseablePanel), new PropertyMetadata(default(FrameworkElement)));

        public FrameworkElement MainObject
        {
            get { return (FrameworkElement) GetValue(MainObjectProperty); }
            set { SetValue(MainObjectProperty, value); }
        }

        // SPLITTER
        public static readonly DependencyProperty SplitterProperty =
            DependencyProperty.Register("Splitter", typeof (GridSplitter), typeof (CloseablePanel), new PropertyMetadata(default(GridSplitter), PropertyChangedCallback));

        private static void PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            CloseablePanel panel = dependencyObject as CloseablePanel;
            GridSplitter splitter = dependencyPropertyChangedEventArgs.NewValue as GridSplitter;
            splitter.DragStarted += (sender, args) => panel.Activate();
        }

        public GridSplitter Splitter
        {
            get { return (GridSplitter) GetValue(SplitterProperty); }
            set { SetValue(SplitterProperty, value); }
        }


        public CloseablePanel()
        {
            IsActive = true;    // По умолчанию, открыта панелька

            InitializeComponent();
        }

        public bool IsActive { get; set; }

        public void Activate()
        {
            if (Visibility == Visibility.Collapsed && (_saveWidth.HasValue || _saveHeight.HasValue))
            {
                Splitter.Visibility = Visibility = Visibility.Visible;

                if (Column != null)
                {
                    Column.Width = _saveWidth.Value;
                    Column.MinWidth = _saveMinWidth.Value;

                    _saveWidth = null;
                    _saveMinWidth = null;
                }
                else if (Row != null)
                {
                    Row.Height = _saveHeight.Value;
                    Row.MinHeight = _saveMinHeight.Value;

                    _saveHeight = null;
                    _saveMinHeight = null;
                }
            }

            SetFocus();
            IsActive = true;
        }

        public void Collapse()
        {
            Splitter.Visibility = Visibility = Visibility.Collapsed;
            IsActive = false;
            
            if (Column != null)
            {
                _saveWidth = Column.Width;
                _saveMinWidth = Column.MinWidth;

                Column.Width = GridLength.Auto;
                Column.MinWidth = 0.0;
            }
            else if (Row != null)
            {
                _saveHeight = Row.Height;
                _saveMinHeight = Row.MinHeight;

                Row.Height = GridLength.Auto;
                Row.MinHeight = 0.0;
            }
        }

        // Нужно выбрать какой-то главный эдемент, на который будет выбираться фокус,
        // Тут устанавливаем фокус именно в него
        private void SetFocus()
        {
            if (MainObject != null)
                Dispatcher.BeginInvoke(new Action(() => MainObject.Focus())); // official hack! BAD!
            else if (Content != null)
                Dispatcher.BeginInvoke(new Action(() => Focus())); // official hack! BAD!
        }

        // Тык на заголовок
        private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SetFocus();
        }

        private GridLength? _saveWidth; // Сохраненная длина столбца, чтобы потом восстановить прежнее значение
        private double? _saveMinWidth;
        private GridLength? _saveHeight; // Сохраненная длина строки
        private double? _saveMinHeight;

        // Кнопка закрытия
        private void HeaderCloseButton_Click(object sender, MouseButtonEventArgs e)
        {
            Collapse();
        }
    }
}
