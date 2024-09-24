#pragma warning disable 649
using TriLibCore.General;
using UnityEngine;
using TriLibCore;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SpaceAR.Core.Utils.LoadModel
{
    /// <summary>
    /// Represents a sample that loads the "TriLibSample.obj" Model from the "Models" folder.
    /// </summary>
    public class LoadModelFromFilePath : MonoBehaviour
    {
        //[SerializeField]
        //private string _modelPath;

        public GameObject model;

        /// <summary>
        /// Loads the "Models/TriLibSample.obj" Model using the given AssetLoaderOptions.
        /// </summary>
        /// <remarks>
        /// You can create the AssetLoaderOptions by right clicking on the Assets Explorer and selecting "TriLib->Create->AssetLoaderOptions->Pre-Built AssetLoaderOptions".
        /// </remarks>
        private void Start()
        {

            // persistentDataPath + MapData + S3.Folder name + S3.File name
            string _modelPath = Path.Combine(Application.persistentDataPath, "MapData", "9105ea37-7045-4e60-a33e-6c4fb40f4e19-20221125-000916", "9105ea37-7045-4e60-a33e-6c4fb40f4e19-20221125-000916_navmesh.glb");
            //print("Check : " + _modelPath);

            FileDownLoadForPath(_modelPath);
        }

        public void FileDownLoadForPath(string _modelPath)
        {
            var assetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions();
            var res = AssetLoader.LoadModelFromFile(_modelPath, OnLoad, OnMaterialsLoad, OnProgress, OnError, model, assetLoaderOptions);
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
        }

        /// <summary>
        /// Called when the Model (including Textures and Materials) has been fully loaded.
        /// </summary>
        /// <remarks>The loaded GameObject is available on the assetLoaderContext.RootGameObject field.</remarks>
        /// <param name="assetLoaderContext">The context used to load the Model.</param>
        private void OnMaterialsLoad(AssetLoaderContext assetLoaderContext)
        {
            Debug.Log("Materials loaded. Model fully loaded.");
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
