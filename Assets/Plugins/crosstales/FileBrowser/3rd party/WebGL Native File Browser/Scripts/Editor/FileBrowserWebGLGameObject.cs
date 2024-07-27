#if UNITY_EDITOR
using UnityEditor;
using Crosstales.FB.EditorUtil;

namespace Crosstales.FB.WebGL
{
   /// <summary>Editor component for for adding the prefabs from 'WebGL' in the "Hierarchy"-menu.</summary>
   public static class FileBrowserWebGLGameObject
   {
      [MenuItem("GameObject/" + Crosstales.FB.Util.Constants.ASSET_NAME + "/3rd party/WebGL Native File Browser", false, Crosstales.FB.EditorUtil.EditorHelper.GO_ID + 90)]
      private static void AddWrapper()
      {
         Crosstales.FB.EditorUtil.EditorHelper.InstantiatePrefab("WebGL Native File Browser", $"{EditorConfig.ASSET_PATH}3rd party/WebGL Native File Browser/Prefabs/");
      }

      [MenuItem("GameObject/" + Crosstales.FB.Util.Constants.ASSET_NAME + "/3rd party/WebGL Native File Browser", true)]
      private static bool AddWrapperValidator()
      {
         return !FileBrowserWebGLEditor.isPrefabInScene;
      }
   }
}
#endif
// © 2020-2024 crosstales LLC (https://www.crosstales.com)