#if UNITY_EDITOR
using UnityEditor;

namespace Crosstales.Common.Util
{
   /// <summary>Adds "CT_OPENWINDOW" define symbol to PlayerSettings define symbols.</summary>
   [InitializeOnLoad]
   public class CTOWCompileDefines : Crosstales.Common.EditorTask.BaseCompileDefines
   {
      private const string symbol = "CT_OPENWINDOW";

      static CTOWCompileDefines()
      {
         addSymbolsToAllTargets(symbol);
      }
   }
}
#endif
// © 2022-2024 crosstales LLC (https://www.crosstales.com)