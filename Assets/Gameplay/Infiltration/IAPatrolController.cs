using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;
using System.Linq;

public class IAPatrolController : MonoBehaviour
{
    float visionMaxDistance;
    int coneAngle = 20;
    int coneStep = 1;
    public NavMeshAgent agent;
    Transform target;
    float rotationSpeed = 10.0f;
    float attackRange;

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
    /// Set the navmesh agent destination to [target] position
    /// </summary>
    /// <param name="target"></param>
    public void MoveTo(Vector3 target)
    {
        agent.SetDestination(target);
        // TODO : Set Animation
    }

    /// <summary>
    /// Disable agent and force agent to turn over  for [degree] degrees
    /// Trigger the action event when finish
    /// </summary>
    /// <param name="degree"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public IEnumerator TurnOver(float degree = 360, Action onTurnFinished = null)
    {
        this.agent.enabled = false;
        float currentDegree = 0;
        while(currentDegree < degree)
        {
            var rotation = Quaternion.AngleAxis(this.rotationSpeed, Vector3.up);
            currentDegree += rotationSpeed;
            this.transform.rotation = rotation * this.transform.rotation;
            yield return 0;
        }
        // Trigger Event Finished
        if(onTurnFinished != null)
            onTurnFinished();
        this.agent.enabled = true;
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

    public void Attack()
    {
        // TODO 
        // Attack Behaviour
        // Set Animation
    }
}
