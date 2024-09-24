using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using SpaceAR.Core.Lua;

namespace SpaceAR.Core.World
{
    public class InteractObject : MonoBehaviourPunCallbacks
    {
        [PunRPC]
        public void SendRPC(Player sender, string targetName, string funcName)
        {
            LuaScript[] luaScriptArray = GameObject.FindObjectsOfType<LuaScript>();

            foreach (var luaScript in luaScriptArray)
            {
                if (luaScript.gameObject.name == targetName && luaScript.photonView.Owner == sender)
                {
                    luaScript.SendMessage(funcName, SendMessageOptions.DontRequireReceiver);
                    break;
                }
            }
        }

        [PunRPC]
        public void SendRPC(Player sender, string targetName, string funcName, object param)
        {
            LuaScript[] luaScriptArray = GameObject.FindObjectsOfType<LuaScript>();

            foreach (var luaScript in luaScriptArray)
            {
                if (luaScript.gameObject.name == targetName && luaScript.photonView.Owner == sender)
                {
                    luaScript.SendMessage(funcName, param, SendMessageOptions.DontRequireReceiver);
                    break;
                }
            }
        }
    }
}
