using System.Collections.Generic;
using UnityEngine;

namespace Mirror.Discovery
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Network/NetworkDiscoveryHUD")]
    [HelpURL("https://mirror-networking.com/docs/Articles/Components/NetworkDiscovery.html")]
    [RequireComponent(typeof(NetworkDiscovery))]
    public class DiscoveryHUD : MonoBehaviour
    {
        readonly Dictionary<long, ServerResponse> discoveredServers = new Dictionary<long, ServerResponse>();
        Vector2 scrollViewPos = Vector2.zero;

        public NetworkDiscovery networkDiscovery;
        
        [SerializeField]
        GameObject buttonTemplate;
        
        public string playerName { get; set; } = "Player".PadRight(26);
        private void StorePlayerName()
        {
            PlayerPrefs.SetString("playerName", playerName.Trim(' '));
        }

        /*static public void SetPlayerName(string newName)
        {
            Debug.Log(newName);
            //playerName = newName;
        }*/

        private void Awake()
        {
            buttonTemplate?.SetActive(false);
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            if (networkDiscovery == null)
            {
                networkDiscovery = GetComponent<NetworkDiscovery>();
                UnityEditor.Events.UnityEventTools.AddPersistentListener(networkDiscovery.OnServerFound, OnDiscoveredServer);
                UnityEditor.Undo.RecordObjects(new Object[] { this, networkDiscovery }, "Set NetworkDiscovery");
            }
        }
#endif

        void OnGUI()
        {
            if (NetworkManager.singleton == null)
                return;

            if (NetworkServer.active || NetworkClient.active)
                return;

            if (!NetworkClient.isConnected && !NetworkServer.active && !NetworkClient.active)
                DrawGUI();
        }

        void DrawGUI()
        {
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Find Servers"))
            {
                FindServers();
            }

            // LAN Host
            if (GUILayout.Button("Start Host"))
            {
                HostStart();
            }

            // Dedicated server
            if (GUILayout.Button("Start Server"))
            {
                DedicatedServer();
            }

            GUILayout.Label("Nome do Jogador: ");

            //playerName = GUILayout.TextField(playerName,32);

            GUILayout.EndHorizontal();

            // show list of found server

            GUILayout.Label($"Discovered Servers [{discoveredServers.Count}]:");

            // servers
            scrollViewPos = GUILayout.BeginScrollView(scrollViewPos);

            foreach (ServerResponse info in discoveredServers.Values)
                if (GUILayout.Button(info.EndPoint.Address.ToString()))
                    Connect(info);

            GUILayout.EndScrollView();
        }

        public void FindServers()
        {
            discoveredServers.Clear();
            networkDiscovery.StartDiscovery();
        }

        public void DedicatedServer()
        {
            discoveredServers.Clear();
            NetworkManager.singleton.StartServer();

            networkDiscovery.AdvertiseServer();
            StorePlayerName();
        }

        public void HostStart()
        {
            discoveredServers.Clear();
            NetworkManager.singleton.StartHost();
            networkDiscovery.AdvertiseServer();
            StorePlayerName();
        }

        void Connect(ServerResponse info)
        {
            StorePlayerName();
            NetworkManager.singleton.StartClient(info.uri);
        }

        public void OnDiscoveredServer(ServerResponse info)
        {
            /*if (discoveredServers.ContainsKey(info.serverId))
                return;*/
            // Note that you can check the versioning to decide if you can connect to the server or not using this method
            discoveredServers[info.serverId] = info;

            GameObject go = Instantiate(buttonTemplate) as GameObject;

            go.SetActive(true);
            UnityEngine.UI.Button button = go.GetComponent<UnityEngine.UI.Button>();
            button.onClick.AddListener(delegate { Connect(info); });
            TMPro.TextMeshProUGUI text = go.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            text.SetText(info.EndPoint.Address.ToString());
            // TB.SetName(str);
            go.transform.SetParent(buttonTemplate.transform.parent);
        }
    }
}
