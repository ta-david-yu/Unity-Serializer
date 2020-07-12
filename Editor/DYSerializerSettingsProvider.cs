using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace DYSerializer
{
    public class DYSerializerSettingsProvider : SettingsProvider
    {
        // Const
        public static string c_SettingsFullPath { get { return $"Assets/{c_SettingsPath}/{c_SettingsFilename}"; } }
        public const string c_SettingsPath = "Editor";
        public const string c_SettingsFilename = "DYSerializerSettings.asset";

        class Styles
        {
            public static GUIContent ShowTypeNameWithNameSpace = new GUIContent("Show Namespace", "Show namespace of a type in generic menu.");
        }

        // Data
        private SerializedObject m_Settings;

        public DYSerializerSettingsProvider(string path, SettingsScope scope = SettingsScope.User) : base(path, scope)
        {
            m_Settings = new SerializedObject(DYSerializerSettings.GetOrCreateSettings(c_SettingsPath, c_SettingsFilename));
        }

        public static bool IsSettingsAvailable()
        {
            return File.Exists(c_SettingsFullPath);
        }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            m_Settings = new SerializedObject(DYSerializerSettings.GetOrCreateSettings(c_SettingsPath, c_SettingsFilename));
        }

        public override void OnGUI(string searchContext)
        {
            using (new EditorGUI.IndentLevelScope(1))
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(m_Settings.FindProperty("m_ShowTypeNameWithNameSpace"), Styles.ShowTypeNameWithNameSpace);
                if (EditorGUI.EndChangeCheck())
                {
                    m_Settings.ApplyModifiedProperties();
                }
            }
        }

        /// <summary>
        /// Register this Settings Provider
        /// </summary>
        /// <returns></returns>
        [SettingsProvider]
        public static SettingsProvider CreateSettings()
        {
            if (!IsSettingsAvailable())
            {
                DYSerializerSettings.GetOrCreateSettings(c_SettingsFullPath, c_SettingsFilename);
            }

            var provider = new DYSerializerSettingsProvider("Preferences/DYSerializer", SettingsScope.User);

            provider.keywords = GetSearchKeywordsFromGUIContentProperties<Styles>();

            return provider;
        }
    }
}