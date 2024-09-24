using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;
using System.Threading.Tasks;
using UnityEngine.Events;
using SpaceAR.Core.Model;
using System;

namespace SpaceAR.Core.Behavior
{
    [System.Serializable]
    public class ActionHolder
    {
        [SerializeReference] public Action action;
        [SerializeReference] ActionNode node;
        public ActionNode Node
        {
            get { return node; }
            set { node = value; }
        }
        /// <summary>
        /// Do not use this constructor. It is for serialization.
        /// </summary>
        public ActionHolder()
        {
            action = new ActionMove();
        }
        public ActionHolder(Action action, ActionNode node)
        {
            this.action = action;
            this.Node = node;
        }
        public void SetAction(Action action)
        {
            this.action = action;
        }

        public void SetNode(ActionNode node)
        {
            this.Node = node;
        }

        public void Remove()
        {
            Node.RemoveActionHolder(this);
            action = null;
        }

        public string GetActionType()
        {
            return action.GetActionType();
        }
    }

    [System.Serializable]
    public class ActionNode
    {
        [SerializeField] public List<ActionHolder> actionHolderList;
        [SerializeReference] BehaviorComponent behaviorComponent;
        [SerializeField] 
        public BehaviorComponent BehaviorComponent
        {
            get { return behaviorComponent; }
            set { behaviorComponent = value; }
        }

        /// <summary>
        /// Do not use this constructor. It is for serialization.
        /// </summary>
        public ActionNode()
        {
            actionHolderList = new List<ActionHolder>();
        }
        public ActionNode(BehaviorComponent behaviorComponent)
        {
            this.actionHolderList = new List<ActionHolder>();
            this.BehaviorComponent = behaviorComponent;
        }
        public ActionNode(List<ActionHolder> actionHolderList, BehaviorComponent behaviorComponent)
        {
            this.actionHolderList = actionHolderList;
            this.BehaviorComponent = behaviorComponent;
        }
        public ActionNode(Action action, BehaviorComponent behaviorComponent)
        {
            this.actionHolderList = new List<ActionHolder>();
            this.actionHolderList.Add(new ActionHolder(action, this));
            this.BehaviorComponent = behaviorComponent;
        }
        
        public void AddAction(ActionHolder actionHolder)
        {
            this.actionHolderList.Add(actionHolder);
        }

        public ActionHolder AddAction(Action action)
        {
            var actionHolder = new ActionHolder(action, this);
            this.actionHolderList.Add(actionHolder);
            return actionHolder;
        }
        
        public void RemoveActionHolder(ActionHolder actionHolder)
        {
            this.actionHolderList.Remove(actionHolder);
        }

        public void Remove()
        {
            BehaviorComponent.RemoveActionNode(this);
        }
    }

    [System.Serializable]
    public abstract class Action
    {
        public abstract Task Execute();
        public abstract void Cancel();

        public abstract string GetActionType();
        public abstract string GetActionInfo();
    }

    [System.Serializable]
    public class ActionMove : Action
    {
        [Display(Name = "대상")]
        [SerializeField]
        public GameObject Subject;
        [Display(Name = "방향")]
        [SerializeField] public Vector3 offset;
        [Display(Name = "기간")]
        [SerializeField] public float duration = 1;
        [Display(Name = "정규화")]
        [SerializeField] public bool snapping = false;
        [Display(Name = "애니메이션")]
        [SerializeField] public Ease ease = Ease.Linear;
        [Display(Name = "횟수")]
        [SerializeField] public int playCount = 1;
        [Display(Name = "지연")]
        [SerializeField] public float delay = 0;
        [Display(Name = "반복 방식")]
        [SerializeField] public LoopType loopType = LoopType.Restart;
        [Display(Name = "무한 반복")]
        [SerializeField] public bool infinite = false;

        public override async Task Execute()
        {
            if (infinite) playCount = -1;
            var tween = Subject.transform.DOMove(offset, duration).SetRelative().SetEase(ease).SetLoops(playCount, loopType).SetDelay(delay);
            await tween.AsyncWaitForCompletion();
        }

        public override void Cancel()
        {
            Subject?.transform.DOKill();
        }

        public override string GetActionType()
        {
            return "이동";
        }
        
