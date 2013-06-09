using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace Translator
{
    class CodeProjectManager
    {
        // Singleton
        private CodeProjectManager() { }
        private static CodeProjectManager _instance = new CodeProjectManager();
        public static CodeProjectManager Instance { get { return _instance; } }

        private CodeEditorView _codeEditorView;
        private ErrorsListView _errorsListView;
        private CodeProject _current;
        private MainWindowView _mainWindowView;

        public void Init(CodeEditorView codeEditorView, ErrorsListView errorsListView, MainWindowView mainWindowView)
        {
            _codeEditorView = codeEditorView;
            _errorsListView = errorsListView;
            _mainWindowView = mainWindowView;
        }

        public void CompileProject(CodeProject project)
        {
            Compiler.Instance.ApplyErrorsToRecent();

            _errorsListView.ClearAllErrors();
            //_errorsListView.ErrorsListContainter.Clear();

            if (project == null || project.Files == null)
                return;

            // Тупо компилируем все файлы, какие есть
            foreach (CodeFile file in project.Files)
            {
                file.Compile();
                _errorsListView.AddErrorsCollection(file.ErrorsList);
            }

            // Чтобы подсветить выделить ошибки
            _codeEditorView.Redraw();

            Logger.LogWithBinding("Status_Project_Compiled");
        }

        public void CompileFile(CodeProject project, CodeFile file)
        {
            Compiler.Instance.ApplyErrorsToRecent();
            _errorsListView.ClearAllErrors();
            //_errorsListView.ErrorsListContainter.Clear();

            if (file == null || project == null || project.Files == null)
                return;

            // Тупо компилируем все файлы, какие есть
            foreach (CodeFile _file in project.Files) 
                if (_file.ErrorsList != null)
                    _file.ErrorsList.Clear();

            file.Compile();
            _errorsListView.AddErrorsCollection(file.ErrorsList);

            // Чтобы подсветить выделить ошибки
            _codeEditorView.Redraw();

            Logger.LogWithBinding("Status_File_Compiled");
        }

        public CodeProject Current
        {
            get { return _current; }
            set
            {
                _current = value;
                OnCurrentProjectChanged(new CurrentProjectChangedArgs(_current, value)); 
            }
        }

        public void Load(string path)
        {
            try
            {
                // Закрыть предыдущий
                if (Current != null)
                {
                    CloseCurrent();
                }

                // Открыть новый
                Current = CodeProject.Load(path);

                // Поставить фокус на проджект эксплорер
                //_mainWindowView.ActivateProjectExplorer();

                Logger.LogWithBinding("Status_ProjectOpened");
            }
            catch (ParsingProjectException ex)
            {
                ErrorWindow err = new ErrorWindow
                {
                    ShortDesc = LocalString.Get("Error_OpenProject_short"),
                    FullDesc = LocalString.Get("Error_OpenProject_full"),
                    Exception = ex
                };
                err.ShowDialog();
            }
            catch (FileNotFoundException ex)
            {
                ErrorWindow err = new ErrorWindow
                {
                    ShortDesc = LocalString.Get("Error_OpenProject_short"),
                    FullDesc = LocalString.Get("Error_OpenProject_full"),
                    Exception = ex
                };
                err.ShowDialog();
            }
        }

        public void SaveCurrent()
        {
            try
            {
                if (Current != null)
                    Current.Save();
            }
            catch (SaveProjectException ex)
            {
                ErrorWindow err = new ErrorWindow
                {
                    ShortDesc = LocalString.Get("Error_SaveProject_short"),
                    FullDesc = LocalString.Get("Error_SaveProject_full"),
                    Exception = ex
                };
                err.ShowDialog();
            }
        }

        public void CloseCurrent()
        {
            try
            {
                if (Current != null)
                {
                    Current.Close();
                    Current = null;

                    Logger.LogWithBinding("Status_ProjectClosed");
                }
            }
            catch (SaveProjectException ex)
            {
                ErrorWindow err = new ErrorWindow
                {
                    ShortDesc = LocalString.Get("Error_CloseProject_short"),
                    FullDesc = LocalString.Get("Error_CloseProject_full"),
                    Exception = ex
                };
                err.ShowDialog();
            }
        }

        public event CurrentProjectArgsHandler CurrentProjectChanged;
        private void OnCurrentProjectChanged(CurrentProjectChangedArgs changedArgs)
        {
            CurrentProjectArgsHandler handler = CurrentProjectChanged;
            if (handler != null) handler(this, changedArgs);
        }
    }

    delegate void CurrentProjectArgsHandler(object sender, CurrentProjectChangedArgs changedArgs);
    class CurrentProjectChangedArgs : EventArgs
    {
        public CodeProject Old { get; private set; }
        public CodeProject New { get; private set; }

        public CurrentProjectChangedArgs(CodeProject oldProject, CodeProject newProject)
        {
            Old = oldProject;
            New = newProject;
        }
    }
}
