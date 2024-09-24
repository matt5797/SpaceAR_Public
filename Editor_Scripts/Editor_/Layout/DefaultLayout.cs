using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Battlehub.UIControls.DockPanels;
using Battlehub.RTEditor;
//using Battlehub.RTEditor.Examples.Layout;

public class DefaultLayout : LayoutExtension
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
        LayoutInfo console = wm.CreateLayoutInfo(BuiltInWindowNames.Console);
        LayoutInfo game = wm.CreateLayoutInfo(BuiltInWindowNames.Game);
        LayoutInfo project = wm.CreateLayoutInfo(BuiltInWindowNames.Project);
        LayoutInfo behavior = wm.CreateLayoutInfo("Behavior");

        LayoutInfo gameAndbehavior = LayoutInfo.Group(game, behavior);

        LayoutInfo inspectorAndconsole =
            LayoutInfo.Vertical(inspector, console, ratio: 3 / 4.0f);

        LayoutInfo sceneAndgame =
            LayoutInfo.Vertical(scene, gameAndbehavior, ratio: 3 / 4.0f);

        LayoutInfo hierarchyAndproject =
            LayoutInfo.Vertical(hierarchy, project, ratio: 3 / 4.0f);
        
        LayoutInfo layout1 = LayoutInfo.Horizontal(inspectorAndconsole, sceneAndgame, ratio: 1 / 3.0f);
        LayoutInfo layoutRoot = LayoutInfo.Horizontal(layout1, hierarchyAndproject, ratio: 3 / 4.0f);
        
        return layoutRoot;
    }

}
