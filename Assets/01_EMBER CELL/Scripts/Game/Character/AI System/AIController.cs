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

            characterBase.Initialize(false);
        }

        public void Update()
        {
            //TODO : 목적지 설정 코드를 Update -> AI가 플레이어 위치 감지했을 때 위치로 설정
            navAgent.nextPosition = transform.position; // NavMeshAgent의 위치를 캐릭터의 위치로

            if (navAgent.hasPath && !navAgent.pathPending)
            {
                Vector3 desire = navAgent.desiredVelocity;
                desire.y = 0f;

                if (desire.sqrMagnitude > 0.0001f)
                {
                    transform.forward = desire.normalized;

                    Vector3 local = transform.InverseTransformDirection(desire.normalized);
                    Vector2 input = new Vector2(local.x, local.z);

                    characterBase.Move(input, 0);       
                }
                else
                {
                    characterBase.Move(Vector2.zero, 0);
                }
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

            // 추가 NavMesh 위로 보정
            // if (!navAgent.isOnNavMesh)
            // {
            //     if (NavMesh.SamplePosition(transform.position, out NavMeshHit selfHit, 3.0f, NavMesh.AllAreas))
            //     {
            //         navAgent.Warp(selfHit.position);
            //     }
            //     else
            //     {
            //         navAgent.ResetPath();
            //         return;
            //     }
            // }

            // // [CHANGED] 목적지 보정 반경 확대
            // if (NavMesh.SamplePosition(destination, out NavMeshHit destHit, 5.0f, NavMesh.AllAreas))
            // {
            //     navAgent.SetDestination(destHit.position);
            //     return;
            // }

            // navAgent.ResetPath();
        }

        public void Stop()
        {
            navAgent.isStopped = true;
            // navAgent.ResetPath(); //Combat 중 경로 사라지는 문제 발생
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
