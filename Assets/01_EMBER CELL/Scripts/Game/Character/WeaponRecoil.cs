using Cinemachine;
using UnityEngine;

namespace TEC
{
    [RequireComponent(typeof(CinemachineImpulseSource))]
    public class WeaponRecoil : MonoBehaviour
    {
        private CinemachineImpulseSource impulseSource;

        private void Awake()
        {
            impulseSource = GetComponent<CinemachineImpulseSource>();
        }

        public void GenerateRecoil()
        {
            if (impulseSource != null)
            {
                impulseSource.GenerateImpulse();
            }
        }
    }
}
