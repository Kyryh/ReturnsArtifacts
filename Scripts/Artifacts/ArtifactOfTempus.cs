using BepInEx.Configuration;
using UnityEngine;
using RoR2;
using static ReturnsArtifacts.Scripts.Plugin;
using System.Collections.Generic;
using UnityEngine.UI;
using MonoMod.Cil;
using System.Reflection;
using RoR2.Artifacts;


namespace ReturnsArtifacts.Scripts.Artifacts {
    class ArtifactOfTempus : ArtifactBase {
        public override string ArtifactName => "Artifact of Tempus";
        public override string ArtifactLangTokenName => "ARTIFACT_OF_TEMPUS";
        public override string ArtifactDescription => "Items are worth multiple stacks. All items are temporary.";
        public override Sprite ArtifactEnabledIcon => Assets.LoadAsset<Sprite>("ArtifactOfTempusEnabled.png");
        public override Sprite ArtifactDisabledIcon => Assets.LoadAsset<Sprite>("ArtifactOfTempusDisabled.png");

        public static GameObject temporaryItemUI = Assets.LoadAsset<GameObject>("TemporaryItemIndicatorUI.prefab");

        private static TemporaryInventory playerInventory;

        private static bool giveMoreStacks = true;

        public static readonly float itemLifespan = 120f;

        private static readonly int baseNumStacks = 3;

        private static readonly int bonusNumStacks = 1;

        private static readonly int stagesPerBonusStacks = 2;

        private static int totalStacks => baseNumStacks + bonusNumStacks * (Run.instance.stageClearCount / stagesPerBonusStacks);
        public override void Init() {
            CreateLang();
            CreateArtifact();
            Hooks();
        }

        public override void Hooks() {
            Inventory.onServerItemGiven += Inventory_onServerItemGiven;
            CharacterBody.onBodyStartGlobal += CharacterBody_onBodyStartGlobal;
            SceneDirector.onGenerateInteractableCardSelection += (sceneDirector, dccs) => {
                if (ArtifactEnabled)
                    CommandArtifactManager.OnGenerateInteractableCardSelection(sceneDirector, dccs);
            };
            On.RoR2.PurchaseInteraction.CanBeAffordedByInteractor += PurchaseInteraction_CanBeAffordedByInteractor;
            On.RoR2.UI.ItemIcon.Awake += (orig, self) => {
                orig(self);
                GameObject temporaryItemIndicator = GameObject.Instantiate(temporaryItemUI, self.rectTransform, false);
            };
            On.RoR2.UI.ItemInventoryDisplay.UpdateDisplay += (orig, self) => {
                playerInventory.ResetIndicators();
                orig(self);
            };
            On.RoR2.UI.ItemIcon.SetItemIndex += (orig, self, newItemIndex, newItemCount) => {
                orig(self, newItemIndex, newItemCount);
                Transform temporaryItemIndicatorUI = self.transform.Find("TemporaryItemIndicatorUI(Clone)");
                if (ItemCatalog.GetItemDef(newItemIndex).tier == ItemTier.NoTier) {
                    temporaryItemIndicatorUI.gameObject.SetActive(false);
                } else {
                    temporaryItemIndicatorUI.gameObject.SetActive(true);
                    playerInventory.AddTemporaryItemIndicator(newItemIndex, temporaryItemIndicatorUI.GetComponent<Image>());
                }
            };
        }


        private bool PurchaseInteraction_CanBeAffordedByInteractor(On.RoR2.PurchaseInteraction.orig_CanBeAffordedByInteractor orig, PurchaseInteraction self, Interactor activator) {
            // don't want to deal with regenerating scrap shenanigans
            // so fuck you printers and cauldrons are disabled
            switch (self.costType) {
                case CostTypeIndex.WhiteItem:
                case CostTypeIndex.GreenItem:
                case CostTypeIndex.RedItem:
                case CostTypeIndex.BossItem:
                    return false;
            }
            return orig(self, activator);
        }

        private void CharacterBody_onBodyStartGlobal(CharacterBody body) {
            if (body.isPlayerControlled && playerInventory?.inventory != body.inventory) {
                playerInventory = new TemporaryInventory(body.inventory);
            }
        }


        private void Inventory_onServerItemGiven(Inventory inventory, ItemIndex index, int count) {
            if (inventory != playerInventory?.inventory)
                return;
            if (giveMoreStacks) {
                giveMoreStacks = false;
                ItemDef itemDef = ItemCatalog.GetItemDef(index);
                int bonusItems = (totalStacks - 1) * count;
                if (itemDef.tier != ItemTier.NoTier) {
                    
                    inventory.GiveItem(index, bonusItems);
                    playerInventory.AddItem(index);
                }
            } else {
                giveMoreStacks = true;
            }
        }

        
    }

    
}