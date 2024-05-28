using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PublicCard : MonoBehaviour
{
    [SerializeField]private Image[] cardImages;
    public List<Card> publicCards{get; private set;}
    private int currentIndex = 0;

    private void Awake(){
        foreach(var item in cardImages){
            item.gameObject.SetActive(false);
        }

        publicCards = new List<Card>();
    }

    public void Initialize3Cards(){
        for(int i=0;i<3;i++){
            publicCards.Add(new Card());
            cardImages[i].gameObject.SetActive(true);

            cardImages[i].sprite = CardFileLoader.LoadFile(54);
        }
    }
    public void Show3Cards(){
        for(;currentIndex<3;currentIndex++){
            cardImages[currentIndex].sprite = publicCards[currentIndex].FindSprite();
        }
    }
    public void AddCard(){
        publicCards.Add(new Card());
        cardImages[currentIndex].gameObject.SetActive(true);
        cardImages[currentIndex].sprite = publicCards[currentIndex].FindSprite();
        currentIndex++;
    }
}
