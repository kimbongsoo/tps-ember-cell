using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
// using UnityEngine.Animations.Rigging;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

namespace TEC
{
    public class CharacterPlayerController : MonoBehaviour
    {
        public static CharacterPlayerController Instance { get; private set; } = null;

        // public InteractionSensor InteractionSensor => interactionSensor;
        private CharacterBase characterBase;
        // private InteractionSensor interactionSensor;

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


        private void Awake()
        {
            characterBase = GetComponent<CharacterBase>();
            Instance = this;

            GameObject sensorObject = new GameObject("Interaction Sensor");
            sensorObject.transform.SetParent(transform);
            // interactionSensor = sensorObject.AddComponent<InteractionSensor>();
        }

        private void Start()
        {
            // InputManager.Singleton.OnTab += CameraTab;
            // InputManager.Singleton.OnCrouch += ToggleCrouch;
            // InputManager.Singleton.OnReload += ExecuteReload;
            // InputManager.Singleton.OnHolster += ExecuteHolster;
            // InputManager.Singleton.OnPrimaryWeapon += ExecuteEquipPrimaryWeapon;
            // InputManager.Singleton.OnJump += ExecuteJump;
            // InputManager.Singleton.OnRoll += ExecuteRoll;

            // InputManager.Singleton.OnInteract += ExecuteInteract;

            // OnFired(characterBase.primaryWeapon.RemainAmmo, characterBase.primaryWeapon.MaxAmmo);
        }

        private void OnDestroy()
        {
            // InputManager.Singleton.OnTab -= CameraTab;
            // InputManager.Singleton.OnCrouch -= ToggleCrouch;
            // InputManager.Singleton.OnReload -= ExecuteReload;
            // InputManager.Singleton.OnHolster -= ExecuteHolster;
            // InputManager.Singleton.OnPrimaryWeapon -= ExecuteEquipPrimaryWeapon;
            // InputManager.Singleton.OnJump -= ExecuteJump;
            // InputManager.Singleton.OnRoll -= ExecuteRoll;

            // InputManager.Singleton.OnInteract -= ExecuteInteract;
        }

        private void OnEnable()
        {
            characterBase.onFireEvent += OnFired;
            characterBase.onReloadCompleteEvent += OnReloadCompleted;
            characterBase.OnchangedHP += OnChangedHP;
            characterBase.OnChangedSP += OnChangedSP;
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

        private void Update()
        {
            bool isInputRunning = InputManager.Singleton.InputSprint;
            characterBase.IsRunning = isInputRunning;

            bool isAimingInput = InputManager.Singleton.InputAim;
            characterBase.IsAiming = isAimingInput;

            // if (InputManager.Singleton.InputFire)
            // {
            //     characterBase.Fire();
            // }

            characterBase.Move(InputManager.Singleton.InputMove, Camera.main.transform.eulerAngles.y);
            // characterBase.Rotate(CameraSystem.Instance.AimingPoint);

            // characterBase.AimingPoint = CameraSystem.Instance.AimingPoint;

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


        void ToggleCrouch()
        {
            characterBase.IsCrouch = !characterBase.IsCrouch;
        }


    }

}
