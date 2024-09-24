using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceAR.Core.Behavior
{
    public enum AllowReTriggering
    {
        Immediately,
        AfterActionsComplete,
        Never
    }

    [System.Serializable]
    public abstract class Trigger
    {
        [SerializeReference] public GameObject Subject;
        [SerializeField] public AllowReTriggering allowReTriggering = AllowReTriggering.AfterActionsComplete;
        [SerializeField] public int triggerCount = 0;

        public abstract string Name { get; }
    }

    [System.Serializable]
    public class TriggerStart : Trigger
    {
        [SerializeField] public override string Name { get => "시작할때"; }
        [SerializeField] public static string GetTriggerPath() { return "상태/시작"; }
        public static bool Validate(GameObject gameObject)
        {
            return true;
        }
    }
    [System.Serializable]
    public class TriggerTap : Trigger
    {
        [SerializeField] public override string Name { get => "탭"; }
        [SerializeField] public static string GetTriggerPath() { return "제스쳐/탭"; }
        public static bool Validate(GameObject gameObject)
        {
            return true;
        }
    }
    [System.Serializable]
    public class TriggerLongPress : Trigger
    {
        public override string Name { get => "길게 탭"; }
        public static string GetTriggerPath() { return "제스쳐/길게 탭"; }
        public static bool Validate(GameObject gameObject)
        {
            return true;
        }
    }

    [System.Serializable]
    public class TriggerProximityEnter : Trigger
    {
        public bool ignoreIfHidden = true;
        public float distance = 1;
        public override string Name { get => "가까워졌을때"; }
        public static string GetTriggerPath() { return "상태/가까워졌을때"; }
        public static bool Validate(GameObject gameObject)
        {
            return true;
        }
    }
    [System.Serializable]
    public class TriggerProximityExit : Trigger
    {
        public override string Name { get => "멀어졌을때"; }
        public static string GetTriggerPath() { return "상태/멀어졌을때"; }
        public static bool Validate(GameObject gameObject)
        {
            return true;
        }
    }

    [System.Serializable]
    public class TriggerTriggerEnter : Trigger
    {
        public override string Name { get => "진입 시작"; }
        public static string GetTriggerPath() { return "물리/진입 시작"; }
        public static bool Validate(GameObject gameObject)
        {
            Collider[] colliders = gameObject.GetComponentsInChildren<Collider>();
            if (colliders.Length == 0)
                return false;
            bool hasTrigger = false;
            foreach (Collider collider in colliders)
            {
                if (collider.isTrigger)
                {
                    hasTrigger = true;
                    break;
                }
            }
            return hasTrigger;
        }
    }
    [System.Serializable]
    public class TriggerTriggerExit : Trigger
    {
        public override string Name { get => "진입 종료"; }
        public static string GetTriggerPath() { return "물리/진입 종료"; }
        public static bool Validate(GameObject gameObject)
        {
            Collider[] colliders = gameObject.GetComponentsInChildren<Collider>();
            if (colliders.Length == 0)
                return false;
            bool hasTrigger = false;
            foreach (Collider collider in colliders)
            {
                if (collider.isTrigger)
                {
                    hasTrigger = true;
                    break;
                }
            }
            return hasTrigger;
        }
    }
    [System.Serializable]
    public class TriggerCollisionEnter : Trigger
    {
        public override string Name { get => "충돌 시작"; }
        public static string GetTriggerPath() { return "물리/충돌 시작"; }
        public static bool Validate(GameObject gameObject)
        {
            Collider[] colliders = gameObject.GetComponentsInChildren<Collider>();
            if (colliders.Length == 0)
                return false;
            bool hasCollision = false;
            foreach (Collider collider in colliders)
            {
                if (!collider.isTrigger)
                {
                    hasCollision = true;
                    break;
                }
            }
            return hasCollision;
        }
    }
    [System.Serializable]
    public class TriggerCollisionExit : Trigger
    {
        public override string Name { get => "충돌 종료"; }
        public static string GetTriggerPath() { return "물리/충돌 종료"; }
        public static bool Validate(GameObject gameObject)
        {
            Collider[] colliders = gameObject.GetComponentsInChildren<Collider>();
            if (colliders.Length == 0)
                return false;
            bool hasCollision = false;
            foreach (Collider collider in colliders)
            {
                if (!collider.isTrigger)
                {
                    hasCollision = true;
                    break;
                }
            }
            return hasCollision;
        }
    }

    [System.Serializable]
    public class TriggerCustomCall : Trigger
    {
        public override string Name { get => "커스텀 트리거"; }
        public static string GetTriggerPath() { return "커스텀/커스텀 트리거"; }
        public static bool Validate(GameObject gameObject)
        {
            return true;
        }
    }
}