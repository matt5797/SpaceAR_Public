using Battlehub.RTCommon;
using Battlehub.RTEditor;
using UnityEngine;
using Gpm.Ui;

using Battlehub.UIControls.DockPanels;

using SpaceAR.Editor.Views.Login;
using UnityEngine.UI;
using System.Collections;
using System.IO;

namespace SpaceAR.Core.Editor
{
    public class EditorManager : MonoBehaviour
    {
        public static EditorManager Instance { get; private set; }

        IWindowManager wm;
        Transform LoginInspector;

        // 월드오픈창 
        bool isOpen = false;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            CrateBaseDirectory();
        }

        void Start()
        {
            wm = IOC.Resolve<IWindowManager>();
            EditorManager.Instance.WindowCreateResize("Login", 620, 540, 620, 540);
            //wm.CreateWindow("Login");

            LoginInspector = wm.GetWindow("Login");
        }

        private void Update()
        {
            LoginInspector = wm.GetWindow("Login");

            // 로그인을 했으면
            if (LoginManager.loginResult && isOpen == false)
            {
                Tab tab = Region.FindTab(LoginInspector);
                tab.Close();
                wm.CreateWindow("WorldOpen");
                isOpen = true;
            }
        }

        public void WindowCreateResize(string windowname, float width, float height, float minWidth, float minHeight)
        {
            IWindowManager wm;
            wm = IOC.Resolve<IWindowManager>();

            wm.CreateWindow(windowname);
            
            var region = wm.GetWindow(windowname).transform.GetComponentInParent<Region>();

            region.MinWidth = minWidth;
            region.MinHeight = minHeight;

            RectTransform rt = (RectTransform)region.transform;
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);

            region.Fit(true);
        }

        private Texture _mainTexture;
        private Texture2D _texture2D;
        private RenderTexture _renderTexture;

        public Texture2D getTexture_to_Texture2D(Texture image)
        {
            if (_mainTexture == null)
                _mainTexture = image;

            _texture2D = new Texture2D(_mainTexture.width, _mainTexture.height, TextureFormat.RGBA32, false);
            //RenderTexture currentRT = RenderTexture.active;

            if (_renderTexture == null)
                _renderTexture = new RenderTexture(_mainTexture.width, _mainTexture.height, 32);

            // mainTexture 의 픽셀 정보를 renderTexture 로 카피
            Graphics.Blit(_mainTexture, _renderTexture);

            // renderTexture 의 픽셀 정보를 근거로 texture2D 의 픽셀 정보를 작성
            RenderTexture.active = _renderTexture;

            _texture2D.ReadPixels(new Rect(0, 0, _renderTexture.width, _renderTexture.height), 0, 0);
            _texture2D.Apply();

            _mainTexture = null;
            _renderTexture = null;

            return _texture2D;

        }

        public void URLToSprite(string path)
        {
            WebCacheImage chach = new WebCacheImage();
            chach = GetComponent<WebCacheImage>();
            chach.SetUrl(path);
            
            // Texture -> Texture2D
            //RawImage image = GetComponent<RawImage>();
            //Texture2D _texture2D = getTexture_to_Texture2D(image.texture);
            //print(1);
            
            // Texture2D -> Sprite
            //Rect rect = new Rect(0, 0, _texture2D.width, _texture2D.height);
            //Sprite getSprite = Sprite.Create(_texture2D, rect, new Vector2(0.5f, 0.5f), 100);
        }

        public void CrateBaseDirectory()
        {
            string assetPath = "DefaultProject/Assets";
            string mapDataPath = "MapData";
            string worldDataPath = "WorldData";

            if (!Directory.Exists(Application.persistentDataPath + "/" + assetPath + "/DownloadAsset"))
            {
                Directory.CreateDirectory(Application.persistentDataPath + "/" + assetPath + "/DownloadAsset");
            }

            if (!Directory.Exists(Application.persistentDataPath + "/" + mapDataPath))
            {
                Directory.CreateDirectory(Application.persistentDataPath + "/" + mapDataPath);
            }

            if (!Directory.Exists(Application.persistentDataPath + "/" + worldDataPath))
            {
                Directory.CreateDirectory(Application.persistentDataPath + "/" + worldDataPath);
            }
        }

    }

}
