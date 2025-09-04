using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;
using UnityEngine.XR;

namespace TEC
{
    public class CharacterBase : MonoBehaviour
    {
        public float CurrentHP => currentHP;
        public float CurrentSP => currentSP;
        public float MaxHP => maxHP;
        public float MaxSP => maxSP;

        public bool IsRunning { get => isRunning; set => isRunning = value; }
        private bool isRunning = false;

        public bool IsCrouch { get => isCrouch; set => isCrouch = value; }
        private bool isCrouch = false;

        public bool IsAiming { get => isAiming; set => isAiming = value; }

        public Vector3 AimingPoint { get => aimingTargetPoint.position; set => aimingTargetPoint.position = value; }

        public bool IsReloading { get; private set; } = false;
        private bool isAiming = false;

        public bool IsArmed { get; private set; } = false;
        private bool isArmed = false;
        public WeaponBase PrimaryWeapon => primaryWeapon;

        [Header("Character Stat")]
        public float maxHP = 1000f;
        public float maxSP = 100f;
        private float currentHP = 1000f;
        private float currentSP = 100f;

        [Header("Weapon Setting")]
        public WeaponBase primaryWeaponPrefab;
        private WeaponBase primaryWeapon;

        [Header("Character Setting")]
        public float moveSpeed = 3.0f;
        public float noneStrafeRotationSpeed = 1f;
        public float strafeRotationSpeed = 180f;
        private float blendCrouch = 0f;
        private float blendRunning = 0f;

        [Header("IK Setting")]
        public Transform aimingTargetPoint;
        public TwoBoneIKConstraint leftHandIk;
        public Rig aimingRig;
        private float targetRotation;
        private float targetHorizontal;
        private float targetVertical;

        [Header("Weapon Holster")]
        public Transform weaponHolsterPlace;
        public Transform weaponEquipPlace;

        [Header("Components")]
        public Animator characterAnimator;
        public UnityEngine.CharacterController unityCharacterController;

        [Header("GroundCheck")]
        public float groundCheckRadius = 0.05f;
        public float groundOffset = 0.1f;
        public LayerMask groundLayer;
        public bool isGrounded;

        [Header("Gravity")]
        public float verticalVelocity;
        public float terminalVelocity = 50f;
        public float gravity = -15f;

        [Header("Jump")]
        public float jumpHeight = 1.2f;
        public float jumpTimeout = 0.3f;
        public float fallTimeout = 0.15f;
        private float jumpTimeoutDelta;
        private float fallTimeoutDelta;

        [Header("Rolling")]
        private bool isRolling = false;
        public float rollingSpeed = 5f;
        public AnimationCurve rollingCurve;
        private float rollingTime = 0f;
        private float rollingDuration = 1.5f;


        public System.Action<int, int> onFireEvent;
        public System.Action<int, int> onReloadCompleteEvent;
        public System.Action<float, float> OnchangedHP; //체력이 바뀔 떄 호출되는 Event
        public System.Action<float, float> OnChangedSP; //스태미너가 바뀔 때 호출되는 Event


        private void Awake()
        {
            characterAnimator = GetComponent<Animator>();
            unityCharacterController = GetComponent<UnityEngine.CharacterController>();
            // RollingStateMachineBehaviour rollingStateMachine = characterAnimator.GetBehaviour<RollingStateMachineBehaviour>();
            // rollingStateMachine.Initialize(this);

            //수정
            UnarmedStateMachineBehaviour unarmedStateMachine = characterAnimator.GetBehaviour<UnarmedStateMachineBehaviour>();
            unarmedStateMachine.Initialize(this);
        }

        private void Start()
        {
            currentHP = maxHP;
            currentSP = maxSP;

            OnchangedHP?.Invoke(currentHP, maxHP);
            OnChangedSP?.Invoke(currentSP, maxSP);

            primaryWeapon = Instantiate(primaryWeaponPrefab, weaponHolsterPlace);
        }

        private void Update()
        {
            FreeFall();
            JumpAndGravity();
            CheckGround();

            if (IsRunning && currentSP > 0f)
            {
                currentSP -= Time.deltaTime;
                OnChangedSP?.Invoke(currentSP, maxSP);
            }
            else
            {
                currentSP += Time.deltaTime;
                OnChangedSP?.Invoke(currentSP, maxSP);
            }
            currentSP = Mathf.Clamp(currentSP, 0f, maxSP);
            if (!IsRunning && CurrentHP < maxHP)
            {
                currentHP += Time.deltaTime;
                OnchangedHP?.Invoke(currentHP, maxHP);
            }
            currentHP = Mathf.Clamp(currentHP, 0f, maxHP);

            float targetBlendRunning = isRunning && currentSP > 0 ? 1f : 0f;
            blendRunning = Mathf.Lerp(blendRunning, targetBlendRunning, Time.deltaTime * 10f);
            characterAnimator.SetFloat("Running", blendRunning);

            float targetBlendCrouch = isCrouch ? 1f : 0f;
            blendCrouch = Mathf.Lerp(blendCrouch, targetBlendCrouch, Time.deltaTime * 10f);
            characterAnimator.SetFloat("Crouch", blendCrouch);

            characterAnimator.SetFloat("Aiming", isAiming ? 1f : 0f);
            characterAnimator.SetFloat("Horizontal", targetHorizontal);
            characterAnimator.SetFloat("Vertical", targetVertical);

            aimingRig.weight = IsArmed && isAiming ? 1f : 0f;
            leftHandIk.weight = IsArmed && IsReloading ? 0f : 1f;

            if (isRolling)
            {
                rollingTime = Time.deltaTime;
                float t = rollingTime / rollingDuration;
                float speedRate = rollingCurve.Evaluate(t);
                unityCharacterController.Move(transform.forward * rollingSpeed * speedRate * Time.deltaTime);

            }

        }


