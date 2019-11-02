using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopSceneMain : SceneMain
{

    ItemSlot m_SelectedSlot;
    public override bool InitializeScene()
    {
        return true;
    }
    public override bool DeleteScene()
    {
        return true;
    }

    public void SelectItem(ItemSlot _slot)
    {
        if (m_SelectedSlot)
            m_SelectedSlot.UnSelectSlot();

        m_SelectedSlot = _slot;
    }

    public void UnSelectItem(ItemSlot _slot)
    {
        m_SelectedSlot = null;
    }

}
