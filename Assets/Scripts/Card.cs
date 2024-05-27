using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Card
{
#region 静态
    /// <summary>
    /// 从0-51编号的卡牌数据为：
    /// 0-12: 黑桃A-K
    /// 13-25: 红桃A-K
    /// 26-38: 方块A-K
    /// 39-51: 梅花A-K
    /// </summary>
    public static List<CardData> cards = new List<CardData>();

    static Card()
    {
        InitializeCards();
    }

    public static void InitializeCards(){
        cards.Clear();
        for (int i = 0; i < 52; i++)
        {
            uint cardType = (uint)(i / 13);
            uint cardNum = (uint)(i % 13) + 1;
            cards.Add(new CardData(cardType, cardNum));
        }
    }

    /// <summary>
    /// 随机抽取一张牌
    /// </summary>
    public static CardData GetRandomCard()
    {
        if(cards.Count == 0){
            Debug.LogError("没有牌了！");
            return null;
        }
        int index = Random.Range(0, cards.Count-1);
        // while (cards[index].Used)
        // {
        //     index = Random.Range(0, 52);
        // }
        CardData cardData = cards[index];
        cards.Remove(cards[index]);
        return cardData;
    }
#endregion
   
    private CardData cardData;
    public CardData CardData { get { return cardData; } set { cardData = value; } }

    public Card(){
        cardData = GetRandomCard();
    }

    public Sprite FindSprite(){

        /// 这里需要根据cardData.CardType和cardData.CardNum找到对应的sprite在Sprites文件夹中的index，进而找到对应的sprite
        uint index = 0;
        index = CardData.CardType * 13 + CardData.CardNum - 1;

        return CardFileLoader.LoadFile(index);
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
    public uint CardType { get { return cardType; } }
    public uint CardNum { get { return cardNum; } }
    public CardData(uint cardType, uint cardNum)
    {
        this.cardType = cardType;
        this.cardNum = cardNum;
    }
}

public static class CardFactory
{
    public static Card CreateCard()
    {
        Card card = new Card();
        
        return card;
    }
}

public static class CardFileLoader{
    private static string path = "Assets/PlayingCards/Textures/Sprites";

    /// <summary>
    /// 根据索引获得卡牌的Sprite
    /// </summary>
    /// <param name="index">0-51为正常卡牌，52-53为大小王，54-58为牌背，59为空白</param>
    public static Sprite LoadFile(uint index){
        string fullpath = path + "/card_list_2d_" + index + ".asset";
        //Sprite sprite = Resources.Load<Sprite>(fullpath);
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(fullpath);
        return sprite;
    }
}
