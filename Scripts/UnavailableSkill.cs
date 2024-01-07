using RoR2;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ReturnsArtifacts.Scripts {
    internal class UnavailableSkill : SkillDef {

        public string tokenName => "DISTORTION_UNAVAILABLE";
        public override bool CanExecute(GenericSkill skillSlot) {
            return false;
        }

        public override bool IsReady(GenericSkill skillSlot) {
            return false;
        }

        
    }
}
