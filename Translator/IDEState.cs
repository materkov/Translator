using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Xml;
using System.Xml.Serialization;

namespace Translator
{
    public class IDEState: INotifyPropertyChanged
    {
        public static IDEState Get()
        {
            return Application.Current.Resources["IDEState"] as IDEState;
        }

        public IDEState()
        {
            
        }

        private void Configure()
        {
            FocusedHeaderBrushB = new SolidColorBrush(FocusedHeaderColorB);
            FocusedHeaderBrushF = new SolidColorBrush(FocusedHeaderColorF);
            NotFocusedHeaderBrushB = new SolidColorBrush(NotFocusedHeaderColorB);
            NotFocusedHeaderBrushF = new SolidColorBrush(NotFocusedHeaderColorF);
            MainWindowBackgroundBrush = new SolidColorBrush(MainWindowBackgroundColor);
            BookmarkBrushB = new SolidColorBrush(BookmarkColorB);
            BookmarkBrushF = new SolidColorBrush(BookmarkColorF);

            EditorColor_CurrentLineBrush = new SolidColorBrush(EditorColor_CurrentLine);
            EditorColor_MouseHoverTabFocusedBrush = new SolidColorBrush(EditorColor_MouseHoverTabFocused);
            EditorColor_MouseHoverTabNotFocusedBrush = new SolidColorBrush(EditorColor_MouseHoverTabNotFocused);


            //FocusedElementBrush = new SolidColorBrush(FocusedElementColor);
            //NotFocusedElementBrush = new SolidColorBrush(NotFocusedElementColor);
        }

        public bool IsFirstTime
        {
            get { return _config.IsFirstTime; } 
        }

        public int NextProjectNumber
        {
            get { return _config.NextProjectNumber; }
            set
            {
                _config.NextProjectNumber = value;
                OnPropertyChanged("LastProjectNumber");
            }
        }

        public WindowState WindowState
        {
            get { return _config.WindowState; }
            set
            {
                _config.WindowState = value;
                OnPropertyChanged("WindowState");
            }
        }

        public int WindowWidth
        {
            get { return _config.WindowWidth; }
            set
            {
                _config.WindowWidth = value;
                OnPropertyChanged("WindowWidth");
            }
        }

        public int WindowHeight
        {
            get { return _config.WindowHeight; }
            set
            {
                _config.WindowHeight = value;
                OnPropertyChanged("WindowHeight");
            }
        }

        public int WindowTop
        {
            get { return _config.WindowTop; }
            set 
            {
                _config.WindowTop = value;
                OnPropertyChanged("WindowTop");
            }
        }

        public int WindowLeft
        {
            get { return _config.WindowLeft; }
            set
            {
                _config.WindowLeft = value;
                OnPropertyChanged("WindowLeft");
            }
        }

        public int HotkeysWindowWidth
        {
            get { return _config.HotkeysWindowWidth; }
            set
            {
                _config.HotkeysWindowWidth = value;
                OnPropertyChanged("HotkeysWindowWidth");
            }
        }

        public int HotkeysWindowHeight
        {
            get { return _config.HotkeysWindowHeight; }
            set
            {
                _config.HotkeysWindowHeight = value;
                OnPropertyChanged("HotkeysWindowHeight");
            }
        }

        public int HotkeysWindowTop
        {
            get { return _config.HotkeysWindowTop; }
            set
            {
                _config.HotkeysWindowTop = value;
                OnPropertyChanged("HotkeysWindowTop");
            }
        }

        public int HotkeysWindowLeft
        {
            get { return _config.HotkeysWindowLeft; }
            set
            {
                _config.HotkeysWindowLeft = value;
                OnPropertyChanged("HotkeysWindowLeft");
            }
        }

        public double ProjectExplorerColumn_Width
        {
            get { return _config.ProjectExplorerColumn_Width; }
            set
            {
                _config.ProjectExplorerColumn_Width = value;
                OnPropertyChanged("ProjectExplorerColumn_Width");
            }
        }

        public double ErrorsRow_Height
        {
            get { return _config.ErrorsRow_Height; }
            set
            {
                _config.ErrorsRow_Height = value;
                OnPropertyChanged("ErrorsRow_Height");
            }
        }

        public double BookmarksRow_Height
        {
            get { return _config.BookmarksRow_Height; }
            set
            {
                _config.BookmarksRow_Height = value;
                OnPropertyChanged("BookmarksRow_Height");
            }
        }

        public double ContentsColumn_Width
        {
            get { return _config.ContentsColumn_Width; }
            set
            {
                _config.ContentsColumn_Width = value;
                OnPropertyChanged("ContentsColumn_Width");
            }
        }

        public bool ProjectExplorer_Visible
        {
            get { return _config.ProjectExplorer_Visible; }
            set
            {
                _config.ProjectExplorer_Visible = value;
                OnPropertyChanged("ProjectExplorer_Visible");
            }
        }

