using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace BehaviourTree
{
    /// <summary>
    /// This node return, when the callback return an other value of Running, Success
    /// </summary>
    public class BehaviorForceSuccess : BaseBehaviourNode
    {
        public BehaviorForceSuccess(ActionNodeState callback = null, string nName = "", BaseBehaviourNode child = null) : base(callback, "Force Success - " + nName, child)
        {
            
        }

        public override void OnStart()
        {
            
        }

        protected override void OnEnter()
        {
            this.state = NodeState.Running;
        }

        protected override void OnExit()
        {
            Debug.Log($"{nodeName} On Exit()");
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

            this.state = NodeState.Success;
            OnExit();
            
            return this.state;
        }
    }
}