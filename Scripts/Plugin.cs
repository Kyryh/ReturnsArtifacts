using BepInEx;
using Newtonsoft.Json.Linq;
using R2API.Utils;
using ReturnsArtifacts.Scripts.Artifacts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ReturnsArtifacts.Scripts
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, "Returns Artifacts", PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {

        public static AssetBundle Assets { get; private set; }
        public static BepInEx.Logging.ManualLogSource Log { get; private set; }

        private void Awake()
        {
            Log = Logger;

            Assembly assembly = Assembly.GetExecutingAssembly();

            // Load the asset bundle from the mod's assembly
            using (Stream stream = assembly.GetManifestResourceStream("ReturnsArtifacts.Resources.returnsartifacts")) {
                Assets = AssetBundle.LoadFromStream(stream);
            }

            //LogDebug(Assets);
            //Assets.GetAllAssetNames().ForEachTry(Log.LogDebug);

            foreach (Type type in assembly.GetTypesOfType<ArtifactBase>()) {
                ArtifactBase artifact = (ArtifactBase)Activator.CreateInstance(type);
                artifact.Init();
            }



            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }

        public static void LogDebug(object data) {
            Log.LogDebug(data);
        }

        public static void LogError(object data) {
            Log.LogError(data);
        }
    }
}
