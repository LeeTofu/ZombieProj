using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUI : BaseUI
{
    [SerializeField]
    TMPro.TextMeshProUGUI m_CountDown;

    [SerializeField]
    TMPro.TextMeshProUGUI m_MoneyText;

    [SerializeField]
    TMPro.TextMeshProUGUI m_PartsText;

    Dictionary<ITEM_SLOT_SORT, BattleItemSlotButton> m_ItemSlots = new Dictionary<ITEM_SLOT_SORT, BattleItemSlotButton>();

    GameObject m_ItemButtonPrefabs;

    [SerializeField]
    RectTransform[] m_ItemSlotTransform;

    private void Awake()
    {
        m_ItemButtonPrefabs = Resources.Load<GameObject>("Prefabs/ItemUI/BattleItemSlotUI");
    }

    private void Start()
    {
        Debug.Log("Battle  UI 불러옴");
        StartCoroutine(CountDown_C());
    }

    IEnumerator CountDown_C()
    {
        m_CountDown.text = 3.ToString();
        yield return new WaitForSeconds(1.0f);
        m_CountDown.text = 2.ToString();
        yield return new WaitForSeconds(1.0f);
        m_CountDown.text = 1.ToString();
        yield return new WaitForSeconds(1.0f);
        m_CountDown.text = "살아남아라!!";
        yield return new WaitForSeconds(1.0f);
        m_CountDown.text = " ";
    }

    public override void InitializeUI()
    {
        for (ITEM_SLOT_SORT i = ITEM_SLOT_SORT.MAIN; i != ITEM_SLOT_SORT.END; i++)
        {
            if (!m_ItemSlots.ContainsKey(i))
            {
                GameObject newObject = Instantiate(m_ItemButtonPrefabs);

                var item = InvenManager.Instance.GetEquipedItemSlot(i);

                BattleItemSlotButton newSlot = newObject.GetComponent<BattleItemSlotButton>();

                if (newSlot == null) continue;

                newSlot.SetItem(item);
                m_ItemSlots.Add(i, newSlot);

                newObject.transform.SetParent(transform);

                if (i != ITEM_SLOT_SORT.MAIN)
                {
                    m_ItemSlots[i].GetComponent<RectTransform>().localScale *= 0.85f;
                }

                m_ItemSlots[i].GetComponent<RectTransform>().localPosition = m_ItemSlotTransform[((int)i - 1)].localPosition;

            }
          else
            {
                var item = InvenManager.Instance.GetEquipedItemSlot(i);
                 m_ItemSlots[i].SetItem(item);
            }
        }
    }

    public override void DeleteUI()
    {

    }
}
