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
        ParsingCheck();

        return true;
    }
    public void ParsingCheck()
    {
        TextAsset textAsset = (TextAsset)Resources.Load("Data/Buff/BuffData");

        if (textAsset == null)
            CreateXML();
        else LoadBuffData();
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
    public void CreateXML()
    {
        if (m_IsCreateXML || m_IsParsing) return;

        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.AppendChild(xmlDocument.CreateXmlDeclaration("1.0", "utf-8", "yes"));

        XmlNode root = xmlDocument.CreateNode(XmlNodeType.Element, "BuffInfo", string.Empty);
        xmlDocument.AppendChild(root);

        XmlNode child = xmlDocument.CreateNode(XmlNodeType.Element, "Buff", string.Empty);
        root.AppendChild(child);

        XmlElement BUFFTYPE = xmlDocument.CreateElement("BUFFTYPE");
        BUFFTYPE.InnerText = "ADRENALINE";
        child.AppendChild(BUFFTYPE);

        XmlElement DurationTime = xmlDocument.CreateElement("DurationTime");
        DurationTime.InnerText = "5";
        child.AppendChild(DurationTime);

        XmlElement TickTime = xmlDocument.CreateElement("TickTime");
        TickTime.InnerText = "1";
        child.AppendChild(TickTime);

        child = xmlDocument.CreateNode(XmlNodeType.Element, "Buff", string.Empty);
        root.AppendChild(child);

        BUFFTYPE = xmlDocument.CreateElement("BUFFTYPE");
        BUFFTYPE.InnerText = "BLESSING";
        child.AppendChild(BUFFTYPE);

        DurationTime = xmlDocument.CreateElement("DurationTime");
        DurationTime.InnerText = "5";
        child.AppendChild(DurationTime);

        TickTime = xmlDocument.CreateElement("TickTime");
        TickTime.InnerText = "1";
        child.AppendChild(TickTime);

        child = xmlDocument.CreateNode(XmlNodeType.Element, "Buff", string.Empty);
        root.AppendChild(child);

        BUFFTYPE = xmlDocument.CreateElement("BUFFTYPE");
        BUFFTYPE.InnerText = "POISON";
        child.AppendChild(BUFFTYPE);

        DurationTime = xmlDocument.CreateElement("DurationTime");
        DurationTime.InnerText = "5";
        child.AppendChild(DurationTime);

        TickTime = xmlDocument.CreateElement("TickTime");
        TickTime.InnerText = "1";
        child.AppendChild(TickTime);

        xmlDocument.Save("Assets/Project/Resources/Data/Buff/BuffData.xml");

        foreach (BUFF_TYPE i in System.Enum.GetValues(typeof(BUFF_TYPE)))
        {
            Buff buff;
            STAT stat = new STAT();
            switch (i)
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
                    Debug.LogError("Buff Parse Fail BuffType 확인할 것 : " + i);
                    continue;
            }
            buff.m_DurationTime = 5f;
            buff.m_TickTime = 1;
            m_BuffTable.Add(i, buff);
        }

        m_IsCreateXML = true;
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