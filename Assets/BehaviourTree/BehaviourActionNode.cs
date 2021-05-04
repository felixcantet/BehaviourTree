using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace BehaviourTree
{
    /// <summary>
    /// Basic action node
    /// Call a callback when process
    /// </summary>
    public class BehaviourActionNode : BaseBehaviourNode
    {
        public override void OnStart()
        {
            
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
                Debug.Log($"Proccessing {nodeName}");
                this.state = actionCallback();
                await Task.Delay(TimeSpan.FromSeconds(Time.deltaTime));
                
            } while (this.state.Equals(NodeState.Running));

            //if(this.state != NodeState.Running)
                
            OnExit();
            
            return this.state;
        }

        public BehaviourActionNode(ActionNodeState callback, string nName = "node", BaseBehaviourNode child = null) : base(callback, nName, child)
        {
            
        }
    }
}
