using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    /// <summary>
    /// Sequence node is like an AND
    /// </summary>
    [System.Serializable]
    public class BehaviourSequenceNode : BaseBehaviourNodeContainer
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
            Debug.Log($"Sequence {nodeName} On Exit()");
            //throw new NotImplementedException();
        }

        public override void OnReset()
        {
            this.state = NodeState.NotExecuted;
        }
        
        public override NodeState Process()
        {
            NodeState currentExecutedNodeStateResult = this.state;
            foreach (var n in this.nodes)
            {
                currentExecutedNodeStateResult = n.Process();
                
                if (currentExecutedNodeStateResult.Equals(NodeState.Failure))
                {
                    this.state = currentExecutedNodeStateResult;
                    Debug.LogWarning($"Sequence {nodeName} : Failure");
                    
                    if(this.state != NodeState.Running)
                        OnExit();
                    
                    return this.state;
                }
            }

            this.state = currentExecutedNodeStateResult;
            Debug.LogWarning($"Sequence {nodeName} : {this.state.ToString()}");
            
            if(this.state != NodeState.Running)
                OnExit();
            
            return this.state;
        }

        public BehaviourSequenceNode(string nName = "node", BaseBehaviourNode child = null) : base(nName, child)
        {
            
        }
    }
}
