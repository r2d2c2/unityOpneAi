using System.Linq;
using Crosstales;
using UnityEngine;

namespace Crosstales.FB.WebGL
{
   /// <summary>
   /// WebGL Native File Browser wrapper.
   /// NOTE: This wrapper needs "WebGL Native File Browser" https://assetstore.unity.com/packages/slug/41902?aid=1011lNGT
   /// </summary>
   [HelpURL("https://www.crosstales.com/media/data/assets/FileBrowser/api/class_crosstales_1_1_f_b_1_1_web_g_l_1_1_file_browser_web_g_l.html")]
   public class FileBrowserWebGL : Crosstales.FB.Wrapper.BaseCustomFileBrowser
   {
      #region Variables

      [Header("Labels"), SerializeField] private string openFileDescription = "Select file for loading:";
      [SerializeField] private string openFileSelectButton = "Select";
      [SerializeField] private string openFileCloseButton = "Close";

      private byte[] _currentLoadedData;
      //private string _currentLoadedDataResolution;

      #endregion


      #region Properties

      public override bool canOpenFile => true;
      public override bool canOpenFolder => false;
      public override bool canSaveFile => true;

      public override bool canOpenMultipleFiles => false;

      public override bool canOpenMultipleFolders => false;

      public override bool isPlatformSupported => Crosstales.FB.Util.Helper.isWebGLPlatform || Crosstales.FB.Util.Helper.isEditor;

      public override bool isWorkingInEditor => false;

      public override string CurrentOpenSingleFile { get; set; }
      public override string[] CurrentOpenFiles { get; set; }
      public override string CurrentOpenSingleFolder { get; set; }
      public override string[] CurrentOpenFolders { get; set; }
      public override string CurrentSaveFile { get; set; }
      public override byte[] CurrentOpenSingleFileData => _currentLoadedData;

      public static string OpenFileDescription;
      public static string OpenFileSelectButton;
      public static string OpenFileCloseButton;

      #endregion


      #region MonoBehaviour methods

      private void Start()
      {
#if FG_WEBGLFB
         if (isPlatformSupported) // && FileBrowser.Instance.CustomWrapper == this)
         {
            FrostweepGames.Plugins.WebGLFileBrowser.WebGLFileBrowser.FilePopupWasClosedEvent += filePopupWasClosedEventHandler;
            FrostweepGames.Plugins.WebGLFileBrowser.WebGLFileBrowser.FilesWereOpenedEvent += filesWereOpenedEventHandler;
            FrostweepGames.Plugins.WebGLFileBrowser.WebGLFileBrowser.FileOpenFailedEvent += fileOpenFailedEventHandler;
            FrostweepGames.Plugins.WebGLFileBrowser.WebGLFileBrowser.FileWasSavedEvent += fileWasSavedEventHandler;
            FrostweepGames.Plugins.WebGLFileBrowser.WebGLFileBrowser.FileSaveFailedEvent += fileSaveFailedEventHandler;

            OpenFileDescription = openFileDescription;
            OpenFileSelectButton = openFileSelectButton;
            OpenFileCloseButton = openFileCloseButton;
         }
         else
         {
#if !UNITY_EDITOR
            if (FileBrowser.Instance.CustomWrapper == this)
            {
               FileBrowser.Instance.CustomMode = false;
               Debug.LogWarning($"'{GetType().Name}' is not supported under the current build platform. Falling back to the default wrapper.", this);
            }
#endif
         }
#else
         if (FileBrowser.Instance.CustomWrapper == this)
            FileBrowser.Instance.CustomMode = false;

         Debug.LogWarning($"'WebGL Native File Browser' not found! Please install or buy it from the Unity AssetStore: {Crosstales.FB.Util.Constants.ASSET_3P_WEBGL}", this);
#endif
      }

#if FG_WEBGLFB
      private void OnDestroy()
      {
         FrostweepGames.Plugins.WebGLFileBrowser.WebGLFileBrowser.FilePopupWasClosedEvent -= filePopupWasClosedEventHandler;
         FrostweepGames.Plugins.WebGLFileBrowser.WebGLFileBrowser.FilesWereOpenedEvent -= filesWereOpenedEventHandler;
         FrostweepGames.Plugins.WebGLFileBrowser.WebGLFileBrowser.FileOpenFailedEvent -= fileOpenFailedEventHandler;
         FrostweepGames.Plugins.WebGLFileBrowser.WebGLFileBrowser.FileWasSavedEvent -= fileWasSavedEventHandler;
         FrostweepGames.Plugins.WebGLFileBrowser.WebGLFileBrowser.FileSaveFailedEvent -= fileSaveFailedEventHandler;
      }
#endif

