using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace BehaviourTree
{

    public class BehaviorTimeOutNode : BaseBehaviourNode
    {
        private float timeOutDelay = 5.0f;
        
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

            float currentDelay = 0.0f;
            
            do
            {
                Debug.Log($"Proccessing {nodeName}");
                this.state = actionCallback();
                await Task.Delay(TimeSpan.FromSeconds(Time.deltaTime));

                currentDelay += Time.deltaTime;
                Debug.Log($"Delay for the node {nodeName} is {currentDelay}");

            } while (this.state.Equals(NodeState.Running) && currentDelay < this.timeOutDelay);

            if (this.state.Equals(NodeState.Running))
            {
                Debug.Log($"{nodeName} is time out !");
                this.state = NodeState.Failure;
            }
            
            OnExit();
            
            return this.state;
        }

        public BehaviorTimeOutNode(ActionNodeState callback, float timeOutDelay, string nName = "node", BaseBehaviourNode child = null) : base(callback, "Time Out Node - " + nName, child)
        {
            this.timeOutDelay = timeOutDelay;
        }
    }
}
