#if UNITY_EDITOR
using UnityEditor;
using Crosstales.FB.EditorUtil;

namespace Crosstales.FB.WebGL
{
   /// <summary>Editor component for for adding the prefabs from 'WebGL' in the "Tools"-menu.</summary>
   public static class FileBrowserWebGLMenu
   {
      [MenuItem("Tools/" + Crosstales.FB.Util.Constants.ASSET_NAME + "/Prefabs/3rd party/WebGL Native File Browser", false, Crosstales.FB.EditorUtil.EditorHelper.MENU_ID + 220)]
      private static void AddWrapper()
      {
         Crosstales.FB.EditorUtil.EditorHelper.InstantiatePrefab("WebGL Native File Browser", $"{EditorConfig.ASSET_PATH}3rd party/WebGL Native File Browser/Prefabs/");
      }

      [MenuItem("Tools/" + Crosstales.FB.Util.Constants.ASSET_NAME + "/Prefabs/3rd party/WebGL Native File Browser", true)]
      private static bool AddWrapperValidator()
      {
         return !FileBrowserWebGLEditor.isPrefabInScene;
      }
   }
}
#endif
// © 2020-2024 crosstales LLC (https://www.crosstales.com)