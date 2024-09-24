using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Events;

namespace SpaceAR.Core.Behavior
{
    [System.Serializable]
    public class BehaviorComponent : MonoBehaviour, IGesture
    {
        public static List<BehaviorComponent> behaviorComponents = new List<BehaviorComponent>();
        public static UnityEvent onListChange = new UnityEvent();

        //[SerializeReference] public Trigger _trigger = new TriggerStart();
        [SerializeReference] public Trigger _trigger = new TriggerTriggerEnter();
        public Trigger Trigger
        {
            get => _trigger;
            set
            {
                _trigger = value;
                onTriggerChange.Invoke();
            }
        }
        public UnityEvent onTrigger = new UnityEvent();
        public UnityEvent onTriggerChange = new UnityEvent();

        [SerializeField] public List<ActionNode> actionNodeList = new List<ActionNode>();
        [HideInInspector] public bool isRunning = false;
        Coroutine action;

        bool proximity = false;
        GameObject proximityTarget;

#if SPACEAR_EDITOR
        static bool m_isRuntimeEditorStart = false;

        private void RuntimeStart()
        {
            m_isRuntimeEditorStart = true;
        }
        private void OnRuntimeDestroy()
        {
            m_isRuntimeEditorStart = false;
        }
#endif

        private void Awake()
        {
            behaviorComponents.Add(this);
        }

        // Start is called before the first frame update
        void Start()
        {
            proximityTarget = GameObject.Find("ARCamera");

            if (Trigger is TriggerStart)
            {
                StartAction();
            }
        }

        void Update()
        {
            /*if (Input.GetKeyDown(KeyCode.Space))
            {
                print($"Space {gameObject} Trigger: {Trigger} actionNodeList: {actionNodeList.Count}");
                int cnt = 0;
                foreach (var actionNode in actionNodeList)
                {
                    Debug.Log($"{cnt++} : {actionNode}");
                    foreach (var actionHolder in actionNode.actionHolderList)
                    {
                        Debug.Log(actionHolder.action.GetActionInfo());
                    }
                }
            }*/
            /*
            float proximityDistance = Vector3.Distance(transform.position, proximityTarget.transform.position);
            
            if (proximity && proximityDistance < 10f)
            {
                if (Trigger is TriggerProximityExit)
                {
                    StartAction();
                }
            }
            */
        }

        private void OnDestroy()
        {
            behaviorComponents.Remove(this);
        }

        #region �׼� ����/����
        IEnumerator StartActions()
        {
#if SPACEAR_EDITOR
            if (!m_isRuntimeEditorStart)
                yield break;
#endif
            Debug.Log($"{gameObject} StartActions");
            Debug.Log($"actionNodeList.Count: {actionNodeList.Count}");
            isRunning = true;
            Trigger.triggerCount++;
            foreach (ActionNode actionNode in actionNodeList)
            {
                print("actionNode : " + actionNode);
                List<Task> taskList = new List<Task>();
                foreach (ActionHolder actionHolder in actionNode.actionHolderList)
                {
                    if (actionHolder.action != null)
                        taskList.Add(actionHolder.action.Execute());
                }
                yield return new WaitUntil(() =>
                {
                    bool complete = true;
                    foreach (Task task in taskList)
                    {
                        if (!task.IsCompleted)
                            complete = false;
                    }
                    return complete;
                });
            }
            isRunning = false;
            yield break;
        }

        void StopActions()
        {
            foreach (ActionNode actionNode in actionNodeList)
            {
                foreach (ActionHolder actionHolder in actionNode.actionHolderList)
                {
                    if (actionHolder.action != null)
                        actionHolder.action.Cancel();
                }
            }
        }

        void StartAction()
        {
            Debug.Log($"{gameObject} StartAction");
            switch (Trigger.allowReTriggering)
            {
                case AllowReTriggering.Never:
                    if (Trigger.triggerCount == 0)
                        action = StartCoroutine(StartActions());
                    break;
                case AllowReTriggering.Immediately:
                    if (action != null)
                        StopCoroutine(action);
                    StopActions();
                    action = StartCoroutine(StartActions());
                    break;
                case AllowReTriggering.AfterActionsComplete:
                    if (!isRunning)
                    {
                        if (action != null)
                            StopCoroutine(action);
                        StopActions();
                        action = StartCoroutine(StartActions());
                    }
                    break;
            }
        }
        #endregion

        #region �׼� �߰�/����
        public ActionNode AddActionNode()
        {
            ActionNode newActionNode = new ActionNode(this);
            actionNodeList.Add(newActionNode);
            return newActionNode;
        }

        public ActionNode AddActionNode(Action action)
        {
            ActionNode newActionNode = new ActionNode(this);
            newActionNode.AddAction(action);
            actionNodeList.Add(newActionNode);
            return newActionNode;
        }

        public void RemoveActionNode(ActionNode actionNode)
        {
            actionNodeList.Remove(actionNode);
        }
        #endregion

        #region Triggers

        private void OnTriggerEnter(Collider other)
        {
            if (Trigger is TriggerTriggerEnter)
            {
                StartAction();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (Trigger is TriggerTriggerExit)
            {
                StartAction();
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (Trigger is TriggerCollisionEnter)
            {
                StartAction();
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (Trigger is TriggerCollisionExit)
            {
                StartAction();
            }
        }

        public void TriggerCustomCall()
        {
            if (Trigger is TriggerCustomCall)
            {
                StartAction();
            }
        }

        #endregion

        #region ����ó �������̽�
        void IGesture.OnTap()
        {
            if (Trigger is TriggerTap)
            {
                Debug.Log($"{gameObject} OnTap");
                StartAction();
            }
        }

        void IGesture.OnLongPressBegan(float screenX, float screenY)
        {

        }

        void IGesture.OnLongPressExecuting(float screenX, float screenY)
        {

        }

        void IGesture.OnLongPressEnded(float velocityXScreen, float velocityYScreen)
        {
            if (Trigger is TriggerLongPress)
            {
                Debug.Log($"{gameObject} OnLongPressEnded");
                StartAction();
            }
        }

        void IGesture.OnRotate(float angleDelta)
        {

        }
        #endregion
    }
}
