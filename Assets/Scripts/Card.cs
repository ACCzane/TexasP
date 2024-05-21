using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    /// <summary>
    /// 从0-51编号的卡牌数据为：
    /// 0-12: 黑桃A-K
    /// 13-25: 红桃A-K
    /// 26-38: 方块A-K
    /// 39-51: 梅花A-K
    /// </summary>
    public static CardData[] cards = new CardData[52];

    static Card()
    {
        for (int i = 0; i < 52; i++)
        {
            uint cardType = (uint)(i / 13);
            uint cardNum = (uint)(i % 13) + 1;
            cards[i] = new CardData(cardType, cardNum);
        }
    }

    /// <summary>
    /// 随机抽取一张牌
    /// </summary>
    public static CardData GetRandomCard()
    {
        int index = Random.Range(0, 52);
        while (cards[index].Used)
        {
            index = Random.Range(0, 52);
        }
        cards[index].Used = true;
        return cards[index];
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public class CardData{
    /// <summary>
    /// 0-3:Club, Diamond, Heart, Spade
    /// </summary>
    private uint cardType;
    /// <summary>
    /// 1-13: A-K
    /// </summary>
    private uint cardNum;
    /// <summary>
    /// 0:unused, 1:used
    /// </summary>
    private bool used;
    public uint CardType { get { return cardType; } }
    public uint CardNum { get { return cardNum; } }
    public bool Used { get { return used; } set { used = value; } }
    public CardData(uint cardType, uint cardNum)
    {
        this.cardType = cardType;
        this.cardNum = cardNum;
        this.used = false;
    }
}