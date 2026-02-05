using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace TEC
{
    public class AIController : MonoBehaviour
    {
        //dummy transform
        [SerializeField] private Transform destinationPoint;
        private CharacterBase characterBase;
        private NavMeshAgent navAgent;
        private AISensor sensor;
        private AIBrain brain;

        private void Awake()
        {
            characterBase = GetComponent<CharacterBase>();
            navAgent = GetComponent<NavMeshAgent>();
            sensor = GetComponentInChildren<AISensor>();
            brain = GetComponent<AIBrain>();

            navAgent.updatePosition = false;
            navAgent.updateRotation = false;
        }

        public void Start()
        {
            navAgent.speed = characterBase.moveSpeed;
        }

        public void Update()
        {
            //TODO : 목적지 설정 코드를 Update -> AI가 플레이어 위치 감지했을 때 위치로 설정
            navAgent.SetDestination(destinationPoint.position); //목적지 설정 
            navAgent.nextPosition = transform.position; // NavMeshAgent의 위치를 캐릭터의 위치로

            if (navAgent.hasPath)
            {
                Vector3 moveDirection = (navAgent.steeringTarget - transform.position).normalized; //목적지 방향 계산
                Vector2 input = new Vector2(moveDirection.x, moveDirection.z); // 2D 입력벡터 생성

                characterBase.Move(input, 0); //이동
            }
            else
            {
                characterBase.Move(Vector2.zero, 0);
            }
        }

        // public void SetDestination(Vector3 destination)
        // {
        //     navAgent.SetDestination(destination);
        // }
    }
}
