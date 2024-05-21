using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dealer : MonoBehaviour
{
    private string playerPath = "/PlayerCanvas/Players";    //场景中Player的路径
    private Player[] players;

    private void SendMoneyToPlayer(Player player, int money){
        player.Money += money;
    }

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

    private void SendMoneyToEveryPlayer(int money) {
        if(players != null){
            foreach(Player player in players){
                SendMoneyToPlayer(player, money);
            }
        }
    }
}
