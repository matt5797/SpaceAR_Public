using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SpaceAR.Core.Behavior;
using TMPro;
using System.Reflection;
using Battlehub.RTCommon;
using DG.Tweening;
using System;
using System.Linq;
using SpaceAR.Core.Utils;
using SpaceAR.Core.Model;

namespace SpaceAR.Editor.Behavior
{
    public abstract class PropertyPanel : MonoBehaviour
    {
        public abstract void UpdateProperty(FieldInfo fieldInfo);
    }
    
    public class BehaviorPropertyView : MonoBehaviour
    {
        public static BehaviorPropertyView Instance;

        BehaviorComponent _behaviorComponent;
        public BehaviorComponent BehaviorComponent
        {
            get => _behaviorComponent;
            set
            {
                _behaviorComponent = value;
            }
        }

        ActionHolder _actionHolder;
        public ActionHolder ActionHolder
        {
            get
            {
                return _actionHolder;
            }
            set
            {
                _actionHolder = value;
            }
        }
        
        public Transform contentTransform;
        public TMP_Dropdown dropdown;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
            }
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SetBehaviorAction(BehaviorComponent behaviorComponent)
        {
            BehaviorComponent = behaviorComponent;
        }
        
        public void SetBehaviorAction(BehaviorComponent behaviorComponent, ActionHolder actionHolder)
        {
            BehaviorComponent = behaviorComponent;
            ActionHolder = actionHolder;
        }

        public void OnBuild(bool isTrigger=false)
        {
            if (isTrigger)
                BuildTrigger();
            else
                BuildAction();
            BuildProperty(isTrigger);
        }

        void BuildTrigger()
        {
            dropdown.ClearOptions();
            Type sourceType = typeof(SpaceAR.Core.Behavior.Trigger);
            List<Type> typeList = Assembly.GetAssembly(sourceType).GetTypes().Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(sourceType)).ToList();

            List<string> optionList = new List<string>();
            int optionIndex = 0;
            foreach (Type type in typeList)
            {
                MethodInfo method = type.GetMethod("GetTriggerPath");
                if (method != null)
                {
                    string path = (string)method.Invoke(null, null);
                    optionList.Add(path);
                }
                else
                {
                    optionList.Add(type.Name);
                }

                if (BehaviorComponent.Trigger.GetType() == type)
                {
                    optionIndex = optionList.Count - 1;
                }
            }
            dropdown.AddOptions(optionList);
            //dropdown.value = optionIndex;
            dropdown.SetValueWithoutNotify(optionIndex);
            dropdown.onValueChanged.AddListener((int index) =>
            {
                if (typeList.Count > index)
                {
                    BehaviorComponent.Trigger = (SpaceAR.Core.Behavior.Trigger)Activator.CreateInstance(typeList[index]);
                    UnityMainThreadDispatcher.Instance().Enqueue(() => OnBuild(true));
                }
            });
        }

        void BuildAction()
        {
            dropdown.ClearOptions();
            Type sourceType = typeof(SpaceAR.Core.Behavior.Action);
            List<Type> typeList = Assembly.GetAssembly(sourceType).GetTypes().Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(sourceType)).ToList();

            List<string> optionList = new List<string>();
            int optionIndex = 0;
            foreach (Type type in typeList)
            {
                MethodInfo method = type.GetMethod("GetActionPath");
                if (method != null)
                {
                    string path = (string)method.Invoke(null, null);
                    optionList.Add(path);
                }
                else
                {
                    optionList.Add(type.Name);
                }
                
                if (ActionHolder.action.GetType() == type)
                {
                    optionIndex = optionList.Count - 1;
                }
            }
            dropdown.AddOptions(optionList);
            dropdown.SetValueWithoutNotify(optionIndex);
            dropdown.onValueChanged.AddListener((int index) =>
            {
                if (typeList.Count > index)
                {
                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        ActionHolder.action = (SpaceAR.Core.Behavior.Action)Activator.CreateInstance(typeList[index]);
                        OnBuild();
                    });
                }
            });
        }

        void BuildProperty(bool isTrigger = false)
        {
            FieldInfo[] fields;
            if (isTrigger)
            {
                fields = BehaviorComponent.Trigger.GetType().GetFields();
            }
            else
            {
                fields = ActionHolder.action.GetType().GetFields();
            }
            
            foreach (Transform child in contentTransform)
            {
                if (child != contentTransform.GetChild(0))
                {
                    Destroy(child.gameObject);
                }
            }
            
            foreach (var field in fields)
            {
                if (field.FieldType == typeof(GameObject))
                {
                    PropertyGameObjectPanel panel = Instantiate(Resources.Load<PropertyGameObjectPanel>("Editor/Behavior/Property GameObject Panel"), contentTransform);
                    panel.UpdateProperty(field);
                }
                else if (field.FieldType == typeof(float))
                {
                    PropertyFloatPanel panel = Instantiate(Resources.Load<PropertyFloatPanel>("Editor/Behavior/Property Float Panel"), contentTransform);
                    panel.UpdateProperty(field);
                }
                else if (field.FieldType == typeof(int))
                {
                    PropertyIntPanel panel = Instantiate(Resources.Load<PropertyIntPanel>("Editor/Behavior/Property Int Panel"), contentTransform);
                    panel.UpdateProperty(field);
                }
                else if (field.FieldType == typeof(Vector3))
                {
                    PropertyVector3Panel panel = Instantiate(Resources.Load<PropertyVector3Panel>("Editor/Behavior/Property Vector3 Panel"), contentTransform);
                    panel.UpdateProperty(field);
                }
                else if (field.FieldType.IsEnum)
                {
                    PropertyEnumPanel panel = Instantiate(Resources.Load<PropertyEnumPanel>("Editor/Behavior/Property Enum Panel"), contentTransform);
                    panel.UpdateProperty(field);
                }
                else if (field.FieldType == typeof(bool))
                {
                    PropertyBoolPanel panel = Instantiate(Resources.Load<PropertyBoolPanel>("Editor/Behavior/Property Bool Panel"), contentTransform);
                    panel.UpdateProperty(field);
                }
            }
        }
    }
}
