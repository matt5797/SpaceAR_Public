using Battlehub.RTEditor;
using Battlehub.UIControls.DockPanels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpertModeLayout : LayoutExtension
{
   public LayoutInfo GetLayoutInfoExpert(IWindowManager wm)
    {
        LayoutInfo scene = wm.CreateLayoutInfo(BuiltInWindowNames.Scene);
        scene.IsHeaderVisible = true;

        LayoutInfo hierarchy = wm.CreateLayoutInfo(BuiltInWindowNames.Hierarchy);
        LayoutInfo inspector = wm.CreateLayoutInfo(BuiltInWindowNames.Inspector);
        LayoutInfo console = wm.CreateLayoutInfo(BuiltInWindowNames.Console);
        LayoutInfo game = wm.CreateLayoutInfo(BuiltInWindowNames.Game);
        LayoutInfo project = wm.CreateLayoutInfo(BuiltInWindowNames.Project);

        // ∞°øÓµ•ø° æ¿/∞‘¿”æ¿
        LayoutInfo sceneAndgame = LayoutInfo.Vertical(scene, game, ratio: 3 / 4.0f);

        LayoutInfo hierarchyAndproject =
            LayoutInfo.Vertical(hierarchy, project, ratio: 3 / 4.0f);

        LayoutInfo inspectorAndConsole = LayoutInfo.Vertical(inspector, console);

        LayoutInfo layout1 = LayoutInfo.Horizontal(inspectorAndConsole, sceneAndgame, ratio: 1 / 3.0f);
        LayoutInfo layoutRoot = LayoutInfo.Horizontal(layout1, hierarchyAndproject, ratio: 3 / 4.0f);

        return layoutRoot;
    }
}
