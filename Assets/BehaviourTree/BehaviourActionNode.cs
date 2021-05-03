using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{

    public class BehaviourActionNode : BaseBehaviourNode
    {
        public override void OnStart()
        {
            throw new NotImplementedException();
        }

        public override void OnEnter()
        {
            this.state = NodeState.Running;
        }

        public override void OnExit()
        {
            throw new NotImplementedException();
        }

        public override void OnReset()
        {
            this.state = NodeState.NotExecuted;
        }

        public override NodeState Process()
        {
            this.state = actionCallback() ? NodeState.Success : NodeState.Failure;
            
            return this.state;
        }

        public BehaviourActionNode(ActionBool callback, string nName = "node", BaseBehaviourNode child = null) : base(callback, nName, child)
        {
            
        }
    }
}
