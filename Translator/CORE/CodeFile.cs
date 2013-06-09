using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Xml;
using System.Xml.Schema;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.SharpZipLib.Zip;
using Translator.Annotations;
using Translator.Properties;

namespace Translator
{
    public class CodeFile : INotifyPropertyChanged
    {
        // Create new file
        public CodeFile(string path, bool isNewFile)
        {
            _versions = new LinkedList<Version>();

            if (isNewFile) _versions.AddFirst(new Version());
            _codeFileFS = new CodeFileFS(this, path, isNewFile);

            Document = CurrentVersion.Document;
            IsChanged = false;
        }

        private CodeFile()
        {
        }

        // Детали представления файла в файловой системе
        private class CodeFileFS
        {
            public CodeFileFS(CodeFile codeFile, string initPath, bool isNewFile)
            {
                _codeFile = codeFile;
                Path = initPath;

                if (isNewFile)
                    SaveXML();
                else
                    LoadXML();
            }

            public string Path;
            private readonly CodeFile _codeFile;
            private const string DefaultMetaFileName = "m.xml";
            private UTF8Encoding _UTF8Encoder = new UTF8Encoding();

            public void Move(string newPath)
            {
                File.Move(Path, newPath);
            }

            private class FileEntry
            {
                public byte[] Bytes;
                public string Name;
            }

            public void SaveXML()
            {
                ZipOutputStream zipStream = null;
                try
                {
                    // Выходной поток
                    FileStream fsOut = File.Create(Path);
                    zipStream = new ZipOutputStream(fsOut);

                    ZipEntry newEntry = new ZipEntry(DefaultMetaFileName);
                    zipStream.PutNextEntry(newEntry);

                    // Файлы, сразу их писатьне получится, так что сначала будем запоминать, что надо бы записать
                    // Непосредственная апись в конце
                    LinkedList<FileEntry> fileEntries = new LinkedList<FileEntry>();

                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.CloseOutput = false; // Не закрывать ZIP поток после записи метафайла!
                    settings.Indent = true;
                    using (var w = XmlTextWriter.Create(zipStream, settings))
                    {
                        w.WriteStartElement("File");
                        w.WriteStartElement("Versions");

                        // Сами файлы будут иметь имя просто 0, 1, 2, ...
                        int counter = 0;
                        foreach (Version version in _codeFile._versions)
                        {
                            w.WriteStartElement("Version");
                            w.WriteAttributeString("Key", version.Key.ToString());
                            w.WriteAttributeString("Mark", version.Mark);

                            w.WriteStartElement("Text");
                            w.WriteAttributeString("Hash", version.Document.Text.GetHashCode().ToString());
                            string name = (counter++).ToString();
                            w.WriteAttributeString("Name", name);
                            w.WriteEndElement(); // Text

                            fileEntries.AddLast(new FileEntry
                            {
                                Bytes = _UTF8Encoder.GetBytes(version.Document.Text),
                                Name = name
                            });

                            w.WriteStartElement("Checkboxes");
                            foreach (TextAnchor cb in version.Checkboxes.Where(cb => !cb.IsDeleted))
                            {
                                w.WriteStartElement("Checkbox");
                                w.WriteAttributeString("Offset", cb.Offset.ToString());
                                w.WriteEndElement(); // Checkbox
                            }
                            w.WriteEndElement(); // Checkboxes


                            w.WriteStartElement("Comments");
                            foreach (Comment comment in version.Comments.Where(comment => !comment.Anchor.IsDeleted))
                            {
                                w.WriteStartElement("Comment");
                                w.WriteAttributeString("Offset", comment.Anchor.Offset.ToString());
                                w.WriteAttributeString("Text", comment.Text);
                                w.WriteEndElement(); // Comment
                            }
                            w.WriteEndElement(); // Comments


                            w.WriteStartElement("ColorSegments");
                            foreach (ColorSegment segment in version.ColorSegments.Where(segment => segment.IsAcive()))
                            {
                                w.WriteStartElement("ColorSegment");
                                w.WriteAttributeString("Start", segment.Start.Offset.ToString());
                                w.WriteAttributeString("End", segment.End.Offset.ToString());
                                w.WriteAttributeString("Color", segment.Color.Color.ToString());
                                w.WriteEndElement(); // ColorSegment
                            }
                            w.WriteEndElement(); // ColorSegments


                            w.WriteStartElement("Bookmarks");
                            foreach (Bookmark bookmark in version.Bookmarks.Where(bookmark => bookmark.IsActive))
                            {
                                w.WriteStartElement("Bookmark");
                                w.WriteAttributeString("Offset", bookmark.Offset.ToString());
                                w.WriteAttributeString("Key", bookmark.Key.ToString());
                                w.WriteAttributeString("Mark", bookmark.Mark);
                                w.WriteEndElement(); // Bookmark
                            }
                            w.WriteEndElement(); // Bookmarks

                            w.WriteEndElement(); // Version
                        }

                        w.WriteEndElement(); // Versions
                        w.WriteEndElement(); // File
                    }

                    zipStream.CloseEntry(); // Закончили метафайл

                    // Записываем сами файлы
                    foreach (FileEntry entry in fileEntries)
                    {
                        ZipEntry zipEntry = new ZipEntry(entry.Name);
                        zipStream.PutNextEntry(zipEntry);

                        zipStream.Write(entry.Bytes, 0, entry.Bytes.Length);

                        zipStream.CloseEntry();
                    }
                }
                catch (IOException ex) { throw new SaveCodeFileException(Path, ex); }
                catch (UnauthorizedAccessException ex) { throw new SaveCodeFileException(Path, ex); }
                finally
                {
                    if (zipStream != null)
                    {
                        zipStream.IsStreamOwner = true;
                        zipStream.Close(); // Закончили весь ZIP архив
                    }
                }
            }

