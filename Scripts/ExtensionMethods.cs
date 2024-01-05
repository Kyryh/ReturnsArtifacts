using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RoR2;
using System.Text;
using R2API;

namespace ReturnsArtifacts.Scripts {
    internal static class ExtensionMethods {
        public static IEnumerable<Type> GetTypesOfType<T>(this Assembly assembly) {
            return assembly.GetTypes().Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(T)));
        }

        public static bool IsMountainShrine(this SpawnCard spawnCard) {
            return spawnCard.name.ToLower().Contains(DirectorAPI.Helpers.InteractableNames.ShrineOftheMountain);
        }
    }
}
