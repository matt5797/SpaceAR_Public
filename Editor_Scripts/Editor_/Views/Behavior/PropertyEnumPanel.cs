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
    public class PropertyEnumPanel : PropertyPanel
    {
        public TMP_Text label;
        public TMP_Dropdown dropdown;

        public override void UpdateProperty(FieldInfo fieldInfo)
        {
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
            List<string> options = new List<string>();
            foreach (var item in fieldInfo.FieldType.GetEnumNames())
            {
                options.Add(item);
            }
            dropdown.AddOptions(options);
            dropdown.value = (int)fieldInfo.GetValue(BehaviorPropertyView.Instance.ActionHolder.action);
            //dropdown.value = options.IndexOf(fieldInfo.GetValue(BehaviorPropertyView.Instance.Action).ToString());

            dropdown.onValueChanged.AddListener((int value) =>
            {
                fieldInfo.SetValue(BehaviorPropertyView.Instance.ActionHolder.action, value);
            });
        }
    }
}