        public bool Errors_visible
        {
            get { return _config.Errors_visible; }
            set
            {
                _config.Errors_visible = value;
                OnPropertyChanged("Errors_visible");
            }
        }

        public bool Bookmarks_visible
        {
            get { return _config.Bookmarks_visible; }
            set
            {
                _config.Bookmarks_visible = value;
                OnPropertyChanged("Bookmarks_visible");
            }
        }

        public bool Contents_Visible
        {
            get { return _config.Contents_Visible; }
            set
            {
                _config.Contents_Visible = value;
                OnPropertyChanged("Contents_Visible");
            }
        }

        public int StructureWindow_Width
        {
            get { return _config.StructureWindow_Width; }
            set
            {
                _config.StructureWindow_Width = value;
                OnPropertyChanged("StructureWindow_Width");
            }
        }

        public int StructureWindow_Height
        {
            get { return _config.StructureWindow_Height; }
            set
            {
                _config.StructureWindow_Height = value;
                OnPropertyChanged("StructureWindow_Height");
            }
        }

        public int StructureWindow_Top
        {
            get { return _config.StructureWindow_Top; }
            set
            {
                _config.StructureWindow_Top = value;
                OnPropertyChanged("StructureWindow_Top");
            }
        }

        public int StructureWindow_Left
        {
            get { return _config.StructureWindow_Left; }
            set
            {
                _config.StructureWindow_Left = value;
                OnPropertyChanged("StructureWindow_Left");
            }
        }

        public WindowState StructureWindow_WindowState
        {
            get { return _config.WindowState; }
            set
            {
                _config.WindowState = value;
                OnPropertyChanged("WindowState");
            }
        }

        public int ColorSegmentCurrent
        {
            get { return _config.ColorSegmentCurrent; }
            set 
            { 
                _config.ColorSegmentCurrent = value;
                OnPropertyChanged("ColorSegmentCurrent");
            }
        }

        public ObservableCollection<string> FavoriteDirs
        {
            get { return _config.FavoriteDirs; }
            set 
            { 
                _config.FavoriteDirs = value;
                OnPropertyChanged("FavoriteDirs");
            }
        } 

        public ObservableCollection<string> RecentProjects
        {
            get { return _config.RecentProjects; }
            set 
            { 
                _config.RecentProjects = value;
                OnPropertyChanged("RecentProjects");
            }
        } 

        public int CurrentLanguage
        {
            get { return _config.CurrentLanguage; }
            set
            {
                _config.CurrentLanguage = value;
                OnPropertyChanged("CurrentLanguage");
            }
        }

        public Color EditorColor_ErrorB
        {
            get { return _config.EditorColor_ErrorB;  }
            set
            {
                _config.EditorColor_ErrorB = value; 
                OnPropertyChanged("EditorColor_ErrorB");
            }
        }

        public Color EditorColor_ErrorF
        {
            get { return _config.EditorColor_ErrorF; }
            set
            {
                _config.EditorColor_ErrorF = value;
                OnPropertyChanged("EditorColor_ErrorF");
            }
        }

        public Color EditorColor_SpecialErrorB
        {
            get { return _config.EditorColor_SpecialErrorB; }
            set
            {
                _config.EditorColor_SpecialErrorB = value;
                OnPropertyChanged("EditorColor_SpecialErrorB");
            }
        }

        public Color EditorColor_SpecialErrorF
        {
            get { return _config.EditorColor_SpecialErrorF; }
            set
            {
                _config.EditorColor_SpecialErrorF = value;
                OnPropertyChanged("EditorColor_SpecialErrorF");
            }
        }

        public Color EditorColor_PopupErrorB
        {
            get { return _config.EditorColor_PopupErrorB; }
            set
            {
                _config.EditorColor_PopupErrorB = value;
                OnPropertyChanged("EditorColor_PopupErrorB");
            }
        }

        public Color EditorColor_PopupErrorF
        {
            get { return _config.EditorColor_PopupErrorF; }
            set
            {
                _config.EditorColor_PopupErrorF = value;
                OnPropertyChanged("EditorColor_PopupErrorF");
            }
        }

        public Color EditorColor_CurrentLine
        {
            get { return _config.EditorColor_CurrentLine; }
            set
            {
                _config.EditorColor_CurrentLine = value;
                EditorColor_CurrentLineBrush.Color = value;

                OnPropertyChanged("EditorColor_CurrentLine");
                OnPropertyChanged("EditorColor_CurrentLineBrush");
            }
        }

        public SolidColorBrush EditorColor_CurrentLineBrush { get; private set; }

