using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

namespace DYSerializer
{
    [CustomPropertyDrawer(typeof(SerializeRefUIAttribute))]
    public class SerializeRefUIAttributePropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var labelPosition = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(labelPosition, label);
            
            // get possible types
            var types = TypeCache.GetTypesDerivedFrom(property.GetManagedReferenceFieldType()).Where((type) =>
                !type.IsSubclassOf(typeof(UnityEngine.Object)) &&
                !type.IsAbstract)
                .OrderBy((type) => type.Name);

            // draw dropdown menu
            var backgroundColor = Color.yellow;

            var buttonPosition = position;
            buttonPosition.x += EditorGUIUtility.labelWidth + 1 * EditorGUIUtility.standardVerticalSpacing;
            buttonPosition.width = position.width - EditorGUIUtility.labelWidth - 1 * EditorGUIUtility.standardVerticalSpacing;
            buttonPosition.height = EditorGUIUtility.singleLineHeight;

            var storedIndent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            var storedColor = GUI.backgroundColor;
            GUI.backgroundColor = backgroundColor;

            var names = SerializedPropertyUtility.GetSplitNamesFromManagerReferenceFullTypename(property.managedReferenceFullTypename);
            var className = string.IsNullOrEmpty(names.ClassName) ? "(Null)" :
                DYSerializerSettings.GetOrCreateSettings(DYSerializerSettingsProvider.c_SettingsPath, DYSerializerSettingsProvider.c_SettingsFilename).ShowTypeNameWithNameSpace? names.ClassName : property.GetManagedReferenceFullType().Name;
            var assemblyName = names.AssemblyName;

            if (GUI.Button(buttonPosition, new GUIContent(className, names.ClassName + " (" + assemblyName + ")")))
                drawTypeDropDownList(position, property, types.ToList());

            GUI.backgroundColor = storedColor;
            EditorGUI.indentLevel = storedIndent;

            // draw property field
            EditorGUI.PropertyField(position, property, GUIContent.none, true);
            EditorGUI.EndProperty();

        }

        private void drawTypeDropDownList(Rect position, SerializedProperty property, List<Type> types)
        {
            var menu = new GenericMenu();

            menu.AddItem(new GUIContent("(Null)"), property.GetManagedReferenceFullType() is null, property.SetManagedReferenceToNull);


            foreach (var type in types)
            {
                var entryName = type.Name;
                menu.AddItem(new GUIContent(entryName), property.GetManagedReferenceFullType() == type, assignNewInstanceCmd, new AssignNewInstanceData(type, property));
            }

            void assignNewInstanceCmd(object obj)
            {
                var data = (AssignNewInstanceData)obj;
                data.Property.AssignNewInstanceOfTypeToManagedReference(data.Type);
            }

            menu.ShowAsContext();
        }

        private readonly struct AssignNewInstanceData
        {
            public readonly SerializedProperty Property;
            public readonly Type Type;

            public AssignNewInstanceData(Type type, SerializedProperty property)
            {
                Type = type;
                Property = property;
            }
        }
    }
}

