using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RoR2;
using R2API;
using ReturnsArtifacts.Scripts.Artifacts;
using Random = UnityEngine.Random;

namespace ReturnsArtifacts.Scripts {
    internal static class ExtensionMethods {
        public static IEnumerable<Type> GetTypesOfType<T>(this Assembly assembly) {
            return assembly.GetTypes().Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(T)));
        }

        public static bool IsMountainShrine(this SpawnCard spawnCard) {
            return spawnCard.name.ToLower().Contains(DirectorAPI.Helpers.InteractableNames.ShrineOftheMountain);
        }

        public static T[] Shuffled<T>(this T[] collection) {
            for (int s = 0; s < collection.Length - 1; s++) {
                int randomIndex = Random.Range(s, collection.Length);

                (collection[randomIndex], collection[s]) = (collection[s], collection[randomIndex]);
            }
            return collection;
        }

        public static bool IndexOfInventory(this List<TemporaryInventory> inventories, Inventory inventory, out int index) {
            index = inventories.FindIndex(x => x.inventory == inventory);
            return index != -1;
        }
    }
}
