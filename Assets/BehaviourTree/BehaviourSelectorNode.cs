using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    [System.Serializable]
    public class BehaviourSelectorNode : BaseBehaviourNodeContainer
    {
        public override NodeState Process()
        {
            throw new System.NotImplementedException();
        }

        public BehaviourSelectorNode(Action<bool> callback) : base(callback)
        {
        }
    }
}
