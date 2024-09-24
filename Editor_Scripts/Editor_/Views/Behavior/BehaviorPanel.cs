using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpaceAR.Core.Behavior;
using UnityEngine.UI;

using TMPro;

namespace SpaceAR.Editor.Behavior
{
    public class BehaviorPanel : MonoBehaviour
    {
        BehaviorComponent behaviorComponent;

        public BehaviorComponent BehaviorComponent
        {
            get { return behaviorComponent; }
            set
            {
                behaviorComponent = value;
                OnBuild();
            }
        }
        
        public BehaviorActionPanel actionPanel;
        public TMP_Text TriggerNameText;
        
        void OnBuild()
        {
            TriggerNameText.text = $" {BehaviorComponent.name} : {BehaviorComponent.Trigger.Name} ";
            actionPanel.BehaviorComponent = BehaviorComponent;
        }

        public void OnButtonClick()
        {
            if (BehaviorPropertyView.Instance != null)
            {
                BehaviorPropertyView.Instance.SetBehaviorAction(BehaviorComponent);
                BehaviorPropertyView.Instance.OnBuild(true);
            }
        }
    }
}