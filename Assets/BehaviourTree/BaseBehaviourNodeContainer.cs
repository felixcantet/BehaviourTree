using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;

namespace BehaviourTree
{

    [System.Serializable]
    public abstract class BaseBehaviourNodeContainer : BaseBehaviourNode
    {
        private List<BaseBehaviourNode> nodes = new List<BaseBehaviourNode>();

        public List<BaseBehaviourNode> Nodes
        {
            get { return this.nodes; }
            set { this.nodes = value; }
        }
    }
}
