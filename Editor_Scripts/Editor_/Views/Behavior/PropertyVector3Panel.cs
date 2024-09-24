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
    public class PropertyVector3Panel : PropertyPanel
    {
        public TMP_Text label;
        public TMP_InputField inputField_X;
        public TMP_InputField inputField_Y;
        public TMP_InputField inputField_Z;

        Vector3 vector3;

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

            vector3 = (Vector3)fieldInfo.GetValue(BehaviorPropertyView.Instance.ActionHolder.action);
            inputField_X.text = vector3.x.ToString();
            inputField_Y.text = vector3.y.ToString();
            inputField_Z.text = vector3.z.ToString();

            inputField_X.onEndEdit.AddListener((string value) =>
            {
                vector3.x = float.Parse(value);
                fieldInfo.SetValue(BehaviorPropertyView.Instance.ActionHolder.action, vector3);
            });
            inputField_Y.onEndEdit.AddListener((string value) =>
            {
                vector3.y = float.Parse(value);
                fieldInfo.SetValue(BehaviorPropertyView.Instance.ActionHolder.action, vector3);
            });
            inputField_Z.onEndEdit.AddListener((string value) =>
            {
                vector3.z = float.Parse(value);
                fieldInfo.SetValue(BehaviorPropertyView.Instance.ActionHolder.action, vector3);
            });
        }
    }
}