using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
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
    /// 玩家是否弃牌
    /// </summary>
    private bool fold;

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

    public bool Fold { get => fold; set => fold = value; }

    public bool Allin { get => allin; set => allin = value; }

    public bool Check { get => check; set => check = value; }

    /// <summary>
    /// 0-3代表下注、弃牌、全下、过牌
    /// </summary>
    /// <param name="action"></param>
    public void Decide(uint action)
    {
        // 玩家决定下注、弃牌、全下、过牌等操作
        switch (action)
        {
            case 0:
                // 下注
                break;
            case 1:
                // 弃牌
                break;
            case 2:
                // 全下
                break;
            case 3:
                // 过牌
                break;
        }
    }

    public void CountValue()
    {
        // 计算玩家手牌的牌型组合

        // 得到最好的牌型组合

        // 最好的牌型中的最大牌的点数
    }

    private void Player_Bet(float bet)
    {
        // 玩家下注
    }

    private void Player_Fold()
    {
        // 玩家弃牌
    }

    private void Player_Allin()
    {
        // 玩家全下
    }

    private void Player_Check()
    {
        // 玩家过牌
    }
}
