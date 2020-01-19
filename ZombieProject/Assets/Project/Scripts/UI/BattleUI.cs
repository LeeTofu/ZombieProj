using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : BaseUI
{
    [SerializeField]
    TwinkleTextUI m_WaringText;

    [SerializeField]
    TMPro.TextMeshProUGUI m_CountDown;

    [SerializeField]
    TMPro.TextMeshProUGUI m_MoneyText;

    [SerializeField]
    TMPro.TextMeshProUGUI m_PartsText;

    static Dictionary<ITEM_SLOT_SORT, BattleItemSlotButton> m_ItemSlots = new Dictionary<ITEM_SLOT_SORT, BattleItemSlotButton>();

    public static InputContoller m_InputController { private set; get; }

    public Image m_HpImage;

    public Coroutine m_HpDownCoroutine;

    public Image[] m_ListBuffImage;
    public Image[] m_ListDeBuffImage;

    private void Awake()
    {
        m_InputController = GetComponentInChildren<InputContoller>();
    }

    private void Start()
    {
        Debug.Log("Battle UI 불러옴");
        StartCoroutine(CountDown_C());
        m_WaringText.gameObject.SetActive(false);
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
        AddPlayerAction();
    }

    private void AddPlayerAction()
    {
        PlayerManager.Instance.m_Player.m_Stat.AddPropertyChangeAction(() =>
        {
            if (m_HpDownCoroutine != null)
                StopCoroutine(m_HpDownCoroutine);
            m_HpDownCoroutine = StartCoroutine(HpDown());
        });
        PlayerManager.Instance.m_Player.AddBuffFunction(() =>
        {
            List<Buff> listBuff = PlayerManager.Instance.m_Player.GetListBuff();
            for (int i = 0; i < listBuff.Count; i++)
            {
                m_ListBuffImage[i].enabled = true;
                m_ListBuffImage[i].sprite = Resources.Load(listBuff[i].m_ImagePath, typeof(Sprite)) as Sprite;
            }
        });
        PlayerManager.Instance.m_Player.AddDeBuffFunction(() =>
        {
            List<Buff> listDeBuff = PlayerManager.Instance.m_Player.GetListDeBuff();
            for (int i = 0; i < listDeBuff.Count; i++)
            {
                m_ListDeBuffImage[i].enabled = true;
                m_ListDeBuffImage[i].sprite = Resources.Load(listDeBuff[i].m_ImagePath, typeof(Sprite)) as Sprite;
            }
        });
    }

    public override void InitializeUI()
    {
        BattleItemSlotButton[] buttons = GetComponentsInChildren<BattleItemSlotButton>();
        m_HpImage = transform.Find("HPBar").GetChild(0).GetComponent<Image>();
        m_ListBuffImage = transform.Find("BuffList").GetComponentsInChildren<Image>();
        m_ListDeBuffImage = transform.Find("DeBuffList").GetComponentsInChildren<Image>();

        for (int i = 0; i < buttons.Length; i++)
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

    public static BattleItemSlotButton GetItemSlot(ITEM_SLOT_SORT _itemSlot)
    {
        return m_ItemSlots[_itemSlot];
    }

    public IEnumerator HpDown()
    {
        float CurHpAmount = PlayerManager.Instance.m_Player.m_Stat.CurHP / PlayerManager.Instance.m_Player.m_Stat.MaxHP;
        for (float i = m_HpImage.fillAmount; i > CurHpAmount; i -= 0.01f)
        {
            m_HpImage.fillAmount = i;
            yield return new WaitForSeconds(0.01f);
        }
        m_HpImage.fillAmount = CurHpAmount;
    }
    public void PlayWaringText()
    {
        m_WaringText.gameObject.SetActive(true);
        m_WaringText.StartTextUITwinkle();
    }

   



}
