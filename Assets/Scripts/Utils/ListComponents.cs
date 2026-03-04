using UnityEngine;
using System;
using System.Linq;
using System.Reflection;

public class ListAllComponents : MonoBehaviour
{
    [ContextMenu("List All Components")]
    private void ListComponents()
    {
        // Get all loaded assemblies
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        foreach (var assembly in assemblies)
        {
            // Get all types in the assembly
            Type[] types;
            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                types = e.Types.Where(t => t != null).ToArray();
            }

            foreach (var type in types)
            {
                // Check if type inherits from UnityEngine.Component
                if (typeof(Component).IsAssignableFrom(type))
                {
                    Debug.Log($"Component: {type.FullName} ({type.Assembly.GetName().Name})");
                }
            }
        }
    }
}