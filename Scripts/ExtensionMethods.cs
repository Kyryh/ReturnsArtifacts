using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ReturnsArtifacts.Scripts {
    internal static class ExtensionMethods {
        private static IEnumerable<Type> GetTypesOfType<T>(this Assembly assembly) {
            return assembly.GetTypes().Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(T)));
        }
    }
}
