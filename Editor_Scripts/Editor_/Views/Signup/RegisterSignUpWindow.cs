using Battlehub.RTCommon;
using Battlehub.RTEditor;
using Battlehub.UIControls.MenuControl;
using SpaceAR.Core.Editor;
using UnityEngine;

namespace SpaceAR.Editor.Views.Signup
{
    [MenuDefinition]
    public class RegisterSignUpWindow : EditorExtension
    {
        [SerializeField]
        private GameObject m_prefab = null;

        [SerializeField]
        private Sprite m_icon = null;
		
        [SerializeField]
        private string m_header = "SignUp";

        [SerializeField]
        private bool m_isDialog = false;
		
        [SerializeField]
        private int m_maxWindows = -1;

        protected override void OnInit()
        {
            base.OnInit();

            IWindowManager wm = IOC.Resolve<IWindowManager>();
            wm.RegisterWindow("SignUp", m_header, m_icon, m_prefab, m_isDialog, m_maxWindows);
        }

        [MenuCommand("Account/Sing Up", "Singup", priority: 20)]
        public void Open()
        {
            IWindowManager wm = IOC.Resolve<IWindowManager>();
            //wm.CreateWindow("SignUp");
            EditorManager.Instance.WindowCreateResize("SignUp", 620, 540, 620, 540);

        }
    }
}



