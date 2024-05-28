using System.Collections;
using Unity.VisualScripting;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class PlayerBetAmount : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Player Player{get; set;}
    private TMPro.TextMeshProUGUI textMesh;
    private float maxBetAmount;
    private float minBetAmount;
    private float minDoubleBetAmount;
    private Coroutine scrollCoroutine;
    private void Awake()
    {
        textMesh = GetComponent<TMPro.TextMeshProUGUI>();
    }

    /// <summary>
    /// 最开始显示最小金额
    /// </summary>
    public void FirstShowAmount(){
        maxBetAmount = Player.Money;
        minBetAmount = Math.Max(Player.Upper_bet - Player.Bet, 0.1f);
        minDoubleBetAmount = Player.Upper_bet * 2 - Player.Bet;
        textMesh.text = minBetAmount.ToString("F1");
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("Enter");
        scrollCoroutine = StartCoroutine(CheckScroll());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log("Exit");
        StopCoroutine(scrollCoroutine);
    }

    private IEnumerator CheckScroll()
    {
        int speed = 1;
        float previousScroll = 0f; // 用于跟踪上一次的滚动方向
        float timeSinceLastScroll = 0f; // 用于计时

        while (true)
        {
            float currentScroll = Input.GetAxis("Mouse ScrollWheel");

            if (currentScroll != 0)
            {
                // 如果连续两次滚动的时间间隔超过0.5秒，重置speed
                if (timeSinceLastScroll > 0.5f)
                {
                    speed = 1;
                }

                // 检查滚动方向是否与上一次相同
                if (Mathf.Sign(currentScroll) == Mathf.Sign(previousScroll) && previousScroll!= 0)
                {
                    // 方向相同，增加speed，但不超过10
                    speed = Mathf.Min(speed + 1, 10);
                }
                else
                {
                    // 方向不同或是超时后的第一次滚动，重置speed
                    speed = 1;
                }

                float scrollAmount = currentScroll * speed;
                float newFloat = float.Parse(textMesh.text) + scrollAmount;
                if (newFloat < minBetAmount)
                {
                    newFloat = minBetAmount;
                }
                if(minBetAmount < newFloat && newFloat < minDoubleBetAmount){
                    if(scrollAmount > 0){
                        newFloat = minDoubleBetAmount;
                    }else if(scrollAmount < 0){
                        newFloat = minBetAmount;
                    }
                }
                if (newFloat > maxBetAmount)
                {
                    newFloat = maxBetAmount;
                }
                textMesh.text = newFloat.ToString("F1");

                // 更新上一次的滚动值和时间
                previousScroll = currentScroll;
                timeSinceLastScroll = 0f; // 重置计时器
            }
            else
            {
                // 没有滚动时，增加计时器的时间
                timeSinceLastScroll += Time.deltaTime;
                
                // 如果超过一定时间没有滚动
                if (timeSinceLastScroll > 0.5f)
                {
                    // 重置speed
                    speed = 1;
                    // 重置上一次的滚动值
                    previousScroll = 0;
                }
            }
            yield return null;
        }
    }
}
