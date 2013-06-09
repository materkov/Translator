using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Translator
{
    class Compiler
    {
        private Compiler()
        {
            App.Current.Exit += (sender, args) => ApplyErrorsToRecent();
        }

        private static Compiler _compiler = new Compiler();
        public static Compiler Instance { get { return _compiler; } }

        private Dictionary<CompileError.Type, int> _recent = new Dictionary<CompileError.Type, int>();

        public List<CompileError> Compile(CodeFile file)
        {
            // что есть сейчас
            //_recent.Clear();

            string text = file.Text;
            var errors = new List<CompileError>();

            for (int i = 0; i < text.Length; i++ )
            {
                Char c = text[i];

                if(Char.IsDigit(c))
                {
                    CompileError.Type type = CompileError.Type.Type0;

                    if (c == '0') type = CompileError.Type.Type0;
                    else if(c == '1') type = CompileError.Type.Type1;
                    else if(c == '2') type = CompileError.Type.Type2;
                    else if(c == '3') type = CompileError.Type.Type3;
                    else if(c == '4') type = CompileError.Type.Type4;
                    else if(c == '5') type = CompileError.Type.Type5;
                    else if(c == '6') type = CompileError.Type.Type6;
                    else if(c == '7') type = CompileError.Type.Type7;
                    else if(c == '8') type = CompileError.Type.Type8;
                    else if(c == '9') type = CompileError.Type.Type9;

                    errors.Add(new CompileError(type, file, i, i+1));

                    if (_recent.ContainsKey(type))
                        _recent[type] += 1;
                    else
                        _recent[type] = 1;
                }
                else
                {
                    // No error
                }
            }

            OnCompiled();

            return errors;
        }

        public void ApplyErrorsToRecent()
        {
            if (_recent == null) return;

            foreach (KeyValuePair<CompileError.Type, int> pair in _recent)
            {
                if (IDEState.Get().RecentErrors.ContainsKey(pair.Key))
                    IDEState.Get().RecentErrors[pair.Key] += pair.Value;
                else
                    IDEState.Get().RecentErrors.Add(pair.Key, pair.Value);
            }
        }

        public event EventHandler<EventArgs> Compiled;
        protected virtual void OnCompiled()
        {
            if (Compiled != null) Compiled(this, new EventArgs());
        }
    }
}
