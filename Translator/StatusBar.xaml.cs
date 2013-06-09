using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Interaction logic for StatusBar.xaml
    /// </summary>
    public partial class StatusBar : UserControl, StatusBarView
    {
        public StatusBar()
        {
            InitializeComponent();
        }

        private LinkedList<string> _log = new LinkedList<string>();

        public void LogString(string message)
        {
            // Удаляем предыдущий биндинг
            logItem.ClearValue(TextBlock.TextProperty);
            _lastBinding = null;

            logItem.Text = _lastLogMessage = message;
        }

        public void LogWithBinding(string key)
        {
            // Удаляем предыдущий биндинг
            logItem.ClearValue(TextBlock.TextProperty);

            // Ставим новый
            _lastBinding = new Binding
            {
                Source = Application.Current.Resources["LocalString"], 
                Path = new PropertyPath("Dict[" + key + "]")
            };
            logItem.SetBinding(TextBlock.TextProperty, _lastBinding);
            _lastLogMessage = null;
        }

        private LinkedList<string> _lastPopupMessages = new LinkedList<string>();
        private string _lastLogMessage = null;
        private Binding _lastBinding = null;

        public void ShowPopup(string message)
        {
            // Удаляем предыдущий биндинг
            if (_lastBinding != null)
            {
                logItem.ClearValue(TextBlock.TextProperty);
            }
            else
            {
                // Если не было биндинга
                _lastLogMessage = logItem.Text;
            }

            _lastPopupMessages.AddFirst(message);
            logItem.Text = message;
        }

        public void HidePopup(string message)
        {
            bool a = _lastPopupMessages.Remove(message);
            Debug.Assert(a);    // Удостовериться, что что-то действительно удалено

            if (_lastPopupMessages.Count > 0)
                logItem.Text = _lastPopupMessages.First();
            else
            {
                if (_lastBinding != null && _lastLogMessage == null)
                {
                    logItem.SetBinding(TextBlock.TextProperty, _lastBinding);
                }
                else
                    logItem.Text = _lastLogMessage;
            }
        }
    }
}
