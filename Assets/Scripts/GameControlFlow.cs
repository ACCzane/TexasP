using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Unity.Netcode;

public class GameControlFlow : MonoBehaviour
{
    [SerializeField]private Transform Players;    //场景中Player的路径
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
        GameObject playersParent = Players.gameObject;

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
                List<Player> winners = CalculateWinner();
                Debug.Log("winnerCount:"+winners.Count+" total:"+dealer.totalMoney);
                
                foreach(Player player in winners){
                    //赢钱
                    player.WinMoney( (float)Math.Round((dealer.totalMoney/winners.Count),1,MidpointRounding.AwayFromZero) );
                    //输出牌型信息
                    Debug.Log(player + ": " + player.bestCardsValue.Type);
                }
                //底池归零
                dealer.ResetTotalMoney();
                break;
        }
    }

    /// <summary>
    /// 当前回合（指玩家可控制的回合）的循环
    /// </summary>
    /// <returns></returns>
    public IEnumerator CurrentTermLoop(){
        //Debug.Log(sharedInfo.CurrentActivePlayerNode.Item.Bet);
        //Debug.Log(sharedInfo.CurrentActivePlayerNode.Prev.Item.Bet);

        //庄家不管怎样要先决策
        yield return PlayerDecisionLoop();

        //如果上一玩家的下注比当前玩家的下注大，或者上一玩家过牌且自己曾经没有过牌，或者上个玩家状态为Hold，则等待玩家决策
        while(sharedInfo.CurrentActivePlayerNode.Item.Bet < sharedInfo.CurrentActivePlayerNode.Prev.Item.Bet 
        || (sharedInfo.CurrentActivePlayerNode.Prev.Item.Stat == 1 && currentPlayer.Stat != 1)
        || sharedInfo.CurrentActivePlayerNode.Prev.Item.Stat == 5){

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

    [ClientRpc]
    private void ShowPlayerControlClientRpc(ulong plyaerId){
        if(NetworkManager.Singleton.LocalClientId == plyaerId){
            foreach(Transform player in Players){
                Player _player = player.GetComponent<Player>();  
                if(_player.PlayerID.Value == plyaerId){
                    _player.ShowPlayerControl();
                }
            }
        }
    }

    private IEnumerator PlayerDecisionLoop(){
        //玩家记录上家信息
        currentPlayer.Upper_bet = sharedInfo.CurrentActivePlayerNode.Prev.Item.Bet;
        //玩家决策
        ShowPlayerControlClientRpc(currentPlayer.PlayerID.Value);

        //玩家做决策的时间, 如果currentPlayer.transistionToNextPlayer == false，则说明该交互窗口已经被关闭了
        while(currentPlayer.transistionToNextPlayer == false){
            yield return null;
        }

        //玩家做完了决策
        //检测玩家是否弃牌
        if(currentPlayer.Stat == 0){
            //玩家弃牌, 同时获取到下一个玩家
            sharedInfo.ActivePlayers.Delete(currentPlayer);
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

    public List<Player> CalculateWinner(){
        //获胜者
        List<Player> winners = new List<Player>();
        //缓存玩家手牌
        List<Card> playerBestCards;

        Node<Player> activePlayerNode = sharedInfo.ActivePlayers.FirstNode;
        currentPlayer = activePlayerNode.Item;

        //计算当前玩家牌型，playerBestCards用于缓存目前的五张牌
        CardsValue maxCardsValue = currentPlayer.CountHandValue(publicCard.publicCards, out playerBestCards);
        //玩家操作，更新目前最好的牌型和牌
        currentPlayer.SetBestValuedCards(maxCardsValue, playerBestCards);
        //添加胜利玩家
        winners.Add(currentPlayer);

        activePlayerNode = activePlayerNode.Next;

        while(activePlayerNode != sharedInfo.ActivePlayers.FirstNode){

            currentPlayer = activePlayerNode.Item;

            
            maxCardsValue = currentPlayer.CountHandValue(publicCard.publicCards, out playerBestCards);

            List<Card> tempBestCards;
            CardsValue tempMaxCardsValue = winners[winners.Count-1].CountHandValue(publicCard.publicCards, out tempBestCards);

            if(maxCardsValue > tempMaxCardsValue){
                winners.Clear();
                currentPlayer.SetBestValuedCards(maxCardsValue, playerBestCards);
                winners.Add(currentPlayer);
            }
            else if(maxCardsValue == tempMaxCardsValue){
                currentPlayer.SetBestValuedCards(maxCardsValue, playerBestCards);
                winners.Add(currentPlayer);
            }

            activePlayerNode = activePlayerNode.Next;
        }

        //此时winners保存了所有胜者（可能有多个）, 每个胜者保存了最好牌型和价值

        return winners;
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
