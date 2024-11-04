using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KW {
    public class PlayerManager : CharacterManager
    {
        [HideInInspector] public PlayerLocomotionManager playerLocomotionManager;
        [HideInInspector] public PlayerAnimatorManager playerAnimatorManager;
        [HideInInspector] public PlayerNetworkManager playerNetworkManager;
        [HideInInspector] public PlayerInventoryManager playerInventoryManager;

        protected override void Awake() {
            base.Awake();
            playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
            playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
            playerNetworkManager = GetComponent<PlayerNetworkManager>();
            playerInventoryManager = GetComponent<PlayerInventoryManager>();

        }

        protected override void Update() {
            base.Update();

            //can only control player if owner 
            if (!IsOwner) {
                return;
            }

            playerLocomotionManager.HandleAllMovement();
        }
        protected override void LateUpdate() {
            if (!IsOwner) {
                return;
            }
            base.LateUpdate();
            PlayerCamera.instance.HandleAllCameraActions();
        }
        public override void OnNetworkSpawn() {
            base.OnNetworkSpawn();

            if (IsOwner) {
                PlayerCamera.instance.player = this;
                PlayerInputManager.instance.player = this;
            }

        }
    }
}
