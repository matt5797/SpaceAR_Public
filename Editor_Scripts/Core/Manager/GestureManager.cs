//
// Fingers Gestures
// (c) 2015 Digital Ruby, LLC
// Source code may be used for personal or commercial projects.
// Source code may NOT be redistributed or sold.
// 

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using DigitalRubyShared;
using SpaceAR.Core.Behavior;

interface IGesture
{
    void OnTap();
    void OnLongPressBegan(float screenX, float screenY);
    void OnLongPressExecuting(float screenX, float screenY);
    void OnLongPressEnded(float velocityXScreen, float velocityYScreen);
    void OnRotate(float angleDelta);
}

public class GestureManager : MonoBehaviour
{
    public static GameObject TargetObject
    {
        get; set;
    }
    public static List<GameObject> TargetObjects
    {
        get; set;
    }

    private TapGestureRecognizer tapGesture;
    private TapGestureRecognizer doubleTapGesture;
    private TapGestureRecognizer tripleTapGesture;
    private SwipeGestureRecognizer swipeGesture;
    private PanGestureRecognizer panGesture;
    private ScaleGestureRecognizer scaleGesture;
    private RotateGestureRecognizer rotateGesture;
    private LongPressGestureRecognizer longPressGesture;

    private readonly List<Vector3> swipeLines = new List<Vector3>();
    
    private void TapGestureCallback(GestureRecognizer gesture)
    {
        if (gesture.State == GestureRecognizerState.Ended)
        {
            if (TargetObjects.Count() <= 0)
                return;
            foreach (var target in TargetObjects)
            {
                if (target == null)
                    continue;
                foreach (IGesture component in target.GetComponents<IGesture>())
                {
                    component.OnTap();
                }
            }
            TargetObjects.Clear();
        }
    }

    private void CreateTapGesture()
    {
        tapGesture = new TapGestureRecognizer();
        tapGesture.StateUpdated += TapGestureCallback;
        tapGesture.RequireGestureRecognizerToFail = doubleTapGesture;
        FingersScript.Instance.AddGesture(tapGesture);
    }

    private void RotateGestureCallback(GestureRecognizer gesture)
    {
        if (gesture.State == GestureRecognizerState.Executing)
        {
            //Earth.transform.Rotate(0.0f, 0.0f, rotateGesture.RotationRadiansDelta * Mathf.Rad2Deg);
            //OnRotate(rotateGesture.RotationRadiansDelta);

            if (TargetObjects.Count() <= 0)
                return;
            foreach (var target in TargetObjects)
            {
                foreach (IGesture component in target.GetComponents<IGesture>())
                {
                    component.OnRotate(rotateGesture.RotationRadiansDelta);
                }
            }
            TargetObjects.Clear();
        }
    }

    private void CreateRotateGesture()
    {
        rotateGesture = new RotateGestureRecognizer();
        rotateGesture.StateUpdated += RotateGestureCallback;
        FingersScript.Instance.AddGesture(rotateGesture);
    }

    private void LongPressGestureCallback(GestureRecognizer gesture)
    {
        if (gesture.State == GestureRecognizerState.Began)
        {
            /*DebugText("Long press began: {0}, {1}", gesture.FocusX, gesture.FocusY);
            BeginDrag(gesture.FocusX, gesture.FocusY);*/
            //OnLongPressBegan()
            if (TargetObjects.Count() <= 0)
                return;
            foreach (var target in TargetObjects)
            {
                if (target == null)
                    continue;
                foreach (IGesture component in target.GetComponents<IGesture>())
                {
                    component.OnLongPressBegan(gesture.FocusX, gesture.FocusY);
                }
            }
            TargetObjects.Clear();
        }
        else if (gesture.State == GestureRecognizerState.Executing)
        {
            /*DebugText("Long press moved: {0}, {1}", gesture.FocusX, gesture.FocusY);
            DragTo(gesture.FocusX, gesture.FocusY);*/
            if (TargetObjects.Count() <= 0)
                return;
            foreach (var target in TargetObjects)
            {
                if (target == null)
                    continue;
                foreach (IGesture component in target.GetComponents<IGesture>())
                {
                    component.OnLongPressExecuting(gesture.FocusX, gesture.FocusY);
                }
            }
            TargetObjects.Clear();
        }
        else if (gesture.State == GestureRecognizerState.Ended)
        {
            /*DebugText("Long press end: {0}, {1}, delta: {2}, {3}", gesture.FocusX, gesture.FocusY, gesture.DeltaX, gesture.DeltaY);
            EndDrag(longPressGesture.VelocityX, longPressGesture.VelocityY);*/
            if (TargetObjects.Count() <= 0)
                return;
            foreach (var target in TargetObjects)
            {
                if (target == null)
                    continue;
                foreach (IGesture component in target.GetComponents<IGesture>())
                {
                    component.OnLongPressEnded(longPressGesture.VelocityX, longPressGesture.VelocityY);
                }
            }
            TargetObjects.Clear();
        }
    }

    private void CreateLongPressGesture()
    {
        longPressGesture = new LongPressGestureRecognizer();
        longPressGesture.MaximumNumberOfTouchesToTrack = 1;
        longPressGesture.StateUpdated += LongPressGestureCallback;
        FingersScript.Instance.AddGesture(longPressGesture);
    }

    private static bool? CaptureGestureHandler(GameObject obj)
    {
        //Debug.Log($"CaptureGestureHandler {obj}");
        TargetObjects.Add(obj);
        // I've named objects PassThrough* if the gesture should pass through and NoPass* if the gesture should be gobbled up, everything else gets default behavior
        if (obj.name.StartsWith("PassThrough"))
        {
            // allow the pass through for any element named "PassThrough*"
            return false;
        }
        else if (obj.name.StartsWith("NoPass"))
        {
            // prevent the gesture from passing through, this is done on some of the buttons and the bottom text so that only
            // the triple tap gesture can tap on it
            return true;
        }

        // fall-back to default behavior for anything else
        return null;
    }

    void Awake()
    {
        TargetObjects = new List<GameObject>();
    }

    private void Start()
    {
        // don't reorder the creation of these :)
        CreateTapGesture();
        CreateRotateGesture();
        CreateLongPressGesture();

        // pan, scale and rotate can all happen simultaneously
        /*panGesture.AllowSimultaneousExecution(scaleGesture);
        panGesture.AllowSimultaneousExecution(rotateGesture);
        scaleGesture.AllowSimultaneousExecution(rotateGesture);*/

        // prevent the one special no-pass button from passing through,
        //  even though the parent scroll view allows pass through (see FingerScript.PassThroughObjects)
        FingersScript.Instance.CaptureGestureHandler = CaptureGestureHandler;
    }

}
