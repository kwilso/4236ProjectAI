using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KW {
    public class PlayerLocomotionManager : CharacterLocomotionManager
    {
        PlayerManager player;
        [HideInInspector] public float verticalMovement;
        [HideInInspector] public float horizontalMovement;
        [HideInInspector] public float moveAmount;

        [Header("Movement Settings")]
        private Vector3 moveDirection;
        private Vector3 targetRotationDirection;
        [SerializeField] float walkingSpeed = 2;
        [SerializeField] float runningSpeed = 5;
        [SerializeField] float sprintingSpeed = 8;
        [SerializeField] float rotationSpeed = 5;

        [Header("Dodge")]
        private Vector3 rollDirection;

        protected override void Awake() {
            base.Awake();

            player = GetComponent<PlayerManager>();
        }

        protected override void Update() {
            base.Update();

            if (player.IsOwner) {
                player.characterNetworkManager.verticalMovement.Value = verticalMovement;
                player.characterNetworkManager.horizontalMovement.Value = horizontalMovement;
                player.characterNetworkManager.networkMoveAmount.Value = moveAmount;
            } else {
                verticalMovement = player.characterNetworkManager.verticalMovement.Value;
                horizontalMovement = player.characterNetworkManager.horizontalMovement.Value;
                moveAmount = player.characterNetworkManager.networkMoveAmount.Value;

                // if not locked on, pass move amount
                player.playerAnimatorManager.UpdateAnimatorMovementParameters(0, moveAmount, player.playerNetworkManager.isSprinting.Value);

                // if locked on, pass vertical and horizontal
            }
        } 
        public void HandleAllMovement() {
            HandleGroundedMovement();
            HandleRotation();
        }

        private void GetMovementValues() {
            verticalMovement = PlayerInputManager.instance.verticalInput;
            horizontalMovement = PlayerInputManager.instance.horizontalInput;
            moveAmount = PlayerInputManager.instance.moveAmount;

            // clamp the movements!
        }
        private void HandleGroundedMovement() {
            if (!player.canMove) {
                return;
            }

            GetMovementValues();
            // move direction is based on camera's facing perspective and our inputs
            moveDirection = PlayerCamera.instance.transform.forward * verticalMovement;
            moveDirection = moveDirection + PlayerCamera.instance.transform.right * horizontalMovement;
            moveDirection.Normalize();
            moveDirection.y = 0;

            if (player.playerNetworkManager.isSprinting.Value) {
                player.characterController.Move(moveDirection * sprintingSpeed * Time.deltaTime);
            } else {
                if (PlayerInputManager.instance.moveAmount > 0.5f) {
                    // move at running speed
                    player.characterController.Move(moveDirection * runningSpeed * Time.deltaTime);
                } else if (PlayerInputManager.instance.moveAmount <= 0.5f) {
                    // move at walking speed
                    player.characterController.Move(moveDirection * walkingSpeed * Time.deltaTime);
                }
            }


        }

        // rotate player with camera movement
        private void HandleRotation() {
            if (!player.canRotate) {
                return;
            }
            targetRotationDirection = Vector3.zero;
            targetRotationDirection = PlayerCamera.instance.cameraObject.transform.forward * verticalMovement;
            targetRotationDirection = targetRotationDirection + PlayerCamera.instance.cameraObject.transform.right * horizontalMovement;
            targetRotationDirection.Normalize();
            targetRotationDirection.y = 0;

            if (targetRotationDirection == Vector3.zero) {
                targetRotationDirection = transform.forward;
            }

            Quaternion newRotation = Quaternion.LookRotation(targetRotationDirection);
            Quaternion targetRotation = Quaternion.Slerp(transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
            transform.rotation = targetRotation;
        }

        public void AttemptDodge() {
            if (player.isPerformingAction) {
                return;
            }
            // if moving, roll
            if (moveAmount > 0) {
                rollDirection = PlayerCamera.instance.cameraObject.transform.forward * verticalMovement;
                rollDirection += PlayerCamera.instance.cameraObject.transform.right * horizontalMovement;

                rollDirection.y = 0;
                rollDirection.Normalize();
                Quaternion playerRotation = Quaternion.LookRotation(rollDirection);
                player.transform.rotation = playerRotation;

                player.playerAnimatorManager.PlayTargetActionAnimation("Roll_Forward_01", true);

            // if stationary, backstep
            } else {
                player.playerAnimatorManager.PlayTargetActionAnimation("Back_Step_01", true);
            }
        }
        public void HandleSprinting() {
            if (player.isPerformingAction) {
                player.playerNetworkManager.isSprinting.Value = false;
            }

            if (moveAmount >= 0.5) {
                player.playerNetworkManager.isSprinting.Value = true;
            } else {
                player.playerNetworkManager.isSprinting.Value = false;
            }
        }
        
        public void TestRightHandSwing() {
            if (player.isPerformingAction) {
                player.playerNetworkManager.isSwingingWeapon.Value = false;
            }
            if (Input.GetMouseButtonDown(0)) {
                player.playerNetworkManager.isSwingingWeapon.Value = true;
            }
            player.playerAnimatorManager.PlayTargetActionAnimation("Right_Hand_Swing_01", true);
        }

        public void HandleJumping() {
            if (player.isPerformingAction) {
                player.playerNetworkManager.isJumping.Value = false;
            }
            player.playerAnimatorManager.PlayTargetActionAnimation("Jump_01", true);
        }
    }
}
