using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpaceAR.Core.Behavior;
using UnityEngine.UI;
using TMPro;
using Battlehub.RTEditor;
using Battlehub.RTCommon;

namespace SpaceAR.Editor.Behavior
{
    public class BehaviorActionNode : MonoBehaviour
    {
        BehaviorComponent behaviorComponent;
        public BehaviorComponent BehaviorComponent
        {
            get { return behaviorComponent; }
            set { behaviorComponent = value; }
        }

        ActionNode actionNode;
        public ActionNode ActionNode
        {
            get { return actionNode; }
            set { actionNode = value; }
        }

        public BehaviorActionButton ActionButtonPrefab;

        public void SetBehavior(BehaviorComponent behaviorComponent, ActionNode actionNode)
        {
            BehaviorComponent = behaviorComponent;
            ActionNode = actionNode;
            OnBuild();
        }

        void OnBuild()
        {
            for (int i = 0; i < ActionNode.actionHolderList.Count; i++)
            {
                BehaviorActionButton newActionButton = Instantiate<BehaviorActionButton>(ActionButtonPrefab, transform);
                newActionButton.transform.SetParent(transform);
                newActionButton.transform.SetSiblingIndex(transform.childCount - 2);
                newActionButton.SetBehavior(BehaviorComponent, ActionNode.actionHolderList[i]);
            }
        }

        public void OnAddActionButton(Action action)
        {
            var subject = action.GetType().GetField("Subject");
            if (subject != null)
                subject.SetValue(action, BehaviorComponent.gameObject);

            ActionHolder actionHolder = ActionNode.AddAction(action);

            BehaviorActionButton newActionButton = Instantiate<BehaviorActionButton>(ActionButtonPrefab, transform);
            newActionButton.transform.SetSiblingIndex(transform.childCount - 2);
            newActionButton.SetBehavior(BehaviorComponent, actionHolder);

            IOC.Resolve<IWindowManager>().ForceLayoutUpdate();
        }

        public void OpenActionContextMenu()
        {
            BehaviorActionContextMenu.OpenActionNodeContextMenu(OnAddActionButton);
        }
    }
}