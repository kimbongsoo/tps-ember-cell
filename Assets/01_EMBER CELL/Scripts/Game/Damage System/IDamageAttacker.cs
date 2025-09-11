using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TEC
{
    public interface IDamageAttacker
    {
        void AttackDamage(IDamageReceiver target);
    }
}