        public void Rotate(Vector3 targetPoint)
        {
            if (isAiming)
            {
                Vector3 target = targetPoint;
                target.y = transform.position.y;
                Vector3 viewForward = Camera.main.transform.forward;
                viewForward.y = 0f;
                transform.rotation = Quaternion.LookRotation(Vector3.Lerp(transform.forward, viewForward, Time.deltaTime * 10f));
            }
        }

        public void Move(Vector2 input, float yAxisAngle)
        {
            if (isRolling)
                return;
            characterAnimator.SetFloat("Magnitude", input.magnitude);
            Vector3 movement = Vector3.zero;
            if (input.magnitude > 0f)
            {
                if (!isAiming)
                {
                    Vector3 inputDirection = new Vector3(input.x, 0f, input.y);
                    targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + yAxisAngle;
                    transform.rotation = Quaternion.Euler(0f, targetRotation, 0f);
                }

                if (isAiming)
                {
                    targetHorizontal = input.x;
                    targetVertical = input.y;
                    movement = (transform.forward * input.y + transform.right * input.x) * moveSpeed * Time.deltaTime;
                }
                else
                {
                    targetVertical = 1f;
                    movement = transform.forward * moveSpeed * Time.deltaTime;
                }
            }
            else
            {
                targetHorizontal = 0f;
                targetVertical = 0f;
            }

            // transform.position += movement;
            unityCharacterController.Move(movement + new Vector3(0, verticalVelocity, 0));
        }
        public void Fire()
        {
            if (IsReloading || !IsArmed)
                return;

            if (isAiming)
            {
                if (PrimaryWeapon.Shoot(out int remain, out int max))
                {
                    onFireEvent?.Invoke(remain, max);
                }
                else
                {
                    if (PrimaryWeapon.IsEmpty())
                    {
                        Reload();
                    }
                }
            }
        }

        public void Reload()
        {
            if (IsReloading || !IsArmed || isRolling)
                return;
            IsReloading = true;
            characterAnimator.SetTrigger("Reload Trigger");
            leftHandIk.weight = 0f;
            characterAnimator.SetLayerWeight(2, 0);
        }

        public void ReloadComplete()
        {
            IsReloading = false;
            int fullAmmo = PrimaryWeapon.SetFullAmmo();
            onReloadCompleteEvent?.Invoke(fullAmmo, fullAmmo);
            characterAnimator.SetLayerWeight(2, 1);
        }


        public void EquipWeapon()
        {
            characterAnimator.SetTrigger("Equip Trigger");
            characterAnimator.SetFloat("IsEquip", 1f);
            IsArmed = true;
            characterAnimator.SetLayerWeight(2, 1f);
        }

        public void HolsterWeapon()
        {
            characterAnimator.SetTrigger("Holster Trigger");
            characterAnimator.SetFloat("IsEquip", 0f);
            IsArmed = false;
        
        }

        public void OnWeaponToEquipPlace()
        {
            PrimaryWeapon.transform.SetParent(weaponEquipPlace);
            PrimaryWeapon.transform.localPosition = Vector3.zero;
            PrimaryWeapon.transform.localRotation = Quaternion.identity;
            // primaryWeapon.transform.localRotation = Quaternion.Euler(0, -90f, 0);
            // primaryWeapon.transform.localRotation = Quaternion.Euler(-6, -162f, -191f);

        }

        public void OnWeaponToHolsterPlace()
        {
            PrimaryWeapon.transform.SetParent(weaponHolsterPlace);
            PrimaryWeapon.transform.localPosition = Vector3.zero;
            PrimaryWeapon.transform.localRotation = Quaternion.identity;
        }


        public void CheckGround()
        {
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - groundOffset, transform.position.z);
            isGrounded = Physics.CheckSphere(spherePosition, groundCheckRadius, groundLayer, QueryTriggerInteraction.Ignore);
            characterAnimator.SetBool("IsGrounded", isGrounded);
        }

        public void JumpAndGravity()
        {
            if (isGrounded)
            {
                if (verticalVelocity < 0f)
                    verticalVelocity = -2f;

                if (jumpTimeoutDelta >= 0f)
                {
                    jumpTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    if (verticalVelocity < terminalVelocity)
                    {
                        verticalVelocity += gravity * Time.deltaTime;
                    }
                }
            }
        }

        public void FreeFall()
        {
            if (isGrounded)
            {
                fallTimeoutDelta = fallTimeout;
                characterAnimator.SetBool("IsFalling", false);
            }
            else
            {
                if (fallTimeoutDelta >= 0f)
                {
                    fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    if (false == characterAnimator.GetBool("IsFalling"))
                    {
                        characterAnimator.SetBool("IsFalling", true);
                    }
                }
            }
        }

        public void Jump()
        {
            if (isGrounded)
            {
                isGrounded = false;
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
                jumpTimeoutDelta = jumpTimeout;
                characterAnimator.SetTrigger("Jump Trigger");
            }
        }

        public void Roll()
        {
            if (isRolling)
                return;

            isRolling = true;
            rollingTime = 0f;
            characterAnimator.SetTrigger("Roll Trigger");
            characterAnimator.SetLayerWeight(1, 0);
            characterAnimator.SetLayerWeight(2, 0);
        }

        public void RollingComplete()
        {
            isRolling = false;
            characterAnimator.SetLayerWeight(1, 1);
            characterAnimator.SetLayerWeight(2, 1);
        }

        public Transform GetAvatarBoneTransform(HumanBodyBones bone)
        {
            return characterAnimator.GetBoneTransform(bone);
        }
    }
}