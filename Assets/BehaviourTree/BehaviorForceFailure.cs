
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace BehaviourTree
{
    public class BehaviorForceFailure : BaseBehaviourNode
    {
        public BehaviorForceFailure(ActionNodeState callback = null, string nName = "", BaseBehaviourNode child = null) : base(callback, "Force Failure - " + nName, child)
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
                this.state = actionCallback();
                await Task.Delay(TimeSpan.FromSeconds(Time.deltaTime));
                
            } while (this.state.Equals(NodeState.Running));

            this.state = NodeState.Failure;
            OnExit();
            
            return this.state;
        }
    }
}