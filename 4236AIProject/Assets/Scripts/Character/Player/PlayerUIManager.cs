using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace KW {
    public class PlayerUIManager : MonoBehaviour
    {
        public static PlayerUIManager instance;
        [Header("NETWORK JOIN")]
        [SerializeField] bool startGameAsClient;

        private void Awake() {
            if (instance == null) {
                instance = this;
            } else {
                Destroy(gameObject);
            }
        }
        
        private void Start() {
            DontDestroyOnLoad(gameObject);
        }

        private void Update() {
            if (startGameAsClient) {
                startGameAsClient = false;
                // shut down as host
                NetworkManager.Singleton.Shutdown();
                // restart as client
                NetworkManager.Singleton.StartClient();
            }
        }
    }
}
