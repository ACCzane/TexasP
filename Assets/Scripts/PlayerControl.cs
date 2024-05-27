using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControl : MonoBehaviour
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

    public void BindPlayer(Player player){

        Player = player;
        playerBetAmount.Player = player;

        gameObject.SetActive(false);
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

    public void Player_Check(){
        Player.Decide(2,0f);
    }

    public void Player_Fold(){
        Player.Decide(1,0f);
    }

    public void Player_Bet(float bet){
        Player.Decide(0, bet);
    }
}
