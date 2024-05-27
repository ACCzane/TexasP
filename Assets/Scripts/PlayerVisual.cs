using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerVisual : MonoBehaviour
{
    [SerializeField]private Image icon;
    [SerializeField]private Image infoPanel;
    [SerializeField]private TextMeshProUGUI playerName;
    [SerializeField]private TextMeshProUGUI rank;
    [SerializeField]private TextMeshProUGUI money;
    [SerializeField]private TextMeshProUGUI bet;
    [SerializeField]private TextMeshProUGUI stat;

    private Color controlColor = new Color32(70, 231, 62, 135);
    private Color defaultColor = new Color32(255, 255, 255, 130);
    private Color foldColor = new Color32(255, 20, 20, 130);

    [SerializeField]private GameObject card1;
    [SerializeField]private GameObject card2;


    [SerializeField]private Player player;

    private void Start() {
        player.OnChangeTotalMoney += UpdateTotalMoney;
        player.OnBet += UpdateBet;
        player.OnChangeStat += UpdateStat;
        player.OnUpdateCards += UpdateCardsSprite;
        player.OnControl += UpdateColor;
    }

    private void UpdateCardsSprite(object sender, uint e)
    {
        if(e == 1){
            UpdateCardsSprite();
        }else if(e == 0){
            HideCards();
        }else if(e == 2){
            card1.GetComponent<Image>().sprite = null;
            card2.GetComponent<Image>().sprite = null;
        }
    }

    private void UpdateTotalMoney(object sender, float e){
        money.text = e.ToString("F1") + " $";
    }

    private void UpdateBet(object sender, float e)
    {
        bet.text = e.ToString("F1") + " $";
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
            case 5:
                stat.text = "Hold";
                break;
            default:
                stat.text = "Unknown";
                break;
        }
    }

    /// <summary>
    /// 0: default, 1: control, -1: fold
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void UpdateColor(object sender, int e)
    {
        switch (e){
            case 0:
                infoPanel.color = defaultColor;
                break;
            case 1:
                infoPanel.color = controlColor;
                break;
            case -1:
                infoPanel.color = foldColor;
                break;
            default:
                infoPanel.color = defaultColor;
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
