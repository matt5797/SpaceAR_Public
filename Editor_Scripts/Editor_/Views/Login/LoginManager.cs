using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

using SpaceAR.Core.AWS;

//namespace SpaceAR.Editor.LoginManager
namespace SpaceAR.Editor.Views.Login
{
    public class LoginManager : MonoBehaviour
    {
        public TMP_InputField emailField;
        public TMP_InputField passwordField;
        public Button loginButton;

        string email;
        string password;

        public static bool loginResult;

        public async void OnLoginButton()
        {
            email = emailField.text;
            password = passwordField.text;

            Debug.Log($"email : {email}");
            Debug.Log($"password : {password}");

            loginResult = await AWSManager.Instance.Login(email, password);
            Debug.Log($"{email} Login Done : {loginResult}");
        }
    }

}
