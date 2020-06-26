using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DYSerializer
{
    public abstract class InterfaceContainer
    {
        [SerializeField]
        protected object m_Field;
    }

    public abstract class InterfaceContainer<TInterface> : InterfaceContainer, ISerializationCallbackReceiver
    {
        private System.Type m_Type;
        public System.Type Type
        {
            get { return m_Type; }
            set
            {
                if (value != null && !value.IsClass)
                    throw new ArgumentException(string.Format("'{0}' is not a class type.", value.FullName), "value");

                m_Type = value;
                m_TypeName = (value == null) ? "" : value.AssemblyQualifiedName;
            }
        }

        [SerializeField]
        private string m_TypeName;

        private TInterface m_Value;

        public TInterface Value
        {
            get
            {
                return m_Value;
            }

            set
            {
                // TODO
            }
        }

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {

        }
    }
}
