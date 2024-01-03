using BepInEx.Configuration;
using UnityEngine;
using UnityEngine.Networking;
using RoR2;
using static ReturnsArtifacts.Scripts.Plugin;

namespace ReturnsArtifacts.Scripts
{
    class ExampleArtifact : ArtifactBase
    {
        public static ConfigEntry<int> TimesToPrintMessageOnStart;
        public override string ArtifactName => "Artifact of Example";
        public override string ArtifactLangTokenName => "ARTIFACT_OF_EXAMPLE";
        public override string ArtifactDescription => "When enabled, print a message to the chat at the start of the run.";
        public override Sprite ArtifactEnabledIcon => Assets.LoadAsset<Sprite>("ArtifactOfCognationEnabled.png");
        public override Sprite ArtifactDisabledIcon => Assets.LoadAsset<Sprite>("ArtifactOfCognationDisabled.png");
        



        public override void Init() {
            CreateLang();
            CreateArtifact();
            Hooks();
        }

        public override void Hooks()
        {
            Run.onRunStartGlobal += PrintMessageToChat;

        }
        private void PrintMessageToChat(Run run)
        {
            if (NetworkServer.active && ArtifactEnabled)
            {
                Chat.AddMessage("Example Artifact has been Enabled.");
            }
        }
    }
}