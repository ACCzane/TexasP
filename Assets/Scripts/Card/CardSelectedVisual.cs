using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSelectedVisual : MonoBehaviour
{
    [SerializeField] Vector3 cardDefaultScale;
    [SerializeField] float cardSelectedZoomFactor;
    private Vector3 cardSelectedScale;

    [HideInInspector] public Vector3 cardDefaultPos;
    private Vector3 cardPosOffset = new Vector3(0, 0.2f, -0.5f);
    
    public Poker card;
    private void Awake()
    {
        card = GetComponent<Poker>();
        cardSelectedScale = cardDefaultScale * 1.2f;
    }
    private void Update()
    {
       if(PlayerControl.Instance.currentState == PlayerControl.State.SelectingCard)
       {
            ResetPosNScale();
       }
    }

    private void ResetPosNScale()
    {
        if (PlayerControl.Instance.selectedCard == card)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, cardSelectedScale, Time.deltaTime * 5f);
            if (Vector3.Distance(transform.localScale, cardSelectedScale) < 0.01f)
            {
                transform.localScale = cardSelectedScale;
            }

        }
        else
        {
            transform.localScale = Vector3.Lerp(transform.localScale, cardDefaultScale, Time.deltaTime * 10f);
            if (Vector3.Distance(transform.localScale, cardDefaultScale) < 0.01f)
            {
                transform.localScale = cardDefaultScale;
            }

        }
    }
}
