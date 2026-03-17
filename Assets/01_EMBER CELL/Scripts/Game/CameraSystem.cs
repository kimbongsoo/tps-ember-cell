using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Cinemachine;
using UnityEngine;

namespace TEC
{
    public class CameraSystem : MonoBehaviour
    {
        public static CameraSystem Instance { get; private set; }
        public Vector3 AimingPoint => cameraAimingPoint;
        [field: SerializeField] public Camera MainCamera { get; private set; }

        [field: SerializeField] public Cinemachine.CinemachineVirtualCamera TpsCamera { get; private set; }

        [field: SerializeField] public LayerMask LayerMask { get; private set; }

        private Cinemachine.Cinemachine3rdPersonFollow tpsCamera3rdFollow;
        private bool isCameraSideRight = true;
        private float cameraSideBlend = 0f;

        private Vector3 cameraAimingPoint;

        //스코프
        [SerializeField] private GameObject scopeCamera;
        [SerializeField] private float tpsFov = 60f;
        [SerializeField] private float scopeFov = 40f;
        [SerializeField] private float fovBlendTime = 0.12f;
        private Coroutine fovCoroutine;

        //npc
        [SerializeField] private CinemachineVirtualCamera dialogueCamera;
        [SerializeField] private int tpsPriority = 10;
        [SerializeField] private int dialoguePriority = 20;


        private void Awake()
        {
            Instance = this;
            tpsCamera3rdFollow = TpsCamera.GetCinemachineComponent<Cinemachine.Cinemachine3rdPersonFollow>();
            Debug.Log("TPS Camera follow: " + tpsCamera3rdFollow); //디버깅용
            cameraSideBlend = isCameraSideRight ? 1 : 0;

            // Dialogue Camera 초기 설정
            if (TpsCamera != null)
                TpsCamera.Priority = tpsPriority;

            if (dialogueCamera != null)
                dialogueCamera.Priority = 0;
            
            //스코프
            if (scopeCamera != null)
                scopeCamera.SetActive(false);
        }

        private void Update()
        {
            Ray ray = MainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            if (Physics.Raycast(ray, out RaycastHit hitInfo, 1000f, LayerMask, QueryTriggerInteraction.Ignore))
            {
                cameraAimingPoint = hitInfo.point;
                //추가
                // if (Input.GetMouseButtonDown(0) && hitInfo.collider.CompareTag("Capsule"))
                // {
                //     Debug.Log("hitInfo :" + hitInfo);
                //     Transform capsule = hitInfo.collider.transform;
                //     capsule.localScale *= 1.1f;
                // }
            }
            else
            {
                cameraAimingPoint = ray.GetPoint(1000f);
            }

            tpsCamera3rdFollow.CameraSide = Mathf.Lerp(tpsCamera3rdFollow.CameraSide, cameraSideBlend, Time.deltaTime * 10f);

        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(MainCamera.transform.position, cameraAimingPoint);
        }

        public void SetChangeCameraSide(bool isRight)
        {
            isCameraSideRight = isRight;
            cameraSideBlend = isCameraSideRight ? 1 : 0;
        }

        public void SetChangeCameraSide()
        {
            isCameraSideRight = !isCameraSideRight;
            cameraSideBlend = isCameraSideRight ? 1 : 0;

        }

        //스코프
        public void EnterScopeMode()
        {
            if (scopeCamera != null) scopeCamera.SetActive(true);
            StartFovLerp(Camera.main.fieldOfView, scopeFov, fovBlendTime);
        }

        public void ExitScopeMode()
        {
            if (scopeCamera != null) scopeCamera.SetActive(false);
            StartFovLerp(Camera.main.fieldOfView, tpsFov, fovBlendTime);
        }

        //추가
        public void EnterDialogueMode(Transform followTarget, Transform lookTarget)
        {
            if (dialogueCamera == null)
                return;

            dialogueCamera.Follow = followTarget;
            dialogueCamera.LookAt = lookTarget;
            dialogueCamera.Priority = dialoguePriority;
        }

        //추가
        public void ExitDialogueMode()
        {
            if (dialogueCamera == null)
                return;

            dialogueCamera.Priority = 0;
            dialogueCamera.Follow = null;
            dialogueCamera.LookAt = null;

            if (TpsCamera != null)
                TpsCamera.Priority = tpsPriority;
        }

        private void StartFovLerp(float from, float to, float duration)
        {
            if (fovCoroutine != null) StopCoroutine(fovCoroutine);
            fovCoroutine = StartCoroutine(FovLerp(from, to, duration));
        }

        private IEnumerator FovLerp(float from, float to, float duration)
        {
            float time = 0f;
            while (time < duration)
            {
                time += Time.deltaTime;
                Camera.main.fieldOfView = Mathf.Lerp(from, to, time / duration);
                yield return null;
            }
            Camera.main.fieldOfView = to;
        }

    }
    

}

