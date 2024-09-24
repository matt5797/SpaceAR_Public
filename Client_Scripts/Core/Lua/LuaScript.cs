using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XLua;
using System;

using Photon.Pun;

namespace SpaceAR.Core.Lua
{
    [LuaCallCSharp]
    public class LuaScript : MonoBehaviourPunCallbacks, IGesture
    {
        public TextAsset _luaScript;
        public TextAsset luaScript
        {
            get { return _luaScript; }
            set { _luaScript = value; }
        }

        internal static LuaEnv luaEnv; //all lua behaviour shared one luaenv only!
        internal static float lastGCTime = 0;
        internal const float GCInterval = 1;//1 second 

        public enum LualifeCycle
        {
            Awake,
            Start,
            Update,
            OnDestroy,
            OnEnable,
            OnDisable,
        }

        LualifeCycle lualifeCycle;
        private Action luaAction;
        private Action luaOnEnable;
        private Action luaOnDisable;
        private Action luaOnDestroy;
        private Action<object> luaOnTriggerEnter;
        private Action<object> luaOnTriggerStay;
        private Action<object> luaOnTriggerExit;
        private Action<object> luaOnCollisionEnter;
        private Action<object> luaOnCollisionStay;
        private Action<object> luaOnCollisionExit;

        private Action luaOnClick;

        private Action luaOnTap;
        private Action<object, object> luaOnLongPressBegan;
        private Action<object, object> luaOnLongPressExecuting;
        private Action<object, object> luaOnLongPressEnded;
        private Action<object> luaOnRotate;

        private LuaTable scriptEnv;

        #region lua life cycle
        private void Awake()
        {
            if (luaEnv == null)
                luaEnv = new LuaEnv();

            if (luaScript != null)
                LoadScript();
        }
        
        public override void OnEnable()
        {
            base.OnEnable();
            luaOnEnable?.Invoke();
        }
        
        public override void OnDisable()
        {
            base.OnDisable();
            luaOnDisable?.Invoke();
        }

        private void OnDestroy()
        {
            luaOnDestroy?.Invoke();
            scriptEnv?.Dispose();
        }

        void OnTriggerEnter(Collider other)
        {
            luaOnTriggerEnter?.Invoke(other);
        }
        
        void OnTriggerStay(Collider other)
        {
            luaOnTriggerStay?.Invoke(other);
        }
        
        void OnTriggerExit(Collider other)
        {
            luaOnTriggerExit?.Invoke(other);
        }

        void OnCollisionEnter(Collision collision)
        {
            luaOnCollisionEnter?.Invoke(collision);
            //collision.gameObject.GetComponent<LuaScript>()?.luaOnCollisionEnter?.Invoke(collision);
        }

        void OnCollisionStay(Collision collision)
        {
            luaOnCollisionStay?.Invoke(collision);
        }

        void OnCollisionExit(Collision collision)
        {
            luaOnCollisionExit?.Invoke(collision);
        }

        public void OnClick()
        {
            luaOnClick?.Invoke();
        }

        IEnumerator OnUpdate()
        {
            yield return null;
            while (true)
            {
#if SPACEAR_EDITOR
                if (!m_isRuntimeEditorStart)
                {
                    yield return null;
                    continue;
                }
#endif
                if (luaAction != null)
                {
                    luaAction();
                }
                if (Time.time - LuaScript.lastGCTime > GCInterval)
                {
                    luaEnv.Tick();
                    LuaScript.lastGCTime = Time.time;
                }
                if (lualifeCycle == LualifeCycle.Awake)
                {
                    scriptEnv.Get("Start", out luaAction);
                }
                else if (lualifeCycle == LualifeCycle.Start)
                {
                    scriptEnv.Get("Update", out luaAction);
                }
                if (lualifeCycle == LualifeCycle.Awake)
                {
                    lualifeCycle = LualifeCycle.Start;
                }

                yield return null;
            }
        }
        #endregion

        #region lua script
        public void LoadScript()
        {
            StopAllCoroutines();
            //luaEnv = new LuaEnv();

            scriptEnv = luaEnv.NewTable();
            LuaTable meta = luaEnv.NewTable();
            meta.Set("__index", luaEnv.Global);
            scriptEnv.SetMetaTable(meta);
            meta.Dispose();

            scriptEnv.Set("self", this);

            luaEnv.DoString(luaScript.text, luaScript.name, scriptEnv);

            luaAction = scriptEnv.Get<Action>("Awake");
            lualifeCycle = LualifeCycle.Awake;

            // Lifecycle
            scriptEnv.Get("OnEnable", out luaOnEnable);
            scriptEnv.Get("OnDisable", out luaOnDisable);
            scriptEnv.Get("OnDestroy", out luaOnDestroy);
            
            luaOnTriggerEnter = scriptEnv.Get<string, Action<object>>("OnTriggerEnter");
            luaOnTriggerStay = scriptEnv.Get<string, Action<object>>("OnTriggerStay");
            luaOnTriggerExit = scriptEnv.Get<string, Action<object>>("OnTriggerExit");
            
            luaOnCollisionEnter = scriptEnv.Get<string, Action<object>>("OnCollisionEnter");
            luaOnCollisionStay = scriptEnv.Get<string, Action<object>>("OnCollisionStay");
            luaOnCollisionExit = scriptEnv.Get<string, Action<object>>("OnCollisionExit");

            // Custom
            luaOnClick = scriptEnv.Get<Action>("OnClick");

            // Gesture
            luaOnTap = scriptEnv.Get<Action>("OnTap");
            luaOnLongPressBegan = scriptEnv.Get<string, Action<object, object>>("OnLongPressBegan");
            luaOnLongPressExecuting = scriptEnv.Get<string, Action<object, object>>("OnLongPressExecuting");
            luaOnLongPressEnded = scriptEnv.Get<string, Action<object, object>>("OnLongPressEnded");
            luaOnRotate = scriptEnv.Get<string, Action<object>>("OnRotate");

            StartCoroutine(OnUpdate());
        }

