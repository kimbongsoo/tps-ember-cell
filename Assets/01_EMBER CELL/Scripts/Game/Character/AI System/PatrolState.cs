using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TEC
{
    public class PatrolState : IState
    {
        //Patrol State의 생성자
        public PatrolState(AIBrain brain, Vector3[] arrPatrolPoint)
        {
            this.brain = brain;
            this.patrolPoints = arrPatrolPoint;
        }

        public AIBrain AIBrain => brain;

        public Vector3[] patrolPoints;
        public float randomWaitTime = 2f;
        public int patrolIndex = 0;
        private float patrolTimer = 0f;
        private bool isWaiting = false;

        private AIBrain brain;

        public void Enter()
        {
            // Patrol State에 진입한 첫 순간
            brain.AIController.OnDestinationReached += OnDestinationReached;
            // ExecutePatrolPlan(0); // 첫 번째 인덱스의 포인트가 목적지가 됨

            //추가
            isWaiting = false; // 복귀 시 상태 초기화
            ExecutePatrolPlan(patrolIndex); // patrolIndex 0 고정 대신 현재 patrolIndex로

        }
        public void Update()
        {
            if(isWaiting == false) 
                return;

            if(Time.time > patrolTimer + randomWaitTime)
            {
                int nextIndex = (patrolIndex + 1) % patrolPoints.Length; //다음 인덱스 계산(순환)
                ExecutePatrolPlan(nextIndex);
            }
        }

        public void Exit()
        {
            brain.AIController.OnDestinationReached -= OnDestinationReached;
        }

        void ExecutePatrolPlan(int index)
        {
            isWaiting = false; //대기 상태 해제
            patrolIndex = index; // Patrol 시작 인덱스 초기화       
            randomWaitTime = Random.Range(3f, 5f); //랜덤시간 대기
            brain.AIController.SetDestination(patrolPoints[patrolIndex]); //첫 번째 목적지 설정
        }

        void OnDestinationReached()
        {
            isWaiting = true;
            patrolTimer = Time.time; 
        }
    }
}