            private static XmlReader GetXMLReader(Stream inStream)
            {
                XmlSchemaSet sc = new XmlSchemaSet();
                sc.Add(XmlSchema.Read(new StringReader(Resources.ResourceManager.GetString("FileSchema")), null));

                XmlReaderSettings settings = new XmlReaderSettings();
                settings.ValidationType = ValidationType.Schema;
                settings.Schemas = sc;
                settings.CloseInput = false;

                XmlReader reader = XmlReader.Create(inStream, settings);
                return reader;
            }

            public void LoadXML()
            {
                ZipFile zf = null;

                try
                {
                    FileStream fs = File.OpenRead(Path);
                    zf = new ZipFile(fs);
                    int idx = zf.FindEntry(DefaultMetaFileName, false);
                    ZipEntry metaFile = zf[idx];

                    using (XmlReader r = GetXMLReader(zf.GetInputStream(metaFile)))
                    {
                        Version currentVersion = null;

                        while (r.Read())
                            if (r.NodeType == XmlNodeType.Element && r.Name == "File")
                            {
                            }
                            else if (r.NodeType == XmlNodeType.Element && r.Name == "Versions")
                            {
                            }
                            else if (r.NodeType == XmlNodeType.Element && r.Name == "Version")
                            {
                                currentVersion = new Version();
                                currentVersion.Key = DateTime.Parse(r.GetAttribute("Key"));
                                currentVersion.Mark = r.GetAttribute("Mark");

                                _codeFile._versions.AddLast(currentVersion);
                            }
                            else if (r.NodeType == XmlNodeType.EndElement && r.Name == "Version")
                            {
                                currentVersion = null;
                            }
                            // Checkboxes
                            else if (r.NodeType == XmlNodeType.Element && r.Name == "Checkboxes")
                            {
                            }
                            else if (r.NodeType == XmlNodeType.Element && r.Name == "Checkbox")
                            {
                                int offset = Convert.ToInt32(r.GetAttribute("Offset"));
                                currentVersion.Checkboxes.Add(currentVersion.Document.CreateAnchor(offset));
                            }
                            // Comments
                            else if (r.NodeType == XmlNodeType.Element && r.Name == "Comments")
                            {
                            }
                            else if (r.NodeType == XmlNodeType.Element && r.Name == "Comment")
                            {
                                string text = r.GetAttribute("Text");
                                int offset = Convert.ToInt32(r.GetAttribute("Offset"));

                                currentVersion.Comments.Add(new Comment()
                                {
                                    Anchor = currentVersion.Document.CreateAnchor(offset),
                                    Text = text
                                });
                            }
                            // ColorSegments
                            else if (r.NodeType == XmlNodeType.Element && r.Name == "ColorSegments")
                            {
                            }
                            else if (r.NodeType == XmlNodeType.Element && r.Name == "ColorSegment")
                            {
                                string textColor = r.GetAttribute("Color");
                                int start = Convert.ToInt32(r.GetAttribute("Start"));
                                int end = Convert.ToInt32(r.GetAttribute("End"));

                                Color? color = ColorConverter.ConvertFromString(textColor) as Color?;
                                SolidColorBrush b = new SolidColorBrush(color.Value);

                                currentVersion.ColorSegments.Add(new ColorSegment()
                                {
                                    Color = b,
                                    Start = currentVersion.Document.CreateAnchor(start),
                                    End = currentVersion.Document.CreateAnchor(end)
                                });
                            }
                            // Bookmarks
                            else if (r.NodeType == XmlNodeType.Element && r.Name == "Bookmarks")
                            {
                            }
                            else if (r.NodeType == XmlNodeType.Element && r.Name == "Bookmark")
                            {
                                int offset = Convert.ToInt32(r.GetAttribute("Offset"));
                                int key = Convert.ToInt32(r.GetAttribute("Key"));

                                Bookmark bookmark = new Bookmark(_codeFile, currentVersion.Document.CreateAnchor(offset), key, r.GetAttribute("Mark"));
                                currentVersion.Bookmarks.Add(bookmark);
                            }
                            else if (r.NodeType == XmlNodeType.Element && r.Name == "Text")
                            {
                                int hash = Convert.ToInt32(r.GetAttribute("Hash"));
                                string name = r.GetAttribute("Name");

                                // Загружаем файл по этому имени
                                idx = zf.FindEntry(name, false);
                                ZipEntry entryFile = zf[idx];
                                StreamReader reader = new StreamReader(zf.GetInputStream(entryFile));
                                currentVersion.Document.Text = reader.ReadToEnd();

                                // Сравниваем хеш-код
                                if (currentVersion.Document.Text.GetHashCode() != hash)
                                    throw new FileHashException(_codeFile.Path);
                            }
                    }
                }
                catch (IOException ex)                  { throw new LoadCodeFileException(Path, ex); }
                catch (XmlException ex)                 { throw new LoadCodeFileException(Path, ex); }
                catch (XmlSchemaException ex)           { throw new LoadCodeFileException(Path, ex); }
                catch (UnauthorizedAccessException ex)  { throw new LoadCodeFileException(Path, ex); }
                finally
                {
                    if (zf != null)
                    {
                        zf.IsStreamOwner = true; // Makes close also shut the underlying stream
                        zf.Close();
                    }
                }
            }

