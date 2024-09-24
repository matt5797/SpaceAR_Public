using SpaceAR.Core.Utils.LoadModel;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using Vuforia;

namespace SpaceAR.Client.Vuforia
{
    public class LoadAreaTargetViaRuntime : MonoBehaviour
    {
        public AreaTargetBehaviour targetBehaviour;                     // the behaviour of the created area target
        public AreaTargetObserverEventHandler eventHandler;             // the event handler of the created area target

        public GameObject MeshsetGameObject;                            // the area target mesh game object

        public string DataSetName;                                      // name of the area target

        string DataSetXmlPath;                                          // path to the XML file
        string DataSetFolder;                                           // path to the area target folder

        public Material ModelMaterial;                                  // can be any material

        void Start()
        {
            //LoadAreaTargetData(DataSetName);
        }

        public string GetDataSetPathBasedOnName(string name)
        {
            //var scanningDirectory = new DirectoryInfo(Application.streamingAssetsPath);
            var scanningDirectory = new DirectoryInfo(Application.persistentDataPath + "/MapData");

            var directoryInfos = scanningDirectory.GetDirectories();

            foreach (var directoryInfo in directoryInfos)
            {
                if (directoryInfo.Name == name)
                {
                    var datasetFileXML = FindFileWithExtension(directoryInfo.FullName, PATTERN_XML);
                    var datasetFileDat = FindFileWithExtension(directoryInfo.FullName, PATTERN_DAT);
                    var datasetFileOcclusionModel = FindFileWithExtension(directoryInfo.FullName, PATTERN_3DT);
                    var datasetFileGLB = FindFileWithExtension(directoryInfo.FullName, PATTERN_GLB);

                    if (datasetFileDat != null && datasetFileXML != null && (datasetFileOcclusionModel != null || datasetFileGLB != null))
                    {
                        return directoryInfo.FullName;
                    }
                }
            }

            Debug.LogError("No area target data was found with name: " + name);
            return null;
        }

        public void LoadAreaTargetData(string name)
        {
            DataSetFolder = GetDataSetPathBasedOnName(DataSetName);
            DataSetXmlPath = DataSetFolder + "/" + DataSetName + ".xml";

            StartCoroutine(LoadAreaTargetDataAsync(name, DataSetXmlPath, DataSetFolder));
        }

        IEnumerator LoadAreaTargetDataAsync(string name, string xml, string path)
        {
            yield return new WaitForEndOfFrame();

            if (!string.IsNullOrEmpty(name))
            {
                if (!IsAreaTargetDataSet(xml))
                    yield break;

                if (!LoadAreaTarget(name, xml, path))
                {

                    yield break;
                }
            }
            else
                Debug.LogError("Invalid dataset name");

        }

        public bool LoadDatasetModel(string name, string xml, string path, bool isGLBAreaTarget)
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("Invalid path: path to model file is empty.");
                return false;
            }

            targetBehaviour = VuforiaBehaviour.Instance.ObserverFactory.CreateAreaTarget(xml, name);

            eventHandler = targetBehaviour.GetComponent<AreaTargetObserverEventHandler>();
            if (!eventHandler)
            {
                eventHandler = targetBehaviour.gameObject.AddComponent<AreaTargetObserverEventHandler>();
                eventHandler.StatusFilter = DefaultObserverEventHandler.TrackingStatusFilter.Tracked_ExtendedTracked_Limited;

                eventHandler.OnTargetFound = new UnityEngine.Events.UnityEvent();
                eventHandler.OnTargetLost = new UnityEngine.Events.UnityEvent();
                eventHandler.OnTargetFound.AddListener(() => { OnTargetFound(); });
                eventHandler.OnTargetLost.AddListener(() => { OnTargetLost(); });
            }


            if (isGLBAreaTarget)
            {
                MeshsetGameObject = LoadGLBGameObject(path);
                MeshsetGameObject.transform.SetParent(targetBehaviour.transform);
            }
            else
                MeshsetGameObject = LoadMeshSetGameObject(path, targetBehaviour.TargetName, targetBehaviour.transform);

            if (MeshsetGameObject == null)
            {
                Debug.LogError("Error while extracting model from file: model is null.");
                return false;
            }

            GameObject multiArea = GameObject.Find("MultiArea");
            targetBehaviour.transform.SetParent(multiArea.transform);

            

