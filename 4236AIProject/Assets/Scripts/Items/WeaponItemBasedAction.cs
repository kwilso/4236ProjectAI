using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KW {
    public class WeaponItemBasedAction : ScriptableObject
    {
        public int actionID;

        public virtual void AttemptToPerformAction(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction) {
            
        }
    }
}
