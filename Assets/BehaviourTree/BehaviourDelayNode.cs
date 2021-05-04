using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace BehaviourTree
{
    [System.Serializable]
    public class BehaviourDelayNode : BaseBehaviourNode
    {
        float delay;
        public BehaviourDelayNode(float delay) : base(null, "Delay Node", null)
        {
            this.delay = delay;
        }

        public BehaviourDelayNode(ActionNodeState callback, float delay, string nName = "node", BaseBehaviourNode child = null) : base(callback, nName, child)
        {
            this.delay = delay;
        }


        public override void OnReset()
        {
            this.state = NodeState.NotExecuted;
        }

        public override void OnStart()
        {
            
        }

        public override async Task<NodeState> Process()
        {
            this.actionCallback();
            await Task.Delay(TimeSpan.FromSeconds(delay));
            return NodeState.Success;
        }

        protected override void OnEnter()
        {
            this.state = NodeState.Running;
        }

        protected override void OnExit()
        {
            Debug.Log($"{nodeName} On Exit()");
        }
    }
}
