using Battlehub.RTCommon;
using Battlehub.RTEditor;
using Battlehub.UIControls.MenuControl;
using SpaceAR.Core.Editor;
using UnityEngine;

namespace SpaceAR.Editor.Views.AssetDown
{
    [MenuDefinition]
    public class RegisterAssetDownWindow : EditorExtension
    {
        [SerializeField]
        private GameObject m_prefab = null;

        [SerializeField]
        private Sprite m_icon = null;
		
        [SerializeField]
        private string m_header = "AssetDown";

        [SerializeField]
        private bool m_isDialog = false;
		
        [SerializeField]
        private int m_maxWindows = 1;

        protected override void OnInit()
        {
            base.OnInit();

            IWindowManager wm = IOC.Resolve<IWindowManager>();
            wm.RegisterWindow("AssetDown", m_header, m_icon, m_prefab, m_isDialog, m_maxWindows);
        }

        [MenuCommand("Asset/Asset Download", "WorldDownload", priority: 10)]
        public static void Open()
        {
            IWindowManager wm = IOC.Resolve<IWindowManager>();
            // wm.CreateWindow("AssetDown");
            EditorManager.Instance.WindowCreateResize("AssetDown", 700, 500, 700, 500);
        }
    }
}



