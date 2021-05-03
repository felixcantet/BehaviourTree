using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;
using UnityEngine.Assertions;

namespace BehaviourTree
{

    [System.Serializable]
    public abstract class BaseBehaviourNodeContainer : BaseBehaviourNode
    {
        [SerializeField] protected List<BaseBehaviourNode> nodes = new List<BaseBehaviourNode>();
        public List<BaseBehaviourNode> Nodes
        {
            get { return this.nodes; }
            set { this.nodes = value; }
        }


        public override void OnStart()
        {
            foreach (var n in nodes)
            {
                n.OnStart();
                n.tree = this.tree;
            }
        }

        public override void OnReset()
        {
            this.state = NodeState.NotExecuted;
            foreach (var n in nodes)
                n.OnReset();
        }

        public abstract override void OnEnter();

        public  abstract override void OnExit();

        public  abstract override NodeState Process();

        
        public BaseBehaviourNodeContainer(string nName = "node", BaseBehaviourNode child = null) : base(null, nName, child)
        {
            
        }

        public void Add(BaseBehaviourNode node, int siblingIndex = -1)
        {
            Assert.IsNotNull(node, "Add null node to a container is not available !!");
            
            if(siblingIndex + 1 == nodes.Count && nodes.Count > 0)
                nodes.Insert(siblingIndex, node);
            else
                nodes.Add(node);
        }
    }
}
