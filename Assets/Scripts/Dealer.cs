using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dealer
{
    private LinkList<Player> players;

    /// <summary>
    /// 当前轮次的总金额
    /// </summary>
    private float currentRoundTotalMoney;
    /// <summary>
    /// 底池
    /// </summary>
    private float totalMoney;

    public Dealer(LinkList<Player> players) {
        this.players = players;
    }

    /// <summary>
    /// 游戏最开始所有玩家获得的本金
    /// </summary>
    /// <param name="money">本金</param>
    public void SendMoneyToEveryPlayer(int money) {
        if(players != null){
            Node<Player> current = players.FirstNode;
            while(current != null){
                SendMoneyToPlayer(current.Item, money);
                current = current.Next;
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
    public void SendCardToEveryPlayer(){
        if(players != null){
            Node<Player> current = players.FirstNode;
            while(current != null){
                SendCardToPlayer(current.Item);
                current = current.Next;
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
    
    /// <summary>
    /// 回合结束时，从所有玩家那里取钱放进底池
    /// </summary>
    public void TakeMoneyFromAllPlayers(){
        if(players != null){
            Node<Player> current = players.FirstNode;
            while(current != null){
                TakeMoneyFromPlayer(current.Item);
                current = current.Next;
            }
        }
    }

    /// <summary>
    /// 将玩家所下的金额放进底池
    /// </summary>
    /// <param name="player"></param>
    private void TakeMoneyFromPlayer(Player player){
        currentRoundTotalMoney += player.Bet;
        player.Bet = 0;
    }
}
