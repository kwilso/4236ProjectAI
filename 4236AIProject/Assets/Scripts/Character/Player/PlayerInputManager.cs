using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KW {
    public class PlayerInputManager : MonoBehaviour
    {
        PlayerControls playerControls;
        public static PlayerInputManager instance;
        public PlayerManager player;

        [Header("Camera Input")]
        [SerializeField] Vector2 cameraInput;
        public float cameraVerticalInput;
        public float cameraHorizontalInput;

        [Header("Movement Input")]
        [SerializeField] Vector2 movementInput;
        public float verticalInput;
        public float horizontalInput;

        [Header("Player Action Input")]
        [SerializeField] bool dodgeInput = false;
        [SerializeField] bool sprintInput = false;
        [SerializeField] bool jumpInput = false;
        [SerializeField] bool rightHandHitInput = false;


        public float moveAmount;


        private void Awake() {
            if (instance == null) {
                instance = this;
            } else if (instance != this) {
                Destroy(gameObject); 
            }
        }
        private void Start() {
            DontDestroyOnLoad(gameObject);
            // when scene changes, run this function
            SceneManager.activeSceneChanged += OnSceneChange;
            instance.enabled = false;
        }

        private void OnSceneChange(Scene oldScene, Scene newScene) {
            if (newScene.buildIndex == SaveGameManager.instance.worldSceneIndex) {
                instance.enabled = true;
            } else {
                instance.enabled = false;
            }

            rightHandHitInput = false;

        }
        
        private void OnEnable() {
            if (playerControls == null) {
                playerControls = new PlayerControls();

                playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
                playerControls.PlayerCamera.CameraControls.performed += i => cameraInput = i.ReadValue<Vector2>();
                
                // dodge
                playerControls.PlayerActions.Dodge.performed += i => dodgeInput = true;

                // holding key sets bool to true, releasing to false
                playerControls.PlayerActions.Sprint.performed += i => sprintInput = true;
                playerControls.PlayerActions.Sprint.canceled += i => sprintInput = false;

                // swinging sword test
                playerControls.PlayerActions.RightHandSwingWeapon.performed += instance => rightHandHitInput = true;

                // jumping
                playerControls.PlayerActions.Jump.performed += i => jumpInput = true;


            }

            playerControls.Enable();
        }
        
        private void OnDestroy() {
            // if we destroy this object, we unsubscribe from this event 
            SceneManager.activeSceneChanged -= OnSceneChange;
        }

        // if window minimized, can't move
        private void OnApplicationFocus(bool focus) {
            if (enabled) {
                if (focus) {
                    playerControls.Enable();
                } else {
                    playerControls.Disable();
                }
            }
        }

        private void Update() {
            HandleAllInputs();
        }
        private void HandleAllInputs() {
            HandleMovementInput();
            HandleCameraMovementInput();
            HandleDodgeInput();
            HandleSprinting();
            HandleRightHandSwing();
            HandleJumping();
        }
        private void HandleMovementInput() {
            verticalInput = movementInput.y;
            horizontalInput = movementInput.x;

            moveAmount = Mathf.Clamp01(Mathf.Abs(verticalInput) + Mathf.Abs(horizontalInput));

            // clamping values of moveAmount so it's always = to either 0, 0.5, or 1
            if (moveAmount <= 0.5f && movementInput.magnitude > 0) {
                moveAmount = 0.5f;
            } else if (moveAmount > 0.5f && moveAmount <= 1f) {
                moveAmount = 1;
            }
            
            if (player == null) {
                return;
            }
            // passing 0 because only want non-strafing movement, horizontal is for locked on movement
            player.playerAnimatorManager.UpdateAnimatorMovementParameters(0, moveAmount, player.playerNetworkManager.isSprinting.Value);

        }
        private void HandleCameraMovementInput() {
            cameraVerticalInput = cameraInput.y;
            cameraHorizontalInput = cameraInput.x;
        }
        private void HandleDodgeInput() {
            if (dodgeInput) {
                dodgeInput = false;
                
                player.playerLocomotionManager.AttemptDodge();

            }
        }
        private void HandleSprinting() {
            if (sprintInput) {
                player.playerLocomotionManager.HandleSprinting();
            } else {
                player.playerNetworkManager.isSprinting.Value = false;
            }
        }

        private void HandleRightHandSwing() {
            if (rightHandHitInput) {
                rightHandHitInput = false;  
                player.playerLocomotionManager.TestRightHandSwing();
                player.playerNetworkManager.isSwingingWeapon.Value = true;
            } else {
                player.playerNetworkManager.isSwingingWeapon.Value = false;
            }
        }
        private void HandleJumping() {
            if (jumpInput) {
                jumpInput = false;
                player.playerLocomotionManager.HandleJumping();
                player.playerNetworkManager.isJumping.Value = true;
            } else {
                player.playerNetworkManager.isJumping.Value = false;
            }
        }
    }
}
