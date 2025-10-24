using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TEC
{
    public interface IInteractionProvider
    {
        public List<IInteractionData> Interactions { get; }

        public void Interact(IInteractionData data);
    }
}
