using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TEC
{
    public interface IDamageReceiver
    {
        void ReceiveDamage(IDamageData damageData);
        float CurrentHP { get; }
        float MaxHP { get; }
    }
}
