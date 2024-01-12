using BepInEx;
using Newtonsoft.Json.Linq;
using R2API.Utils;
using ReturnsArtifacts.Scripts.Artifacts;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using RoR2;
using R2API;
using R2API.ContentManagement;
using RiskOfOptions.OptionConfigs;
using RiskOfOptions.Options;
using RiskOfOptions;


namespace ReturnsArtifacts.Scripts
{
    [BepInDependency("com.rune580.riskofoptions", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(DirectorAPI.PluginGUID)]
    [BepInDependency(RecalculateStatsAPI.PluginGUID)]
    [BepInDependency(R2APIContentManager.PluginGUID)]
    [BepInDependency(LanguageAPI.PluginGUID)]
    [BepInPlugin(PluginInfo.PLUGIN_GUID, "Returns Artifacts", PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {

        public static AssetBundle Assets { get; private set; }
        public static BepInEx.Logging.ManualLogSource Log { get; private set; }

        public static BepInEx.Configuration.ConfigFile ConfigFile { get; private set; }

        public static event Action<float> onGameTimeChanged;

        private static float? previousGameTime;

        private void Awake()
        {
            Log = Logger;

            ConfigFile = Config;

            Assembly assembly = Assembly.GetExecutingAssembly();

            // Load the asset bundle from the mod's assembly
            using (Stream stream = assembly.GetManifestResourceStream("ReturnsArtifacts.Resources.returnsartifactsbundle")) {
                Assets = AssetBundle.LoadFromStream(stream);
            }

            foreach (Type type in assembly.GetTypesOfType<ArtifactBase>()) {
                ArtifactBase artifact = (ArtifactBase)Activator.CreateInstance(type);
                artifact.Init();
            }

            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.rune580.riskofoptions")) {
                AddConfigOptions();
            }


            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }

        private void FixedUpdate() {
            if (Run.instance != null) {
                float gameTime = Run.instance.GetRunStopwatch();
                if (gameTime != previousGameTime) {
                    onGameTimeChanged?.Invoke(gameTime);
                    previousGameTime = gameTime;
                }
            }
        }

        private void AddConfigOptions() {
            ModSettingsManager.AddOption(new IntSliderOption(ArtifactOfCognation.cloneLifespan, new IntSliderConfig() { min = 5, max = 60 }));

            ModSettingsManager.AddOption(new IntSliderOption(ArtifactOfDistortion.cycleDuration, new IntSliderConfig() { min = 20, max = 300 }));
            ModSettingsManager.AddOption(new SliderOption(ArtifactOfDistortion.cooldownReduction, new SliderConfig() { min = 0, max = 100 }));
            ModSettingsManager.AddOption(new IntSliderOption(ArtifactOfDistortion.skillsToLock, new IntSliderConfig() { min = 1, max = 3 }));

            ModSettingsManager.AddOption(new IntSliderOption(ArtifactOfPrestige.numMountainShrinesToSpawn, new IntSliderConfig() { min = 0, max = 5 }));
            ModSettingsManager.AddOption(new ColorOption(ArtifactOfPrestige.shrineSymbolColor));

            ModSettingsManager.AddOption(new IntSliderOption(ArtifactOfTempus.itemLifespan, new IntSliderConfig() { min = 30, max = 600 }));
            ModSettingsManager.AddOption(new IntSliderOption(ArtifactOfTempus.baseNumStacks, new IntSliderConfig() { min = 1, max = 10 }));
            ModSettingsManager.AddOption(new IntSliderOption(ArtifactOfTempus.bonusNumStacks, new IntSliderConfig() { min = 0, max = 5 }));
            ModSettingsManager.AddOption(new IntSliderOption(ArtifactOfTempus.stagesPerBonusStacks, new IntSliderConfig() { min = 1, max = 5 }));
        }
        
        

        public static void LogDebug(object data) {
            Log.LogDebug(data);
        }

        public static void LogError(object data) {
            Log.LogError(data);
        }
    }
}