        public Color EditorColor_MouseHoverTabFocused
        {
            get { return _config.EditorColor_MouseHoverTabFocused; }
            set
            {
                _config.EditorColor_MouseHoverTabFocused = value;
                EditorColor_MouseHoverTabFocusedBrush.Color = value;

                OnPropertyChanged("EditorColor_MouseHoverTabFocused");
                OnPropertyChanged("EditorColor_MouseHoverTabFocusedBrush");
            }
        }

        public SolidColorBrush EditorColor_MouseHoverTabFocusedBrush { get; private set; }

        public Color EditorColor_MouseHoverTabNotFocused
        {
            get { return _config.EditorColor_MouseHoverTabNotFocused; }
            set
            {
                _config.EditorColor_MouseHoverTabNotFocused = value;
                EditorColor_MouseHoverTabNotFocusedBrush.Color = value;

                OnPropertyChanged("EditorColor_MouseHoverTabNotFocused");
                OnPropertyChanged("EditorColor_MouseHoverTabNotFocusedBrush");
            }
        }

        public SolidColorBrush EditorColor_MouseHoverTabNotFocusedBrush { get; private set; }

        /*public Color FocusedElementColor
        {
            get { return _config.FocusedElementColor; }
            set
            {
                _config.FocusedElementColor = value;
                FocusedElementBrush.Color = value;
                OnPropertyChanged("FocusedElementColor");
                OnPropertyChanged("FocusedElementBrush");
            }
        }

        public SolidColorBrush FocusedElementBrush { get; private set; }

        public Color NotFocusedElementColor
        {
            get { return _config.NotFocusedElementColor; }
            set
            {
                _config.NotFocusedElementColor = value;
                FocusedElementBrush.Color = value;
                OnPropertyChanged("NotFocusedElementColor");
                OnPropertyChanged("NotFocusedElementBrush");
            }
        }

        public SolidColorBrush NotFocusedElementBrush { get; private set; }*/

        public Color MainWindowBackgroundColor 
        {
            get { return _config.MainWindowBackgroundColor; }
            set
            {
                _config.MainWindowBackgroundColor = value;
                MainWindowBackgroundBrush.Color = value;

                OnPropertyChanged("MainWindowBackgroundColor");
                OnPropertyChanged("MainWindowBackgroundBrush");
            }
        }

        public SolidColorBrush MainWindowBackgroundBrush { get; private set; }

        public Color FocusedHeaderColorB
        {
            get { return _config.FocusedHeaderColorB; }
            set
            {
                _config.FocusedHeaderColorB = value;
                FocusedHeaderBrushB.Color = value;
                
                OnPropertyChanged("FocusedHeaderColorB");
                OnPropertyChanged("FocusedHeaderBrushB");
            }
        }

        public SolidColorBrush FocusedHeaderBrushB { get; private set; }

        public Color FocusedHeaderColorF
        {
            get { return _config.FocusedHeaderColorF; }
            set
            {
                _config.FocusedHeaderColorF = value;
                FocusedHeaderBrushF.Color = value;

                OnPropertyChanged("FocusedHeaderColorF");
                OnPropertyChanged("FocusedHeaderBrushF");
            }
        }
        public SolidColorBrush FocusedHeaderBrushF { get; private set; }

        public Color NotFocusedHeaderColorB
        {
            get { return _config.NotFocusedHeaderColorB; }
            set
            {
                _config.NotFocusedHeaderColorB = value;
                NotFocusedHeaderBrushB.Color = value;

                OnPropertyChanged("NotFocusedHeaderColorB");
                OnPropertyChanged("NotFocusedHeaderBrushB");
            }
        }

        public SolidColorBrush NotFocusedHeaderBrushB { get; private set; }

        public Color NotFocusedHeaderColorF
        {
            get { return _config.NotFocusedHeaderColorF; }
            set
            {
                _config.NotFocusedHeaderColorF = value;
                NotFocusedHeaderBrushF.Color = value;

                OnPropertyChanged("NotFocusedHeaderColorF");
                OnPropertyChanged("NotFocusedHeaderBrushF");
            }
        }

        public SolidColorBrush NotFocusedHeaderBrushF { get; private set; }

        public Color BookmarkColorB
        {
            get { return _config.BookmarkColorB; }
            set
            {
                _config.BookmarkColorB = value;
                BookmarkBrushB.Color = value;

                OnPropertyChanged("BookmarkColorB");
                OnPropertyChanged("BookmarkBrushB");
            }
        }

        public SolidColorBrush BookmarkBrushB { get; private set; }

        public Color BookmarkColorF
        {
            get { return _config.BookmarkColorF; }
            set
            {
                _config.BookmarkColorF = value;
                BookmarkBrushF.Color = value;

                OnPropertyChanged("BookmarkColorF");
                OnPropertyChanged("BookmarkBrushF");
            }
        }
        public SolidColorBrush BookmarkBrushF { get; private set; }

