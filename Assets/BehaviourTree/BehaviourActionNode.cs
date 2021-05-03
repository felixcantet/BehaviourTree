using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{

    public class BehaviourActionNode : BaseBehaviourNode
    {
        public override NodeState Process()
        {
            throw new System.NotImplementedException();
        }

        public BehaviourActionNode(Action<bool> callback) : base(callback)
        {
        }
    }
}
