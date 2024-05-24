using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dealer : MonoBehaviour
{
    private string playerPath = "/PlayerCanvas/Players";    //场景中Player的路径
    private Player[] players;

    /// <summary>
    /// 游戏开始时初始化玩家信息
    /// </summary>
    private void GameStart() {
        GameObject playersParent = GameObject.Find(playerPath);

        if(playersParent != null){
            int playerNum = playersParent.transform.childCount;
            players = new Player[playerNum];
            for(int i = 0; i < playerNum; i++){
                GameObject playerObj = playersParent.transform.GetChild(i).gameObject;
                players[i] = playerObj.GetComponent<Player>();
            }
        }
    }

    /// <summary>
    /// 游戏最开始所有玩家获得的本金
    /// </summary>
    /// <param name="money">本金</param>
    private void SendMoneyToEveryPlayer(int money) {
        if(players != null){
            foreach(Player player in players){
                SendMoneyToPlayer(player, money);
            }
        }
    }

    /// <summary>
    /// 发钱给指定玩家
    /// </summary>
    /// <param name="player"></param>
    /// <param name="money"></param>
    private void SendMoneyToPlayer(Player player, int money){
        player.Money += money;
    }

    /// <summary>
    /// 游戏开始时发牌给所有玩家
    /// </summary>
    private void SendCardToEveryPlayer(){
        if(players != null){
            foreach(Player player in players){
                SendCardToPlayer(player);
            }
        }
    }

    /// <summary>
    /// 发牌给指定玩家
    /// </summary>
    /// <param name="player">玩家</param>
    private void SendCardToPlayer(Player player){
        Card card1 = CardFactory.CreateCard();
        Card card2 = CardFactory.CreateCard();
        player.Addcard(card1, card2);       
    }
    
}
