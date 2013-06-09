using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Translator
{
    class LocalStringDictionaryRus : LocalStringDictionary
    {
        public LocalStringDictionaryRus()
        {
            V = new StringDictionary
                {
                    {"DefautlFileName", "Программа"},

                    {"Err_t0", "Ошибка #0"},
                    {"Err_t1", "Ошибка #1"},
                    {"Err_t2", "Ошибка #2"},
                    {"Err_t3", "Ошибка #3"},
                    {"Err_t4", "Ошибка #4"},
                    {"Err_t5", "Ошибка #5"},
                    {"Err_t6", "Ошибка #6"},
                    {"Err_t7", "Ошибка #7"},
                    {"Err_t8", "Ошибка #8"},
                    {"Err_t9", "Ошибка #9"},

                    {"Err_0_short", "Коротко об ошибке 0"},
                    {"Err_1_short", "Коротко об ошибке 1"},
                    {"Err_2_short", "Коротко об ошибке 2"},
                    {"Err_3_short", "Коротко об ошибке 3"},
                    {"Err_4_short", "Коротко об ошибке 4"},
                    {"Err_5_short", "Коротко об ошибке 5"},
                    {"Err_6_short", "Коротко об ошибке 6"},
                    {"Err_7_short", "Коротко об ошибке 7"},
                    {"Err_8_short", "Коротко об ошибке 8"},
                    {"Err_9_short", "Коротко об ошибке 9"},

                    {"Err_0_full", "Очень очень очень подробное подробнейшее описание ошибки 0"},
                    {"Err_1_full", "Очень очень очень подробное подробнейшее описание ошибки 1"},
                    {"Err_2_full", "Очень очень очень подробное подробнейшее описание ошибки 2"},
                    {"Err_3_full", "Очень очень очень подробное подробнейшее описание ошибки 3"},
                    {"Err_4_full", "Очень очень очень подробное подробнейшее описание ошибки 4"},
                    {"Err_5_full", "Очень очень очень подробное подробнейшее описание ошибки 5"},
                    {"Err_6_full", "Очень очень очень подробное подробнейшее описание ошибки 6"},
                    {"Err_7_full", "Очень очень очень подробное подробнейшее описание ошибки 7"},
                    {"Err_8_full", "Очень очень очень подробное подробнейшее описание ошибки 8"},
                    {"Err_9_full", "Очень очень очень подробное подробнейшее описание ошибки 9"},

                    {"Line", "Строка"},
                    {"Column", "Столбец"},

                    {"WorkDirs", "Рабочие каталоги"},

                    {"Byte_short", "Б"},
                    {"KB_short", "КБ"},
                    {"MB_short", "МБ"},
                    {"GB_short", "ГБ"},

                    
                    {"Button_NewFile_Tooltip", "Новая программа"},
                    {"Button_NewProject_Tooltip", "Новый проект"},
                    {"Button_AddExistingFile_Tooltip", "Присоеднить существующую программу ..."},
                    {"Button_OpenProject_Tooltip", "Открыть проект"},
                    {"Button_SaveCurrentFile_Tooltip", "Сохранить текущую программу"},
                    {"Button_SaveAllFiles_Tooltip", "Сохранить все открытые программы"},
                    {"Button_Compile", "Транслировать проект"},
                    {"Button_Bookmarks", "Поставить закладку"},
                    {"Button_Colors", "Выделить цветом фрагмент"},
                    {"Button_Select", "Выделить"},

                    // Up menu in MainWindow
                    {"Menu_File", "Программа"},
                    {"Menu_FileT", "Действия с текущей программой"},
                    
                    {"Menu_New", "Новый"},
                    {"Menu_NewT", "Новый"},
                    
                    {"Menu_NewFile", "Новая программа"},
                    {"Menu_NewFileT", "Создать новую программу"},
                    
                    {"Menu_NewProject", "Новый проект ..."},
                    {"Menu_NewProjectT", "Создать новый проект"},
                    
                    {"Menu_Open", "Открыть..."},
                    {"Menu_OpenT", "Открыть..."},

                    {"Menu_AddExistingFile", "Присоеднить существующую программу ..."},
                    {"Menu_AddExistingFileT", "Присоеднить к проекту существующую программу"},
                    {"Menu_OpenProject", "Открыть проект ..."},
                    {"Menu_OpenProjectT", "Открыть проект"},


                    {"Menu_Close", "Закрыть текущую программу"},
                    {"Menu_CloseT", "Закрыть текущую программу"},

                    {"Menu_CloseAllFiles", "Закрыть все программы"},
                    {"Menu_CloseAllFilesT", "Закрыть все программы"},

                    {"Menu_CloseProject", "Закрыть проект"},
                    {"Menu_CloseProjectT", "Закрыть текущий проект"},

                    
                    {"Menu_File_Rename", "Переименовать"},
                    {"Menu_File_RenameT", "Переименовать текущую программу"},
                    
                    {"Menu_File_Move", "Переместить ..."},
                    {"Menu_File_MoveT", "Переместить текущую программу"},

                    {"Menu_File_NewCopy", "Создать копию"},
                    {"Menu_File_NewCopyT", "Создать копию текущей программы"},

                    {"Menu_File_NewVersion", "Создать версию"},
                    {"Menu_File_NewVersionT", "Создать новую версию текущей программы"},

                    {"Menu_File_GoToVersion", "Вернуться к версии ..."},
                    {"Menu_File_GoToVersionT", "Вернуть текущую программу к версии"},

                    {"Menu_File_RecentProjects", "Последние проекты"},
                    {"Menu_File_RecentProjectsT", "Последние проекты"},

                    {"Menu_Exit", "Выход"},
                    {"Menu_ExitT", "Выйти из программы"},


                    {"Menu_Edit", "Правка"},
                    {"Menu_EditT", "Правка текста программы"},

                    {"Menu_Edit_Cut", "Вырезать"},
                    {"Menu_Edit_CutT", "Вырезать выделенный фрагмент"},

                    {"Menu_Edit_Copy", "Копировать"},
                    {"Menu_Edit_CopyT", "Копировать выделенный фрагмент в буфер обмена"},

                    {"Menu_Edit_Paste", "Вставить"},
                    {"Menu_Edit_PasteT", "Вставить фрагмент из буфера обмена"},

                    {"Menu_View", "Вид"},
                    {"Menu_ViewT", "Виды программы"},

                    {"Menu_View_Code", "Код"},
                    {"Menu_View_CodeT", "Перейти к коду"},

                    {"Menu_View_CodeCheckboxes", "Код (чекбоксы)"},
                    {"Menu_View_CodeCheckboxesT", "Перейти к чекбоксам кода"},

                    {"Menu_View_CodeComments", "Код (комментарии)"},
                    {"Menu_View_CodeCommentsT", "Перейти к комментариям кода"},

                    {"Menu_View_Errors", "Список ошибок"},
                    {"Menu_View_ErrorsT", "Перейти к списку ошибок"},

                    {"Menu_View_Hotkeys", "Горячие клавиши"},
                    {"Menu_View_HotkeysT", "Перейти к списку горячих клавиш"},

                    {"Menu_View_SolutionExplorer", "Обозреватель проекта"},
                    {"Menu_View_SolutionExplorerT", "Перейти к обозревателю проекта"},

                    {"Menu_View_Bookmarks", "Поставить закладку"},
                    {"Menu_View_BookmarksT", "Поставить закладку"},

                    {"Menu_View_Contents", "Оглавление"},
                    {"Menu_View_ContentsT", "Перейти к оглавлению программы"},

                    {"Menu_Compile", "Компилировать"},
                    {"Menu_CompileT", "Действия по компиляции"},

                    {"Menu_Compile_Project", "Весь проект"},
                    {"Menu_Compile_ProjectT", "Скомпилировать весь проект"},

                    {"Menu_Compile_CurrentFile", "Только текущий файл"},
                    {"Menu_Compile_CurrentFileT", "Компилировать только текущий файл"},
                    

                    {"Menu_Tools", "Настройки"},
                    {"Menu_ToolsT", "Настройки программы"},
                    {"Menu_Tools_Settings", "Настройки"},
                    {"Menu_Tools_SettingsT", "Настройки программы"},

                    {"Menu_Window", "Окно"},
                    {"Menu_WindowT", "Действия с открытыми окнами"},

                    {"Menu_Window_Windows", "Окна..."},
                    {"Menu_Window_WindowsT", "Список всех открытх окон"},

                    {"Menu_Service", "Сервис"},
                    {"Menu_ServiceT", "Сервис"},

                    {"Menu_Help", "Помощь"},
                    {"Menu_HelpT", "Помощь по программе"},

                    {"Menu_Help_Help", "Показать справку"},
                    {"Menu_Help_HelpT", "Показать справку по программе"},

                    {"Menu_Help_StartPage", "Показать стартовое окно"},
                    {"Menu_Help_StartPageT", "Показать стартовое окно"},

                    {"Bookmarks_No", "В этом файле закладок нет"},

                    {"ProjectMenu_BuildProject", "Скомпилировать проект"},
                    {"ProjectMenu_Add", "Добавить"},
                    {"ProjectMenu_Add_NewFile", "Добавить новую программу"},
                    {"ProjectMenu_Add_ExistFile", "Добавить существующую программу"},

                    {"ProjectMenu_OpenFile", "Открыть"},
                    {"ProjectMenu_DeleteFromDisk", "Удалить с диска"},
                    {"ProjectMenu_Exclude", "Исключить из проекта"},
                    {"ProjectMenu_Rename", "Переименовать"},
                    {"ProjectMenu_Move", "Переместить ..."},
                    {"ProjectMenu_Copy", "Сделать копию"},
                    {"ProjectMenu_ProjectSettings", "Свойства"},
                    {"ProjectMenu_NewVer", "Новая версия"},
                    {"ProjectMenu_GoToVer", "Выбрать версию..."},
                    {"ProjectMenu_Properties", "Свойства..."},

                    {"Editor_ClearCheckboxes", "Сбросить все"},
                    {"Editor_SetAllCheckboxes", "Установить все"},
                    {"Editor_ClearSorting", "Убрать сортировку"},

                    {"Editor_BookmarksPanel", "Панель закладок"},
                    {"Editor_AddBookmark", "Добавить закладку"},
                    {"Editor_DeleteBookmark", "Удалить"},
                    {"Editor_ErrorsMarkerTooltip", "Количество ошибок в строке"},
                    {"Editor_BookmarkTooltip", "Закладка"},

                        // NEW PROJECT WINDOW
                    {"Header_NewProject", "Новый проект"},
                    {"NewProjectWindow_Name", "Имя:"},
                    {"NewProjectWindow_Path", "Путь:"},
                    {"NewProjectWindow_CreateDir", "Создать новую папку для проекта"},
                    {"NewProjectWindow_Browse", "Открыть"},
                    {"NewProjectWindow_OK", "OK"},
                    {"NewProjectWindow_Cancel", "Отмена"},
                    {"NewProjectWindow_DirExists", "Такая папка уже есть. Рекомендуется выбрать другое имя проекта." + Environment.NewLine + Environment.NewLine + "Все равно создать?"},
                    {"NewProjectWindow_Attention", "Внимание!"},

                        // Add new file window
                    {"AddNewFileWindow_FileName", "Имя файла:"},
                    {"AddNewFileWindow_OkButton", "OK"},
                    {"AddNewFileWindow_CancelButton", "Отмена"},
                    {"AddNewFileWindow_Title", "Новый файл"},
                    {"AddNewFileWindow_Path", "Путь:"},
                    {"AddNewFileWindow_AddWorkDir", "(новый)"},
                        
                    {"OpenProjectWindow_Header_Name", "Имя"},
                    {"OpenProjectWindow_Header_Type", "Тип"},
                    {"OpenProjectWindow_Header_Changed", "Дата изменения"},
                    {"OpenProjectWindow_Header_Size", "Размер"},
                    {"OpenProjectWindow_ElementType_File", "Файл"},
                    {"OpenProjectWindow_ElementType_Dir", "Папка"},
                    {"OpenProjectWindow_ElementType_Drive", "Диск"},
                    {"OpenProjectWindow_Favourite", "Избранное"},
                    {"OpenProjectWindow_OK", "OK"},
                    {"OpenProjectWindow_Cancel", "Отмена"},

                    {"OpenFileWindow_Title", "Добавить существующую программу"},


                    {"OpenFileWindowBase_Up", "(вверх)"},
                    {"OpenFIleWindowBase_Filter", "Фильтр:"},
                    {"OpenFileWindowBase_Preview", "Предпросмотр"},
                    {"OpenFileWindowBase_Preview_NotAvialable", "(предпросмотр недоступен)"},

                    // OpenProjetcWindow
                    {"OpenProjectWindow_Title", "Открыть проект"},

                    {"OpenProject_ErrorBox_Title", "Ошибка"},
                    {"OpenProject_ErrorBox_Text1", "Не удалось загрузить проект "},
                    {"OpenProject_ErrorBox_Text2", "Возможно, файл поврежден."},

                    {"ProjectMenu_SaveProject", "Сохранить проект"},
                    
                    // Settings window
                    {"SettingsWindow_Title", "Настройки"},
                    {"SettingsWindow_Lang_Group", "Языковые настройки"},
                    {"SettingsWindow_Lang", "Язык"},

                    {"SettingsWindow_Color_Group", "Цвета"},
                    {"SettingsWindow_Color_Background", "Фон"},
                    {"SettingsWindow_Color_Foreground", "Шрифт"},
                    {"SettingsWindow_Color_Error", "Ошибка"},
                    {"SettingsWindow_Color_SpecialError", "Отмеченная ошибка"},
                    {"SettingsWindow_Color_PopupError", "Ошибка с аеростатом"},
                    {"SettingsWindow_Color_Bookmark", "Закладка"},
                    {"SettingsWindow_Color_FocusedHeader", "Заголовок панели в фокусе"},
                    {"SettingsWindow_Color_NotFocusedHeader", "Заголовок панели без фокуса"},
                    {"SettingsWindow_Color_WindowBackground", "Общий фон"},

                    {"SettingsWindow_ColorEditor_Group", "Цвета редактора"},
                    {"SettingsWindow_ColorEditor_Line", "Текущая строка"},
                    {"SettingsWindow_ColorEditor_MouseHoverFocused", "Вкладка при наведнии, с фокусом"},
                    {"SettingsWindow_ColorEditor_MouseHoverNotFocused", "Вкладка при наведнии, без фокуса"},


                    {"SettingsWindow_ToDefault", "Вернуть значения по умолчанию"},
                    {"SettingsWindow_OK", "OK"},
                    {"SettingsWindow_Cancel", "Отмена"},

                    // Project settings window
                    {"ProjectSettingsWindow_Name", "Имя:"},
                    {"ProjectSettingsWindow_WorkDirs", "Рабочие каталоги:"},
                    {"ProjectSettingsWindow_NextFileNumber", "Следующий номер файла:"},
                    {"ProjectSettingsWindow_OK", "OK"},
                    {"ProjectSettingsWindow_Cancel", "Отмена"},
                    {"ProjectSettingsWindow_Title", "Настройки проекта"},

                    // Settings window - color picker
                    {"SettingsWindow_Color_Standart", "Стандартные цвета"},
                    {"SeetingsWindow_Color_Available", "Доступные цвета"},
                    {"SeetingsWindow_Color_Advanced", "Расширенный цвет"},

                    {"SettingsWindow_FavouriteDirGroup", "Избранные папки"},

                    // Windiws window (окно со спискомм табов в едиторе)
                    {"WindowsWindow_Title", "Окна"},
                    {"WindowsWindow_ActivateButton", "Перейти"},
                    {"WindowsWindow_CloseButton", "Закрыть"},
                    {"WindowsWindow_CloseAllButton", "Закрыть все"},
                    {"WindowsWindow_OKButton", "OK"},

                    // Errors list
                    {"ErrorsList_Header_File", "Программа"},
                    {"ErrorsList_Header_Line", "Строка"},
                    {"ErrorsList_Header_Position", "Позиция"},
                    {"ErrorsList_Header_Type", "Тип"},
                    {"ErrorsList_Header_Diag", "Диагностика"},
                    {"ErrorsList_ErrorsCounter", "Всего: "},

                    {"OpenFolderWindow_Title", "Выбрать каталог"},
                    {"OpenFolderWindow_RootNode", "Компьютер"},
                    {"OpenFolderWindow_OKButton", "OK"},
                    {"OpenFolderWindow_CancelButton", "Отмена"},

                    // Validation errors
                    {"ValidationError_PositiveNumber", "Это должно быть положительно число"},

                    // Header
                    {"Header_ProjectExplorer", "Обозреватель проекта"},
                    {"Header_Errors", "Список ошибок"},
                    {"Header_Contents", "Оглавление программы"},
                    {"Header_Bookmarks", "Список закладок"},

                    // Version select
                    {"VersionSelectWindow_Title", "Выбрать версию"},
                    {"VersionSelectWindow_OK", "Перейти"},
                    {"VersionSelectWindow_Cancel", "Отмена"},
                    {"VersionSelectWindow_Header_Date", "Дата"},
                    {"VersionSelectWindow_Header_Mark", "Пометка"},
                    {"VersionSelectWindow_Group_Ver", "Выберите версию"},
                    {"VersionSelectWindow_Group_Preview", "Предпросмотр"},
                    {"VersionSelectWindow_Del_Tooltip", "Удалить версию"},
                    {"VersionSelectWindow_Compare_Tooltip", "Сравнить две версии"},
                    {"VersionSelectWindow_Compare_Title", "Сравнить две версии"},

                    // Content window
                    {"ContentsWindow_Title", "Оглавление программы"},
                    {"ContentsWindow_Noerror", "Ошибок в структуре нет"},
                    {"ContentsWindow_ManyEndFunc", "Обнаружен лишний endfunc"},
                    {"ContentsWindow_NoEndFunc", "Не хватает endfunc"},

                    {"ErrorWindow_Title", "Ошибка"},
                    {"ErrorWindow_Details", "Детали"},
                    {"ErrorWindow_OK", "OK"},

                    // Errors
                    {"Error_OpenProject_short", "Не удалось загрузить проект"},
                    {"Error_OpenProject_full", "Возможно, проект был поврежден или некорректно сохранен"},
                    {"Error_CloseProject_short", "Не удалось корректно закрыть проект"},
                    {"Error_OloseProject_full", "Проект был закрыт с ошибками."},
                    {"Error_SaveProject_short", "Не удалось сохранить проект"},
                    {"Error_SaveProject_full", "Проект был сохранен с ошибками."},
                    {"Error_LoadFileInProject_short", "Не удалось загрузить программу"},
                    {"Error_LoadFileInProject_full", "Похоже, она была кем-то случайно повреждена. Эта программа будет исключена из проекта."},
                    {"Error_SaveFileInProject_short", "Не удалось сохранить программу"},
                    {"Error_SaveFileInProject_full", "Похоже, возникли пробелмы. Эта программа будет исключена из проекта."},

                    {"Error_MoveFile_short", "Не удалось переместить программу."},
                    {"Error_MoveFile_full", "Возможно, стоит попробовать выбрать другое место на компьютере."},

                    // Start page window
                    {"StartPage_Title", "Стартовая страница"},
                    {"StartPage_RecentProjects", "Последние проекты"},
                    {"StartPage_Hint", "Подсказка"},
                    {"StartPage_ShowNext", "Показывать в следующий раз?"},
                    {"StartPage_Close", "Да уйди уже!"},

                    {"StartPage_Msg_FirstTime", "Похоже, вы здесь в первый раз! Урааа, еще один пользователь!"},
                    {"StartPage_Msg_NoErrors", "В прошлый раз вы не сделали ни одной ошибки! Как удивительно..."},
                    {"StartPage_Msg_MostError", "Ваша самая частовстречаемая ошибка в прошлый раз: "},
                    {"StartPage_Msg_Short", "Коротко: "},
                    {"StartPage_Msg_Full", "Подробно: "},

                    // Hotkeys window
                    {"HotkeyWindow_Title", "Горячие клавишы"},

                    // Status
                    {"Status_Ready", "Готов"},
                    {"Status_Project_Compiled", "Компиляция проекта завершена"},
                    {"Status_File_Compiled", "Компиляция программы завершена"},
                    {"Status_ProjectOpened", "Проект открыт"},
                    {"Status_ProjectClosed", "Проект закрыт"},
                    {"Status_FileCloned", "Копия программы создана успешно"},
                    {"Status_FileMoved", "Программа перемещена успешно"},
                    {"Status_FileRenamed", "Программа переименована успешно"},
                    {"Status_MakeVersion", "Версия создана успешно"},
                    {"Status_GoToVersion", "Переход к версии выполнен"},

                    // Last item
                    {"", ""}
                };
        }

        public StringDictionary V { get; set; }
    }
}