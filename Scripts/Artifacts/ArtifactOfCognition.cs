using BepInEx.Configuration;
using UnityEngine;
using UnityEngine.Networking;
using RoR2;
using static ReturnsArtifacts.Scripts.Plugin;

namespace ReturnsArtifacts.Scripts.Artifacts
{
    class ArtifactOfCognition : ArtifactBase
    {
        public override string ArtifactName => "Artifact of Cognation";
        public override string ArtifactLangTokenName => "ARTIFACT_OF_COGNATION";
        public override string ArtifactDescription => "Enemies create a temporary clone on death.";
        public override Sprite ArtifactEnabledIcon => Assets.LoadAsset<Sprite>("ArtifactOfCognationEnabled.png");
        public override Sprite ArtifactDisabledIcon => Assets.LoadAsset<Sprite>("ArtifactOfCognationDisabled.png");




        public override void Init()
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
                Chat.AddMessage("Example Artifact has been Enabled.");
            }
        }
    }
}