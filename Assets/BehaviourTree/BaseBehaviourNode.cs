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
    
    public delegate NodeState ActionNodeState();

    /// <summary>
    /// Base class for every custom node we want to create
    /// </summary>
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
        
        /// <summary>
        /// Create a node
        /// </summary>
        /// <param name="callback">Callback have to return Running, Success or Failure</param>
        /// <param name="nName"></param>
        /// <param name="child"></param>
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

        /// <summary>
        /// Check if this node has a child node
        /// </summary>
        /// <returns></returns>
        public bool HasChild()
        {
            return childNode != null;
        }

        /// <summary>
        /// Get the child node if exist
        /// </summary>
        /// <returns></returns>
        public BaseBehaviourNode GetChild()
        {
            if (HasChild())
                return this.childNode;

            return null;
        }
        
        /*
         * These next functions are base function to the right run of the graph
         */
        
        public abstract void OnStart();
        
        protected abstract void OnEnter();
        
        protected abstract void OnExit();

        public abstract void OnReset();

        public abstract Task<NodeState> Process();
    }
}
