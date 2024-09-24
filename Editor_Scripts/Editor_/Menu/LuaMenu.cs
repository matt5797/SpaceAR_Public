using Battlehub.RTCommon;
using Battlehub.RTScripting;
using Battlehub.RTSL.Interface;
using Battlehub.UIControls.MenuControl;
using System;
using System.Threading.Tasks;
using UnityEngine;

//namespace Battlehub.RTEditor
namespace SpaceAR.Editor.Menu
{
    [MenuDefinition(order: -60)]
    public class LuaMenu : MonoBehaviour
    {
        /// add new command to exising menu
        //[MenuCommand("MenuWindow/Create My Window")]
        //public static void CreateMyWindow()
        //{
        //    Debug.Log("Create My Window");
        //}

        private static string s_nl = Environment.NewLine;
        private static string s_LuaTemplate =
            "-- How To Use --" + "\n" +
            "-- 1. Add Component \"LuaBehaviour\" Script -- " + "\n" +
            "-- 2. Drag And Drop Your Lua Script in \"LuaBehaviour Inspector\" --" + "\n"+
            "----------------------------------------------------------------------" + "\n" + "\n" +
            "-- Start is called before the first frame update --" + s_nl +
            "function start()" + s_nl +
            "{0}" + s_nl +
            "end" + s_nl + "\n" +
            "-- Update is called once per frame -- " + s_nl +
            "function update()" + s_nl +
            "{1}" + s_nl +
            "end" + s_nl
            ;


        /// add new command to new menu
        [MenuCommand("Lua Script/Create Lua Script", "RTEAsset_Lua", priority: 10)]
        public async void CreateLuaScript()
        {
            Debug.Log("Satrt Lua Script!!");

            IProjectAsync project = IOC.Resolve<IProjectAsync>();
            IRuntimeScriptManager scriptManager = IOC.Resolve<IRuntimeScriptManager>();

            string desiredTypeName = "HelloLua";

            // ���, �̸�
            ProjectItem assetItem =
                await scriptManager.CreateScriptAsync(
                    project.State.RootFolder,
                    desiredTypeName);

            RuntimeTextAsset lua = await scriptManager.LoadScriptAsync(assetItem);

            string typeName = lua.name;

            lua.Text =
                string.Format(s_LuaTemplate, "      ", "      ");
            await scriptManager.SaveScriptAsync(assetItem, lua);
            //await scriptManager.CompileAsync();

            // ���÷� �� ��ü ���� �� ������Ʈ �ֱ�
            //GameObject testGo = new GameObject("Lua Scripting example");
            //testGo.AddComponent<ExposeToEditor>();
            //testGo.AddComponent<LuaTestBehaviour>();
            //testGo.AddComponent(scriptManager.GetType(typeName));

            await Task.Yield();

            IRTE editor = IOC.Resolve<IRTE>();
            editor.IsPlaying = true;

            await Task.Yield();

            editor.IsPlaying = false;

        }

        /// disable menu item
        //[MenuCommand("Lua Script/Create Lua Script", validate: true)]
        //public static bool ValidateMyCommand()
        //{
        //    Debug.Log("Disable My Command");
        //    return false;
        //}

        ///// replace existing menu item
        //[MenuCommand("MenuFile/Close")]
        //public static void Close()
        //{
        //    Debug.Log("Intercepted");

        //    IRuntimeEditor rte = IOC.Resolve<IRuntimeEditor>();
        //    rte.Close();
        //}

        ///// Hide existing menu item    
        //[MenuCommand("MenuHelp/About RTE", hide: true)]
        //public static void HideAbout() { }
    }
}


