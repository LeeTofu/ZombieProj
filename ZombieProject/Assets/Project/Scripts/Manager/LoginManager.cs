using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Firebase;
//using Firebase.Database;
//using Firebase.Unity.Editor;

public class USER_DATA
{
    string ID;
    string Password;

    public USER_DATA(string ID, string Password)
    {
        this.ID = ID;
        this.Password = Password;
    }
}


public class LoginManager : Singleton<LoginManager>
{


    public override bool Initialize()
    {
        return true;
    }
    ////DatabaseReference m_Reference;
    ////Firebase.Auth.FirebaseAuth m_Auth;
    ////public override bool Initialize()
    ////{
    ////    m_Auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
    ////    FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://zombieproject-982d9.firebaseio.com/");

    ////    m_Reference = FirebaseDatabase.DefaultInstance.RootReference;

    ////    FirebaseApp.DefaultInstance.SetEditorP12FileName("zombieproject-982d9-596b73dd658f.p12");
    ////    FirebaseApp.DefaultInstance.SetEditorServiceAccountEmail("firebase-adminsdk-go499@zombieproject-982d9.iam.gserviceaccount.com");
    ////    FirebaseApp.DefaultInstance.SetEditorP12Password("notasecret");


    ////    return true;
    //}//


    //private void Start()
    //{

    //}

    //public void WriteNewUser(string uid, string name, string email)
    //{
    //  //  USER_DATA user = new USER_DATA();
    //  //  string json = JsonUtility.ToJson(user);
    //   // m_Reference.Child("user").Child();
    //}


    //public void JointToFireBase(string _email, string _password)
    //{
    //    m_Auth.CreateUserWithEmailAndPasswordAsync(_email, _password).ContinueWith(task => {
    //        if (task.IsCanceled)
    //        {
    //            Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
    //            return;
    //        }
    //        if (task.IsFaulted)
    //        {
    //            Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
    //            return;
    //        }

    //        // Firebase user has been created.
    //        Firebase.Auth.FirebaseUser newUser = task.Result;
    //        Debug.LogFormat("Firebase user created successfully: {0} ({1})",
    //            newUser.DisplayName, newUser.UserId);
    //    });
    //}

    //public void LoginToFireBase(string _email, string _password)
    //{
    //    m_Auth.SignInWithEmailAndPasswordAsync(_email, _password).ContinueWith(task => {
    //        if (task.IsCanceled)
    //        {
    //            Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
    //            return;
    //        }
    //        if (task.IsFaulted)
    //        {
    //            Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
    //            return;
    //        }

    //        Firebase.Auth.FirebaseUser newUser = task.Result;
    //        Debug.LogFormat("User signed in successfully: {0} ({1})",
    //            newUser.DisplayName, newUser.UserId);
    //    });
    //}




}
