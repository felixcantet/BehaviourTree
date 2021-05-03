using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        [SerializeField] private NodeState state;
        #endregion

        #region Properties
        public NodeState State
        {
            get { return state; }
            set { state = value; }
        }
        #endregion

        public abstract NodeState Process();
        
    }
}
