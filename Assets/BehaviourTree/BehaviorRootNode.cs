
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BehaviourTree
{
    public class BehaviorRootNode : BaseBehaviourNode
    {
        public BehaviorRootNode(BaseBehaviourNode child) : base(null, "Root", child)
        {
            this.actionCallback += child.Process;
        }

        public override void OnStart()
        {
            
        }

        public override void OnEnter()
        {
            this.state = NodeState.Running;
        }

        public override void OnExit()
        {
            Debug.Log($"{nodeName} On Exit()");
            //throw new NotImplementedException();
        }

        public override void OnReset()
        {
            this.state = NodeState.NotExecuted;
        }

        public override NodeState Process()
        {
            this.state = actionCallback();
            if(this.state != NodeState.Running)
                OnExit();
            return this.state;
        }
    }
}