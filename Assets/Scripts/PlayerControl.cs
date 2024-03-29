using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public static PlayerControl Instance;

    [SerializeField] private CardPlayingArea cardManager;

    public LayerMask cardLayer;
    public LayerMask areaLayer;

    public enum State{
        SelectingCard,
        InfoCard,
        AutoMoveCard,
        Null
    }
    public State currentState;

    public Poker selectedCard;
    private CardArrangement puttableArea;


    private Vector3 mouseAndCardCenterOffset;
    private Vector3 mousePosition;
    private Vector3 cardPrimaryPos;
    private Vector3 cardDestinyPos;

    private float moveDuration = 0.5f;
    private float moveTimeCounter = 0f;


    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        Debug.Log(selectedCard);

        mousePosition = Input.mousePosition;
        mousePosition.z = 10f;

        switch (currentState)
        {
            case State.SelectingCard:
                MouseSelectCard();
                if (selectedCard != null)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        currentState = State.InfoCard;
                    }
                }
                break;
            case State.InfoCard:
                if (Input.GetMouseButtonDown(0))
                {
                    //turn page of the card

                }
                if (Input.GetMouseButtonDown(1))
                {
                    //stop info
                    currentState = State.SelectingCard;
                }
                break;
            case State.AutoMoveCard:
                if(moveTimeCounter < moveDuration)
                {
                    moveTimeCounter += Time.deltaTime;

                    float t = moveTimeCounter / moveDuration;
                    t = Mathf.SmoothStep(0f, 1f, t);
                    selectedCard.transform.position = Vector3.Lerp(cardPrimaryPos, cardDestinyPos, t);
                }
                
                //Card been put automatically
                if(selectedCard.transform.position == cardDestinyPos)
                {
                    moveTimeCounter = 0f;

                    selectedCard = null;
                    currentState = State.SelectingCard;
                }
                break;
            case State.Null:
                break;
        }
#if UNITY_EDITOR
        ///FOR DEBUG
        if (Input.GetKeyDown(KeyCode.Space))
        {

        }
#endif
    }

    private void MouseSelectCard()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, 15, cardLayer))
        {
            if (hit.collider.TryGetComponent(out Poker card))
            {
                if(selectedCard != card)
                {
                    selectedCard = card;
                }
            }
        }
        else
        {
            if(selectedCard != null)
            {
                selectedCard = null;
            }
        }
    }

    public void SwitchOpenAndCloseState()
    {
        if(currentState == State.Null)
        {
            currentState = State.SelectingCard;
        }
        else if(currentState == State.SelectingCard)
        {
            currentState = State.Null;
        }
    }

    private void MoveCard()
    {
        Vector3 newPosition = Camera.main.ScreenToWorldPoint(mousePosition) + mouseAndCardCenterOffset;
        selectedCard.transform.position = new Vector3(newPosition.x, newPosition.y, -0.5f);
    }
}
