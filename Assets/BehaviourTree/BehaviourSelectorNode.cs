using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    /// <summary>
    /// Sequence node is like an OR
    /// </summary>
    [System.Serializable]
    public class BehaviourSelectorNode : BaseBehaviourNodeContainer
    {
        public override void OnStart()
        {
            base.OnStart();
        }

        public override void OnEnter()
        {
            this.state = NodeState.Running;
        }

        public override void OnExit()
        {
            Debug.Log($"Selector {nodeName} On Exit()");
            //throw new NotImplementedException();
        }

        public override void OnReset()
        {
            base.OnReset();
        }
        
        public override NodeState Process()
        {
            NodeState currentExecutedNodeStateResult = this.state;
            foreach (var n in nodes)
            {
                currentExecutedNodeStateResult = n.Process();
                
                if (currentExecutedNodeStateResult.Equals(NodeState.Success))
                {
                    this.state = currentExecutedNodeStateResult;
                    Debug.Log($"Selector {nodeName} : Success");
                    
                    if(this.state != NodeState.Running)
                        OnExit();
                    
                    return this.state;
                }
            }
            
            this.state = currentExecutedNodeStateResult;
            
            Debug.LogWarning($"Selector {nodeName} : {this.state.ToString()}");

            if(this.state != NodeState.Running)
                OnExit();
            
            return this.state;
        }

        public BehaviourSelectorNode(string nName = "node", BaseBehaviourNode child = null) : base(nName, child)
        {
            
        }
    }
}
