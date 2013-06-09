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
using Xceed.Wpf.Toolkit;

namespace Translator
{
    /// <summary>
    /// Interaction logic for ColorDropDownControl.xaml
    /// </summary>
    public partial class ColorDropDownControl : UserControl
    {
        public ColorDropDownControl()
        {
            InitializeComponent();
        }

        public SplitButton ParentButton { get; set; }
        public SolidColorBrush Selected { get; private set; }

        private CodeEditorView _codeEditor;
        private SolidColorBrush _transparentBrush = new SolidColorBrush(Colors.Transparent);

        public void Init(CodeEditorView codeEditor)
        {
            Selected = _transparentBrush;

            _codeEditor = codeEditor;
            BookmarksConverter._codeEditor = codeEditor;
        }

        private void Color_OnClick(object sender, MouseButtonEventArgs e)
        {
            Rectangle rect = (sender as Grid).Children[0] as Rectangle;
            if (rect == null)
            {
                Selected = _transparentBrush;
            }
            else
            {
                SolidColorBrush brush = rect.Fill as SolidColorBrush;
                Selected = brush;
            }

            e.Handled = true;
            ParentButton.IsOpen = false;
        }

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                ParentButton.Focus();
                Selected = null;
            }

            if (_codeEditor.CurrentCodeFile == null)
                return;
        }
    }
}
