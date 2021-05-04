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
            
        }

        public override void OnEnter()
        {
            this.state = NodeState.Running;
        }

        public override void OnExit()
        {
            Debug.Log($"Action Node {nodeName} On Exit()");
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

        public BehaviourActionNode(ActionNodeState callback, string nName = "node", BaseBehaviourNode child = null) : base(callback, nName, child)
        {
            
        }
    }
}
