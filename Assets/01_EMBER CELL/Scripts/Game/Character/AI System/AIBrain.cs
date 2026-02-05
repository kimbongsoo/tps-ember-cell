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
            patrolPositions = patrolPoints.Select(p => p.position).ToArray(); //Transform 배열을 Vector3 배열로 변환

            controller = GetComponent<AIController>();
            combatState = new CombatState(this);
            patrolState = new PatrolState(this, patrolPositions);

            detectSensor = GetComponentInChildren<AISensor>();
            detectSensor.OnDetectedCharacter += OnDetectedCharacter;
            detectSensor.OnLostCharacter += OnLostCharacter;
        }

        private void Start()
        {
            SetState(initState);
        }

        private void Update()
        {
            currentState?.Update(); //현재 상태 업데이트
        }

        public void SetState(AIState newState)
        {
            currentState?.Exit(); //현재 상태 종료
            
            //새로운 상태 설정
            currentAIState = newState;
            switch (newState)
            {
                case AIState.Patrol: currentState = patrolState; break;
                case AIState.Combat: currentState = combatState; break;
            }
            
            currentState?.Enter(); // 새로운 상태 시작
        }

        void OnDetectedCharacter(CharacterBase character)
        {
            targetCharacter = character; //타켓 설정

            SetState(AIState.Combat);
        }

        void OnLostCharacter(CharacterBase character)
        {
            targetCharacter = null; // 타겟 해제

            SetState(AIState.Patrol);
        }
    }
}
