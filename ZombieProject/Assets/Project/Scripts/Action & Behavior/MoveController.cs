using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveController : MonoBehaviour
{
   public MovingObject m_Character { private set; get; }

    [SerializeField]
    private GameObject m_FirePos;

    Ray m_Ray = new Ray();
    RaycastHit m_RayCastHit = new RaycastHit();


    public void Initialized(MovingObject _player)
    {
        m_Character = _player;
        m_FirePos = m_Character.gameObject;
    }


    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButtonUp(0))
        {
            InputAttack();
        }
#elif UNITY_ANDROID
        if (Input.touchCount > 0)
        {

           Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Ended)
            {
                InputAttack();
            }
        }
#endif

    }

    public void InputAttack()
    {
        Camera cam = Camera.main;

        Vector3 point = new Vector3();
        Vector2 mousePos = new Vector2();
        mousePos = Input.mousePosition;

        m_Ray = cam.ScreenPointToRay(new Vector3(mousePos.x, mousePos.y, cam.nearClipPlane));

       if(Physics.Raycast(m_Ray, out m_RayCastHit, 50.0f, 1 << LayerMask.NameToLayer("Ground")))
        {
            Vector3 hitPoint = m_RayCastHit.point;
            hitPoint.y = m_Character.transform.position.y;

            Attack(m_RayCastHit.point, (hitPoint - m_Character.transform.position).normalized);
        }

    }

    public void Attack(Vector3 _FirePosition, Vector3 _AttackDir )
    {
        m_Character.transform.rotation = Quaternion.LookRotation(_AttackDir, Vector3.up);

        Item currentEquipedItem = InvenManager.Instance.m_EquipedItemSlots[ITEM_SLOT_SORT.MAIN];

        if(currentEquipedItem.m_ItemStat.m_isRayAttack)
        {
            GameObject newBulletObj = Instantiate(Resources.Load<GameObject>("Prefabs/Weapon/Bullet/" + currentEquipedItem.m_ItemStat.m_BulletString));

            Bullet newBullet = newBulletObj.GetComponent<Bullet>();

            if(newBullet)
                newBullet.FireBullet(m_FirePos.transform.position, m_Character.transform.forward, currentEquipedItem.m_ItemStat.m_BulletSpeed);

        }
        else
        {

        }


    }


}
