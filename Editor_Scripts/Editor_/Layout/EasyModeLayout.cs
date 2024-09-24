using Battlehub.RTEditor;
using Battlehub.UIControls.DockPanels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EasyModeLayout : LayoutExtension
{
    public LayoutInfo GetLayoutInfoEasy(IWindowManager wm)
    {
        LayoutInfo scene = wm.CreateLayoutInfo(BuiltInWindowNames.Scene);
        scene.IsHeaderVisible = true;

        LayoutInfo game = wm.CreateLayoutInfo(BuiltInWindowNames.Game);
        LayoutInfo hierarchy = wm.CreateLayoutInfo(BuiltInWindowNames.Hierarchy);
        LayoutInfo project = wm.CreateLayoutInfo(BuiltInWindowNames.Project);
        LayoutInfo behavior = wm.CreateLayoutInfo("Behavior");

        // 가운데에 씬/게임씬
        LayoutInfo sceneAndgame = LayoutInfo.Group(game, scene);
        // 가운데 아래에 이지블럭
        LayoutInfo sceneAndgameAndBehavior = LayoutInfo.Vertical(sceneAndgame, behavior, ratio: 3 / 4.0f);

        LayoutInfo hierarchyAndproject = LayoutInfo.Group(project, hierarchy);

        LayoutInfo layoutRoot = LayoutInfo.Horizontal(sceneAndgameAndBehavior, hierarchyAndproject, ratio: 3 / 4.0f);
        //LayoutInfo layoutRoot = LayoutInfo.Horizontal(layout1, hierarchy, ratio: 3 / 4.0f);
        
        return layoutRoot;
    }
}
