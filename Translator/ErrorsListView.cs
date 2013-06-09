using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Translator
{
    public interface ErrorsListView
    {
        void AddErrorsCollection(IEnumerable<CompileError> list);
        void ClearAllErrors();
        IEnumerable<CompileError> ErrorsList { get; }

        void SelectError(CompileError err, bool setFocus, bool setEditorFocus);
        void ScrollToError(CompileError err);
    }
}
