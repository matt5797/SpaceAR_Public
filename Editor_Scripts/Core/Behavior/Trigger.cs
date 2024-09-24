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
        [SerializeField] public override string Name { get => "�����Ҷ�"; }
        [SerializeField] public static string GetTriggerPath() { return "����/����"; }
        public static bool Validate(GameObject gameObject)
        {
            return true;
        }
    }
    [System.Serializable]
    public class TriggerTap : Trigger
    {
        [SerializeField] public override string Name { get => "��"; }
        [SerializeField] public static string GetTriggerPath() { return "������/��"; }
        public static bool Validate(GameObject gameObject)
        {
            return true;
        }
    }
    [System.Serializable]
    public class TriggerLongPress : Trigger
    {
        public override string Name { get => "��� ��"; }
        public static string GetTriggerPath() { return "������/��� ��"; }
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
        public override string Name { get => "�����������"; }
        public static string GetTriggerPath() { return "����/�����������"; }
        public static bool Validate(GameObject gameObject)
        {
            return true;
        }
    }
    [System.Serializable]
    public class TriggerProximityExit : Trigger
    {
        public override string Name { get => "�־�������"; }
        public static string GetTriggerPath() { return "����/�־�������"; }
        public static bool Validate(GameObject gameObject)
        {
            return true;
        }
    }

    [System.Serializable]
    public class TriggerTriggerEnter : Trigger
    {
        public override string Name { get => "���� ����"; }
        public static string GetTriggerPath() { return "����/���� ����"; }
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
        public override string Name { get => "���� ����"; }
        public static string GetTriggerPath() { return "����/���� ����"; }
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
        public override string Name { get => "�浹 ����"; }
        public static string GetTriggerPath() { return "����/�浹 ����"; }
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
        public override string Name { get => "�浹 ����"; }
        public static string GetTriggerPath() { return "����/�浹 ����"; }
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
        public override string Name { get => "Ŀ���� Ʈ����"; }
        public static string GetTriggerPath() { return "Ŀ����/Ŀ���� Ʈ����"; }
        public static bool Validate(GameObject gameObject)
        {
            return true;
        }
    }
}