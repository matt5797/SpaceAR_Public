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

        // ������ �ǳ�
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
            
            // �α����� owener��������
            string owner = AWSManager.Instance.GetUsersId();

            // ���� ���ε��� �� ����Ʈ ��������
            var res = await AWSManager.Instance.GetMapList(owner);

            // �� ��Ŀ 
            mapMarker = GameObject.Find("Map").GetComponent<SpawnOnMap>();
                
            print(res);
            
            mapMarker._locationStrings = new List<string>();

            foreach (var data in res)
            {
                // �ӽÿ� (�����)
                GameObject content = Instantiate(contentPrefab, parent);

                mapContents = content.GetComponent<MapContents>();
                mapContents.name = data["name"].ToString();
                mapContents.username = data["username"].ToString();

                // ======== ���� ���� ==============//
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
                // ��Ŀ �߰�
                mapMarker._locationStrings.Add(mapContents.latitude + ", " + mapContents.longitude);

                // ����� �߰�
                if (mapContents.key.Length != 0)
                {
                    content.GetComponent<WebCacheImage>().SetUrl($"https://{data["bucket"].ToString()}.s3.ap-northeast-2.amazonaws.com/{data["key"].ToString()}/1.png");
                }

                content.GetComponent<Button>().onClick.AddListener(() => OnContentUpdate(content));
            }
            // ��Ŀ ����
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

            // ���� ����� Ȱ��ȭ
            image.SetActive(true);
            if (mapContents.key.Length != 0)
            {
                image.GetComponent<WebCacheImage>().SetUrl($"https://{mapContents.bucket}.s3.ap-northeast-2.amazonaws.com/{mapContents.key}/1.png");
            }
            else
            {
                image.GetComponent<RawImage>().texture = baseThumbnail;
            }
        
            // ���� ���� ��ư Ȱ��ȭ
            worldCreate.SetActive(true);

            IWindowManager wm;
            wm = IOC.Resolve<IWindowManager>();
            
            worldCreate.GetComponent<Button>().onClick.AddListener(() => { 
                Debug.Log($"{mapContents.mapId} Ŭ�� �Ϸ�");

                // ���� â ����
                EditorManager.Instance.WindowCreateResize("WorldCreateDetail", 500f, 500f, 500, 500);

            }) ;

            // �̸�
            _mapname.GetComponent<TextMeshProUGUI>().text = $"�̸� : {mapContents.mapName}";

            // ������
            ownerName.GetComponent<TextMeshProUGUI>().text = $"������ : {mapContents.username}";

            // ���ε� ��¥

            CultureInfo ciKo = new CultureInfo("ko-KR");

            DateTime tmp = DateTime.ParseExact(mapContents.mapCreateDate, "yyyy-MM-dd HH:mm:ss.ffffff", ciKo);

            date.GetComponent<TextMeshProUGUI>().text = $"���ε� ��¥ : {tmp.ToString("yyyy.MM.dd.ddd HH:mm", ciKo)}";

            // ��������
            isPublic.GetComponent<TextMeshProUGUI>().text = $"�������� : {mapContents.mapIsPublic}";

            // Ȱ�� ���� ��
            worldCount.GetComponent<TextMeshProUGUI>().text = $"Ȱ�� ���� �� : {mapContents.mapCount}";

        }
    }
}

