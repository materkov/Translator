using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Translator
{
    namespace UICommands
    {
        public class AddNewFile : RoutedUICommand
        {
            private AddNewFile()
            {
                InputGestures.Add(new KeyGesture(Key.O, ModifierKeys.Control));
            }

            private static AddNewFile _instance = new AddNewFile();
            public static AddNewFile Instance { get { return _instance; } }
        }

        public class AddExistingFile : RoutedUICommand
        {
            private AddExistingFile()
            {
                
            }

            private static AddExistingFile _instance = new AddExistingFile();
            public static AddExistingFile Instance { get { return _instance; } }
        }

        public class ExcludeFile : RoutedUICommand
        {
            private ExcludeFile()
            {

            }

            private static ExcludeFile _instance = new ExcludeFile();
            public static ExcludeFile Instance { get { return _instance; } }
        }

        public class Rename : RoutedUICommand
        {
            private Rename()
            {

            }

            private static Rename _instance = new Rename();
            public static Rename Instance { get { return _instance; } }
        }

        public class Move : RoutedUICommand
        {
            private Move()
            {

            }

            private static Move _instance = new Move();
            public static Move Instance { get { return _instance; } }
        }

        public class NewCopy : RoutedUICommand
        {
            private NewCopy()
            {

            }

            private static NewCopy _instance = new NewCopy();
            public static NewCopy Instance { get { return _instance; } }
        }

        public class BuildProject : RoutedUICommand
        {
            private BuildProject()
            {

            }

            private static BuildProject _instance = new BuildProject();
            public static BuildProject Instance { get { return _instance; } }
        }

        public class Exit : RoutedUICommand
        {
            private Exit()
            {

            }

            private static Exit _instance = new Exit();
            public static Exit Instance { get { return _instance; } }
        }

        public class CloseCurrentTab : RoutedUICommand
        {
            private CloseCurrentTab()
            {

            }

            private static CloseCurrentTab _instance = new CloseCurrentTab();
            public static CloseCurrentTab Instance { get { return _instance; } }
        }

        public class CloseAllFiles : RoutedUICommand
        {
            private CloseAllFiles()
            {

            }

            private static CloseAllFiles _instance = new CloseAllFiles();
            public static CloseAllFiles Instance { get { return _instance; } }
        }

        public class NewVersion : RoutedUICommand
        {
            private NewVersion()
            {

            }

            private static NewVersion _instance = new NewVersion();
            public static NewVersion Instance { get { return _instance; } }
        }

        public class GoToVersion : RoutedUICommand
        {
            private GoToVersion()
            {

            }

            private static GoToVersion _instance = new GoToVersion();
            public static GoToVersion Instance { get { return _instance; } }
        }

        public class CloseProject : RoutedUICommand
        {
            private CloseProject()
            {

            }

            private static CloseProject _instance = new CloseProject();
            public static CloseProject Instance { get { return _instance; } }
        }

        public class OpenProject : RoutedUICommand
        {
            private OpenProject()
            {

            }

            private static OpenProject _instance = new OpenProject();
            public static OpenProject Instance { get { return _instance; } }
        }

        public class NewProject : RoutedUICommand
        {
            private NewProject()
            {

            }

            private static NewProject _instance = new NewProject();
            public static NewProject Instance { get { return _instance; } }
        }

        public class Compile : RoutedUICommand
        {
            private Compile()
            {
                InputGestures.Add(new KeyGesture(Key.F5, ModifierKeys.None));
            }

            private static Compile _instance = new Compile();
            public static Compile Instance { get { return _instance; } }
        }

        public class SaveCurrentFile : RoutedUICommand
        {
            private SaveCurrentFile()
            {

            }

            private static SaveCurrentFile _instance = new SaveCurrentFile();
            public static SaveCurrentFile Instance { get { return _instance; } }
        }

        public class SaveAllFiles : RoutedUICommand
        {
            private SaveAllFiles()
            {

            }

            private static SaveAllFiles _instance = new SaveAllFiles();
            public static SaveAllFiles Instance { get { return _instance; } }
        }

        public class SaveProject : RoutedUICommand
        {
            private SaveProject()
            {

            }

            private static SaveProject _instance = new SaveProject();
            public static SaveProject Instance { get { return _instance; } }
        }

        public class ShowSettings : RoutedUICommand
        {
            private ShowSettings()
            {

            }

            private static ShowSettings _instance = new ShowSettings();
            public static ShowSettings Instance { get { return _instance; } }
        }

        public class RenameFileInProjectExplorer  : RoutedUICommand
        {
            private RenameFileInProjectExplorer()
            {

            }

            private static RenameFileInProjectExplorer _instance = new RenameFileInProjectExplorer();
            public static RenameFileInProjectExplorer Instance { get { return _instance; } }
        }
    }
    
    /*static class UICommands
    {
        static UICommands()
        {
            //AddNewFile.
        }

        public static class AddNewFile : RoutedUICommand
        {
            
        }
        public static readonly RoutedUICommand AddNewFile = new RoutedUICommand(,,Type.DefaultBinder, );
        public static readonly RoutedUICommand AddExistingFile = new RoutedUICommand();
        public static readonly RoutedUICommand ExcludeFile = new RoutedUICommand();
        public static readonly RoutedUICommand Rename = new RoutedUICommand();
        public static readonly RoutedUICommand Move = new RoutedUICommand();
        public static readonly RoutedUICommand NewCopy = new RoutedUICommand();
        public static readonly RoutedUICommand BuildProject = new RoutedUICommand();
        public static readonly RoutedUICommand Exit = new RoutedUICommand();
        public static readonly RoutedUICommand CloseCurrentTab = new RoutedUICommand();
        public static readonly RoutedUICommand CloseAllFiles = new RoutedUICommand();
        public static readonly RoutedUICommand NewVersion = new RoutedUICommand();
        public static readonly RoutedUICommand GoToVersion = new RoutedUICommand();
        public static readonly RoutedUICommand CloseProject = new RoutedUICommand();
        public static readonly RoutedUICommand OpenProject = new RoutedUICommand();
        public static readonly RoutedUICommand NewProject = new RoutedUICommand();
        public static readonly RoutedUICommand Compile = new RoutedUICommand();
        public static readonly RoutedUICommand SaveCurrentFile = new RoutedUICommand();
        public static readonly RoutedUICommand SaveAllFiles = new RoutedUICommand();
        public static readonly RoutedUICommand SaveProject = new RoutedUICommand();
        public static readonly RoutedUICommand ShowSettings = new RoutedUICommand();

        public static readonly RoutedUICommand RenameFileInProjectExplorer = new RoutedUICommand();
        
    }*/
}
