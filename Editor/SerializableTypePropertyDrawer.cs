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
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty nameProperty = property.FindPropertyRelative("m_TypeName");
            var attr = attribute as ClassConstraintAttribute;

            var types = attr.GetEnumerableOfSatisfiedTypes().ToList();

            List<string> typeNameList = new List<string>();
            typeNameList.Add("(Null)");
            typeNameList.AddRange(types.Select((type) => { return type.FullName; }));
            
            EditorGUI.BeginProperty(position, label, nameProperty);
            {
                string propertyString = nameProperty.stringValue;
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
                index = EditorGUI.Popup(position, label.text, index, typeNameList.ToArray());

                // Adjust the actual string value of the property based on the selection
                if (index == 0)
                {
                    nameProperty.stringValue = "";
                }
                else if (index >= 1)
                {
                    nameProperty.stringValue = types[index - 1].AssemblyQualifiedName;
                }
                else
                {
                    nameProperty.stringValue = "";
                }
            }
            EditorGUI.EndProperty();
        }
    }
}
