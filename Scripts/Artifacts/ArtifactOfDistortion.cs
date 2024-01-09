using BepInEx.Configuration;
using UnityEngine;
using RoR2;
using static ReturnsArtifacts.Scripts.Plugin;
using UnityEngine.AddressableAssets;
using R2API;
using System;
using RiskOfOptions;
using RiskOfOptions.Options;
using RiskOfOptions.OptionConfigs;

namespace ReturnsArtifacts.Scripts.Artifacts {
    class ArtifactOfDistortion : ArtifactBase {
        public override string ArtifactName => "Artifact of Distortion";
        public override string ArtifactLangTokenName => "ARTIFACT_OF_DISTORTION";
        public override string ArtifactDescription => "Lock a random skill every minute, but skills have decreased cooldowns.";
        public override Sprite ArtifactEnabledIcon => Assets.LoadAsset<Sprite>("ArtifactOfDistortionEnabled.png");
        public override Sprite ArtifactDisabledIcon => Assets.LoadAsset<Sprite>("ArtifactOfDistortionDisabled.png");

        private static readonly ConfigEntry<int> cycleDuration = Plugin.ConfigFile.Bind(
            "Distortion",
            "CycleDuration",
            60,
            "How many seconds it takes to change locked skill(s)"
        );

        private static readonly ConfigEntry<float> cooldownReduction = Plugin.ConfigFile.Bind(
            "Distortion",
            "CooldownReduction",
            25f,
            "Percentage of cooldown reduction to apply to non-locked skills"
        );

        private static readonly ConfigEntry<int> skillsToLock = Plugin.ConfigFile.Bind(
            "Distortion",
            "SkillsToLock",
            1,
            "How many skills to lock each cycle"
        );

        private static event Action onCyclePassed;

        private static UnavailableSkill unavailableSkill;

        private static int[] skillIndexes = [0, 1, 2, 3];


        public override void Init() {
            ModSettingsManager.AddOption(new IntSliderOption(cycleDuration, new IntSliderConfig() { min = 20, max = 300}));
            ModSettingsManager.AddOption(new SliderOption(cooldownReduction, new SliderConfig() { min = 0, max = 100}));
            ModSettingsManager.AddOption(new IntSliderOption(skillsToLock, new IntSliderConfig() { min = 1, max = 3}));
            CreateLang();
            CreateArtifact();
            unavailableSkill = InitSkill();
            Plugin.onGameTimeChanged += OnGameTimeChanged;
            Hooks();
            
        }

        private UnavailableSkill InitSkill() {
            UnavailableSkill skill = ScriptableObject.CreateInstance<UnavailableSkill>();

            skill.icon = Addressables.LoadAssetAsync<Sprite>("RoR2/Base/UI/texGenericSkillIcons.png").WaitForCompletion();
            skill.skillName = "SKILL_" + skill.tokenName;
            skill.skillNameToken = "SKILL_" + skill.tokenName + "_NAME";
            skill.skillDescriptionToken = "SKILL_" + skill.tokenName + "_DESCRIPTION";

            LanguageAPI.Add("SKILL_DISTORTION_UNAVAILABLE_NAME", "Unavailable");
            LanguageAPI.Add("SKILL_DISTORTION_UNAVAILABLE_DESCRIPTION", "It's unavailable, at least for now...");

            return skill;
        }

        public override void Hooks() {
            Run.onRunStartGlobal += _ => {
                if (!ArtifactEnabled)
                    return;
                onCyclePassed = null;
                skillIndexes = skillIndexes.Shuffled();
                onCyclePassed += () => { skillIndexes = skillIndexes.Shuffled(); };
            };
            CharacterBody.onBodyStartGlobal += CharacterBody_onBodyStartGlobal;
            R2API.RecalculateStatsAPI.GetStatCoefficients += ReduceCooldown;
            
        }
        private void OnGameTimeChanged(float time) {
            if (GetTimeCycle(time) != GetTimeCycle(time - Time.fixedDeltaTime)) {
                onCyclePassed?.Invoke();
            }
            
        }
        private int GetTimeCycle(float time) {
            return (int)(time / cycleDuration.Value);
        }

        private void ReduceCooldown(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args) {
            if (ArtifactEnabled)
                args.cooldownMultAdd -= cooldownReduction.Value/100;
        }

        private void CharacterBody_onBodyStartGlobal(CharacterBody body) {
            if (ArtifactEnabled && body.isPlayerControlled) {
                SetUnavailableSkill(body);
                void action() {
                    if (body == null) {
                        onCyclePassed -= action;
                        return;
                    }
                    foreach (GenericSkill skill in body.skillLocator.allSkills) {
                        skill.UnsetSkillOverride(this, unavailableSkill, GenericSkill.SkillOverridePriority.Replacement);
                    }

                    SetUnavailableSkill(body);
                }
                onCyclePassed += action;
            }
        }


        private void SetUnavailableSkill(CharacterBody body) {
            for (int i = 0; i < skillsToLock.Value; i++) {
                switch (skillIndexes[i]) {
                    case 0:
                        SetSkillOverride(body.skillLocator.primary);
                        break;
                    case 1:
                        SetSkillOverride(body.skillLocator.secondary);
                        break;
                    case 2:
                        SetSkillOverride(body.skillLocator.utility);
                        break;
                    case 3:
                        SetSkillOverride(body.skillLocator.special);
                        break;
                }
            }
        }

        private void SetSkillOverride(GenericSkill skill) {
            skill.SetSkillOverride(this, unavailableSkill, GenericSkill.SkillOverridePriority.Replacement);
        }
    }
}

