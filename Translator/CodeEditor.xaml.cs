using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;

namespace Translator
{
    /// <summary>
    /// Interaction logic for CodeEditor.xaml
    /// </summary>
    public partial class CodeEditor : UserControl, CodeEditorView
    {
        public CodeEditor()
        {
            _specialErrors = new ObservableCollection<CompileError.Type>();

            InitializeComponent();
        }

        private ErrorsListView _errorListView;

        // Colorizers
        private ColorizeErrors _colorizeErrors;
        private ColorizeErrorForPopup _colorizeErrorForPopUp;

        // ----------------------------------------------------------------------------------------
        public void Init(ErrorsListView errorsListView)
        {
            OnPropertyChanged("CurrentCodeFile"); // Без этого не скрывает все изначально

            _colorizeErrors = new ColorizeErrors(this);
            _colorizeErrorForPopUp = new ColorizeErrorForPopup();

            // Tabs control
            tabControl.SelectionChanged += (sender, args) =>
                {
                    SetNewCodeFile();
                    OnPropertyChanged("CurrentCodeFile");
                    OnActiveFileChanged();
                    //Dispatcher.BeginInvoke(new Action(() => AvalonEditor.Focus())); // official hack! BAD!
                };
            tabControl.FileOpened += (sender, args) => OnFileOpened(args);
            tabControl.FileClosed += (sender, args) => OnFileClosed(args);

            tabControl.Init(this);

            // Setting view
            _errorListView = errorsListView;
            
            // Скроллбары
            AvalonEditor.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            AvalonEditor.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;

            IsKeyboardFocusWithinChanged += (sender, args) =>
            {
                IsEditorFocused = IsKeyboardFocusWithin;
                _isEditorFocused.Value = IsKeyboardFocusWithin || CommentsPanel.IsKeyboardFocusWithin;
                Redraw();
                OnPropertyChanged("IsEditorFocused");
            };
            CommentsPanel.IsKeyboardFocusWithinChanged += (sender, args) =>
            {
                _isEditorFocused.Value = IsKeyboardFocusWithin || CommentsPanel.IsKeyboardFocusWithin;
                Redraw();
            };

            // Выделение цветом
            XBackgroundRenderer currentLineColorize = new XBackgroundRenderer(AvalonEditor, _isEditorFocused, CommentsPanel);
            AvalonEditor.TextArea.TextView.BackgroundRenderers.Add(currentLineColorize);
            AvalonEditor.TextArea.TextView.LineTransformers.Add(new ColorizeColorSegments(this));
            AvalonEditor.TextArea.TextView.LineTransformers.Add(_colorizeErrors);
            AvalonEditor.TextArea.TextView.LineTransformers.Add(_colorizeErrorForPopUp);
            

            // Отключаем drag&drop
            // Если разрешить DragDrop, выделение цветом рушится, и вообще нестабильно, ну его
            AvalonEditor.TextArea.Options.EnableTextDragDrop = false;

            AvalonEditor.Options.EnableEmailHyperlinks = false;
            AvalonEditor.Options.EnableHyperlinks = false;
            

            // Изначально никакой файл не открыт
            //SetNewCodeFile();
            
            // Line numbers
            AvalonEditor.ShowLineNumbers = true;
            AvalonEditor.LineNumbersForeground = new SolidColorBrush(Colors.SteelBlue);
            
            // Update Line and Column in the bottom
            AvalonEditor.TextArea.Caret.PositionChanged += (sender, args) =>
                {
                    OnPropertyChanged("Caret");
                    SelectCurrentErrorForCaret();

                    if (CaretLine.Text != AvalonEditor.TextArea.Caret.Line.ToString())
                    {
                        OnPropertyChanged("Line");
                        ScrollToErrorInCurrentLine();
                    }
                    if (CaretColumn.Text != AvalonEditor.TextArea.Caret.Column.ToString())
                        OnPropertyChanged("Column");

                    CaretLine.Text      = AvalonEditor.TextArea.Caret.Line.ToString();
                    CaretColumn.Text    = AvalonEditor.TextArea.Caret.Column.ToString();
                };

            // Verical editor panels
            CheckboxesPanel.Init(this);
            CheckboxesPanel.InitCheckboxes(AvalonEditor);
            CommentsPanel.Init(this);
            CommentsPanel.InitComments(currentLineColorize, AvalonEditor);
            ErrorMarkersPanel.Init(this);
            BookmarksPanel.Init(this);
            BookmarksPanel.Init2(Resources["BookmarksContextMenu"] as ContextMenu, Resources["BookmarksEmptyContextMenu"] as ContextMenu);
            
            // Popups
            InitPopups();
        }

        // ----------------------------------------------------------------------------------------
        public void Redraw()
        {
            // Перерисовать все строки
            // нужно, например, при раскраске строки - тут надо обновить все, а то видимов все хитро кешируется
            AvalonEditor.TextArea.TextView.Redraw();
        }

        public void ForceFocus()
        {
            Dispatcher.BeginInvoke(new Action(() => AvalonEditor.Focus())); // official hack! BAD!
            AvalonEditor.Focus();
        }

