using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace BehaviourTree
{
    [System.Serializable]
    public class BehaviourTree
    {
        [SerializeField] private List<BaseBehaviourNode> nodes = new List<BaseBehaviourNode>();
                
        private BaseBehaviourNode entryPoint;
        private BaseBehaviourNode currentNode;

        public BehaviourTree(BaseBehaviourNode entryNode)
        {
            this.nodes.Add(entryNode);
            this.entryPoint = entryNode;
            this.currentNode = this.entryPoint;
        }
        
        public void InitGraph()
        {
            foreach (var n in nodes)
            {
                n.tree = this;
                n.OnStart();
            }

            currentNode.Process();
        }

        public void ResetGraph()
        {
            // Set all nodes to NotExecuted state
            foreach (var n in nodes)
                n.OnReset();

            this.currentNode = this.entryPoint;
        }

        /*
         * - Le behavior tree a besoins de contenir que le node start
         * - Cas de base Start => Selector
         * - Si le node start = success ==> le tree s'arrete
         * - Callback de start = nextNode.process()
         * - Après le start il n'y a pas forcément de Selector derriere !
         *     --> L'utilisateur doit pouvoir faire ce qu'il veut
         * - Node = node avec des process differents
         * - BT Toujours composé de Séquence et de sélector dans l'idée !
         * - Node "TimeOut" ==> au bout de N tick je stop l'action
         * - Node Repeat ==> Repeat N fois
         */
        
        public void Tick()
        {
            var result = currentNode.Process();
            Assert.AreEqual(result, NodeState.Failure, "Result is failure in Behavior Tree");
        }
    }
}
