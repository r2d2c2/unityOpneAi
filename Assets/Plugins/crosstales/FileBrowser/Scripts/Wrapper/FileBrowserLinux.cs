﻿#if UNITY_STANDALONE_LINUX || CT_DEVELOP
using System;
using UnityEngine;

namespace Crosstales.FB.Wrapper
{
   /// <summary>File browser implementation for Linux (GTK).</summary>
   public class FileBrowserLinux : BaseFileBrowserStandalone
   {
      #region Variables

      private static FileBrowserLinux instance;

      private static Action<string[]> _openFileCb;
      private static Action<string[]> _openFolderCb;
      private static Action<string> _saveFileCb;

      private const char splitChar = (char)28;

      private static bool useFallback;
      private static string fallbackExtension;

      #endregion


      #region Constructor

      public FileBrowserLinux()
      {
         instance = this;

         Linux.NativeMethods.DialogInit();
      }

      #endregion


      #region Implemented methods

      public override bool canOpenMultipleFolders => true;

      public override bool isPlatformSupported => Crosstales.FB.Util.Helper.isLinuxPlatform; // || Util.Helper.isLinuxEditor;

      public override bool isWorkingInEditor => false;

      public override string[] OpenFiles(string title, string directory, string defaultName, bool multiselect, params ExtensionFilter[] extensions)
      {
         if (!string.IsNullOrEmpty(defaultName))
            Debug.LogWarning("'defaultName' is not supported under Linux.");

         //resetOpenFiles();

         return openFiles(title, directory, defaultName, multiselect, false, extensions);
      }

      public override string[] OpenFolders(string title, string directory, bool multiselect)
      {
         //resetOpenFolders();

         return openFolders(title, directory, multiselect, false);
      }

      public override string SaveFile(string title, string directory, string defaultName, params ExtensionFilter[] extensions)
      {
         //resetSaveFile();

         return saveFile(title, directory, defaultName, false, extensions);
      }

      public override void OpenFilesAsync(string title, string directory, string defaultName, bool multiselect, ExtensionFilter[] extensions, Action<string[]> cb)
      {
         if (!string.IsNullOrEmpty(defaultName))
            Debug.LogWarning("'defaultName' is not supported under Linux.");

         //resetOpenFiles();

         _openFileCb = cb;

         openFiles(title, directory, defaultName, multiselect, true, extensions);
      }

      public override void OpenFoldersAsync(string title, string directory, bool multiselect, Action<string[]> cb)
      {
         //resetOpenFolders();

         _openFolderCb = cb;

         openFolders(title, directory, multiselect, true);
      }

      public override void SaveFileAsync(string title, string directory, string defaultName, ExtensionFilter[] extensions, Action<string> cb)
      {
         //resetSaveFile();

         _saveFileCb = cb;

         saveFile(title, directory, defaultName, true, extensions);
      }

      #endregion


      #region Private methods

      private static string getFilterFromFileExtensionList(ExtensionFilter[] extensions)
      {
         if (extensions?.Length > 0)
         {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            for (int xx = 0; xx < extensions.Length; xx++)
            {
               ExtensionFilter filter = extensions[xx];

               sb.Append(filter.Name);
               sb.Append(";");

               for (int ii = 0; ii < filter.Extensions.Length; ii++)
               {
                  sb.Append(filter.Extensions[ii]);

                  if (ii + 1 < filter.Extensions.Length)
                     sb.Append(",");
               }

               if (xx + 1 < extensions.Length)
                  sb.Append("|");
            }

            if (Crosstales.FB.Util.Config.DEBUG)
               Debug.Log($"getFilterFromFileExtensionList: {sb}");

            return sb.ToString();
         }

         return string.Empty;
      }

      #endregion

      private static string[] openFiles(string title, string directory, string defaultName, bool multiselect, bool isAsync, params ExtensionFilter[] extensions)
      {
         try
         {
            if (isAsync)
            {
               Linux.NativeMethods.DialogOpenFilePanelAsync(title, directory, getFilterFromFileExtensionList(extensions), multiselect, openFileCb);
            }
            else
            {
               string paths = System.Runtime.InteropServices.Marshal.PtrToStringAnsi(Linux.NativeMethods.DialogOpenFilePanel(title, directory, getFilterFromFileExtensionList(extensions), multiselect));

               if (string.IsNullOrEmpty(paths))
               {
                  instance.CurrentOpenFiles = System.Array.Empty<string>();
                  instance.CurrentOpenSingleFile = string.Empty;

                  return null;
               }

               string[] pathArray = paths.Split(splitChar);

               instance.CurrentOpenFiles = pathArray;
               instance.CurrentOpenSingleFile = pathArray[0];

               return instance.CurrentOpenFiles;
            }
         }
         catch (Exception ex)
         {
            instance.CurrentOpenFiles = System.Array.Empty<string>();
            instance.CurrentOpenSingleFile = string.Empty;

            Debug.LogError($"Open file dialog threw an error: {ex}");
         }

         return null;
      }

