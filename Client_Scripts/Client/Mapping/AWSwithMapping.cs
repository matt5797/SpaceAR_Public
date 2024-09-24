using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Amazon.Extensions.CognitoAuthentication;
using Amazon.CognitoIdentity;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using System;
using System.Threading.Tasks;
using System.Net;
using System.IO;

using Amazon.S3;
using Amazon.S3.Model;

using Newtonsoft.Json.Linq;

using SpaceAR.Core.Utils;
using Amazon.S3.Transfer;

namespace SpaceAR.Core.AWS
{
    public class AWSwithMapping : MonoBehaviour
    {
        public static AWSwithMapping Instance { get; private set; }

        // the AWS region of where your services live
        public static Amazon.RegionEndpoint Region = Amazon.RegionEndpoint.APNortheast2;

        // In production, should probably keep these in a config file
        const string IdentityPool = "ap-northeast-2:59bf2e52-fee5-403f-9e17-337a0eec2a3f"; //insert your Cognito User Pool ID, found under General Settings
        const string AppClientID = "7t1d9b42vnfbe34smquausctcm"; //insert App client ID, found under App Client Settings
        const string userPoolId = "ap-northeast-2_bBb9Poul0";

        private AmazonCognitoIdentityProviderClient _provider;
        private CognitoAWSCredentials _cognitoAWSCredentials;
        private static string _userid = "";
        private CognitoUser _user;

        List<AttributeType> userAttributes;

        private IAmazonS3 _s3Client;

        private IAmazonS3 S3Client
        {
            get
            {
                if (_s3Client == null)
                {
                    _s3Client = new AmazonS3Client(_cognitoAWSCredentials, Region);
                }
                //test comment
                return _s3Client;
            }
        }

        public string UserId
        {
            get
            {
                return _userid;
            }
        }

        public string Username
        {
            get
            {
                AttributeType username = userAttributes?.Find(x => x.Name == "preferred_username");
                return username?.Value;
            }
        }

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            Debug.Log("AuthenticationManager: Awake");
            _provider = new AmazonCognitoIdentityProviderClient(new Amazon.Runtime.AnonymousAWSCredentials(), Region);
            /*bool successfulRefresh = await RefreshSession();
            if (!successfulRefresh)
            {
                Debug.Log("AuthenticationManager: Failed to refresh session");
            }*/
        }

        private void Start()
        {
            /*if (UserId == "")
            {
                bool res = await Login("matt5796@naver.com", "1q2w3e4r!");
                print($"Done Login: {res}");
            }
            S3TestCode();*/

            /* ???? ????? ????
            var res = await GetAssetPurchaseList();
            print($"result : {res}");
            */

            /* ?? ????? ????
            var res = await GetMapList("7666620e-fd18-427e-810c-e37108094ea6");
            print($"result : {res}");
             */

            //CognitoTestCode();

            //var res = GetWorldList("127.09923230777225", "37.41370554789107", "10");
            //print($"result : {res}");
        }

        #region Cognito

        private async void CognitoTestCode()
        {
            // ???????
            /*bool res = await Signup("???????????", "matt5796@naver.com", "1q2w3e4r!");
            print($"Done Signup: {res}");*/

            // ?ес???
            /*bool res = await Login("matt5796@naver.com", "1q2w3e4r!");
            print($"Done Login: {res}");*/

            // ???? ????
            /*bool res = await RefreshSession();
            print($"Done RefreshSession: {res}");*/

            // ??м█?? ????
            //ResetPassword("???????????");
            //ConfirmForgotPassword("???????????", "631520", "1q2w3e4r!");
        }

