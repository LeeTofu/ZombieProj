using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public enum BUFF_TYPE
{
    NONE = 0,

    ADRENALINE,
    BLESSING,
    POISON,
    FIRE,

    END
}
public class BuffManager : Singleton<BuffManager>
{
    public bool m_IsCreateXML = false;
    public bool m_IsParsing = false;
    Dictionary<BUFF_TYPE, List<Buff>> m_BuffTable = new Dictionary<BUFF_TYPE, List<Buff>>();
    public override bool Initialize()
    {
        LoadBuffData();
        return true;
    }

    public void LoadBuffData()
    {
        if (m_IsParsing) return;

        TextAsset textAsset = (TextAsset)Resources.Load("Data/Buff/BuffData");
        Debug.Log(textAsset);
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(textAsset.text);

        XmlNodeList nodes = xmlDoc.SelectNodes("BuffInfo/Buff");

        foreach (XmlNode node in nodes)
        {
            Buff buff;
            STAT stat = new STAT();
            BUFF_TYPE bufftype;
            System.Enum.TryParse<BUFF_TYPE>(node.SelectSingleNode("BUFFTYPE").InnerText, out bufftype);

            switch(bufftype)
            {
                case BUFF_TYPE.ADRENALINE:
                    buff = new Adrenaline(stat);
                    break;
                case BUFF_TYPE.BLESSING:
                    buff = new Blessing(stat);
                    break;
                case BUFF_TYPE.POISON:
                    buff = new Poison(stat);
                    break;
                case BUFF_TYPE.FIRE:
                    buff = new Fire(stat);
                    break;
                default:
                    Debug.LogError("Buff Parse Fail BuffType 확인할 것 : " + bufftype);
                    continue;
            }

            
            buff.m_DurationTime = float.Parse(node.SelectSingleNode("DurationTime").InnerText);
            buff.m_TickTime = int.Parse(node.SelectSingleNode("TickTime").InnerText);

            // ================= 추가 ======================
            buff.Attack = float.Parse(node.SelectSingleNode("Attack").InnerText);
            buff.MoveSpeed = float.Parse(node.SelectSingleNode("MoveSpeed").InnerText);
            buff.AttackSpeed = float.Parse(node.SelectSingleNode("AttackSpeed").InnerText);
            buff.m_Level = int.Parse(node.SelectSingleNode("Level").InnerText);

            List<Buff> buffList;

            if(m_BuffTable.TryGetValue(bufftype, out buffList))
            {
                buffList.Add(buff);
            }
            else
            {
                buffList = new List<Buff>();
                buffList.Add(buff);
                m_BuffTable.Add(bufftype, buffList);
            }
            
            // =============================================
        }

        m_IsParsing = true;
    }
    public override void DestroyManager()
    {
    }
    Buff CloneBuff(Buff _buff, ItemStat _stat)
    {
        Buff newBuff = null;
        STAT stat = new STAT();

        switch (_buff.m_BuffType)
        {
            case BUFF_TYPE.ADRENALINE:
                newBuff = new Adrenaline(stat);
                break;
            case BUFF_TYPE.BLESSING:
                newBuff = new Blessing(stat);
                break;
            case BUFF_TYPE.POISON:
                newBuff = new Poison(stat);
                break;
            case BUFF_TYPE.FIRE:
                newBuff = new Fire(stat);
                break;
            default:
                Debug.LogError("Buff Parse Fail BuffType 확인할 것 : " + _buff.m_BuffType);
                break;
        }

        newBuff.m_Level = 0;
        newBuff.m_BuffExitAction = _buff.m_BuffExitAction;
        newBuff.m_DurationTime = _stat.m_Range;
        newBuff.m_TickTime = _stat.m_AttackSpeed;

        newBuff.Attack = _stat.m_AttackPoint;
        newBuff.MoveSpeed = _stat.m_MoveSpeed;
        newBuff.AttackSpeed = _stat.m_AttackSpeed;

        return newBuff;
    }


