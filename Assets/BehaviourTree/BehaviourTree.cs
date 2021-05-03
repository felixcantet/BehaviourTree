using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    [System.Serializable]
    public class BehaviourTree
    {
        private BaseBehaviourNode currentNode;
        private BaseBehaviourNode entryPoint;

        void InitGraph()
        {
            GetEntryNode();
        }

        void ResetGraph()
        {
            // Set all nodes to NotExecuted state
        }

        void GetEntryNode()
        {

        }

        void GetNext()
        {

        }

        void Tick()
        {

        }
    }
}
