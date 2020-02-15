using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : BaseUI
{
    Coroutine m_DamagedCoroutine;

    [SerializeField]
    static Image m_DeathPanel;

    [SerializeField]
    RawImage m_DamagedImage;

    [SerializeField]
    TwinkleTextUI m_WaringText;

    [SerializeField]
    TMPro.TextMeshProUGUI m_WaveText;

    [SerializeField]
    TMPro.TextMeshProUGUI m_CurHPText;

    [SerializeField]
    TMPro.TextMeshProUGUI m_MaxHPText;

    [SerializeField]
    TMPro.TextMeshProUGUI m_CountDown;

    [SerializeField]
    TMPro.TextMeshProUGUI m_MoneyText;

    [SerializeField]
    TMPro.TextMeshProUGUI m_InfoText;

    [SerializeField]
    WeaponStatUI m_WeaponStatUI;


    static Dictionary<SHOP_SORT, NpcShopButton> m_DicNPCButton = new Dictionary<SHOP_SORT, NpcShopButton>();
    static Dictionary<ITEM_SLOT_SORT, BattleItemSlotButton> m_ItemSlots = new Dictionary<ITEM_SLOT_SORT, BattleItemSlotButton>();

    public static InputContoller m_InputController { private set; get; }

    public Image m_HpImage;

    public Coroutine m_HpDownCoroutine;

    public TMPro.TextMeshProUGUI[] m_ListBuffText;
    public TMPro.TextMeshProUGUI[] m_ListDeBuffText;

    GameObject m_SupplyAmmoIcon;
    GameObject m_UpgradeWeaponRangeIcon;
    GameObject m_UpgradeWeaponAttackIcon;
    GameObject m_UpgradeWeaponAttackSpeedIcon;


    private void Awake()
    {
        m_InputController = GetComponentInChildren<InputContoller>();
        m_DeathPanel = transform.Find("DeathPanel").GetComponent<Image>();

        m_InfoText.text = " ";
    }

    public IEnumerator Start()
    {
        while (PlayerManager.Instance.m_Player == null)
        {
            yield return null;
        }

        while (PlayerManager.Instance.m_Player.m_Stat == null)
        {
            yield return null;
        }

        m_CurHPText.text = PlayerManager.Instance.m_Player.m_Stat.CurHP.ToString();
        m_MaxHPText.text = PlayerManager.Instance.m_Player.m_Stat.MaxHP.ToString();
    }

    public void  InsertNPCUpgradeButton(NpcShopButton _button)
    {
        if(_button == null)
        {
            Debug.LogError("Button 이 없어 ");
            return;
        }

        if (!m_DicNPCButton.ContainsKey(_button.m_slotType))
        {
            m_DicNPCButton.Add(_button.m_slotType, _button);
        }
    }

    private void OnEnable()
    {
        Debug.Log("Battle UI 불러옴");
        m_DamagedImage.gameObject.SetActive(false);
        m_DeathPanel.gameObject.SetActive(false);
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

        int hurtSoundIdx = Random.Range((int)UI_SOUND.HURT1, (int)UI_SOUND.HURT3 + 1);

        SoundManager.Instance.OneShotPlay((UI_SOUND)hurtSoundIdx);

        CameraManager.Instance.ShakeCamera(0.4f, 0.1f);
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

        m_SupplyAmmoIcon = transform.Find("NpcShop").GetChild(0).gameObject;
        m_UpgradeWeaponRangeIcon = transform.Find("NpcShop").GetChild(1).gameObject;
        m_UpgradeWeaponAttackIcon = transform.Find("NpcShop").GetChild(2).gameObject;
        m_UpgradeWeaponAttackSpeedIcon = transform.Find("NpcShop").GetChild(3).gameObject;

        InsertNPCUpgradeButton(m_SupplyAmmoIcon.GetComponent<NpcShopButton>());
        InsertNPCUpgradeButton(m_UpgradeWeaponRangeIcon.GetComponent<NpcShopButton>());
        InsertNPCUpgradeButton(m_UpgradeWeaponAttackIcon.GetComponent<NpcShopButton>());
        InsertNPCUpgradeButton(m_UpgradeWeaponAttackSpeedIcon.GetComponent<NpcShopButton>());
        
        m_SupplyAmmoIcon.SetActive(false);
        m_UpgradeWeaponRangeIcon.SetActive(false);
        m_UpgradeWeaponAttackIcon.SetActive(false);
        m_UpgradeWeaponAttackSpeedIcon.SetActive(false);


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
        StopCoroutine(CountDown_C());
        m_HpImage.fillAmount = 1f;
        m_CurHPText.text = "100";
    }

    public static BattleItemSlotButton GetItemSlot(ITEM_SLOT_SORT _itemSlot)
    {
        return m_ItemSlots[_itemSlot];
    }

    public IEnumerator HpChange()
    {
        if (PlayerManager.Instance.m_Player == null) yield break;
        if (PlayerManager.Instance.m_Player.m_Stat == null) yield break;

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

    // 좀비가 몰려옵니다! 메세지 출력해줌.
    public void PlayWaringText()
    {
        m_WaringText.gameObject.SetActive(true);
        m_WaringText.StartTextUITwinkle();
    }

    public static void SetDeathPanelActive(bool _is)
    {

        m_DeathPanel.gameObject.SetActive(_is);
    }

    public void ChangeWaveAction(int _wave)
    {
        PlayWaringText();
        m_WaveText.text = "Wave " + _wave.ToString();
    }

    public void EndWaveAction()
    {
        m_CountDown.text = "생 존";
    }

    // 상단에 있는 노란색 글씨의 인포 메세지 셋팅
    public void PlayInfoMessage(string _str)
    {
        m_InfoText.text = _str;
    }

    public void NpcCollision(bool _is)
    {
        m_SupplyAmmoIcon.SetActive(_is);
        m_UpgradeWeaponRangeIcon.SetActive(_is);
        m_UpgradeWeaponAttackIcon.SetActive(_is);
        m_UpgradeWeaponAttackSpeedIcon.SetActive(_is);

       SetUpgradeItem(PlayerManager.Instance.m_CurrentEquipedItemObject);
    }

    public static void SetUpgradeItem(ItemObject _object)
    {
        if (_object == null) return;

        foreach (var npcButton in m_DicNPCButton.Values)
        {
            npcButton.SetUpgradeWeapon(_object);
        }
    }
    public void UpdateMoney(int _money)
    {
        m_MoneyText.text = "$ " + _money.ToString();
    }

    public void UpdateCount(ITEM_SLOT_SORT _sort, short _acc)
    {
        BattleItemSlotButton button = GetItemSlot(_sort);

        if (!button) return;
        if (button.gameObject.activeSelf == false) return;
        
        GetItemSlot(_sort).plusItemStackCount(_acc);
    }

    public void UpdateWeapnStatUI(ItemObject _object)
    {
        if( _object == null)
        {
            Debug.LogError("무기가 없는대 뭘 업그레이드 할려 하나");
            return;
        }

        if (m_WeaponStatUI == null) return;

        m_WeaponStatUI.SetWeaponStat(_object);
    }
}
