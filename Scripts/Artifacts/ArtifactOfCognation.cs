using BepInEx.Configuration;
using UnityEngine;
using UnityEngine.Networking;
using RoR2;
using static ReturnsArtifacts.Scripts.Plugin;
using RiskOfOptions;
using RiskOfOptions.Options;
using RiskOfOptions.OptionConfigs;

namespace ReturnsArtifacts.Scripts.Artifacts {
    class ArtifactOfCognation : ArtifactBase {
        public override string ArtifactName => "Artifact of Cognation";
        public override string ArtifactLangTokenName => "ARTIFACT_OF_COGNATION";
        public override string ArtifactDescription => "Enemies create a temporary clone on death.";
        public override Sprite ArtifactEnabledIcon => Assets.LoadAsset<Sprite>("ArtifactOfCognationEnabled.png");
        public override Sprite ArtifactDisabledIcon => Assets.LoadAsset<Sprite>("ArtifactOfCognationDisabled.png");

        public static readonly ConfigEntry<int> cloneLifespan = Plugin.ConfigFile.Bind(
            "Cognation",
            "CloneLifespan",
            10,
            "The lifespan of clones spawned by the Artifact of Cognation"
        );


        public override void Init() {
            ModSettingsManager.AddOption(new IntSliderOption(cloneLifespan, new IntSliderConfig() { min = 5, max = 60}));
            CreateLang();
            CreateArtifact();
            Hooks();
        }

        public override void Hooks() {
            GlobalEventManager.onCharacterDeathGlobal += SpawnMonsterClone;
        }

        private void SpawnMonsterClone(DamageReport damageReport) {
            // a bajillion checks 
            if (NetworkClient.active && ArtifactEnabled && damageReport != null && damageReport.victimBody && damageReport.victimTeamIndex != TeamIndex.Player) {
                CharacterBody charBody = Util.TryToCreateGhost(damageReport.victimBody, damageReport.victimBody, cloneLifespan.Value);
                // remove the "BoostDamage" item because otherwise
                // the clones fucking demolish you
                charBody.master.onBodyStart += (charBody) => {
                    charBody.inventory.RemoveItem(RoR2Content.Items.BoostDamage, 150);
                };
            }
        }

    }
}