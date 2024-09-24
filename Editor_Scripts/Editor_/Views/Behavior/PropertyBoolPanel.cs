using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SpaceAR.Core.Behavior;
using TMPro;
using System.Reflection;
using Battlehub.RTCommon;
using UnityEngine.UI;
using SpaceAR.Core.Model;

namespace SpaceAR.Editor.Behavior
{
    public class PropertyBoolPanel : PropertyPanel
    {
        public Text label;
        public Toggle toggle;

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

            toggle.isOn = (bool)fieldInfo.GetValue(BehaviorPropertyView.Instance.ActionHolder.action);
            toggle.onValueChanged.AddListener((bool value) =>
            {
                fieldInfo.SetValue(BehaviorPropertyView.Instance.ActionHolder.action, value);
            });
        }
    }
}