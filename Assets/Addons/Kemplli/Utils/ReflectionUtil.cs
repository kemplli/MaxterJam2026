using System;
using System.Linq;
using System.Reflection;

public static class ReflectionUtil
{
    public static Type[] FindAllSubclaces<T>()
    {
        Type baseType = typeof(T);
        Assembly assembly = Assembly.GetAssembly(baseType);

        Type[] types = assembly.GetTypes();
        Type[] subclasses = types.Where(type => type.IsSubclassOf(baseType) && !type.IsAbstract).ToArray();

        return subclasses;
    }

    public static Type[] FindAllSublacesWithInterfaces<T>()
    {
        Type baseType = typeof(T);
        Assembly assembly = Assembly.GetAssembly(baseType);

        Type[] types = assembly.GetTypes();

        return types.Where(x => x.GetInterfaces().Contains(typeof(T))).ToArray();
    } 
}

