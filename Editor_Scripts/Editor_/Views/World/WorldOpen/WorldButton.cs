using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.UI;
using TMPro;
using Battlehub.RTEditor;
using Battlehub.RTCommon;
using System;
using SpaceAR.Core.World;
using SpaceAR.Core.AWS;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Gpm.Ui;
using SpaceAR.Core.Utils.LoadModel;
using SpaceAR.Editor.Views.WorldCreate.Views;
using XLua;
using SpaceAR.Editor.Behavior;
using UnityEngine.Networking;

namespace SpaceAR.Editor.Views.WorldOpen
{
    public class WorldButton : MonoBehaviour
    {
        //public Image thumbnail;
        public WebCacheImage thumbnail;
        public TextMeshProUGUI worldName;
        Button button;

        WorldData _worldData;

        string mapId;
        string mapName;

        string mapKey;

        string nav_modelURL;
        string author_modelURL;

        string baseURL = "https://spacear-maps.s3.ap-northeast-2.amazonaws.com/";

        public WorldData WorldData
        {
            get { return _worldData; }
            set
            {
                _worldData = value;
                SetName();
                LoadThumbnail();
            }
        }

        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(OnClick);
        }

        void SetName()
        {
            worldName.text = _worldData.name;
        }

        async void LoadThumbnail()
        {
            //Debug.Log($"{WorldData.id} {WorldData.name}");
            WorldData.thumbnailList = new List<Texture2D>();
            JArray thumbnailArray = await AWSManager.Instance.GetWorldThumbnailList(WorldData.id);

            // 이미지 없으면 기본이미지 넣어주기
            if (thumbnailArray.Count != 0)
            {
                thumbnail.SetUrl($"https://{thumbnailArray[0]["bucket"].ToString()}.s3.ap-northeast-2.amazonaws.com/{thumbnailArray[0]["key"].ToString()}/1.png");
            }
        }

        async void GetMapKey()
        {


        }

        public void OnClick()
        {
            WorldManager.Instance.WorldId = WorldData.id;

            GetMapKey();
            
            // URL 세팅
            mapId = _worldData.map;
            mapName = _worldData.map_name;


            IWindowManager wm = IOC.Resolve<IWindowManager>();

            wm.Confirmation("World Download", "Confirm to Download World?",
                async (sender, args) =>
                {
                    var res = await AWSManager.Instance.GetMapAreaTarget(mapId);

                    Debug.Log("MapKey Check " + res[0]["name"]);

                    mapKey = res[0]["name"].ToString();

                    Progress<float> progress = new Progress<float>((percent) =>
                    {
                        Debug.Log("Load World :" + percent + "%");
                    });

                    // ==================== 다운로드 ==============
                    // 맵 내려 받기
                    nav_modelURL = baseURL + mapKey + "/" + mapKey + "_navmesh.glb";
                    author_modelURL = baseURL + mapKey + "/" + mapKey + "_authoring.glb";
                    Debug.Log("URL Check : " + nav_modelURL);
                    Debug.Log("URL Check : " + author_modelURL);

                    StartCoroutine(WebCheck(nav_modelURL, author_modelURL));

                    // 월드 내려 받기
                    bool result = await WorldManager.Instance.LoadWorldAsync(progress);

                    WorldManager.Instance.AddExpose();
                    // ============================================

                    if (result)
                    {
                        wm.MessageBox("World Download", "World Download Sucessed", (sender, args) => { });
                        
                        Transform behavior = wm.GetWindow("behavior");
                        if (behavior != null)
                        {
                            BehaviorActionView actionView = behavior.GetComponentInChildren<BehaviorActionView>();
                            if (actionView != null)
                            {
                                actionView.OnBuild();
                            }
                        }
                        CloseWindow();
                    }
                    else
                    {
                        wm.MessageBox("World Download", "World Download Failed", (sender, args) => { });
                    }
                },
                (sender, args) =>
                {
                    //Debug.Log("No click");
                },
                "Yes", "No");
        }

        IEnumerator WebCheck(string nav_url, string author_url)
        {
            UnityWebRequest www = UnityWebRequest.Head(author_url);
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                // 다시 navmesh로 시도
                Debug.Log("authoring Data 가 존재 하지 않습니다. navmesh Data를 다운 합니다.");
                LoadModelFromURL.Instance.ModelDown(nav_url);
            }
            else
            {
                // author 다운로드
                Debug.Log("authoring Data 가 존재 합니다. authoring Data를 다운 합니다.");
                LoadModelFromURL.Instance.ModelDown(author_url);
            }
        }


        public void CloseWindow()
        {
            IWindowManager wm = IOC.Resolve<IWindowManager>();

            var root = wm.ComponentsRoot;
            wm.DestroyWindowsOfType("worldopen");
        }
    }
}