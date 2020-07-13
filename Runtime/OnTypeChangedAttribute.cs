using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DYSerializer
{
    /// <summary>
    /// OnTypeChangedAttribute can be applied to SerializableType or SerializeRefUI, the callback method when the type or the type of the instance is changed (Generic Menu button invoke)
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class OnTypeChangedAttribute : PropertyAttribute
    {
        public string CallbackName { get; private set; }
        public OnTypeChangedAttribute(string callbackName)
        {
            CallbackName = callbackName;
        }
    }
}
