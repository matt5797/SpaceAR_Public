using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Battlehub.UIControls.DockPanels;
using Battlehub.RTEditor;
using Battlehub.RTCommon;
using Battlehub.UIControls.MenuControl;
using UnityEditor;
using SpaceAR.Core.World;

namespace SpaceAR.Editor.Layout
{
    [MenuDefinition]
    public class LayoutManager : MonoBehaviour
    {
        public List<LayoutExtension> Layouts;

        [MenuCommand("Mode/Easy Mode", "", priority: 10)]
        public void EasyMode()
        {
            Debug.Log("Easy Mode Layout");
            //Layouts.ForEach(l => l.enabled = false);
            //Layouts[0].enabled = true;
            wm.SetLayout(GetComponentInChildren<EasyModeLayout>().GetLayoutInfoEasy);

        }

        [MenuCommand("Mode/Expert Mode", "", priority: 20)]
        public void ExpertMode()
        {
            Debug.Log("Expert Mode Layout");
            //Layouts.ForEach(l => l.enabled = false);
            //Layouts[1].enabled = true;
            wm.SetLayout(GetComponentInChildren<ExpertModeLayout>().GetLayoutInfoExpert);
            //wm.SetLayout(GetLayoutInfoExpert);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="wm"></param>
        /// <returns></returns>
        /// 

        IWindowManager wm;

        private void Start()
        {
            wm = IOC.Resolve<IWindowManager>();
            EasyMode();
        }

        //protected LayoutInfo GetLayoutInfoEasy(IWindowManager wm)
        //{
        //    LayoutInfo scene = wm.CreateLayoutInfo(BuiltInWindowNames.Scene);
        //    scene.IsHeaderVisible = true;

        //    LayoutInfo hierarchy = wm.CreateLayoutInfo(BuiltInWindowNames.Hierarchy);
        //    LayoutInfo game = wm.CreateLayoutInfo(BuiltInWindowNames.Game);
        //    LayoutInfo project = wm.CreateLayoutInfo(BuiltInWindowNames.Project);
        //    LayoutInfo behavior = wm.CreateLayoutInfo("Behavior");

        //    // 가운데에 씬/게임씬
        //    LayoutInfo sceneAndgame = LayoutInfo.Horizontal(scene, game);
        //    // 가운데 아래에 이지블럭
        //    LayoutInfo sceneAndgameAndBehavior = LayoutInfo.Vertical(sceneAndgame, behavior, ratio : 3 / 4.0f);
        
        //    LayoutInfo hierarchyAndproject =
        //        LayoutInfo.Vertical(hierarchy, project, ratio: 3 / 4.0f);

        //    LayoutInfo layout1 = LayoutInfo.Horizontal(project, sceneAndgameAndBehavior, ratio: 1 / 3.0f);
        //    LayoutInfo layoutRoot = LayoutInfo.Horizontal(layout1, hierarchy, ratio: 3 / 4.0f);

        //    return layoutRoot;
        //}

        //protected LayoutInfo GetLayoutInfoExpert(IWindowManager wm)
        //{
        //    LayoutInfo scene = wm.CreateLayoutInfo(BuiltInWindowNames.Scene);
        //    scene.IsHeaderVisible = true;

        //    LayoutInfo hierarchy = wm.CreateLayoutInfo(BuiltInWindowNames.Hierarchy);
        //    LayoutInfo inspector = wm.CreateLayoutInfo(BuiltInWindowNames.Inspector);
        //    LayoutInfo console = wm.CreateLayoutInfo(BuiltInWindowNames.Console);
        //    LayoutInfo game = wm.CreateLayoutInfo(BuiltInWindowNames.Game);
        //    LayoutInfo project = wm.CreateLayoutInfo(BuiltInWindowNames.Project);

        //    // 가운데에 씬/게임씬
        //    LayoutInfo sceneAndgame = LayoutInfo.Vertical(scene, game, ratio : 3 / 4.0f);

        //    LayoutInfo hierarchyAndproject =
        //        LayoutInfo.Vertical(hierarchy, project, ratio: 3 / 4.0f);

        //    LayoutInfo inspectorAndConsole = LayoutInfo.Vertical(inspector, console);

        //    LayoutInfo layout1 = LayoutInfo.Horizontal(inspectorAndConsole, sceneAndgame, ratio: 1 / 3.0f);
        //    LayoutInfo layoutRoot = LayoutInfo.Horizontal(layout1, hierarchyAndproject, ratio: 3 / 4.0f);

        //    return layoutRoot;
        //}

        private void Update()
        {
            //if (Input.GetKeyDown(KeyCode.F1))
            //{
            //    wm = IOC.Resolve<IWindowManager>();
            //    wm.SetLayout(GetLayoutInfoEasy);
            //}
            //else if (Input.GetKeyDown(KeyCode.F2))
            //{
            //    wm = IOC.Resolve<IWindowManager>();
            //    wm.SetLayout(GetLayoutInfoExpert);
            //}

        }
    }

}