        public override string GetActionInfo()
        {
            return $"subject: {Subject}, offset: {offset}, duration: {duration}, ease: {ease}, playCount: {playCount}, delay: {delay}, loopType: {loopType}, infinite: {infinite}, HashCode: {GetHashCode()}";
        }

        public static string GetActionPath()
        {
            return "위치/이동";
        }

        public static bool Validate(GameObject gameObject)
        {
            return gameObject.GetComponent<Transform>() != null;
        }
    }

    [System.Serializable]
    public class ActionLocalMove : Action
    {
        [Display(Name = "대상")] [SerializeField] public GameObject Subject;
        [Display(Name = "방향")] [SerializeField] public Vector3 offset;
        [Display(Name = "기간")] [SerializeField] public float duration = 1;
        [Display(Name = "정규화")] [SerializeField] public bool snapping = false;
        [Display(Name = "애니메이션")] [SerializeField] public Ease ease = Ease.Linear;
        [Display(Name = "횟수")] [SerializeField] public int playCount = 1;
        [Display(Name = "지연")] [SerializeField] public float delay = 0;
        [Display(Name = "반복 방식")] [SerializeField] public LoopType loopType = LoopType.Restart;
        [Display(Name = "무한 반복")] [SerializeField] public bool infinite = false;

        public override async Task Execute()
        {
            if (infinite) playCount = -1;
            //var endValue = Subject.gameObject.transform.localPosition + offset;
            var tween = Subject.transform.DOLocalMove(offset, duration).SetRelative().SetEase(ease).SetLoops(playCount, loopType).SetDelay(delay);
            await tween.AsyncWaitForCompletion();
        }

        public override void Cancel()
        {
            Subject?.transform.DOKill();
        }

        public override string GetActionType()
        {
            return "로컬 이동";
        }

        public override string GetActionInfo()
        {
            return $"subject: {Subject}, offset: {offset}, duration: {duration}, ease: {ease}, playCount: {playCount}, delay: {delay}, loopType: {loopType}, infinite: {infinite}, HashCode: {GetHashCode()}";
        }

        public static string GetActionPath()
        {
            return "위치/로컬 이동";
        }

        public static bool Validate(GameObject gameObject)
        {
            return gameObject.GetComponent<Transform>() != null;
        }
    }

    [System.Serializable]
    public class ActionMoveTo : Action
    {
        [Display(Name = "대상")] [SerializeField] public GameObject Subject;
        [Display(Name = "목표")][SerializeField] public GameObject Target;
        [Display(Name = "거리")][SerializeField] public float distance = 0;
        [Display(Name = "기간")] [SerializeField] public float duration = 1;
        [Display(Name = "애니메이션")] [SerializeField] public Ease ease = Ease.Linear;
        [Display(Name = "횟수")] [SerializeField] public int playCount = 1;
        [Display(Name = "지연")] [SerializeField] public float delay = 0;
        [Display(Name = "반복 방식")] [SerializeField] public LoopType loopType = LoopType.Restart;

        public override async Task Execute()
        {
            var endValue = Target.transform.position + (Target.transform.position - Subject.transform.position).normalized * distance;
            var tween = Subject.transform.DOMove(endValue, duration).SetEase(ease).SetLoops(playCount, loopType).SetDelay(delay);
            await tween.AsyncWaitForCompletion();
        }

        public override void Cancel()
        {
            Subject?.transform.DOKill();
        }

        public override string GetActionType()
        {
            return "목적지로 이동";
        }

        public override string GetActionInfo()
        {
            return $"subject: {Subject}, target: {Target}, distance: {distance}, duration: {duration}, ease: {ease}, playCount: {playCount}, delay: {delay}, loopType: {loopType}, HashCode: {GetHashCode()}";
        }

        public static string GetActionPath()
        {
            return "위치/목적지로 이동";
        }

        public static bool Validate(GameObject gameObject)
        {
            return gameObject.GetComponent<Transform>() != null;
        }
    }

    [System.Serializable]
    public class ActionRotate : Action
    {
        [Display(Name = "대상")] [SerializeField] public GameObject Subject;
        [Display(Name = "회전")] [SerializeField] public Vector3 offset;
        [Display(Name = "기간")] [SerializeField] public float duration = 1;
        [Display(Name = "회전 방식")] [SerializeField] public RotateMode mode = RotateMode.LocalAxisAdd;
        [Display(Name = "애니메이션")] [SerializeField] public Ease ease = Ease.Linear;
        [Display(Name = "횟수")] [SerializeField] public int playCount = 1;
        [Display(Name = "지연")] [SerializeField] public float delay = 0;
        [Display(Name = "반복 방식")] [SerializeField] public LoopType loopType = LoopType.Restart;
        [Display(Name = "무한 반복")] [SerializeField] public bool infinite = false;
        
