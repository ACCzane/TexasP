using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PoolAmount : MonoBehaviour
{
    [SerializeField]private TMPro.TextMeshProUGUI textMesh;
    //private Coroutine scrollCoroutine;
    private void Start()
    {
        textMesh.text = "0.0";
    }
    
    public void SetAmount(float amount){
        textMesh.text = amount.ToString("F1");
    }
}
