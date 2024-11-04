using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KW {
    public class PlayerInventoryManager : CharacterInventoryManager
    {
        protected override void Awake() {
            base.Awake();
        }
        
        public WeaponItem currentRightHandWeapon;
        public WeaponItem currentLeftHandWeapon;
    }
}