        public Dictionary<CompileError.Type, int> RecentErrors
        {
            get { return _config.RecentErrors; }
        } 

        public bool ShowStartPage
        {
            get { return _config.ShowStartPage; }
            set
            {
                _config.ShowStartPage = value;
                OnPropertyChanged("ShowStartPage");
            }
        }

        // ----------------------------------------------------------------------------------------
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null) 
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        [Serializable]
        private class Config
        {
            public bool IsFirstTime = true;
            public int NextProjectNumber = 1;
            public WindowState WindowState = WindowState.Maximized;
            public int WindowWidth = 400;
            public int WindowHeight = 400;
            public int WindowTop = 100;
            public int WindowLeft = 50;
            public int HotkeysWindowWidth = 300;
            public int HotkeysWindowHeight = 140;
            public int HotkeysWindowTop = 100;
            public int HotkeysWindowLeft = 100;
            public int StructureWindow_Width = 900;
            public int StructureWindow_Height = 400;
            public int StructureWindow_Left = 50;
            public int StructureWindow_Top = 100;
            public WindowState StructureWindow_WindowState = WindowState.Normal;
            public double ProjectExplorerColumn_Width = 100;
            public double ErrorsRow_Height = 100;
            public double BookmarksRow_Height = 100;
            public double ContentsColumn_Width = 100;
            public bool ProjectExplorer_Visible = true;
            public bool Errors_visible = true;
            public bool Contents_Visible = true;
            public bool Bookmarks_visible = true;
            public int ColorSegmentCurrent = 0;
            public ObservableCollection<string> FavoriteDirs = new ObservableCollection<string>();
            public ObservableCollection<string> RecentProjects = new ObservableCollection<string>();
            public int CurrentLanguage = 1; // 0=ENG, 1=RUS
            public SerializableColor EditorColor_ErrorB = Color.FromRgb(171, 97, 107);          // B=background, фон         F=foreground, цвет щрифта
            public SerializableColor EditorColor_ErrorF = Colors.White;
            public SerializableColor EditorColor_SpecialErrorB = Color.FromRgb(0, 97, 107);
            public SerializableColor EditorColor_SpecialErrorF = Colors.White;
            public SerializableColor EditorColor_PopupErrorB = Color.FromRgb(0, 0, 255);
            public SerializableColor EditorColor_PopupErrorF = Colors.White;

            public SerializableColor EditorColor_CurrentLine = Color.FromArgb(40, 41, 109, 135);
            public SerializableColor EditorColor_MouseHoverTabFocused = Color.FromRgb(28, 151, 234);
            public SerializableColor EditorColor_MouseHoverTabNotFocused = Color.FromRgb(228, 225, 222);
            
            //public SerializableColor FocusedElementColor = Colors.Blue;
            //public SerializableColor NotFocusedElementColor = /*Color.FromRgb(239, 237, 235)*/Colors.White;
            public SerializableColor MainWindowBackgroundColor = Colors.White;
            public SerializableColor FocusedHeaderColorB = Color.FromRgb(0, 122, 204);
            public SerializableColor FocusedHeaderColorF = Colors.White;
            public SerializableColor NotFocusedHeaderColorB = Color.FromRgb(216, 210, 205);
            public SerializableColor NotFocusedHeaderColorF = Colors.Black;

            public SerializableColor BookmarkColorB = Color.FromRgb(0, 122, 204);
            public SerializableColor BookmarkColorF = Colors.White;

            public Dictionary<CompileError.Type, int> RecentErrors = new Dictionary<CompileError.Type, int>();
            public bool ShowStartPage = true;

            public Config()
            {
                string path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Translator");
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                FavoriteDirs.Add(path);
            }
        }

        private Config _config = new Config();

        private readonly string ConfigFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"Translator\config.bin");

        public void LoadConfiguration()
        {
            try
            {
                FileStream fs = new FileStream(ConfigFileName, FileMode.Open);
                BinaryFormatter bf = new BinaryFormatter();
                _config = (Config) bf.Deserialize(fs);
                fs.Close();
            }
            catch (Exception)
            {
                // Если что-то не так, то просто создаем новую конфигурацию
                _config = new Config();
            }
            finally
            {
                Configure();
            }
        }

        public void SaveConfiguration()
        {
            _config.IsFirstTime = false;

            FileStream fs = new FileStream(ConfigFileName, FileMode.OpenOrCreate);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, _config);

            fs.Close();
        }


        public void ToDefault()
        {
            // Просто создаем новую конфигурацию
            Config newConfig = new Config();
            SaveOldValues(_config, newConfig);
            _config = newConfig;
            OnPropertyChanged("");
        }

        private void SaveOldValues(Config oldC, Config newC)
        {
            //newC.
        }
    }
}
