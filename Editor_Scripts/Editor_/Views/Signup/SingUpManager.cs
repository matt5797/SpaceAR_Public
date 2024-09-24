using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

using SpaceAR.Core.AWS;

namespace SpaceAR.Editor.Views.Signup
{
    public class SingUpManager : MonoBehaviour
    {
        public TMP_InputField emailField;
        public TMP_InputField passwordField;
        public TMP_InputField usernameField;
        public Button singUpButton;

        string email;
        string password;
        string username;

        public async void OnSingUpButton()
        {
            email = emailField.text;
            password = passwordField.text;
            username = usernameField.text;

            Debug.Log($"email : {email}");
            Debug.Log($"password : {password}");
            Debug.Log($"username : {username}");

            bool res = await AWSManager.Instance.Signup(username, email, password);
            Debug.Log($"{email} SingUp Done : {res}");
        }
    }

}