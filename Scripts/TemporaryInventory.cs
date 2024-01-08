using System;
using System.Collections.Generic;
using UnityEngine;
using RoR2;
using System.Linq;
using RoR2.UI;
using UnityEngine.UI;

namespace ReturnsArtifacts.Scripts {
    internal class TemporaryInventory {
        private static readonly float startingRemainingTime = 10f;
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
                        SetItemTime(itemIndex, startingRemainingTime);
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
                temporaryItemIndicator.fillAmount = newRemainingTime / startingRemainingTime;
            }
        }

        public void AddItem(ItemIndex itemIndex) {
            if (!itemsRemainingTime.ContainsKey(itemIndex))
                itemsRemainingTime.Add(
                    itemIndex,
                    startingRemainingTime
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
