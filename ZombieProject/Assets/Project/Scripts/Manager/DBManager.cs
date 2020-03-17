using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Unity.Editor;
using Firebase.Database;
using Firebase;

public class DBManager : Singleton<DBManager>
{
    FirebaseApp m_FireBaseApp;
    DatabaseReference m_DataBaseReferences;

    int m_UserID;

    bool m_isFirstCheckLoginData = false;

    public override bool Initialize()
    {

        m_FireBaseApp = FirebaseDatabase.DefaultInstance.App;
        m_FireBaseApp.SetEditorDatabaseUrl("https://legendzombie-c224f.firebaseio.com/");

        return true;
    }


    public void GetMaxClearWaveFromDB()
    {
        Debug.Log("db 부르는거 시작중");

        FirebaseDatabase.DefaultInstance
            .GetReference("users").Child(LoginManager.Instance.m_UserFireBaseID) // 읽어올 데이터 이름
            .GetValueAsync().ContinueWith(task =>
            {

                Debug.Log("될까 ");


                if (task.IsFaulted)
                {
                    PlayerManager.Instance.m_MaxClearWave = 1;
                    Debug.Log("db 불러오는거 fail 했다");
                    return;
                }

                if (!task.Result.Exists || task.Result == null)
                {
                    Debug.Log("Snapshot이 없어서 유저 정보를 써야한다...");

                  //  WriteNewUserDataToFireBase();
                    PlayerManager.Instance.m_MaxClearWave = 1;
                    return;
                }

                if (task.IsFaulted)
                {
                    Debug.Log("db 불러오는거 fail 했다");
                    return;
                }
                if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;

                    if (snapshot == null)
                    {
                        Debug.Log("Snapshot이 없어서 유저 정보를 써야한다...");

                      //  WriteNewUserDataToFireBase();
                        PlayerManager.Instance.m_MaxClearWave = 1;
                        return;
                    }

                    foreach (var c in snapshot.Children)
                    {
                        Debug.Log(c.Key);
                       if(c.Key == "MaxWave")
                       {
                            USER_DATA userData = JsonUtility.FromJson<USER_DATA>(snapshot.GetRawJsonValue());
                            PlayerManager.Instance.m_MaxClearWave = userData.MaxWave;
                            break;
                        }
                    }
                }
            });
    }



    public void UpdateUserClearWaveToFireBase(int _clearWave)
    {
        Dictionary<string, object> childUpdates = new Dictionary<string, object>();
        childUpdates["/users/" + LoginManager.Instance.m_UserFireBaseID + "/" + "MaxWave"] = _clearWave;

        m_DataBaseReferences = FirebaseDatabase.DefaultInstance.RootReference;
        m_DataBaseReferences.UpdateChildrenAsync(childUpdates);

        PlayerManager.Instance.m_MaxClearWave = _clearWave;
    }


    public void WriteNewUserDataToFireBase()
    {
        if(LoginManager.Instance.GetUserData() == null)
        {
            Debug.LogError("틀럭 UserID : " + LoginManager.Instance.m_UserFireBaseID);
            return;
        }

        USER_DATA user = new USER_DATA(LoginManager.Instance.GetUserData().UserName, 1);
        string json = JsonUtility.ToJson(user);

        Debug.Log(json);
        Debug.Log(LoginManager.Instance.GetUserData().UserName);
        m_DataBaseReferences = FirebaseDatabase.DefaultInstance.RootReference;

        if (m_DataBaseReferences == null)
        {
            Debug.Log("그런거 없는데?");
            return;
        }

        m_DataBaseReferences.Child("users").Child(LoginManager.Instance.m_UserFireBaseID).SetRawJsonValueAsync(json);   
    }

    public override void DestroyManager()
    {

    }
}
