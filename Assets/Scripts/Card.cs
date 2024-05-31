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

    #region 重载运算符
    public static bool operator <(Card card1, Card card2){
        Debug.Log(card1 + " " + card1.CardData + " " + card1.CardData.CardNum);
        if(card1.CardData.CardNum < card2.CardData.CardNum){
            if(card1.CardData.CardNum == 1){
                //A是最大的
                return false;
            }
            return true;
        }
        if(card1.CardData.CardNum > card2.CardData.CardNum){
            if(card2.CardData.CardNum == 1){
                //A是最大的
                return true;
            }
            return false;
        }
        return card1.CardData.CardType < card2.CardData.CardType;
    }

    public static bool operator >(Card card1, Card card2){
        if(card1.CardData.CardNum < card2.CardData.CardNum){
            if(card1.CardData.CardNum == 1){
                //A是最大的
                return true;
            }
            return false;
        }
        if(card1.CardData.CardNum > card2.CardData.CardNum){
            if(card2.CardData.CardNum == 1){
                //A是最大的
                return false;
            }
            return true;
        }
        return card1.CardData.CardType > card2.CardData.CardType;      
    }
    #endregion
   
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
    /// 0-3:Club, Diamond, Heart, Spade
    /// </summary>
    public uint CardType { get { return cardType; } }
    /// <summary>
    /// 1-13: A-K
    /// </summary>
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

    /// <summary>
    /// 根据索引获得卡牌的Sprite
    /// </summary>
    /// <param name="index">0-51为正常卡牌，52-53为大小王，54-58为牌背，59为空白</param>
    public static Sprite LoadFile(uint index){
        string path = "Sprites/card_list_2d_" + index;
        //Sprite sprite = Resources.Load<Sprite>(fullpath);
        Sprite sprite = Resources.Load<Sprite>(path);
        return sprite;
    }
}

public enum CombinationType{
        //维持程序正常运转不可避免的啊啊啊啊啊啊啊啊啊啊的非正常“0”
        Invalid,
        //高牌
        HighCard,
        //一对
        OnePair,
        //两对    
        TwoPair,
        //三条
        ThreeOfAKind,
        //葫芦
        FullHouse,
        //四条
        FourOfAKind,
        //顺子
        Straight,
        //同花
        Flush,
        //同花顺
        StraightFlush,
        //皇家同花顺
        RoyalFlush
    }