        private async void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                print($"Username: {Username}");
            }
        }

        public async Task<bool> RefreshSession()
        {
            DateTime issued = DateTime.Now;
            UserSessionCache userSessionCache = new UserSessionCache();
            //SaveDataManager.LoadJsonData(userSessionCache);
            if (ES3.KeyExists("userSessionCache"))
            {
                userSessionCache = ES3.Load<UserSessionCache>("userSessionCache");
            }

            if (userSessionCache != null && userSessionCache._refreshToken != null && userSessionCache._refreshToken != "")
            {
                try
                {
                    CognitoUserPool userPool = new CognitoUserPool(userPoolId, AppClientID, _provider);

                    // apparently the username field can be left blank for a token refresh request
                    CognitoUser user = new CognitoUser("", AppClientID, userPool, _provider);

                    // The "Refresh token expiration (days)" (Cognito->UserPool->General Settings->App clients->Show Details) is the
                    // amount of time since the last login that you can use the refresh token to get new tokens. After that period the refresh
                    // will fail Using DateTime.Now.AddHours(1) is a workaround for https://github.com/aws/aws-sdk-net-extensions-cognito/issues/24
                    user.SessionTokens = new CognitoUserSession(
                       userSessionCache.getIdToken(),
                       userSessionCache.getAccessToken(),
                       userSessionCache.getRefreshToken(),
                       issued,
                       DateTime.Now.AddDays(30)); // TODO: need to investigate further. 
                                                  // It was my understanding that this should be set to when your refresh token expires...

                    // Attempt refresh token call
                    AuthFlowResponse authFlowResponse = await user.StartWithRefreshTokenAuthAsync(new InitiateRefreshTokenAuthRequest
                    {
                        AuthFlowType = AuthFlowType.REFRESH_TOKEN_AUTH
                    })
                    .ConfigureAwait(false);

                    // Debug.Log("User Access Token after refresh: " + token);
                    Debug.Log("User refresh token successfully updated!");

                    // update session cache
                    UserSessionCache userSessionCacheToUpdate = new UserSessionCache(
                       authFlowResponse.AuthenticationResult.IdToken,
                       authFlowResponse.AuthenticationResult.AccessToken,
                       authFlowResponse.AuthenticationResult.RefreshToken,
                       userSessionCache.getUserId());

                    UnityMainThreadDispatcher.Instance().Enqueue(() => ES3.Save("userSessionCache", userSessionCacheToUpdate));

                    // update credentials with the latest access token
                    _cognitoAWSCredentials = user.GetCognitoAWSCredentials(IdentityPool, Region);

                    _user = user;

                    return true;
                }
                catch (NotAuthorizedException ne)
                {
                    // https://docs.aws.amazon.com/cognito/latest/developerguide/amazon-cognito-user-pools-using-tokens-with-identity-providers.html
                    // refresh tokens will expire - user must login manually every x days (see user pool -> app clients -> details)
                    Debug.Log("NotAuthorizedException: " + ne);
                }
                catch (WebException webEx)
                {
                    // we get a web exception when we cant connect to aws - means we are offline
                    Debug.Log("WebException: " + webEx);
                }
                catch (Exception ex)
                {
                    Debug.Log("Exception: " + ex);
                }
            }
            return false;
        }

        public async Task<bool> Login(string email, string password)
        {
            // Debug.Log("Login: " + email + ", " + password);

            CognitoUserPool userPool = new CognitoUserPool(userPoolId, AppClientID, _provider);
            CognitoUser user = new CognitoUser(email, AppClientID, userPool, _provider);

            InitiateSrpAuthRequest authRequest = new InitiateSrpAuthRequest()
            {
                Password = password
            };

            try
            {
                AuthFlowResponse authFlowResponse = await user.StartWithSrpAuthAsync(authRequest).ConfigureAwait(false);

                _userid = await GetUserIdFromProvider(authFlowResponse.AuthenticationResult.AccessToken);
                // Debug.Log("Users unique ID from cognito: " + _userid);

                UserSessionCache userSessionCache = new UserSessionCache(
                   authFlowResponse.AuthenticationResult.IdToken,
                   authFlowResponse.AuthenticationResult.AccessToken,
                   authFlowResponse.AuthenticationResult.RefreshToken,
                   _userid);

                //SaveDataManager.SaveJsonData(userSessionCache);
                UnityMainThreadDispatcher.Instance().Enqueue(() => ES3.Save("userSessionCache", userSessionCache));

                // This how you get credentials to use for accessing other services.
                // This IdentityPool is your Authorization, so if you tried to access using an
                // IdentityPool that didn't have the policy to access your target AWS service, it would fail.
                _cognitoAWSCredentials = user.GetCognitoAWSCredentials(IdentityPool, Region);

                _user = user;

                GetUserAttributes();

                return true;
            }
            catch (Exception e)
            {
                Debug.Log("Login failed, exception: " + e);
                return false;
            }
        }

        // Limitation note: so this GlobalSignOutAsync signs out the user from ALL devices, and not just the game.
        // So if you had other sessions for your website or app, those would also be killed.  
        // Currently, I don't think there is native support for granular session invalidation without some work arounds.
        public async void SignOut()
        {
            await _user.GlobalSignOutAsync();

            // Important! Make sure to remove the local stored tokens 
            UserSessionCache userSessionCache = new UserSessionCache("", "", "", "");
            //SaveDataManager.SaveJsonData(userSessionCache);
            UnityMainThreadDispatcher.Instance().Enqueue(() => ES3.Save("userSessionCache", userSessionCache));

            Debug.Log("user logged out.");
        }

        public async Task<bool> Signup(string username, string email, string password)
        {
            // Debug.Log("SignUpRequest: " + username + ", " + email + ", " + password);

            SignUpRequest signUpRequest = new SignUpRequest()
            {
                ClientId = AppClientID,
                Username = email,
                Password = password
            };

            // must provide all attributes required by the User Pool that you configured
            List<AttributeType> attributes = new List<AttributeType>()
            {
                new AttributeType()
                {
                    Name = "email", Value = email
                },
                new AttributeType(){
                    Name = "preferred_username", Value = username
                }
            };
            signUpRequest.UserAttributes = attributes;

            try
            {
                SignUpResponse sighupResponse = await _provider.SignUpAsync(signUpRequest);
                Debug.Log("Sign up successful!\n response: " + sighupResponse);
                return true;
            }
            catch (UsernameExistsException e)
            {
                Debug.Log("Sign up failed, UsernameExistsException: " + e);
                return false;
            }
            catch (Exception e)
            {
                Debug.Log("Sign up failed, exception: " + e);
                return false;
            }
        }

        // access to the user's authenticated credentials to be used to call other AWS APIs
        public CognitoAWSCredentials GetCredentials()
        {
            return _cognitoAWSCredentials;
        }

        // access to the user's access token to be used wherever needed - may not need this at all.
        public string GetAccessToken()
        {
            UserSessionCache userSessionCache = new UserSessionCache();
            if (ES3.KeyExists("userSessionCache"))
            {
                userSessionCache = ES3.Load<UserSessionCache>("userSessionCache");
            }
            return userSessionCache.getAccessToken();
        }

        // Make the user's unique id available for GameLift APIs, linking saved data to user, etc
        public string GetUsersId()
        {
            // Debug.Log("GetUserId: [" + _userid + "]");
            if (_userid == null || _userid == "")
            {
                // load userid from cached session 
                UserSessionCache userSessionCache = new UserSessionCache();
                //SaveDataManager.LoadJsonData(userSessionCache);
                if (ES3.KeyExists("userSessionCache"))
                {
                    userSessionCache = ES3.Load<UserSessionCache>("userSessionCache");
                }
                _userid = userSessionCache.getUserId();
            }
            return _userid;
        }

        // we call this once after the user is authenticated, then cache it as part of the session for later retrieval 
        private async Task<string> GetUserIdFromProvider(string accessToken)
        {
            // Debug.Log("Getting user's id...");
            string subId = "";

            Task<GetUserResponse> responseTask =
               _provider.GetUserAsync(new GetUserRequest
               {
                   AccessToken = accessToken
               });

            GetUserResponse responseObject = await responseTask;

            // set the user id
            foreach (var attribute in responseObject.UserAttributes)
            {
                if (attribute.Name == "sub")
                {
                    subId = attribute.Value;
                    break;
                }
            }

            return subId;
        }

        public async void GetUserAttributes()
        {
            if (_user == null)
                return;

            GetUserResponse response = await _provider.GetUserAsync(new GetUserRequest
            {
                AccessToken = _user.SessionTokens.AccessToken
            });

            userAttributes = response.UserAttributes;
        }

        public async void ResetPassword(string username)
        {
            // reset password
            AdminResetUserPasswordRequest request = new AdminResetUserPasswordRequest()
            {
                UserPoolId = userPoolId,
                Username = username
            };
            try
            {
                AdminResetUserPasswordResponse response = await _provider.AdminResetUserPasswordAsync(request);
                Debug.Log("Reset password successful!\n response: " + response);
            }
            catch (Exception e)
            {
                Debug.Log("Reset password failed, exception: " + e);
            }
        }

        public async void ConfirmForgotPassword(string username, string confirmationCode, string password)
        {
            ConfirmForgotPasswordRequest request = new ConfirmForgotPasswordRequest()
            {
                ClientId = AppClientID,
                Username = username,
                ConfirmationCode = confirmationCode,
                Password = password
            };

            try
            {
                ConfirmForgotPasswordResponse response = await _provider.ConfirmForgotPasswordAsync(request);
                Debug.Log("Confirm forgot password successful!\n response: " + response);
            }
            catch (Exception e)
            {
                Debug.Log("Confirm forgot password failed, exception: " + e);
            }
        }

        #endregion

        #region S3

        private async void S3TestCode()
        {
            // ???? ?????? ?ех? ????
            /*string json = await GetJsonObjectAsync("spacear-worlds", "myWorldTest.json");
            Debug.Log("S3TestCode: " + json);

            string path = Path.Combine(Application.persistentDataPath, "myWorldTest.json");
            StreamWriter writer;
            if (!File.Exists(path))
            {
                writer = File.CreateText(path);
            }
            else
            {
                writer = new StreamWriter(path);
            }
            await writer.WriteLineAsync(json);
            writer.Close();
            print("File written to: " + path);*/

            // ???? ?????? ???ех?
            /*bool result = await PutObjectAsync("spacear-worlds", "uploadtest.json");
            print("PutObjectAsync result: " + result);*/

            // ?? ?????? ???ех?
            /*string path = Path.Combine(Application.persistentDataPath, "AreaTargetData");
            await DownloadFileObjectAsync("spacear-maps", path, "c30c012d-7b5d-4b11-bb25-bdb5fd36e0ab/circle_navmesh.glb", () => { print("111111111"); }, (e) => { print(e.Message); });*/
        }

        public async Task GetObjectAsync(string S3BucketName, string fileName)
        {
            string responseBody = "";
            try
            {
                GetObjectRequest request = new GetObjectRequest
                {
                    BucketName = S3BucketName,
                    Key = fileName
                };
                using (GetObjectResponse response = await S3Client.GetObjectAsync(request))
                using (Stream responseStream = response.ResponseStream)
                using (StreamReader reader = new StreamReader(responseStream))
                {
                    string title = response.Metadata["x-amz-meta-title"]; // Assume you have "title" as medata added to the object.
                    string contentType = response.Headers["Content-Type"];
                    Console.WriteLine("Object metadata, Title: {0}", title);
                    Console.WriteLine("Content type: {0}", contentType);

                    responseBody = reader.ReadToEnd(); // Now you process the response body.
                }
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error encountered ***. Message:'{0}' when reading object", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown encountered on server. Message:'{0}' when reading object", e.Message);
            }
        }

        public async Task<bool> PutObjectAsync(string S3BucketName, string fileName)
        {
            try
            {
                Debug.Log(Application.persistentDataPath + Path.DirectorySeparatorChar + fileName);

                string fullpath = Path.Combine(Application.persistentDataPath, fileName);
                FileStream stream = new FileStream(fullpath, FileMode.Open, FileAccess.Read, FileShare.Read);
                //FileStream stream = new FileStream(Application.persistentDataPath + Path.DirectorySeparatorChar + fileName, FileMode.Open, FileAccess.Read, FileShare.Read);

                PutObjectRequest request = new PutObjectRequest
                {
                    BucketName = S3BucketName,
                    Key = fileName,
                    InputStream = stream,
                    //CannedACL = S3CannedACL.PublicRead
                };

                PutObjectResponse response = await S3Client.PutObjectAsync(request);

                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    Debug.Log("PutObjectAsync HttpStatusCode: " + response.HttpStatusCode);
                    return true;
                }
                else
                {
                    Debug.Log("PutObjectAsync HttpStatusCode: " + response.HttpStatusCode);
                    return false;
                }
            }
            catch (AmazonS3Exception e)
            {
                Debug.LogAssertion($"Error encountered ***. Message:'{e.Message}' when reading object");
            }
            catch (Exception e)
            {
                Debug.LogAssertion($"Unknown encountered on server. Message:'{e.Message}' when reading object");
            }
            return false;
        }

        public async Task<string> GetJsonObjectAsync(string S3BucketName, string fileName)
        {
            string responseBody = "";
            try
            {
                GetObjectRequest request = new GetObjectRequest
                {
                    BucketName = S3BucketName,
                    Key = fileName
                };
                using (GetObjectResponse response = await S3Client.GetObjectAsync(request))
                using (Stream responseStream = response.ResponseStream)
                using (StreamReader reader = new StreamReader(responseStream))
                {
                    string title = response.Metadata["x-amz-meta-title"]; // Assume you have "title" as medata added to the object.
                    string contentType = response.Headers["Content-Type"];
                    Console.WriteLine("Object metadata, Title: {0}", title);
                    Console.WriteLine("Content type: {0}", contentType);

                    responseBody = reader.ReadToEnd(); // Now you process the response body.
                }
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error encountered ***. Message:'{0}' when reading object", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown encountered on server. Message:'{0}' when reading object", e.Message);
            }
            return responseBody;
        }

        // ??????, ?????? ????, ???????, ?????? ???, ???м▀? ???
        public async Task DownloadFileObjectAsync(string S3BucketName, string path, string key, Action onComplete = null, Action<Exception> onError = null)
        {
            Byte[] data = null;

            string[] splitKey = key.Split('/');
            string fileName = splitKey[splitKey.Length - 1];
            string folderNmae = key.Substring(0, key.Length - fileName.Length - 1);

            string folderPath = Path.Combine(path, folderNmae);
            string fullPath = Path.Combine(folderPath, fileName);

            try
            {
                GetObjectRequest request = new GetObjectRequest
                {
                    BucketName = S3BucketName,
                    Key = key
                };

                using (GetObjectResponse response = await S3Client.GetObjectAsync(request))
                using (Stream responseStream = response.ResponseStream)
                using (StreamReader reader = new StreamReader(responseStream))
                {
                    using (MemoryStream memory = new MemoryStream())
                    {
                        var buffer = new byte[1024];
                        var bytesRead = 0;

                        while ((bytesRead = reader.BaseStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            memory.Write(buffer, 0, bytesRead);
                        }

                        data = memory.ToArray();
                    }
                }

                Directory.CreateDirectory(folderPath);

                Debug.Log("DownloadFileObjectAsync fullPath: " + fullPath);

                using (FileStream fs = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(data, 0, data.Length);
                }

                if (onComplete != null)
                    onComplete();
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error encountered ***. Message:'{0}' when reading object", e.Message);
                if (onError != null)
                    onError(e);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown encountered on server. Message:'{0}' when reading object", e.Message);
                if (onError != null)
                    onError(e);
            }
        }

        /**
         * @param S3BucketName ??? ???
         * @param path ??????
         * @param s3FolderName S3???????
         * @param ?????? ???
         * @param ???м▀? ???
         */
        public async Task DownloadDirectoryAsync(string S3BucketName, string path, string s3FolderName, Action onComplete = null, Action<Exception> onError = null)
        {
            try
            {
                TransferUtilityDownloadDirectoryRequest request = new TransferUtilityDownloadDirectoryRequest()
                {
                    BucketName = S3BucketName,
                    S3Directory = s3FolderName,
                    LocalDirectory = path,
                };

                TransferUtility transferUtility = new TransferUtility(S3Client);
                transferUtility.DownloadDirectory(request);

                if (onComplete != null)
                    onComplete();
            }
            catch (AmazonS3Exception s3Exception)
            {
                Console.WriteLine(s3Exception.Message,
                                s3Exception.InnerException);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message, exception.InnerException);
            }

        }

        public async Task<Texture2D> GetTextureAsync(string S3BucketName, string fileName)
        {
            Texture2D tex = new Texture2D(2, 2);
            try
            {
                GetObjectRequest request = new GetObjectRequest
                {
                    BucketName = S3BucketName,
                    Key = fileName
                };
                using (GetObjectResponse response = await S3Client.GetObjectAsync(request))
                using (Stream responseStream = response.ResponseStream)
                {
                    byte[] data = null;

                    if (response.ResponseStream != null)
                    {
                        byte[] buffer = new byte[16 * 1024];
                        using (MemoryStream ms = new MemoryStream())
                        {
                            int read;
                            while ((read = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                ms.Write(buffer, 0, read);
                            }
                            data = ms.ToArray();
                        }
                        tex.LoadImage(data);
                    }
                    else
                    {
                        Debug.Log("S3 response is Null");
                    }
                }
            }
            catch (AmazonS3Exception e)
            {
                // If bucket or object does not exist
                Console.WriteLine("Error encountered ***. Message:'{0}' when reading object", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown encountered on server. Message:'{0}' when reading object", e.Message);
            }
            return tex;
        }

        public Texture2D BytesToTexture2D(byte[] imageBytes)
        {
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(imageBytes);
            return tex;
        }

        #endregion

        #region API Gateway

        string BaseAPI = "https://uttj39dyej.execute-api.ap-northeast-2.amazonaws.com/prototype";

        private async void APITestCode()
        {
            // ????? ???? ???? ????? ????????
            /*bool res = await GetUserWorldList();
            print($"GetUserWorldList: {res}");*/
        }

        public async Task<JArray> GetUserWorldList()
        {
            string pathAPI = "/user/world";
            JObject result = null;
            try
            {
                WebRequest request = WebRequest.Create(BaseAPI + pathAPI);
                request.Method = "GET";
                request.ContentType = "application/json";
                request.Headers.Add("userid", UserId);

                using (WebResponse response = await request.GetResponseAsync())
                using (Stream dataStream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(dataStream))
                {
                    result = JObject.Parse(reader.ReadToEnd());
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
            if (result["statusCode"].ToString() == "200")
            {
                return JArray.Parse(result["body"].ToString());
            }
            else
            {
                return null;
            }
        }

        public async Task<JArray> GetWorldThumbnailList(string worldid)
        {
            string pathAPI = "/world/thumbnail";
            JObject result = null;
            try
            {
                WebRequest request = WebRequest.Create(BaseAPI + pathAPI);
                request.Method = "GET";
                request.ContentType = "application/json";
                request.Headers.Add("worldid", worldid);

                using (WebResponse response = await request.GetResponseAsync())
                using (Stream dataStream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(dataStream))
                {
                    result = JObject.Parse(reader.ReadToEnd());
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
            if (result["statusCode"].ToString() == "200")
            {
                return JArray.Parse(result["body"].ToString());
            }
            else
            {
                return null;
            }
        }

        public async Task<JArray> GetAssetPurchaseList()
        {
            string pathAPI = "/asset/purchase";
            JObject result = null;
            try
            {
                WebRequest request = WebRequest.Create(BaseAPI + pathAPI);
                request.Method = "GET";
                request.ContentType = "application/json";

                using (WebResponse response = await request.GetResponseAsync())
                using (Stream dataStream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(dataStream))
                {
                    result = JObject.Parse(reader.ReadToEnd());
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
            if (result["statusCode"].ToString() == "200")
            {
                return JArray.Parse(result["body"].ToString());
            }
            else
            {
                return null;
            }
        }

        public async Task<JArray> GetMapList(string owner)
        {
            string pathAPI = "/map";
            JObject result = null;
            try
            {
                WebRequest request = WebRequest.Create(BaseAPI + pathAPI);
                request.Method = "GET";
                request.ContentType = "application/json";
                request.Headers.Add("owner", owner);

                using (WebResponse response = await request.GetResponseAsync())
                using (Stream dataStream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(dataStream))
                {
                    result = JObject.Parse(reader.ReadToEnd());
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
            if (result["statusCode"].ToString() == "200")
            {
                return JArray.Parse(result["body"].ToString());
            }
            else
            {
                return null;
            }
        }

        public async Task<JArray> GetWorldList(string longitude, string latitude, string radius)
        {
            string pathAPI = "/world";
            JObject result = null;
            try
            {
                WebRequest request = WebRequest.Create(BaseAPI + pathAPI);
                request.Method = "GET";
                request.ContentType = "application/json";
                request.Headers.Add("longitude", longitude);
                request.Headers.Add("latitude", latitude);
                request.Headers.Add("radius", radius);

                using (WebResponse response = await request.GetResponseAsync())
                using (Stream dataStream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(dataStream))
                {
                    result = JObject.Parse(reader.ReadToEnd());
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
            if (result["statusCode"].ToString() == "200")
            {
                return JArray.Parse(result["body"].ToString());
            }
            else
            {
                return null;
            }
        }

        /*public async Task<JArray> PostMap(string name, string owner, bool is_public)
        {
            string pathAPI = "/map";
            JObject result = null;
            try
            {
                WebRequest request = WebRequest.Create(BaseAPI + pathAPI);
                request.Method = "GET";
                request.ContentType = "application/json";
                request.Headers.Add("owner", owner);

                using (WebResponse response = await request.GetResponseAsync())
                using (Stream dataStream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(dataStream))
                {
                    result = JObject.Parse(reader.ReadToEnd());
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
            if (result["statusCode"].ToString() == "200")
            {
                return JArray.Parse(result["body"].ToString());
            }
            else
            {
                return null;
            }
        }*/

        #endregion
    }
}