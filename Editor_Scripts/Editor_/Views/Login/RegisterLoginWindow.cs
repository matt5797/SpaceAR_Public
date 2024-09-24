using Battlehub.RTCommon;
using Battlehub.RTEditor;
using Battlehub.UIControls.MenuControl;
using UnityEngine;

using SpaceAR.Core.Editor;
using SpaceAR.Core.AWS;

namespace SpaceAR.Editor.Views.Login
{
    [MenuDefinition]
    public class RegisterLoginWindow : EditorExtension
    {
        [SerializeField]
        private GameObject m_prefab = null;

        [SerializeField]
        private Sprite m_icon = null;
		
        [SerializeField]
        private string m_header = "Login";

        [SerializeField]
        private bool m_isDialog = false;
		
        [SerializeField]
        private int m_maxWindows = -1;

        protected override void OnInit()
        {
            base.OnInit();

            IWindowManager wm = IOC.Resolve<IWindowManager>();
            wm.RegisterWindow("Login", m_header, m_icon, m_prefab, m_isDialog, m_maxWindows);
        }

        [MenuCommand("Account/Log In", "LogIn", priority: 10)]
        public void Open()
        {
            IWindowManager wm = IOC.Resolve<IWindowManager>();
            //wm.CreateWindow("Login");
            //WindowCreateResize
            EditorManager.Instance.WindowCreateResize("Login", 620, 540, 620, 540);
        }
    }
}



