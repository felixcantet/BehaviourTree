using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;
using System.Linq;
using BehaviourTree;

public class IAPatrolController : MonoBehaviour
{
    public BehaviourTree.BehaviourTree tree;


    [SerializeField] float visionMaxDistance = 15;
    [SerializeField] int coneAngle = 20;
    [SerializeField] int coneStep = 1;
    [SerializeField] public NavMeshAgent agent;
    [SerializeField] Transform target;
    [SerializeField] float rotationSpeed = 10.0f;
    [SerializeField] float attackRange = 3;
    [SerializeField] Animator anim;
    Coroutine turnRoutine;
    NodeState turnRoutineOutState = NodeState.Running;
    public List<Transform> waypoints;
    public int currentWayPoint;
    bool looseTarget = false;
    
    /// <summary>
    /// You can the visual of the tree at : Asset\Gameplay\Infiltration\Diagram_Patrol.png
    /// </summary>
    private void Awake()
    {
        // Find the closest waypoint
        var closer = waypoints.OrderBy(x => Vector3.Distance(x.position, this.transform.position)).First();
        this.currentWayPoint = this.waypoints.IndexOf(closer);

        #region Setup Tree

        // Le node d'entr�e est un selector
        BehaviourSelectorNode entry = new BehaviourSelectorNode("Entry Node");
        
        // On assigne le noeud d'entr�e au Tree
        this.tree = new BehaviourTree.BehaviourTree(entry);
        // Le premier item du Selector est une s�quence de recherche
        var searchSequence = new BehaviourSequenceNode(name = "Search Sequence");
        
        // N�cessite que l'agent n'ai pas de target (il n'a pas trouv� le joueur)
        var condition = new BehaviourConditionNode(() =>
        {
            Debug.Log("Run Has Target Condition");
            return !this.HasTarget() ? NodeState.Success : NodeState.Failure;
        }, name = "Has Target Condition");

        // Dans ce cas la, il va essayer de detecter le joueur
        var searchAction = new BehaviourActionNode(() =>
        {
            this.DetectPlayer();
            Debug.Log("Detect Player Action");
            // Renvoi Success si le joueur est trouv�
            return this.target != null ? NodeState.Success : NodeState.Failure;
        }, "Search Action");

        // On ajoute la s�quence au selector
        entry.Add(searchSequence);
        // On ajoute les noeuds qui composent la sequence dans la s�quence
        searchSequence.Add(condition);
        searchSequence.Add(searchAction);

        // Le deuxi�me item est une sequence de mouvement
        var moveSequence = new BehaviourSequenceNode(name = "Move Sequence");
        // On ajoute la s�quence 
        entry.Add(moveSequence);

        // Requier que l'agent ai le player en target
        var hasTarget = new BehaviourConditionNode(() =>
        {
            return this.HasTarget() ? NodeState.Success : NodeState.Failure;
        }, name = "Has Target");

        // On ajoute la condition en premier �l�ment de la s�quence
        moveSequence.Add(hasTarget);
        // Si l'agent a une target, alors on il se d�place vers elle
        var moveToAction = new BehaviourActionNode(() =>
        {
            this.agent.enabled = true;
            this.MoveTo(this.target.position);
            this.anim.SetBool("Move", true);
            var state = IsInAttackRange() ? NodeState.Success : NodeState.Running;
            // Si on perd la targer de vue, on retourne Failure pour passer en �tat de recherche
            if (!DetectPlayer())
            {
                this.anim.SetTrigger("Search");
                this.target = null;
                this.looseTarget = true;
                return NodeState.Failure;
            }
            return state;
        });
        // Si le joueur est dans le range, on renvoi Failure pour passer en Behaviour attack, sinon on renvoi Success
        var invertInRangeCondition = new BehaviourConditionNode(() =>
        {
            return !this.IsInAttackRange() ? NodeState.Success : NodeState.Failure;
        }, name = "Stop Move");
        // On ajoute les noeuds � la s�quence
        moveSequence.Add(moveToAction);
        moveSequence.Add(invertInRangeCondition);

        // S�quence permettant de g�rer la situation o� l'agent perd le joueur de vue
        var looseTargetSequence = new BehaviourSequenceNode("Loose Target Event");
        // Condition bas� sur un flag remplit par l'�tat pr�c�dent
        var looseTargetCondition = new BehaviourConditionNode(() =>
        {
            if (this.looseTarget)
            {
                return NodeState.Success;
            }
            return NodeState.Failure;
        });
        // On met l'agent en pause, il passe en �tat de recherche. Il attend 3 seconde dans cet �tat
        var looseTargetAction = new BehaviourDelayNode(() =>
        {
            this.agent.enabled = false;
            this.looseTarget = false;
            this.anim.SetBool("Move", false);
            this.anim.SetTrigger("Search");
            return NodeState.Success;
        }, delay: 3.0f);

        // On ajoute les noeuds � la s�quence et la s�quence au selector
        looseTargetSequence.Add(looseTargetCondition);
        looseTargetSequence.Add(looseTargetAction);
        entry.Add(looseTargetSequence);

        // S�quence d'attaque
        var attackSequence = new BehaviourSequenceNode("Attack Sequence");
        entry.Add(attackSequence);
        // N�cessite d'�tre dans le range du player
        var isInRangeCondition = new BehaviourConditionNode(() =>
        {
            return this.IsInAttackRange() ? NodeState.Success : NodeState.Failure;
        }, name = "In range Condition");
        
        attackSequence.Add(isInRangeCondition);
        // Action d'attaque
        var attackAction = new BehaviourActionNode(() =>
        {
            this.anim.SetBool("Move", false);
            this.anim.SetTrigger("Attack");
            this.agent.enabled = false;
            this.Attack();

            return NodeState.Success;
        }, name = "Attack Action");
        attackSequence.Add(attackAction);

        // D�lais de deux secondes apr�s l'attaque
        var delayNode = new BehaviourDelayNode(() =>
        {
            this.anim.SetTrigger("Search");
            return NodeState.Success;
        }, 2.0f);
        attackSequence.Add(delayNode);

        // Action Fallback de d�placement de Waypoint en waypoint
        var moveToWayPoint = new BehaviourActionNode(() =>
        {
            this.agent.enabled = true;
            this.anim.SetBool("Move", true);
            this.MoveTo(this.waypoints[currentWayPoint].position);
            if (this.DetectPlayer()) { return NodeState.Success; }
            if (agent.remainingDistance < 1.0f)
            {
                this.currentWayPoint++;
                this.currentWayPoint %= this.waypoints.Count;
                return NodeState.Success;
            }
            return NodeState.Running;
        }, "Move To Current Waypoint");

        entry.Add(moveToWayPoint);
        #endregion
        this.tree.InitGraph();
    }

