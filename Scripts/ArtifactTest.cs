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
        public override Sprite ArtifactEnabledIcon => Assets.LoadAsset<Sprite>("ExampleArtifactEnabledIcon.png");
        public override Sprite ArtifactDisabledIcon => Assets.LoadAsset<Sprite>("ExampleArtifactDisabledIcon.png");
        public override void Init(ConfigFile config)
        {
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
                for (int i = 0; i < TimesToPrintMessageOnStart.Value; i++)
                {
                    Chat.AddMessage("Example Artifact has been Enabled.");
                }
            }
        }
    }
}