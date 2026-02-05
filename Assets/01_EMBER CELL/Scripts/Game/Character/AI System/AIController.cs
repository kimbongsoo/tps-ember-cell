using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace TEC
{
    public class AIController : MonoBehaviour
    {
        private CharacterBase characterBase;
        private NavMeshAgent navAgent;
        private AISensor sensor;
        private AIBrain brain;

        public event System.Action OnDestinationReached;

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
            navAgent.nextPosition = transform.position; // NavMeshAgent의 위치를 캐릭터의 위치로

            if (navAgent.hasPath && !navAgent.pathPending)
            {
                Vector3 moveDirection = (navAgent.steeringTarget - transform.position).normalized; //목적지 방향 계산
                Vector2 input = new Vector2(moveDirection.x, moveDirection.z); // 2D 입력벡터 생성

                characterBase.Move(input, 0); //이동

                // 목적지 도착했는지 판단
                if(navAgent.remainingDistance <= navAgent.stoppingDistance)
                {
                    navAgent.isStopped = true;
                    navAgent.ResetPath();
                    OnDestinationReached?.Invoke();
                }
            }
            else
            {
                characterBase.Move(Vector2.zero, 0);
            }
        }

        public void SetDestination(Vector3 destination)
        {
            navAgent.isStopped = false;
            navAgent.SetDestination(destination);
        }

        public void Stop()
        {
            navAgent.isStopped = true;
            navAgent.ResetPath();
        }

        public void EquipWeapon()
        {
            characterBase.EquipWeapon();
            characterBase.IsAiming = true;
        }

        public void UnEquipWeapon()
        {
            characterBase.HolsterWeapon();
            characterBase.IsAiming = false;
        }

        public void SetAiming(Vector3 aimingPoint)
        {
            characterBase.AimingPoint = aimingPoint;
        }

        public void Fire()
        {
            characterBase.Fire();
        }
    }
}
