using UnityEngine;
using System.Reflection;
using Battlehub.Utils;
using Battlehub.RTCommon;

using SpaceAR.Core.Lua;
using Battlehub.RTEditor;
using Battlehub.RTSL.Interface;

namespace SpaceAR.Editor.Descriptor
{
    public class LuaScriptComponentDescriptor : ComponentDescriptorBase<LuaScript>
    {
        public override object CreateConverter(ComponentEditor editor)
        {
            object[] converters = new object[editor.Components.Length];
            Component[] components = editor.Components;
            for (int i = 0; i < components.Length; ++i)
            {
                LuaScript luaScript = (LuaScript)components[i];
                ExposeToEditor test = luaScript.GetComponent<ExposeToEditor>();
                if (luaScript != null)
                {
                    converters[i] = new LuaScriptPropertyConverter
                    {
                        LuaScript = luaScript.GetComponent<LuaScript>()
                    };
                }
            }
            return converters;
        }

        public override PropertyDescriptor[] GetProperties(
            ComponentEditor editor, object converter)
        {
            object[] converters = (object[])converter;

            MemberInfo luaScript = Strong.PropertyInfo(
                (LuaScript x) => x.luaScript, "luaScript");
            MemberInfo luaScriptConverted = Strong.PropertyInfo(
                (LuaScriptPropertyConverter x) => x.runtimeLuaScript, "luaScript");

            return new[]
            {
                new PropertyDescriptor( "LuaScript", converters, luaScriptConverted, luaScript)
            };
        }
    }

    public class LuaScriptPropertyConverter
    {
        //private ISettingsComponent m_settingsComponent = IOC.Resolve<ISettingsComponent>();

        RuntimeTextAsset _runtimeLuaScript;
        public RuntimeTextAsset runtimeLuaScript
        {
            get
            {
                if (_runtimeLuaScript == null && LuaScript.luaScript != null)
                {
                    _runtimeLuaScript = ScriptableObject.CreateInstance<RuntimeTextAsset>();
                    _runtimeLuaScript.Text = LuaScript.luaScript.text;
                    LuaScript.LoadScript();
                }
                return _runtimeLuaScript;
            }
            set
            {
                _runtimeLuaScript = value;
                if (LuaScript != null)
                {
                    LuaScript.luaScript = new TextAsset(_runtimeLuaScript.Text);
                    LuaScript.LoadScript();
                }
            }
        }
        
        public TextAsset luaScript
        {
            get
            {
                /*if (ExposeToEditor == null)
                {
                    return LuaScript.luaScript;
                }

                return ExposeToEditor.LocalPosition;*/
                return LuaScript.luaScript;
            }
            set
            {
                /*if (ExposeToEditor == null)
                {
                    LuaScript.luaScript = value;
                    return;
                }
                ExposeToEditor.LocalPosition = value;*/
                LuaScript.luaScript = value;
            }
        }

        public LuaScript LuaScript
        {
            get;
            set;
        }
    }
}