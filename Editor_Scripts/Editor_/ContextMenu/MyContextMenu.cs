using Battlehub.RTCommon;
using Battlehub.RTEditor;
using Battlehub.UIControls.MenuControl;
using UnityEngine;

namespace SpaceAR.Editor.ContextMenu
{
    public static class MyContextMenu
    {
        public static void OpenContextMenu()
        {
            IContextMenu contextMenu = IOC.Resolve<IContextMenu>();

            MenuItemInfo cmd1 = new MenuItemInfo { Path = "My Command 1" };
            cmd1.Action = new MenuItemEvent();
            cmd1.Action.AddListener((args) =>
            {
                Debug.Log("Run My Command1");

                IRuntimeEditor editor = IOC.Resolve<IRuntimeEditor>();
                Debug.Log(editor.Selection.activeGameObject);
            });

            MenuItemInfo cmd2 = new MenuItemInfo { Path = "My Command 2" };
            cmd2.Validate = new MenuItemValidationEvent();
            cmd2.Action = new MenuItemEvent();
            cmd2.Validate.AddListener((args) =>
            {
                args.IsValid = false;
            });

            cmd2.Action.AddListener((args) =>
            {
                Debug.Log("Run My Command2");
            });

            contextMenu.Open(new[]
            {
            cmd1, cmd2
        });
        }
    }
}