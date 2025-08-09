using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TEC
{
    public class CameraSystem : MonoBehaviour
    {
        [field: SerializeField] public Camera MainCamera { get; private set; }

        [field: SerializeField] public Cinemachine.CinemachineVirtualCamera TpsCamera { get; private set; }
        
        
    }
}
