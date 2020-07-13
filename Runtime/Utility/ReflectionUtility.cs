using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace DYSerializer
{
    public static class ReflectionUtility
    {
        static ReflectionUtility() { }
        /*
        public static IEnumerable<T> GetEnumerableOfTypeInstance<T>(params object[] constructorArgs) where T : class
        {
            List<T> objects = new List<T>();
            foreach (Type type in
                Assembly.GetAssembly(typeof(T)).GetTypes()
                .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(T))))
            {
                objects.Add((T)Activator.CreateInstance(type, constructorArgs));
            }
            return objects;
        }*/

        public static IEnumerable<Type> GetEnumerableOfClassExtension<T>(bool allowAbstract = false) where T : class
        {
            return GetEnumerableOfClassExtension(typeof(T), allowAbstract);
        }

        public static IEnumerable<Type> GetEnumerableOfClassExtension(Type baseType, bool allowAbstract = false)
        {
            List<Type> types = new List<Type>();
            foreach (Type type in TypeCache.GetTypesDerivedFrom(baseType)
                //Assembly.GetAssembly(baseType).GetTypes()
                .Where(myType =>
                    myType.IsClass && !myType.IsGenericType &&
                    (allowAbstract || !myType.IsAbstract) &&
                    myType.IsSubclassOf(baseType)))
            {
                types.Add(type);
            }
            types.Sort((a, b) => a.FullName.CompareTo(b.FullName));
            return types;
        }

        public static IEnumerable<Type> GetEnumerableOfInterfaceImplmentation<T>(bool allowAbstract = false)
        {
            return GetEnumerableOfInterfaceImplmentation(typeof(T), allowAbstract);
        }

        public static IEnumerable<Type> GetEnumerableOfInterfaceImplmentation(Type interfaceType, bool allowAbstract = false)
        {
            if (interfaceType != null && interfaceType.IsInterface)
            {
                List<Type> types = new List<Type>();

                foreach (Type type in TypeCache.GetTypesDerivedFrom(interfaceType)
                    //Assembly.GetAssembly(interfaceType).GetTypes()
                    .Where(myType =>
                        !myType.IsInterface && !myType.IsGenericType &&
                        (allowAbstract || !myType.IsAbstract) &&
                        interfaceType.IsAssignableFrom(myType)))
                {
                    types.Add(type);
                }
                types.Sort((a, b) => a.FullName.CompareTo(b.FullName));
                return types;
            }
            UnityEngine.Debug.LogWarning("Fail to get a list of class that implements " + (interfaceType == null));
            return new List<Type>();
        }

        public static bool HasImplicitConversion(Type baseType, Type targetType)
        {
            return baseType.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(mi => mi.Name == "op_Implicit" && mi.ReturnType == targetType)
                .Any(mi => {
                    ParameterInfo pi = mi.GetParameters().FirstOrDefault();
                    return pi != null && pi.ParameterType == baseType;
                });
        }

        public static IEnumerable<FieldInfo> GetAllFields(object target, Func<FieldInfo, bool> predicate)
        {
            List<Type> types = new List<Type>()
            {
                target.GetType()
            };

            while (types.Last().BaseType != null)
            {
                types.Add(types.Last().BaseType);
            }

            for (int i = types.Count - 1; i >= 0; i--)
            {
                IEnumerable<FieldInfo> fieldInfos = types[i]
                    .GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly)
                    .Where(predicate);

                foreach (var fieldInfo in fieldInfos)
                {
                    yield return fieldInfo;
                }
            }
        }

        public static IEnumerable<PropertyInfo> GetAllProperties(object target, Func<PropertyInfo, bool> predicate)
        {
            List<Type> types = new List<Type>()
            {
                target.GetType()
            };

            while (types.Last().BaseType != null)
            {
                types.Add(types.Last().BaseType);
            }

            for (int i = types.Count - 1; i >= 0; i--)
            {
                IEnumerable<PropertyInfo> propertyInfos = types[i]
                    .GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly)
                    .Where(predicate);

                foreach (var propertyInfo in propertyInfos)
                {
                    yield return propertyInfo;
                }
            }
        }

        public static IEnumerable<MethodInfo> GetAllMethods(object target, Func<MethodInfo, bool> predicate)
        {
            IEnumerable<MethodInfo> methodInfos = target.GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(predicate);

            return methodInfos;
        }

        public static FieldInfo GetField(object target, string fieldName)
        {
            return GetAllFields(target, f => f.Name.Equals(fieldName, StringComparison.InvariantCulture)).FirstOrDefault();
        }

        public static PropertyInfo GetProperty(object target, string propertyName)
        {
            return GetAllProperties(target, p => p.Name.Equals(propertyName, StringComparison.InvariantCulture)).FirstOrDefault();
        }

        public static MethodInfo GetMethod(object target, string methodName)
        {
            return GetAllMethods(target, m => m.Name.Equals(methodName, StringComparison.InvariantCulture)).FirstOrDefault();
        }
    }
}