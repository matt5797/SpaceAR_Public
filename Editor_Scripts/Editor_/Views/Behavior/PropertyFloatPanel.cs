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
    public class PropertyFloatPanel : PropertyPanel
    {
        public TMP_Text label;
        public TMP_InputField inputField;

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

            inputField.text = fieldInfo.GetValue(BehaviorPropertyView.Instance.ActionHolder.action).ToString();
            inputField.onEndEdit.AddListener((string value) =>
            {
                fieldInfo.SetValue(BehaviorPropertyView.Instance.ActionHolder.action, float.Parse(value));
            });
        }
    }
}