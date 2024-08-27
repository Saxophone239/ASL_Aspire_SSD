using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class PatrolEnemy : MonoBehaviour
{
    [SerializeField] private NavMeshAgent enemyAgent;
    // [SerializeField] private Player player;
	public Player player;
    public LayerMask whatIsGround;
    public LayerMask whatIsPlayer;

    // Patrolling
    public Vector3 walkPoint;
    private bool isWalkPointSet;
    public float walkPointRange;


    // States
    public float renderRange; // Enemy only moves if player can see it, done for performance purposes
    public bool isPlayerInRenderRange;
    public float sightRange;
    public bool isPlayerInSightRange;

    // Start is called before the first frame update
    private void Start()
    {
        enemyAgent = GetComponent<NavMeshAgent>();
        StartCoroutine(Delay());
    }

    // Update is called once per frame
    private void Update()
    {
		if (player == null)
		{
			// Player is not yet found, find player
			if (GameObject.FindGameObjectWithTag("Player").TryGetComponent<Player>(out Player playerComponent))
			{
				player = playerComponent;
			}
			return;
		}

        isPlayerInRenderRange = Physics.CheckSphere(transform.position, renderRange, whatIsPlayer);
        isPlayerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);

        if (isPlayerInRenderRange) {
            if(isPlayerInSightRange){
                // If the coin is touched but the enemy hasn't stopped
                if(!enemyAgent.isStopped && player.IsCoinTouched){
                    enemyAgent.isStopped = true;
                }else if(enemyAgent.isStopped && !player.IsCoinTouched){
                    enemyAgent.isStopped = false;
                } else {
                    ChasePlayer();
                }
            } else {
                Patrol();
            }
        }
    }

    private void Patrol()
    {
        if (!isWalkPointSet) SearchWalkPoint();
        else enemyAgent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        // Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
        {
            isWalkPointSet = false;
        }
        
    }

    private void SearchWalkPoint()
    {
        // Collect random point in range
        float randomX = UnityEngine.Random.Range(-walkPointRange, walkPointRange);
        float randomZ = UnityEngine.Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2.0f, whatIsGround))
        {
            isWalkPointSet = true;
        }
    }

    private void ChasePlayer()
    {
        enemyAgent.SetDestination(player.gameObject.transform.position);
    }

    private IEnumerator Delay()
    {
        yield return new WaitForSecondsRealtime(3.0f);
    }
}