      private static string[] openFolders(string title, string directory, bool multiselect, bool isAsync)
      {
         try
         {
            if (isAsync)
            {
               Linux.NativeMethods.DialogOpenFolderPanelAsync(title, directory, multiselect, openFolderCb);
            }
            else
            {
               string paths = System.Runtime.InteropServices.Marshal.PtrToStringAnsi(Linux.NativeMethods.DialogOpenFolderPanel(title, directory, multiselect));

               if (string.IsNullOrEmpty(paths))
               {
                  instance.CurrentOpenFolders = System.Array.Empty<string>();
                  instance.CurrentOpenSingleFolder = string.Empty;

                  return null;
               }

               string[] pathArray = paths.Split(splitChar);

               instance.CurrentOpenFolders = pathArray;
               instance.CurrentOpenSingleFolder = pathArray[0];

               return instance.CurrentOpenFolders;
            }
         }
         catch (Exception ex)
         {
            instance.CurrentOpenFolders = System.Array.Empty<string>();
            instance.CurrentOpenSingleFolder = string.Empty;

            Debug.LogError($"Folder dialog threw an error: {ex}");
         }

         return null;
      }

      private static string saveFile(string title, string directory, string defaultName, bool isAsync, params ExtensionFilter[] extensions)
      {
         useFallback = extensions != null && extensions.Length == 1 && extensions[0].Extensions.Length == 1;
         fallbackExtension = useFallback ? $".{extensions[0].Extensions[0]}" : string.Empty;

         try
         {
            if (isAsync)
            {
               Linux.NativeMethods.DialogSaveFilePanelAsync(title, directory, defaultName, getFilterFromFileExtensionList(extensions), saveFileCb);
            }
            else
            {
               string path = System.Runtime.InteropServices.Marshal.PtrToStringAnsi(Linux.NativeMethods.DialogSaveFilePanel(title, directory, defaultName, getFilterFromFileExtensionList(extensions)));

               if (string.IsNullOrEmpty(path))
               {
                  instance.CurrentSaveFile = string.Empty;

                  return null;
               }

               if (useFallback && !path.EndsWith(fallbackExtension))
                  path += fallbackExtension;

               instance.CurrentSaveFile = path;

               return instance.CurrentSaveFile;
            }
         }
         catch (Exception ex)
         {
            instance.CurrentSaveFile = string.Empty;

            Debug.LogError($"Save file dialog threw an error: {ex}");
         }

         return null;
      }

      //[AOT.MonoPInvokeCallbackAttribute(typeof(Action))]
      [AOT.MonoPInvokeCallback(typeof(AsyncCallback))]
      private static void openFileCb(string result)
      {
         if (string.IsNullOrEmpty(result))
         {
            instance.CurrentOpenFiles = System.Array.Empty<string>();
            instance.CurrentOpenSingleFile = string.Empty;

            _openFileCb?.Invoke(null);
         }
         else
         {
            string[] pathArray = result.Split(splitChar);

            instance.CurrentOpenFiles = pathArray;
            instance.CurrentOpenSingleFile = pathArray[0];

            _openFileCb?.Invoke(pathArray);
         }
      }

      //[AOT.MonoPInvokeCallbackAttribute(typeof(Action))]
      [AOT.MonoPInvokeCallback(typeof(AsyncCallback))]
      private static void openFolderCb(string result)
      {
         if (string.IsNullOrEmpty(result))
         {
            instance.CurrentOpenFolders = System.Array.Empty<string>();
            instance.CurrentOpenSingleFolder = string.Empty;

            _openFolderCb?.Invoke(null);
         }
         else
         {
            string[] pathArray = result.Split(splitChar);

            instance.CurrentOpenFolders = pathArray;
            instance.CurrentOpenSingleFolder = pathArray[0];

            _openFolderCb?.Invoke(pathArray);
         }
      }

      //[AOT.MonoPInvokeCallbackAttribute(typeof(Action))]
      [AOT.MonoPInvokeCallback(typeof(AsyncCallback))]
      private static void saveFileCb(string result)
      {
         if (string.IsNullOrEmpty(result))
         {
            instance.CurrentSaveFile = string.Empty;

            _saveFileCb?.Invoke(null);
         }
         else
         {
            if (useFallback && !result.EndsWith(fallbackExtension))
               result += fallbackExtension;

            instance.CurrentSaveFile = result;

            _saveFileCb?.Invoke(result);
         }
      }
   }
}

namespace Crosstales.FB.Wrapper.Linux
{
   /// <summary>Native methods (bridge to Linux).</summary>
   internal static class NativeMethods
   {
      [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
      public delegate void AsyncCallback(string path);

      [System.Runtime.InteropServices.DllImport("FileBrowser")]
      internal static extern void DialogInit();

      [System.Runtime.InteropServices.DllImport("FileBrowser")]
      internal static extern IntPtr DialogOpenFilePanel(string title, string directory, string extension, bool multiselect);

      [System.Runtime.InteropServices.DllImport("FileBrowser")]
      internal static extern IntPtr DialogOpenFolderPanel(string title, string directory, bool multiselect);

      [System.Runtime.InteropServices.DllImport("FileBrowser")]
      internal static extern IntPtr DialogSaveFilePanel(string title, string directory, string defaultName, string extension);

      [System.Runtime.InteropServices.DllImport("FileBrowser")]
      internal static extern void DialogOpenFilePanelAsync(string title, string directory, string extension, bool multiselect, AsyncCallback callback);

      [System.Runtime.InteropServices.DllImport("FileBrowser")]
      internal static extern void DialogOpenFolderPanelAsync(string title, string directory, bool multiselect, AsyncCallback callback);

      [System.Runtime.InteropServices.DllImport("FileBrowser")]
      internal static extern void DialogSaveFilePanelAsync(string title, string directory, string defaultName, string extension, AsyncCallback callback);
   }
}
#endif
// © 2017-2024 crosstales LLC (https://www.crosstales.com)