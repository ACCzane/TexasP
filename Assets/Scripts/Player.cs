using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Unity.Netcode;

public class Player : NetworkBehaviour
{
#region 数据
    /// <summary>
    /// 玩家用户名
    /// </summary>
    private string playerName;

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
    public float Upper_bet { get => upper_bet; set => upper_bet = value; }

    /// <summary>
    /// 玩家状态, 0-5:弃牌、过牌、跟注、加注、AllIn、等待
    /// </summary>
    public uint Stat { get => stat; set => stat = value; }

    public bool Allin { get => allin; set => allin = value; }

    public bool Check { get => check; set => check = value; }


    public GameObject PlayerPrefab;
    public GameObject PlayerControlPrefab;
    [SerializeField] private Transform playerControlParent;
    private GameObject playerControl;

    public bool isMyTurn = false;
    public bool transistionToNextPlayer = false;

    /// <summary>
    /// 当前手牌牌型
    /// </summary>
    public CardsValue bestCardsValue { get; set; }
    /// <summary>
    /// 最好牌型的手牌
    /// </summary>
    public List<Card> bestCards { get; set; }

#endregion

#region 事件
    public EventHandler<string> OnChangeName;

    public EventHandler<float> OnChangeTotalMoney;
    public EventHandler<float> OnBet;
    public EventHandler<uint> OnChangeStat;

    /// <summary>
    /// 玩家手牌更新事件, uint值为0表示牌背、1表示牌面、2表示没牌
    /// </summary>
    public EventHandler<uint> OnUpdateCards;

    /// <summary>
    /// 当前是本玩家回合触发
    /// </summary>
    public EventHandler<int> OnControl;

