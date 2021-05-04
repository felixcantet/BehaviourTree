
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace BehaviourTree
{
    public class BehaviorRootNode : BaseBehaviourNode
    {
        public BehaviorRootNode(BaseBehaviourNode child) : base(null, "Root", child)
        {
            //this.actionCallback += child.Process;
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
            // this.state = actionCallback();
            // if(this.state != NodeState.Running)
            //     OnExit();
            
            OnEnter();
            
            Debug.Log($"Proccessing {nodeName}");
            var tsk = childNode.Process();

            await Task.WhenAll(tsk);
            
            Assert.AreNotEqual(tsk.Result, NodeState.Running, $"The result in the root node is Running .. {tsk.Result}");
            Assert.AreNotEqual(tsk.Result, NodeState.Failure, $"The result in the root node is Failure .. {tsk.Result}");

            this.state = tsk.Result;
            
            OnExit();
            
            return this.state;
        }
    }
}