            string mapForderName = GetComponent<ARSceneManager>().mapForderName;
            string mapMeshBasePath = GetComponent<ARSceneManager>().mapMeshBasePath;
            string totalPath = mapMeshBasePath + mapForderName + "/" + mapForderName + "_navmesh.glb";
            LoadModelFromURL.Instance.ModelDown(totalPath, targetBehaviour.gameObject);

            //////여기서 부터 로드->멀티 아리아 찾고 하위오브젝트들과 자신 layer를 특정 layer로 바꿈
            //Transform[] allChildren = multiArea.GetComponentsInChildren<Transform>();
            //foreach (Transform child in allChildren)
            //{
            //    //수행할 함수 작성
            //    //Ex. AddComponent
            //    child.gameObject.layer = 6;
            //}

            eventHandler.SetAugmentation(MeshsetGameObject);
            MeshsetGameObject.SetActive(true);
            targetBehaviour.enabled = true;

            return true;
        }

        GameObject LoadGLBGameObject(string pathToFile)
        {
            return new GLTFModelCreator().ExtractModel(pathToFile, new Material(ModelMaterial), true);
        }

        GameObject LoadMeshSetGameObject(string path, string name, Transform parent)
        {
            return MeshSetModelCreator.LoadMeshSet(name, path, parent, StorageType.ABSOLUTE,
                false, true, new Material(ModelMaterial));
        }

        bool LoadAreaTarget(string name, string xml, string path)
        {
            var loadSuccessful = false;
            var isGLB = false;
            var datasetFile = DirectoryScanner.FindFileWithExtension(path, "*.3dt");
            if (datasetFile == null)
            {
                datasetFile = DirectoryScanner.FindFileWithExtension(path, "*.glb");
                isGLB = true;
            }

            var fullPath = Path.Combine(path, datasetFile);
            fullPath = fullPath.Replace("\\", "/");
            loadSuccessful = LoadDatasetModel(name, xml, fullPath, isGLB);

            return loadSuccessful;
        }

        bool IsAreaTargetDataSet(string datasetXmlFullPath)
        {
            var xmlDoc = new XmlDocument();
            var dataSetXmlContent = File.ReadAllText(datasetXmlFullPath);

            if (!string.IsNullOrEmpty(dataSetXmlContent))
            {
                xmlDoc.LoadXml(dataSetXmlContent);

                var areaNode = FindXmlNodeByName(xmlDoc, "AreaTarget");

                if (areaNode == null)
                {
                    Debug.LogError("Invalid AreaTarget Database XML: does not contain the AreaTarget node.");
                    return false;
                }

                return true;
            }

            Debug.LogError("Invalid AreaTarget Database XML: file is empty.");
            return false;
        }

        XmlNode FindXmlNodeByName(XmlNode rootNode, string nodeName)
        {
            if (rootNode.Name == nodeName)
                return rootNode;

            foreach (var childNode in rootNode.ChildNodes)
            {
                var matchingNode = FindXmlNodeByName(childNode as XmlNode, nodeName);
                if (matchingNode != null)
                    return matchingNode;
            }
            return null;
        }

        const string PATTERN_XML = "*.xml";
        const string PATTERN_DAT = "*.dat";
        const string PATTERN_3DT = "*.3dt";
        const string PATTERN_GLB = "*.glb";

        public static string FindFileWithExtension(string dirPath, string extension)
        {
            if (!Directory.Exists(dirPath))
                return null;

            var files = new DirectoryInfo(dirPath).GetFiles(extension, SearchOption.AllDirectories);
            if (files.Length == 0)
                return null;

            return files[0].Name;
        }

        public GameObject rightArrow;
        public GameObject leftArrow;
        public GameObject phone;
        public GameObject findingTarget;
        public GameObject deTect;
        public GameObject UnDetect;
        bool check = true;
        public void OnTargetFound()
        {
            check = false;
            findingTarget.SetActive(false);

        }

        public void OnTargetLost()
        {
            StartCoroutine(IE_LostTarget());
            
        }

        IEnumerator IE_LostTarget()
        {
            check = true;
            findingTarget.SetActive(true);

            while (true)
            {
                if(check)
                {
                    rightArrow.transform.localPosition += 0.3f * Mathf.Cos(Time.time * 3) * rightArrow.transform.right;
                    leftArrow.transform.localPosition += 0.3f * Mathf.Cos(Time.time * 3) * -leftArrow.transform.right;
                    yield return null;
                }
                else
                {
                    yield break;
                }
                
            }
        }
    }

}
