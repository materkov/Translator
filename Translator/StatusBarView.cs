using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Translator
{
    interface StatusBarView
    {
        void LogString(string message);
        void LogWithBinding(string key);

        void ShowPopup(string message);
        void HidePopup(string message);
    }
}
