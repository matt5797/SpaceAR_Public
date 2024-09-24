using Battlehub.RTEditor.Views;
using UnityEngine;
using SpaceAR.Core.AWS;
using UnityEngine.UI;
using TMPro;
using System;
using System.Globalization;
using Mapbox.Examples;
using Battlehub.RTCommon;
using Battlehub.RTEditor;
using Battlehub.UIControls.DockPanels;

using SpaceAR.Core.Editor;
using System.Collections.Generic;
using Gpm.Ui;

namespace SpaceAR.Editor.Views.WorldCreate.Views
{
    public class WorldCreateView : View
    {
        public static string nowMapId;
        public static string nowMapName;
            
        public GameObject contentPrefab;
        public Transform parent;
        public Texture2D baseThumbnail;

        MapContents mapContents;

        // 오른쪽 판낼
        public GameObject _mapname;
        public GameObject ownerName;
        public GameObject date;
        public GameObject image;
        public GameObject isPublic;
        public GameObject worldCount;
        public GameObject worldCreate;
        
        SpawnOnMap mapMarker;
        
        protected override async void Start()
        {
            base.Start();
            
            // 로그인한 owener가져오기
            string owner = AWSManager.Instance.GetUsersId();

            // 내가 업로드한 맵 리스트 가져오기
            var res = await AWSManager.Instance.GetMapList(owner);

            // 맵 마커 
            mapMarker = GameObject.Find("Map").GetComponent<SpawnOnMap>();
                
            print(res);
            
            mapMarker._locationStrings = new List<string>();

            foreach (var data in res)
            {
                // 임시용 (썸네일)
                GameObject content = Instantiate(contentPrefab, parent);

                mapContents = content.GetComponent<MapContents>();
                mapContents.name = data["name"].ToString();
                mapContents.username = data["username"].ToString();

                // ======== 정보 세팅 ==============//
                mapContents.mapId = data["id"].ToString();
                //mapContents.mapOwner = data["owner"].ToString();
                mapContents.mapName = data["name"].ToString();
                mapContents.mapCreateDate = data["create_date"].ToString();
                mapContents.mapIsPublic = data["is_public"].ToString();
                mapContents.mapCount = data["count"].ToString();
                mapContents.longitude = data["longitude"].ToString();
                mapContents.latitude = data["latitude"].ToString();
                mapContents.bucket = data["bucket"].ToString();
                mapContents.key = data["key"].ToString();
                // ================================//

                //37.413918, 127.099184
                // 마커 추가
                mapMarker._locationStrings.Add(mapContents.latitude + ", " + mapContents.longitude);

                // 썸네일 추가
                if (mapContents.key.Length != 0)
                {
                    content.GetComponent<WebCacheImage>().SetUrl($"https://{data["bucket"].ToString()}.s3.ap-northeast-2.amazonaws.com/{data["key"].ToString()}/1.png");
                }

                content.GetComponent<Button>().onClick.AddListener(() => OnContentUpdate(content));
            }
            // 마커 생성
            if(mapMarker.isSpawn == false)
            {
                mapMarker.InstantiateMarker();
            }

        }

        public void OnContentUpdate(GameObject content)
        {
            mapContents = content.GetComponent<MapContents>();

            nowMapId = mapContents.mapId;
            nowMapName = mapContents.mapName;

            // 월드 썸네일 활성화
            image.SetActive(true);
            if (mapContents.key.Length != 0)
            {
                image.GetComponent<WebCacheImage>().SetUrl($"https://{mapContents.bucket}.s3.ap-northeast-2.amazonaws.com/{mapContents.key}/1.png");
            }
            else
            {
                image.GetComponent<RawImage>().texture = baseThumbnail;
            }
        
            // 월드 생성 버튼 활성화
            worldCreate.SetActive(true);

            IWindowManager wm;
            wm = IOC.Resolve<IWindowManager>();
            
            worldCreate.GetComponent<Button>().onClick.AddListener(() => { 
                Debug.Log($"{mapContents.mapId} 클릭 완료");

                // 월드 창 생성
                EditorManager.Instance.WindowCreateResize("WorldCreateDetail", 500f, 500f, 500, 500);

            }) ;

            // 이름
            _mapname.GetComponent<TextMeshProUGUI>().text = $"이름 : {mapContents.mapName}";

            // 소유자
            ownerName.GetComponent<TextMeshProUGUI>().text = $"소유자 : {mapContents.username}";

            // 업로드 날짜

            CultureInfo ciKo = new CultureInfo("ko-KR");

            DateTime tmp = DateTime.ParseExact(mapContents.mapCreateDate, "yyyy-MM-dd HH:mm:ss.ffffff", ciKo);

            date.GetComponent<TextMeshProUGUI>().text = $"업로드 날짜 : {tmp.ToString("yyyy.MM.dd.ddd HH:mm", ciKo)}";

            // 공개여부
            isPublic.GetComponent<TextMeshProUGUI>().text = $"공개여부 : {mapContents.mapIsPublic}";

            // 활성 월드 수
            worldCount.GetComponent<TextMeshProUGUI>().text = $"활성 월드 수 : {mapContents.mapCount}";

        }
    }
}