        // ----------------------------------------------------------------------------------------
        #region Errors Selecting
        private void ScrollToErrorInCurrentLine()
        {
            CompileError lastError = null;
            foreach (CompileError err in _errorListView.ErrorsList)
            {
                if (err.IsActive && err.File == CurrentCodeFile && err.BeginLine == Line)
                {
                    
                    lastError = err;
                    //break;
                }
            }

            if (lastError != null)
                _errorListView.ScrollToError(lastError);

            foreach (CompileError err in _errorListView.ErrorsList)
            {
                if (err.IsActive && err.File == CurrentCodeFile && err.BeginLine == Line)
                {
                    _errorListView.ScrollToError(err);
                    break;
                }
            }
        }

        private void SelectCurrentErrorForCaret()
        {
            _colorizeErrorForPopUp.Error = null;

            foreach (CompileError err in _errorListView.ErrorsList)
            {
                if (err.IsActive && err.File == CurrentCodeFile && err.BeginOffset == AvalonEditor.TextArea.Caret.Offset)
                {
                    _colorizeErrorForPopUp.Error = err;
                    break;
                }
            }

            _errorListView.SelectError(_colorizeErrorForPopUp.Error, false, false);
            Redraw();
        }

        private void GoToMouseError(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            _errorListView.SelectError(_colorizeErrorForPopUp.Error, true, false);
            AvalonEditor.PreviewMouseLeftButtonDown -= GoToMouseError;
            //mouseButtonEventArgs.Handled = true;
        }

        private ObservableCollection<CompileError.Type> _specialErrors;

        public ObservableCollection<CompileError.Type> SpecialErrors
        {
            get { return _specialErrors; }
            set
            {
                _specialErrors = value;
            }
        }

        #endregion

        // ----------------------------------------------------------------------------------------
        private InsightWindow _errorPopup;

        private void InitPopups()
        {
            AvalonEditor.MouseHover += (sender, args) =>
                {
                    if (CurrentCodeFile == null)
                        return; // Текущего файла нет совсем

                    TextViewPosition? pos = AvalonEditor.GetPositionFromPoint(Mouse.GetPosition(AvalonEditor.TextArea));
                    if (!pos.HasValue)
                        return; // Не в области редактора

                    int offset = AvalonEditor.Document.GetOffset(pos.Value.Location);
                    CompileError error = CurrentCodeFile.GetErrorByOffset(offset);

                    if (error == null)
                        return; // Ошибки в этом месте нет

                    _errorPopup = new InsightWindow(AvalonEditor.TextArea);
                    _errorPopup.StartOffset = offset + 1;
                    _errorPopup.EndOffset = offset + 1;
                    _errorPopup.Padding = new Thickness(10, 0, 10, 0);
                    _errorPopup.Content = error.GetDiagnostic();
                    _errorPopup.CloseAutomatically = false;

                    _colorizeErrorForPopUp.Error = error;   // Выделить цветом ошибку, над которой аэростат
                    Redraw();
                    AvalonEditor.TextArea.Cursor = Cursors.Hand;
                    AvalonEditor.PreviewMouseLeftButtonDown += GoToMouseError;

                    //_errorListView.SelectError(error, false);

                    _errorPopup.Show();
                };

            AvalonEditor.MouseHoverStopped += (sender, args) =>
                {
                    if (_errorPopup != null)
                    {
                        AvalonEditor.PreviewMouseLeftButtonDown -= GoToMouseError;
                        _colorizeErrorForPopUp.Error = null;
                        AvalonEditor.TextArea.Cursor = null;
                        Redraw();
                        
                        _errorPopup.Close();
                    }
                };
        }


        // ----------------------------------------------------------------------------------------
        #region Error colors
        public void SetAnotherErrorColor(CompileError.Type type)
        {
            _colorizeErrors.SpecialErrors.Add(type);
            _specialErrors.Add(type);
            Redraw();
        }

        public void UnSetAnotherErrorColor(CompileError.Type type)
        {
            _colorizeErrors.SpecialErrors.Remove(type);
            _specialErrors.Remove(type);
            Redraw();
        }

        public bool GetIfAnotherErrorColor(CompileError.Type type)
        {
            return _colorizeErrors.SpecialErrors.Contains(type);
        }
        #endregion

        // ----------------------------------------------------------------------------------------
        public int Line
        {
            get { return (CurrentCodeFile == null) ? 0 : AvalonEditor.TextArea.Caret.Line; }
        }

        public int Column
        {
            get { return (CurrentCodeFile == null) ? 0 : AvalonEditor.TextArea.Caret.Column; }
        }

        // ----------------------------------------------------------------------------------------
        public IEnumerable<CodeFile> OpenedFiles
        {
            get { return tabControl.Elements; }
        }

        public CodeFile CurrentCodeFile
        {
            get { return (tabControl != null) ? tabControl.CurrentCodeFile : null; }
        }