            private const string DefaultCopyName = " - копия";
            public const string DefaultExt = ".code";            // Файл с кодом (пользовательский)

            // Найти имя для копии
            public string FindCopyName()
            {
                string directory = System.IO.Path.GetDirectoryName(this.Path);
                string fileNameWithoutExt = System.IO.Path.GetFileNameWithoutExtension(this.Path);
                string fileExt = System.IO.Path.GetExtension(this.Path);

                string fileName = fileNameWithoutExt + DefaultCopyName + fileExt;
                string path = System.IO.Path.Combine(directory, fileName);

                bool foundName = !File.Exists(path);

                int num = 2;
                while (!foundName)
                {
                    fileName = fileNameWithoutExt + DefaultCopyName + " (" + num.ToString() + ")" + fileExt;
                    path = System.IO.Path.Combine(directory, fileName);

                    foundName = !File.Exists(path);
                    num++;
                }

                return path;
            }

            public static string GetPreview(string path)
            {
                ZipFile zf = null;

                try
                {
                    FileStream fs = File.OpenRead(path);
                    zf = new ZipFile(fs);

                    int idx = zf.FindEntry("0", false);
                    ZipEntry entryFile = zf[idx];
                    using (StreamReader reader = new StreamReader(zf.GetInputStream(entryFile)))
                    {
                        return reader.ReadToEnd();
                    }
                }
                catch (Exception)
                {
                    // Если хоть что-то не так, просто возвращаем null, т.е. превью будет недоступно
                    return null;
                }
                finally
                {
                    if (zf != null)
                    {
                        zf.IsStreamOwner = true; // Makes close also shut the underlying stream
                        zf.Close(); // Ensure we release resources
                    }
                }
            }
        }   // CodeFileFS