    Buff CloneBuff(Buff _buff)
    {
        Buff newBuff = null;
        STAT stat = new STAT();

        switch (_buff.m_BuffType)
        {
            case BUFF_TYPE.ADRENALINE:
                newBuff = new Adrenaline(_buff.m_Stat);
                break;
            case BUFF_TYPE.BLESSING:
                newBuff = new Blessing(_buff.m_Stat);
                break;
            case BUFF_TYPE.POISON:
                newBuff = new Poison(_buff.m_Stat);
                break;
            case BUFF_TYPE.FIRE:
                newBuff = new Fire(_buff.m_Stat);
                break;
            default:
                Debug.LogError("Buff Parse Fail BuffType 확인할 것 : " + _buff.m_BuffType);
                break;
        }

        newBuff.m_Level = _buff.m_Level;
        newBuff.m_BuffExitAction = _buff.m_BuffExitAction;
        newBuff.m_DurationTime = _buff.m_DurationTime;
        newBuff.m_TickTime = _buff.m_TickTime;

        newBuff.Attack = _buff.Attack;
        newBuff.MoveSpeed = _buff.MoveSpeed;
        newBuff.AttackSpeed = _buff.AttackSpeed;

        return newBuff;
    }

    // ======================== 추가 ========================
    public void ApplyBuff(BUFF_TYPE _bufftype, MovingObject _object, int _buffLevel )
    {
        if (_object != null)
        {
            Buff buff = GetBuff(_bufftype, _buffLevel);
            buff.SetStat(_object.m_Stat);

            buff = CloneBuff(buff);

            if (buff == null) return;

            if(buff.m_BuffType == BUFF_TYPE.END || buff.m_BuffType == BUFF_TYPE.NONE)
            {
                Debug.LogError("End나 None이 왜");
                return;
            }
            _object.AddBuff(buff);
        }
    }

    public void ApplyBuff(BUFF_TYPE _bufftype, MovingObject _object, ItemStat _itemStat)
    {
        if (_object != null)
        {
            Buff buff = GetBuff(_bufftype, 1);
            buff.SetStat(_object.m_Stat);

            buff = CloneBuff(buff, _itemStat);

            if (buff == null) return;

            if (buff.m_BuffType == BUFF_TYPE.END || buff.m_BuffType == BUFF_TYPE.NONE)
            {
                Debug.LogError("End나 None이 왜");
                return;
            }
            _object.AddBuff(buff);
        }
    }

    public void ApplyBuff(Buff _buff, MovingObject _object)
    {
        if (_object != null)
        {
            _buff.SetStat(_object.m_Stat);
            Buff newBuff = CloneBuff(_buff);

            if (newBuff == null) return;
            _object.AddBuff(newBuff);
        }
    }

    // 버프 레벨1 -> buff가 담긴 리스트의 0번째에 들어있음,
    // 버프 레벨2 -> buff가 담긴 리스트의 1번째에 들어있음,
    // 버프 레벨3 -> buff가 담긴 리스트의 2번째에 들어있음,
    // ...

    public Buff GetItemBuff(BUFF_TYPE _bufftype, ItemStat _itemStat)
    {
        Buff newBuff = GetBuff(_bufftype, 1);
        newBuff = CloneBuff(newBuff, _itemStat);

        return newBuff;
    }

    public Buff GetBuff(BUFF_TYPE _bufftype, int _Level)
    {
        List<Buff> listBuff;

        Debug.Log(_bufftype);

        if (!m_BuffTable.TryGetValue(_bufftype, out listBuff))
        {
            Debug.LogError("그런 버프 없음");
            // ========= 추가 ========
            return null;
            // =======================
        }

        foreach(Buff buff in listBuff)
        {
            if(buff.m_Level == _Level)
            {
                return buff;
            }
        }

        Debug.LogError("그런 버프 없음" + _Level);

        return null;
    }
    // ====================================================

}