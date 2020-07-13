using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DYSerializer
{
    [AttributeUsage(AttributeTargets.Field)]
    public class OnSerializableTypeChangedAttribute : PropertyAttribute
    {
        public string CallbackName { get; private set; }
        public OnSerializableTypeChangedAttribute(string callbackName)
        {
            CallbackName = callbackName;
        }
    }
}