        // Создание копиии файла (вместе со всеми версиями)
        private CodeFile Clone(string path)
        {
            CodeFile newFile = new CodeFile(); //CodeFile newFile = this.MemberwiseClone() as CodeFile;  <<<< ПРОБЛЕМЫ С событиями будут!!

            // Deep copy версий
            newFile._versions = new LinkedList<Version>();
            foreach (Version ver in _versions)
                newFile._versions.AddLast(ver.Clone());

            // Документ общий - это документ из первой версии
            newFile.Document = newFile.CurrentVersion.Document;

            // Ошибок изначально нет
            newFile.ErrorsList = null;

            // Deep copy FS
            newFile._codeFileFS = new CodeFileFS(newFile, path, true);

            return newFile;
        }

        public CodeFile MakeCopy(string fileName)
        {
            // Сохраняем исходный
            this.Save();
            string path = "";

            // Ищем имя
            if (fileName == null)
                path = _codeFileFS.FindCopyName();
            else
            {
                // Если задано нужное имя
                string directory = System.IO.Path.GetDirectoryName(this.Path);
                path = System.IO.Path.Combine(directory, fileName);
            }

            Logger.LogWithBinding("Status_FileCloned");

            CodeFile newFile = this.Clone(path);
            return newFile;
        }

        public void Move(string newPath)
        {
            Path = newPath;
        }

        // Получить текст от последней версии файла
        public static string GetPreview(string path)
        {
            return CodeFileFS.GetPreview(path);
        }

        // Просто брать их первой версии нельзя, т.к. при смене надо подписаться на события
        private TextDocument _document;
        public TextDocument Document
        {
            get { return _document; }
            private set
            {
                // Отписываемся от старого
                if (_document != null)
                {
                    _document.TextChanged -= DocumentOnTextChanged;
                    _document.PropertyChanged -= DocumentOnPropertyChanged;
                }

                _document = value;

                // Подписываемся на новый
                if (_document != null)
                {
                    _document.TextChanged += DocumentOnTextChanged;
                    _document.PropertyChanged += DocumentOnPropertyChanged;
                }
            }
        }

