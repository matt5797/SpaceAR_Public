using Battlehub.RTEditor.Views;
using SpaceAR.Core.AWS;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using SpaceAR.Editor.Views.WorldCreate.Views;
using SpaceAR.Core.World;
using Battlehub.UIControls.DockPanels;
using Battlehub.RTCommon;
using Battlehub.RTEditor;
using SpaceAR.Core.Utils.LoadModel;
using UnityEngine.Networking;
using System.Threading.Tasks;

namespace SpaceAR.Editor.Views.World.WorldCreateDetail.Views
{
    public class WorldCreateDetailView : View
    {
        public TMP_InputField worldnameField;
        public Toggle isPublicField;
        public TMP_InputField detailField;
        public Button thumbnailBtn;
        public Button worldCreateBtn;

        string worldname;
        bool isPublic;
        string detail;
        string owner;
        
        string mapKey;
        string mapId;

        string BaseURL = "https://spacear-maps.s3.ap-northeast-2.amazonaws.com/";

        string navURL;
        string authorURL;
        protected override async void Start()
        {
            base.Start();
            owner = AWSManager.Instance.UserId;

            mapId = WorldCreateView.nowMapId;

            var res = await AWSManager.Instance.GetMapAreaTarget(mapId);

            Debug.Log("MapKey Check " + res[0]["name"]);
            
            mapKey = res[0]["name"].ToString();
            
            worldCreateBtn.onClick.AddListener(() => { OnCreateButton(); });

        }

        public async void OnCreateButton()
        {
            worldname = worldnameField.text;
            isPublic = isPublicField.isOn;
            detail = detailField.text;

            // URL 세팅
            // BaseURL + mapId + "/" + mapName + ".glb"
            navURL = BaseURL + mapKey + "/" + mapKey + "_navmesh.glb";
            authorURL = BaseURL + mapKey + "/" + mapKey + "_authoring.glb";

            Debug.Log($"owner : {owner}");
            Debug.Log($"map : {mapKey}");
            Debug.Log($"worldname : {worldname}");
            Debug.Log($"isPublic : {isPublic}");
            Debug.Log($"detail : {detail}");

            // TODO : 썸네일 이미지 업로드
            // TODO : 월드 생성

            var res = await AWSManager.Instance.PostWorld(owner, mapId, worldname, isPublic, detail);

            WorldManager.Instance.WorldId = res[0]["id"].ToString();

            IWindowManager wm;
            wm = IOC.Resolve<IWindowManager>();
            // 창 닫고 
            Region.FindTab(wm.GetWindow("worldCreate")).Close();
            Region.FindTab(wm.GetWindow("worldCreateDetail")).Close();

            // S3에서 프리뷰 모델을 가져오기
            await WebCheck(navURL, authorURL);
            // requset _preview model at bucket 
            // if not exist : download _navmesh model
            //LoadModelFromURL.Instance.ModelDown(modelURL);
            // else : download _preview model            
        }

        public static async Task WebCheck(string nav_url, string author_url)
        {
            UnityWebRequest www = UnityWebRequest.Head(author_url);
            await Task.Yield();
            www.SendWebRequest();
            
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
    }
}


