using BepInEx.Configuration;
using UnityEngine;
using UnityEngine.Networking;
using RoR2;
using static ReturnsArtifacts.Scripts.Plugin;
using R2API;
using UnityEngine.AddressableAssets;
using RiskOfOptions;
using RiskOfOptions.Options;
using RiskOfOptions.OptionConfigs;

namespace ReturnsArtifacts.Scripts.Artifacts {
    class ArtifactOfPrestige : ArtifactBase {
        public override string ArtifactName => "Artifact of Prestige";
        public override string ArtifactLangTokenName => "ARTIFACT_OF_PRESTIGE";
        public override string ArtifactDescription => "At least one Shrine of the Mountain spawns every stage. Shrine of the Mountain effects are permanent.";
        public override Sprite ArtifactEnabledIcon => Assets.LoadAsset<Sprite>("ArtifactOfPrestigeEnabled.png");
        public override Sprite ArtifactDisabledIcon => Assets.LoadAsset<Sprite>("ArtifactOfPrestigeDisabled.png");

        private InteractableSpawnCard[] mountainShrineSpawnCards = [
            Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/Base/ShrineBoss/iscShrineBoss.asset").WaitForCompletion(),
            Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/Base/ShrineBoss/iscShrineBossSandy.asset").WaitForCompletion(),
            Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/Base/ShrineBoss/iscShrineBossSnowy.asset").WaitForCompletion()
        ];

        public static readonly ConfigEntry<int> numMountainShrinesToSpawn = Plugin.ConfigFile.Bind(
            "Prestige",
            "MountainShrinesToSpawn",
            1,
            "Number of Shrines of the Mountain to spawn each stage"
        );
        
        public static readonly ConfigEntry<Color> shrineSymbolColor = Plugin.ConfigFile.Bind(
            "Prestige",
            "ShrineSymbolColor",
            new Color(0.8f, 0.2f, 0.8f),
            "Color of the Shrine of the Mountain symbol"
        );

        private static int shrineStacks;
        public override void Init() {
            CreateLang();
            CreateArtifact();
            Hooks();
        }

        public override void Hooks() {
            SceneDirector.onGenerateInteractableCardSelection += SpawnMountainShrine;
            SpawnCard.onSpawnedServerGlobal += ConvertSpawnedMountainShrines;
            // Set the shrine stacks to 0 on run start
            Run.onRunStartGlobal += (_) => {
                //LogDebug("resetting shrine stacks");
                if (!ArtifactEnabled || NetworkServer.active)
                    shrineStacks = 0;
            };
            // Load the previous teleporter shrine stacks
            Stage.onServerStageBegin += (_) => {
                //LogDebug($"setting teleporter shrinestacks to {shrineStacks}");
                if (!ArtifactEnabled || TeleporterInteraction.instance == null)
                    return;
                for (int i = 0; i < shrineStacks; i++) {
                    TeleporterInteraction.instance.AddShrineStack();
                }
                TeleporterInteraction.instance.transform.Find("TeleporterBaseMesh/BossShrineSymbol").GetComponent<MeshRenderer>().material.SetColor("_TintColor", shrineSymbolColor.Value);
            };

            On.RoR2.ShrineBossBehavior.AddShrineStack += OnAddShrineStack;
        }



        private void ConvertSpawnedMountainShrines(SpawnCard.SpawnResult result) {
            if (ArtifactEnabled && result.success) {
                if (result.spawnRequest.spawnCard.IsMountainShrine()) {
                    GameObject shineOfTheMountain = result.spawnedInstance;
                    shineOfTheMountain.transform.Find("Symbol").GetComponent<MeshRenderer>().material.SetColor("_TintColor", shrineSymbolColor.Value);
                    shineOfTheMountain.GetComponent<PurchaseInteraction>().setUnavailableOnTeleporterActivated = false;
                }
            }
        }

        
        private void SpawnMountainShrine(SceneDirector director, DirectorCardCategorySelection selection) {
            if (!ArtifactEnabled)
                return;
            InteractableSpawnCard mountainShrine = GetMountainShrine();

            if (mountainShrine == null)
                return;

            int shrinesSpawned = 0;

            //LogDebug("hopefully this doesn't end up in a infinite loop");
            while (shrinesSpawned < numMountainShrinesToSpawn.Value) {
                GameObject go = DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(
                    mountainShrine,
                    new DirectorPlacementRule {
                        placementMode = DirectorPlacementRule.PlacementMode.Random
                    },
                    director.rng
                ));
                if (go != null)
                    shrinesSpawned++;
            } 
        }

        private void OnAddShrineStack(On.RoR2.ShrineBossBehavior.orig_AddShrineStack orig, ShrineBossBehavior self, Interactor interactor) {
            shrineStacks++;
            orig(self, interactor);
        }
        private InteractableSpawnCard GetMountainShrine() {
            DirectorAPI.Stage stage = DirectorAPI.GetStageEnumFromSceneDef(SceneCatalog.GetSceneDefForCurrentScene());

            // checks which version of the shrine of the mountain to use
            // technically on some stages (e.g. rallypoint delta) shrines
            // of the mountain can't spawn, but i spawn them anyway
            // because why not
            switch (stage) {
                case DirectorAPI.Stage.AbyssalDepths:
                case DirectorAPI.Stage.AphelianSanctuary:
                case DirectorAPI.Stage.DistantRoost:
                case DirectorAPI.Stage.ScorchedAcres:
                case DirectorAPI.Stage.SirensCall:
                case DirectorAPI.Stage.SkyMeadow:
                case DirectorAPI.Stage.SulfurPools:
                case DirectorAPI.Stage.SunderedGrove:
                case DirectorAPI.Stage.TitanicPlains:
                case DirectorAPI.Stage.WetlandAspect:
                    return mountainShrineSpawnCards[0];
                case DirectorAPI.Stage.AbandonedAqueduct:
                    return mountainShrineSpawnCards[1];
                case DirectorAPI.Stage.SiphonedForest:
                case DirectorAPI.Stage.RallypointDelta:
                    return mountainShrineSpawnCards[2];
            }
            return null;
        }

    }
}