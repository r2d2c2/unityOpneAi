using UnityEngine;
using UnityEngine.EventSystems;
using System.Runtime.InteropServices;

//#define WEBGL_COPY_AND_PASTE_SUPPORT_TEXTMESH_PRO //Uncomment this line if "TextMesh Pro" is used
namespace Crosstales.Internal
{
   /// <summary>
   /// Allows copy and paste in WebGL.
   /// 
   /// Based on https://github.com/greggman/unity-webgl-copy-and-paste
   /// </summary>
   public class WebGLCopyAndPaste : Crosstales.Common.Util.Singleton<WebGLCopyAndPaste>
   {
#if UNITY_WEBGL
      public delegate void StringCallback(string content);

      #region Initalizer

      [RuntimeInitializeOnLoadMethod]
      private static void setup()
      {
         PrefabPath = "Prefabs/WebGLCopyAndPaste";

         if (instance == null)
            CreateInstance();
      }

      #endregion


      #region MonoBehaviour methods

      private void Start()
      {
#if !UNITY_EDITOR //|| CT_DEVELOP
         WebGLCopyAndPasteAPI.Init();
#endif
      }

      #endregion


      #region Private methods

      private static void sendKey(string baseKey)
      {
         string appleKey = "%" + baseKey;
         string naturalKey = "^" + baseKey;

         GameObject currentObj = EventSystem.current.currentSelectedGameObject;
         if (currentObj == null)
            return;

         //if (Crosstales.Common.Util.BaseConstants.DEV_DEBUG)
         Debug.Log("sendKey: " + naturalKey + " - " + appleKey);

         {
            UnityEngine.UI.InputField input = currentObj.GetComponent<UnityEngine.UI.InputField>();
            if (input != null)
            {
               // I don't know what's going on here. The code in InputField is looking for ctrl-c but that fails on Mac Chrome/Firefox
               input.ProcessEvent(Event.KeyboardEvent(naturalKey));
               input.ProcessEvent(Event.KeyboardEvent(appleKey));
               // so let's hope one of these is basically a noop
               return;
            }
         }
#if WEBGL_COPY_AND_PASTE_SUPPORT_TEXTMESH_PRO
         {
            TMPro.TMP_InputField input = currentObj.GetComponent<TMPro.TMP_InputField>();
            if (input != null)
            {
               // I don't know what's going on here. The code in InputField is looking for ctrl-c but that fails on Mac Chrome/Firefox
               // so let's hope one of these is basically a noop
               input.ProcessEvent(Event.KeyboardEvent(naturalKey));
               input.ProcessEvent(Event.KeyboardEvent(appleKey));
            }
         }
#endif
      }

      #endregion


      #region Callbacks

      [AOT.MonoPInvokeCallback(typeof(StringCallback))]
      public static void GetClipboard(string key)
      {
         //if (Crosstales.Common.Util.BaseConstants.DEV_DEBUG)
         Debug.Log("GetClipboard: " + key);

         sendKey(key);
#if !UNITY_EDITOR //|| CT_DEVELOP
         WebGLCopyAndPasteAPI.PassCopyToBrowser(GUIUtility.systemCopyBuffer);
#endif
      }

      [AOT.MonoPInvokeCallback(typeof(StringCallback))]
      public static void ReceivePaste(string str)
      {
         //if (Crosstales.Common.Util.BaseConstants.DEV_DEBUG)
         Debug.Log("ReceivePaste: " + str);

         GUIUtility.systemCopyBuffer = str;
      }

      #endregion

#endif
   }

#if UNITY_WEBGL && !UNITY_EDITOR //|| CT_DEVELOP
   public class WebGLCopyAndPasteAPI
   {
      [DllImport("__Internal")]
      private static extern void initWebGLCopyAndPaste(WebGLCopyAndPaste.StringCallback cutCopyCallback, WebGLCopyAndPaste.StringCallback pasteCallback);

      [DllImport("__Internal")]
      private static extern void passCopyToBrowser(string str);

      public static void Init()
      {
         initWebGLCopyAndPaste(WebGLCopyAndPaste.GetClipboard, WebGLCopyAndPaste.ReceivePaste);
      }

      public static void PassCopyToBrowser(string str)
      {
         Debug.Log("PassCopyToBrowser: " + str);

         passCopyToBrowser(str);
      }
   }
#endif
}
// © 2021-2024 crosstales LLC (https://www.crosstales.com)