using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NameRegister : MonoBehaviour
{
    public string playerName;
    [SerializeField] TMP_InputField inputField;

    public void RegisterName(){
        Debug.Log(inputField.text);
        playerName = inputField.text;
    }

    public void ShowNameRegisterPanel(){
        gameObject.SetActive(true);
    }

    public void HideNameRegisterPanel(){
        gameObject.SetActive(false);
    }
}
