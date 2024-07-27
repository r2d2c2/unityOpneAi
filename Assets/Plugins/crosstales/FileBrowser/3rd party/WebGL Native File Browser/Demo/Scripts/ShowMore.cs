using UnityEngine;

namespace Crosstales.FB.WebGL
{
   /// <summary>Shows the details for WebGL Native File Browser.</summary>
   [HelpURL("https://www.crosstales.com/media/data/assets/FileBrowser/api/class_crosstales_1_1_f_b_1_1_web_g_l_1_1_show_more.html")]
   public class ShowMore : MonoBehaviour
   {
      #region Public methods

      public void Show()
      {
         Crosstales.Common.Util.NetworkHelper.OpenURL(Crosstales.FB.Util.Constants.ASSET_3P_WEBGL);
      }

      #endregion
   }
}
// © 2020-2024 crosstales LLC (https://www.crosstales.com)