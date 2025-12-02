using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace TEC
{
    public class NpcCharacter : CharacterBase
    {
        private void Reset()
        {
            maxHP = 100f;
            // currentHP = maxHP;
            moveSpeed = 3.0f;
        }
    }
}
