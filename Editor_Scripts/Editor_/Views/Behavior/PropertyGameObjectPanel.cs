using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SpaceAR.Core.Behavior;
using TMPro;
using System.Reflection;
using Battlehub.RTCommon;
using SpaceAR.Core.Model;

namespace SpaceAR.Editor.Behavior
{
    public class PropertyGameObjectPanel : PropertyPanel
    {
        public TMP_Text label;
        public TMP_Dropdown dropdown;

        public override void UpdateProperty(FieldInfo fieldInfo)
        {
            var testAttributes = fieldInfo.GetCustomAttributes();
            var testAttributes2 = fieldInfo.Attributes;

            DisplayAttribute displayAttribute = fieldInfo.GetCustomAttribute<DisplayAttribute>();
            if (displayAttribute != null)
            {
                label.text = displayAttribute.Name;
            }
            else
            {
                label.text = fieldInfo.Name;
            }
            dropdown.ClearOptions();
            List<string> options = new List<string>() { "None" };
            foreach (var go in GameObject.FindObjectsOfType<ExposeToEditor>())
            {
                if (go.TryGetComponent<ExposeToEditor>(out _))
                {
                    options.Add(go.name);
                }
            }
            dropdown.AddOptions(options);
            
            dropdown.onValueChanged.AddListener((int value) =>
            {
                //string res = $"<color=red>{BehaviorPropertyView.Instance.ActionHolder.action.GetActionInfo()} {BehaviorPropertyView.Instance.ActionHolder.action.GetHashCode()}</color>\n";
                fieldInfo.SetValue(BehaviorPropertyView.Instance.ActionHolder.action, GameObject.Find(options[value]));
                //res += $"<color=blue>{BehaviorPropertyView.Instance.ActionHolder.action.GetActionInfo()} {BehaviorPropertyView.Instance.ActionHolder.action.GetHashCode()}</color>";
                //Debug.Log(res);
            });

            int index;
            try
            {
                Debug.Log($"<color=green> ÇÁ·ÎÆÛÆ¼ºä: {BehaviorPropertyView.Instance.ActionHolder.action.GetActionInfo()} {BehaviorPropertyView.Instance.ActionHolder.action.GetHashCode()}</color>");
                var value = fieldInfo.GetValue(BehaviorPropertyView.Instance.ActionHolder.action);
                if (value == null)
                    index = 0;
                else
                {
                    GameObject go = value as GameObject;
                    index = options.IndexOf(go.name);
                }
            }
            catch (System.Exception e) 
            {
                Debug.LogException(e);
                index = 0; 
            }
            //dropdown.value = index;
            dropdown.SetValueWithoutNotify(index);
        }
    }
}
