using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poker : MonoBehaviour
{
    public enum CardPos
    {
        Private,
        Public,
        Abandoned
    }

    public CardPos cardPos;

    public int rank;
    public int type;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        string name = gameObject.name;

        if (name.Contains("Club")){ type = 0; }
        else if (name.Contains("Diamond")) { type = 1; }
        else if (name.Contains("Heart")) { type = 2; }
        else if (name.Contains("Spade")) { type = 3; }

        if (name.Contains("A")) { rank = 0; }
        else if (name.Contains("2")) {  rank = 1; }
        else if (name.Contains("3")) {  rank = 2; }
        else if (name.Contains("4")) {  rank = 3; }
        else if (name.Contains("5")) {  rank = 4; }
        else if (name.Contains("6")) {  rank = 5; }
        else if (name.Contains("7")) { rank = 6; }
        else if(name.Contains("8")) {  rank = 7; }
        else if (name.Contains("9")) { rank = 8; }
        else if (name.Contains("10")) {  rank = 9; }
        else if (name.Contains("J")) { rank=10; }
        else if (name.Contains("Q")) { rank = 11; }
        else if (name.Contains("K")) { rank = 12; }
    }
}
