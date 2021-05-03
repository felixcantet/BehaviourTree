using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    [System.Serializable]
    public class BehaviourSequenceNode : BaseBehaviourNodeContainer
    {
        public BehaviourSequenceNode(Action<bool> callback) : base(callback)
        {
        }
        
        public override NodeState Process()
        {
            throw new System.NotImplementedException();

            foreach (var n in this.nodes)
            {
                if (n.Process().Equals(NodeState.Failure))
                {
                    Debug.LogWarning("Sequence : Node Failure");
                    break;
                }
            }
        }

    }
}
