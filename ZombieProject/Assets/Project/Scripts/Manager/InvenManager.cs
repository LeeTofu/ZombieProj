using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvenManager : Singleton<InvenManager>
{
    MovingObject m_Player;

    public List<ItemSlot> m_ItemInventory = new List<ItemSlot>();
    public ItemSlot m_MainItemSlot { get; private set; }
    public ItemSlot m_SecondItemSlot { get; private set; }
    public ItemSlot m_ThirdItemSlot { get; private set; }
    public ItemSlot m_ForthSlot { get; private set; }


    public override bool Initialize()
    {
        return true;
    }

    void GetItemFromInven()
    {

    }




}

