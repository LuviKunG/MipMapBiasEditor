using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LuviKunG.MipMapBias
{
    public sealed class MipMapBiasEditor : EditorWindow
    {
        public enum BiasLevel : byte
        {
            Normal,
            High,
            VeryHigh,
        }

        private static readonly GUIContent CONTENT_MIP_MAP_BIAS_LEVEL = new GUIContent("Mipmap Bias Level", "This will set all your selection textures mipmap bias depend on level.\nNormal = 0\nHigh = -0.5\nVery High = -1.5");
        private const string KEY_MIP_MAP_BIAS_INFO = "com.luvikung.mipmapbias.showinfo";
        private const string MESSAGE_MIP_MAP_BIAS_INFO = "Note that this editor isn't support for OpenGL ES and Metal because they should be configure mip map bias value in the shader.";
        private const string MESSAGE_MIP_MAP_BIAS_INFO_CODE = "half4 texColor = SAMPLE_TEXTURE2D_BIAS(_BaseMap, sampler_BaseMap, uv, _Bias);";
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
        private bool isShowInfo;

        public void SetSelection(Object[] objects)
        {
            foreach (Texture texture in objects)
                list.Add(texture);
        }

        private void OnEnable()
        {
            isShowInfo = EditorPrefs.GetBool(KEY_MIP_MAP_BIAS_INFO, true);
        }

        private void OnGUI()
        {
            if (list != null && list.Count > 0)
            {
                biasLevel = (BiasLevel)EditorGUILayout.EnumPopup(CONTENT_MIP_MAP_BIAS_LEVEL, biasLevel);
                if (isShowInfo)
                {
                    using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                    {
                        EditorGUILayout.LabelField(MESSAGE_MIP_MAP_BIAS_INFO, EditorStyles.wordWrappedMiniLabel);
                        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                        {
                            EditorGUILayout.LabelField(MESSAGE_MIP_MAP_BIAS_INFO_CODE, EditorStyles.wordWrappedMiniLabel);
                        }
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            GUILayout.FlexibleSpace();
                            if (GUILayout.Button("I Understand", EditorStyles.miniButton, GUILayout.Width(150.0f)))
                            {
                                isShowInfo = false;
                                EditorPrefs.SetBool(KEY_MIP_MAP_BIAS_INFO, false);
                            }
                            GUILayout.FlexibleSpace();
                        }
                    }
                }
                if (GUILayout.Button("Update Mipmap Bias", GUILayout.Height(30.0f)))
                {
                    float currentBiasLevel = GetBiasLevelValue(biasLevel);
                    for (int i = 0; i < list.Count; i++)
                    {
                        string path = AssetDatabase.GetAssetPath(list[i]);
                        (AssetImporter.GetAtPath(path) as TextureImporter).mipMapBias = currentBiasLevel;
                        AssetDatabase.ImportAsset(path);
                    }
                    AssetDatabase.SaveAssets();
                }
                GUILayout.Space(10.0f);
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
            return level switch
            {
                BiasLevel.Normal => 0.0f,
                BiasLevel.High => -0.5f,
                BiasLevel.VeryHigh => -1.5f,
                _ => 0.0f,
            };
        }
    }
}