using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using UnityEngine.UI;


public class PlayerManager : Singleton<PlayerManager>
{
    public struct PlayerInfo
    {
        public int PlayerID;
        public int Lv;
        public int Money;
        public int Parts;
        public int exp;
        public string playerName;
    }

    public Item m_MainItem;
    public Item m_SecondaryItem;


    [SerializeField]
    private GameObject m_PlayerHeadModelPrefabs;

    [SerializeField]
    private GameObject m_PlayerBodyModelPrefabs;

    public MovingObject m_Player;

    // 플레이어를 도울 용병등등... 드론..? 용병?
    public List<MovingObject> m_PlayableObject = new List<MovingObject>();

    private ObjectFactory m_PlayerFactory;

    [HideInInspector]
    public GameObject m_PlayerCreateZone;

    public PlayerInfo m_PlayerInfo;


    public void LoadPlayerInfo()
    {
        TextAsset playerInvenList = (TextAsset)Resources.Load("Data/Player/PlayerInfo");
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(playerInvenList.text);

        XmlNodeList all_nodes = xmlDoc.SelectNodes("Root/text");

        foreach (XmlNode node in all_nodes)
        {
            int playerID = int.Parse(node.SelectSingleNode("PlayerID").InnerText);
            int Lv = int.Parse(node.SelectSingleNode("Level").InnerText);
            int Money = int.Parse(node.SelectSingleNode("Money").InnerText);
            int Parts = int.Parse(node.SelectSingleNode("Parts").InnerText);
            int exp = int.Parse(node.SelectSingleNode("Exp").InnerText);
            string playerName = node.SelectSingleNode("PlayerName").InnerText;

            m_PlayerInfo.PlayerID = playerID;
            m_PlayerInfo.Lv = Lv;
            m_PlayerInfo.Money = Money;
            m_PlayerInfo.Parts = Parts;
            m_PlayerInfo.exp = exp;
            m_PlayerInfo.playerName = playerName;
        }
    }

    void Start()
    {
        m_PlayerCreateZone = GameObject.Find("PlayerCreateZone");

        if(m_PlayerCreateZone != null)
            m_PlayerCreateZone.GetComponent<MeshRenderer>().enabled = false;
    }


    public MovingObject CreatePlayer(Vector3 _pos, Quaternion _quat)
    {
        if (m_PlayerCreateZone != null)
            m_PlayerCreateZone.GetComponent<MeshRenderer>().enabled = false;

        // 설정 //
        m_Player = m_PlayerFactory.CreateObject(_pos, _quat);

        m_MainItem = InvenManager.Instance.GetEquipedItemSlot(ITEM_SLOT_SORT.MAIN);
        m_SecondaryItem = InvenManager.Instance.GetEquipedItemSlot(ITEM_SLOT_SORT.SECOND);

        m_Player.SetWeapon(m_MainItem == null ? m_SecondaryItem : m_MainItem);

        return m_Player;
    }

    public override bool Initialize()
    {
        LoadPlayerInfo();

        m_PlayerFactory = gameObject.AddComponent<PlayerFactory>();
        m_PlayerFactory.Initialize(2);

        return true;
    }

}
