using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SpaceAR.Core.Behavior;
using TMPro;

namespace SpaceAR.Editor.Behavior
{
    public class BehaviorActionButton : MonoBehaviour
    {
        public TMP_Text text;
        public string Text { get { return text.text; } set { text.text = value; } }
        
        BehaviorComponent _behaviorComponent;
        public BehaviorComponent BehaviorComponent
        {
            get { return _behaviorComponent; }
            set { _behaviorComponent = value; }
        }
        
        ActionHolder _action;
        public ActionHolder ActionHolder
        {
            get
            {
                return _action;
            }
            set 
            {
                _action = value;
                Text = _action.GetActionType();
            }
        }

        public void SetBehavior(BehaviorComponent behaviorComponent, ActionHolder actionHolder)
        {
            BehaviorComponent = behaviorComponent;
            ActionHolder = actionHolder;
        }

        public void OnButtonClick()
        {
            if (BehaviorPropertyView.Instance != null)
            {
                BehaviorPropertyView.Instance.SetBehaviorAction(BehaviorComponent, ActionHolder);
                BehaviorPropertyView.Instance.OnBuild();
            }
        }
    }
}