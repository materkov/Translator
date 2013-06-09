using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml;
using System.Xml.Schema;
using Translator.Properties;

namespace Translator
{
    public class CodeProject : INotifyPropertyChanged
    {
        public CodeProject(string path)
        {
            _path = path;   // Path = path <<< при инициализации свойство использовать нельзя, т.к. атм есть сохранение
            Files = new ObservableCollection<CodeFile>();
            WorkDirs = new ObservableCollection<string>();

            // Default constants
            NextDefaultFileNumber = 1;
        }

        private void LoadXML()
        {
            try
            {
                using (var xml_reader = new XmlTextReader(Path))
                using (var reader = new XmlValidatingReader(xml_reader))
                {
                    int fileErrorsCounter = 0;

                    XmlSchema schema = XmlSchema.Read(new StringReader(Resources.ResourceManager.GetString("ProjectSchema")), null);
                    reader.Schemas.Add(schema);

                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element && reader.Name == "Project")
                        {
                            //project.Name = reader.GetAttribute("Name");
                            //project.DefaultCodeFileExtension = reader.GetAttribute("DefaultCodeFileExtension");
                            NextDefaultFileNumber = Convert.ToInt32(reader.GetAttribute("NextDefaultFileNumber"));
                        }
                        // Files
                        else if (reader.NodeType == XmlNodeType.Element && reader.Name == "Files")
                        {
                        }
                        else if (reader.NodeType == XmlNodeType.Element && reader.Name == "File")
                        {
                            try
                            {
                                CodeFile file = new CodeFile(reader.GetAttribute("Path"), false);
                                AddFile(file, false);
                            }
                            catch (LoadCodeFileException ex)
                            {
                                fileErrorsCounter++;

                                ErrorWindow err = new ErrorWindow
                                {
                                    ShortDesc = LocalString.Get("Error_LoadFileInProject_short") + " '" + System.IO.Path.GetFileName(ex.FileName) + "'.",
                                    FullDesc = LocalString.Get("Error_LoadFileInProject_full"),
                                    Exception = ex
                                };
                                err.ShowDialog();
                            }
                        }
                        // WorkDirs
                        else if (reader.NodeType == XmlNodeType.Element && reader.Name == "WorkDirs")
                        {
                        }
                        if (reader.NodeType == XmlNodeType.Element && reader.Name == "WorkDir")
                        {
                            WorkDirs.Add(reader.GetAttribute("Path"));
                        }
                    }

                    if (fileErrorsCounter > 0)
                    {
                    }
                }
            }
            catch (IOException ex)                  { throw new ParsingProjectException(Path, ex); }
            catch (XmlException ex)                 { throw new ParsingProjectException(Path, ex); }
            catch (XmlSchemaException ex)           { throw new ParsingProjectException(Path, ex); }
            catch (UnauthorizedAccessException ex)  { throw new ParsingProjectException(Path, ex); }
        }

        private void SaveXML()
        {
            try
            {
                using (var w = new XmlTextWriter(Path, Encoding.UTF8))
                {
                    w.Formatting = Formatting.Indented;

                    w.WriteStartElement("Project");
                    //w.WriteAttributeString("Name", Name);
                    w.WriteAttributeString("NextDefaultFileNumber", NextDefaultFileNumber.ToString());

                    w.WriteStartElement("Files");
                    foreach (CodeFile file in Files)
                    {
                        try
                        {
                            file.Save();

                            // Файл будет записан в проект, только если сохранение успешно прошло
                            w.WriteStartElement("File");
                            w.WriteAttributeString("Path", file.Path);
                            w.WriteEndElement(); // File
                        }
                        catch (SaveCodeFileException ex)
                        {
                            ErrorWindow err = new ErrorWindow
                            {
                                ShortDesc = LocalString.Get("Error_SaveFileInProject_short") + " '" + System.IO.Path.GetFileName(ex.FileName) + "'.",
                                FullDesc = LocalString.Get("Error_SaveFileInProject_full"),
                                Exception = ex
                            };
                            err.ShowDialog();
                        }
                    }
                    w.WriteEndElement(); // Files


                    w.WriteStartElement("WorkDirs");
                    foreach (string dir in WorkDirs)
                    {
                        w.WriteStartElement("WorkDir");
                        w.WriteAttributeString("Path", dir);
                        w.WriteEndElement();
                    }
                    w.WriteEndElement(); // WorkDirs

                    w.WriteEndElement(); // Project
                }
            }
            catch (IOException ex)                  { throw new SaveProjectException(Path, ex); }
            catch (UnauthorizedAccessException ex)  { throw new SaveProjectException(Path, ex); }
        }

        public ObservableCollection<CodeFile> Files { get; private set; }
        public ObservableCollection<string> WorkDirs { get; private set; } 

        public void AddFile(CodeFile file, bool save = true)
        {
            // Проверим, сузествует ли уже такой файл
            foreach (CodeFile exFile in Files.Where(exFile => exFile.Path == file.Path))
            {
                throw new FileExistsException(exFile);
            }

            Files.Add(file);
            file.PropertyChanged += FileNameChanged;

            if(save)
                Save(); // Обновим информацию в текущем проектк

            OnFileAdded(new FileArgs(file));
        }

        public void RemoveFile(CodeFile file)
        {
            Files.Remove(file);
            file.PropertyChanged -= FileNameChanged;

            Save();

            OnFileRemoved(new FileArgs(file));
        }

        private void FileNameChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == "Name")
            {
                // Rename in project XML
                Save();
            }
        }

        public const string DefaultExtension = ".proj";

        public string Name
        {
            get
            {
                return System.IO.Path.GetFileNameWithoutExtension(_path);
            }
            set
            {
                // Устанавливаем Name
                // Будем считать, что это означает ____переименование_____
                // Т.е путь остается прежним, меняется только само имя файла

                // Перед этим надо бы сохранить все
                Save();

                Rename(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(_path), value + DefaultExtension));
            }
        }

        private string _path;

        private string Path
        {
            get { return _path; } 
            set
            {
                Save();

                FSRename(_path, value);
                _path = value;

                // Переименуем в списке последних проектов,
                // Т.к. текущий проект может быть только один, то наш проект - там самый верхний
                var recent = IDEState.Get().RecentProjects;
                if (recent.Count > 0)
                    recent[0] = _path;  // Если запускаем первый раз, там будет пусто

                OnPropertyChanged("Name");
                OnPropertyChanged("Path");
            }
        }

        //private static string DefaultCodeFileExtension { get; set; }
        //private static string DefaultCodeFileName { get; set; }
        public int NextDefaultFileNumber { get; set; }

        public string GetDefaultNewFilePath()
        {
            bool nameFound;
            string path;

            do
            {
                string fileName = CodeFile.DefaultName + (NextDefaultFileNumber++).ToString() + CodeFile.DefaultExt;
                path = System.IO.Path.Combine(WorkDirs[0], fileName);
                nameFound = !File.Exists(path);
            } 
            while (!nameFound);

            return path;
        }

        public static string GetPreview(string path)
        {
            string text = "[+] " + System.IO.Path.GetFileNameWithoutExtension(path) + Environment.NewLine;

            try
            {
                using (var xml_reader = new XmlTextReader(path))
                using (var reader = new XmlValidatingReader(xml_reader))
                {
                    XmlSchema schema = XmlSchema.Read(new StringReader(Resources.ResourceManager.GetString("ProjectSchema")), null);
                    reader.Schemas.Add(schema);

                    while (reader.Read())
                        if (reader.NodeType == XmlNodeType.Element && reader.Name == "File")
                            text += " |--- " + System.IO.Path.GetFileName(reader.GetAttribute("Path")) + Environment.NewLine;
                    
                }
            }
            catch (Exception)
            {
                return null;
            }

            return text;
        }

        public static CodeProject MakeNew(string fileName)
        {
            CodeProject project = new CodeProject(fileName);
            AddToRecentProjects(project);

            project.WorkDirs.Add(System.IO.Path.GetDirectoryName(project.Path)); // По умолчанию, папка в которой проект - перваый рабочий каталог
            project.Save();
            
            return project;
        }

        public static CodeProject Load(string fileName)
        {
            CodeProject project = new CodeProject(fileName);
            project.LoadXML();
            

            AddToRecentProjects(project);
            return project;
        }

        private static void AddToRecentProjects(CodeProject project)
        {
            var recent = IDEState.Get().RecentProjects;

            // Проверить, есть ли уже такой, удалить если есть (потом все равно добавим)
            if (recent.IndexOf(project.Path) != -1)
            {
                recent.Remove(project.Path);
            }

            // Delete last
            while (recent.Count >= 10)
                recent.RemoveAt(recent.Count - 1);

            // Добавляем в начало
            recent.Insert(0, project.Path);
        }

        public void Save()
        {
            SaveXML();
        }

        public void Close()
        {
            Save();
        }

        public void Rename(string newPath)
        {
            Path = newPath;
        }

        // Rename in FILE SYSTEM
        private void FSRename(string oldName, string newName)
        {
            if (oldName == null)
                return; // Раньше имени не было, только создали, переименовывать не надо.

            File.Move(oldName, newName);
        }

        // ----------------------------------------------------------------------------------------
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public event EventHandler<FileArgs> FileAdded;
        protected virtual void OnFileAdded(FileArgs e)
        {
            EventHandler<FileArgs> handler = FileAdded;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<FileArgs> FileRemoved;
        protected virtual void OnFileRemoved(FileArgs e)
        {
            EventHandler<FileArgs> handler = FileRemoved;
            if (handler != null) handler(this, e);
        }
    }

    class FileExistsException : ApplicationException
    {
        public CodeFile File { get; private set; }

        public FileExistsException(CodeFile file)
            : base("File [" + file.Path + "] already exists!")
        {
            File = file;
        }
    }

    class ParsingProjectException : ApplicationException
    {
        public ParsingProjectException(string filename, Exception inner)
            : base("Error parsing project XML '" + filename + "'.", inner)
        {
            FileName = filename;
        }

        public string FileName { get; private set; }
    }

    class SaveProjectException : ApplicationException
    {
        public SaveProjectException(string filename, Exception inner)
            : base("Error saving project XML '" + filename + "'.", inner)
        {
            FileName = filename;
        }

        public string FileName { get; private set; }
    }

    // Валидация для NextDefaultNumber
    public class NextDefaultNumber_ValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            UInt32 val = 0;
            bool result = UInt32.TryParse((string) value, out val) && (val > 0);
            return new ValidationResult(result, LocalString.Get("ValidationError_PositiveNumber"));
        }
    }
}