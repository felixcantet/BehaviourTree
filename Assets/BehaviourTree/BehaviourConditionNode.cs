using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    [System.Serializable]
    public class BehaviourConditionNode : BaseBehaviourNode
    {
        public override NodeState Process()
        {
            throw new System.NotImplementedException();
        }

        public BehaviourConditionNode(Action<bool> callback) : base(callback)
        {
            
        }
    }
}
