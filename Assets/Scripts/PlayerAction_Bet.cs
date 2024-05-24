using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAction_Bet : MonoBehaviour
{
    private Button betButton;
    [SerializeField] private TMPro.TextMeshProUGUI betText;
    [SerializeField] private PlayerControl playerControl;
    private void Awake(){
        betButton = GetComponent<Button>();
        betButton.onClick.AddListener(OnBetButtonClick);
    }
    private void OnBetButtonClick(){
        float betAmount = float.Parse(betText.text);
        playerControl.Player_Bet(betAmount);
    }
}
