using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DYSerializer
{
    [AttributeUsage(AttributeTargets.Field)]
    public class SerializeRefUIAttribute : PropertyAttribute
    {
    }
}
