using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

namespace BehaviourTree
{
    /// <summary>
    /// This node retry R times, when callback end running and return failure
    /// </summary>
    public class BehaviorRetryNode : BaseBehaviourNode
    {
        [SerializeField] private int repeatNumber = 1;
        
        public BehaviorRetryNode(ActionNodeState callback, int repeatNumber, string nName = "node", BaseBehaviourNode child = null) : base(callback, nName, child)
        {
            Assert.AreNotEqual(repeatNumber, 0, "You've set repeat number to 0. This is not possible !");
            this.repeatNumber = repeatNumber;
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
            NodeState lastState = this.state;

            OnEnter();
            
            Debug.Log($"Proccessing {nodeName}");

            for (int i = 0; i < repeatNumber; i++)
            {
                do
                {
                    lastState = actionCallback();
                    await Task.Delay(TimeSpan.FromSeconds(Time.deltaTime));
                
                } while (lastState.Equals(NodeState.Running));
                
                if (lastState.Equals(NodeState.Success))
                    break;
            }

            this.state = lastState;
            OnExit();
            
            return this.state;
        }
    }
}