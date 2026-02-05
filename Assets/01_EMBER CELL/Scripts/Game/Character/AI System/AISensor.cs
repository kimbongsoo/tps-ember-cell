using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TEC
{
    public class AISensor : MonoBehaviour
    {
        public event System.Action<CharacterBase> OnDetectedCharacter;
        public event System.Action<CharacterBase> OnLostCharacter;

        //TODO : 플레이어 캐릭터를 감지하는 센서 기능 구현.
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (other.transform.TryGetComponent(out CharacterBase character))
                {
                    OnDetectedCharacter?.Invoke(character);   
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (other.transform.TryGetComponent(out CharacterBase character))
                {
                    OnLostCharacter?.Invoke(character);   
                }
            } 
        }
    }
}
