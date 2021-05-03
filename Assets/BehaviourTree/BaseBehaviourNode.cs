using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BehaviourTree
{
    public enum NodeState
    {
        NotExecuted,
        Running,
        Failure,
        Success

    }

    [System.Serializable]
    public abstract class BaseBehaviourNode
    {
        #region Variables
        [SerializeField] protected NodeState state;
        [SerializeField] protected Action<bool> actionCallback;

        [SerializeField] protected BaseBehaviourNode previousNode;
        [SerializeField] protected BaseBehaviourNode nextNode;
        #endregion

        #region Properties
        public NodeState State
        {
            get { return state; }
            set { state = value; }
        }
        #endregion

        public BaseBehaviourNode(Action<bool> callback)
        {
            this.actionCallback += callback;
        }
        
        public abstract NodeState Process();
    }
}
