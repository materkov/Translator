using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Translator.Annotations;

namespace Translator
{
    public class Pulser : INotifyPropertyChanged
    {
        public object Pulse { get; set; }

        public void DoPulse()
        {
            OnPropertyChanged("Pulse");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public interface CodeEditorView : INotifyPropertyChanged
    {
        void Redraw();

        void SetCaretTo(CodeFile file, int offset);
        void SetFocusInCheckboxes();
        void SetFocusInComments();
        void ScrollTo(CodeFile file, int offset);
        void HighlightError(CodeFile file, CompileError error);
        void UnHighlightError();

        void ForceFocus();

        void SetAnotherErrorColor(CompileError.Type type);
        void UnSetAnotherErrorColor(CompileError.Type type);
        bool GetIfAnotherErrorColor(CompileError.Type type);

        int Line { get; }
        int Column { get; }
        bool IsEditorFocused { get; }

        CodeFile CurrentCodeFile { get; }
        IEnumerable<CodeFile> OpenedFiles { get; }

        event EventHandler<FileArgs> FileOpened;
        event EventHandler<FileArgs> FileClosed;

        event EventHandler CurrentFileChanged;  // Изменилось СОДЕРЖИМОЕ ФАЙЛА!
        event EventHandler ActiveFileChanged;   // Поменялся сам выбранный в настоящий момент файл

        void UpdateBookmarks();

        void OpenTab(CodeFile file, bool setFocus);
        void RemoveFromTabs(CodeFile file);
        void CloseAllTabs();
        void CloseCurrentTab();
        bool IsFileOpened(CodeFile file);

        void AddColorSegment(SolidColorBrush color);
        void EraseColorSegment();
        void ClearAllColorSegments();
    }

    public class FileArgs : EventArgs
    {
        private readonly CodeFile _file;

        public FileArgs(CodeFile file)
        {
            _file = file;
        }

        public CodeFile File
        {
            get { return _file; }
        }
    }
}
