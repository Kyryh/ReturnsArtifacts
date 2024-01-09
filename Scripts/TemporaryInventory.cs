using System;
using System.Collections.Generic;
using UnityEngine;
using RoR2;
using System.Linq;
using UnityEngine.UI;
using static ReturnsArtifacts.Scripts.Artifacts.ArtifactOfTempus;

namespace ReturnsArtifacts.Scripts {
    internal class TemporaryInventory {
        public Inventory inventory { get; private set; }
        private readonly Dictionary<ItemIndex, float> itemsRemainingTime = new Dictionary<ItemIndex, float>();
        private readonly Dictionary<ItemIndex, Image> temporaryItemIndicators = new Dictionary<ItemIndex, Image>();
        public TemporaryInventory(Inventory inventory) {
            this.inventory = inventory;
            Plugin.onGameTimeChanged += OnGameTimeChanged;
        }

        private void OnGameTimeChanged(float time) {
            foreach (ItemIndex itemIndex in itemsRemainingTime.Keys.ToArray()) {
                float newRemainingTime = itemsRemainingTime[itemIndex] - Time.fixedDeltaTime;
                if (newRemainingTime > 0) {
                    SetItemTime(itemIndex, newRemainingTime);
                } else {
                    inventory.RemoveItem(itemIndex);
                    if (inventory.GetItemCount(itemIndex) > 0 ) {
                        SetItemTime(itemIndex, itemLifespan);
                    } else {
                        itemsRemainingTime.Remove(itemIndex);
                        temporaryItemIndicators.Remove(itemIndex);
                    }
                }
            }
        }

        private void SetItemTime(ItemIndex itemIndex, float newRemainingTime) {
            itemsRemainingTime[itemIndex] = newRemainingTime;
            temporaryItemIndicators.TryGetValue(itemIndex, out Image temporaryItemIndicator);
            if (temporaryItemIndicator != null ) {
                temporaryItemIndicator.fillAmount = newRemainingTime / itemLifespan;
            }
        }

        public void AddItem(ItemIndex itemIndex) {
            if (!itemsRemainingTime.ContainsKey(itemIndex))
                itemsRemainingTime.Add(
                    itemIndex,
                    itemLifespan
                );
        }

        public void AddTemporaryItemIndicator(ItemIndex itemIndex, Image temporaryItemIndicator) {
            if (!temporaryItemIndicators.ContainsKey(itemIndex))
                temporaryItemIndicators.Add(
                    itemIndex,
                    temporaryItemIndicator
                );
        }

        public void ResetIndicators() {
            temporaryItemIndicators.Clear();
        }
    }
}
