using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerVisual : MonoBehaviour
{
    [SerializeField]private Image icon;
    [SerializeField]private TextMeshProUGUI playerName;
    [SerializeField]private TextMeshProUGUI rank;
    [SerializeField]private TextMeshProUGUI money;
    [SerializeField]private TextMeshProUGUI bet;
    [SerializeField]private TextMeshProUGUI stat;


    [SerializeField]private GameObject card1;
    [SerializeField]private GameObject card2;


    [SerializeField]private Player player;

    private void Start() {
        player.OnChangeTotalMoney += UpdateTotalMoney;
        player.OnBet += UpdateBet;
        player.OnChangeStat += UpdateStat;
    }

    private void UpdateTotalMoney(object sender, float e){
        money.text = e.ToString() + " $";
    }

    private void UpdateBet(object sender, float e)
    {
        bet.text = e.ToString() + " $";
    }

    private void UpdateStat(object sender, uint e)
    {
        switch(e){
            case 0:
                stat.text = "Fold";
                break;
            case 1:
                stat.text = "Check";
                break;
            case 2:
                stat.text = "Call";
                break;
            case 3:
                stat.text = "Raise";
                break;
            case 4:
                stat.text = "All-In";
                break;
            default:
                stat.text = "Unknown";
                break;
        }
    }

    private void UpdateCardsSprite(){
        card1.GetComponent<Image>().sprite = player.Cards[0].FindSprite();
        card2.GetComponent<Image>().sprite = player.Cards[1].FindSprite();
    }

    private void HideCards(){
        card1.GetComponent<Image>().sprite = CardFileLoader.LoadFile(54);
        card2.GetComponent<Image>().sprite = CardFileLoader.LoadFile(54);
    }
}