public class CardCombinationHelper{
    /// <summary>
    /// 输入五张牌的List，返回牌型和高牌（如有）、对子1数值（如有）、对子2数值（如有）
    /// </summary>
    /// <param name="cards">五张按照优先大小其次类型排序的牌</param>
    /// <param name="highCard">高牌</param>
    /// <param name="pair1">对子(包括1对、3条、4条，若一对则只需要pair1)</param>
    /// <param name="pair2">对子(第二对，包括2对，葫芦，若两对则需要pair2)</param>
    /// <returns></returns>
    public static CombinationType GetCombinationType(List<Card> cards, out Card highCard, out Card pair1, out Card pair2){
        highCard = null;
        pair1 = null;
        pair2 = null;
        if(cards.Count != 5){
            Debug.LogError("牌数不对！");
            return CombinationType.HighCard;
        }
        
        //判断同花
        bool isSameType = true;
        uint type = cards[0].CardData.CardType;
        foreach(var card in cards){
            if(card.CardData.CardType != type){
                isSameType = false;
                break;
            }
        }

        if(isSameType){
            //如果是同花
            //判断是否同花顺
            bool isStraightFlush = true;
            for(int i = 0; i < 4; i++){
                if(cards[i].CardData.CardNum != cards[i+1].CardData.CardNum - 1){
                    //A是特殊情况，其CardNum为1，但是最大
                    if(cards[i+1].CardData.CardNum == 1 && cards[i].CardData.CardNum == 13){
                        continue;
                    }
                    isStraightFlush = false;
                    break;
                }
            }
            if(isStraightFlush){
                //如果是同花顺
                //判断是否为皇家同花顺
                if(cards[4].CardData.CardNum == 1){
                    //如果最高牌为A，则为皇家同花顺
                    highCard = cards[4];
                    return CombinationType.RoyalFlush;
                }
                else{
                    //不是皇家同花顺，则为同花顺
                    highCard = cards[4];
                    return CombinationType.StraightFlush;
                }
            }
            else{
                //如果不是同花顺，则为同花
                highCard = cards[4];
                return CombinationType.Flush;
            }
        }else{
            //如果不是同花
            //判断是否为顺子
            bool isStraight = true;
            for(int i = 0; i < 4; i++){
                if(cards[i].CardData.CardNum != cards[i+1].CardData.CardNum - 1){
                    //A是特殊情况，其CardNum为1，但是最大
                    if(cards[i+1].CardData.CardNum == 1 && cards[i].CardData.CardNum == 13){
                        continue;
                    }
                    isStraight = false;
                    break;
                }
            }
            if(isStraight){
                //如果是顺子
                return CombinationType.Straight;
            }else{
                //如果不是顺子
                //判断是否为四条
                bool isFourOfAKind = false;
                bool frontFourSame = true;
                bool backFourSame = true;
                {
                    //检查前四张牌
                    for(int i = 0; i < 4; i++){
                        if(cards[i].CardData.CardNum != cards[0].CardData.CardNum){
                            frontFourSame = false;
                            break;
                        }
                    }
                    //检查后四张牌
                    for(int i = 1; i < 5; i++){
                        if(cards[i].CardData.CardNum != cards[4].CardData.CardNum){
                            backFourSame = false;
                            break;
                        }
                    }
                    isFourOfAKind = frontFourSame || backFourSame;
                }
                if(isFourOfAKind){
                    //如果是四条
                    pair1 = frontFourSame ? cards[0] : cards[4];
                    return CombinationType.FourOfAKind;
                }else{
                    //如果不是四条
                    //判断是否为三条
                    bool isThreeOfAKind = false;
                    int startIndex = 0;
                    for(int i=0;i<3;i++){
                        int count = 1;
                        for(int j=0;j<2;j++){
                            if(cards[i+j].CardData.CardNum == cards[i+j+1].CardData.CardNum){
                                count++;
                            }
                        }
                        if(count == 3){
                            isThreeOfAKind = true;
                            startIndex = i;
                            break;
                        }
                    }
                    if(isThreeOfAKind){
                        //如果是三条
                        //判断是否为葫芦
                        bool isFullHouse = false;
                        //startIndex只可能是0、1、2
                        if(startIndex == 1){
                            //startIndex为0，不可能再出现一对了
                            isFullHouse = false;
                        }else{
                            //startIndex为0或2，可能出现一对
                            if(startIndex == 0){
                                if(cards[3].CardData.CardNum == cards[4].CardData.CardNum){
                                    isFullHouse = true;
                                }
                            }else{
                                if(cards[0].CardData.CardNum == cards[1].CardData.CardNum){
                                    isFullHouse = true;
                                }
                            }
                        }
                        if(isFullHouse){
                            //如果是葫芦
                            pair1 = cards[startIndex];
                            //pair2 = cards[secondPairStartIndex]; --no need
                            return CombinationType.FullHouse;
                        }else{
                            //如果不是葫芦,则为三条
                            pair1 = cards[startIndex];
                            return CombinationType.ThreeOfAKind;
                        }
                    }
                    //如果不是三条
                    //判断是否为一对
                    bool isOnePair = false;
                    int pairStartIndex = 0;
                    for(int i=0;i<3;i++){
                        if(cards[i].CardData.CardNum == cards[i+1].CardData.CardNum){
                            isOnePair = true;
                            pairStartIndex = i;
                            break;
                        }
                    }
                    if(isOnePair){
                        //如果是一对
                        //判断是否为两对
                        bool isTwoPair = false;
                        int secondPairStartIndex = 0;
                        switch(pairStartIndex){
                            case 0:
                                for(int i = 2; i < 3; i++){
                                    if(cards[i].CardData.CardNum == cards[i+1].CardData.CardNum){
                                        isTwoPair = true;
                                        secondPairStartIndex = i;
                                        break;
                                    }
                                }
                                break;
                            case 1:
                                if(cards[3].CardData.CardNum == cards[4].CardData.CardNum){
                                    isTwoPair = true;
                                    secondPairStartIndex = 3;
                                }
                                break;
                            //其他情况不可能出现，保证了pairStartIndex绝对小于secondPairStartIndex
                            default:
                                break;
                        }
                        if(isTwoPair){
                            //如果是两对, pair1绝对比pair2小
                            pair1 = cards[pairStartIndex];
                            pair2 = cards[secondPairStartIndex];
                            if(pairStartIndex == 0){
                                if(secondPairStartIndex == 2){
                                    highCard = cards[4];
                                }else{
                                    highCard = cards[2];
                                }
                            }else{
                                highCard = cards[0];
                            }
                            return CombinationType.TwoPair;
                        }else{
                            //如果不是两对
                            pair1 = cards[pairStartIndex];
                            if(pairStartIndex == 3){
                                highCard = cards[2];
                            }else{
                                highCard = cards[4];
                            }
                            return CombinationType.OnePair;
                        }
                    }
                    //如果不是一对
                    //那就是高牌
                    highCard = cards[4];
                    return CombinationType.HighCard;
                }
            }
        }
    }
}