        public override async Task Execute()
        {
            if (infinite) playCount = -1;
            var myTween = Subject.transform.DORotate(offset, duration, mode).SetRelative().SetEase(ease).SetLoops(playCount, loopType).SetDelay(delay);
            await myTween.AsyncWaitForCompletion();
        }

        public override void Cancel()
        {
            Subject?.transform.DOKill();
        }

        public override string GetActionType()
        {
            return "회전";
        }

        public override string GetActionInfo()
        {
            return $"subject: {Subject}, offset: {offset}, duration: {duration}, ease: {ease}, playCount: {playCount}, delay: {delay}, loopType: {loopType}, infinite: {infinite}, HashCode: {GetHashCode()}";
        }

        public static string GetActionPath()
        {
            return "위치/회전";
        }

        public static bool Validate(GameObject gameObject)
        {
            return gameObject.GetComponent<Transform>() != null;
        }
    }

    [System.Serializable]
    public class ActionLocalRotate : Action
    {
        [Display(Name = "대상")] [SerializeField] public GameObject Subject;
        [Display(Name = "회전")] [SerializeField] public Vector3 offset;
        [Display(Name = "기간")] [SerializeField] public float duration = 1;
        [Display(Name = "회전 방식")] [SerializeField] public RotateMode mode = RotateMode.LocalAxisAdd;
        [Display(Name = "애니메이션")] [SerializeField] public Ease ease = Ease.Linear;
        [Display(Name = "횟수")] [SerializeField] public int playCount = 1;
        [Display(Name = "지연")] [SerializeField] public float delay = 0;
        [Display(Name = "반복 방식")] [SerializeField] public LoopType loopType = LoopType.Restart;
        [Display(Name = "무한 반복")] [SerializeField] public bool infinite = false;

        public override async Task Execute()
        {
            if (infinite) playCount = -1;
            var myTween = Subject.transform.DOLocalRotate(offset, duration, mode).SetRelative().SetEase(ease).SetLoops(playCount, loopType).SetDelay(delay);
            await myTween.AsyncWaitForCompletion();
        }

        public override void Cancel()
        {
            Subject?.transform.DOKill();
        }

        public override string GetActionType()
        {
            return "로컬 회전";
        }

        public override string GetActionInfo()
        {
            return $"subject: {Subject}, offset: {offset}, duration: {duration}, ease: {ease}, playCount: {playCount}, delay: {delay}, loopType: {loopType}, infinite: {infinite}, HashCode: {GetHashCode()}";
        }

        public static string GetActionPath()
        {
            return "위치/로컬 회전";
        }

        public static bool Validate(GameObject gameObject)
        {
            return gameObject.GetComponent<Transform>() != null;
        }
    }

    [System.Serializable]
    public class ActionScale : Action
    {
        [Display(Name = "대상")] [SerializeField] public GameObject Subject;
        [Display(Name = "배수")] [SerializeField] public Vector3 multiplier = default;
        [Display(Name = "기간")] [SerializeField] public float duration = 1;
        [Display(Name = "애니메이션")] [SerializeField] public Ease ease = Ease.Linear;
        [Display(Name = "횟수")] [SerializeField] public int playCount = 1;
        [Display(Name = "지연")] [SerializeField] public float delay = 0;
        [Display(Name = "반복 방식")] [SerializeField] public LoopType loopType = LoopType.Restart;
        [Display(Name = "무한 반복")] [SerializeField] public bool infinite = false;

        public override async Task Execute()
        {
            if (infinite) playCount = -1;
            Vector3 endValue = Vector3.Scale(Subject.transform.lossyScale, multiplier);
            var myTween = Subject.transform.DOScale(endValue, duration).SetRelative().SetEase(ease).SetLoops(playCount, loopType).SetDelay(delay);
            await myTween.AsyncWaitForCompletion();
        }

        public override void Cancel()
        {
            Subject?.transform.DOKill();
        }

        public override string GetActionType()
        {
            return "크기";
        }

