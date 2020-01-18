using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySceneMain : SceneMain
{

    //public ItemSlot m_SelectedSlot { get; private set; }
    public override bool InitializeScene()
    {
        return true;
    }
    public override bool DeleteScene()
    {
        return true;
    }

    ////public bool SelectItem(ItemSlot _slot)
    ////{
    ////    if (m_SelectedSlot)
    ////    {
    ////        m_SelectedSlot.UnSelectSlot();

    ////        if (_slot == m_SelectedSlot)
    ////        {
    ////            m_SelectedSlot = null;
    ////            return false;
    ////        }
    ////    }

    ////    m_SelectedSlot = _slot;
    ////    m_SelectedSlot.SelectItem();

    ////    return true;
        
    ////}
}
