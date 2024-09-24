using Battlehub.RTCommon;
using Battlehub.RTEditor;
using Battlehub.UIControls.MenuControl;
using UnityEngine;

namespace SpaceAR.Editor.Views.Behavior
{
    [MenuDefinition]
    public class RegisterBehaviorWindow : EditorExtension
    {
        [SerializeField]
        private GameObject m_prefab = null;

        [SerializeField]
        private Sprite m_icon = null;
		
        [SerializeField]
        private string m_header = "Behavior";

        [SerializeField]
        private bool m_isDialog = false;
		
        [SerializeField]
        private int m_maxWindows = 1;

        protected override void OnInit()
        {
            base.OnInit();

            IWindowManager wm = IOC.Resolve<IWindowManager>();
            wm.RegisterWindow("Behavior", m_header, m_icon, m_prefab, m_isDialog, m_maxWindows);
        }

        //[MenuCommand("MenuWindow/Behavior", "")]
        [MenuCommand("MenuWindow/General/Behavior", "RTE_View_Behavior", priority: 10)]
        public static void Open()
        {
            IWindowManager wm = IOC.Resolve<IWindowManager>();
            wm.CreateWindow("Behavior");
        }
    }
}