        private void DocumentOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == "LineCount")
            {
                MergeDuplicateCheckboxes();
                MergeDuplicateComments();

                OnLineCountChanged();
            }
        }

        private void DocumentOnTextChanged(object sender, EventArgs eventArgs)
        {
            IsChanged = true;
            OnTextChanged();
        }

        public string Text
        {
            get { return (Document == null) ? null : Document.Text; }
        }

        public string Name
        {
            get { return System.IO.Path.GetFileName(Path); }
            set
            {
                // Устанавливаем Name
                // Будем считать, что это означает ____переименование_____
                // Т.е путь остается прежним, меняется только само имя файла

                string newPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Path), value);
                Path = newPath;

                Logger.LogWithBinding("Status_FileRenamed");
                //OnPropertyChanged("Name"); <<<< это вызовется автоматически при ихменении Path
            }
        }

        public string Path
        {
            get { return _codeFileFS.Path; }
            set
            {
                try
                {
                    Save();// Перед этим надо сохранить все в файл

                    _codeFileFS.Move(value);
                    _codeFileFS.Path = value;

                    OnPropertyChanged("Name");
                    OnPropertyChanged("Path");

                    Logger.LogWithBinding("Status_FileMoved");
                }
                catch (UnauthorizedAccessException ex) { throw new FileMoveException(Path, ex); }
                catch (IOException ex) { throw new FileMoveException(Path, ex); }
                catch (NotSupportedException ex) { throw new FileMoveException(Path, ex); }
            }
        }

        private class Version
        {
            public DateTime Key { get; set; }
            public int LastCaretOffset { get; set; }
            public List<ColorSegment> ColorSegments { get; set; }
            public List<TextAnchor> Checkboxes { get; set; }
            public List<Comment> Comments { get; set; }
            public List<Bookmark> Bookmarks { get; set; }
            public TextDocument Document { get; set; }
            public string Mark { get; set; }

            public Version()
            {
                Document = new TextDocument();
                LastCaretOffset = 0;
                ColorSegments = new List<ColorSegment>();
                Checkboxes = new List<TextAnchor>();
                Comments = new List<Comment>();
                Bookmarks = new List<Bookmark>();
                Mark = "";
            }

            public Version Clone()
            {
                Version newVersion = new Version();

                // Делаем глубокую копию
                newVersion.LastCaretOffset = LastCaretOffset;
                newVersion.ColorSegments = new List<ColorSegment>(ColorSegments);
                newVersion.Checkboxes = new List<TextAnchor>(Checkboxes);
                newVersion.Comments = new List<Comment>(Comments);
                newVersion.Bookmarks = new List<Bookmark>(Bookmarks);
                newVersion.Key = DateTime.Now;
                newVersion.Mark = String.Copy(Mark);
                newVersion.Document = new TextDocument(Document.Text);

                return newVersion;
            }
        }

        /*
         * Для создания иллюзии, будто бы никаких версий нет
         */
        private CodeFileFS _codeFileFS;
        private Version CurrentVersion
        {
            get { return _versions.First.Value; }
        }

        public int LastCaretOffset
        {
            get { return CurrentVersion.LastCaretOffset; }
            set { CurrentVersion.LastCaretOffset = value; }
        }

        public List<ColorSegment> ColorSegments { get { return CurrentVersion.ColorSegments; } }
        public List<TextAnchor> Checkboxes { get { return CurrentVersion.Checkboxes; } }
        public List<Comment> Comments { get { return CurrentVersion.Comments; } }
        public List<Bookmark> Bookmarks { get { return CurrentVersion.Bookmarks; } }

        public event EventHandler BookmarksChanged;
        protected virtual void OnBookmarksChanged()
        {
            EventHandler handler = BookmarksChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        private bool _isChanged;
        public bool IsChanged
        {
            get { return _isChanged; }
            set
            {
                _isChanged = value;
                OnPropertyChanged("IsChanged");
            }
        }

        public static string DefaultName
        {
            get { return LocalString.Get("DefautlFileName"); }
        }

        public static string DefaultExt
        {
            get { return CodeFileFS.DefaultExt; }
        }

        // ----------------------------------------------------------------------------------------
        private LinkedList<Version> _versions;
        public IEnumerable<KeyValuePair<DateTime, string>> Versions
        {
            get
            {
                int idx = 0;
                foreach (Version version in _versions)
                {
                    if (idx != 0)
                        yield return new KeyValuePair<DateTime, string>(version.Key, version.Document.Text);

                    idx++;
                }
            }
        }

        // Создает новую версию как копию текущего состояния
        public void MakeVersion()
        {
            IsChanged = true;

            Version newVersion = CurrentVersion.Clone();
            _versions.AddLast(newVersion);

            Save();

            Logger.LogWithBinding("Status_MakeVersion");
        }

        public void RemoveVersion(DateTime key)
        {
            IsChanged = true;

            Version founded = _versions.First(v => v.Key == key);
            if (founded != null)
                _versions.Remove(founded);

            Debug.Assert(_versions.Count >= 1);

            Save();
        }

        public void GoToVersion(DateTime key)
        {
            IsChanged = true;
            this.Save();

            // Сохраняем текущее состояние как версию (в конец)
            MakeVersion();
            _versions.RemoveFirst();// Текущую сохранили в конце, можно удалить

            Version founded = _versions.First(v => v.Key == key);
            if (founded != null)
            {
                // Swap
                // Найденный становится первым
                _versions.Remove(founded);
                _versions.AddFirst(founded);
            }

            // Вызвать все события
            Document = CurrentVersion.Document;
            OnPropertyChanged("");

            OnBookmarksChanged();
            OnTextChanged();
            OnLineCountChanged();

            IsChanged = true;
            Save();

            Logger.LogWithBinding("Status_GoToVersion");
        }

        // Сохраняет только сам файл на диск, ПРОЕКТ НЕ МЕНЯЕТСЯ!!!!!!!
        public void Save()
        {
            if (IsChanged)
            {
                _codeFileFS.SaveXML();
                IsChanged = false;
            }
        }


        // ----------------------------------------------------------------------------------------
        //  Раскраска сегментов
        // ----------------------------------------------------------------------------------------
        public class ColorSegment
        {
            public TextAnchor Start;
            public TextAnchor End;
            public SolidColorBrush Color;

            public bool IsAcive()
            {
                return !Start.IsDeleted && !End.IsDeleted && (End.Offset > Start.Offset);
            }
        }

        public void AddColorSegment(int start, int end, SolidColorBrush color)
        {
            TextAnchor startAnchor = Document.CreateAnchor(start);
            startAnchor.MovementType = AnchorMovementType.AfterInsertion;
            startAnchor.SurviveDeletion = true;

            TextAnchor endAnchor = Document.CreateAnchor(end);
            endAnchor.MovementType = AnchorMovementType.BeforeInsertion;
            endAnchor.SurviveDeletion = true;

            ColorSegment segment = new ColorSegment() { Color = color, Start = startAnchor, End = endAnchor };
            ColorSegments.Add(segment);

            IsChanged = true;
        }

        // Просматривает всю коллекцию цветосегментов, и удалаяет все сегменты в интервале [start, end)
        public void EraseColorSegment(int start, int end)
        {
            for (int i = ColorSegments.Count - 1; i >= 0; i--)
            {
                ColorSegment segment = ColorSegments[i];

                if ((start < segment.Start.Offset && end < segment.Start.Offset) ||
                    (start > segment.End.Offset && end > segment.End.Offset))
                {
                    continue;
                }

                if (start <= segment.Start.Offset && end >= segment.End.Offset)
                {
                    ColorSegments.RemoveAt(i);
                }
                else if (start <= segment.Start.Offset && end < segment.End.Offset)
                {
                    TextAnchor startAnchor = Document.CreateAnchor(end);
                    segment.Start = startAnchor;
                }
                else if (segment.Start.Offset < start && segment.End.Offset <= end)
                {
                    TextAnchor endAnchor = Document.CreateAnchor(start);
                    segment.End = endAnchor;
                }
                else if (segment.Start.Offset < start && end < segment.End.Offset)
                {
                    // разбиваем на 2

                    // add new
                    AddColorSegment(end, segment.End.Offset, segment.Color);

                    TextAnchor endAnchor = Document.CreateAnchor(start);
                    segment.End = endAnchor;
                }
                else
                {
                    Debug.Assert(false, "WE CAN'T BE HERE!!!!!");
                }
            }

            IsChanged = true;
        }

        public void ClearAllColorSegments()
        {
            ColorSegments.Clear();
            IsChanged = true;
        }

        // ----------------------------------------------------------------------------------------
        //  Bookmarks
        // ----------------------------------------------------------------------------------------
        public class Bookmark : INotifyPropertyChanged
        {
            public Bookmark(CodeFile file, int line, int key, string mark)
            {
                File = file;
                Key = key;
                Mark = mark;

                int offset = file.Document.GetLineByNumber(line).Offset;
                _anchor = file.Document.CreateAnchor(offset);

                file.CurrentVersion.Document.TextChanged += (sender, args) => OnPropertyChanged("Line");
            }

            public Bookmark(CodeFile file, TextAnchor anchor, int key, string mark)
            {
                File = file;
                Key = key;
                Mark = mark;
                _anchor = anchor;

                file.CurrentVersion.Document.TextChanged += (sender, args) => OnPropertyChanged("Line");
            }

            private TextAnchor _anchor;
            public bool IsActive { get { return !_anchor.IsDeleted; } }
            public int Offset { get { return _anchor.Offset; } }
            public int Line
            {
                get { return IsActive ? _anchor.Line : 0; }
            }
            public int Key { get; private set; }
            public string Mark { get; set; }
            public CodeFile File { get; private set; }
            
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged(string propertyName)
            {
                PropertyChangedEventHandler handler = PropertyChanged;
                if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void AddBookmark(int line, int key)
        {
            // Если уже есть на текущей линнии с таким же ключлм, то удаляем совсем и выходим
            if (Bookmarks.RemoveAll(b => b.Line == line && b.Key == key) > 0)
            {
            }
            else
            {
                // Если уже есть на текущей линни, удалим его
                Bookmarks.RemoveAll(b => b.Line == line);

                // Удалим все с текущим ключом
                Bookmarks.RemoveAll(b => b.Key == key);

                Bookmark bookmark = new Bookmark(this, line, key, "");
                Bookmarks.Add(bookmark);
            }

            IsChanged = true;
            OnBookmarksChanged();
        }

        public void DeleteBookmark(int line)
        {
            Bookmarks.RemoveAll(b => b.Line == line);
            IsChanged = true;
            OnBookmarksChanged();
        }


        // ----------------------------------------------------------------------------------------
        //  Checkboxes
        // ----------------------------------------------------------------------------------------
        // line - с единицы
        public void AddCheckbox(int line)
        {
            TextAnchor anchor = Document.CreateAnchor(Document.Lines[line - 1].Offset);
            Checkboxes.Add(anchor);

            IsChanged = true;
        }

        public void DeleteCheckbox(int line)
        {
            Checkboxes.RemoveAll(anchor => anchor.Line == line);

            IsChanged = true;
        }

        private void MergeDuplicateCheckboxes()
        {
            List<TextAnchor> checkboxes2Del = new List<TextAnchor>();

            for (int i = 0; i < Checkboxes.Count; i++)
            {
                if (!Checkboxes[i].IsDeleted)
                {
                    for (int j = i + 1; j < Checkboxes.Count; j++)
                    {
                        if (!Checkboxes[j].IsDeleted)
                        {
                            if (Checkboxes[i].Line == Checkboxes[j].Line)
                            {
                                checkboxes2Del.Add(Checkboxes[j]);
                            }
                        }
                    }
                }
            }

            foreach (TextAnchor checkbox2Del in checkboxes2Del)
            {
                Checkboxes.Remove(checkbox2Del);
            }

            checkboxes2Del.Clear();
            IsChanged = true;
        }


        // ----------------------------------------------------------------------------------------
        // Комментарии справа
        // ----------------------------------------------------------------------------------------
        public class Comment
        {
            public TextAnchor Anchor;
            public string Text;
        }

        public void UpdateComment(int line, string commentText)
        {
            IsChanged = true;

            foreach (Comment comment in Comments)
            {
                if (!comment.Anchor.IsDeleted && comment.Anchor.Line == line)
                {
                    // Уже есть коммент в этой строке, просто заменяем его на новый
                    comment.Text = commentText;
                    return;
                }
            }

            // Not found...
            // Создаем новый
            TextAnchor anchor = Document.CreateAnchor(Document.Lines[line - 1].Offset);
            Comments.Add(new Comment() { Anchor = anchor, Text = commentText });
        }

        private void MergeDuplicateComments()
        {
            List<Comment> comments2Del = new List<Comment>();

            for (int i = 0; i < Comments.Count; i++)
            {
                if (!Comments[i].Anchor.IsDeleted)
                {
                    for (int j = i + 1; j < Comments.Count; j++)
                    {
                        if (!Comments[j].Anchor.IsDeleted)
                        {
                            if (Comments[i].Anchor.Line == Comments[j].Anchor.Line)
                            {
                                Comments[i].Text += "+" + Comments[j].Text;

                                comments2Del.Add(Comments[j]);
                            }
                        }
                    }
                }
            }

            foreach (Comment comment2Del in comments2Del)
            {
                Comments.Remove(comment2Del);
            }

            comments2Del.Clear();

            IsChanged = true;
        }

        // ----------------------------------------------------------------------------------------
        // Ошибки компиляции
        // ----------------------------------------------------------------------------------------
        public void Compile()
        {
            if (ErrorsList != null)
                ErrorsList.Clear();

            ErrorsList = Compiler.Instance.Compile(this);

            OnCompiled();
        }

        public List<CompileError> ErrorsList { get; private set; }

        public CompileError GetErrorByOffset(int offset)
        {
            if (ErrorsList != null)
                return ErrorsList.FirstOrDefault(error => error.IsActive && offset == error.BeginOffset);
            else
                return null;
        }

    
        // ----------------------------------------------------------------------------------------
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public event EventHandler TextChanged;
        protected void OnTextChanged()
        {
            EventHandler handler = TextChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public event EventHandler LineCountChanged;
        protected void OnLineCountChanged()
        {
            EventHandler handler = LineCountChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public event EventHandler Compiled;
        protected virtual void OnCompiled()
        {
            EventHandler handler = Compiled;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }   // class CodeFile

    // --------------------------------------------------------------------------------------------
    class FileHashException : ApplicationException
    {
        public string FileName { get; private set; }

        public FileHashException(string fileName)
            : base("File '" + fileName + "' corrupted!")
        {
            FileName = fileName;
        }
    }

    class SaveCodeFileException : ApplicationException
    {
        public string FileName { get; private set; }

        public SaveCodeFileException(string fileName, Exception innerException)
            : base("Error saving file '" + fileName + "'.", innerException)
        {
            FileName = fileName;
        }
    }

    class LoadCodeFileException : ApplicationException
    {
        public string FileName { get; private set; }

        public LoadCodeFileException(string fileName, Exception innerException)
            : base("Error loading file '" + fileName + "'.", innerException)
        {
            FileName = fileName;
        }
    }

    public class FileMoveException : ApplicationException
    {
        public string FileName { get; private set; }

        public FileMoveException(string fileName, Exception innerException)
            : base("Error moving file '" + fileName + "'.", innerException)
        {
            FileName = fileName;
        }
    }
}