using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControlFlow
{
    public string playerPath = "/PlayerCanvas/Players";    //场景中Player的路径
    public Dealer dealer;
    public LinkList<Player> players;
    public SharedInfo sharedInfo;
    /// <summary>
    /// 小盲注金额
    /// </summary>
    public float blind;
    /// <summary>
    /// 0代表翻牌前、1代表翻拍、2代表转派、3代表河牌、4代表结算
    /// </summary>
    public uint currentTerm;
    public Player currentPlayer;

    /// <summary>
    /// 游戏开始时初始化玩家信息
    /// </summary>
    public void GameStart() {
        GameObject playersParent = GameObject.Find(playerPath);

        if(playersParent != null){
            int playerNum = playersParent.transform.childCount;
            for(int i = 0; i < playerNum; i++){
                GameObject playerObj = playersParent.transform.GetChild(i).gameObject;
                players.Add(playerObj.GetComponent<Player>());
            }
        }

        dealer = new Dealer(players);
        //初始化共享信息, 包括玩家列表、当前玩家索引、TODO: 上一玩家索引、当前阶段、上一阶段
        sharedInfo = new SharedInfo(players);

        //初始化玩家的钱
        int playerInitialMoney = 100;
        dealer.SendMoneyToEveryPlayer(playerInitialMoney);
    }

    public void Perform_CurrentTerm() {
        switch(currentTerm){
            case 0:
                //发牌
                dealer.SendCardToEveryPlayer();
                //小盲注
                currentPlayer = sharedInfo.GetNextActivePlayer();
                currentPlayer.Decide(0, blind);
                //大盲注
                currentPlayer = sharedInfo.GetNextActivePlayer();
                currentPlayer.Decide(0, blind * 2);
                //第一个自主决策的人
                currentPlayer = sharedInfo.GetNextActivePlayer();

                //只要当前玩家Bet小于上家Bet，就一直循环
                while(sharedInfo.CurrentActivePlayerNode.Item.Bet < sharedInfo.CurrentActivePlayerNode.Prev.Item.Bet){
                    //玩家决策


                    //进入下一玩家
                    currentPlayer = sharedInfo.GetNextActivePlayer();
                }

                //最后下注的玩家的下家必定是下一轮的庄家
                currentPlayer = sharedInfo.GetNextActivePlayer();
                //所有玩家的下注注入底池
                dealer.TakeMoneyFromAllPlayers();

                //进入下一阶段
                break;
            case 1:
                //3张公共牌
                //决定庄家
                //第一个作决策的人开始决策，直到所有人都做完决策，进入下一阶段或直接结算
                break;
            case 2:
                //加1张公共牌
                //决定庄家
                //第一个作决策的人开始决策，直到所有人都做完决策，进入下一阶段或直接结算
                break;
            case 3:
                //加1张公共牌
                //决定庄家
                //第一个作决策的人开始决策，直到所有人都做完决策，进入下一阶段或直接结算
                break;
            case 4:
                //每个还留在牌局里的人计算手牌大小
                //最大的获胜
                break;
        }
    }
}