    private void Update()
    {
        //this.tree.LaunchGraph();
    }

    bool HasTarget()
    {
        return this.target != null;
    }

    /// <summary>
    /// cast rays in a cone defined by [-coneAngle, +coneAngle] around forward axis
    /// Cast one ray every [coneStep] angle
    /// Return true if there is at least one transform found
    /// </summary>
    /// <param name="transform">List of all transform founds</param>
    /// <returns></returns>
    public bool DetectLineOfSight(out List<Transform> transform)
    {
        var forward = this.transform.forward;
        transform = new List<Transform>();
        for (int i = -coneAngle; i < coneAngle; i += coneStep)
        {
            RaycastHit hit;
            Ray ray = new Ray(this.transform.position, Quaternion.AngleAxis(i, Vector3.up) * forward);
            if (Physics.Raycast(ray, out hit, visionMaxDistance))
            {
                if (!transform.Contains(hit.transform))
                {
                    transform.Add(hit.transform);
                }
            }
        }
        if (transform.Count > 0)
            return true;
        return false;

    }


    void OnDrawGizmosSelected()
    {
        float totalFOV = coneAngle * 2;// 70.0f;
        float rayRange = visionMaxDistance;
        float halfFOV = totalFOV / 2.0f;
        Quaternion leftRayRotation = Quaternion.AngleAxis(-halfFOV, Vector3.up);
        Quaternion rightRayRotation = Quaternion.AngleAxis(halfFOV, Vector3.up);
        Vector3 leftRayDirection = leftRayRotation * transform.forward;
        Vector3 rightRayDirection = rightRayRotation * transform.forward;
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, leftRayDirection * rayRange);
        Gizmos.DrawRay(transform.position, rightRayDirection * rayRange);

    }

    /// <summary>
    /// Set the navmesh agent destination to [target] position
    /// </summary>
    /// <param name="target"></param>
    public void MoveTo(Vector3 target)
    {
        agent.SetDestination(target);
        // TODO : Set Animation
    }

    /// <summary>
    /// Return true if current target is in attack range
    /// </summary>
    /// <returns></returns>
    public bool IsInAttackRange()
    {
        if (this.target == null)
            return false;
        return Vector3.Distance(this.target.position, this.transform.position) < this.attackRange;
    }

    /// <summary>
    /// Trigger Detection and set player to target if Player is detected
    /// </summary>
    /// <returns></returns>
    public bool DetectPlayer()
    {
        List<Transform> tr;
        if (this.DetectLineOfSight(out tr))
        {
            var player = tr.Where(x => x.CompareTag("Player")).FirstOrDefault();
            if (player)
            {
                this.target = player;
                return true;
            }
            return false;

        }

        return false;

    }

    /// <summary>
    /// Reset Target variable
    /// </summary>
    public void ForgotPlayer()
    {
        this.target = null;
    }

    public void Attack()
    {
        // TODO 
        // Attack Behaviour
        // Set Animation
        Debug.Log("Attack");
    }
}
