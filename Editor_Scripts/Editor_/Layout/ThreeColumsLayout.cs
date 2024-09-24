using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Battlehub.UIControls.DockPanels;
using Battlehub.RTEditor;
//using Battlehub.RTEditor.Examples.Layout;

namespace SpaceAR.Editor.Layout
{
    public class ThreeColumsLayout : LayoutExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="wm"></param>
        /// <returns></returns>
        protected override LayoutInfo GetLayoutInfo(IWindowManager wm)
        {
            LayoutInfo scene = wm.CreateLayoutInfo(BuiltInWindowNames.Scene);
            scene.IsHeaderVisible = true;

            LayoutInfo hierarchy = wm.CreateLayoutInfo(BuiltInWindowNames.Hierarchy);
            LayoutInfo inspector = wm.CreateLayoutInfo(BuiltInWindowNames.Inspector);

            //Defines a region divided into two equal parts (ratio 1 / 2)
            LayoutInfo sceneAndHierarchy =
                LayoutInfo.Horizontal(scene, hierarchy, ratio: 1 / 2.0f);

            //Defines a region divided into two parts 
            //1/3 for the inspector and 2/3 for the scene and hierarchy)
            LayoutInfo layoutRoot =
                LayoutInfo.Horizontal(inspector, sceneAndHierarchy, ratio: 1 / 3.0f);

            return layoutRoot;
        }

    }
}