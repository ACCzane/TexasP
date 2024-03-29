using System.Collections.Generic;
using UnityEngine;

public class PlayerHand : MonoBehaviour {

    [SerializeField] private CardArrangement hand;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Add_2Card();
        }
    }

    private void Add_2Card()
    {
        for (int i = 0; i < 2; i++)
        {
            GameObject poker = CardPool.CreatePokerRandom();
            poker.transform.SetParent(hand.transform, false);
        }
        hand.RearrangeCard();
    }
}