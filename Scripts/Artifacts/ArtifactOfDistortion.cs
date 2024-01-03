using BepInEx.Configuration;
using UnityEngine;
using UnityEngine.Networking;
using RoR2;
using static ReturnsArtifacts.Scripts.Plugin;

namespace ReturnsArtifacts.Scripts.Artifacts {
    class ArtifactOfDistortion : ArtifactBase {
        public override string ArtifactName => "Artifact of Distortion";
        public override string ArtifactLangTokenName => "ARTIFACT_OF_DISTORTION";
        public override string ArtifactDescription => "Lock a random skill every minute, but skills have decreased cooldowns.";
        public override Sprite ArtifactEnabledIcon => Assets.LoadAsset<Sprite>("ArtifactOfDistortionEnabled.png");
        public override Sprite ArtifactDisabledIcon => Assets.LoadAsset<Sprite>("ArtifactOfDistortionDisabled.png");




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