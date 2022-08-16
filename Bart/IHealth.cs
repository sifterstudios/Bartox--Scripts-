using UnityEngine;

namespace Bartox.Player
{
    public interface IIHealth
    {
        void TakeDamage(int damageAmount, Vector3 hitDirection = default);
    }
}