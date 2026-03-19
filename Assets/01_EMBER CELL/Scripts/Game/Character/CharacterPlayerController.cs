using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TEC
{
    public class CharacterPlayerController : MonoBehaviour
    {
        public static CharacterPlayerController Instance { get; private set; } = null;

        public InteractionSensor InteractionSensor => interactionSensor;
        private CharacterBase characterBase;
        private InteractionSensor interactionSensor;


        [Header("Camera Setting")]
        public Transform cameraPivot;
        public float bottomClampLimit = -80f;
        public float topClampLimit = 80f;
        private float threshold = 0.01f;
        private float targetYaw = 0f;
        private float targetPitch = 0f;

        [Header("Corsshair Setting")]
        public float crosshairSpreadSpeed = 0.1f;
        public float crosshairRecoverySpeed = 0.2f;
        public float crosshairSpreadMax = 1f;
        public float crosshairSpreadMin = 0.1f;
        private float crosshairCurrentSpread = 0f;

        [Header("Camera Recoil Setting")]
        public float recoilRecoverySpeed = 2f;
        private Vector3 targetRotation;
        private Vector3 currentRotation;

        // 1. 스코프 필드 추가
        private bool isScoped = false;

        //추가
        private bool wasDialogueUIOpen = false;
        private bool isSequenceControl = false;
        public void SetSequenceControl(bool active)
        {
            isSequenceControl = active;
        }

        // 추가 인벤 열려있을 때 체크
        private bool IsInventoryUIOpen()
        {
            var inventoryUI = InventoryRenewalUI.Instance;
            return inventoryUI != null && inventoryUI.gameObject.activeSelf;
        }
        private bool IsContextMenuOpen()
        {
            var menu = InventoryContextMenu.Instance;
            return menu != null && menu.gameObject.activeSelf;
        }
        // 대화 창 오픈 여부
        private bool IsDialogueUIOpen()
        {
            return DialogueUI.IsDialogueOpen
                || QuestAcceptUI.IsQuestAcceptOpen
                || NPCDialogueProvider.IsConversationSequenceOpen
                || isSequenceControl;
        }   

        private void Awake()
        {
            characterBase = GetComponent<CharacterBase>();
            Instance = this;

            GameObject sensorObject = new GameObject("Interaction Sensor");
            sensorObject.transform.SetParent(transform);
            interactionSensor = sensorObject.AddComponent<InteractionSensor>();
        }

        private void Start()
        {
            InputManager.Singleton.OnTab += CameraTab;
            InputManager.Singleton.OnCrouch += ToggleCrouch;
            InputManager.Singleton.OnReload += ExecuteReload;
            InputManager.Singleton.OnHolster += ExecuteHolster;
            InputManager.Singleton.OnPrimaryWeapon += ExecuteEquipPrimaryWeapon;
            InputManager.Singleton.OnJump += ExecuteJump;
            InputManager.Singleton.OnRoll += ExecuteRoll;

            InputManager.Singleton.OnInteract += ExecuteInteract;
            InputManager.Singleton.OnInventory += ExecuteInventory;
            InputManager.Singleton.OnActionUI += ExecuteActionUI;

            //스코프
            // InputManager.Singleton.OnRightClickDouble += ToggleRedDotUI;
            InputManager.Singleton.OnRightClickDouble += OnRightClickDouble;

            InputManager.Singleton.OnQuickSlot1 += ExecuteQuickSlot1;
            InputManager.Singleton.OnQuickSlot2 += ExecuteQuickSlot2;

            characterBase.Initialize(true);


            // OnFired(characterBase.PrimaryWeapon.RemainAmmo, characterBase.PrimaryWeapon.MaxAmmo);
        }

        void OnLinkedCharacterArmedChanged(bool isArmed)
        {
            // var crossHairUI = UIManager.Singleton.GetUI<CrossHairUI>(UIList.CrossHairUI);
            // var mainHudUI = UIManager.Singleton.GetUI<MainHUD>(UIList.MainHUD);

            CrossHairUI.Instance.ToggleCrosshairByArmedState(isArmed);
            MainHUD.Instance.ToggleAmmoTextByArmedState(isArmed);
        }

        void OnLinkedCharacterDeadState(bool isDead)
        {
            CrossHairUI.Instance.ToggleCrosshairByDeadState(isDead);
            MainHUD.Instance.ToggleAmmoTextByDeadState(isDead);
        }

        private void OnDestroy()
        {
            InputManager.Singleton.OnTab -= CameraTab;
            InputManager.Singleton.OnCrouch -= ToggleCrouch;
            InputManager.Singleton.OnReload -= ExecuteReload;
            InputManager.Singleton.OnHolster -= ExecuteHolster;
            InputManager.Singleton.OnPrimaryWeapon -= ExecuteEquipPrimaryWeapon;
            InputManager.Singleton.OnJump -= ExecuteJump;
            InputManager.Singleton.OnRoll -= ExecuteRoll;

            InputManager.Singleton.OnInteract -= ExecuteInteract;
            InputManager.Singleton.OnActionUI -= ExecuteActionUI;

            //스코프
            // InputManager.Singleton.OnRightClickDouble -= ToggleRedDotUI;
            InputManager.Singleton.OnRightClickDouble -= OnRightClickDouble;

            InputManager.Singleton.OnQuickSlot1 -= ExecuteQuickSlot1;
            InputManager.Singleton.OnQuickSlot2 -= ExecuteQuickSlot2;



        }

        private void OnEnable()
        {
            characterBase.onFireEvent += OnFired;
            characterBase.onReloadCompleteEvent += OnReloadCompleted;
            characterBase.OnchangedHP += OnChangedHP;
            characterBase.OnChangedSP += OnChangedSP;
            characterBase.OnArmedStateChanged += OnLinkedCharacterArmedChanged;
            characterBase.OnDeadStateChanged += OnLinkedCharacterDeadState;
            //인디케이터 추가
            characterBase.OnHitAttackerPosition += OnHitAttackerPosition;

            // characterBase.OnArmedStateChanged += OnArmedStateChanged;
            if (UserDataModel.Singleton != null)
            {
                UserDataModel.Singleton.OnInventoryChanged += OnInventoryChanged;
                UserDataModel.Singleton.OnItemEffectRequested += OnItemEffectRequested; //추가

                UserDataModel.Singleton.OnQuickSlotChanged += OnQuickSlotChanged;
            }
        }

        private void OnDisable()
        {
            if (characterBase == null) return;

            characterBase.onFireEvent -= OnFired;
            characterBase.onReloadCompleteEvent -= OnReloadCompleted;
            characterBase.OnchangedHP -= OnChangedHP;
            characterBase.OnChangedSP -= OnChangedSP;
            characterBase.OnArmedStateChanged -= OnLinkedCharacterArmedChanged;
            characterBase.OnDeadStateChanged -= OnLinkedCharacterDeadState;
            //인디케이터 추가
            characterBase.OnHitAttackerPosition -= OnHitAttackerPosition;

            // characterBase.OnArmedStateChanged -= OnArmedStateChanged;
            if (UserDataModel.Singleton != null)
            {
                UserDataModel.Singleton.OnInventoryChanged -= OnInventoryChanged;
                UserDataModel.Singleton.OnItemEffectRequested -= OnItemEffectRequested; //추가

                UserDataModel.Singleton.OnQuickSlotChanged -= OnQuickSlotChanged;
            }
        }

        private void OnReloadCompleted(int current, int max)
        {
            MainHUD.Instance.SetAmmoText(current, max);
        }

        private void OnChangedHP(float current, float max)
        {
            MainHUD.Instance.SetHP(current, max);
        }
        private void OnChangedSP(float current, float max)
        {
            MainHUD.Instance.SetSP(current, max);
        }
        private void OnFired(int current, int max)
        {
            MainHUD.Instance.SetAmmoText(current, max);

            crosshairCurrentSpread = Mathf.Clamp(crosshairCurrentSpread + crosshairSpreadSpeed, crosshairSpreadMin, crosshairSpreadMax);
            CrossHairUI.Instance.SetCrosshairSpread(crosshairCurrentSpread / crosshairSpreadMax);
        }
        private void MinimapRotation()
        {
            float platerYaw = characterBase.transform.eulerAngles.y;
            MainHUD.Instance.UpdateCompass(platerYaw);
        }
        // private void ToggleRedDotUI()
        // {
        //     RedDotUI.Instance.Toggle();
        // }

        private void Update()
        {
            //추가
            // if (characterBase.IsDead) return;
            //부활 임시코드
            if (characterBase.IsDead)
            {
                if (Input.GetKeyDown(KeyCode.P))
                    characterBase.Revive();
                return;
            }
            
            if (isSequenceControl)
            {
                characterBase.IsRunning = false;
                characterBase.IsAiming = false;

                if (isScoped)
                    ExitScopeMode();

                crosshairCurrentSpread = Mathf.Clamp(
                    crosshairCurrentSpread - (crosshairRecoverySpeed * Time.deltaTime),
                    crosshairSpreadMin,
                    crosshairSpreadMax);

                CrossHairUI.Instance.SetCrosshairSpread(crosshairCurrentSpread / crosshairSpreadMax);
                return;
            }
            
            bool isInventoryOpen = IsInventoryUIOpen();

            if (isInventoryOpen)
            {
                characterBase.IsAiming = false;

                if (isScoped)
                    ExitScopeMode();

                if (Input.GetMouseButtonDown(1) && IsContextMenuOpen())
                {
                    InventoryContextMenu.Instance.Hide();
                }
            }

            bool isDialogueOpen = IsDialogueUIOpen();

            if (isDialogueOpen)
            {
                InputManager.Singleton.SetCursorForcedByUI(true, true);
            }
            
            if (wasDialogueUIOpen != isDialogueOpen)
            {
                var interactionUI = UIManager.Singleton.GetUI<InteractionUI>(UIList.InteractionUI);
                if (interactionUI != null)
                {
                    if (isDialogueOpen)
                    {
                        interactionUI.ClearData();
                        interactionUI.gameObject.SetActive(false);
                    }
                    else
                    {
                        interactionSensor.PulseManuallyNextFrame();
                    }
                }

                if (MainHUD.Instance != null)
                    MainHUD.Instance.SetDialogueMode(isDialogueOpen);

                if (isDialogueOpen)
                {
                    InputManager.Singleton.SetCursorForcedByUI(true, true);
                }
                else
                {
                    InputManager.Singleton.SetCursorForcedByUI(false, false);
                    InputManager.Singleton.SetCursorVisible(false);
                }

                wasDialogueUIOpen = isDialogueOpen;
            }
            //여기까지 추가

            if (isDialogueOpen)
            {
                characterBase.IsRunning = false;
                characterBase.IsAiming = false;

                if (isScoped)
                    ExitScopeMode();

                characterBase.Move(Vector2.zero, Camera.main.transform.eulerAngles.y);

                crosshairCurrentSpread = Mathf.Clamp(
                    crosshairCurrentSpread - (crosshairRecoverySpeed * Time.deltaTime),
                    crosshairSpreadMin,
                    crosshairSpreadMax);

                CrossHairUI.Instance.SetCrosshairSpread(crosshairCurrentSpread / crosshairSpreadMax);
                return;
            }


            bool isInputRunning = InputManager.Singleton.InputSprint;
            characterBase.IsRunning = isInputRunning;

            bool isAimingInput = !isInventoryOpen && InputManager.Singleton.InputAim;
            characterBase.IsAiming = isAimingInput;
            // 스코프 해제 
            if (!InputManager.Singleton.InputAim && isScoped)
            {
                ExitScopeMode();
            }

            if (!isInventoryOpen && InputManager.Singleton.InputFire)
            {
                characterBase.Fire();
            }

            characterBase.Move(InputManager.Singleton.InputMove, Camera.main.transform.eulerAngles.y);
            characterBase.Rotate(CameraSystem.Instance.AimingPoint);

            characterBase.AimingPoint = CameraSystem.Instance.AimingPoint;

            //크로스헤어 줄어들기
            
            crosshairCurrentSpread = Mathf.Clamp(
                crosshairCurrentSpread - (crosshairRecoverySpeed * Time.deltaTime)
                , crosshairSpreadMin
                , crosshairSpreadMax);

            CrossHairUI.Instance.SetCrosshairSpread(crosshairCurrentSpread / crosshairSpreadMax);


        }


        private void LateUpdate()
        {
            if (characterBase == null || characterBase.IsDead)
                return;

            if (IsInventoryUIOpen() || IsDialogueUIOpen())
                return;

            CameraRotation();
            CameraRecovery();
            MinimapRotation();
        }

        public void CameraRotation()
        {
            Vector2 look = InputManager.Singleton.InputLook;

            if (look.sqrMagnitude > threshold)
            {
                float yaw = look.x;
                float pitch = -look.y;

                targetYaw += yaw;
                targetPitch += pitch;
            }

            targetYaw = ClampAngle(targetYaw, float.MinValue, float.MaxValue);
            targetPitch = ClampAngle(targetPitch, bottomClampLimit, topClampLimit);

            cameraPivot.rotation = Quaternion.Euler(targetPitch + currentRotation.x, targetYaw + currentRotation.y, 0f);

        }

        public void CameraRecoil(float recoilAmount, float vertical = 2f, float horizontal = 1f)
        {
            float xRecoil = -vertical * recoilAmount;
            float yRecoil = UnityEngine.Random.Range(-horizontal, horizontal) * recoilAmount;
            targetRotation += new Vector3(xRecoil, yRecoil, 0f);
        }

        public void CameraRecovery()
        {
            targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, Time.deltaTime * recoilRecoverySpeed);
            currentRotation = Vector3.Lerp(currentRotation, targetRotation, Time.deltaTime * recoilRecoverySpeed);
        }

        private static float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360f)
            {
                angle += 360f;
            }
            if (angle > 360f)
            {
                angle -= 360f;
            }

            return Mathf.Clamp(angle, min, max);
        }

        void CameraTab()
        {
            if (IsDialogueUIOpen())
                return;
            CameraSystem.Instance.SetChangeCameraSide();
        }


        void ToggleCrouch()
        {
            if (IsDialogueUIOpen())
                return;

            if (IsDialogueUIOpen())
                return;

            characterBase.IsCrouch = !characterBase.IsCrouch;
        }

        void ExecuteReload()
        {
            if (IsDialogueUIOpen())
                return;

            characterBase.Reload();
        }


        void ExecuteHolster()
        {
            if (IsDialogueUIOpen())
                return;

            characterBase.HolsterWeapon();
        }

        void ExecuteEquipPrimaryWeapon()
        {
            if (IsDialogueUIOpen())
                return;

            characterBase.EquipWeapon();
        }

        void ExecuteJump()
        {
            if (IsDialogueUIOpen())
                return;

            characterBase.Jump();
        }

        void ExecuteRoll()
        {
            if (IsDialogueUIOpen())
                return;

            characterBase.Roll();
        }

        void ExecuteInteract()
        {
            if (IsInventoryUIOpen() || IsDialogueUIOpen())
                return;

            var interactionUI = UIManager.Singleton.GetUI<InteractionUI>(UIList.InteractionUI);
            interactionUI.TryInteract();
        }
        
        //스코프
        private void OnRightClickDouble()
        {
            if (IsDialogueUIOpen())
                return;

            // 우클릭 더블탭 직후, 버튼이 눌린 상태면 스코프 진입
            if (IsInventoryUIOpen())
                return;

            if (InputManager.Singleton.InputAim && !isScoped)
            {
                EnterScopeMode();
            }
        }

        private void EnterScopeMode()
        {
            isScoped = true;

            // 카메라 전환
            CameraSystem.Instance.EnterScopeMode();

            // UI 전환
            CrossHairUI.Instance?.gameObject.SetActive(false);
            RedDotUI.Instance?.gameObject.SetActive(true);
        }

        private void ExitScopeMode()
        {
            isScoped = false;

            // 카메라 복귀
            CameraSystem.Instance.ExitScopeMode();

            // UI 복귀
            CrossHairUI.Instance?.gameObject.SetActive(true);
            RedDotUI.Instance?.gameObject.SetActive(false);

            Debug.Log("⬅ ExitScopeMode()");
        }

        void ExecuteInventory()
        {
            if (IsDialogueUIOpen())
                return;

            var inventoryUI = InventoryRenewalUI.Instance;
            if (inventoryUI.gameObject.activeSelf)
            {
                if (IsContextMenuOpen())
                    InventoryContextMenu.Instance.Hide();

                UIManager.Hide<InventoryRenewalUI>(UIList.InventoryRenewalUI);    
            }
            else
            {
                UIManager.Show<InventoryRenewalUI>(UIList.InventoryRenewalUI);
            }
            
        }

        private void OnInventoryChanged()
        {
            if (characterBase == null || characterBase.PrimaryWeapon == null)
                return;
            MainHUD.Instance.SetAmmoText(characterBase.PrimaryWeapon.RemainAmmo, characterBase.PrimaryWeapon.MaxAmmo);

            MainHUD.Instance.RefreshQuickSlots();

            // 인벤 UI가 열려있으면 리스트 갱신
            var inventoryUI = InventoryRenewalUI.Instance;
            if (inventoryUI != null && inventoryUI.gameObject.activeSelf)
            {
                inventoryUI.Refresh();
            }
        }
        private void OnQuickSlotChanged()
        {
            MainHUD.Instance.RefreshQuickSlots();
        }

        private void ExecuteActionUI()
        {
            if (IsDialogueUIOpen())
                return;

            MainHUD.Instance.ToggleActionUI();
        }

        private void OnHitAttackerPosition(Vector3 attackerPosition)
        {
            MainHUD.Instance.ShowHitDirection(transform, attackerPosition);
        }

        private void OnItemEffectRequested(ItemUseEffectType effectType, float value) // [CHANGED]
        {
            if (characterBase == null || characterBase.IsDead)
                return;

            switch (effectType)
            {
                case ItemUseEffectType.HealHP:
                    characterBase.HealHP(value);
                    break;

                case ItemUseEffectType.RecoverSP:
                    characterBase.RecoverSP(value);
                    break;
            }
        }

        private void ExecuteQuickSlot1()
        {
            if (IsDialogueUIOpen())
                return;

            if (UserDataModel.Singleton == null)
                return;

            UserDataModel.Singleton.TryUseQuickSlot(0);
        }

        private void ExecuteQuickSlot2()
        {
            if (IsDialogueUIOpen())
                return;

            if (UserDataModel.Singleton == null)
                return;

            UserDataModel.Singleton.TryUseQuickSlot(1);
        }

        public void RequestExit()
        {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
        }


    }

}