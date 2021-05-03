using UnityEngine;
using UnityEngine.Assertions;

namespace BehaviourTree
{
    /// <summary>
    /// Tant que c'est Success je repeat
    /// </summary>
    public class BehaviorRepeatNode : BaseBehaviourNode
    {
        [SerializeField] private int repeatNumber = 1;
        
        public BehaviorRepeatNode(ActionBool callback, int repeatNumber, string nName = "node", BaseBehaviourNode child = null) : base(callback, nName, child)
        {
            Assert.AreNotEqual(repeatNumber, 0, "You've set repeat number to 0. This is not possible !");
            this.repeatNumber = repeatNumber;
        }

        public override void OnStart()
        {
            
        }

        public override void OnEnter()
        {
            this.state = NodeState.Running;
        }

        public override void OnExit()
        {
            Debug.Log($"{nodeName} On Exit()");
            //throw new NotImplementedException();
        }

        public override void OnReset()
        {
            this.state = NodeState.NotExecuted;
        }
        
        public override NodeState Process()
        {
            NodeState lastState = this.state;

            for (int i = 0; i < repeatNumber; i++)
            {
                lastState = actionCallback() ? NodeState.Success : NodeState.Failure;
                if (lastState.Equals(NodeState.Failure))
                    break;
            }

            this.state = lastState;
            OnExit();
            return this.state;
        }
    }
}