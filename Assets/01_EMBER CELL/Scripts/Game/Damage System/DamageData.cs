using UnityEngine;

namespace TEC
{
    public class DamageData : IDamageData
    {
        public float DamageAmount { get; private set; }
        public GameObject Attacker { get; private set; }

        public DamageData(float damageAmount, GameObject attacker)
        {
            DamageAmount = damageAmount;
            Attacker = attacker;
        }
    }
}
