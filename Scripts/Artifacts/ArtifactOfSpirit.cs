using BepInEx.Configuration;
using UnityEngine;
using UnityEngine.Networking;
using RoR2;
using static ReturnsArtifacts.Scripts.Plugin;
using R2API;
using System;
using HarmonyLib;

namespace ReturnsArtifacts.Scripts.Artifacts {
    class ArtifactOfSpirit : ArtifactBase {
        public override string ArtifactName => "Artifact of Spirit";
        public override string ArtifactLangTokenName => "ARTIFACT_OF_SPIRIT";
        public override string ArtifactDescription => "Characters run faster at lower health.";
        public override Sprite ArtifactEnabledIcon => Assets.LoadAsset<Sprite>("ArtifactOfSpiritEnabled.png");
        public override Sprite ArtifactDisabledIcon => Assets.LoadAsset<Sprite>("ArtifactOfSpiritDisabled.png");

        private static float minPercentageChange = 0.05f;

        private float healthSegments = 1 / minPercentageChange;

        public override void Init() {
            CreateLang();
            CreateArtifact();
            Hooks();
        }

        private int getSegment(float health) {
            return (int)(health * healthSegments);
        }

        public override void Hooks() {
            LogDebug("Adding hooks");
            On.RoR2.HealthComponent.Heal += OnHeal;
            On.RoR2.HealthComponent.TakeDamage += OnDamage;

            RecalculateStatsAPI.GetStatCoefficients += RecalculateSpeed;

            LogDebug("hooks added");
        }

        private void OnDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo) {

            float oldHealth = self.combinedHealthFraction;

            orig(self, damageInfo);

            float newHealth = self.combinedHealthFraction;

            CheckHealthChanged(self, oldHealth, newHealth);
        }

        private float OnHeal(On.RoR2.HealthComponent.orig_Heal orig, HealthComponent self, float amount, ProcChainMask procChainMask, bool nonRegen) {

            float oldHealth = self.combinedHealthFraction;

            float result = orig(self, amount, procChainMask, nonRegen);

            float newHealth = self.combinedHealthFraction;

            CheckHealthChanged(self, oldHealth, newHealth);

            return result;
        }

        private void CheckHealthChanged(HealthComponent health, float oldHealth, float newHealth) {
            if (!NetworkClient.active || !ArtifactEnabled)
                return;

            // check if the health changes healthSegment
            // e.g. if minPercentageChange is 0.05f and
            // the health goes from 0.82 (segment 0.80-0.85)
            // to 0.81 (segment 0.80-0.85) then there's no need
            // to recalculate stats
            // if the health goes from 0.82 to 0.79
            // (segment 0.75-0.80) then we need to recalculate
            // stats
            if (getSegment(oldHealth) != getSegment(newHealth)) {
                health.body.MarkAllStatsDirty();
            }
        }

        private void RecalculateSpeed(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args) {

            // Approximates the health to a health segment's start
            float health = getSegment(sender.healthComponent.combinedHealthFraction)/healthSegments;

            // Calculates the speed through the formula (1-x)^2
            // where x is the health's percentage
            float bonusSpeed = (float)Math.Pow(1f - health, 2);

            LogDebug($"{sender}:\nhealth: {sender.healthComponent.combinedHealth}\nmax health: {sender.healthComponent.fullCombinedHealth}\nbonus speed: {bonusSpeed}");

            args.moveSpeedMultAdd += bonusSpeed;
            
        }

    }
}