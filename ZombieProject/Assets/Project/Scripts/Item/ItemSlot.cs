using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public ItemStat m_ItemStat { private set; get; }

    public void Initialize(ItemStat _stat)
    {
        m_ItemStat = _stat;


    }


}
