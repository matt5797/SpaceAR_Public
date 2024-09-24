using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceAR.Core.AWS
{
    public class UserSessionCache
    {
        public string _idToken;
        public string _accessToken;
        public string _refreshToken;
        public string _userId;

        public UserSessionCache() { }

        public UserSessionCache(string idToken, string accessToken, string refreshToken, string userId)
        {
            _idToken = idToken;
            _accessToken = accessToken;
            _refreshToken = refreshToken;
            _userId = userId;
        }

        public string getIdToken()
        {
            return _idToken;
        }

        public string getAccessToken()
        {
            return _accessToken;
        }

        public string getRefreshToken()
        {
            return _refreshToken;
        }

        public string getUserId()
        {
            return _userId;
        }

        public string ToJson()
        {
            return JsonUtility.ToJson(this);
        }

        public void LoadFromJson(string jsonToLoadFrom)
        {
            JsonUtility.FromJsonOverwrite(jsonToLoadFrom, this);
        }

        public string FileNameToUseForData()
        {
            return "bad_data_01.dat";
        }
    }
}