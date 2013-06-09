using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;

namespace Translator
{
    public class StringDictionary : Dictionary<string, string>
    {
        public new string this[string key]
        {
            get
            {
                // Если не найдено, вернем сам ключ с добавлением доллара, чтоб сразу видно было что косяк
                string res;
                return TryGetValue(key, out res) ? res : "$" + key + "$";
            }
        }
    }
    
    public interface LocalStringDictionary
    {
        StringDictionary V { get; set; }
    }

    public class LocalString : INotifyPropertyChanged
    {
        public static string Get(string key)
        {
            return (Application.Current.Resources["LocalString"] as LocalString).Dict[key];
        }

        public LocalString()
        {
            var state = Application.Current.Resources["IDEState"] as IDEState;
            if (state == null)
                SetLanguage(0);
            else
            {
                SetLanguage(state.CurrentLanguage);

                state.PropertyChanged += (sender, args) =>
                    {
                        if (args.PropertyName == "CurrentLanguage")
                        {
                            SetLanguage(state.CurrentLanguage);
                            OnPropertyChanged("Dict");
                        }
                    };
            }
        }

        private LocalStringDictionary _s = null;

        private void SetLanguage(int key)
        {
            if (key == 0)
                _s = new LocalStringDictionaryEng();
            else if (key == 1)
                _s = new LocalStringDictionaryRus();
        }

        public StringDictionary Dict
        {
            get { return _s.V; }
            set { _s.V = value;}
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
