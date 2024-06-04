using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControl : NetworkBehaviour
{
    /// <summary>
    /// 联机游戏中的本地玩家
    /// </summary>
    public Player Player;
    [SerializeField]private PlayerBetAmount playerBetAmount;
    [SerializeField]private PlayerAction_Bet playerAction_Bet;
    [SerializeField]private GameObject checkPanel;
    private void Start(){
        playerAction_Bet.BetButton.onClick.AddListener(() => playerAction_Bet.OnBetButtonClick(this));
    }

    public void Show(){
        playerBetAmount.FirstShowAmount();
        gameObject.SetActive(true);
    }
    public void Show(bool canCheck){
        playerBetAmount.FirstShowAmount();
        gameObject.SetActive(true);
        if(!canCheck){
            checkPanel.SetActive(false);
        }else{
            checkPanel.SetActive(true);
        }
    }

    public void Hide(){
        gameObject.SetActive(false);
    }
    
    [ServerRpc]
    public void Player_CheckServerRpc(){
        Player.Decide(2,0f);
    }

    [ServerRpc]
    public void Player_FoldServerRpc(){
        Player.Decide(1,0f);
    }

    [ServerRpc]
    public void Player_BetServerRpc(float bet){
        Player.Decide(0, bet);
    }
}
