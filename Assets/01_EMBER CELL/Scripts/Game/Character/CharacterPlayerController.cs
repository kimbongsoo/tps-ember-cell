using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

namespace TEC
{
    public class CharacterPlayerController : MonoBehaviour
    {
        public static CharacterPlayerController Instance { get; private set; } = null;

        public InteractionSensor InteractionSensor => interactionSensor;
        private CharacterBase characterBase;
        private InteractionSensor interactionSensor;
        private PickupItemInteractor pickupItemInteractor;


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




        private void Awake()
        {
            characterBase = GetComponent<CharacterBase>();
            pickupItemInteractor = GetComponent<PickupItemInteractor>();
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
            InputManager.Singleton.OnPickup += ExecutePickup;

            //스코프
            // InputManager.Singleton.OnRightClickDouble += ToggleRedDotUI;
            InputManager.Singleton.OnRightClickDouble += OnRightClickDouble;


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
            InputManager.Singleton.OnPickup -= ExecutePickup;

            //스코프
            // InputManager.Singleton.OnRightClickDouble -= ToggleRedDotUI;
            InputManager.Singleton.OnRightClickDouble -= OnRightClickDouble;



        }

        private void OnEnable()
        {
            characterBase.onFireEvent += OnFired;
            characterBase.onReloadCompleteEvent += OnReloadCompleted;
            characterBase.OnchangedHP += OnChangedHP;
            characterBase.OnChangedSP += OnChangedSP;
            characterBase.OnArmedStateChanged += OnLinkedCharacterArmedChanged;
            characterBase.OnDeadStateChanged += OnLinkedCharacterDeadState;

            // characterBase.OnArmedStateChanged += OnArmedStateChanged;
            if (UserDataModel.Singleton != null)
                UserDataModel.Singleton.OnInventoryChanged += OnInventoryChanged;
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

            // characterBase.OnArmedStateChanged -= OnArmedStateChanged;
            if (UserDataModel.Singleton != null)
                UserDataModel.Singleton.OnInventoryChanged -= OnInventoryChanged;
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

            bool isInputRunning = InputManager.Singleton.InputSprint;
            characterBase.IsRunning = isInputRunning;

            bool isAimingInput = InputManager.Singleton.InputAim;
            characterBase.IsAiming = isAimingInput;
            // 스코프 해제 
            if (!InputManager.Singleton.InputAim && isScoped)
            {
                ExitScopeMode();
            }

            if (InputManager.Singleton.InputFire)
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
            CameraSystem.Instance.SetChangeCameraSide();
        }


        void ToggleCrouch()
        {
            characterBase.IsCrouch = !characterBase.IsCrouch;
        }

        void ExecuteReload()
        {
            characterBase.Reload();
        }


        void ExecuteHolster()
        {
            characterBase.HolsterWeapon();
        }

        void ExecuteEquipPrimaryWeapon()
        {
            characterBase.EquipWeapon();
        }

        void ExecuteJump()
        {
            characterBase.Jump();
        }

        void ExecuteRoll()
        {
            characterBase.Roll();
        }

        void ExecuteInteract()
        {
            var interactionUI = UIManager.Singleton.GetUI<InteractionUI>(UIList.InteractionUI);
            interactionUI.TryInteract();
        }
        
        //스코프
        private void OnRightClickDouble()
        {
            // 우클릭 더블탭 직후, 버튼이 눌린 상태면 스코프 진입
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

            Debug.Log("🎯 EnterScopeMode()");
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
            var inventoryUI = UIManager.Singleton.GetUI<InventoryRenewalUI>(UIList.InventoryRenewalUI);
            if (inventoryUI.gameObject.activeSelf)
            {
                UIManager.Hide<InventoryRenewalUI>(UIList.InventoryRenewalUI);    
            }
            else
            {
                UIManager.Show<InventoryRenewalUI>(UIList.InventoryRenewalUI);
            }
            
        }

        private void ExecutePickup()
        {
            if (characterBase == null || characterBase.IsDead)
                return;

            pickupItemInteractor?.TryPickupNearestDropItem();
        }

        private void OnInventoryChanged()
        {
            if (characterBase == null || characterBase.PrimaryWeapon == null)
                return;
            MainHUD.Instance.SetAmmoText(characterBase.PrimaryWeapon.RemainAmmo, characterBase.PrimaryWeapon.MaxAmmo);
        }


    }

}