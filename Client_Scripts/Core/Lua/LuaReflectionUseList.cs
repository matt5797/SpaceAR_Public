using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using XLua;
using Photon.Pun;

public static class LuaReflectionUseList
{
    [LuaCallCSharp]
    [ReflectionUse]
    public static List<Type> custom_lua_call_cs_list = new List<Type>()
    {
        typeof(System.Action<float>),
        typeof(System.Action<float, float>)
    };

    [LuaCallCSharp]
    [ReflectionUse]
    public static List<Type> dotween_lua_call_cs_list = new List<Type>()
    {
        typeof(DG.Tweening.AutoPlay),
        typeof(DG.Tweening.AxisConstraint),
        typeof(DG.Tweening.Ease),
        typeof(DG.Tweening.LogBehaviour),
        typeof(DG.Tweening.LoopType),
        typeof(DG.Tweening.PathMode),
        typeof(DG.Tweening.PathType),
        typeof(DG.Tweening.RotateMode),
        typeof(DG.Tweening.ScrambleMode),
        typeof(DG.Tweening.TweenType),
        typeof(DG.Tweening.UpdateType),

        typeof(DG.Tweening.DOTween),
        typeof(DG.Tweening.DOVirtual),
        typeof(DG.Tweening.EaseFactory),
        typeof(DG.Tweening.Tweener),
        typeof(DG.Tweening.Tween),
        typeof(DG.Tweening.Sequence),
        typeof(DG.Tweening.TweenParams),
        typeof(DG.Tweening.Core.ABSSequentiable),

        typeof(DG.Tweening.Core.TweenerCore<Vector3, Vector3, DG.Tweening.Plugins.Options.VectorOptions>),

        typeof(DG.Tweening.TweenCallback),
        typeof(DG.Tweening.TweenExtensions),
        typeof(DG.Tweening.TweenSettingsExtensions),
        typeof(DG.Tweening.ShortcutExtensions),
        //typeof(DG.Tweening.ShortcutExtensions43),
        //typeof(DG.Tweening.ShortcutExtensions46),
        //typeof(DG.Tweening.ShortcutExtensions50),
       
        //dotween pro ������
        //typeof(DG.Tweening.DOTweenPath),
        //typeof(DG.Tweening.DOTweenVisualManager),
    };

    [LuaCallCSharp]
    [ReflectionUse]
    public static List<Type> photon_lua_call_cs_list = new List<Type>()
    {
        typeof(Photon.Pun.RpcTarget)
    };
}
