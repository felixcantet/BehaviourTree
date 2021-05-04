using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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

            LaunchGraph();
        }

        public void ResetGraph()
        {
            // Set all nodes to NotExecuted state
            foreach (var n in nodes)
                n.OnReset();

            this.currentNode = this.entryPoint;
            
            LaunchGraph();
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
         *     --> Stocker un start time + avoir un pseudo timer dans le Tree
         * - Node Repeat ==> Repeat N fois
         */

        private const float delay = 1.0f / 60.0f;
        
        public async void LaunchGraph()
        {
            var result = currentNode.Process();

            await Task.WhenAll(result);
            
            Assert.AreNotEqual(result.Result, NodeState.Running, "Result is Running in Behavior Tree");
            Assert.AreNotEqual(result.Result, NodeState.Failure, "Result is failure in Behavior Tree");

            await Task.Delay(TimeSpan.FromSeconds(delay));
            
            ResetGraph();
        }

        public async Task<NodeState> TestTaskInsteadOfCoroutine2()
        {
            //var tmp = MyTask();
            //await Task.WhenAny(tmp, Task.Delay((10)));
            var myInt = 0;

            while (myInt < 10)
            {
                await Task.Delay(1000);
                myInt++;
                Debug.Log("Delay 100 ms " + myInt);
            }
            
            Debug.Log("On a fini la task !!! " + myInt);
            
            return NodeState.Failure;
        }

        public void callback()
        {
            Debug.Log("Ceci est un callback de test !");    
        }
        
        public async Task<NodeState> TestTaskInsteadOfCoroutine()
        {
            //var tmp = MyTask();
            //await Task.WhenAny(tmp, Task.Delay((10)));
            var myInt = 0;
            var st = NodeState.Running;
            
            while (st.Equals(NodeState.Running))
            {
                await Task.Delay(1000);
                myInt++;
                
                callback();
                
                if (myInt >= 5)
                    st = NodeState.Success;
                
                Debug.Log("Delay 100 ms " + myInt + " et mon etat = " + st);
            }
            
            Debug.Log(st + " On a fini la task !!! " + myInt);
            
            return st;
        }
    }
}
