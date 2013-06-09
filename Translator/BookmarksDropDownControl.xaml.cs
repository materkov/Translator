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
using ICSharpCode.AvalonEdit.Document;
using Xceed.Wpf.Toolkit;

namespace Translator
{
    /// <summary>
    /// Interaction logic for BookmarksDropDownControl.xaml
    /// </summary>
    public partial class BookmarksDropDownControl : UserControl
    {
        public BookmarksDropDownControl()
        {
            InitializeComponent();
        }

        public SplitButton ParentButton { get; set; }
        private CodeEditorView _codeEditor;

        public void Init(CodeEditorView codeEditor)
        {
            _codeEditor = codeEditor;
            BookmarksConverter._codeEditor = codeEditor;
        }

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool) e.NewValue)
            {
                ParentButton.Focus();
                //Dispatcher.BeginInvoke(new Action(() => listView1.Focus())); // official hack! BAD!
            }

            if(_codeEditor.CurrentCodeFile == null)
                return;

            _codeEditor.CurrentCodeFile.Bookmarks.Sort((b1, b2) =>
            {
                if (b1.Key > b2.Key)
                    return 1;
                else if (b1.Key == b2.Key)
                    return 0;
                else
                    return -1;
            });

            listView1.DataContext = null;
            if (_codeEditor.CurrentCodeFile.Bookmarks.Count > 0)
            {
                listView1.Visibility = Visibility.Visible;
                EmptyTextBlock.Visibility = Visibility.Collapsed;

                listView1.DataContext = _codeEditor.CurrentCodeFile.Bookmarks;
            }
            else
            {
                listView1.Visibility = Visibility.Collapsed;
                EmptyTextBlock.Visibility = Visibility.Visible;
            }
        }

        private void Item_MouseClick(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement item = sender as FrameworkElement;
            if (item.DataContext != null)
            {
                CodeFile.Bookmark b = item.DataContext as CodeFile.Bookmark;
                _codeEditor.SetCaretTo(_codeEditor.CurrentCodeFile, b.Offset);
            }

            ParentButton.IsOpen = false;
            e.Handled = true;
        }

        private void Item_Delete(object sender, RoutedEventArgs e)
        {
            Button item = sender as Button;
            if (item.DataContext != null)
            {
                CodeFile.Bookmark b = item.DataContext as CodeFile.Bookmark;
                _codeEditor.CurrentCodeFile.DeleteBookmark(b.Line);
                _codeEditor.UpdateBookmarks();
            }

            ParentButton.IsOpen = false;
            e.Handled = true;
        }

        private void Item_Selected(object sender, RoutedEventArgs e)
        {
            ListViewItem item = sender as ListViewItem;
            if (item.DataContext != null)
            {
                CodeFile.Bookmark b = item.DataContext as CodeFile.Bookmark;
                _codeEditor.SetCaretTo(_codeEditor.CurrentCodeFile, b.Offset);
            }
            ParentButton.IsOpen = false;
            e.Handled = true;
        }
    }

    class BookmarksConverter : IValueConverter
    {
        public static CodeEditorView _codeEditor;

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            CodeFile.Bookmark b = value as CodeFile.Bookmark;
            if (b != null)
            {
                DocumentLine line = _codeEditor.CurrentCodeFile.Document.GetLineByNumber(b.Line);
                string str = _codeEditor.CurrentCodeFile.Text.Substring(line.Offset, line.Length);

                return str;
            }
            else
                return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    class BookmarksKeyConverter : IValueConverter
    {
        public static CodeEditorView _codeEditor;

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            CodeFile.Bookmark b = value as CodeFile.Bookmark;
            if (b != null && b.Key < 10)
                return "Alt+" + b.Key;
            else
                return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