        public void ExecuteFunc(string funcName)
        {
#if SPACEAR_EDITOR
            if (!m_isRuntimeEditorStart)
                return;
#endif
            Action thisfunc = scriptEnv.Get<Action>(funcName);
            thisfunc();
        }

        public void ExecuteFunc(string funcName, object[] param)
        {
#if SPACEAR_EDITOR
            if (!m_isRuntimeEditorStart)
                return;
#endif
            Action<object> thisfunc = scriptEnv.Get<Action<object>>(funcName);
            thisfunc(param);
        }

        public void ExecuteFunc(object[] param)
        {
#if SPACEAR_EDITOR
            if (!m_isRuntimeEditorStart)
                return;
#endif
            object[] parameters = param[1] as object[];
            switch (parameters.Length)
            {
                case 0:
                    scriptEnv.Get<Action>(param[0].ToString())();
                    break;
                case 1:
                    scriptEnv.Get<Action<object>>(param[0].ToString())(parameters[0]);
                    break;
                case 2:
                    scriptEnv.Get<Action<object, object>>(param[0].ToString())(parameters[0], parameters[1]);
                    break;
                case 3:
                    scriptEnv.Get<Action<object, object, object>>(param[0].ToString())(parameters[0], parameters[1], parameters[2]);
                    break;
                case 4:
                    scriptEnv.Get<Action<object, object, object, object>>(param[0].ToString())(parameters[0], parameters[1], parameters[2], parameters[3]);
                    break;
                case 5:
                    scriptEnv.Get<Action<object, object, object, object, object>>(param[0].ToString())(parameters[0], parameters[1], parameters[2], parameters[3], parameters[4]);
                    break;
                case 6:
                    scriptEnv.Get<Action<object, object, object, object, object, object>>(param[0].ToString())(parameters[0], parameters[1], parameters[2], parameters[3], parameters[4], parameters[5]);
                    break;
            }
        }
        #endregion

        #region Photon
        public void RPC(string targetName, string funcName, RpcTarget rpcTarget = RpcTarget.All)
        {
            try
            {
                photonView.RPC("SendRPC", rpcTarget, PhotonNetwork.LocalPlayer, targetName, funcName);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public void RPC(string targetName, string funcName, object param, RpcTarget rpcTarget = RpcTarget.All)
        {
            LuaTable luaTable;
            object[] objects;
            try
            {
                if (param is LuaTable)
                {
                    luaTable = param as LuaTable;
                    objects = luaTable.Cast<object[]>();
                    photonView.RPC("SendRPC", rpcTarget, PhotonNetwork.LocalPlayer, targetName, funcName, objects);
                }
                else
                {
                    photonView.RPC("SendRPC", rpcTarget, PhotonNetwork.LocalPlayer, targetName, funcName, param);
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public void RPCLua(string luaFuncName, RpcTarget rpcTarget = RpcTarget.All)
        {
            try
            {
                photonView.RPC("SendRPC", rpcTarget, PhotonNetwork.LocalPlayer, gameObject.name, nameof(ExecuteFunc), luaFuncName);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public void RPCLua(string luaFuncName, object param, RpcTarget rpcTarget = RpcTarget.All)
        {
            LuaTable luaTable;
            object[] objects;
            object[] parameters;
            try
            {
                if (param is LuaTable)
                {
                    luaTable = param as LuaTable;
                    objects = luaTable.Cast<object[]>();
                    parameters = new object[2] { luaFuncName, objects };
                    photonView.RPC("SendRPC", rpcTarget, PhotonNetwork.LocalPlayer, gameObject.name, nameof(ExecuteFunc), parameters);
                }
                else
                {
                    photonView.RPC("SendRPC", rpcTarget, PhotonNetwork.LocalPlayer, gameObject.name, nameof(ExecuteFunc), luaFuncName);
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
        #endregion

        #region Gesture
        void IGesture.OnTap()
        {
            if (luaOnTap != null)
            {
                luaOnTap();
            }
        }

        void IGesture.OnLongPressBegan(float screenX, float screenY)
        {
            //print($"OnLongPressBegan {screenX} {screenY}");
            luaOnLongPressBegan?.Invoke(screenX, screenY);
        }

        void IGesture.OnLongPressExecuting(float screenX, float screenY)
        {
            //print($"OnLongPressExecuting {screenX} {screenY}");
            luaOnLongPressExecuting?.Invoke(screenX, screenY);
        }

        void IGesture.OnLongPressEnded(float velocityXScreen, float velocityYScreen)
        {
            //print($"OnLongPressEnded {velocityXScreen} {velocityYScreen}");
            luaOnLongPressEnded?.Invoke(velocityXScreen, velocityYScreen);
        }

        void IGesture.OnRotate(float angleDelta)
        {
            //print($"OnRotate {angleDelta}");
            luaOnRotate?.Invoke(angleDelta);
        }
        #endregion
        
        #region Runtime Editor
#if SPACEAR_EDITOR
        static bool m_isRuntimeEditorStart = false;

        private void RuntimeStart()
        {
            //Debug.Log("Start in play mode");
            m_isRuntimeEditorStart = true;
        }
        private void OnRuntimeDestroy()
        {
            //Debug.Log("Destroy in play mode");
            m_isRuntimeEditorStart = false;
        }
#endif
        #endregion
    }
}
