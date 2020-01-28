using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : BaseUI
{
    Coroutine m_DamagedCoroutine;

    [SerializeField]
    RawImage m_DamagedImage;

    [SerializeField]
    TwinkleTextUI m_WaringText;

    [SerializeField]
    TMPro.TextMeshProUGUI m_CurHPText;

    [SerializeField]
    TMPro.TextMeshProUGUI m_MaxHPText;

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

    public TMPro.TextMeshProUGUI[] m_ListBuffText;
    public TMPro.TextMeshProUGUI[] m_ListDeBuffText;

    private void Awake()
    {
        m_InputController = GetComponentInChildren<InputContoller>();
    }

    private void Start()
    {
        Debug.Log("Battle UI 불러옴");
        m_DamagedImage.gameObject.SetActive(false);
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

    public void OnDamagedEffect()
    {
        if (m_DamagedImage == null) return;

        m_DamagedImage.gameObject.SetActive(true);
        m_DamagedImage.color = Color.red;

        if (m_DamagedCoroutine != null)
            StopCoroutine(m_DamagedCoroutine);

        CameraManager.Instance.ShakeCamera(0.5f, 0.2f);
        m_DamagedCoroutine = StartCoroutine(DamageEffect_C(2.0f));
    }

    IEnumerator DamageEffect_C(float _duration)
    {
        float time = 0.0f;
        while (time < _duration)
        {
            
            m_DamagedImage.color = Color.Lerp(m_DamagedImage.color, new Color(1,0,0,0), Time.deltaTime * 2.0f);
            time += Time.deltaTime;
            yield return null;
        }

        m_DamagedImage.gameObject.SetActive(false);
    }




    private void AddPlayerAction()
    {
        PlayerManager.Instance.m_Player.m_Stat.AddPropertyChangeAction(() =>
        {
            if (m_HpDownCoroutine != null)
                StopCoroutine(m_HpDownCoroutine);

            m_HpDownCoroutine = StartCoroutine(HpChange());
        });

        PlayerManager.Instance.m_Player.AddBuffFunction((List<Buff> _listbuff) =>
        {
            int lastnum = 0, num = -1;
            Dictionary<BUFF_TYPE, int> buffNumTable = new Dictionary<BUFF_TYPE, int>();
            for (int i=0; i< m_ListBuffText.Length; i++)
            {
                m_ListBuffText[i].text = null;
            }
            for (int i = 0; i < _listbuff.Count; i++)
            {
                if(!buffNumTable.TryGetValue(_listbuff[i].m_BuffType, out num))
                {
                    buffNumTable.Add(_listbuff[i].m_BuffType, lastnum);
                    m_ListBuffText[lastnum].text = _listbuff[i].m_Text;
                    lastnum++;
                }
            }
        });
        PlayerManager.Instance.m_Player.AddDeBuffFunction((List<Buff> _listdebuff) =>
        {
            int lastnum = 0, num = -1;
            Dictionary<BUFF_TYPE, int> buffNumTable = new Dictionary<BUFF_TYPE, int>();
            for (int i = 0; i < m_ListDeBuffText.Length; i++)
            {
                m_ListDeBuffText[i].text = null;
            }
            for (int i = 0; i < _listdebuff.Count; i++)
            {
                if (!buffNumTable.TryGetValue(_listdebuff[i].m_BuffType, out num))
                {
                    buffNumTable.Add(_listdebuff[i].m_BuffType, lastnum);
                    m_ListDeBuffText[lastnum].text = _listdebuff[i].m_Text;
                    lastnum++;
                }
            }
        });
    }

    public override void InitializeUI()
    {
        BattleItemSlotButton[] buttons = GetComponentsInChildren<BattleItemSlotButton>();
        m_HpImage = transform.Find("HPBar").GetChild(0).GetComponent<Image>();
        m_ListBuffText = transform.Find("BuffList").GetComponentsInChildren<TMPro.TextMeshProUGUI>();
        m_ListDeBuffText = transform.Find("DeBuffList").GetComponentsInChildren<TMPro.TextMeshProUGUI>();

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

    public IEnumerator HpChange()
    {
        m_CurHPText.text = PlayerManager.Instance.m_Player.m_Stat.CurHP.ToString();
        m_MaxHPText.text = PlayerManager.Instance.m_Player.m_Stat.MaxHP.ToString();

        float CurHpAmount = PlayerManager.Instance.m_Player.m_Stat.CurHP / PlayerManager.Instance.m_Player.m_Stat.MaxHP;
        float PrevHpAmount = m_HpImage.fillAmount;
        for (float i = 0f; m_HpImage.fillAmount * 100 != CurHpAmount * 100; i += 0.01f)
        {
            m_HpImage.fillAmount = Mathf.Lerp(PrevHpAmount, CurHpAmount, i);
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
