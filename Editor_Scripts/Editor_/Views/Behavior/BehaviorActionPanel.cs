using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpaceAR.Core.Behavior;
using UnityEngine.UI;
using Battlehub.RTEditor;
using Battlehub.RTCommon;

namespace SpaceAR.Editor.Behavior
{
    public class BehaviorActionPanel : MonoBehaviour
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

        public BehaviorActionNode ActionNodePrefab;

        void OnBuild()
        {
            for (int i = 0; i < behaviorComponent.actionNodeList.Count; i++)
            {
                BehaviorActionNode newActionNode = Instantiate(ActionNodePrefab, transform);
                newActionNode.transform.SetParent(transform);
                newActionNode.transform.SetSiblingIndex(transform.childCount - 2);
                newActionNode.SetBehavior(BehaviorComponent, behaviorComponent.actionNodeList[i]);
            }

            
        }

        public void OnAddActionNode(Action action)
        {
            var subject = action.GetType().GetField("Subject");
            if (subject != null)
                subject.SetValue(action, BehaviorComponent.gameObject);

            BehaviorActionNode newActionNode = Instantiate(ActionNodePrefab, transform);
            newActionNode.transform.SetSiblingIndex(transform.childCount - 2);
            var actionNode = BehaviorComponent.AddActionNode(action);
            newActionNode.SetBehavior(BehaviorComponent, actionNode);
            //newActionNode.OnAddActionButton(action);

            Debug.Log($"<color=green> 액션패널: {action.GetActionInfo()} {action.GetHashCode()}</color>");

            IOC.Resolve<IWindowManager>().ForceLayoutUpdate();
        }

        public void OpenActionNodeContextMenu()
        {
            BehaviorActionContextMenu.OpenActionNodeContextMenu(OnAddActionNode);
        }
    }
}