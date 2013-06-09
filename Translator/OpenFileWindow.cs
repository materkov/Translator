using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Translator.Annotations;

namespace Translator
{
    class OpenFileWindow : OpenFileWindowBase
    {
        public OpenFileWindow(Window owner)
            :base(owner)
        {
            base.Title = LocalString.Get("OpenFileWindow_Title");
        }

        private readonly string DefaultExtension = CodeFile.DefaultExt;

        public override string ExtensionFilter
        {
            get { return "*" + DefaultExtension; }
        }

        protected override bool CheckTemplate(string filename)
        {
            return System.IO.Path.GetExtension(filename) == DefaultExtension;
        }

        protected override string GetPreview(string path)
        {
            return CodeFile.GetPreview(path);
        }
    }
}
