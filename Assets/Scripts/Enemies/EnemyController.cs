using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{


    public float lookRadius = 10f;
    public bool targetInRange;
    public bool movingToTarget = false;
    public Transform closestPlayer;
    bool checkingForEnemy = false;

    [SerializeField] private Transform target = null;

    public NavMeshAgent agent;

    private void Start()
    {

        EnemyFindTarget();
        agent = GetComponent<NavMeshAgent>();
        closestPlayer = null;

    }

    private void Update()
    {
        closestPlayer = SetTarget();
        if (checkingForEnemy == false)
        {
        StartCoroutine(GetTargetTimer());
        }
        EnemyFindTarget();
    }

    public IEnumerator GetTargetTimer()
    {
        checkingForEnemy = true;
        yield return new WaitForSecondsRealtime(2f);
        {
            SetTarget();
            Debug.Log("Our attack target is " + closestPlayer);
            checkingForEnemy = false;
        }
    }

    public Transform SetTarget()
    {
        float closestDistance = Mathf.Infinity;
        Transform attackTarget = null;

        foreach (string _playerID in GameManager.players.Keys)
        {
            target = GameManager.players[_playerID].transform;
            float currentDistance;
            currentDistance = Vector3.Distance(transform.position, target.position);
            if (currentDistance < closestDistance)
            {
                closestDistance = currentDistance;
                attackTarget = target.transform;
            }
        }
        return attackTarget;
    }

    public void EnemyFindTarget()
    {
        if (closestPlayer != null)
        {
            float distance = Vector3.Distance(closestPlayer.position, transform.position);

            if (distance <= lookRadius)
            {
                agent.SetDestination(closestPlayer.position);

                if (distance <= agent.stoppingDistance)
                {
                    //attack the target
                    movingToTarget = true;
                    FaceTarget();
                    targetInRange = true;
                }
                if (distance >= agent.stoppingDistance)
                {   
                    targetInRange = false;
                    movingToTarget = true;
                }
            }
        }

    }


    void FaceTarget()
    {
        Vector3 direction = (closestPlayer.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }
}
