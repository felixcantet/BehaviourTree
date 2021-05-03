using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BehaviourTree
{
    public class BehaviorForceSuccess : BaseBehaviourNode
    {
        public BehaviorForceSuccess(ActionBool callback = null, string nName = "", BaseBehaviourNode child = null) : base(callback, "Force Success - " + nName, child)
        {
            
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
            actionCallback();
            
            OnExit();
            return NodeState.Success;
        }
    }
}