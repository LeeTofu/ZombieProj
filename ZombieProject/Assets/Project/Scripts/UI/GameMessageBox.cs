using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMessageBox : MonoBehaviour
{
    [SerializeField]
    TMPro.TextMeshProUGUI m_Text;

    public void SetString(string _str)
    {
        m_Text.text = _str;
    }


    public void ClickButton()
    {
        gameObject.SetActive(false);
    }

}
