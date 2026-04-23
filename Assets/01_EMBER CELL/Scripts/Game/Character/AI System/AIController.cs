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

        private bool isDead = false; 

        private void Awake()
        {
            characterBase = GetComponent<CharacterBase>();
            navAgent = GetComponent<NavMeshAgent>();
            sensor = GetComponentInChildren<AISensor>();
            brain = GetComponent<AIBrain>();

            if (navAgent != null) 
            {
                navAgent.updatePosition = false;
                navAgent.updateRotation = false;
            }
        }

        private void OnEnable() 
        {
            if (characterBase != null) 
            {
                characterBase.OnDeadStateChanged += OnDeadStateChanged; 
            }
        }

        private void OnDisable() 
        {
            if (characterBase != null) 
            {
                characterBase.OnDeadStateChanged -= OnDeadStateChanged; 
            }
        }

        public void Start()
        {
            if (navAgent != null) 
            {
                navAgent.speed = characterBase.moveSpeed;
            }

            characterBase.Initialize(false);
        }

        public void Update()
        {
            if (isDead)
                return;

            if (navAgent == null) 
                return; 

            navAgent.nextPosition = transform.position;

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

                if (navAgent.remainingDistance <= navAgent.stoppingDistance)
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
            if (isDead) 
                return; 

            if (navAgent == null) 
                return; 

            navAgent.isStopped = false;
            navAgent.SetDestination(destination);
        }

        public void Stop()
        {
            if (navAgent == null) 
                return; 

            navAgent.isStopped = true;
            // navAgent.ResetPath(); //Combat 중 경로 사라지는 문제 발생
        }

        public void EquipWeapon()
        {
            if (isDead) 
                return; 

            characterBase.EquipWeapon();
            characterBase.IsAiming = true;
        }

        public void UnEquipWeapon()
        {
            if (isDead) 
                return; 

            characterBase.HolsterWeapon();
            characterBase.IsAiming = false;
        }

        public void SetAiming(Vector3 aimingPoint)
        {
            if (isDead) 
                return; 

            characterBase.AimingPoint = aimingPoint;
        }

        public void Fire()
        {
            if (isDead) 
                return; 

            characterBase.Fire();
        }

        private void OnDeadStateChanged(bool dead) 
        {
            if (!dead)
                return; 

            isDead = true; 

            if (navAgent != null) 
            {
                navAgent.isStopped = true; 
                navAgent.ResetPath(); 
                navAgent.enabled = false; 
            }

            if (sensor != null) 
            {
                sensor.enabled = false; 
            }

            if (brain != null) 
            {
                brain.enabled = false;
            }

            enabled = false; 
        }
    }
}