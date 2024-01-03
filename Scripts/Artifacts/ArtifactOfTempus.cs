using BepInEx.Configuration;
using UnityEngine;
using UnityEngine.Networking;
using RoR2;
using static ReturnsArtifacts.Scripts.Plugin;

namespace ReturnsArtifacts.Scripts.Artifacts {
    class ArtifactOfTempus : ArtifactBase {
        public override string ArtifactName => "Artifact of Tempus";
        public override string ArtifactLangTokenName => "ARTIFACT_OF_TEMPUS";
        public override string ArtifactDescription => "Items are worth multiple stacks. All items are temporary.";
        public override Sprite ArtifactEnabledIcon => Assets.LoadAsset<Sprite>("ArtifactOfTempusEnabled.png");
        public override Sprite ArtifactDisabledIcon => Assets.LoadAsset<Sprite>("ArtifactOfTempusDisabled.png");




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