using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Battlehub.RTEditor.Views;
using SpaceAR.Core.Behavior;
using Battlehub.RTCommon;
using Battlehub.RTEditor;

namespace SpaceAR.Editor.Behavior
{
    public class BehaviorActionView : HierarchicalDataView
    {
        public Component[] behaviorComponents;

        public BehaviorPanel behaviorPanelPrefab;
        public Transform behaviorPanelParent;

        protected override void Start()
        {
            base.Start();

            BehaviorComponent.onListChange.AddListener(OnListChange);

            OnBuild();

        }
        
        protected virtual void Update()
        {
            ViewInput.HandleInput();
        }

        public void OnBuild()
        {
            behaviorComponents = BehaviorComponent.behaviorComponents.ToArray();
            for (int i = 0; i < behaviorComponents.Length; i++)
            {
                BehaviorPanel newBehaviorPanel = Instantiate(behaviorPanelPrefab, behaviorPanelParent);
                newBehaviorPanel.transform.SetSiblingIndex(behaviorPanelParent.childCount - 2);
                //newBehaviorPanel.GetComponentInChildren<BehaviorActionPanel>().BehaviorComponent = behaviorComponents[i] as BehaviorComponent;
                newBehaviorPanel.BehaviorComponent = behaviorComponents[i] as BehaviorComponent;
            }
        }

        void OnListChange()
        {

        }

        public void OnAddBehaviorPanel(BehaviorComponent behaviorComponent)
        {
            BehaviorPanel newActionNode = Instantiate(behaviorPanelPrefab, behaviorPanelParent);
            newActionNode.transform.SetSiblingIndex(behaviorPanelParent.childCount - 2);
            newActionNode.BehaviorComponent = behaviorComponent;

            //editor.Selection.activeGameObject

            IOC.Resolve<IWindowManager>().ForceLayoutUpdate();
        }
        
        public void OnBehaviorContextOpen()
        {
            BehaviorActionContextMenu.OpenBehaviorContextMenu(OnAddBehaviorPanel);
        }
    }
}


