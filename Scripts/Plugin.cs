using BepInEx;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ReturnsArtifacts
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, "Returns Artifacts", PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {

        public static AssetBundle Assets { get; private set; }
        public static BepInEx.Logging.ManualLogSource Log {  get; private set; }

        private void Awake()
        {
            Log = Logger;

            Assembly assembly = Assembly.GetExecutingAssembly();

            // Load the asset bundle from the mod's assembly
            using (Stream stream = assembly.GetManifestResourceStream("ReturnsArtifacts.Resources.returnsartifacts")) {
                Assets = AssetBundle.LoadFromStream(stream);
            }



            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }

        private IEnumerable<T> GetTypesOfType<T>(this Assembly assembly) {
            return assembly.GetTypes().Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(T)));
        }
    }
}
