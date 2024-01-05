using BepInEx.Configuration;
using UnityEngine;
using UnityEngine.Networking;
using RoR2;
using static ReturnsArtifacts.Scripts.Plugin;
using R2API;
using R2API.Utils;
using R2API.AddressReferencedAssets;
using System;
using System.Runtime;
using Newtonsoft.Json.Utilities;
using UnityEngine.AddressableAssets;

namespace ReturnsArtifacts.Scripts.Artifacts {
    class ArtifactOfPrestige : ArtifactBase {
        public override string ArtifactName => "Artifact of Prestige";
        public override string ArtifactLangTokenName => "ARTIFACT_OF_PRESTIGE";
        public override string ArtifactDescription => "At least one Shrine of the Mountain spawns every stage. Shrine of the Mountain effects are permanent.";
        public override Sprite ArtifactEnabledIcon => Assets.LoadAsset<Sprite>("ArtifactOfPrestigeEnabled.png");
        public override Sprite ArtifactDisabledIcon => Assets.LoadAsset<Sprite>("ArtifactOfPrestigeDisabled.png");

        private SpawnCard mountainShrineSpawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/ShrineBoss/iscShrineBoss.asset").WaitForCompletion();


        private readonly static Color shrineSymbolColor = new Color(0.8f, 0.2f, 0.8f);

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
                LogDebug("resetting shrine stacks");
                shrineStacks = 0;
            };
            // Save the teleporter shrine stacks
            Stage.onServerStageComplete += (_) => {
                LogDebug($"setting shrinestacks to {TeleporterInteraction.instance.shrineBonusStacks}");
                shrineStacks = TeleporterInteraction.instance.shrineBonusStacks;
            };
            // Load the previous teleporter shrine stacks
            Stage.onServerStageBegin += (_) => {
                LogDebug($"setting teleporter shrinestacks to {shrineStacks}");
                for (int i = 0; i < shrineStacks; i++) {
                    TeleporterInteraction.instance.AddShrineStack();
                }
                
            };

        }

        private void ConvertSpawnedMountainShrines(SpawnCard.SpawnResult result) {
            if (result.success) {
                if (result.spawnRequest.spawnCard.IsMountainShrine()) {
                    GameObject shineOfTheMountain = result.spawnedInstance;
                    shineOfTheMountain.transform.Find("Symbol").GetComponent<MeshRenderer>().sharedMaterial.SetColor("_TintColor", shrineSymbolColor);
                    shineOfTheMountain.GetComponent<PurchaseInteraction>().setUnavailableOnTeleporterActivated = false;
                }
            }
        }

        private DirectorCard FindMountainShrine(DirectorCardCategorySelection selection) {
            foreach (DirectorCardCategorySelection.Category category in selection.categories) {
                foreach (DirectorCard card in category.cards) {
                    if (card.spawnCard.IsMountainShrine()) {
                        return card;
                    }
                }
            }
            return null;
        }
        private void SpawnMountainShrine(SceneDirector director, DirectorCardCategorySelection selection) {
            // checks if 
            DirectorCard mountainShrine = FindMountainShrine(selection);

            if (mountainShrine == null)
                return;

            LogDebug("hopefully this doesn't end up in a infinite loop");
            GameObject go;
            do {
                go = DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(
                    mountainShrine.spawnCard,
                    new DirectorPlacementRule {
                        placementMode = DirectorPlacementRule.PlacementMode.Random
                    },
                    director.rng
                ));
            } while (go == null);
        }
    }
}