using UnityEngine;

namespace TEC
{
    public class DamageData : IDamageData
    {
        public float Amount { get; private set; }
        public GameObject Attacker { get; private set; }

        public DamageData(float amount, GameObject attacker)
        {
            Amount = amount;
            Attacker = attacker;
        }
    }
}