        public override string GetActionInfo()
        {
            return $"subject: {Subject}, multiplier: {multiplier}, duration: {duration}, ease: {ease}, playCount: {playCount}, delay: {delay}, loopType: {loopType}, infinite: {infinite}, HashCode: {GetHashCode()}";
        }

        public static string GetActionPath()
        {
            return "위치/크기";
        }

        public static bool Validate(GameObject gameObject)
        {
            return gameObject.GetComponent<Transform>() != null;
        }
    }
    
    [System.Serializable]
    public class ActionJump : Action
    {
        [Display(Name = "대상")] [SerializeField] public GameObject Subject;
        [Display(Name = "종료 지점")] [SerializeField] public Vector3 endValue;
        [Display(Name = "점프 파워")] [SerializeField] public float jumpPower = 1;
        [Display(Name = "점프 횟수")] [SerializeField] public int numJumps = 1;
        [Display(Name = "기간")] [SerializeField] public float duration = 1;
        [Display(Name = "정규화")] [SerializeField] public bool snapping = false;
        [Display(Name = "애니메이션")] [SerializeField] public Ease ease = Ease.Linear;
        [Display(Name = "횟수")] [SerializeField] public int playCount = 1;
        [Display(Name = "지연")] [SerializeField] public float delay = 0;
        [Display(Name = "반복 방식")] [SerializeField] public LoopType loopType = LoopType.Restart;
        [Display(Name = "무한 반복")] [SerializeField] public bool infinite = false;

        public override async Task Execute()
        {
            if (infinite) playCount = -1;
            Debug.Log("ActionScale.Execute");
            var myTween = Subject.transform.DOJump(endValue, jumpPower, numJumps, duration, snapping).SetRelative().SetEase(ease).SetLoops(playCount, loopType).SetDelay(delay);
            await myTween.AsyncWaitForCompletion();
        }

        public override void Cancel()
        {
            Subject?.transform.DOKill();
        }

        public override string GetActionType()
        {
            return "점프";
        }

        public override string GetActionInfo()
        {
            return $"subject: {Subject}, endValue: {endValue}, jumpPower: {jumpPower}, numJumps: {numJumps}, duration: {duration}, snapping: {snapping}, ease: {ease}, playCount: {playCount}, delay: {delay}, loopType: {loopType}, infinite: {infinite}, HashCode: {GetHashCode()}";
        }

        public static string GetActionPath()
        {
            return "위치/점프";
        }

        public static bool Validate(GameObject gameObject)
        {
            return gameObject.GetComponent<Transform>() != null;
        }
    }


    [System.Serializable]
    public class ActionLocalJump : Action
    {
        [Display(Name = "대상")] [SerializeField] public GameObject Subject;
        [Display(Name = "종료 지점")] [SerializeField] public Vector3 endValue;
        [Display(Name = "점프 파워")] [SerializeField] public float jumpPower = 1;
        [Display(Name = "점프 횟수")] [SerializeField] public int numJumps = 1;
        [Display(Name = "기간")] [SerializeField] public float duration = 1;
        [Display(Name = "정규화")] [SerializeField] public bool snapping = false;
        [Display(Name = "애니메이션")] [SerializeField] public Ease ease = Ease.Linear;
        [Display(Name = "횟수")] [SerializeField] public int playCount = 1;
        [Display(Name = "지연")] [SerializeField] public float delay = 0;
        [Display(Name = "반복 방식")] [SerializeField] public LoopType loopType = LoopType.Restart;
        [Display(Name = "무한 반복")] [SerializeField] public bool infinite = false;

        public override async Task Execute()
        {
            if (infinite) playCount = -1;
            var myTween = Subject.transform.DOLocalJump(endValue, jumpPower, numJumps, duration, snapping).SetRelative().SetEase(ease).SetLoops(playCount, loopType).SetDelay(delay);
            await myTween.AsyncWaitForCompletion();
        }

        public override void Cancel()
        {
            Subject?.transform.DOKill();
        }

        public override string GetActionType()
        {
            return "로컬 점프";
        }

        public override string GetActionInfo()
        {
            return $"subject: {Subject}, endValue: {endValue}, jumpPower: {jumpPower}, numJumps: {numJumps}, duration: {duration}, snapping: {snapping}, ease: {ease}, playCount: {playCount}, delay: {delay}, loopType: {loopType}, infinite: {infinite}, HashCode: {GetHashCode()}";
        }

