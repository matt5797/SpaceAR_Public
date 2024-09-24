using Battlehub.RTCommon;
using Battlehub.RTEditor;
using Battlehub.UIControls.MenuControl;
using UnityEngine;
using System;

using SpaceAR.Core.AWS;
using SpaceAR.Core.World;
using SpaceAR.Core.Editor;

namespace SpaceAR.Editor.Views.World
{
    [MenuDefinition(order: -60)]
    public static class WorldMenu
    {
        [MenuCommand("World/Create World", "WorldCreate", priority: 10)]
        public static void Create()
        {
            IWindowManager wm = IOC.Resolve<IWindowManager>();
            EditorManager.Instance.WindowCreateResize("WorldCreate", 960f, 700f, 500, 500);
        }

        [MenuCommand("World/World Open", "WorldOpen", priority: 20)]
        public static void WorldOpen()
        {
            IWindowManager wm = IOC.Resolve<IWindowManager>();
            EditorManager.Instance.WindowCreateResize("WorldOpen", 700, 440, 700, 440);
            //wm.CreateWindow("WorldOpen");
        }

        //[MenuCommand("World/World Info", "WorldInfo", priority: 30)]
        //public static void Info()
        //{

        //}

        [MenuCommand("World/Upload World", "WorldUpload", priority: 30)]
        public static void Upload()
        {
            IWindowManager wm = IOC.Resolve<IWindowManager>();
            wm.Confirmation("World Upload", "Confirm to upload world?",
                async (sender, args) =>
                {
                    Progress<float> progress = new Progress<float>((percent) =>
                    {
                        //Debug.Log("Save World :" + percent + "%");
                    });
                    bool result = await WorldManager.Instance.SaveWorldAsync(progress);
                    result = await AWSManager.Instance.PutObjectAsync("spacear-worlds", $"WorldData/{WorldManager.Instance.WorldId}.json");

                    if (result)
                    {
                        wm.MessageBox("World Upload", "World Upload Sucessed", (sender, args) => { });
                    }
                    else
                    {
                        wm.MessageBox("World Upload", "World Upload Failed", (sender, args) => { });
                    }
                },
                (sender, args) =>
                {
                    //Debug.Log("No click");
                },
                "Yes", "No");
        }

        //[MenuCommand("World/Douwnload World", "WorldDownload", priority: 40)]
        //public static void Douwnload()
        //{
        //    IWindowManager wm = IOC.Resolve<IWindowManager>();
        //    wm.Confirmation("World Download", "Confirm to Download World?",
        //        async (sender, args) =>
        //        {
        //            Progress<float> progress = new Progress<float>((percent) =>
        //            {
        //                Debug.Log("Load World :" + percent + "%");
        //            });
        //            bool result = await WorldManager.Instance.LoadWorldAsync(progress);

        //            if (result)
        //            {
        //                wm.MessageBox("World Download", "World Download Sucessed", (sender, args) => { });
        //            }
        //            else
        //            {
        //                wm.MessageBox("World Download", "World Download Failed", (sender, args) => { });
        //            }
        //        },
        //        (sender, args) =>
        //        {
        //            //Debug.Log("No click");
        //        },
        //        "Yes", "No");
        //}
    }
}



