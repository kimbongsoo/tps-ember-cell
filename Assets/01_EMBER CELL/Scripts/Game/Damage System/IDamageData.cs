using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TEC
{
    public interface IDamageData
    {
        float DamageAmount { get; }
        GameObject Attacker { get; }
    }
}
