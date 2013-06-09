using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Translator
{
    public interface MainWindowView
    {
        void ActivateProjectExplorer();
        void ActivateErrors();
        void ActivateContents();
        void ActivateCode();
    }
}