        public static string GetActionPath()
        {
            return "위치/로컬 점프";
        }

        public static bool Validate(GameObject gameObject)
        {
            return gameObject.GetComponent<Transform>() != null;
        }
    }

    [System.Serializable]
    public class ActionPunchScale : Action
    {
        [Display(Name = "대상")] [SerializeField] public GameObject Subject;
        [Display(Name = "방향")] [SerializeField] public Vector3 punch = default;
        [Display(Name = "기간")] [SerializeField] public float duration = 1;
        [Display(Name = "진동")] [SerializeField] public int vibrato = 1;
        [Display(Name = "탄력")] [SerializeField] public float elasticity = 1;
        [Display(Name = "애니메이션")] [SerializeField] public Ease ease = Ease.Linear;
        [Display(Name = "횟수")] [SerializeField] public int playCount = 1;
        [Display(Name = "지연")] [SerializeField] public float delay = 0;
        [Display(Name = "반복 방식")] [SerializeField] public LoopType loopType = LoopType.Restart;
        [Display(Name = "무한 반복")] [SerializeField] public bool infinite = false;

        public override async Task Execute()
        {
            if (infinite) playCount = -1;
            var myTween = Subject.transform.DOPunchScale(punch, duration, vibrato, elasticity).SetRelative().SetEase(ease).SetLoops(playCount, loopType).SetDelay(delay);
            await myTween.AsyncWaitForCompletion();
        }

        public override void Cancel()
        {
            Subject?.transform.DOKill();
        }

        public override string GetActionType()
        {
            return "펀치 크기";
        }

        public override string GetActionInfo()
        {
            return $"subject: {Subject}, punch: {punch}, duration: {duration}, vibrato: {vibrato}, elasticity: {elasticity}, ease: {ease}, playCount: {playCount}, delay: {delay}, loopType: {loopType}, infinite: {infinite}, HashCode: {GetHashCode()}";
        }

        public static string GetActionPath()
        {
            return "위치/펀치 크기";
        }

        public static bool Validate(GameObject gameObject)
        {
            return gameObject.GetComponent<Transform>() != null;
        }
    }

    [System.Serializable]
    public class ActionWait : Action
    {
        [Display(Name = "기간")] [SerializeField] public float duration = 1;

        public override async Task Execute()
        {
            await Task.Delay(TimeSpan.FromSeconds(duration));
        }

        public override void Cancel()
        {
            //
        }

        public override string GetActionType()
        {
            return "대기";
        }

        public override string GetActionInfo()
        {
            return $"duration: {duration}, HashCode: {GetHashCode()}";
        }

        public static string GetActionPath()
        {
            return "기타/대기";
        }

        public static bool Validate(GameObject gameObject)
        {
            return gameObject != null;
        }
    }

    [System.Serializable]
    public class ActionRandomWait : Action
    {
        [Display(Name = "최소 기간")] [SerializeField] public float minDuration = 0;
        [Display(Name = "최대 기간")] [SerializeField] public float maxDuration = 1;

        public override async Task Execute()
        {
            await Task.Delay(TimeSpan.FromSeconds(UnityEngine.Random.Range(minDuration, maxDuration)));
        }

        public override void Cancel()
        {
            //
        }

        public override string GetActionType()
        {
            return "랜덤 대기";
        }

        public override string GetActionInfo()
        {
            return $"minDuration: {minDuration}, maxDuration: {maxDuration}, HashCode: {GetHashCode()}";
        }

        public static string GetActionPath()
        {
            return "기타/랜덤 대기";
        }
        
        public static bool Validate(GameObject gameObject)
        {
            return gameObject != null;
        }
    }
    
    [System.Serializable]
    public class ActionActivate : Action
    {
        [Display(Name = "대상")] [SerializeField] public GameObject Subject;
        [Display(Name = "활성화")] [SerializeField] public bool activate = true;
        [Display(Name = "무조건 반대로")] [SerializeField] public bool reverse = false;
        
        public override Task Execute()
        {
            if (reverse)
            {
                Subject.SetActive(!Subject.activeSelf);
            }
            else
            {
                Subject.SetActive(activate);
            }
            return Task.CompletedTask;
        }

        public override void Cancel()
        {
            //
        }

        public override string GetActionType()
        {
            return "활성화";
        }

        public override string GetActionInfo()
        {
            return $"subject: {Subject}, HashCode: {GetHashCode()}";
        }