    /// <summary>
    /// 玩家获胜
    /// </summary>
    public EventHandler OnWin;

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
        HidePlayerControl();
    }
    public void CountValue()
    {
        // 计算玩家手牌的牌型组合

        // 得到最好的牌型组合

        // 最好的牌型中的最大牌的点数
    }

    /// <summary>
    /// 此处bet为增加值，不是直接下注值
    /// </summary>
    /// <param name="bet"></param>
    public void Player_Bet(float bet)
    {
        // 玩家下注
        if(money > bet){
            //满足下注的条件：跟、加
            if(upper_bet == 0){
                //成为第一个下注的人
                DoBet(bet);
                OnChangeStat?.Invoke(this, Stat);
            }else{
                //上家下过注，可以跟、加
                if(bet + Bet == Upper_bet){
                    //跟上家
                    DoBet(bet);

                    Stat = 2;
                    OnChangeStat?.Invoke(this, Stat);
                }else if(bet + Bet >= Upper_bet * 2){
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
        OnControl?.Invoke(this, -1);
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

    public void Player_Hold(){
        Stat = 5;
        OnChangeStat?.Invoke(this, Stat);
    }

    public void Addcard(Card card1, Card card2){
        // 玩家摸牌
        cards[0] = card1;
        cards[1] = card2;
        OnUpdateCards?.Invoke(this, 0);

        if(IsOwner){
            OnUpdateCards?.Invoke(this, 1);
        }
    }

    // public void CreatePlayerPrefabAndControl(){
    //     // 创建玩家预制体并更改父物体
    //     GameObject _playerParent = GameObject.Find("Players");
    //     Debug.Log(_playerParent);
    //     transform.SetParent(_playerParent.transform, false);


    //     // 创建玩家控制面板并更改父物体
    //     GameObject _playerControlParent = GameObject.Find("PlayerCanvas");
    //     playerControlParent = _playerControlParent.transform;
    //     playerControl = Instantiate(PlayerControlPrefab, playerControlParent, false);
    //     //TODO: 将player（this）绑定到prefab的脚本中
    //     playerControl.GetComponent<PlayerControl>().BindPlayer(this);
    // }

    public void ShowPlayerControl(){
        if(Upper_bet != 0){
            //无法过牌
            playerControl.GetComponent<PlayerControl>().Show(false);
        }else{
            playerControl.GetComponent<PlayerControl>().Show(true);
        }
        transistionToNextPlayer = false;
        isMyTurn = true;
        OnControl?.Invoke(this, 1);
    }

    public void HidePlayerControl(){
        playerControl.GetComponent<PlayerControl>().Hide();
        transistionToNextPlayer = true;
        isMyTurn = false;
        if(stat != 0)
            OnControl?.Invoke(this, 0);
    }

    /// <summary>
    /// 把当前下的注全清空，更新Visual的元素
    /// </summary>
    public void Update_toNextTerm(){
        Bet = 0;
        OnBet?.Invoke(this, 0);
        OnChangeTotalMoney?.Invoke(this, Money);
        //OnChangeTotalMoney?.Invoke(this, 0);
        //OnChangeStat?.Invoke(this, 0);
    }

    class CardComparer : IComparer<Card>
    {
        public int Compare(Card card1, Card card2)
        {
            if(card1.CardData.CardNum < card2.CardData.CardNum){
                if(card1.CardData.CardNum == 1){
                    //A是最大的
                    return -1;
                }
                return 1;
            }else if(card1.CardData.CardNum > card2.CardData.CardNum){
                if(card2.CardData.CardNum == 1){
                    //A是最大的
                    return 1;
                }
                return -1;
            }
            //card1.CardData.CardNum == card2.CardData.CardNum
            if(card1.CardData.CardType < card2.CardData.CardType){
                return -1;
            }else if(card1.CardData.CardType == card2.CardData.CardType){
                return 0;
            }else{
                return 1;
            }
        }
    }

    /// <summary>
    /// 输入公共牌的List<Card>，返回CardsValue类，里面定义了类型、高牌、pair1、pair2，也重载了运算符
    /// </summary>
    /// <param name="publicCards"></param>
    public CardsValue CountHandValue(List<Card> publicCards, out List<Card> bestCards){
        //cards保存7张牌，包括2张手牌和5张公共牌
        List<Card> sevenCards = new List<Card>();
        for(int i = 0; i < 2; i++){
            sevenCards.Add(this.cards[i]);
        }
        sevenCards.AddRange(publicCards);
        //Card类的运算符已重载，可以排序
        sevenCards.Sort(new CardComparer());
        
        //7个里挑选5个

        CombinationType type;
        Card highCard;
        Card pair1;
        Card pair2;

        List<Card> fiveCards = new List<Card>();
        bestCards = null;

        CardsValue maxCardsValue = new CardsValue(0,null,null,null);

        for(int i=0;i<6;i++){
            for(int j=i+1;j<7;j++){
                fiveCards.Clear();
                for(int k=0;k<7;k++){
                    if(k == i || k == j){
                        continue;
                    }
                    fiveCards.Add(sevenCards[k]);
                }
                //取到五张牌后, 维护最大牌型
                type = CardCombinationHelper.GetCombinationType(fiveCards, out highCard, out pair1, out pair2);
                CardsValue newCardsValue = new CardsValue(type, highCard, pair1, pair2);

                if(newCardsValue > maxCardsValue){
                    maxCardsValue = newCardsValue;
                    bestCards = fiveCards;
                }
            }
        }

        return maxCardsValue;
    }

    public void SetBestValuedCards(CardsValue bestCardsValue, List<Card> bestCards){
        this.bestCardsValue = bestCardsValue;
        this.bestCards = bestCards;
    }

    public void WinMoney(float money){
        Money += money;
        OnChangeTotalMoney?.Invoke(this, Money);
        OnWin?.Invoke(this, EventArgs.Empty);
    }

    public void ChangeName(string name){
        playerName = name;
        OnChangeName?.Invoke(this, name);
    }
#endregion

#region 网络同步
    
#endregion
}

/// <summary>
/// 双向环状链表的结点
/// </summary>
/// <typeparam name="T"></typeparam>
public class Node<T>{
    private T item;
    private Node<T> next;
    private Node<T> prev;
    public T Item { get => item; set => item = value; }
    public Node<T> Next { get => next; set => next = value; }
    public Node<T> Prev { get => prev; set => prev = value; }
    public Node(T item){
        Item = item;
        Next = null;
        Prev = null;
    }
    public Node(T item, Node<T> next){
        Item = item;
        Next = next;
    }
    public Node(T item, Node<T> prev, Node<T> next){
        Item = item;
        Prev = prev;
        Next = next;
    }
}

/// <summary>
/// 双向环状链表，可以Add、Delete
/// </summary>
/// <typeparam name="T"></typeparam>
public class LinkList<T>{
    private Node<T> firstNode;
    private Node<T> lastNode;
    private bool isEmpty;
    public Node<T> FirstNode { get => firstNode; set => firstNode = value; }
    public Node<T> LastNode { get => lastNode; set => lastNode = value; }
    public bool IsEmpty { get => isEmpty;}

    public void Add(T item){
        if(FirstNode == null){
            Node<T> node3 = new Node<T>(item);
            FirstNode = node3;
            LastNode = FirstNode;
            FirstNode.Prev = FirstNode;
            FirstNode.Next = FirstNode;
            return;
        }
        Node<T> node = FirstNode;
        while(node != LastNode){
            //node.Next.Prev = node;
            node = node.Next;
        }
        Node<T> node2 = new Node<T>(item, FirstNode);
        LastNode = node2;
        node.Next = node2;
        node2.Prev = node;

        node2.Next = FirstNode;
        FirstNode.Prev = node2;
    }
    public bool Delete(T item){
        if(FirstNode == null){
            Debug.LogError("LinkList is empty");
            return false;
        }
        if(FirstNode == LastNode && FirstNode.Item.Equals(item)){
            FirstNode = null;
            LastNode = null;
            isEmpty = true;
            return true;
        }
        Node<T> node = FirstNode;
        if(node.Item.Equals(item)){
            //如果删除的是头结点，则需要更新头节点
            FirstNode = node.Next;
            node.Prev.Next = node.Next;
            node.Next.Prev = node.Prev;
            return true;
        }
        node = node.Next;
        while(node != FirstNode){
            if(node.Item.Equals(item)){
                node.Prev.Next = node.Next;
                node.Next.Prev = node.Prev;
                //如果删除的是尾结点，则需要更新尾节点
                if(node == LastNode){
                    LastNode = node.Prev;
                }
                return true;
            }
            node = node.Next;
        }
        return false;
    }
    public int Count(){
        Node<T> node = FirstNode;
        if(FirstNode == null){
            return 0;
        }
        else if(FirstNode == LastNode){
            return 1;
        }
        node = FirstNode.Next;
        int count = 1;
        while(node != FirstNode){
            count++;
            node = node.Next;
        }
        return count;
    }

    public LinkList<T> CopySelf(){
        LinkList<T> linkList = new LinkList<T>();
        if(this.Count() == 0){
            return linkList;
        }
        else if(this.Count() == 1){
            Node<T> node1 = FirstNode;
            linkList.Add(node1.Item);
            return linkList;
        }else{
            Node<T> node = FirstNode;
            linkList.Add(node.Item);
            node = node.Next;
            while(node!=FirstNode){
                linkList.Add(node.Item);
                node = node.Next;
            }
            return linkList;
        }
    }
}