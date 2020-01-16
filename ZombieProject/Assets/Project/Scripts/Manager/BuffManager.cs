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
    END
}
public class BuffManager : Singleton<BuffManager>
{
    public bool m_IsCreateXML = false;
    public bool m_IsParsing = false;
    Dictionary<BUFF_TYPE, Buff> m_BuffTable = new Dictionary<BUFF_TYPE, Buff>();
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
                default:
                    Debug.LogError("Buff Parse Fail BuffType 확인할 것 : " + bufftype);
                    continue;
            }
            buff.m_DurationTime = float.Parse(node.SelectSingleNode("DurationTime").InnerText);
            buff.m_TickTime = int.Parse(node.SelectSingleNode("TickTime").InnerText);
            m_BuffTable.Add(bufftype, buff);
        }

        m_IsParsing = true;
    }

    public Buff GetBuff(BUFF_TYPE _bufftype)
    {
        Buff buff;
        if (!m_BuffTable.TryGetValue(_bufftype, out buff))
        {
            Debug.LogError("그런 버프 없음");
        }
        return buff;
    }

    public void SetStat(STAT _stat)
    {
        Buff buff;
        foreach(BUFF_TYPE i in System.Enum.GetValues(typeof(BUFF_TYPE)))
        {
            if (m_BuffTable.TryGetValue(i, out buff))
            {
                buff.SetStat(_stat);
            }
        }
    }
}