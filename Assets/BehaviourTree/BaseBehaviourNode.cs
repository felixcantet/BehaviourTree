using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    
    public delegate NodeState ActionNodeState(); // delegate = pointeur sur fonction ==> Bloquant
    // Peut etre faire un event
    // ==> public event MonDelegate callback;
    // Peut etre hériter de IEnumerator ?
    // 
    
    [System.Serializable]
    public abstract class BaseBehaviourNode
    {
        #region Variables
        [SerializeField] public BehaviourTree tree;

        [SerializeField] protected string nodeName = "node";
        
        [SerializeField] protected NodeState state = NodeState.NotExecuted;
        [SerializeField] protected ActionNodeState actionCallback = null;

        [SerializeField] protected BaseBehaviourNode parentNode = null;
        [SerializeField] protected BaseBehaviourNode childNode = null;
        #endregion

        #region Properties
        public NodeState State
        {
            get { return state; }
            set { state = value; }
        }
        #endregion
        
        public BaseBehaviourNode(ActionNodeState callback, string nName = "node", BaseBehaviourNode child = null)
        {
            this.actionCallback += callback;

            this.nodeName = nName;
            
            if(child != null)
                SetChild(child);
        }
        
        
        protected void SetChild(BaseBehaviourNode child)
        {
            this.childNode = child;
            child.parentNode = this;
        }

        public bool HasChild()
        {
            return childNode != null;
        }

        public BaseBehaviourNode GetChild()
        {
            if (HasChild())
                return this.childNode;

            return null;
        }
        
        public abstract void OnStart();
        
        protected abstract void OnEnter();
        
        protected abstract void OnExit();

        public abstract void OnReset();

        public abstract Task<NodeState> Process();
    }
}