public class CardsValue{
        public CombinationType Type{get;set;}
        public Card highCard{get;set;}
        public Card pair1{get;set;}
        public Card pair2{get;set;}
        public CardsValue(CombinationType type, Card highCard ,Card pair1 ,Card pair2){
            this.Type = type;
            this.highCard = highCard;
            this.pair1 = pair1;
            this.pair2 = pair2;
        }
        public static bool operator <(CardsValue left, CardsValue right){
            if(left.Type != right.Type){
                return (int)left.Type < (int)right.Type;
            }else{
                switch(left.Type){
                    case CombinationType.StraightFlush:
                        return left.highCard < right.highCard;
                    case CombinationType.Flush:
                        return left.highCard < right.highCard;
                    case CombinationType.Straight:
                        return left.highCard < right.highCard;
                    case CombinationType.FourOfAKind:
                        return left.pair1 < right.pair1;
                    case CombinationType.FullHouse:
                        return left.pair1 < right.pair1;
                    case CombinationType.ThreeOfAKind:
                        return left.pair1 < right.pair1;
                    case CombinationType.TwoPair:
                        if(left.pair2 == right.pair2){
                            if(left.pair1 == right.pair1){
                                return left.highCard < right.highCard; 
                            }else{
                                return left.pair1 < right.pair1;
                            }
                        }else{
                            return left.pair2 < right.pair2;
                        }
                    case CombinationType.OnePair:
                        if(left.pair1 == right.pair1){
                            return left.highCard < right.highCard;
                        }else{
                            return left.pair1 < right.pair1;
                        }
                    case CombinationType.HighCard:
                        return left.highCard < right.highCard;
                    default:
                        return left < right;
                }
            }
        }
        public static bool operator >(CardsValue left,CardsValue right){
            if(left.Type != right.Type){
                return (int)left.Type > (int)right.Type;
            }else{
                switch(left.Type){
                    case CombinationType.StraightFlush:
                        return left.highCard > right.highCard;
                    case CombinationType.Flush:
                        return left.highCard > right.highCard;
                    case CombinationType.Straight:
                        return left.highCard > right.highCard;
                    case CombinationType.FourOfAKind:
                        return left.pair1 > right.pair1;
                    case CombinationType.FullHouse:
                        return left.pair1 > right.pair1;
                    case CombinationType.ThreeOfAKind:
                        return left.pair1 > right.pair1;
                    case CombinationType.TwoPair:
                        if(left.pair2 == right.pair2){
                            if(left.pair1 == right.pair1){
                                return left.highCard > right.highCard; 
                            }else{
                                return left.pair1 > right.pair1;
                            }
                        }else{
                            return left.pair2 > right.pair2;
                        }
                    case CombinationType.OnePair:
                        if(left.pair1 == right.pair1){
                            return left.highCard > right.highCard;
                        }else{
                            return left.pair1 > right.pair1;
                        }
                    case CombinationType.HighCard:
                        return left.highCard > right.highCard;
                    default:
                        return left > right;
                }
            }
        }
        public static bool operator ==(CardsValue left, CardsValue right){
            return left.Type == right.Type && left.pair1 == right.pair1 && left.pair2 == right.pair2;
        }
        public static bool operator !=(CardsValue left, CardsValue right){
            return !(left.Type == right.Type && left.pair1 == right.pair1 && left.pair2 == right.pair2);
        }
    }