        public static string GetActionPath()
        {
            return "기타/활성화";
        }

        public static bool Validate(GameObject gameObject)
        {
            return gameObject != null;
        }
    }
    
    [System.Serializable]
    public class ActionChangeRigidbody : Action
    {
        [Display(Name = "대상")] [SerializeField] public GameObject Subject;
        [Display(Name = "질량")] [SerializeField] public float mass = 1;
        [Display(Name = "저항력")] [SerializeField] public float drag = 0;
        [Display(Name = "회전 저항력")] [SerializeField] public float angularDrag = 0.05f;
        [Display(Name = "중력 적용")] [SerializeField] public bool useGravity = true;
        [Display(Name = "물리 비적용")] [SerializeField] public bool isKinematic = false;
        [Display(Name = "보간법")] [SerializeField] public RigidbodyInterpolation interpolation = RigidbodyInterpolation.None;
        [Display(Name = "충돌 감지 방식")] [SerializeField] public CollisionDetectionMode collisionDetectionMode = CollisionDetectionMode.Discrete;
        [Display(Name = "X축 이동 고정")] [SerializeField] public bool constrainPositionX = false;
        [Display(Name = "Y축 이동 고정")] [SerializeField] public bool constrainPositionY = false;
        [Display(Name = "Z축 이동 고정")] [SerializeField] public bool constrainPositionZ = false;
        [Display(Name = "X축 회전 고정")] [SerializeField] public bool constrainRotationX = false;
        [Display(Name = "Y축 회전 고정")] [SerializeField] public bool constrainRotationY = false;
        [Display(Name = "Z축 회전 고정")] [SerializeField] public bool constrainRotationZ = false;

        public override async Task Execute()
        {
            var rigidbody = Subject.GetComponent<Rigidbody>();
            if (rigidbody == null)
            {
                return;
            }
            rigidbody.mass = mass;
            rigidbody.drag = drag;
            rigidbody.angularDrag = angularDrag;
            rigidbody.useGravity = useGravity;
            rigidbody.isKinematic = isKinematic;
            rigidbody.interpolation = interpolation;
            rigidbody.collisionDetectionMode = collisionDetectionMode;
            rigidbody.constraints = RigidbodyConstraints.None;
            if (constrainPositionX)
            {
                rigidbody.constraints |= RigidbodyConstraints.FreezePositionX;
            }
            if (constrainPositionY)
            {
                rigidbody.constraints |= RigidbodyConstraints.FreezePositionY;
            }
            if (constrainPositionZ)
            {
                rigidbody.constraints |= RigidbodyConstraints.FreezePositionZ;
            }
            if (constrainRotationX)
            {
                rigidbody.constraints |= RigidbodyConstraints.FreezeRotationX;
            }
            if (constrainRotationY)
            {
                rigidbody.constraints |= RigidbodyConstraints.FreezeRotationY;
            }
            if (constrainRotationZ)
            {
                rigidbody.constraints |= RigidbodyConstraints.FreezeRotationZ;
            }
        }

        public override void Cancel()
        {
            
        }

        public override string GetActionType()
        {
            return "물리 설정";
        }

        public override string GetActionInfo()
        {
            return $"subject: {Subject}, mass: {mass}, drag: {drag}, angularDrag: {angularDrag}, useGravity: {useGravity}, isKinematic: {isKinematic}, HashCode: {GetHashCode()}";
        }

        public static string GetActionPath()
        {
            return "물리/물리 설정";
        }

        public static bool Validate(GameObject gameObject)
        {
            return gameObject.GetComponent<Rigidbody>() != null;
        }
    }

    [System.Serializable]
    public class ActionAddForce : Action
    {
        [Display(Name = "대상")] [SerializeField] public GameObject Subject;
        [Display(Name = "방향")] [SerializeField] public Vector3 direction;
        [Display(Name = "힘 크기")] [SerializeField] public float power = 1;
        [Display(Name = "힘 모드")] [SerializeField] public ForceMode forceMode = ForceMode.Impulse;

        public override async Task Execute()
        {
            var rigidbody = Subject.GetComponent<Rigidbody>();
            if (rigidbody == null)
            {
                return;
            }
            rigidbody.AddForce(direction * power, forceMode);
        }

        public override void Cancel()
        {
            
        }

