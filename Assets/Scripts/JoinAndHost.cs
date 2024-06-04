using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class JoinAndHost : NetworkBehaviour
{
    #region Prefab
    [SerializeField]private GameObject PlayerPrefab;
    [SerializeField]private GameObject PlayerControlPrefab;
    [SerializeField]private GameObject PublicCardsPrefab;
    #endregion
    [SerializeField]private Transform PlayerParent;
    [SerializeField]private Transform playCanvas;

    [SerializeField]private Button HostButton;
    [SerializeField]private Button ClientButton;

    public void StartHost(){
        //
        NetworkManager.Singleton.StartHost();
        //初始化Player
        Player player = CreatePlayer(NetworkManager.Singleton.LocalClientId);
        
        HostButton.gameObject.SetActive(false);
        ClientButton.gameObject.SetActive(false);
    }
    public void StartClient(){
        NetworkManager.Singleton.StartClient();
        StartCoroutine(WaitingConnection());

        HostButton.gameObject.SetActive(false);
        ClientButton.gameObject.SetActive(false);
    }

    private IEnumerator WaitingConnection(){
        while(!NetworkManager.Singleton.IsConnectedClient){
            yield return new WaitForSeconds(0.1f);
        }
        Debug.Log(NetworkManager.Singleton.LocalClientId);
        CreatePlayerServerRpc(NetworkManager.Singleton.LocalClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    public void CreatePlayerServerRpc(ulong clientID)
    {
        Player player = CreatePlayer(clientID);
    }

    [ClientRpc]
    private void CreatePlayerClientRpc(ulong networkObjectId)
    {
        
    }

    private Player CreatePlayer(ulong clientID){
        //玩家初始化
        GameObject player = Instantiate(PlayerPrefab);
        NetworkObject networkObject = player.GetComponent<NetworkObject>();
        networkObject.Spawn();

        player.transform.SetParent(PlayerParent, false);

        player.GetComponent<Player>().PlayerID.Value = clientID;
        return player.GetComponent<Player>();
    }
}