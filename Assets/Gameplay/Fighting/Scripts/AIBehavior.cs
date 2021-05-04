using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    
    private void Awake()
    {
        this.agent = GetComponent<NavMeshAgent>();

        this.currentHealDelay = Time.time + this.healDelay;
        this.currentSpellDelay = Time.time + this.spellDelay;
    }

    private void Update()
    {
        if (this.currentHealDelay < Time.time)
            this.healAvailable = true;

        if (this.currentSpellDelay < Time.time)
            this.spellAvailable = true;
    }

    public bool HasTarget()
    {
        return this.target != null;
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
        agent.SetDestination(target);
        // TODO : Set Animation
    }

    public void CastSpell(Vector3 target)
    {
        if (!spellAvailable)
            return;
        
        Debug.Log("Cast a FIRE BALL");
    }

    public void Heal()
    {
        if (!healAvailable)
            return;
        
        Debug.Log("Heal myself");
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
