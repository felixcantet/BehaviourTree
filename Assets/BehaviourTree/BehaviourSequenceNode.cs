using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        protected override void OnEnter()
        {
            this.state = NodeState.Running;
        }

        protected override void OnExit()
        {
            Debug.Log($"Sequence {nodeName} On Exit()");
            //throw new NotImplementedException();
        }

        public override void OnReset()
        {
            this.state = NodeState.NotExecuted;
        }
        
        public override async Task<NodeState> Process()
        {
            NodeState currentExecutedNodeStateResult = this.state;
            
            OnEnter();
            
            foreach (var n in nodes)
            {
                do
                {
                    var tsk = n.Process();
                    await Task.WhenAll(tsk);
                    currentExecutedNodeStateResult = tsk.Result;
                    
                } while (currentExecutedNodeStateResult.Equals(NodeState.Running));
                
                if (currentExecutedNodeStateResult.Equals(NodeState.Failure))
                {
                    this.state = currentExecutedNodeStateResult;
                    break;
                }
            }
            
            this.state = currentExecutedNodeStateResult;
            
            Debug.LogWarning($"Selector {nodeName} : {this.state.ToString()}");

            OnExit();
            
            return this.state;
        }

        public BehaviourSequenceNode(string nName = "node", BaseBehaviourNode child = null) : base(nName, child)
        {
            
        }
    }
}
