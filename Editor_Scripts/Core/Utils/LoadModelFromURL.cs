using UnityEngine;
using SpaceAR.Core.AWS;

using Unity.VisualScripting;

#if SPACEAR_EDITOR
using Battlehub.UIControls.DockPanels;
using Battlehub.UIControls.Dialogs;
using Battlehub.RTCommon;
using Battlehub.RTEditor;
#endif
using TriLibCore;

namespace SpaceAR.Core.Utils.LoadModel
{
    /// <summary>
    /// Represents a sample that loads a compressed (Zipped) Model.
    /// </summary>
    public class LoadModelFromURL : MonoBehaviour
    {
        public static LoadModelFromURL Instance { get; private set; }

        private void Awake()
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
        }

        /// <summary>
        /// The Model URL.
        /// EX) https://spacear-maps.s3.ap-northeast-2.amazonaws.com/54861be0-ecef-442c-b0c7-4f56bc54733c/office_navmesh.glb
        /// BaseURL + mapId + "/" + mapName + "_navmesh.glb"
        /// </summary>
        public string ModelURL;

        /// <summary>
        /// Creates the AssetLoaderOptions instance, configures the Web Request, and downloads the Model.
        /// </summary>
        /// <remarks>
        /// You can create the AssetLoaderOptions by right clicking on the Assets Explorer and selecting "TriLib->Create->AssetLoaderOptions->Pre-Built AssetLoaderOptions".
        /// </remarks>
        /// 
#if SPACEAR_EDITOR

        IWindowManager wm;
        private void Start()
        {
            wm = IOC.Resolve<IWindowManager>();
        }
#endif

        public void ModelDown(string url)
        {
            var assetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions();
            var webRequest = AssetDownloader.CreateWebRequest(url);
            //AssetDownloader.LoadModelFromUri(webRequest, OnLoad, OnMaterialsLoad, OnProgress, OnError, null, assetLoaderOptions);
            AssetDownloader.LoadModelFromUri(webRequest, OnLoad, OnMaterialsLoad, OnProgress, OnError, null, assetLoaderOptions);
            
#if SPACEAR_EDITOR
            wm.MessageBox("Download", $"Downloading Mapping Data. Progress: 0:P", (sender, args) =>
            {
                Debug.Log("OK Click");
            });
#endif
        }
        public void ModelDown(string url, GameObject wrapper)
        {
            var assetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions();
            var webRequest = AssetDownloader.CreateWebRequest(url);
            //AssetDownloader.LoadModelFromUri(webRequest, OnLoad, OnMaterialsLoad, OnProgress, OnError, null, assetLoaderOptions);
            AssetDownloader.LoadModelFromUri(webRequest, OnLoad, OnMaterialsLoad, OnProgress, OnError, wrapper, assetLoaderOptions);
        }

        /// <summary>
        /// Called when any error occurs.
        /// </summary>
        /// <param name="obj">The contextualized error, containing the original exception and the context passed to the method where the error was thrown.</param>
        private void OnError(IContextualizedError obj)
        {
            Debug.LogError($"An error occurred while loading your Model: {obj.GetInnerException()}");
        }
        /// <summary>
        /// Called when the Model loading progress changes.
        /// </summary>
        /// <param name="assetLoaderContext">The context used to load the Model.</param>
        /// <param name="progress">The loading progress.</param>
        private void OnProgress(AssetLoaderContext assetLoaderContext, float progress)
        {
            Debug.Log($"Loading Model. Progress: {progress:P}");
            
#if SPACEAR_EDITOR
            
            Dialog dialog = GameObject.FindObjectOfType<Dialog>();
            dialog.ContentText = $"Downloading Mapping Data. Progress: {progress:P}";
            
            if(progress == 100)
            {
                dialog.ContentText = $"Download Success!!";
            }
#endif
        }

        /// <summary>
        /// Called when the Model (including Textures and Materials) has been fully loaded.
        /// </summary>
        /// <remarks>The loaded GameObject is available on the assetLoaderContext.RootGameObject field.</remarks>
        /// <param name="assetLoaderContext">The context used to load the Model.</param>
        private void OnMaterialsLoad(AssetLoaderContext assetLoaderContext)
        {
            Debug.Log("Materials loaded. Model fully loaded.");

            /*wm.MessageBox("Success", "World Create Success!!!", (sender, args) =>
            {
                Debug.Log("OK Click");

            });*/
#if SPACEAR_CLIENT
            ////여기서 부터 로드->멀티 아리아 찾고 하위오브젝트들과 자신 layer를 특정 layer로 바꿈
            Transform[] allChildren = GameObject.Find("MultiArea").GetComponentsInChildren<Transform>();
            foreach (Transform child in allChildren)
            {
                //수행할 함수 작성
                //Ex. AddComponent
                child.gameObject.layer = 6;
            }
            GameObject.Find("MultiArea").GetComponent<NavigationBaker>().OnClickBake();
#endif
        }

        /// <summary>
        /// Called when the Model Meshes and hierarchy are loaded.
        /// </summary>
        /// <remarks>The loaded GameObject is available on the assetLoaderContext.RootGameObject field.</remarks>
        /// <param name="assetLoaderContext">The context used to load the Model.</param>
        private void OnLoad(AssetLoaderContext assetLoaderContext)
        {
            Debug.Log("Model loaded. Loading materials.");
        }
    }
}
