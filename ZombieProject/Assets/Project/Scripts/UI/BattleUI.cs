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

    static Dictionary<ITEM_SLOT_SORT, BattleItemSlotButton> m_ItemSlots = new Dictionary<ITEM_SLOT_SORT, BattleItemSlotButton>();

    public static InputContoller m_InputController { private set; get; }

    private void Awake()
    {
        m_InputController = GetComponentInChildren<InputContoller>();
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
        BattleItemSlotButton[] buttons = GetComponentsInChildren<BattleItemSlotButton>();
        
        for (int i = 0; i  < buttons.Length; i++)
        {
            ITEM_SLOT_SORT type = buttons[i].m_slotType;

            if (type == ITEM_SLOT_SORT.END || type == ITEM_SLOT_SORT.NONE) continue;

            if (!m_ItemSlots.ContainsKey(type))
            {
                var item = InvenManager.Instance.GetEquipedItemSlot(type);

                buttons[i].Init(PlayerManager.Instance.m_Player, item);

                m_ItemSlots.Add(type, buttons[i]);
                if (type != ITEM_SLOT_SORT.MAIN)
                {
                    m_ItemSlots[type].GetComponent<RectTransform>().localScale *= 0.85f;
                }
            }
          else
            {
                var item = InvenManager.Instance.GetEquipedItemSlot(type);
                 m_ItemSlots[type].Init(PlayerManager.Instance.m_Player, item);
            }
        }

    }

    public override void DeleteUI()
    {

    }

    public static BattleItemSlotButton GetItemSlot(ITEM_SLOT_SORT _itemSlot )
    {
        return m_ItemSlots[_itemSlot];
    }



}
