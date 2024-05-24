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
    [SerializeField]private Player player;

    public void Player_Check(){
        player.Decide(2,0f);
    }

    public void Player_Fold(){
        player.Decide(1,0f);
    }

    public void Player_Bet(float bet){
        player.Decide(0, bet);
    }

#region debug
    private void Update(){
        if(Input.GetKeyDown(KeyCode.Space)){
            //开始

            //玩家获得本金
            player.Money = 100f;
            player.OnChangeTotalMoney?.Invoke(this, player.Money);
        
            //Bet初始为0
            player.Bet = 0f;
            player.OnBet?.Invoke(this, player.Bet);
        }
    }
#endregion
}