        public event EventHandler<FileArgs> FileOpened;
        protected void OnFileOpened(FileArgs e)
        {
            EventHandler<FileArgs> handler = FileOpened;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<FileArgs> FileClosed;
        protected void OnFileClosed(FileArgs e)
        {
            EventHandler<FileArgs> handler = FileClosed;
            if (handler != null) handler(this, e);
        }

        public event EventHandler CurrentFileChanged;
        protected virtual void OnCurrentFileChanged(object obj, EventArgs e)
        {
            EventHandler handler = CurrentFileChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public event EventHandler ActiveFileChanged;
        protected virtual void OnActiveFileChanged()
        {
            EventHandler handler = ActiveFileChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }


        private TextDocument _document;
        public TextDocument Document
        {
            get { return _document; }
            set
            {
                _document = value; 
                OnPropertyChanged("Document");
            }
        }

        private CodeFile _lastFile = null;

        private void SetNewCodeFile()
        {
            CodeFile newFile = CurrentCodeFile;

            // Отписываемся от событий предыдущего документа
            if (_lastFile != null)
            {
                _lastFile.LastCaretOffset = AvalonEditor.TextArea.Caret.Offset;
                _lastFile.TextChanged -= UpdateErrorsMarkers;
                _lastFile.TextChanged -= OnCurrentFileChanged;
                _lastFile.LineCountChanged -= UpdateVerticalPanels;
                _lastFile.Compiled -= FileOnCompiled;
            }

            // Выделять ошибки уже не надо
            _colorizeErrorForPopUp.Error = null;

            if (newFile != null)
            {
                newFile.TextChanged += UpdateErrorsMarkers;
                newFile.TextChanged += OnCurrentFileChanged;
                newFile.LineCountChanged += UpdateVerticalPanels; // для обновления вертикальных панелей
                newFile.Compiled += FileOnCompiled;

                // Обновим вертикальные панели
                CheckboxesPanel.UpdateCodeFile(CurrentCodeFile);
                CheckboxesPanel.UpdateGrid();

                CommentsPanel.UpdateCodeFile(CurrentCodeFile);
                CommentsPanel.UpdateGrid();

                ErrorMarkersPanel.UpdateCodeFile(CurrentCodeFile);
                ErrorMarkersPanel.UpdateGrid();

                BookmarksPanel.UpdateCodeFile(CurrentCodeFile);
                BookmarksPanel.UpdateGrid();

                AvalonEditor.DataContext = newFile;
                AvalonEditor.TextArea.Caret.Offset = newFile.LastCaretOffset;
            }
            else
            {
                AvalonEditor.DataContext = null;
                //Document = null;
            }

            OnCurrentFileChanged(null, null);

            // Установим видимости контролов
            // /*tabControl1.Visibility = */AvalonEditor.Visibility = (newFile == null) ? Visibility.Hidden : Visibility.Visible;

            // Set new last file
            _lastFile = newFile;
        }

        private void FileOnCompiled(object sender, EventArgs eventArgs)
        {
            ErrorMarkersPanel.UpdateView();
        }

        void UpdateVerticalPanels(object sender, EventArgs e)
        {
            CheckboxesPanel.UpdateGrid();
            CommentsPanel.UpdateGrid();
            ErrorMarkersPanel.UpdateGrid();
            BookmarksPanel.UpdateGrid();
        }

        private void UpdateErrorsMarkers(object sender, EventArgs eventArgs)
        {
            ErrorMarkersPanel.UpdateView();
        }

        public bool IsFileOpened(CodeFile file)
        {
            return tabControl.Elements.Count(codeFile => codeFile == file) > 0;
        }

        // ----------------------------------------------------------------------------------------
        public void UpdateBookmarks()
        {
            BookmarksPanel.UpdateView();
        }

        // ----------------------------------------------------------------------------------------
        #region TABS
        public void OpenTab(CodeFile file, bool setFocus)
        {
            // Если он уже есть в открытых файлах
            foreach (CodeFile tabFile in tabControl.Elements)
            {
                if (tabFile == file)
                {
                    // Уже добавлен, просто фокусируемся на нем
                    FocusTabOnFile(file, setFocus);
                    return;
                }
            }

            tabControl.AddTab(file);

            // Позиционироваться на только открытый файл
            FocusTabOnFile(file, setFocus);
        }

        public void CloseCurrentTab()
        {
            RemoveFromTabs(CurrentCodeFile);
        }

        public void RemoveFromTabs(CodeFile file)
        {
            tabControl.CloseTab(file);
        }

        public void CloseAllTabs()
        {
            tabControl.CloseAllTabs();
            //FocusTabOnFile(null, false);
        }

        public void GoToNextTab()
        {
            
        }

        private void FocusTabOnFile(CodeFile file, bool setFocus)
        {
            tabControl.FocusOn(file);

            if (setFocus)
            {
                Dispatcher.BeginInvoke(new Action(() => AvalonEditor.Focus())); // official hack! BAD!
                AvalonEditor.Focus();
            }
        }
        #endregion

        // ----------------------------------------------------------------------------------------
        public void SetCaretTo(CodeFile file, int offset)
        {
            OpenTab(file, false);

            AvalonEditor.CaretOffset = offset;
            AvalonEditor.Focus();
            ScrollTo(file, offset);

            SelectCurrentErrorForCaret();
        }

        public void ScrollTo(CodeFile file, int offset)
        {
            OpenTab(file, false);

            TextLocation location = file.Document.GetLocation(offset);

            Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() => AvalonEditor.ScrollToLine(location.Line)));
            Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() => AvalonEditor.ScrollTo(location.Line, location.Column)));
        }

        public void SetFocusInComments()
        {
            // Установить фокус в CommentsPanel
            CommentsPanel.SetFocusToLine(Line);
        }

        public void SetFocusInCheckboxes()
        {
            CheckboxesPanel.SetFocusToLine(Line);
        }

        
        // ----------------------------------------------------------------------------------------
        public void HighlightError(CodeFile file, CompileError error)
        {
            OpenTab(file, false);

            _colorizeErrorForPopUp.Error = error;
            Redraw();
        }

        public void UnHighlightError()
        {
            _colorizeErrorForPopUp.Error = null;
            Redraw();
        }


        // ----------------------------------------------------------------------------------------
        public void AddColorSegment(SolidColorBrush color)
        {
            if (CurrentCodeFile == null || AvalonEditor.SelectionLength == 0)
                return;

            // Стираем все, что было на этом месте
            CurrentCodeFile.EraseColorSegment(AvalonEditor.SelectionStart, AvalonEditor.SelectionStart + AvalonEditor.SelectionLength);

            // Рисуем там же новый цвет
            CurrentCodeFile.AddColorSegment(AvalonEditor.SelectionStart, AvalonEditor.SelectionStart + AvalonEditor.SelectionLength, color);

            // Ставим каретку в начало выделения
            AvalonEditor.SelectionLength = 0;
            Redraw();
        }

        public void EraseColorSegment()
        {
            if (CurrentCodeFile == null || AvalonEditor.SelectionLength == 0)
                return;

            CurrentCodeFile.EraseColorSegment(AvalonEditor.SelectionStart, AvalonEditor.SelectionStart + AvalonEditor.SelectionLength);

            // Ставим каретку в начало выделения
            AvalonEditor.SelectionLength = 0;
            Redraw();
        }

        public void ClearAllColorSegments()
        {
            if (CurrentCodeFile == null)
                return;

            CurrentCodeFile.ClearAllColorSegments();
            Redraw();
        }


        // ----------------------------------------------------------------------------------------
        private readonly ObjectBool _isEditorFocused = new ObjectBool();

        public bool IsEditorFocused { get; private set; }

        private void AvalonEditor_GotFocus(object sender, RoutedEventArgs e)
        {
            SelectCurrentErrorForCaret();
            OnPropertyChanged("IsEditorFocused");
        }

        private void AvalonEditor_LostFocus(object sender, RoutedEventArgs e)
        {
            OnPropertyChanged("IsEditorFocused");
        }


        // ----------------------------------------------------------------------------------------
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnAddBookmarkCommand(object sender, ExecutedRoutedEventArgs e)
        {
            CurrentCodeFile.AddBookmark(AvalonEditor.TextArea.Caret.Line, Convert.ToInt32(e.Parameter));
            BookmarksPanel.UpdateView();
        }

        private void OnGoToBookmarkCommand(object sender, ExecutedRoutedEventArgs e)
        {
            int key = Convert.ToInt32(e.Parameter);
            CodeFile.Bookmark bookmark = CurrentCodeFile.Bookmarks.Find(b => b.Key == key);
            if (bookmark != null)
            {
                // Если команда выполнилась, то фокус уже в редакторе, все нормально
                SetCaretTo(CurrentCodeFile, bookmark.Offset);
            }
        }

        private void DeleteBookmark_OnClick(object sender, RoutedEventArgs e)
        {
            TextBlock parent = ((sender as MenuItem).Parent as ContextMenu).PlacementTarget as TextBlock;
            CurrentCodeFile.DeleteBookmark((int)parent.Tag);
            UpdateBookmarks();
        }

        private void AddBookmark_OnClick(object sender, RoutedEventArgs e)
        {
            bool foundFirstEmptyKey = false;
            int key = 0;

            while (!foundFirstEmptyKey)
            {
                foundFirstEmptyKey = (CurrentCodeFile.Bookmarks.Count(b => b.Key == key) == 0);
                key++;
            }

            TextBlock parent = ((sender as MenuItem).Parent as ContextMenu).PlacementTarget as TextBlock;
            int line = (int)parent.Tag;

            CurrentCodeFile.AddBookmark(line, key - 1);
            codeEditor.UpdateBookmarks();
        }

        private void OnGoToNextTabCommand(object sender, ExecutedRoutedEventArgs e)
        {
            tabControl.GoToNextTab();
        }

        private void CodeEditor_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!AvalonEditor.IsFocused)
                AvalonEditor.Focus();
        }

        private void SetAllCheckboxes_Click(object sender, RoutedEventArgs e)
        {
            // Удаляем все секбоксы в текущем файле
            CurrentCodeFile.Checkboxes.Clear();

            // Поставить чекбокс в начал каждой линии
            for (int i = 1; i < CurrentCodeFile.Document.LineCount + 1; i++ )
                CurrentCodeFile.AddCheckbox(i);

            CheckboxesPanel.UpdateView();
        }

        private void ClearAllCheckboxes_Click(object sender, RoutedEventArgs e)
        {
            // Удаляем все секбоксы в текущем файле
            CurrentCodeFile.Checkboxes.Clear();

            CheckboxesPanel.UpdateView();
        }
    } // class CodeEditor

    // --------------------------------------------------------------------------------------------
    abstract class VerticalEditorPanel<T> : ScrollViewer
        where T : FrameworkElement, new()
    {
        protected Grid ElementsGrid = new Grid();

        protected const double DefaultLineHeight = 14.726;
        protected IList<T> Elements = new List<T>();
        protected bool _isUpdatePhase = false;
        protected CodeFile CurrentCodeFile;
        protected CodeEditorView _codeEditor;

        public void Init(CodeEditorView codeEditor)
        {
            _codeEditor = codeEditor;
            // Only 1 colum
            //ElementsGrid.ColumnDefinitions.Add(new ColumnDefinition(){Width = new GridLength(1, GridUnitType.Star)});
            Background = new SolidColorBrush(Colors.Transparent);
            
            // Set base scroll viewer content
            Content = ElementsGrid;
            //stackPanel.Content = ElementsGrid;
            //stackPanel.Children.Add(ElementsGrid);

            UpdateGrid();
            UpdateView();
        }

        public void UpdateCodeFile(CodeFile codeFile)
        {
            CurrentCodeFile = codeFile;
        }

        protected abstract void SetEvents(T elem);
        

        // Отрегулировать количество строк
        public void UpdateGrid()
        {
            int lineCount = (CurrentCodeFile != null) ? CurrentCodeFile.Document.LineCount : 0; // Если документа ноль, то считаем, что строк = 0

            // Добавляем до нужного количества
            while (ElementsGrid.RowDefinitions.Count < lineCount)
            {
                // Tag - это номер элемента в списке, начиная с 1
                T elem = new T() { Tag = Elements.Count + 1, Height = DefaultLineHeight};
                SetEvents(elem);
                
                Elements.Add(elem);
                //ICSharpCode.AvalonEdit.Rendering.VisualLine line = AvalonEditor.TextArea.TextView.GetVisualLine(1);

                ElementsGrid.RowDefinitions.Add(new RowDefinition() { MaxHeight = DefaultLineHeight});
                ElementsGrid.Children.Add(elem);

                Grid.SetRow(elem, ElementsGrid.RowDefinitions.Count - 1);
                //Grid.SetColumn(elem, 0);
            }

            // Открываем нужное количество
            for (int line = 0; line < Elements.Count; line++)
            {
                if (line < lineCount)
                    Elements[line].Visibility = Visibility.Visible;
                else
                    Elements[line].Visibility = Visibility.Collapsed;
            }

            UpdateView();
        }

        // Обновить содержимое каждой строки
        public void UpdateView()
        {
            if (CurrentCodeFile == null)
                return;

            _isUpdatePhase = true;

            SetDefaultState();
            SetState();

            _isUpdatePhase = false;
        }

        protected abstract void SetState();
        protected abstract void SetDefaultState();
    }

    // --------------------------------------------------------------------------------------------
    class CheckboxesPanel : VerticalEditorPanel<CheckboxesPanel.LineCheckBox>
    {
        public class LineCheckBox : CheckBox
        {
            private CheckboxesPanel _panel;

            public void Init(CheckboxesPanel panel)
            {
                _panel = panel;
            }

            protected override void OnChecked(RoutedEventArgs e)
            {
                if (!_panel._isUpdatePhase)
                    _panel.CurrentCodeFile.AddCheckbox((int)Tag);

                base.OnChecked(e);
            }

            protected override void OnUnchecked(RoutedEventArgs e)
            {
                if (!_panel._isUpdatePhase)
                    _panel.CurrentCodeFile.DeleteCheckbox((int)Tag);

                base.OnUnchecked(e);
            }
        }

        private TextEditor _avalon;

        public void InitCheckboxes(TextEditor avalon)
        {
            _avalon = avalon;
        }

        protected override void SetEvents(LineCheckBox elem)
        {
            elem.Init(this);
            elem.GotFocus += (sender, args) =>
            {
                // Устанавливаем каретку только в том случае, если текущая где-то ни на этой строке
                DocumentLine currentLine = _avalon.Document.GetLineByOffset(_avalon.CaretOffset);
                DocumentLine newLine = _avalon.Document.GetLineByNumber((int) elem.Tag);

                if (currentLine.LineNumber != newLine.LineNumber)
                {
                    _avalon.CaretOffset = newLine.Offset;
                }
            };
        }

        protected override void SetDefaultState()
        {
            // Устанавливаем все чекбоксы в офф
            foreach (LineCheckBox checkBox in Elements)
                if (checkBox.IsChecked.HasValue && checkBox.IsChecked.Value)
                    checkBox.IsChecked = false;
        }

        protected override void SetState()
        {
            foreach (TextAnchor anchor in CurrentCodeFile.Checkboxes)
                if (!anchor.IsDeleted)
                    Elements[anchor.Line - 1].IsChecked = true;
        }

        public void SetFocusToLine(int line)
        {
            Elements[line - 1].Focus();
        }
    }

    // --------------------------------------------------------------------------------------------
    class CommentsPanel : VerticalEditorPanel<CommentsPanel.LineCommentTextbox>
    {
        internal class LineCommentTextbox : TextBox
        {
            private CommentsPanel _panel;

            public void Init(CommentsPanel panel)
            {
                Style = panel.FindResource("TextBoxTemplate") as Style;
                _panel = panel;
                //CaretBrush = panel.FindResource("CaretBrush") as Brush;
            }

            protected override void OnTextChanged(TextChangedEventArgs e)
            {
                if (!_panel._isUpdatePhase)
                    _panel.CurrentCodeFile.UpdateComment((int)Tag, Text);

                base.OnTextChanged(e);
            }
        }

        private XBackgroundRenderer _renderer;
        private TextEditor _avalon;
        private Brush _caretBrush;

        public void InitComments(XBackgroundRenderer renderer, TextEditor avalon)
        {
            _renderer = renderer;
            _avalon = avalon;

            // Events to ScrollViewer
            MouseEnter += (sender, args) => Cursor = Cursor = Cursors.IBeam;
            MouseLeave += (sender, args) => Cursor = null;

            PreviewMouseLeftButtonDown += (sender, args) =>
            {
                // Проверяем попадание в сетку с комментариями
                // Если попадания нет, то юзер тыкнул куда-то в свободное место
                // Тогда поставим фокус в последний комментарий, в самый конец

                Point pt = args.GetPosition((UIElement) sender);
                HitTestResult res = VisualTreeHelper.HitTest(ElementsGrid, pt);

                if (res == null && Elements.Count > 0)
                {
                    Elements[Elements.Count - 1].CaretIndex = Elements[Elements.Count - 1].Text.Length;
                    Dispatcher.BeginInvoke(new Action(() => Elements[Elements.Count - 1].Focus())); // official hack! BAD!
                }
            };
        }

        protected override void SetEvents(LineCommentTextbox elem)
        {
            elem.Init(this);

            elem.PreviewKeyDown += (sender, args) =>
            {
                if (args.Key == Key.Down)
                {
                    int nextId = (int) elem.Tag;
                    if (nextId < Elements.Count)
                        Elements[nextId].Focus();
                }
                else if (args.Key == Key.Up)
                {
                    int nextId = (int)elem.Tag - 2;
                    if (nextId >= 0)
                        Elements[nextId].Focus();
                }
                else if (args.Key == Key.Left)
                {
                    if (elem.CaretIndex == 0)
                    {
                        // Если у левого края, переходим в редактор
                        _avalon.Focus();
                        args.Handled = true; // Чтоб стрелочка в редактор не уходила
                    }
                }
            };

            elem.GotKeyboardFocus += (sender, args) =>
            {
                // Устанавливаем каретку только в том случае, если текущая где-то ни на этой строке
                DocumentLine currentLine = _avalon.Document.GetLineByOffset(_avalon.CaretOffset);
                DocumentLine newLine = _avalon.Document.GetLineByNumber((int) elem.Tag);

                if (currentLine.LineNumber != newLine.LineNumber)
                {
                    _avalon.CaretOffset = newLine.Offset;
                    //_avalon.ScrollToLine(newLine.LineNumber);
                }
            };
        }

        protected override void SetDefaultState()
        {
            // Стираем все комменты
            foreach (LineCommentTextbox box in Elements)
                box.Text = "";
        }

        protected override void SetState()
        {
            foreach (CodeFile.Comment comm in CurrentCodeFile.Comments)
                if (!comm.Anchor.IsDeleted)
                    Elements[comm.Anchor.Line - 1].Text = comm.Text;
        }

        private int _lastLine = -1;

        private readonly SolidColorBrush _colorBrush = IDEState.Get().EditorColor_CurrentLineBrush;
        private readonly SolidColorBrush _transparentBrush = new SolidColorBrush(Colors.Transparent);

        public void HighlightLine(int line)
        {
            line--; // Нумерация с нуля

            if (_lastLine != -1)
                Elements[_lastLine].Background = _transparentBrush;

            if (line != -1)
                Elements[line].Background = _colorBrush;

            _lastLine = line;
        }

        public void SetFocusToLine(int line)
        {
            Elements[line - 1].Focus();
        }
    }

    // --------------------------------------------------------------------------------------------
    class ErrorMarkersPanel : VerticalEditorPanel<ErrorMarkersPanel.ErrorMarkerTextBlock>
    {
        internal class ErrorMarkerTextBlock : TextBlock
        {
            public ErrorMarkerTextBlock()
            {
                TextAlignment = TextAlignment.Center;
                VerticalAlignment = VerticalAlignment.Center;
                SetBinding(ToolTipProperty,
                           new Binding()
                           {
                               Source = Application.Current.Resources["LocalString"],
                               Path = new PropertyPath("Dict[Editor_ErrorsMarkerTooltip]")
                           });
                ToolTipService.SetIsEnabled(this, false);
            }
        }

        protected override void SetEvents(ErrorMarkerTextBlock elem)
        {
        }

        private int GetErrorsInLine(int line)
        {
            if (CurrentCodeFile == null || CurrentCodeFile.ErrorsList == null)
                return 0;
            else
                return CurrentCodeFile.ErrorsList.Count(error => error.IsActive && error.BeginLine == line);
        }

        private SolidColorBrush brushNoError = new SolidColorBrush(Colors.Transparent);
        private SolidColorBrush brushHasError = new SolidColorBrush(Colors.Red);

        protected override void SetDefaultState()
        {
            // Уложились в один проход
        }

        protected override void SetState()
        {
            int line = 1;
            foreach (ErrorMarkerTextBlock s in Elements)
            {
                int errorsNum = GetErrorsInLine(line);
                if (errorsNum == 0)
                {
                    s.Text = "";
                    ToolTipService.SetIsEnabled(s, false);
                    s.Background = brushNoError;
                }
                else
                {
                    s.Text = errorsNum.ToString();
                    ToolTipService.SetIsEnabled(s, true);
                    s.Background = brushHasError;
                }

                line++;
            }
        }
    }

    // --------------------------------------------------------------------------------------------
    class BookmarksPanel : VerticalEditorPanel<TextBlock>
    {
        public BookmarksPanel()
        {
            ElementsGrid.Background = brushBack;
        }

        private ContextMenu _contextMenu;
        private ContextMenu _emptyContextMenu;

        public void Init2(ContextMenu contextMenu, ContextMenu emptyContextMenu)
        {
            Binding binding = new Binding() { Source = Application.Current.Resources["LocalString"], Path = new PropertyPath("Dict[Editor_BookmarksPanel]")};
            ElementsGrid.SetBinding(Grid.ToolTipProperty, binding);

            _contextMenu = contextMenu;
            _emptyContextMenu = emptyContextMenu;
        }

        protected override void SetEvents(TextBlock elem)
        {
            elem.SetBinding(TextBlock.ToolTipProperty,
                           new Binding()
                           {
                               Source = Application.Current.Resources["LocalString"],
                               Path = new PropertyPath("Dict[Editor_BookmarkTooltip]")
                           });
            ToolTipService.SetIsEnabled(elem, false);

            elem.TextAlignment = TextAlignment.Center;
            elem.ContextMenu = _emptyContextMenu;
            
            elem.MouseDown += (sender, args) =>
            {
                // Double click
                if (args.ClickCount == 2)
                {
                    TextBlock b = sender as TextBlock;   
                    if (b.Text != "")
                    {
                        CurrentCodeFile.DeleteBookmark((int) b.Tag);
                        UpdateView();
                    }
                    else
                    {
                        bool foundFirstEmptyKey = false;
                        int key = 0;

                        while (!foundFirstEmptyKey)
                        {
                            foundFirstEmptyKey = (CurrentCodeFile.Bookmarks.Count(bookmark => bookmark.Key == key) == 0);
                            key++;
                        }

                        int line = (int)b.Tag;

                        CurrentCodeFile.AddBookmark(line, key - 1);
                        UpdateView();
                    }
                }
            };
        }

        protected override void SetDefaultState()
        {
            // Устанавливаем все чекбоксы в офф
            foreach (TextBlock block in Elements)
                if (block.Text != "")
                {
                    block.Text = "";
                    ToolTipService.SetIsEnabled(block, false);
                    block.Background = brushTransparent;
                    //block.Foreground =;
                    block.ContextMenu = _emptyContextMenu;
                }
        }

        private SolidColorBrush brushBack = new SolidColorBrush(Color.FromRgb(230, 230, 230));
        private SolidColorBrush brushBookmarkB = IDEState.Get().BookmarkBrushB;
        private SolidColorBrush brushBookmarkF = IDEState.Get().BookmarkBrushF;
        private SolidColorBrush brushTransparent = new SolidColorBrush();

        protected override void SetState()
        {
            foreach (CodeFile.Bookmark bookmark in CurrentCodeFile.Bookmarks)
                if (bookmark.IsActive)
                {
                    Elements[bookmark.Line - 1].Text = bookmark.Key.ToString();
                    ToolTipService.SetIsEnabled(Elements[bookmark.Line - 1], true);
                    Elements[bookmark.Line - 1].Background = brushBookmarkB;
                    Elements[bookmark.Line - 1].Foreground = brushBookmarkF;
                    Elements[bookmark.Line - 1].ContextMenu = _contextMenu;
                }
        }
    }

    // --------------------------------------------------------------------------------------------
    // Раскраска ошибок (каждой ошибки)
    class ColorizeErrors : DocumentColorizingTransformer
    {
        private CodeEditorView _codeEditorView;

        private SolidColorBrush _brushErrorB = new SolidColorBrush();
        private SolidColorBrush _brushErrorF = new SolidColorBrush();
        private SolidColorBrush _brushSpecialErrorB = new SolidColorBrush();
        private SolidColorBrush _brushSpecialErrorF = new SolidColorBrush();

        public HashSet<CompileError.Type> SpecialErrors = new HashSet<CompileError.Type>();

        public ColorizeErrors(CodeEditorView codeEditorView)
        {
            _codeEditorView = codeEditorView;

            // Подписывамся на обновления цветов
            var state = Application.Current.Resources["IDEState"] as IDEState;
            UpdateColors(state);

            state.PropertyChanged += (sender, args) =>
                {
                    if (args.PropertyName == "EditorColor_ErrorB" || args.PropertyName == "EditorColor_ErrorF" ||
                        args.PropertyName == "EditorColor_SpecialErrorB" || args.PropertyName == "EditorColor_SpecialErrorF")
                    {
                        UpdateColors(state);
                    }
                };
        }

        private void UpdateColors(IDEState state)
        {
            _brushErrorB.Color = state.EditorColor_ErrorB;
            _brushErrorF.Color = state.EditorColor_ErrorF;
            _brushSpecialErrorB.Color = state.EditorColor_SpecialErrorB;
            _brushSpecialErrorF.Color = state.EditorColor_SpecialErrorF;
        }

        protected override void ColorizeLine(DocumentLine line)
        {
            if (_codeEditorView.CurrentCodeFile == null || _codeEditorView.CurrentCodeFile.ErrorsList == null)
                return;

            // Выбираем те ошибки, что есть в текущей линии
            foreach (CompileError err in _codeEditorView.CurrentCodeFile.ErrorsList)
                if (err.IsActive && err.BeginLine == line.LineNumber)
                {
                    ChangeLinePart(err.BeginOffset,
                                   err.EndOffset,
                                   elem =>
                                   {
                                       elem.TextRunProperties.SetBackgroundBrush(SpecialErrors.Contains(err.ErrorType)
                                                                                     ? _brushSpecialErrorB
                                                                                     : _brushErrorB);

                                       elem.TextRunProperties.SetForegroundBrush(SpecialErrors.Contains(err.ErrorType)
                                                                                     ? _brushSpecialErrorF
                                                                                     : _brushErrorF);
                                   });
                }
        }
    }

    // --------------------------------------------------------------------------------------------
    // раскрашка ошибки (по умолч. синим) для попапа
    class ColorizeErrorForPopup : DocumentColorizingTransformer
    {
        private CompileError _error;
        public CompileError Error
        {
            get { return _error; }
            set { _error = value; }
        }

        private SolidColorBrush _brushPopupErrorB = new SolidColorBrush();
        private SolidColorBrush _brushPopupErrorF = new SolidColorBrush();

        public ColorizeErrorForPopup()
        {
            var state = Application.Current.Resources["IDEState"] as IDEState;
            UpdateColors(state);

            state.PropertyChanged += (sender, args) =>
                {
                    if (args.PropertyName == "EditorColor_PopupErrorB" || args.PropertyName == "EditorColor_PopupErrorF")
                        UpdateColors(state);
                };
        }

        private void UpdateColors(IDEState state)
        {
            _brushPopupErrorB.Color = state.EditorColor_PopupErrorB;
            _brushPopupErrorF.Color = state.EditorColor_PopupErrorF;
        }

        protected override void ColorizeLine(DocumentLine line)
        {
            if (Error != null && Error.IsActive && Error.BeginLine == line.LineNumber)
            {
                ChangeLinePart(Error.BeginOffset,
                               Error.EndOffset,
                               elem =>
                                   {
                                       elem.TextRunProperties.SetBackgroundBrush(_brushPopupErrorB);
                                       elem.TextRunProperties.SetForegroundBrush(_brushPopupErrorF);
                                   });
            }
        }
    }

    // --------------------------------------------------------------------------------------------
    // РАскраска цветовых сегментов
    class ColorizeColorSegments : DocumentColorizingTransformer
    {
        private CodeEditorView _codeEditorView;

        public ColorizeColorSegments(CodeEditorView codeEditorView)
        {
            _codeEditorView = codeEditorView;
        }

        protected override void ColorizeLine(DocumentLine line)
        {
            if (_codeEditorView.CurrentCodeFile == null)
                return;

            foreach (CodeFile.ColorSegment segment in _codeEditorView.CurrentCodeFile.ColorSegments)
            {
                if (!segment.IsAcive())
                    continue;

                // Если сегмент имеет отношение к текущей линии
                // segment.Start.Line  <= line.lineNumber <= segment.End.Line

                if (segment.Start.Line <= line.LineNumber && line.LineNumber <= segment.End.Line)
                {
                    int startOffset = (segment.Start.Line == line.LineNumber) ? segment.Start.Offset : line.Offset;
                    int endOffset = (segment.End.Line == line.LineNumber) ? segment.End.Offset : line.EndOffset;

                    if (endOffset > startOffset)
                        ChangeLinePart(startOffset, endOffset, elem => elem.TextRunProperties.SetBackgroundBrush(segment.Color));
                }
            }
        }
    }

    // --------------------------------------------------------------------------------------------
    // Подсветка текущей строки (в т.ч. строки комментария)
    class XBackgroundRenderer : IBackgroundRenderer
    {
        private TextEditor _editor;
        private ObjectBool _isFocused;
        private CommentsPanel _commentsPanel;

        public int CurrentLine { get; set; }

        private SolidColorBrush _brush;
        private Pen _pen;

        public XBackgroundRenderer(TextEditor e, ObjectBool isFocused, CommentsPanel commentsPanel)
        {
            _isFocused = isFocused;
            _editor = e;
            _commentsPanel = commentsPanel;

            _brush = IDEState.Get().EditorColor_CurrentLineBrush;
            _pen = new Pen(_brush, 0);
        }

        public KnownLayer Layer
        {
            get { return KnownLayer.Background; }
        }

        public void Draw(TextView textView, DrawingContext drawingContext)
        {
            if (textView.Document == null /*|| !_isFocused.Value*/)
            {
                _commentsPanel.HighlightLine(0);
                return;
            }

            DocumentLine line = _editor.Document.GetLineByOffset(_editor.CaretOffset);

            _commentsPanel.HighlightLine(line.LineNumber);

            textView.EnsureVisualLines();
            foreach (Rect r in BackgroundGeometryBuilder.GetRectsForSegment(textView, new TextSegment() { StartOffset = line.Offset }))
            {
                drawingContext.DrawRectangle(
                    _brush, // Строка
                    _pen,   // Рамка
                    new Rect(r.Location, new Size(Math.Max(_editor.ExtentWidth, _editor.ViewportWidth), r.Height))
                );
            }
        }
    }

    class FileNameConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string name = (string) values[0];
            bool isChanged = (bool)values[1];

            return isChanged ? name + "*" : name;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    class VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool focused = (bool) value;

            if (focused)
                return Visibility.Visible;
            else
                return Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    class HaveFileConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value != null) ? Visibility.Visible : Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
