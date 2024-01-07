﻿using BepInEx.Configuration;
using UnityEngine;
using UnityEngine.Networking;
using RoR2;
using RoR2.Skills;
using static ReturnsArtifacts.Scripts.Plugin;
using UnityEngine.AddressableAssets;
using R2API;
using System;
using System.Collections.Generic;
using System.Collections;
using Random = System.Random;

namespace ReturnsArtifacts.Scripts.Artifacts {
    class ArtifactOfDistortion : ArtifactBase {
        public override string ArtifactName => "Artifact of Distortion";
        public override string ArtifactLangTokenName => "ARTIFACT_OF_DISTORTION";
        public override string ArtifactDescription => "Lock a random skill every minute, but skills have decreased cooldowns.";
        public override Sprite ArtifactEnabledIcon => Assets.LoadAsset<Sprite>("ArtifactOfDistortionEnabled.png");
        public override Sprite ArtifactDisabledIcon => Assets.LoadAsset<Sprite>("ArtifactOfDistortionDisabled.png");

        public static readonly float cycleDuration = 60f;

        public static readonly float cooldownReduction = 0.25f;

        public static readonly int numOfSkillsToLock = 1;

        private static UnavailableSkill unavailableSkill;

        private static int[] skillIndexes = [0, 1, 2, 3];


        public override void Init() {
            
            CreateLang();
            CreateArtifact();
            unavailableSkill = InitSkill();
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
                Plugin.RemoveOnCyclePassedListeners();
                skillIndexes = skillIndexes.Shuffled();
                Plugin.onCyclePassed += () => { skillIndexes = skillIndexes.Shuffled(); };
            };
            CharacterBody.onBodyStartGlobal += CharacterBody_onBodyStartGlobal;
            R2API.RecalculateStatsAPI.GetStatCoefficients += ReduceCooldown;
            
        }

        private void ReduceCooldown(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args) {
            if (ArtifactEnabled)
                args.cooldownMultAdd -= cooldownReduction;
        }

        private void CharacterBody_onBodyStartGlobal(CharacterBody body) {
            if (ArtifactEnabled && body.isPlayerControlled) {
                SetUnavailableSkill(body);
                Plugin.onCyclePassed += () => {
                    foreach (GenericSkill skill in body.skillLocator.allSkills) {
                        skill.UnsetSkillOverride(this, unavailableSkill, GenericSkill.SkillOverridePriority.Replacement);
                    }
                    
                    SetUnavailableSkill(body);
                } ;
            }
        }


        private void SetUnavailableSkill(CharacterBody body) {
            if (body == null)
                return;
            for (int i = 0; i < numOfSkillsToLock; i++) {
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

