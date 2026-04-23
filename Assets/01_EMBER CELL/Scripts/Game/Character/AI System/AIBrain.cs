using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

namespace TEC
{
    public enum AIState
    {
        Combat,
        Patrol,
    }

    public class AIBrain : MonoBehaviour
    {
        public AIController AIController => controller;
        public CharacterBase TargetCharacter => targetCharacter;

        public AIState initState = AIState.Patrol;
        public Transform[] patrolPoints;

        private AIController controller;
        private AISensor detectSensor;

        private AIState currentAIState;
        private IState currentState;
        private CombatState combatState;
        private PatrolState patrolState;
        private Vector3[] patrolPositions;

        private CharacterBase targetCharacter;

        private void Awake()
        {
            patrolPositions = patrolPoints.Select(p => p.position).ToArray();

            controller = GetComponent<AIController>();
            combatState = new CombatState(this);
            patrolState = new PatrolState(this, patrolPositions);

            detectSensor = GetComponentInChildren<AISensor>();
        }

        private void OnEnable()
        {
            if (detectSensor != null) 
            {
                detectSensor.OnDetectedCharacter += OnDetectedCharacter; 
                detectSensor.OnLostCharacter += OnLostCharacter; 
            }
        }

        private void OnDisable() 
        {
            if (detectSensor != null) 
            {
                detectSensor.OnDetectedCharacter -= OnDetectedCharacter; 
                detectSensor.OnLostCharacter -= OnLostCharacter; 
            }
        }

        private void Start()
        {
            SetState(initState);
        }

        private void Update()
        {
            if (controller != null && controller.enabled == false) 
                return; 

            if (targetCharacter != null && targetCharacter.IsDead)
            {
                targetCharacter = null;

                if (currentAIState != AIState.Patrol)
                    SetState(AIState.Patrol);

                return;
            }

            currentState?.Update();
        }

        public void SetState(AIState newState)
        {
            currentState?.Exit();

            currentAIState = newState;
            switch (newState)
            {
                case AIState.Patrol: currentState = patrolState; break;
                case AIState.Combat: currentState = combatState; break;
            }

            currentState?.Enter();
        }

        void OnDetectedCharacter(CharacterBase character)
        {
            if (character == null) 
                return;

            if (character.IsDead) 
                return; 

            targetCharacter = character;

            SetState(AIState.Combat);
        }

        void OnLostCharacter(CharacterBase character)
        {
            targetCharacter = null;

            SetState(AIState.Patrol);
        }
    }
}