using UnityEngine;
using UnityEditor;

namespace LuviKunG.MipMapBias
{
    public static class MipMapBiasMenu
    {
        private static Object[] selection;

        [MenuItem("Assets/LuviKunG/Open Mipmap Bias Window", true)]
        public static bool ValidateSetBias()
        {
            selection = Selection.GetFiltered(typeof(Texture), SelectionMode.DeepAssets);
            return selection.Length > 0;
        }

        [MenuItem("Assets/LuviKunG/Open Mipmap Bias Window", false)]
        public static void SetBias()
        {
            MipMapBiasEditor.OpenWindowWithSelection(selection);
        }
    }
}