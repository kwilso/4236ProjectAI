using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KW {
    public class PlayerCamera : MonoBehaviour
    {
        public static PlayerCamera instance;
        public PlayerManager player; 
        public Camera cameraObject;
        [SerializeField] Transform cameraPivotTransform;
        

        [Header("Camera Settings")]
        private float cameraSmoothSpeed = 1;
        [SerializeField] float upAndDownRotationSpeed = 220;
        [SerializeField] float leftAndRightRotationSpeed = 220;
        [SerializeField] float minimumPivot = -30; // lowest point able to look down
        [SerializeField] float maximumPivot = 60; // highest point able to look up
        [SerializeField] float cameraCollisionRadius = 0.2f;
        [SerializeField] LayerMask collideWithLayers;

        [Header("Camera Values")]
        private Vector3 cameraVelocity;
        private Vector3 cameraObjectPosition;
        [SerializeField] float leftAndRightLookAngle;
        [SerializeField] float upAndDownLookAngle;
        private float cameraZPosition;
        private float targetCameraZPosition;

        private void Start() {
            DontDestroyOnLoad(gameObject);
            cameraZPosition = cameraObject.transform.localPosition.z;
        }
        private void Awake() {
            if (instance == null) {
                instance = this;
            } else {
                Destroy(gameObject);
            }
        }
        public void HandleAllCameraActions() {
            FollowPlayer();
            HandleRotations();
            HandleCollisions();
        }
        private void FollowPlayer() {
            Vector3 targetCameraZPosition = Vector3.SmoothDamp(transform.position, player.transform.position, ref cameraVelocity, cameraSmoothSpeed * Time.deltaTime);
            transform.position = targetCameraZPosition;
        }
        private void HandleRotations() {
            // rotate L&R based on horizontal mouse movements
            leftAndRightLookAngle += (PlayerInputManager.instance.cameraHorizontalInput * leftAndRightRotationSpeed) * Time.deltaTime;
            // rotate U&D based on vertical mouse movements
            upAndDownLookAngle -= (PlayerInputManager.instance.cameraVerticalInput * upAndDownRotationSpeed) * Time.deltaTime;
            // clamp up and down look angle between min and max value
            upAndDownLookAngle = Mathf.Clamp(upAndDownLookAngle, minimumPivot, maximumPivot);

            Vector3 cameraRotation = Vector3.zero;

             // rotates this game object left and right
            cameraRotation.y = leftAndRightLookAngle;
            Quaternion targetRotation = Quaternion.Euler(cameraRotation);
            transform.rotation = targetRotation;

            cameraRotation = Vector3.zero;
            cameraRotation.x = upAndDownLookAngle;
            targetRotation = Quaternion.Euler(cameraRotation);
            cameraPivotTransform.localRotation = targetRotation;
        }
        private void HandleCollisions() {
            targetCameraZPosition = cameraZPosition;
            RaycastHit hit;

            // direction for collision check
            Vector3 direction = cameraObject.transform.position - cameraPivotTransform.position;
            direction.Normalize();

            // check if there is an object in front of desired direction
            if (Physics.SphereCast(cameraPivotTransform.position, cameraCollisionRadius, direction, out hit, Mathf.Abs(targetCameraZPosition), collideWithLayers)) {
                // if there is, get distance from it
                float distanceFromHitObject = Vector3.Distance(cameraPivotTransform.position, hit.point);
                // then make our z position the following
                targetCameraZPosition = -(distanceFromHitObject - cameraCollisionRadius);
            } 
            
            // if target position is less than collision radius, we subtract our collision radius
            if (Mathf.Abs(targetCameraZPosition) < cameraCollisionRadius) {
                targetCameraZPosition = -cameraCollisionRadius;
            }

            // then apply final position using a lerp over time of 0.2f
            cameraObjectPosition.z = Mathf.Lerp(cameraObject.transform.localPosition.z, targetCameraZPosition, 0.2f);
            cameraObject.transform.localPosition = cameraObjectPosition;

        }
    }
}
