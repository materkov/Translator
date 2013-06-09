using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Translator
{
    static class Logger
    {
        private static StatusBarView _statusBar;
        public static void Init(StatusBarView statusBar)
        {
            _statusBar = statusBar;
        }

        public static void LogString(string message)
        {
            _statusBar.LogString(message);
        }

        public static void LogWithBinding(string key)
        {
            _statusBar.LogWithBinding(key);
        }

        public static void ShowPopup(string message)
        {
            _statusBar.ShowPopup(message);
        }

        public static void HidePopup(string message)
        {
            _statusBar.HidePopup(message);
        }
    }
}
