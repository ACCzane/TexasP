using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharedInfo
{
    /// <summary>
    /// 所有的Player
    /// </summary>
    private LinkList<Player> players;
    /// <summary>
    /// 还留在牌局的Player
    /// </summary>
    private LinkList<Player> activePlayers;
    private Node<Player> currentPlayerNode;
    private Node<Player> currentActivePlayerNode;
    private Player currentPlayer;
    private Player currentActivePlayer;

    /// <summary>
    /// 所有的Player
    /// </summary>
    public LinkList<Player> Players { get { return players; } }
    /// <summary>
    /// 还留在牌局的Player
    /// </summary>
    public LinkList<Player> ActivePlayers { get { return activePlayers; } }

    public Node<Player> CurrentPlayerNode { get { return currentPlayerNode; } set{currentPlayerNode = value; currentPlayer = value.Item; }}
    public Node<Player> CurrentActivePlayerNode { get { return currentActivePlayerNode; } set { currentActivePlayerNode = value; currentActivePlayer = value.Item; } }
    public Player CurrentPlayer { get { return currentPlayer; } }


    /// <summary>
    /// 一开始所有玩家都在牌局中
    /// </summary>
    /// <param name="_players"></param>
    public SharedInfo(LinkList<Player> _players){
        players = _players.CopySelf();
        activePlayers = _players.CopySelf();
        CurrentPlayerNode = players.FirstNode;
        CurrentActivePlayerNode = activePlayers.FirstNode;
    }
    public Player GetNextPlayer(){
        CurrentPlayerNode = CurrentPlayerNode.Next;
        return CurrentPlayer;
    }
    public Player GetNextActivePlayer(){
        CurrentActivePlayerNode = CurrentActivePlayerNode.Next;
        return currentActivePlayer;
    }
}
