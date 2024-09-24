using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using ES3Types;

using System;
using SpaceAR.Core.AWS;
using SpaceAR.Core.Lua;
using SpaceAR.Core.Network;
using Photon.Pun;

#if SPACEAR_EDITOR
using Battlehub.RTCommon;
#endif

namespace SpaceAR.Core.World
{
    public class WorldManager : MonoBehaviour
    {
        public static WorldManager Instance;
        string worldId = "e4e61e15-cf2e-4405-8c7a-f32fa496a440";
        string mapId = "c30c012d-7b5d-4b11-bb25-bdb5fd36e0ab";

        public string WorldId
        {
            get
            {
                return worldId;
            }
            set
            {
                worldId = value;
            }
        }

        string DataPath { get; set; }

        void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("More than one instance of WorldManager found!");
                return;
            }
            Instance = this;
            DataPath = Application.persistentDataPath;
        }

        public GameObject MapObject;
        public GameObject InteractObject;
        public GameObject uiObject;

        private void Start()
        {
            
        }

        public async void SaveWorld()
        {
            Progress<float> progress = new Progress<float>((percent) => { });
            bool result = await SaveWorldAsync(progress);
        }


        public Task<bool> SaveWorldAsync(IProgress<float> progress)
        {
            ES3.DeleteFile($"WorldData/{WorldId}.json");

            ES3.Save("MapObject", MapObject, $"WorldData/{WorldId}.json");
            progress.Report(0.25f);

            ES3.Save("InteractObject", InteractObject, $"WorldData/{WorldId}.json");
            progress.Report(0.5f);

            var lightList = new List<Light>();
            lightList.AddRange(InteractObject.GetComponentsInChildren<Light>());
            ES3.Save("LightList", lightList, $"WorldData/{WorldId}.json");

            //HashSet<Mesh> MeshList = new HashSet<Mesh>();
            var MeshList = new List<Mesh>();

            var meshFilterList = new List<MeshFilter>();
            meshFilterList.AddRange(MapObject.GetComponentsInChildren<MeshFilter>());
            meshFilterList.AddRange(InteractObject.GetComponentsInChildren<MeshFilter>());
            meshFilterList.AddRange(uiObject.GetComponentsInChildren<MeshFilter>());
            
            // 매쉬필터 데이터를 넣고 싶다.
            foreach (MeshFilter meshFilter in meshFilterList)
            {
                MeshList.Add(meshFilter.sharedMesh);
            }

            var skinnedMeshList = new List<SkinnedMeshRenderer>();
            skinnedMeshList.AddRange(MapObject.GetComponentsInChildren<SkinnedMeshRenderer>());
            skinnedMeshList.AddRange(InteractObject.GetComponentsInChildren<SkinnedMeshRenderer>());
            skinnedMeshList.AddRange(uiObject.GetComponentsInChildren<SkinnedMeshRenderer>());

            // 매쉬필터 데이터를 넣고 싶다.
            foreach (SkinnedMeshRenderer skinnedMesh in skinnedMeshList)
            {
                MeshList.Add(skinnedMesh.sharedMesh);
            }

            ES3.Save("MeshList", MeshList, $"WorldData/{WorldId}.json");
            progress.Report(0.75f);

            var MaterialList = new List<Material>();

            var rendererList = new List<Renderer>();
            rendererList.AddRange(MapObject.GetComponentsInChildren<Renderer>());
            rendererList.AddRange(InteractObject.GetComponentsInChildren<Renderer>());
            rendererList.AddRange(uiObject.GetComponentsInChildren<Renderer>());
            
            // 매쉬필터 데이터를 넣고 싶다.
            foreach (Renderer renderer in rendererList)
            {
                foreach (Material material in renderer.sharedMaterials)
                {
                    MaterialList.Add(material);
                }
            }

            ES3.Save("MaterialList", MaterialList, $"WorldData/{WorldId}.json");

            var TextureList = new List<Texture>();
            foreach (Material material in MaterialList)
            {
                foreach (string propertyName in material.GetTexturePropertyNames())
                {
                    Texture texture = material.GetTexture(propertyName);
                    if (texture != null)
                    {
                        TextureList.Add(material.GetTexture(propertyName));
                    }
                }
            }
            
            ES3.Save("TextureList", TextureList, $"WorldData/{WorldId}.json");
            progress.Report(0.9f);

            // luaScript string형으로 Json에 저장
            List<LuaScript> LuaScriptList = new List<LuaScript>();
            LuaScriptList.AddRange(MapObject.GetComponentsInChildren<LuaScript>());
            LuaScriptList.AddRange(InteractObject.GetComponentsInChildren<LuaScript>());
            LuaScriptList.AddRange(uiObject.GetComponentsInChildren<LuaScript>());

            List<string> textAsset = new List<string>();
            foreach (LuaScript luaScript in LuaScriptList)
            {
                if (luaScript.luaScript != null)
                    textAsset.Add(luaScript.luaScript.ToString());
                else
                    textAsset.Add(null);
            }
            ES3.Save("LuaScriptList", textAsset, $"WorldData/{WorldId}.json");

            // UI 저장
            ES3.Save("uiObject", uiObject, $"WorldData/{WorldId}.json");

            // Button 컴포넌트 저장
            List<Button>  buttonList = new List<Button>();
            buttonList.AddRange(uiObject.GetComponentsInChildren<Button>());
            ES3.Save("ButtonList", buttonList, $"WorldData/{WorldId}.json");

            progress.Report(1f);

            return Task.FromResult(true);
        }

        public async void LoadWorld(bool fromS3 = true)
        {
            Progress<float> progress = new Progress<float>((percent) => { });
            bool result = await LoadWorldAsync(progress, fromS3);
        }

        public GameObject onlyviewpf;
        public ES3ReferenceMgr testES3ReferenceMgr;
        Mesh mesh1;
        List<Mesh> meshes;
        public async Task<bool> LoadWorldAsync(IProgress<float> progress, bool fromS3=true)
        {
            // 추후 캐싱 추가 시 함수 호출 부로 이동, 조건에 따라 로드
            if (fromS3)
            {
                bool result = await LoadWorldFromS3();                
            }

            // Load Resources First
            //Debug.Log("LoadWorldFromS3 : " + result);
            var buttonList = ES3.Load<List<Button>>("ButtonList", $"WorldData/{WorldId}.json");
            var loadTextAsset = ES3.Load<List<string>>("LuaScriptList", $"WorldData/{WorldId}.json");
            var meshList = ES3.Load<List<Mesh>>("MeshList", $"WorldData/{WorldId}.json");
            var TextureList = ES3.Load<List<Texture>>("TextureList", $"WorldData/{WorldId}.json");
            var MaterialList = ES3.Load<List<Material>>("MaterialList", $"WorldData/{WorldId}.json");
            
            var lightList = ES3.Load<List<Light>>("LightList", $"WorldData/{WorldId}.json");
            
            ES3.LoadInto<GameObject>("MapObject", $"WorldData/{WorldId}.json", MapObject);
            progress.Report(0.25f);
            
            ES3.LoadInto<GameObject>("InteractObject", $"WorldData/{WorldId}.json", InteractObject);
            progress.Report(0.5f);
            
            ES3.LoadInto<GameObject>("uiObject", $"WorldData/{WorldId}.json", uiObject);
            progress.Report(0.75f);

            // TextAsset에 Json에 저장한 내용 넣기
            List<LuaScript> LuaScriptList = new List<LuaScript>();
            LuaScriptList.AddRange(MapObject.GetComponentsInChildren<LuaScript>());
            LuaScriptList.AddRange(InteractObject.GetComponentsInChildren<LuaScript>());
            LuaScriptList.AddRange(uiObject.GetComponentsInChildren<LuaScript>());

            for (int i = 0; i < LuaScriptList.Count; i++)
            {
                LuaScriptList[i].luaScript = new TextAsset(loadTextAsset[i]);
                LuaScriptList[i].LoadScript();
            }

            // import font
            List<Text> textList = new List<Text>();
            textList.AddRange(MapObject.GetComponentsInChildren<Text>());
            textList.AddRange(InteractObject.GetComponentsInChildren<Text>());
            textList.AddRange(uiObject.GetComponentsInChildren<Text>());

            Font font = Resources.Load<Font>("Font/Maplestory Bold");
            for (int i = 0; i < textList.Count; i++)
            {
                textList[i].font = font;
            }

#if SPACEAR_CLIENT
            // change mock photon transform view to photon transform view
            MockPhotonTransformView[] photonTransformViewArray = GameObject.FindObjectsOfType<MockPhotonTransformView>();

            foreach (MockPhotonTransformView mockPhotonTransformView in photonTransformViewArray)
            {
                PhotonTransformView transformView = mockPhotonTransformView.gameObject.AddComponent<PhotonTransformView>();
                transformView.m_SynchronizePosition = mockPhotonTransformView.m_SynchronizePosition;
                transformView.m_SynchronizeRotation = mockPhotonTransformView.m_SynchronizeRotation;
                transformView.m_SynchronizeScale = mockPhotonTransformView.m_SynchronizeScale;
                transformView.m_UseLocal = mockPhotonTransformView.m_UseLocal;
            }
#endif
            progress.Report(1f);
            return true;
        }
        
        /// <summary>
        /// s3에 저장된 월드 데이터를 불러오는 함수
        /// </summary>
        /// <returns></returns>
        async Task<bool> LoadWorldFromS3()
        {
            string json = await AWSManager.Instance.GetJsonObjectAsync("spacear-worlds", $"WorldData/{WorldId}.json");
            Debug.Log("S3TestCode: " + json);

            string path = Path.Combine(DataPath, $"WorldData/{WorldId}.json");
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
            print("File written to: " + path);
            return true;
        }

#if SPACEAR_EDITOR
        public void AddExpose()
        {
            Transform[] transforms = InteractObject.GetComponentsInChildren<Transform>();
            foreach (Transform transform in transforms)
            {
                transform.gameObject.AddComponent<ExposeToEditor>();
            }
            transforms = MapObject.GetComponentsInChildren<Transform>();
            foreach (Transform transform in transforms)
            {
                transform.gameObject.AddComponent<ExposeToEditor>();
            }
            transforms = uiObject.GetComponentsInChildren<Transform>();
            foreach (Transform transform in transforms)
            {
                transform.gameObject.AddComponent<ExposeToEditor>();
            }
        }
#endif
    }
}
