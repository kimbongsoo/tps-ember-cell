using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace TEC
{
    public class AIController : MonoBehaviour
    {
        //dummy transform
        [SerializeField] private Transform dummyDestination;
        private NavMeshAgent navAgent;
        private AISensor sensor;
        private AIBrain brain;

        private void Awake()
        {
            navAgent = GetComponent<NavMeshAgent>();
            sensor = GetComponentInChildren<AISensor>();
            brain = GetComponent<AIBrain>();
        }

        public void Update()
        {
            SetDestination(dummyDestination.position);
        }

        public void SetDestination(Vector3 destination)
        {
            navAgent.SetDestination(destination);
        }
    }
}
