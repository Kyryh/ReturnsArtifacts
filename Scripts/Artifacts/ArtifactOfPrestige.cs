using BepInEx.Configuration;
using UnityEngine;
using UnityEngine.Networking;
using RoR2;
using static ReturnsArtifacts.Scripts.Plugin;

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
            Run.onRunStartGlobal += PrintMessageToChat;

        }
        private void PrintMessageToChat(Run run) {
            if (NetworkServer.active && ArtifactEnabled) {
                Chat.AddMessage("Example Artifact has been Enabled.");
            }
        }
    }
}