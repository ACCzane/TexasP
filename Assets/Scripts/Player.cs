using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Player : MonoBehaviour
{
#region 数据
    /// <summary>
    /// 玩家手牌
    /// </summary>
    private Card[] cards = new Card[2];

    /// <summary>
    /// 玩家排名，按照现有本金的大小排序
    /// </summary>
    private int rank;

    /// <summary>
    /// 玩家现有本金
    /// </summary>
    private float money;

    /// <summary>
    /// 玩家当前下注
    /// </summary>
    private float bet;

    /// <summary>
    /// 上家的下注
    /// </summary>
    private float upper_bet;

    /// <summary>
    /// 玩家状态, 0-4:弃牌、过牌、跟注、加注、AllIn
    /// </summary>
    private uint stat;

    /// <summary>
    /// 玩家是否全下
    /// </summary>
    private bool allin;

    /// <summary>
    /// 玩家是否过牌
    /// </summary>
    private bool check;

    public Card[] Cards { get => cards;}

    public int Rank { get => rank; set => rank = value; }

    public float Money { get => money; set => money = value; }

    public float Bet { get => bet; set => bet = value; }

    public uint Stat { get => stat; set => stat = value; }

    public bool Allin { get => allin; set => allin = value; }

    public bool Check { get => check; set => check = value; }

#endregion

#region 事件
    public EventHandler<float> OnChangeTotalMoney;
    public EventHandler<float> OnBet;
    public EventHandler<uint> OnChangeStat;
#endregion

#region API
    /// <summary>
    /// 0-3代表下注、弃牌、过牌、全压
    /// </summary>
    /// <param name="action"></param>
    public void Decide(uint action, float bet)
    {
        // 玩家决定下注、弃牌、过牌
        switch (action)
        {
            case 0:
                // 下注/跟注/加注
                Player_Bet(bet);
                break;
            case 1:
                // 弃牌
                Player_Fold();
                break;
            case 2:
                // 过牌
                Player_Check();
                break;
            case 3:
                // 全压
                Player_Allin();
                break;
            default:
                break;
        }
    }

    public void CountValue()
    {
        // 计算玩家手牌的牌型组合

        // 得到最好的牌型组合

        // 最好的牌型中的最大牌的点数
    }

    public void Player_Bet(float bet)
    {
        // 玩家下注
        if(money > bet){
            //满足下注的条件：跟、加
            if(upper_bet == 0){
                //成为第一个下注的人
                DoBet(bet);
            }else{
                //上家下过注，可以跟、加
                if(bet == upper_bet){
                    //跟上家
                    DoBet(bet);

                    Stat = 2;
                    OnChangeStat?.Invoke(this, Stat);
                }else if(bet >= upper_bet * 2){
                    //加注
                    DoBet(bet);

                    Stat = 3;
                    OnChangeStat?.Invoke(this, Stat);
                }else{
                    Debug.LogError("下注不能小于上家的下注, 加注不得少于上家的两倍");
                }
            }

        }
        else if(money == bet){
            Debug.Log("是否要AllIn");
        }
        else{
            Debug.LogError("超出本金");
        }
    }

    private void DoBet(float bet){
        Money -= bet;
        Bet += bet;
        OnBet?.Invoke(this, Bet);
        OnChangeTotalMoney?.Invoke(this, Money);
    }

    private void Player_Fold()
    {
        // 玩家弃牌
        Stat = 0;
        OnChangeStat?.Invoke(this, Stat);
    }

    private void Player_Check()
    {
        // 玩家过牌
        Stat = 1;
        OnChangeStat?.Invoke(this, Stat);
    }

    private void Player_Allin(){
        DoBet(bet);
        Stat = 4;
        OnChangeStat?.Invoke(this, Stat);
    }

    public void Addcard(Card card1, Card card2){
        // 玩家摸牌
        cards[0] = card1;
        cards[1] = card2;
    }
    #endregion
}

