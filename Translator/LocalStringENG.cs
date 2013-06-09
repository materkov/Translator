using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Translator
{
    public class LocalStringDictionaryEng : LocalStringDictionary
    {
        public LocalStringDictionaryEng()
        {
            V = new StringDictionary
                {
                    {"DefautlFileName", "Program"},

                    {"Err_t0", "Error #0"},
                    {"Err_t1", "Error #1"},
                    {"Err_t2", "Error #2"},
                    {"Err_t3", "Error #3"},
                    {"Err_t4", "Error #4"},
                    {"Err_t5", "Error #5"},
                    {"Err_t6", "Error #6"},
                    {"Err_t7", "Error #7"},
                    {"Err_t8", "Error #8"},
                    {"Err_t9", "Error #9"},

                    {"Err_0_short", "Short description of error 0"},
                    {"Err_1_short", "Short description of error 1"},
                    {"Err_2_short", "Short description of error 2"},
                    {"Err_3_short", "Short description of error 3"},
                    {"Err_4_short", "Short description of error 4"},
                    {"Err_5_short", "Short description of error 5"},
                    {"Err_6_short", "Short description of error 6"},
                    {"Err_7_short", "Short description of error 7"},
                    {"Err_8_short", "Short description of error 8"},
                    {"Err_9_short", "Short description of error 9"},

                    {"Err_0_full", "Very very very detailed description of error 0"},
                    {"Err_1_full", "Very very very detailed description of error 1"},
                    {"Err_2_full", "Very very very detailed description of error 2"},
                    {"Err_3_full", "Very very very detailed description of error 3"},
                    {"Err_4_full", "Very very very detailed description of error 4"},
                    {"Err_5_full", "Very very very detailed description of error 5"},
                    {"Err_6_full", "Very very very detailed description of error 6"},
                    {"Err_7_full", "Very very very detailed description of error 7"},
                    {"Err_8_full", "Very very very detailed description of error 8"},
                    {"Err_9_full", "Very very very detailed description of error 9"},

                    {"Line", "Line"},
                    {"Column", "Column"},

                    {"WorkDirs", "Work folders"},

                    {"Byte_short", "B"},
                    {"KB_short", "KB"},
                    {"MB_short", "MB"},
                    {"GB_short", "GB"},

                    {"Button_NewFile_Tooltip", "New program"},
                    {"Button_NewProject_Tooltip", "New project"},
                    {"Button_AddExistingFile_Tooltip", "Add existing program"},
                    {"Button_OpenProject_Tooltip", "Open project"},
                    {"Button_SaveCurrentFile_Tooltip", "Save current program"},
                    {"Button_SaveAllFiles_Tooltip", "Save all ppened programs"},
                    {"Button_Compile", "Compile project"},
                    {"Button_Bookmarks", "Add bookmark"},
                    {"Button_Colors", "Highlight fragment"},
                    {"Button_Select", "Highlight"},

                    

                    // Up menu in MainWindow
                    {"Menu_File", "Program"},
                    {"Menu_FileT", "Actions with current program"},
                    
                    {"Menu_New", "New"},
                    {"Menu_NewT", "New"},
                    
                    {"Menu_NewFile", "New program"},
                    {"Menu_NewFileT", "Create new program"},
                    
                    {"Menu_NewProject", "New project ..."},
                    {"Menu_NewProjectT", "Create new project"},
                    
                    {"Menu_Open", "Open ..."},
                    {"Menu_OpenT", "Open"},

                    {"Menu_AddExistingFile", "Add existing program ..."},
                    {"Menu_AddExistingFileT", "Add existing program to the project"},
                    {"Menu_OpenProject", "Open project ..."},
                    {"Menu_OpenProjectT", "Open project"},


                    {"Menu_Close", "Close current program"},
                    {"Menu_CloseT", "Close current program"},

                    {"Menu_CloseAllFiles", "Close all programs"},
                    {"Menu_CloseAllFilesT", "Close all programs"},

                    {"Menu_CloseProject", "Close project"},
                    {"Menu_CloseProjectT", "Close project"},

                    
                    {"Menu_File_Rename", "Rename"},
                    {"Menu_File_RenameT", "Rename current program"},
                    
                    {"Menu_File_Move", "Move ..."},
                    {"Menu_File_MoveT", "Move current program to"},

                    {"Menu_File_NewCopy", "New copy"},
                    {"Menu_File_NewCopyT", "Create copy of existing program"},

                    {"Menu_File_NewVersion", "New version"},
                    {"Menu_File_NewVersionT", "Create new version for current program"},

                    {"Menu_File_GoToVersion", "Go to version ..."},
                    {"Menu_File_GoToVersionT", "Return current program to version"},

                    {"Menu_File_RecentProjects", "Recent projects"},
                    {"Menu_File_RecentProjectsT", "Recent projects"},

                    {"Menu_Exit", "Exit"},
                    {"Menu_ExitT", "Close program"},


                    {"Menu_Edit", "Edit"},
                    {"Menu_EditT", "Edit code actions"},

                    {"Menu_Edit_Cut", "Cut"},
                    {"Menu_Edit_CutT", "Cut selected fragment"},

                    {"Menu_Edit_Copy", "Copy"},
                    {"Menu_Edit_CopyT", "Copy selected fragment"},

                    {"Menu_Edit_Paste", "Paste"},
                    {"Menu_Edit_PasteT", "Paste fragment from exchange buffer"},

                    {"Menu_View", "View"},
                    {"Menu_ViewT", "Program views"},

                    {"Menu_View_Code", "Code"},
                    {"Menu_View_CodeT", "Go to code"},

                    {"Menu_View_CodeCheckboxes", "Code (checkboxex)"},
                    {"Menu_View_CodeCheckboxesT", "Go to checkboxes"},

                    {"Menu_View_CodeComments", "Code (comments)"},
                    {"Menu_View_CodeCommentsT", "Go to comments"},

                    {"Menu_View_Errors", "Errors list"},
                    {"Menu_View_ErrorsT", "Go to errors list"},

                    {"Menu_View_Hotkeys", "Hotkey list"},
                    {"Menu_View_HotkeysT", "Go to hotkey list"},

                    {"Menu_View_SolutionExplorer", "Project explorer"},
                    {"Menu_View_SolutionExplorerT", "Go to project explorer"},

                    {"Menu_View_Bookmarks", "Add bookmark"},
                    {"Menu_View_BookmarksT", "Add bookmark"},

                    {"Menu_View_Contents", "Contents"},
                    {"Menu_View_ContentsT", "Go to contents"},

                    {"Menu_Compile", "Compile"},
                    {"Menu_CompileT", "Compile actions"},

                    {"Menu_Compile_Project", "Project"},
                    {"Menu_Compile_ProjectT", "Compile all project"},

                    {"Menu_Compile_CurrentFile", "Current file"},
                    {"Menu_Compile_CurrentFileT", "Compile only current file"},
                    

                    {"Menu_Tools", "Tools"},
                    {"Menu_ToolsT", "Program tools"},
                    {"Menu_Tools_Settings", "Settings"},
                    {"Menu_Tools_SettingsT", "Program settings"},

                    {"Menu_Window", "Window"},
                    {"Menu_WindowT", "Actions with opened windows"},

                    {"Menu_Window_Windows", "Windows ..."},
                    {"Menu_Window_WindowsT", "List of all opened windows"},

                    {"Menu_Service", "Service"},
                    {"Menu_ServiceT", "Service"},

                    {"Menu_Help", "Help"},
                    {"Menu_HelpT", "Program help"},

                    {"Menu_Help_Help", "View help"},
                    {"Menu_Help_HelpT", "View program help"},

                    {"Menu_Help_StartPage", "Show start page"},
                    {"Menu_Help_StartPageT", "Show start page"},

                    {"Bookmarks_No", "No bookmarks in this file"},

                    {"ProjectMenu_BuildProject", "Comppile project"},
                    {"ProjectMenu_Add", "Add"},
                    {"ProjectMenu_Add_NewFile", "Add new program"},
                    {"ProjectMenu_Add_ExistFile", "Add existing program"},

                    {"ProjectMenu_OpenFile", "Open"},
                    {"ProjectMenu_DeleteFromDisk", "Remove from disk"},
                    {"ProjectMenu_Exclude", "Exclude from project"},
                    {"ProjectMenu_Rename", "Rename"},
                    {"ProjectMenu_Move", "Move"},
                    {"ProjectMenu_Copy", "Make copy"},
                    {"ProjectMenu_ProjectSettings", "Properties ..."},
                    {"ProjectMenu_NewVer", "New version"},
                    {"ProjectMenu_GoToVer", "Go to version ..."},
                    {"ProjectMenu_Properties", "Свойства ..."},

                    {"Editor_ClearCheckboxes", "Clear all"},
                    {"Editor_SetAllCheckboxes", "Select all"},
                    {"Editor_ClearSorting", "Clear sorting"},

                    {"Editor_BookmarksPanel", "Bookmarks"},
                    {"Editor_AddBookmark", "Add bookmark"},
                    {"Editor_DeleteBookmark", "Delete"},
                    {"Editor_ErrorsMarkerTooltip", "Errors in line"},
                    {"Editor_BookmarkTooltip", "Bookmark"},

                        // NEW PROJECT WINDOW
                    {"Header_NewProject", "New project"},
                    {"NewProjectWindow_Name", "Name:"},
                    {"NewProjectWindow_Path", "Path:"},
                    {"NewProjectWindow_CreateDir", "Crate new folder for project"},
                    {"NewProjectWindow_Browse", "Open"},
                    {"NewProjectWindow_OK", "OK"},
                    {"NewProjectWindow_Cancel", "Cancel"},
                    {"NewProjectWindow_DirExists", "Folder already exists. We reccomend you to choose another project name." + Environment.NewLine + Environment.NewLine + "Continue anyway?"},
                    {"NewProjectWindow_Attention", "Warning!"},

                        // Add new file window
                    {"AddNewFileWindow_FileName", "File name:"},
                    {"AddNewFileWindow_OkButton", "OK"},
                    {"AddNewFileWindow_CancelButton", "Cancel"},
                    {"AddNewFileWindow_Title", "New file"},
                    {"AddNewFileWindow_Path", "Path:"},
                    {"AddNewFileWindow_AddWorkDir", "(new)"},
                        
                    {"OpenProjectWindow_Header_Name", "Name"},
                    {"OpenProjectWindow_Header_Type", "Type"},
                    {"OpenProjectWindow_Header_Changed", "Modify date"},
                    {"OpenProjectWindow_Header_Size", "Size"},
                    {"OpenProjectWindow_ElementType_File", "File"},
                    {"OpenProjectWindow_ElementType_Dir", "Folder"},
                    {"OpenProjectWindow_ElementType_Drive", "Drive"},
                    {"OpenProjectWindow_Favourite", "Favourite"},
                    {"OpenProjectWindow_OK", "OK"},
                    {"OpenProjectWindow_Cancel", "Cancel"},

                    {"OpenFileWindow_Title", "Add existing program"},

                    {"OpenFileWindowBase_Up", "(up)"},
                    {"OpenFIleWindowBase_Filter", "Filter:"},
                    {"OpenFileWindowBase_Preview", "Preview"},
                    {"OpenFileWindowBase_Preview_NotAvialable", "(preview unavialable)"},

                    // OpenProjetcWindow
                    {"OpenProjectWindow_Title", "Open project"},

                    {"OpenProject_ErrorBox_Title", "Error"},
                    {"OpenProject_ErrorBox_Text1", "Can't load project "},
                    {"OpenProject_ErrorBox_Text2", "Project can be corrupted."},

                    {"ProjectMenu_SaveProject", "Save project"},
                    
                    // Settings window
                    {"SettingsWindow_Title", "Settings"},
                    {"SettingsWindow_Lang_Group", "Language settings"},
                    {"SettingsWindow_Lang", "Language"},

                    {"SettingsWindow_Color_Group", "Colors"},
                    {"SettingsWindow_Color_Background", "Background"},
                    {"SettingsWindow_Color_Foreground", "Foreground"},
                    {"SettingsWindow_Color_Error", "Error"},
                    {"SettingsWindow_Color_SpecialError", "Marked error"},
                    {"SettingsWindow_Color_PopupError", "Aerostate error"},
                    {"SettingsWindow_Color_Bookmark", "Bookmark"},
                    {"SettingsWindow_Color_FocusedHeader", "Focused header"},
                    {"SettingsWindow_Color_NotFocusedHeader", "Not focused header"},
                    {"SettingsWindow_Color_WindowBackground", "Main background"},

                    {"SettingsWindow_ColorEditor_Group", "Editor colors"},
                    {"SettingsWindow_ColorEditor_Line", "Current line"},
                    {"SettingsWindow_ColorEditor_MouseHoverFocused", "Mouse hover on tab, focused"},
                    {"SettingsWindow_ColorEditor_MouseHoverNotFocused", "Mouse hover on tab, not focused"},

                    {"SettingsWindow_ToDefault", "Back to default settings"},
                    {"SettingsWindow_OK", "OK"},
                    {"SettingsWindow_Cancel", "Cancel"},

                    // Project settings window
                    {"ProjectSettingsWindow_Name", "Name:"},
                    {"ProjectSettingsWindow_WorkDirs", "Worf folders:"},
                    {"ProjectSettingsWindow_NextFileNumber", "Next file number:"},
                    {"ProjectSettingsWindow_OK", "OK"},
                    {"ProjectSettingsWindow_Cancel", "Cancel"},
                    {"ProjectSettingsWindow_Title", "Project settings"},

                    // Settings window - color picker
                    {"SettingsWindow_Color_Standart", "Standart colors"},
                    {"SeetingsWindow_Color_Available", "Available colors"},
                    {"SeetingsWindow_Color_Advanced", "Extended color"},

                    {"SettingsWindow_FavouriteDirGroup", "Favourite folders"},

                    // Windiws window (окно со спискомм табов в едиторе)
                    {"WindowsWindow_Title", "Windows"},
                    {"WindowsWindow_ActivateButton", "Activate"},
                    {"WindowsWindow_CloseButton", "Close"},
                    {"WindowsWindow_CloseAllButton", "Close all"},
                    {"WindowsWindow_OKButton", "OK"},

                    // Errors list
                    {"ErrorsList_Header_File", "Program"},
                    {"ErrorsList_Header_Line", "Line"},
                    {"ErrorsList_Header_Position", "Position"},
                    {"ErrorsList_Header_Type", "Type"},
                    {"ErrorsList_Header_Diag", "Diagnostics"},
                    {"ErrorsList_ErrorsCounter", "All: "},

                    {"OpenFolderWindow_Title", "Select folder"},
                    {"OpenFolderWindow_RootNode", "Computer"},
                    {"OpenFolderWindow_OKButton", "OK"},
                    {"OpenFolderWindow_CancelButton", "Cancel"},

                    // Validation errors
                    {"ValidationError_PositiveNumber", "It must beb positive number!"},

                    // Header
                    {"Header_ProjectExplorer", "Project explorer"},
                    {"Header_Errors", "Error list"},
                    {"Header_Contents", "Contents"},
                    {"Header_Bookmarks", "Bookmark list"},

                    // Version select
                    {"VersionSelectWindow_Title", "Select version"},
                    {"VersionSelectWindow_OK", "Activate"},
                    {"VersionSelectWindow_Cancel", "Cancel"},
                    {"VersionSelectWindow_Header_Date", "Date"},
                    {"VersionSelectWindow_Header_Mark", "Mark"},
                    {"VersionSelectWindow_Group_Ver", "Select version"},
                    {"VersionSelectWindow_Group_Preview", "Preview"},
                    {"VersionSelectWindow_Del_Tooltip", "Delete version"},
                    {"VersionSelectWindow_Compare_Tooltip", "Compare 2 versions"},
                    {"VersionSelectWindow_Compare_Title", "Compare 2 versions"},

                    // Content window
                    {"ContentsWindow_Title", "Contents"},
                    {"ContentsWindow_Noerror", "No errors in structure"},
                    {"ContentsWindow_ManyEndFunc", "Found many endfunc"},
                    {"ContentsWindow_NoEndFunc", "Can't find endfunc"},

                    {"ErrorWindow_Title", "Error"},
                    {"ErrorWindow_Details", "Details"},
                    {"ErrorWindow_OK", "OK"},

                    // Errors
                    {"Error_OpenProject_short", "Can't load project"},
                    {"Error_OpenProject_full", "Mayby? project was corrupted or incorrectly saved."},
                    {"Error_CloseProject_short", "Can't close project correctly."},
                    {"Error_OloseProject_full", "Project was closed with errors."},
                    {"Error_SaveProject_short", "Can't save project"},
                    {"Error_SaveProject_full", "Project was saved with errors."},
                    {"Error_LoadFileInProject_short", "Can't load program"},
                    {"Error_LoadFileInProject_full", "Maybe? it was corrupted. This program will be excluded from project."},
                    {"Error_SaveFileInProject_short", "Can't save program"},
                    {"Error_SaveFileInProject_full", "Maybe? there is a problem. This program will be excluded from project."},
                    {"Error_MoveFile_short", "Can't move program"},
                    {"Error_MoveFile_full", "Please, try anohther location on your computer."},

                    // Start page window
                    {"StartPage_Title", "Start page"},
                    {"StartPage_RecentProjects", "Recent projects"},
                    {"StartPage_Hint", "Hint"},
                    {"StartPage_ShowNext", "Show next time?"},
                    {"StartPage_Close", "Close"},

                    {"StartPage_Msg_FirstTime", "It looks like you are new user in this program! Welocme!"},
                    {"StartPage_Msg_NoErrors", "No errors detected in last time! Interesting ..."},
                    {"StartPage_Msg_MostError", "You'r most often error in last time: "},
                    {"StartPage_Msg_Short", "Shortly: "},
                    {"StartPage_Msg_Full", "Fully: "},

                    // Hotkeys window
                    {"HotkeyWindow_Title", "Hotkey list"},

                    // Status
                    {"Status_Ready", "Ready"},
                    {"Status_Project_Compiled", "Project compilation completed"},
                    {"Status_File_Compiled", "Program compilation completed"},
                    {"Status_ProjectOpened", "Project opened"},
                    {"Status_ProjectClosed", "Project closed"},
                    {"Status_FileCloned", "Program copy created"},
                    {"Status_FileMoved", "Program moved"},
                    {"Status_FileRenamed", "Program renamed"},
                    {"Status_MakeVersion", "New version created"},
                    {"Status_GoToVersion", "Current version changed"},

                    // Last item
                    {"", ""}
                };
        }

        public StringDictionary V { get; set; }
    }
}
