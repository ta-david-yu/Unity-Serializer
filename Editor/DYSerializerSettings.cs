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

        internal static DYSerializerSettings GetOrCreateSettings(string path)
        {
            var settings = AssetDatabase.LoadAssetAtPath<DYSerializerSettings>(path);
            if (settings == null)
            {
                settings = ScriptableObject.CreateInstance<DYSerializerSettings>();
                settings.m_ShowTypeNameWithNameSpace = false;

                AssetDatabase.CreateAsset(settings, path);
                AssetDatabase.SaveAssets();
            }
            return settings;
        }
    }
}