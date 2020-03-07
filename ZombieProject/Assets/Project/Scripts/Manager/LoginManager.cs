using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using Firebase.Auth;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
//using Firebase;
//using Firebase.Database;
//using Firebase.Unity.Editor;


[System.Serializable]
public class USER_DATA
{
    public string UserName;
    public int MaxWave;

    public USER_DATA(string _name, int _wave)
    {
        this.UserName = _name;
        this.MaxWave = _wave;
    }
}


public class LoginManager : Singleton<LoginManager>
{
    // 인증되었는가?
    public bool m_isAllAutherSuccess { private set; get; }
    public bool m_isGoogleAuther { private set; get; }
    public Firebase.Auth.FirebaseAuth m_FireBaseAuth;
    string m_authCode;

    public string m_UserFireBaseID;

    public override bool Initialize()
    {
        m_isAllAutherSuccess = false;
        m_isGoogleAuther = false;
        return true;
    }

    private void Start()
    {
        
    }

    public void CreateFireBaseID(string _mail, string _password)
    {
       
        m_FireBaseAuth.CreateUserWithEmailAndPasswordAsync(_mail, _password).ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }

            // Firebase user has been created.
            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);
        });
    }


    IEnumerator LoginWithGoogleToFireBase()
    {
        Debug.Log("구글 토큰으로 FireBase 에 접근중");

        m_FireBaseAuth = Firebase.Auth.FirebaseAuth.DefaultInstance;

        while (string.IsNullOrEmpty(((PlayGamesLocalUser)Social.localUser).GetIdToken()) || !Social.localUser.authenticated)
            yield return null;

        string idToken = ((PlayGamesLocalUser)Social.localUser).GetIdToken();
        Debug.Log(string.Format("\nToken:{0}", idToken));

        Credential credential = GoogleAuthProvider.GetCredential(idToken, null);

        m_FireBaseAuth.SignInWithCredentialAsync(credential).ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithCredentialAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
                return;
            }

            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);

            m_UserFireBaseID = newUser.UserId;

            m_isAllAutherSuccess = true;
        });
    }

    private void Update()
    {
        if(m_isGoogleAuther == true)
        {
            StartCoroutine(LoginWithGoogleToFireBase());
            m_isGoogleAuther = false;
        }

        if(m_isAllAutherSuccess)
        {
            Debug.Log("파이어베이스에 등록 후 씬 이동");
            SceneMaster.Instance.LoadScene(GAME_SCENE.MAIN);
            m_isAllAutherSuccess = false;
        }

    }

    public void LoginToGoogle()
    {

        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().RequestIdToken().Build();

        PlayGamesPlatform.InitializeInstance(config);

        PlayGamesPlatform.DebugLogEnabled = true;

        PlayGamesPlatform.Activate();

        if (!Social.localUser.authenticated)
        {
            PlayGamesPlatform.Instance.Authenticate((bool success, string message) =>
            {
                if (success)
                {
                    Debug.Log("구글 로그인 성공");
                    m_isGoogleAuther = true;
                    m_authCode = PlayGamesPlatform.Instance.GetServerAuthCode();
                }
                else
                {
                    Debug.LogError("구글 로그인 Fail");
                    Debug.LogError("Google : " + message);
                }
            });
        }
    }


    public USER_DATA GetUserData()
    {
        if (m_FireBaseAuth == null) return null;

        Firebase.Auth.FirebaseUser user = m_FireBaseAuth.CurrentUser;
        if (user != null)
        {
            string name = Social.localUser.userName;
            string email = user.Email;
            System.Uri photo_url = user.PhotoUrl;

            USER_DATA userdata = new USER_DATA(name, 1);

            return userdata;
        }

        return null;
    }



    public override void DestroyManager()
    {
    }
}
