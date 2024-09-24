using Battlehub.RTCommon;
using Battlehub.Utils;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SpaceAR.Editor.Views.Login
{
    public class LoginWindow : RuntimeWindow
    {
        protected override void AwakeOverride()
        {
            WindowType = RuntimeWindowType.Custom;
            base.AwakeOverride();
        }

        protected override void OnDestroyOverride()
        {
            base.OnDestroyOverride();
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            Debug.Log("On Custom Window Activated");
        }

        protected override void OnDeactivated()
        {
            base.OnDeactivated();
            Debug.Log("On Custom Window Deactivated");
        }
    }
}
