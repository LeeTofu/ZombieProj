using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Zombie 생성하는 객체

public class ZombieFactory : MonoBehaviour
{
    private List<MovingObject> m_ListZombies = new List<MovingObject>();
    private GameObject[] m_ZombieModelPrefabs = new GameObject[4];

    [HideInInspector]
    public GameObject m_ZombieCreateZone;

    GameObject m_MovingObejctPrefabs;

    public void Initialize()
    {
        m_MovingObejctPrefabs = Resources.Load<GameObject>("Zombies/Zombie");
        m_ZombieModelPrefabs = Resources.LoadAll<GameObject>("Zombies/Models/Normal");

        m_ZombieCreateZone = GameObject.Find("ZombieCreateZone");

        if (m_MovingObejctPrefabs)
        {
            Debug.Log("m_MovingObejctPrefabs Load Success");
        }

        if (m_ZombieModelPrefabs.Length != 0)
        {
            Debug.Log("m_ZombieModelPrefabs Load Success");
        }
    }

    public MovingObject CreateZombie()
    {
        // 풀링 필요.. 걍 지금은 대충 생성

        GameObject zombieModel = Instantiate(m_ZombieModelPrefabs[Random.Range(0, m_ZombieModelPrefabs.Length)], m_ZombieCreateZone.transform.position, Quaternion.identity);
        GameObject zombieGameObject = Instantiate(m_MovingObejctPrefabs, m_ZombieCreateZone.transform.position, Quaternion.identity);

        MovingObject newZombie = zombieGameObject.GetComponent<MovingObject>();
        newZombie.Initialize(zombieModel, null);

        m_ListZombies.Add(newZombie);

        Debug.Log("Zombie Create Success");

        return newZombie;
    }

 
}
