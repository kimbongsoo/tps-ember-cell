using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TEC
{
    public class DoorInteraction : MonoBehaviour, IInteractionProvider
    {
        [Header("Interaction UI")]
        [SerializeField] private Sprite actionIcon;
        [SerializeField] private string lockedMessage;
        [SerializeField] private string openMessage;
        [SerializeField] private string openedMessage;

        [Header("Quest Lock")]
        [SerializeField] private string requiredQuestID;

        [Header("Door Setting")]
        [SerializeField] private Transform doorPivot;
        [SerializeField] private float openAngle = 90f;
        [SerializeField] private float openSpeed = 5f;

        [Header("Sequence")]
        [SerializeField] private Transform interactionPoint;
        [SerializeField] private float stopDistance = 0.15f;
        [SerializeField] private float moveRotateSpeed = 10f;
        [SerializeField] private float sequenceDelay = 0.4f;

        [Header("Sequence Camera")]
        [SerializeField] private Transform sequenceCameraFollowPoint;
        [SerializeField] private Transform sequenceCameraLookPoint;

        [Header("Scene Transition")] // 추가
        [SerializeField] private SceneType nextSceneType = SceneType.IngameLevel; // 추가

        private bool isOpen = false;
        private bool isSequencePlaying = false;

        private Quaternion closedRotation;
        private Quaternion openedRotation;

        private List<IInteractionData> interactions = new();

        public List<IInteractionData> Interactions => interactions;

        private void Awake()
        {
            if (doorPivot == null)
                doorPivot = transform;

            closedRotation = doorPivot.localRotation;
            openedRotation = closedRotation * Quaternion.Euler(0f, openAngle, 0f);

            RefreshInteractionData();
        }

        private void Start()
        {
            QuestManager.Singleton.OnQuestStateChanged += OnQuestStateChanged;
            RefreshInteractionData();
        }

        private void OnDisable()
        {
            if (QuestManager.Singleton != null)
            {
                QuestManager.Singleton.OnQuestStateChanged -= OnQuestStateChanged;
            }
        }

        private void Update()
        {
            Quaternion targetRotation = isOpen ? openedRotation : closedRotation;
            doorPivot.localRotation = Quaternion.Lerp(
                doorPivot.localRotation,
                targetRotation,
                Time.deltaTime * openSpeed
            );
        }

        public void Interact(IInteractionData data)
        {
            if (data == null)
                return;

            if (data.ID != "DOOR")
                return;

            if (isSequencePlaying)
                return;

            if (isOpen)
                return;

            if (CanInteract() == false)
            {
                RefreshInteractionData();
                return;
            }

            StartCoroutine(OpenDoorSequence());
        }

        private IEnumerator OpenDoorSequence()
        {
            isSequencePlaying = true;
            RefreshInteractionData();

            CharacterPlayerController playerController = CharacterPlayerController.Instance;
            if (playerController == null)
            {
                isSequencePlaying = false;
                RefreshInteractionData();
                yield break;
            }

            CharacterBase playerCharacter = playerController.GetComponent<CharacterBase>();
            if (playerCharacter == null)
            {
                isSequencePlaying = false;
                RefreshInteractionData();
                yield break;
            }

            playerController.SetSequenceControl(true);

            if (CameraSystem.Instance != null
                && sequenceCameraFollowPoint != null
                && sequenceCameraLookPoint != null)
            {
                CameraSystem.Instance.EnterDialogueMode(sequenceCameraFollowPoint, sequenceCameraLookPoint);
            }

            if (interactionPoint != null)
            {
                yield return MovePlayerToPoint(playerController.transform, playerCharacter, interactionPoint.position);
            }

            playerCharacter.Move(Vector2.zero, 0f);

            isOpen = true;
            RefreshInteractionData();

            yield return new WaitForSeconds(sequenceDelay);

            if (CameraSystem.Instance != null)
            {
                CameraSystem.Instance.ExitDialogueMode();
            }

            playerController.SetSequenceControl(false);
            isSequencePlaying = false;
            RefreshInteractionData();

            if (playerController.InteractionSensor != null)
            {
                playerController.InteractionSensor.PulseManuallyNextFrame();
            }

            // 🔥 [CHANGED] 씬 전환 추가 (핵심)
            if (Main.Singleton != null)
            {
                Main.Singleton.ChangeScene(nextSceneType);
            }
        }

        private IEnumerator MovePlayerToPoint(Transform playerTransform, CharacterBase playerCharacter, Vector3 destination)
        {
            while (true)
            {
                Vector3 direction = destination - playerTransform.position;
                direction.y = 0f;

                float distance = direction.magnitude;
                if (distance <= stopDistance)
                    break;

                Vector3 moveDirection = direction.normalized;

                if (moveDirection.sqrMagnitude > 0.0001f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                    playerTransform.rotation = Quaternion.Slerp(
                        playerTransform.rotation,
                        targetRotation,
                        Time.deltaTime * moveRotateSpeed
                    );
                }

                playerCharacter.Move(new Vector2(0f, 1f), 0f);
                yield return null;
            }

            playerCharacter.Move(Vector2.zero, 0f);
        }

        private bool CanInteract()
        {
            if (string.IsNullOrEmpty(requiredQuestID))
                return true;

            QuestState state = QuestManager.Singleton.GetQuestState(requiredQuestID);
            return state != QuestState.NotStarted;
        }

        private void RefreshInteractionData()
        {
            interactions.Clear();

            string message = GetCurrentActionMessage();
            interactions.Add(new DoorInteractionData("DOOR", actionIcon, message));
        }

        private string GetCurrentActionMessage()
        {
            if (isSequencePlaying)
                return string.Empty;

            if (CanInteract() == false)
                return lockedMessage;

            if (isOpen)
                return openedMessage;

            return openMessage;
        }

        private void OnQuestStateChanged(string questID, QuestState state)
        {
            if (string.IsNullOrEmpty(requiredQuestID))
                return;

            if (requiredQuestID != questID)
                return;

            RefreshInteractionData();

            if (CharacterPlayerController.Instance != null
                && CharacterPlayerController.Instance.InteractionSensor != null)
            {
                CharacterPlayerController.Instance.InteractionSensor.PulseManuallyNextFrame();
            }
        }
    }
}