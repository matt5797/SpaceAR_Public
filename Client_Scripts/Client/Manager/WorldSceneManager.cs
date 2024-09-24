using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Vuforia;
using SpaceAR.Client.Vuforia;
using SpaceAR.Core.AWS;

namespace SpaceAR.Client.Manager
{
    public class WorldSceneManager : MonoBehaviour
    {
        public static WorldSceneManager Instance;

        void Awake()
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
        }

        public VuforiaBehaviour vuforiaBehaviour;
        public LoadAreaTargetViaRuntime areaTargetLoader;

        // Start is called before the first frame update
        void Start()
        {
            StartScene();
        }

        // Update is called once per frame
        void Update()
        {

        }

        void StartScene()
        {
            //AWSManager.Instance.Get
        }
    }
}
