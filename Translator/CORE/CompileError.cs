using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using ICSharpCode.AvalonEdit.Document;
using Translator.Annotations;

namespace Translator
{
    public class CompileError : INotifyPropertyChanged
    {
        public CompileError(Type type, CodeFile file, int beginOffset, int endOffset)
        {
            ErrorType = type;
            File = file;

            // Чтоб менялись все позиции в списке ошибок
            file.TextChanged += (sender, args) =>
                {
                    OnPropertyChanged("BeginLine");
                    OnPropertyChanged("EndLine");
                    OnPropertyChanged("BeginPosition");
                    OnPropertyChanged("EndPosition");
                    OnPropertyChanged("BeginOffset");
                    OnPropertyChanged("EndOffset");
                    OnPropertyChanged("ISActive");
                };   

            _anchor1 = file.Document.CreateAnchor(beginOffset);
            _anchor1.MovementType = AnchorMovementType.AfterInsertion;
            
            _anchor2 = file.Document.CreateAnchor(endOffset);
            _anchor2.MovementType = AnchorMovementType.BeforeInsertion;
        }

        public static bool NeedFullDiagnostic(Type type)
        {
            //return true;

            bool res = !IDEState.Get().RecentErrors.ContainsKey(type);
            // Полная диагностика нужна, если такой ошибки еще не было,
            // т.е. ее нету в списке последних
            return !IDEState.Get().RecentErrors.ContainsKey(type);
        }

        public string GetDiagnostic()
        {
            string key;

            switch (ErrorType)
            {
                case Type.Type0: key = NeedFullDiagnostic(Type.Type0) ? "Err_0_full" : "Err_0_short"; break;
                case Type.Type1: key = NeedFullDiagnostic(Type.Type1) ? "Err_1_full" : "Err_1_short"; break;
                case Type.Type2: key = NeedFullDiagnostic(Type.Type2) ? "Err_2_full" : "Err_2_short"; break;
                case Type.Type3: key = NeedFullDiagnostic(Type.Type3) ? "Err_3_full" : "Err_3_short"; break;
                case Type.Type4: key = NeedFullDiagnostic(Type.Type4) ? "Err_4_full" : "Err_4_short"; break;
                case Type.Type5: key = NeedFullDiagnostic(Type.Type5) ? "Err_5_full" : "Err_5_short"; break;
                case Type.Type6: key = NeedFullDiagnostic(Type.Type6) ? "Err_6_full" : "Err_6_short"; break;
                case Type.Type7: key = NeedFullDiagnostic(Type.Type7) ? "Err_7_full" : "Err_7_short"; break;
                case Type.Type8: key = NeedFullDiagnostic(Type.Type8) ? "Err_8_full" : "Err_8_short"; break;
                case Type.Type9: key = NeedFullDiagnostic(Type.Type9) ? "Err_9_full" : "Err_9_short"; break;
                default: return "";
            }

            return LocalString.Get(key);
        }

        public enum Type
        {
            Type0, Type1, Type2, Type3, Type4, Type5, Type6, Type7, Type8, Type9
        }
        
        public Type ErrorType { get; private set; }

        public int BeginLine
        {
            get { return IsActive ? _anchor1.Location.Line : 0; }
        }

        public int EndLine
        {
            get { return IsActive ? _anchor2.Location.Line : 0; }
        }

        public int BeginPosition
        {
            get { return IsActive ? _anchor1.Location.Column : 0; }
        }

        public int EndPosition
        {
            get { return IsActive ? _anchor2.Location.Column : 0; }
        }

        public int BeginOffset
        {
            get { return IsActive ? _anchor1.Offset : 0; }
        }

        public int EndOffset
        {
            get { return IsActive ? _anchor2.Offset : 0; }
        }

        public CodeFile File { get; private set; }

        public bool IsActive
        {
            get { return !_anchor1.IsDeleted && !_anchor2.IsDeleted && (_anchor2.Offset > _anchor1.Offset); }
        }

        private TextAnchor _anchor1;
        private TextAnchor _anchor2;

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
