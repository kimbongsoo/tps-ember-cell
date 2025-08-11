using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Cinemachine;
using UnityEngine;

namespace TEC
{
    public class CameraSystem : MonoBehaviour
    {
        public static CameraSystem Instance {get; private set;}
        public Vector3 AimingPoint => cameraAimingPoint;
        [field: SerializeField] public Camera MainCamera {get; private set;}

        [field: SerializeField] public Cinemachine.CinemachineVirtualCamera TpsCamera {get; private set;}

        [field: SerializeField] public LayerMask LayerMask {get; private set;}

        private Cinemachine.Cinemachine3rdPersonFollow tpsCamera3rdFollow;
        private bool isCameraSideRight = true;
        private float cameraSideBlend = 0f;

        private Vector3 cameraAimingPoint;

        private void Awake()
        {
            Instance = this;
            tpsCamera3rdFollow = TpsCamera.GetCinemachineComponent<Cinemachine.Cinemachine3rdPersonFollow>();
            cameraSideBlend = isCameraSideRight ? 1 : 0;
        }

        private void Update()
        {
            Ray ray = MainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            if (Physics.Raycast(ray, out RaycastHit hitInfo, 1000f, LayerMask , QueryTriggerInteraction.Ignore))
            {
                cameraAimingPoint = hitInfo.point;
                //추가
                if (Input.GetMouseButtonDown(0) && hitInfo.collider.CompareTag("Capsule"))
                {
                    Debug.Log("hitInfo :" + hitInfo);
                    Transform capsule = hitInfo.collider.transform;
                    capsule.localScale *= 1.1f;
                }
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
    }

}

