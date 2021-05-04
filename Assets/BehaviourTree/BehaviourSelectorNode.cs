using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        protected override void OnEnter()
        {
            this.state = NodeState.Running;
        }

        protected override void OnExit()
        {
            Debug.Log($"Selector {nodeName} On Exit()");
            //throw new NotImplementedException();
        }

        public override void OnReset()
        {
            base.OnReset();
        }
        
        public override async Task<NodeState> Process()
        {
            NodeState currentExecutedNodeStateResult = this.state;
            
            OnEnter();
            
            Debug.Log($"Proccessing {nodeName}");

            foreach (var n in nodes)
            {
                do
                {
                    var tsk = n.Process();
                    await Task.WhenAll(tsk);
                    currentExecutedNodeStateResult = tsk.Result;
                    
                } while (currentExecutedNodeStateResult.Equals(NodeState.Running));
                
                if (currentExecutedNodeStateResult.Equals(NodeState.Success))
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

        public BehaviourSelectorNode(string nName = "node", BaseBehaviourNode child = null) : base(nName, child)
        {
            
        }
    }
}
