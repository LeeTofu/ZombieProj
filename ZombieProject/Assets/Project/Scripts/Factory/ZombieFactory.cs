using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Zombie 생성하는 객체

public class ZombieFactory : ObjectFactory
{
    private GameObject[] m_ZombieModelPrefabs = new GameObject[4];
    GameObject m_MovingObejctPrefabs;

    public override void Initialize()
    {
        m_MovingObejctPrefabs = Resources.Load<GameObject>("Prefabs/Zombies/Zombie");
        m_ZombieModelPrefabs = Resources.LoadAll<GameObject>("Prefabs/Zombies/Models/Normal");

        if (m_MovingObejctPrefabs)
        {
            Debug.Log("m_MovingObejctPrefabs Load Success");
        }

        if (m_ZombieModelPrefabs.Length != 0)
        {
            Debug.Log("m_ZombieModelPrefabs Load Success");
        }
    }

    public override MovingObject CreateObject(Vector3 _pos, Quaternion _quat)
    {
        // 풀링 필요.. 걍 지금은 대충 생성

        GameObject zombieModel = Instantiate(
            m_ZombieModelPrefabs[Random.Range(0, m_ZombieModelPrefabs.Length)],
            _pos,
            _quat);

        GameObject zombieGameObject = Instantiate(
            m_MovingObejctPrefabs, 
            _pos + Vector3.forward * Random.Range(-5.5f, 5.5f),
            /*m_ZombieCreateZone.transform.position + Vector3.forward * Random.Range(-5.5f, 5.5f)*/
            _quat);

        zombieModel.transform.SetParent(zombieGameObject.transform);
        zombieModel.transform.localPosition = Vector3.zero;
        zombieModel.transform.localRotation = Quaternion.identity;
        zombieModel.transform.localScale = Vector3.one;

        MovingObject newZombie = zombieGameObject.GetComponent<MovingObject>();
        newZombie.Initialize(zombieModel, null);

        Debug.Log("Zombie Create Success");

        return newZombie;
    }

 
}
