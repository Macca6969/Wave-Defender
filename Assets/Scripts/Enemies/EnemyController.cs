using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{


    public float lookRadius = 10f;
    public bool targetInRange;
    public bool hasTarget = false;

    [SerializeField] private Transform target = null;

    public NavMeshAgent agent;

    private void Start()
    {

        EnemyFindTarget();
        agent = GetComponent<NavMeshAgent>();
        
    }

    private void Update()
    {

        

        foreach (string _playerID in GameManager.players.Keys)
        {
            EnemyFindTarget();
        }
    }

    public void EnemyFindTarget()
    {

        foreach (string _playerID in GameManager.players.Keys)
        {
            target = GameManager.players[_playerID].transform;
           // Player target = GameManager.players[_playerID];
            Debug.Log("the target is " + target);
            hasTarget = true;
        }

        if (hasTarget = true && target != null)
        {
        float distance = Vector3.Distance(target.position, transform.position);

        if (distance <= lookRadius)
        {
            agent.SetDestination(target.position);

            if (distance <= agent.stoppingDistance)
            {
                //attack the target
                FaceTarget();
                targetInRange = true;
            }
            if (distance >= agent.stoppingDistance)
            {
                targetInRange = false;
            }
        }
        }
     
    }


    void FaceTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }
}
