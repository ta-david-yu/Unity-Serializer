using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DYSerializer
{
    [CustomPropertyDrawer(typeof(SerializableType))]
    [CustomPropertyDrawer(typeof(ClassConstraintAttribute), true)]
    public class SerializableTypePropertyDrawer : PropertyDrawer
    {
        private float m_HelpBoxVerticalPadding = 40;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) + ((attribute == null)? m_HelpBoxVerticalPadding : 0);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var attr = attribute as ClassConstraintAttribute;

            // show type name property ui
            if (Type.GetType(label.text) != null)
            {
                label.text = SerializedPropertyUtility.GetSplitNamesFromAssemblyQualifiedName(label.text).AssemblyName;
            }

            SerializedProperty typenameProperty = property.FindPropertyRelative("m_TypeName");
            EditorGUI.BeginProperty(position, label, typenameProperty);
            {
                if (attr == null)
                {
                    var pos = position;
                    pos.height = m_HelpBoxVerticalPadding;
                    pos.y += base.GetPropertyHeight(property, label);
                    EditorGUI.HelpBox(pos, "A SerializableType field requires either 'InterfaceImplementation' or 'ClassExtension' attribute to work properly.", MessageType.Error);
                }

                // get list of derived types' names
                var types = attr == null ? new List<System.Type>() : attr.GetEnumerableOfSatisfiedTypes().ToList();

                List<string> typeNameList = new List<string>();
                typeNameList.Add("(Null)");
                typeNameList.AddRange(types.Select((type) => { return type.FullName; }));

                string propertyString = typenameProperty.stringValue;
                int index = -1;
                if (propertyString == "")
                {
                    // The tag is empty
                    index = 0; // first index is the special <notag> entry
                }
                else
                {
                    // check if there is an entry that matches the entry and get the index
                    // we skip index 0 as that is a special custom case
                    for (int i = 0; i < types.Count; i++)
                    {
                        if (types[i].AssemblyQualifiedName == propertyString)
                        {
                            index = i + 1;
                            break;
                        }
                    }
                }

                // Draw the popup box with the current selected index
                using (var disabledGroup = new EditorGUI.DisabledGroupScope(attr == null))
                {
                    index = EditorGUI.Popup(position, label.text, index, typeNameList.ToArray());
                }

                // Adjust the actual string value of the property based on the selection
                if (index == 0)
                {
                    typenameProperty.stringValue = "";
                }
                else if (index >= 1)
                {
                    typenameProperty.stringValue = types[index - 1].AssemblyQualifiedName;
                }
                else
                {
                    typenameProperty.stringValue = "";
                }
            }
            EditorGUI.EndProperty();
        }
    }
}
