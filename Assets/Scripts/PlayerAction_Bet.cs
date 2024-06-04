using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAction_Bet : MonoBehaviour
{
    public Button BetButton{get; private set;}
    [SerializeField] private TMPro.TextMeshProUGUI betText;

    private void Awake(){
        BetButton = GetComponent<Button>();
    }
    public void OnBetButtonClick(PlayerControl playerControl){
        float betAmount = float.Parse(betText.text);
        playerControl.Player_BetServerRpc(betAmount);
    }
}