        public override string GetActionType()
        {
            return "힘을 가하다";
        }

        public override string GetActionInfo()
        {
            return $"subject: {Subject}, direction: {direction}, power: {power}, HashCode: {GetHashCode()}";
        }

        public static string GetActionPath()
        {
            return "물리/힘을 가하다";
        }

        public static bool Validate(GameObject gameObject)
        {
            return gameObject.GetComponent<Rigidbody>() != null;
        }
    }

    [System.Serializable]
    public class ActionChangeMaterialColor : Action
    {
        [Display(Name = "대상")] [SerializeField] public GameObject Subject;
        [Display(Name = "목표 색상")] [SerializeField] public Color color = Color.white;
        [Display(Name = "프로퍼티 이름")] [SerializeField] public string property = "_Color";
        [Display(Name = "기간")] [SerializeField] public float duration = 1;
        [Display(Name = "애니메이션")] [SerializeField] public Ease ease = Ease.Linear;
        [Display(Name = "횟수")] [SerializeField] public int playCount = 1;
        [Display(Name = "지연")] [SerializeField] public float delay = 0;
        [Display(Name = "반복 방식")] [SerializeField] public LoopType loopType = LoopType.Restart;
        [Display(Name = "무한 반복")] [SerializeField] public bool infinite = false;

        public override async Task Execute()
        {
            /*Debug.Log("Active.Execute");
            //var endValue = Subject.gameObject.transform.localPosition + offset;
            var myTween = Subject.transform.DORotate(offset, duration).SetRelative().SetEase(ease).SetLoops(playCount, loopType).SetDelay(delay);
            await myTween.AsyncWaitForCompletion();
            // Tween the specular value of a material
            //myMaterial.DOColor(Color.green, "_SpecColor", 1);*/
        }

        public override void Cancel()
        {
            //
        }

        public override string GetActionType()
        {
            return "옵션 변경 (색상)";
        }

        public override string GetActionInfo()
        {
            return $"subject: {Subject}, color: {color}, property: {property}, duration: {duration}, ease: {ease}, playCount: {playCount}, delay: {delay}, loopType: {loopType}, infinite: {infinite}, HashCode: {GetHashCode()}";
        }

        public static string GetActionPath()
        {
            return "재질/옵션 변경 (색상)";
        }

        public static bool Validate(GameObject gameObject)
        {
            MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                return meshRenderer.material != null;
            }
            return false;
        }
    }

    [System.Serializable]
    public class ActionChangeMaterialFloat : Action
    {
        [Display(Name = "대상")] [SerializeField] public GameObject Subject;
        [Display(Name = "목표 값")] [SerializeField] public float to = 1;
        [Display(Name = "프로퍼티 이름")] [SerializeField] public string property = "_Color";
        [Display(Name = "기간")] [SerializeField] public float duration = 1;
        [Display(Name = "애니메이션")] [SerializeField] public Ease ease = Ease.Linear;
        [Display(Name = "횟수")] [SerializeField] public int playCount = 1;
        [Display(Name = "지연")] [SerializeField] public float delay = 0;
        [Display(Name = "반복 방식")] [SerializeField] public LoopType loopType = LoopType.Restart;
        [Display(Name = "무한 반복")] [SerializeField] public bool infinite = false;

        public override async Task Execute()
        {
            /*Debug.Log("Active.Execute");
            //var endValue = Subject.gameObject.transform.localPosition + offset;
            var myTween = Subject.transform.DORotate(offset, duration).SetRelative().SetEase(ease).SetLoops(playCount, loopType).SetDelay(delay);
            await myTween.AsyncWaitForCompletion();*/
            // Tween the specular value of a material
            //myMaterial.DOColor(Color.green, "_SpecColor", 1);
        }

        public override void Cancel()
        {
            //
        }

        public override string GetActionType()
        {
            return "옵션 변경 (소수)";
        }

        public override string GetActionInfo()
        {
            return $"subject: {Subject}, to: {to}, property: {property}, duration: {duration}, ease: {ease}, playCount: {playCount}, delay: {delay}, loopType: {loopType}, infinite: {infinite}, HashCode: {GetHashCode()}";
        }

        public static string GetActionPath()
        {
            return "재질/옵션 변경 (소수)";
        }

        public static bool Validate(GameObject gameObject)
        {
            MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                return meshRenderer.material != null;
            }
            return false;
        }
    }
}