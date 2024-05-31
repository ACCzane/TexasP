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
    [SerializeField]private Button StartButton;
    [SerializeField]private NameRegister nameRegister;

    private Player player = null;

    public void StartHost(){
        NetworkManager.Singleton.StartHost();

        player = CreatePlayer();
        CreatePlayerControl(player);

        HostButton.gameObject.SetActive(false);
        ClientButton.gameObject.SetActive(false);
        StartButton.gameObject.SetActive(true);
    
        StartCoroutine(UsrRegisterName(nameRegister));
    }
    public void StartClient(){
        NetworkManager.Singleton.StartClient();
        StartCoroutine(WaitingConnection());

        HostButton.gameObject.SetActive(false);
        ClientButton.gameObject.SetActive(false);

        StartCoroutine(UsrRegisterName(nameRegister));
    }

    private IEnumerator WaitingConnection(){
        while(!NetworkManager.Singleton.IsConnectedClient){
            yield return new WaitForSeconds(0.1f);
        }
        CreatePlayerServerRpc();
    }

    private IEnumerator UsrRegisterName(NameRegister nameRegister){
        nameRegister.ShowNameRegisterPanel();
        while(nameRegister.gameObject.activeSelf){
            yield return null;
        }
        ChangePlayerNameServerRpc(player, nameRegister.playerName);
    }

    [ServerRpc(RequireOwnership = false)]
    public void CreatePlayerServerRpc()
    {
        // 在服务器上生成玩家对象
        GameObject playerGO = Instantiate(PlayerPrefab);
        NetworkObject networkObject = playerGO.GetComponent<NetworkObject>();
        networkObject.Spawn();

        playerGO.transform.SetParent(PlayerParent, false);

        player = playerGO.GetComponent<Player>();
    }

    [ServerRpc]
    public void ChangePlayerNameServerRpc(Player player, string name){
        player.ChangeName(name);
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