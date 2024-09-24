using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using SpaceAR.Core.AWS;
using System.Windows.Forms;
using Button = UnityEngine.UI.Button;
using Battlehub.RTCommon;
using Battlehub.RTEditor;
using SpaceAR.Core.Editor;
using Battlehub.UIControls.DockPanels;

namespace SpaceAR.Editor.Views.WorldOpen
{
    public class WorldOpenManager : MonoBehaviour
    {
        public GameObject worldButtonPrefab;
        public Transform worldButtonParent;
        public GameObject addWorldButton;
        JArray worldArray;

        string userId;

        // Start is called before the first frame update
        async void Start()
        {
            // 월드 생성 창 오픈
            IWindowManager wm;
            wm = IOC.Resolve<IWindowManager>();

            userId = AWSManager.Instance.GetUsersId();

            worldArray = await AWSManager.Instance.GetUserWorldList(userId);
            CreateWorldButtons();
            addWorldButton.GetComponent<Button>().onClick.AddListener(() => {
                EditorManager.Instance.WindowCreateResize("WorldCreate", 960f, 700f, 960f, 700f);
                Region.FindTab(wm.GetWindow("WorldOpen")).Close();
            });
        }

        void CreateWorldButtons()
        {
            foreach (JObject world in worldArray)
            {
                GameObject button = Instantiate(worldButtonPrefab, worldButtonParent);
                WorldData worldData = JsonConvert.DeserializeObject<WorldData>(world.ToString());
                button.GetComponent<WorldButton>().WorldData = worldData;
            }
        }
    }
    
    public class WorldData
    {
        public string id;
        public string owner;
        public string map;
        public string name;
        public string create_date;
        public bool activate;
        public string description;
        public List<Texture2D> thumbnailList;
        public string map_name;
    }
}