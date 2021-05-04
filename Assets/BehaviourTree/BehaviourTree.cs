using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

namespace BehaviourTree
{
    /// <summary>
    /// Classe principale qui stock l'arbre d'un agent
    /// </summary>
    [System.Serializable]
    public class BehaviourTree
    {
        // Contient toutes les nodes à la "surface" d'un arbre pour pouvoir les resets
        [SerializeField] private List<BaseBehaviourNode> nodes = new List<BaseBehaviourNode>();
        
        private BaseBehaviourNode entryPoint;
        private BaseBehaviourNode currentNode;

        /// <summary>
        /// Create the tree
        /// </summary>
        /// <param name="entryNode">The entry node have to be a Root node with a child</param>
        public BehaviourTree(BaseBehaviourNode entryNode)
        {
            this.nodes.Add(entryNode);
            this.entryPoint = entryNode;
            this.currentNode = this.entryPoint;
        }
        
        /// <summary>
        /// Initialize the graph
        /// Search all nodes and "start" it
        /// Finally launch the graph
        /// </summary>
        public void InitGraph()
        {
            Debug.LogWarning("Initialize Behavior tree");

            SearchAllNodes();
            
            Debug.Log($"There is {this.nodes.Count} nodes");
            
            foreach (var n in nodes)
            {
                n.tree = this;
                n.OnStart();
            }

            LaunchGraph();
        }

        
        private void AddChildNode(ref List<BaseBehaviourNode> tmp, BaseBehaviourNode tmpN)
        {
            tmp.Add(tmpN);
            
            if(tmpN.HasChild())
                AddChildNode(ref tmp, tmpN.GetChild());
        }
        
        private void SearchAllNodes()
        {
            this.nodes = new List<BaseBehaviourNode>();
            this.nodes.Add(entryPoint);
            
            if(entryPoint.HasChild())
                AddChildNode(ref this.nodes, entryPoint.GetChild());
            
        }
        
        /// <summary>
        /// Reset the graph to the first node => Root
        /// Re run the graph
        /// </summary>
        public void ResetGraph()
        {
            Debug.LogWarning("Reset Behavior Tree");
            
            // Set all nodes to NotExecuted state
            foreach (var n in nodes)
                n.OnReset();

            this.currentNode = this.entryPoint;
            
            LaunchGraph();
        }
        
        
        private const float delay = 1.0f / 60.0f;
        /// <summary>
        /// Launch the graph
        /// When it finish, wait a small delay to Reset it
        /// </summary>
        public async void LaunchGraph()
        {
            Debug.LogWarning("Launch Behavior Tree");
            
            var result = currentNode.Process();

            await Task.WhenAll(result);
            
            Debug.LogWarning("Graph as reach the end potential node");
            
            Assert.AreNotEqual(result.Result, NodeState.Running, "Result is Running in Behavior Tree");
            Assert.AreNotEqual(result.Result, NodeState.Failure, "Result is failure in Behavior Tree");

            await Task.Delay(TimeSpan.FromSeconds(delay));

            ResetGraph();
        }

        #region Test Function
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
        #endregion
    }
}
