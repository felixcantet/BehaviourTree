using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BehaviourTree;
using UnityEngine;
using UnityEngine.AI;

public class AIBehavior : MonoBehaviour
{
    public BehaviourTree.BehaviourTree tree;

    public NavMeshAgent agent;
    public Transform target;
    
    [SerializeField] private float visionMaxDistance = 15;
    [SerializeField] private int coneAngle = 20;
    [SerializeField] private int coneStep = 1;
    
    [Header("Parameters")] 
    public float health = 100.0f;
    
    public float attackRange = 2.0f;
    public float spellRange = 8.0f;
    public float spellDelay = 5.0f;
    public float healDelay = 10.0f;

    private bool healAvailable = false;
    private float currentHealDelay = 0.0f;
    
    private bool spellAvailable = false;
    private float currentSpellDelay = 0.0f;

    [Header("Feedback Prefab")] 
    public GameObject healFeedback;
    public FireBall spellFeedback;
    
    /// <summary>
    /// Initialize IA
    /// And it's behavior tree
    /// At this path : Assets\Gameplay\Fighting\FightingAITree.png
    /// You can find the visual of the Behavior tree
    /// </summary>
    private void Awake()
    {
        this.agent = GetComponent<NavMeshAgent>();

        this.currentHealDelay = Time.time + this.healDelay;
        this.currentSpellDelay = Time.time + this.spellDelay;
        
        var mainBehavior = new BehaviourSelectorNode("Main Behavior");
        var rootNode = new BehaviorRootNode(mainBehavior);
        
        // Heal Behavior Sequence
        var healBehavior = new BehaviourSequenceNode("Heal Sequence");
        var isCloseToDieBehavior = new BehaviourConditionNode(() => IsCloseToDie() ? NodeState.Success : NodeState.Failure, "Need Heal");
        
        var isHealAvailableBehavior = new BehaviourConditionNode(() =>
        {
            return IsHealAvailable() ? NodeState.Success : NodeState.Failure;
        }, "Heal Available");
        
        var castHealBehavior = new BehaviourDelayNode(() =>
        {
            Heal();
            return NodeState.Success;
        }, 2.0f, "Heal Action");
        healBehavior.Add(castHealBehavior);
        healBehavior.Add(isCloseToDieBehavior, 0);
        healBehavior.Add(isHealAvailableBehavior, 1);
        
        mainBehavior.Add(healBehavior);
        
        // Attack Selector
        var attackBehavior = new BehaviourSelectorNode("Attack Selector");

        // Spell Behavior Sequence
        var spellBehavior = new BehaviourSequenceNode("Spell Sequence");
        var isSpellAvailable = new BehaviourConditionNode(() => IsSpellAvailable() ? NodeState.Success : NodeState.Failure, "Spell Available");
        var isPlayerInSpellRange =  new BehaviourConditionNode(() => IsInSpellRange() ? NodeState.Success : NodeState.Failure, "Spell Range");
        var castSpell = new BehaviourDelayNode(() =>
        {
            CastSpell(this.target.position);
            return NodeState.Success;
        }, 2.0f, "Spell Action");
        
        spellBehavior.Add(isSpellAvailable);
        spellBehavior.Add(isPlayerInSpellRange);
        spellBehavior.Add(castSpell);
        
        attackBehavior.Add(spellBehavior);
        
        var cacBehavior = new BehaviourSequenceNode("Cac Sequence");
        var isPlayerInRange =  new BehaviourConditionNode(() => IsInAttackRange() ? NodeState.Success : NodeState.Failure, "Cac Range");
        var attackCacBehavior = new BehaviourDelayNode(() =>
        {
            Attack();
            return NodeState.Success;
        }, 2.0f, "Attack Action");
        
        cacBehavior.Add(isPlayerInRange);
        cacBehavior.Add(attackCacBehavior);
        
        attackBehavior.Add(cacBehavior);
        mainBehavior.Add(attackBehavior);
        
        // Move Behaviour
        
        var moveBehavior = new BehaviorForceSuccess(() =>
        {
            this.agent.enabled = true;
            this.MoveTo(this.target.position);

            return (this.IsHealAvailable() || this.IsSpellAvailable() || this.IsInAttackRange())
                ? NodeState.Success
                : NodeState.Running;
            
        }, "Move Action");
        
        mainBehavior.Add(moveBehavior);
        
        this.tree = new BehaviourTree.BehaviourTree(rootNode);
        
        tree.InitGraph();
    }

    private void Update()
    {
        if (this.currentHealDelay < Time.time)
            this.healAvailable = true;

        if (this.currentSpellDelay < Time.time)
            this.spellAvailable = true;

        if (Input.GetMouseButtonUp(0))
        {
            Debug.Log($"Remove 10 hp from AI ==> Actual HP {this.health}");
            this.health -= 10.0f;
        }
    }

    public bool HasTarget()
    {
        return this.target != null;
    }

    public bool IsInAttackRange()
    {
        if (this.target == null)
            return false;
        return Vector3.Distance(this.target.position, this.transform.position) < this.attackRange;
    }

    public bool IsInSpellRange()
    {
        if (this.target == null)
            return false;

        var dist = Vector3.Distance(this.target.position, this.transform.position);
        
        return dist < this.spellRange && dist > 2.0f;
    }
    
    public bool IsSpellAvailable()
    {
        return this.spellAvailable;
    }

    public bool IsHealAvailable()
    {
        return this.healAvailable;
    }

    public bool IsCloseToDie()
    {
        return this.health < 33.333f;
    }
    
    
    /// <summary>
    /// Set the navmesh agent destination to [target] position
    /// </summary>
    /// <param name="target"></param>
    public void MoveTo(Vector3 target)
    {
        if(!this.agent.enabled)
            this.agent.enabled = true;
        
        agent.SetDestination(target);
        // TODO : Set Animation
    }

    
    public void CastSpell(Vector3 target)
    {
        if (!spellAvailable)
            return;
        
        this.agent.ResetPath();
        this.agent.enabled = false;
        
        Debug.Log("Cast a FIRE BALL");

        var go = Instantiate(spellFeedback,
            new Vector3(this.transform.position.x, this.agent.height * 0.5f, this.transform.position.z) +
            transform.forward * 1.5f, Quaternion.identity);
        
        go.direction = new Vector3(target.x, 0, target.z) - new Vector3(this.transform.position.x, 0, this.transform.position.z);
        go.direction.Normalize();
        
        Destroy(go.gameObject, this.spellDelay * 0.75f);

        
        this.spellAvailable = false;
        this.currentSpellDelay = Time.time + this.spellDelay;
    }
    
    
    public void Heal()
    {
        if (!healAvailable)
            return;
        
        this.agent.ResetPath();
        this.agent.enabled = false;
        
        Debug.Log("Heal myself");

        var go = Instantiate(healFeedback, transform);
        Destroy(go, 2.25f);

        this.health += 30.0f;
        Debug.Log($"I've more HP now ! ==> Actual HP {this.health}");
        this.healAvailable = false;
        this.currentHealDelay = Time.time + this.healDelay;
    }

    public void Attack()
    {
        // TODO 
        // Attack Behaviour
        // Set Animation
        Debug.Log("Attack");
        
        this.agent.ResetPath();
        this.agent.enabled = false;
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
    
    
    /// <summary>
    /// Trigger Detection and set player to target if Player is detected
    /// </summary>
    /// <returns></returns>
    public bool DetectPlayer()
    {
        List<Transform> tr;
        if(this.DetectLineOfSight(out tr))
        {
            var player = tr.Where(x => x.CompareTag("Player")).First();
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
}
