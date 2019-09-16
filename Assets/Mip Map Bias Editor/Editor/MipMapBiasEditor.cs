using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LuviKunG.MipMapBias
{
    public sealed class MipMapBiasEditor : EditorWindow
    {
        public enum BiasLevel : int
        {
            Normal,
            High,
            VeryHigh,
        }

        private static readonly GUIContent CONTENT_MIP_MAP_BIAS_LEVEL = new GUIContent("Mipmap Bias Level", "This will set all your selection textures mipmap bias depend on level.\nNormal = 0\nHigh = -0.5\nVery High = -1.5");

        public static MipMapBiasEditor OpenWindowWithSelection(Object[] objects)
        {
            MipMapBiasEditor window = GetWindow<MipMapBiasEditor>(true, "Mipmap Bias", true);
            window.SetSelection(objects);
            window.Show();
            return window;
        }

        private List<Texture> list = new List<Texture>();
        private Texture texture;
        private Vector2 scrollview;
        private BiasLevel biasLevel;

        public void SetSelection(Object[] objects)
        {
            foreach (Texture texture in objects)
                list.Add(texture);
        }

        private void OnGUI()
        {
            if (list != null && list.Count > 0)
            {
                biasLevel = (BiasLevel)EditorGUILayout.EnumPopup(CONTENT_MIP_MAP_BIAS_LEVEL, biasLevel);
                if (GUILayout.Button("Update Mipmap Bias"))
                {
                    float currentBiasLevel = GetBiasLevelValue(biasLevel);
                    for (int i = 0; i < list.Count; i++)
                    {
                        string path = AssetDatabase.GetAssetPath(list[i]);
                        (AssetImporter.GetAtPath(path) as TextureImporter).mipMapBias = currentBiasLevel;
                        AssetDatabase.ImportAsset(path);
                    }
                }
                using (var scrollViewScope = new EditorGUILayout.ScrollViewScope(scrollview))
                {
                    scrollview = scrollViewScope.scrollPosition;
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (list[i] == null) list.RemoveAt(i--);
                        else list[i] = (Texture)EditorGUILayout.ObjectField(GUIContent.none, list[i], typeof(Texture), false, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    }
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Please using Assets Menu to configure mipmap bias.", MessageType.Warning, true);
            }
        }

        private float GetBiasLevelValue(BiasLevel level)
        {
            switch (level)
            {
                case BiasLevel.Normal: return 0.0f;
                case BiasLevel.High: return -0.5f;
                case BiasLevel.VeryHigh: return -1.5f;
                default: return 0.0f;
            }
        }
    }
}