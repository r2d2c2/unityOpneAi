#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Crosstales.FB.WebGL
{
   /// <summary>Custom editor for the 'FileBrowserWebGL'-class.</summary>
   [CustomEditor(typeof(Crosstales.FB.WebGL.FileBrowserWebGL))]
   public class FileBrowserWebGLEditor : Editor
   {
      #region Variables

      private Crosstales.FB.WebGL.FileBrowserWebGL script;

      #endregion


      #region Properties

      public static bool isPrefabInScene => GameObject.Find("WebGL Native File Browser") != null;

      #endregion


      #region Editor methods

      private void OnEnable()
      {
         script = (Crosstales.FB.WebGL.FileBrowserWebGL)target;
      }

      public override void OnInspectorGUI()
      {
#if !FG_WEBGLFB
         EditorGUILayout.HelpBox("'WebGL Native File Browser' not found! Please install or buy it from the Unity AssetStore.", MessageType.Error);
#endif
         if (GUILayout.Button(new GUIContent(" Learn more", Crosstales.FB.EditorUtil.EditorHelper.Icon_Manual, "Learn more about WebGL Native File Browser.")))
         {
            Crosstales.Common.Util.NetworkHelper.OpenURL(Crosstales.FB.Util.Constants.ASSET_3P_WEBGL);
         }

         DrawDefaultInspector();

         if (script.isActiveAndEnabled)
         {
            //add stuff if needed
         }
         else
         {
            Crosstales.FB.EditorUtil.EditorHelper.SeparatorUI();
            EditorGUILayout.HelpBox("Script is disabled!", MessageType.Info);
         }
      }

      #endregion
   }
}
#endif
// © 2020-2024 crosstales LLC (https://www.crosstales.com)