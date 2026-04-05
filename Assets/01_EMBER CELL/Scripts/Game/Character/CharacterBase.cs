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
    public class CharacterBase : MonoBehaviour, IDamageReceiver
    {
        public bool IsPlayerCharacter => isPlayerCharacter;
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

        //추가
        public bool IsDead => isDead;
        private bool isDead = false;
        public WeaponBase PrimaryWeapon => primaryWeapon;

        public bool IsJammed { get; private set; } = false;

        [Header("Character Stat")]
        public float maxHP = 100f;
        public float maxSP = 100f;
        private float currentHP = 100f;
        private float currentSP = 100f;

        [Header("Weapon Setting")]
        public WeaponBase primaryWeaponPrefab;
        private WeaponBase primaryWeapon;

        [Header("Character Setting")]
        public float moveSpeed = 3.0f;
        public float strafeRotationSpeed = 180f;
        private float blendCrouch = 0f;
        private float blendRunning = 0f;

        // 추가
        [Header("Move Speed")]
        public float runSpeedMultiplier = 1.5f;
        public float aimingSpeedMultiplier = 0.6f;
        public float crouchSpeedMultiplier = 0.5f;

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
        public RigBuilder rigBuilder;

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

        [Header("Weapon Jam Setting")]
        [Range(0f, 1f)]
        public float jamChance = 0.001f;   // 5% 기본
        // public float jamChance = 0.50f;
        public float jamCooldown = 0.2f;  // 연속 잼 방지(선택)
        private float lastJamTime = -999f;

        public System.Action<int, int> onFireEvent;
        public System.Action<int, int> onReloadCompleteEvent;
        public System.Action<float, float> OnchangedHP;
        public System.Action<float, float> OnChangedSP;
        //추가
        public System.Action<bool> OnArmedStateChanged;
        public System.Action<bool> OnDeadStateChanged;
        //인디케이터 추가
        public System.Action<Vector3> OnHitAttackerPosition;

        private float _blendAiming = 0f;
        private bool isPlayerCharacter;

        private void Awake()
        {
            characterAnimator = GetComponent<Animator>();
            unityCharacterController = GetComponent<UnityEngine.CharacterController>();
            rigBuilder = GetComponent<RigBuilder>();


            RollingStateMachineBehaviour rollingStateMachine = characterAnimator.GetBehaviour<RollingStateMachineBehaviour>();
            rollingStateMachine.Initialize(this);

            //수정
            UnarmedStateMachineBehaviour unarmedStateMachine = characterAnimator.GetBehaviour<UnarmedStateMachineBehaviour>();
            unarmedStateMachine.Initialize(this);
        }

        private void Start()
        {
        }

        public void Initialize(bool isPlayer = false)
        {
            isPlayerCharacter = isPlayer;
            currentHP = maxHP;
            currentSP = maxSP;

            OnchangedHP?.Invoke(currentHP, maxHP);
            OnChangedSP?.Invoke(currentSP, maxSP);

            primaryWeapon = Instantiate(primaryWeaponPrefab, weaponHolsterPlace);
            primaryWeapon.Owner = this;

            if (primaryWeapon != null)
            {
                PrimaryWeapon.Initialize(this);
                // primaryWeapon.InitializeReserveAmmoToInventory(); 지워
                // 탄약 초기 상태 전달
                onReloadCompleteEvent?.Invoke(primaryWeapon.RemainAmmo, primaryWeapon.MaxAmmo);
            }
        }

        private void Update()
        {
            if (isDead)
                return;

            CheckGround();
            JumpAndGravity();
            FreeFall();

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

            float targetAiming = isAiming && !IsReloading ? 1f : 0f;
            _blendAiming = Mathf.Lerp(_blendAiming, targetAiming, Time.deltaTime * 2f);

            characterAnimator.SetFloat("Crouch", blendCrouch);

            characterAnimator.SetFloat("Aiming", !IsReloading && isAiming ? 1f : 0f);
            characterAnimator.SetFloat("Horizontal", targetHorizontal);
            characterAnimator.SetFloat("Vertical", targetVertical);

            aimingRig.weight = isArmed && !IsReloading && isAiming ? 1f : 0f;
            leftHandIk.weight = isArmed && IsReloading ? 0f : 1f;

            if (isRolling)
            {
                // rollingTime = Time.deltaTime;
                rollingTime += Time.deltaTime;
                float t = rollingTime / rollingDuration;
                float speedRate = rollingCurve.Evaluate(t);
                unityCharacterController.Move(transform.forward * rollingSpeed * speedRate * Time.deltaTime);

            }

        }
        public void Rotate(Vector3 targetPoint)
        {
            Vector3 target = targetPoint;
            target.y = transform.position.y;
            Vector3 viewForward = Camera.main.transform.forward;
            viewForward.y = 0f;
            transform.rotation = Quaternion.LookRotation(Vector3.Lerp(transform.forward, viewForward, Time.deltaTime * 10f));
        }

        public void Move(Vector2 input, float yAxisAngle)
        {
            if (isRolling)
                return;

            characterAnimator.SetFloat("Magnitude", input.magnitude);

            Vector3 movement = Vector3.zero;

            if (input.magnitude > 0f)
            {
                targetHorizontal = input.x;
                targetVertical = input.y;

                // movement = (transform.forward * input.y + transform.right * input.x) 
                //             * moveSpeed * Time.deltaTime;

                // 추가
                float currentSpeed = moveSpeed;

                // 추가
                if (isRunning && currentSP > 0f)
                {
                    currentSpeed *= runSpeedMultiplier;
                }

                // 추가
                if (isAiming)
                {
                    currentSpeed *= aimingSpeedMultiplier;
                }

                // 추가
                if (isCrouch)
                {
                    currentSpeed *= crouchSpeedMultiplier;
                }

                Vector3 dir = transform.forward * input.y + transform.right * input.x;
                if (dir.sqrMagnitude > 1f) dir.Normalize();   // 대각 속도 보정
                // movement = dir * moveSpeed * Time.deltaTime;
                movement = dir * currentSpeed * Time.deltaTime;
            }
            else
            {
                targetHorizontal = 0f;
                targetVertical = 0f;
            }

            // unityCharacterController.Move(movement + new Vector3(0, verticalVelocity, 0));
            unityCharacterController.Move(movement + new Vector3(0, verticalVelocity * Time.deltaTime, 0));

        }

        public void Fire()
        {
            if (IsReloading || !isArmed || IsJammed || IsDead)
                return;

            if (isAiming)
            {
                if (PrimaryWeapon.Shoot(out int remain, out int max))
                {
                    onFireEvent?.Invoke(remain, max);

                    // jam 기능 추가
                    if (Time.time - lastJamTime >= jamCooldown && UnityEngine.Random.value < jamChance)
                    {
                        StartUnjam();
                    }
                }
                else
                {
                    if (PrimaryWeapon.RemainAmmo <= 0 && PrimaryWeapon.MaxAmmo > 0)
                    {
                        Reload();
                    }
                }
            }
        }

        public void Reload()
        {
            if (IsReloading || !isArmed || isRolling)
                return;
            if (PrimaryWeapon != null && PrimaryWeapon.RemainAmmo >= PrimaryWeapon.MaxClipAmmo)
                return;

            IsReloading = true;
            characterAnimator.SetTrigger("Reload Trigger");
            leftHandIk.weight = 0f;
            characterAnimator.SetLayerWeight(2, 0);
        }

        public void ReloadComplete()
        {
            IsReloading = false;
            // int fullAmmo = PrimaryWeapon.SetFullAmmo();
            // onReloadCompleteEvent?.Invoke(fullAmmo, fullAmmo);
            PrimaryWeapon.SetFullAmmo();
            // 🔹 현재 탄창 / 예비탄을 이벤트로 전달
            onReloadCompleteEvent?.Invoke(PrimaryWeapon.RemainAmmo, PrimaryWeapon.MaxAmmo);

            characterAnimator.SetLayerWeight(2, 1);
        }


        public void EquipWeapon()
        {
            characterAnimator.SetTrigger("Equip Trigger");
            characterAnimator.SetFloat("IsEquip", 1f);
            isArmed = true;
            characterAnimator.SetLayerWeight(2, 1f);

            OnArmedStateChanged?.Invoke(true);

        }

        public void HolsterWeapon()
        {
            characterAnimator.SetTrigger("Holster Trigger");
            characterAnimator.SetFloat("IsEquip", 0f);
            isArmed = false;

            OnArmedStateChanged?.Invoke(false);

        }

        public void OnWeaponToEquipPlace()
        {
            PrimaryWeapon.transform.SetParent(weaponEquipPlace);
            PrimaryWeapon.transform.localPosition = Vector3.zero;
            PrimaryWeapon.transform.localRotation = Quaternion.identity;
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
            }
            else
            {
                if (verticalVelocity > -terminalVelocity)
                {
                    verticalVelocity += gravity * Time.deltaTime;
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
                Debug.Log("점프점프점프점프");
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

        public void ReceiveDamage(IDamageData damageData)
        {
            if (damageData == null || currentHP <= 0f || isDead) return;

            //인디케이터 추가
            if (damageData.Attacker != null)
            {
                OnHitAttackerPosition?.Invoke(damageData.Attacker.transform.position);
            }

            currentHP -= damageData.DamageAmount;
            currentHP = Mathf.Clamp(currentHP, 0f, maxHP);
            OnchangedHP?.Invoke(currentHP, maxHP);

            if (currentHP <= 0f)
            {
                // TODO: 사망 처리 로직 (애니메이션, 리스폰 등)
                Dead();
            }
        }

        public void Dead()
        {
            if (isDead) return;
            isDead = true;

            if(isPlayerCharacter)
            {
                StartCoroutine(PlayerDeathSlowMotion());
            }

            if (rigBuilder != null)
                rigBuilder.enabled = false;
            
            if (characterAnimator != null)
                characterAnimator.enabled = false;

            if (unityCharacterController != null)
                unityCharacterController.enabled = false;

            // if (characterAnimator != null)
            // {
            //     characterAnimator.SetTrigger("Dead Trigger");
            //     characterAnimator.SetBool("IsDead", true);
            //     characterAnimator.SetLayerWeight(1, 0f);
            //     characterAnimator.SetLayerWeight(2, 0f);
            // }

            if (PrimaryWeapon != null)
                PrimaryWeapon.gameObject.SetActive(false);

            OnDeadStateChanged?.Invoke(true);
        }

        //테스트용 부활 임시코드
        public void Revive()
        {
            isDead = false;
            currentHP = maxHP;
            currentSP = maxSP;

            if (unityCharacterController != null)
                unityCharacterController.enabled = true;

            if (rigBuilder != null)
                rigBuilder.enabled = true;


            if (characterAnimator != null)
            {
                characterAnimator.enabled = true;

                characterAnimator.Rebind();
                // characterAnimator.Update(0f);

                characterAnimator.SetTrigger("Revive Trigger");
                characterAnimator.SetBool("IsDead", false);
                characterAnimator.SetLayerWeight(1, 1f);
                characterAnimator.SetLayerWeight(2, 1f);
            }

            if (PrimaryWeapon != null)
                PrimaryWeapon.gameObject.SetActive(true);

            OnchangedHP?.Invoke(currentHP, maxHP);
            OnChangedSP?.Invoke(currentSP, maxSP);
            OnDeadStateChanged?.Invoke(false);

            Debug.Log(" 부활이다 ");
        }

        private IEnumerator PlayerDeathSlowMotion()
        {
            float originalTimeScale = Time.timeScale;
            float targetTimeScale = 0.2f;
            float slowDuration = 2f;
            float restoreSpeed = 2f;

            Time.timeScale = targetTimeScale;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;

            yield return new WaitForSecondsRealtime(slowDuration);

            float t = 0f;
            while (t < 1f)
            {
                t += Time.unscaledDeltaTime * restoreSpeed;
                Time.timeScale = Mathf.Lerp(targetTimeScale, originalTimeScale, t);
                Time.fixedDeltaTime = 0.02f * Time.timeScale;
                yield return null;
            }

            Time.timeScale = originalTimeScale;
            Time.fixedDeltaTime = 0.02f;
        }
        
        //인벤토리
        public void HealHP(float amount)
        {
            if (isDead)
                return;

            if (amount <= 0f)
                return;

            currentHP = Mathf.Clamp(currentHP + amount, 0f, maxHP);
            OnchangedHP?.Invoke(currentHP, maxHP);
        }

        public void RecoverSP(float amount) // 추가
        {
            if (isDead)
                return;

            if (amount <= 0f)
                return;

            currentSP = Mathf.Clamp(currentSP + amount, 0f, maxSP);
            OnChangedSP?.Invoke(currentSP, maxSP);
        }

        private void StartUnjam()
        {
            if (IsJammed || IsReloading || isRolling)
                return;

            IsJammed = true;
            lastJamTime = Time.time;

            characterAnimator.SetTrigger("Unjam Trigger");
            characterAnimator.SetLayerWeight(2, 0);

            // OnJamStateChanged?.Invoke(true);
        }

        public void UnjamComplete()
        {
            IsJammed = false;

            characterAnimator.SetLayerWeight(2, 1);

            // OnJamStateChanged?.Invoke(false);
        }
    }
}