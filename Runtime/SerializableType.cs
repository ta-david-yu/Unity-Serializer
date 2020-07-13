using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace DYSerializer
{
    [System.Serializable]
    public class SerializableType : ISerializationCallbackReceiver
    {
        private System.Type m_Type;
        public System.Type Type
        {
            get { return m_Type; }
            set
            {
                if (value != null && !(value.IsValueType || value.IsClass))
                    throw new ArgumentException(string.Format("'{0}' is not a class/struct type.", value.FullName), "value");

                m_Type = value;
                m_TypeName = (value == null)? "" : value.AssemblyQualifiedName;
            }
        }

        [SerializeField]
        private string m_TypeName;

        public SerializableType()
        {

        }

        public SerializableType(System.Type type)
        {
            Type = type;
        }

        public SerializableType(string assemblyFullName)
        {
            Type = !string.IsNullOrEmpty(assemblyFullName) ? System.Type.GetType(assemblyFullName) : null;
        }

        public static implicit operator string(SerializableType type)
        {
            return type.m_TypeName;
        }

        public static implicit operator Type(SerializableType type)
        {
            return type.Type;
        }

        public static implicit operator SerializableType(Type type)
        {
            return new SerializableType(type);
        }

        public override string ToString()
        {
            return Type != null ? Type.FullName : "(Null)";
        }

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            if (!string.IsNullOrEmpty(m_TypeName))
            {
                Type = System.Type.GetType(m_TypeName);

                if (Type == null)
                {
                    Debug.LogWarning(string.Format("'{0}' was referenced but class type was not found.", m_TypeName));
                }
            }
            else
            {
                Type = null;
            }
        }
    }
}
