using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DYSerializer
{
    public class DYSerializerSettings : ScriptableObject
    {
        [SerializeField]
        private bool m_ShowTypeNameWithNameSpace = false;
        public bool ShowTypeNameWithNameSpace { get { return m_ShowTypeNameWithNameSpace; } }

        internal static DYSerializerSettings GetOrCreateSettings(string path, string filename)
        {
            var settings = AssetDatabase.LoadAssetAtPath<DYSerializerSettings>(path);
            if (settings == null)
            {
                settings = ScriptableObject.CreateInstance<DYSerializerSettings>();
                settings.m_ShowTypeNameWithNameSpace = false;

                if (!AssetDatabase.IsValidFolder($"Assets/{path}"))
                {
                    AssetDatabase.CreateFolder("Assets", path);
                }

                AssetDatabase.CreateAsset(settings, $"Assets/{path}/{filename}");
                AssetDatabase.SaveAssets();
            }
            return settings;
        }
    }
}