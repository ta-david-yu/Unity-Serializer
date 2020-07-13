using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
                var labelPosition = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
                EditorGUI.LabelField(labelPosition, label);

                if (attr == null)
                {
                    var pos = position;
                    pos.height = m_HelpBoxVerticalPadding;
                    pos.y += base.GetPropertyHeight(property, label);
                    pos.x += EditorGUIUtility.labelWidth + 1 * EditorGUIUtility.standardVerticalSpacing;
                    pos.width -= EditorGUIUtility.labelWidth + 1 * EditorGUIUtility.standardVerticalSpacing;
                    EditorGUI.HelpBox(pos, "A SerializableType field requires either 'InterfaceImplementation' or 'ClassExtension' attribute to work properly.", MessageType.Error);
                }

                var buttonPosition = position;
                buttonPosition.x += EditorGUIUtility.labelWidth + 1 * EditorGUIUtility.standardVerticalSpacing;
                buttonPosition.width -= EditorGUIUtility.labelWidth + 1 * EditorGUIUtility.standardVerticalSpacing;
                buttonPosition.height = EditorGUIUtility.singleLineHeight;

                // get list of derived types' names
                var types = attr == null ? new List<System.Type>() : attr.GetEnumerableOfSatisfiedTypes().ToList();
                Type type = Type.GetType(typenameProperty.stringValue);
                string typename = type == null? "(Null)" : 
                    DYSerializerSettings.GetOrCreateSettings(DYSerializerSettingsProvider.c_SettingsPath, DYSerializerSettingsProvider.c_SettingsFilename).ShowTypeNameWithNameSpace? type.FullName : type.Name;

                var tipmsg = type == null? "" : type.FullName + " (" + type?.Assembly.GetName().Name + ")";

                var storedColor = GUI.backgroundColor;
                GUI.backgroundColor = Color.yellow;

                if (GUI.Button(buttonPosition, new GUIContent(typename, tipmsg)))
                    drawTypeDropDownList(position, property, types);

                GUI.backgroundColor = storedColor;
            }
            EditorGUI.EndProperty();
        }

        private void drawTypeDropDownList(Rect position, SerializedProperty property, List<Type> types)
        {
            SerializedProperty typenameProperty = property.FindPropertyRelative("m_TypeName");
            string typename = typenameProperty.stringValue;
            var currType = System.Type.GetType(typename);

            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("(Null)"), currType is null, assignNewType, "");

            foreach (var type in types)
            {
                var typeName = type.Name;
                menu.AddItem(new GUIContent(typeName), currType == type, assignNewType, type.AssemblyQualifiedName);
            }

            void assignNewType(object typenameObj)
            {
                var newTypename = (string)typenameObj;
                typenameProperty.serializedObject.Update();
                typenameProperty.stringValue = newTypename;
                typenameProperty.serializedObject.ApplyModifiedProperties();

                // Callback
                OnSerializableTypeChangedAttribute[] onSerializableTypeChangedAttirbutes = PropertyUtility.GetAttributes<OnSerializableTypeChangedAttribute>(property);
                if (onSerializableTypeChangedAttirbutes.Length != 0)
                {
                    object target = PropertyUtility.GetTargetObjectWithProperty(property);
                    Type newAssignedType = Type.GetType(newTypename);
                    foreach (var attri in onSerializableTypeChangedAttirbutes)
                    {
                        MethodInfo callbackMethod = ReflectionUtility.GetMethod(target, attri.CallbackName);
                        if (callbackMethod != null &&
                            callbackMethod.ReturnType == typeof(void) &&
                            callbackMethod.GetParameters().Length == 1 &&
                            callbackMethod.GetParameters()[0].ParameterType == typeof(Type))
                        {
                            callbackMethod.Invoke(target, new object[] { newAssignedType });
                        }
                        else
                        {
                            string warning = string.Format(
                                "{0} can only invoke methods with 'void' return type and 1 Type parameter",
                                attri.GetType().Name);

                            Debug.LogWarning(warning, property.serializedObject.targetObject);
                        }
                    }
                }
            }

            menu.ShowAsContext();
        }
    }
}
