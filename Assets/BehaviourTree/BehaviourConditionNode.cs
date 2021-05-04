using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace BehaviourTree
{
    [System.Serializable]
    public class BehaviourConditionNode : BaseBehaviourNode
    {
        public override void OnStart()
        {
            //throw new NotImplementedException();
        }

        protected override void OnEnter()
        {
            this.state = NodeState.Running;
        }

        protected override void OnExit()
        {
            Debug.Log($"Action Node {nodeName} On Exit()");
            //throw new NotImplementedException();
        }

        public override void OnReset()
        {
            this.state = NodeState.NotExecuted;
        }

        public override async Task<NodeState> Process()
        {
            OnEnter();
            
            do
            {
                this.state = actionCallback();
                await Task.Delay(TimeSpan.FromSeconds(Time.deltaTime));
                
            } while (this.state.Equals(NodeState.Running));

            //if(this.state != NodeState.Running)
                
            OnExit();
            
            return this.state;
        }
        
        public BehaviourConditionNode(ActionNodeState callback, string nName = "node", BaseBehaviourNode child = null) : base(callback, nName, child)
        {
        }
    }
}
