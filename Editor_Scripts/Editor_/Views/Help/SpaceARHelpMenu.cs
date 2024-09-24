using Battlehub.RTCommon;
using Battlehub.UIControls.MenuControl;
using UnityEngine;

using Battlehub.RTEditor;

namespace SpaceAR.Editor.Views.Help
{
    [MenuDefinition(order: -50)]
    public class SpaceARHelpMenu : MonoBehaviour
    {
        private IRuntimeEditor Editor
        {
            get { return IOC.Resolve<IRuntimeEditor>(); }
        }

        [MenuCommand("MenuHelp/SpaceAR", priority: 10)]
        public void Help()
        {
            Editor.CreateOrActivateWindow(RuntimeWindowType.About.ToString());
        }
    }

}

