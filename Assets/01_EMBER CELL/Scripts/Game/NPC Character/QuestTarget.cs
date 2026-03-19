using UnityEngine;

namespace TEC
{
    public class EnemyQuestTarget : MonoBehaviour
    {
        [SerializeField] private string questID;

        private CharacterBase characterBase;
        private bool isHandled = false;

        private void Awake()
        {
            characterBase = GetComponent<CharacterBase>();
        }

        private void OnEnable()
        {
            if (characterBase != null)
            {
                characterBase.OnDeadStateChanged += OnDead;
            }
        }

        private void OnDisable()
        {
            if (characterBase != null)
            {
                characterBase.OnDeadStateChanged -= OnDead;
            }
        }

        private void OnDead(bool isDead)
        {
            if (!isDead) return;
            if (isHandled) return;

            isHandled = true;

            if (string.IsNullOrEmpty(questID))
                return;

            // 현재 상태 확인
            QuestState state = QuestManager.Singleton.GetQuestState(questID);

            if (state != QuestState.InProgress)
                return;

            // 완료 처리
            QuestManager.Singleton.CompleteQuest(questID);

            Debug.Log($"[EnemyQuestTarget] Quest Complete : {questID}");
        }
    }
}