      #endregion


      #region Implemented methods

      public override string[] OpenFiles(string title, string directory, string defaultName, bool multiselect, params Crosstales.FB.ExtensionFilter[] extensions)
      {
#if FG_WEBGLFB
         if (!string.IsNullOrEmpty(directory))
            Debug.LogWarning("'directory' is not supported in WebGL File Browser.", this);

         if (!string.IsNullOrEmpty(defaultName))
            Debug.LogWarning("'defaultName' is not supported in WebGL File Browser.", this);

         if (multiselect)
            Debug.LogWarning("'multiselect' is not supported in WebGL File Browser.", this);

         //resetOpenFiles();

         FrostweepGames.Plugins.WebGLFileBrowser.WebGLFileBrowser.FreeMemory(); // free used memory and destroy created content

         FrostweepGames.Plugins.WebGLFileBrowser.WebGLFileBrowser.SetLocalization(FrostweepGames.Plugins.WebGLFileBrowser.LocalizationKey.HEADER_TITLE, title);
         FrostweepGames.Plugins.WebGLFileBrowser.WebGLFileBrowser.SetLocalization(FrostweepGames.Plugins.WebGLFileBrowser.LocalizationKey.DESCRIPTION_TEXT, OpenFileDescription);
         FrostweepGames.Plugins.WebGLFileBrowser.WebGLFileBrowser.SetLocalization(FrostweepGames.Plugins.WebGLFileBrowser.LocalizationKey.SELECT_BUTTON_CONTENT, OpenFileSelectButton);
         FrostweepGames.Plugins.WebGLFileBrowser.WebGLFileBrowser.SetLocalization(FrostweepGames.Plugins.WebGLFileBrowser.LocalizationKey.CLOSE_BUTTON_CONTENT, OpenFileCloseButton);

         //Debug.Log(getFilterFromExtensionFilters(extensions));
         FrostweepGames.Plugins.WebGLFileBrowser.WebGLFileBrowser.OpenFilePanelWithFilters(getFilterFromExtensionFilters(extensions));
#endif
         return null;
      }

      public override string[] OpenFolders(string title, string directory, bool multiselect)
      {
         //resetOpenFolders();

         Debug.LogWarning("'OpenFolders' is not supported in WebGL File Browser.", this);

         return null;
      }

      public override string SaveFile(string title, string directory, string defaultName, params Crosstales.FB.ExtensionFilter[] extensions)
      {
#if FG_WEBGLFB
         if (CurrentSaveFileData == null)
         {
            Debug.LogWarning("'CurrentSaveFileData' is null - can not save the file!", this);
         }
         else if (extensions == null || extensions.Length < 1 || extensions[0].Extensions.Length < 1)
         {
            Debug.LogWarning("'extensions' is null or empty - can not save the file!", this);
         }
         else
         {
            //resetSaveFile();

            FrostweepGames.Plugins.WebGLFileBrowser.WebGLFileBrowser.FreeMemory(); // free used memory and destroy created content

            FrostweepGames.Plugins.WebGLFileBrowser.File file = new FrostweepGames.Plugins.WebGLFileBrowser.File
            {
               fileInfo = new FrostweepGames.Plugins.WebGLFileBrowser.FileInfo
               {
                  fullName = defaultName.Contains(".") ? defaultName : $"{defaultName}.{extensions[0].Extensions[0]}",
#if UNITY_EDITOR
                  name = defaultName,
                  extension = extensions[0].Extensions[0]
#endif
               },
               data = CurrentSaveFileData
            };

            //WebGLFileBrowser.SetLocalization(LocalizationKey.HEADER_TITLE, title);
            //WebGLFileBrowser.SetLocalization(LocalizationKey.DESCRIPTION_TEXT, "Select file for saving:");
            //WebGLFileBrowser.SetLocalization(LocalizationKey.SELECT_BUTTON_CONTENT, "Select");
            //WebGLFileBrowser.SetLocalization(LocalizationKey.CLOSE_BUTTON_CONTENT, "Close");

            CurrentSaveFile = file.fileInfo.fullName; //TODO the result is wrong in the Editor

            FrostweepGames.Plugins.WebGLFileBrowser.WebGLFileBrowser.SaveFile(file);
         }
#endif
         return null;
      }

