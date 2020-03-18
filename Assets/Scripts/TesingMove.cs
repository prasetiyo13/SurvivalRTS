using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class TesingMove : MonoBehaviour
{
    NavMeshAgent agent;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        InvokeRepeating("RandomMove", 1f, 5f);
    }
    private void RandomMove()
    {
        var direction = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1, 1f));
        var destination = transform.position + direction * 10f;
        agent.SetDestination(destination);
    }
}
