using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using Firebase.Auth;
//using Firebase;
//using Firebase.Database;
//using Firebase.Unity.Editor;

public class USER_DATA
{
    public string userName;
    public string e_Mail;

    public USER_DATA(string Name, string EMail)
    {
        this.userName = Name;
        this.e_Mail = EMail;
    }
}


public class LoginManager : Singleton<LoginManager>
{
    // 인증되었는가?
    public bool m_isAuther { private set; get; }
    public Firebase.Auth.FirebaseAuth m_FireBaseAuth;

    public override bool Initialize()
    {
       
        m_isAuther = false;
        return true;
    }

    private void Start()
    {
        m_FireBaseAuth = Firebase.Auth.FirebaseAuth.DefaultInstance;
    }

    public void LoginToFireBase(string _mail, string _password)
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


    public void LoginToGoogle()
    {


        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
     .Build();

        PlayGamesPlatform.InitializeInstance(config);

        PlayGamesPlatform.DebugLogEnabled = true;

        PlayGamesPlatform.Activate();

        if (!Social.localUser.authenticated)
        {
            PlayGamesPlatform.Instance.Authenticate((bool success) =>
            {
                if (success)
                {
                    Debug.Log("로그인 성공");
                    m_isAuther = true;

                    SceneMaster.Instance.LoadScene(GAME_SCENE.MAIN);
                }
                else
                {
                    Debug.LogError("Fail 실패낫!! 뎃챠!!");
                    Debug.LogError("Google : ");
                }
            });
        }

    }

    private void Update()
    {
        if (m_isAuther == false) return;
        if (System.String.IsNullOrEmpty(((PlayGamesLocalUser)Social.localUser).GetIdToken())) return;

        Debug.Log("여기까지는 실행온");

        string idToken = ((PlayGamesLocalUser)Social.localUser).GetIdToken();
        string accessToken = null;

        FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;

        Debug.Log("토큰 : " + idToken);


        Credential credential = GoogleAuthProvider.GetCredential(idToken, accessToken);
        auth.SignInWithCredentialAsync(credential).ContinueWith(task => {
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

            FirebaseUser newUser = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);
        });

        Debug.Log("d으효!!");

        m_isAuther = false;
    }

    IEnumerator Login_C()
    {
        while (System.String.IsNullOrEmpty(((PlayGamesLocalUser)Social.localUser).GetIdToken()))
        {
            Debug.Log("ㅇ");
            yield return null;
        }

        Debug.Log("여기까지는 실행온");

        string idToken = ((PlayGamesLocalUser)Social.localUser).GetIdToken();
        string accessToken = null;

        FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;

        Debug.Log("토큰 : " + idToken);


        Credential credential = GoogleAuthProvider.GetCredential(idToken, accessToken);
        auth.SignInWithCredentialAsync(credential).ContinueWith(task => {
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

            FirebaseUser newUser = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);
        });

        Debug.Log("d으효!!");
    }

    public USER_DATA GetUserData()
    {
        if (m_FireBaseAuth == null) return null;
        if (m_isAuther == false) return null;

        Firebase.Auth.FirebaseUser user = m_FireBaseAuth.CurrentUser;
        if (user != null)
        {
            string name = user.DisplayName;
            string email = user.Email;
            System.Uri photo_url = user.PhotoUrl;

            USER_DATA userdata = new USER_DATA(name, email);

            return userdata;
        }

        return null;
    }



    public override void DestroyManager()
    {
    }
}
