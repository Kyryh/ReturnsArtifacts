using BepInEx.Configuration;
using UnityEngine;
using UnityEngine.Networking;
using RoR2;
using static ReturnsArtifacts.Scripts.Plugin;

namespace ReturnsArtifacts.Scripts.Artifacts {
    class ArtifactOfSpirit : ArtifactBase {
        public override string ArtifactName => "Artifact of Spirit";
        public override string ArtifactLangTokenName => "ARTIFACT_OF_SPIRIT";
        public override string ArtifactDescription => "Characters run faster at lower health.";
        public override Sprite ArtifactEnabledIcon => Assets.LoadAsset<Sprite>("ArtifactOfSpiritEnabled.png");
        public override Sprite ArtifactDisabledIcon => Assets.LoadAsset<Sprite>("ArtifactOfSpiritDisabled.png");




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