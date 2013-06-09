using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Translator
{
    public interface ProjectExplorerView
    {
        void UpdateTree();
        void FocusOnFile(CodeFile file);
        void Rename(CodeFile file);
        void Copy(CodeFile file);
        void Move(CodeFile file);
    }
}
