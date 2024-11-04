using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace KW {
    public class TitleScreenManager : MonoBehaviour
    {

        public void StartNetworkAsHost()
        {
            NetworkManager.Singleton.StartHost();
        }
        public void StartNewGame() {
            StartCoroutine(SaveGameManager.instance.LoadNewGame());
        }
    }
}