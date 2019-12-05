using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFactory : ObjectFactory
{
    private GameObject[] m_PlayerModelPrefabs = new GameObject[4];
    GameObject m_MovingObejctPrefabs;

    public override void Initialize(int _maxCount)
    {
        m_MaxCount = _maxCount;
        m_MovingObejctPrefabs = Resources.Load<GameObject>("Prefabs/Players/Player");
        m_PlayerModelPrefabs = Resources.LoadAll<GameObject>("Prefabs/Players/Models/Normal");

        if (m_MovingObejctPrefabs)
        {
            Debug.Log("Player Object Load Success");
        }

        if (m_PlayerModelPrefabs.Length != 0)
        {
            Debug.Log("Player Model Load Success");
        }
    }

    public override MovingObject CreateObject(Vector3 _pos, Quaternion _quat)
    {
        MovingObject newPlayer = null;
        // 풀링 필요.. 걍 지금은 대충 생성
        if (m_MaxCount < m_ListSleepingMovingObject.Count)
        {
            newPlayer = PopObject(_pos, _quat);
        }

        if (newPlayer != null) return newPlayer;

        GameObject playerModel = Instantiate(
            m_PlayerModelPrefabs[Random.Range(0, m_PlayerModelPrefabs.Length)],
            _pos,
            _quat);

        GameObject playerGameObject = Instantiate(
            m_MovingObejctPrefabs,
            _pos,
            /*m_ZombieCreateZone.transform.position + Vector3.forward * Random.Range(-5.5f, 5.5f)*/
            _quat);

        playerModel.transform.SetParent(playerGameObject.transform);
        playerModel.transform.localPosition = Vector3.zero;
        playerModel.transform.localRotation = Quaternion.identity;

         newPlayer = playerGameObject.GetComponent<PlayerObject>();

        if(!newPlayer)
        {
            Debug.Log("Player Load Fail");
            return null;
        }


        PushObject(newPlayer);
        newPlayer.Initialize(playerModel, null);
        newPlayer.SetFactory(this);

        

        Debug.Log("Player Create Success");

        return newPlayer;

    }
}
