using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KW {
    public class WeaponItem : Item 
    {
        // animator controller override (change attack anim based on weapon using)
        
        [Header("Weapon Model")]
        [SerializeField] public GameObject weaponModel;

        [Header("Weapon Damage")]
        [SerializeField] public int physicalDamage;
        // can implement other types of magic damage?

        // [Header("Stamina Costs")]
        // public int baseStaminaCost;
        // implement when implementing health and stamina
    }
}
