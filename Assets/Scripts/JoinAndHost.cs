using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class JoinAndHost : NetworkBehaviour
{
    [SerializeField]private GameObject PlayerPrefab;
    [SerializeField]private GameObject PlayerControlPrefab;
    [SerializeField]private Transform PlayerParent;
    [SerializeField]private Transform PlayerControlParent;

    [SerializeField]private Button HostButton;
    [SerializeField]private Button ClientButton;

    public void StartHost(){
        NetworkManager.Singleton.StartHost();

        Player player = CreatePlayer();
        CreatePlayerControl(player);

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
        CreatePlayerServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    public void CreatePlayerServerRpc()
    {
        Debug.Log("让我看看你在哪执行");
        // 在服务器上生成玩家对象
        GameObject player = Instantiate(PlayerPrefab);
        NetworkObject networkObject = player.GetComponent<NetworkObject>();
        networkObject.Spawn();

        player.transform.SetParent(PlayerParent, false);
    }

    [ClientRpc]
    private void CreatePlayerClientRpc(ulong networkObjectId)
    {
        
    }

    private Player CreatePlayer(){
        GameObject player = Instantiate(PlayerPrefab);
        NetworkObject networkObject = player.GetComponent<NetworkObject>();
        networkObject.Spawn();

        player.transform.SetParent(PlayerParent, false);

        return player.GetComponent<Player>();
    }

    private void CreatePlayerControl(Player player){
        GameObject playerControl_GO = Instantiate(PlayerControlPrefab);

        playerControl_GO.transform.SetParent(PlayerControlParent, false);

        PlayerControl playerControl = playerControl_GO.GetComponent<PlayerControl>();
        playerControl.BindPlayer(player);

        playerControl.Hide();
    }

}