using BepInEx.Configuration;
using UnityEngine;
using UnityEngine.Networking;
using RoR2;
using static ReturnsArtifacts.Scripts.Plugin;
using R2API;
using System;
using System.Runtime;
using R2API.Utils;
using Newtonsoft.Json.Utilities;

namespace ReturnsArtifacts.Scripts.Artifacts {
    class ArtifactOfPrestige : ArtifactBase {
        public override string ArtifactName => "Artifact of Prestige";
        public override string ArtifactLangTokenName => "ARTIFACT_OF_PRESTIGE";
        public override string ArtifactDescription => "At least one Shrine of the Mountain spawns every stage. Shrine of the Mountain effects are permanent.";
        public override Sprite ArtifactEnabledIcon => Assets.LoadAsset<Sprite>("ArtifactOfPrestigeEnabled.png");
        public override Sprite ArtifactDisabledIcon => Assets.LoadAsset<Sprite>("ArtifactOfPrestigeDisabled.png");




        public override void Init() {
            CreateLang();
            CreateArtifact();
            Hooks();
        }

        public override void Hooks() {
            SceneDirector.onGenerateInteractableCardSelection += SpawnMountainShrine;
            SpawnCard.onSpawnedServerGlobal += ConvertSpawnedMountainShrines;
        }

        private void ConvertSpawnedMountainShrines(SpawnCard.SpawnResult result) {
            if (result.success) {
                if (result.spawnRequest.spawnCard.IsMountainShrine()) {
                    GameObject shineOfTheMountain = result.spawnedInstance;
                    shineOfTheMountain.transform.Find("Symbol").GetComponent<MeshRenderer>().sharedMaterial.SetColor("_TintColor", Color.magenta);
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