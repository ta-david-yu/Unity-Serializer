using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DYSerializer
{
    public static class SerializedPropertyUtility
    {
        /// Creates instance of passed type and assigns it to managed reference
        public static object AssignNewInstanceOfTypeToManagedReference(this SerializedProperty serializedProperty, Type type)
        {
            var instance = Activator.CreateInstance(type);

            Undo.RegisterCompleteObjectUndo(serializedProperty.serializedObject.targetObject, "Change property managed reference type");

            serializedProperty.serializedObject.Update();
            serializedProperty.managedReferenceValue = instance;
            serializedProperty.serializedObject.ApplyModifiedPropertiesWithoutUndo();

            return instance;
        }

        /// Sets managed reference to null
        public static void SetManagedReferenceToNull(this SerializedProperty serializedProperty)
        {
            Undo.RegisterCompleteObjectUndo(serializedProperty.serializedObject.targetObject, "Change property managed reference type to null");

            serializedProperty.serializedObject.Update();
            serializedProperty.managedReferenceValue = null;
            serializedProperty.serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        public static Type GetManagedReferenceFieldType(this SerializedProperty property)
        {
            var names = GetSplitNamesFromManagerReferenceFullTypename(property.managedReferenceFieldTypename);
            var realType = Type.GetType($"{names.ClassName}, {names.AssemblyName}");

            if (realType != null)
                return realType;

            Debug.LogError($"Can not get field type of managed reference : {property.managedReferenceFieldTypename}");
            return null;
        }

        public static Type GetManagedReferenceFullType(this SerializedProperty property)
        {
            var names = GetSplitNamesFromManagerReferenceFullTypename(property.managedReferenceFullTypename);
            var realType = Type.GetType($"{names.ClassName}, {names.AssemblyName}");

            return realType;
        }

        /// Get assembly and class names from typeName
        public static (string AssemblyName, string ClassName) GetSplitNamesFromManagerReferenceFullTypename(string typename)
        {
            if (string.IsNullOrEmpty(typename)) 
                return ("", "");

            var typeSplitString = typename.Split(char.Parse(" "));
            var typeClassName = typeSplitString[1];
            var typeAssemblyName = typeSplitString[0];
            return (typeAssemblyName, typeClassName);
        }

        public static (string AssemblyName, string ClassName) GetSplitNamesFromAssemblyQualifiedName(string typename)
        {
            if (string.IsNullOrEmpty(typename))
                return ("", "");


            var typeSplitString = typename.Split(new string[] { ", " }, StringSplitOptions.None);
            var typeClassName = typeSplitString[1];
            var typeAssemblyName = typeSplitString[0];
            return (typeAssemblyName, typeClassName);
        }
    }
}