      public override void OpenFilesAsync(string title, string directory, string defaultName, bool multiselect, Crosstales.FB.ExtensionFilter[] extensions, System.Action<string[]> cb)
      {
         cb?.Invoke(OpenFiles(title, directory, defaultName, multiselect, extensions));
      }

      public override void OpenFoldersAsync(string title, string directory, bool multiselect, System.Action<string[]> cb)
      {
         Debug.LogWarning("'OpenFoldersAsync' is not supported in WebGL File Browser.", this);
      }

      public override void SaveFileAsync(string title, string directory, string defaultName, Crosstales.FB.ExtensionFilter[] extensions, System.Action<string> cb)
      {
         cb?.Invoke(SaveFile(title, directory, defaultName, extensions));
      }

      #endregion


      #region Private methods

      private string getFilterFromExtensionFilters(Crosstales.FB.ExtensionFilter[] extensions)
      {
         string result = ".";

         if (extensions?.Length > 0)
         {
            if (extensions.Length == 1 && extensions[0].Extensions.Length == 1 && extensions[0].Extensions[0] == "*")
            {
               //do nothing
            }
            else
            {
               System.Text.StringBuilder sb = new System.Text.StringBuilder();
               bool isFirst = true;

               foreach (string ext in extensions.SelectMany(filter => filter.Extensions))
               {
                  if (!isFirst)
                     sb.Append(",");

                  sb.Append(".");
                  sb.Append(ext);

                  isFirst = false;
               }

               result = sb.ToString();
            }
         }

         if (Crosstales.FB.Util.Config.DEBUG)
            Debug.Log($"getFilterFromExtensionFilters: {result}", this);

         return result;
      }

      #endregion


      #region Callbacks

#if FG_WEBGLFB
      private void filesWereOpenedEventHandler(FrostweepGames.Plugins.WebGLFileBrowser.File[] files)
      {
         //TODO add support for multiple files

         if (files == null)
            return;

         //Debug.Log($"File was opened: {files[0].data.Length} - " + files[0].fileInfo.fullName, this);
         //Debug.Log("FILES: " + files.CTDump());

         _currentLoadedData = files[0].data;
#if UNITY_WEBGL && !UNITY_EDITOR
         CurrentOpenSingleFile = files[0].fileInfo.fullName;
         CurrentOpenFiles = new[] { files[0].fileInfo.fullName };
/*
         CurrentOpenFiles = new string[files.Length];
         for (int ii = 0; ii < files.Length; ii++)
         {
            CurrentOpenFiles[ii] = files[ii].fileInfo.fullName;
         }
*/
#else
         CurrentOpenSingleFile = files[0].fileInfo.path;
         CurrentOpenFiles = new[] { files[0].fileInfo.path };
/*
         CurrentOpenFiles = new string[files.Length];
         for (int ii = 0; ii < files.Length; ii++)
         {
            CurrentOpenFiles[ii] = files[ii].fileInfo.path;
         }
*/
#endif
      }

      private void filePopupWasClosedEventHandler()
      {
         //Debug.Log("filePopupWasClosedEventHandler", this);

         CurrentOpenFiles = System.Array.Empty<string>();
         //CurrentOpenSingleFile = null;
         CurrentOpenSingleFile = string.Empty;
      }

      private void fileOpenFailedEventHandler(string error)
      {
         _currentLoadedData = null;

         CurrentOpenFiles = System.Array.Empty<string>();
         //CurrentOpenSingleFile = null;
         CurrentOpenSingleFile = string.Empty;

         Debug.LogWarning($"'OpenFiles' failed: {error}", this);
      }

      private void fileWasSavedEventHandler(FrostweepGames.Plugins.WebGLFileBrowser.File file)
      {
         if (file == null)
            return;

         //Debug.Log($"file was saved: {file.data.Length} - " + file.fileInfo.fullName, this);

#if UNITY_WEBGL && !UNITY_EDITOR
         CurrentSaveFile = file.fileInfo.fullName;
#else
         CurrentSaveFile = file.fileInfo.path;
#endif
      }

      private void fileSaveFailedEventHandler(string error)
      {
         //CurrentSaveFile = null;
         CurrentSaveFile = string.Empty;

         Debug.LogWarning($"'SaveFile' failed: {error}", this);
      }
#endif

      #endregion
   }
}
// © 2020-2024 crosstales LLC (https://www.crosstales.com)