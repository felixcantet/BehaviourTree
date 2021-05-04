using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace BehaviourTree
{
    /// <summary>
    /// Work like an action node.
    /// After processing (return value != Running) wait a delay before return it's value
    /// </summary>
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
            OnEnter();

            do
            {
                Debug.Log($"Proccessing {nodeName}");
                this.state = actionCallback();
                await Task.Delay(TimeSpan.FromSeconds(Time.deltaTime));

            } while (this.state.Equals(NodeState.Running));
            await Task.Delay(TimeSpan.FromSeconds(delay));
            //if(this.state != NodeState.Running)

            OnExit();

            return this.state;
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
