using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DYSerializer
{
    public static class ReflectiveEnumerator
    {
        static ReflectiveEnumerator() { }

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
        }

        public static IEnumerable<Type> GetEnumerableOfClassExtension<T>(bool allowAbstract = false) where T : class
        {
            return GetEnumerableOfClassExtension(typeof(T), allowAbstract);
        }

        public static IEnumerable<Type> GetEnumerableOfClassExtension(Type baseType, bool allowAbstract = false)
        {
            List<Type> types = new List<Type>();
            foreach (Type type in
                Assembly.GetAssembly(baseType).GetTypes()
                .Where(myType =>
                    myType.IsClass &&
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

                foreach (Type type in
                    Assembly.GetAssembly(interfaceType).GetTypes()
                    .Where(myType =>
                        myType.IsClass &&
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
    }
}