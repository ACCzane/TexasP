using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameControlFlow : MonoBehaviour
{
    [HideInInspector]public string playerPath = "/PlayerCanvas/Players";    //场景中Player的路径
    [SerializeField]private Dealer dealer;
    [SerializeField]private PoolAmount pool;
    [SerializeField]private PublicCard publicCard;
    [HideInInspector]public LinkList<Player> players;
    [HideInInspector]public SharedInfo sharedInfo;
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

        players = new LinkList<Player>();

        if(playersParent != null){
            int playerNum = playersParent.transform.childCount;
            for(int i = 0; i < playerNum; i++){
                GameObject playerObj = playersParent.transform.GetChild(i).gameObject;
                players.Add(playerObj.GetComponent<Player>());
            }
        }

        dealer.InitializeDealer(players);
        //初始化共享信息, 包括玩家列表、当前玩家索引、TODO: 上一玩家索引、当前阶段、上一阶段
        sharedInfo = new SharedInfo(players);

        //初始化玩家的钱, 顺便初始化玩家的Visual
        int playerInitialMoney = 100;
        dealer.SendMoneyToEveryPlayer(playerInitialMoney);

    }

    public void Perform_CurrentTerm() {
        switch(currentTerm){
            case 0:

                //初始化公共牌
                publicCard.Initialize3Cards();

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
                
                StartCoroutine(CurrentTermLoop());

                break;
            case 1:
                //3张公共牌
                //决定庄家
                //第一个作决策的人开始决策，直到所有人都做完决策，进入下一阶段或直接结算

                //翻牌
                publicCard.Show3Cards();

                StartCoroutine(CurrentTermLoop());
                break;
            case 2:
                //加1张公共牌
                //决定庄家
                //第一个作决策的人开始决策，直到所有人都做完决策，进入下一阶段或直接结算

                //转牌
                publicCard.AddCard();

                StartCoroutine(CurrentTermLoop());
                break;
            case 3:
                //加1张公共牌
                //决定庄家
                //第一个作决策的人开始决策，直到所有人都做完决策，进入下一阶段或直接结算

                //河牌
                publicCard.AddCard();

                StartCoroutine(CurrentTermLoop());
                break;
            case 4:
                //每个还留在牌局里的人计算手牌大小
                //最大的获胜
                break;
        }
    }

    public IEnumerator CurrentTermLoop(){
        //Debug.Log(sharedInfo.CurrentActivePlayerNode.Item.Bet);
        //Debug.Log(sharedInfo.CurrentActivePlayerNode.Prev.Item.Bet);

        //庄家不管怎样要先决策
        yield return PlayerDecisionLoop();

        //如果上一玩家的下注比当前玩家的下注大，或者上一玩家过牌且自己曾经没有过牌，或者上个玩家状态为Hold，则等待玩家决策
        Debug.Log(sharedInfo.CurrentActivePlayerNode.Prev.Item.Bet);
        Debug.Log(sharedInfo.CurrentActivePlayerNode.Item.Stat);
        while(sharedInfo.CurrentActivePlayerNode.Item.Bet < sharedInfo.CurrentActivePlayerNode.Prev.Item.Bet || (sharedInfo.CurrentActivePlayerNode.Prev.Item.Stat == 1 && currentPlayer.Stat != 1)){

            yield return PlayerDecisionLoop();
            
        }
        //最后决策的玩家的下家必定是下一轮的庄家, 由于循环的最后有GetNextActivePlayer操作，所以不需要再次赋值

        //所有玩家的下注注入底池
        dealer.TakeMoneyFromAllPlayers();

        if(sharedInfo.CurrentActivePlayerNode == sharedInfo.CurrentActivePlayerNode.Prev){
            //TODO:如果一只剩一名玩家，则直接进入结算阶段
            currentTerm = 4;
        }

        //进入下一阶段
        pool.SetAmount(dealer.totalMoney);
        currentTerm++;
    }

    private IEnumerator PlayerDecisionLoop(){
        //玩家记录上家信息
        currentPlayer.Upper_bet = sharedInfo.CurrentActivePlayerNode.Prev.Item.Bet;
        //玩家决策
        currentPlayer.ShowPlayerControl();

        //玩家做决策的时间, 如果currentPlayer.transistionToNextPlayer == false，则说明该交互窗口已经被关闭了
        while(currentPlayer.transistionToNextPlayer == false){
            yield return null;
        }

        //玩家做完了决策
        //检测玩家是否弃牌
        if(currentPlayer.Stat == 0){
            //玩家弃牌
            sharedInfo.ActivePlayers.Delete(currentPlayer);
            Debug.Log(sharedInfo.ActivePlayers.Count());
        }


        //进入下一玩家
        //currentPlayer.HidePlayerControl();
        currentPlayer = sharedInfo.GetNextActivePlayer();
    }

    public void ResetPlayerStat(){
        Node<Player> playerNode = sharedInfo.ActivePlayers.FirstNode;
        Debug.Log(sharedInfo.ActivePlayers.Count());
        playerNode.Item.Player_Hold();
        playerNode = playerNode.Next;
        while(playerNode != sharedInfo.ActivePlayers.FirstNode){
            playerNode.Item.Player_Hold();
            playerNode = playerNode.Next;
        }
    }

#region  Debug
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)){
            GameStart();
            ResetPlayerStat();
            Perform_CurrentTerm();
        }
        if(Input.GetKeyDown(KeyCode.A)){
            ResetPlayerStat();
            Perform_CurrentTerm();
        }
    }
#endregion
}
