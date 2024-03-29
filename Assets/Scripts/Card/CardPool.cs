using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public static class CardPool
{
    public static CardInfo[] cards = new CardInfo[52];
    public static void Init()
    {
        for(int i=0; i < cards.Length; i++)
        {
            cards[i].type = i / 13;
            cards[i].rank = i % 13;
            cards[i].isVisited = false;
        }
    }
    public static void Refresh()
    {
        for(int i=0;i<cards.Length; i++)
        {
            cards[i].isVisited = false;
        }
    }
    public static GameObject CreatePokerRandom()
    {
        //找到CardInfo索引
        bool isVisted = false;
        int randomNum = 0;
        do
        {
            randomNum = UnityEngine.Random.Range(0, cards.Length);
            isVisted = cards[randomNum].isVisited == true;
        } while (isVisted == true);

        cards[randomNum].isVisited = true;

        // 从文件中加载预制体
        string prefabPath = GetCardPrefabFilePath(cards[randomNum]);
        GameObject pokerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

        if (pokerPrefab == null)
        {
            Debug.LogError("Failed to load PokerPrefab from path: " + prefabPath);
            return null;
        }

        // 实例化预制体
        GameObject pokerGameObject = GameObject.Instantiate(pokerPrefab);

        // 挂载Poker脚本并设置属性
        Poker pokerComponent = pokerGameObject.AddComponent<Poker>();
        pokerComponent.rank = cards[randomNum].rank;
        pokerComponent.type = cards[randomNum].type;

        return pokerGameObject;
    }
    private static string GetCardPrefabFilePath(CardInfo cardInfo)
    {
        string path = "Assets/Prefabs/Card";
        switch (cardInfo.type)
        {
            case 0:
                path += "/Clubs/Club_";
                break;
            case 1:
                path += "/Diamonds/Diamond_";
                break;
            case 2:
                path += "/Hearts/Heart_";
                break;
            case 3:
                path += "/Spades/Spade_";
                break;
            default:
                Debug.LogError("Wrong index");
                break;
        }
        path += (cardInfo.rank + 1).ToString() + ".prefab";
        return path;
    }
}

public struct CardInfo
{
    public int rank;
    public int type;    //0: Club, 1: Diamond, 2: Heart, 3: Spade
    public bool isVisited;
}