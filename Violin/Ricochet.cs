using UnityEngine;

namespace Bartox.Violin
{
    public class Ricochet : MonoBehaviour
    {
        public int MaxReflectionCount = 3;

        int _currentSpentReflections;

        void OnCollisionEnter(Collision collision)
        {
            print("WE COLLIDED");
            print("Name of collider: " + collision.gameObject.name);
            Vector3 TargetDirection = Vector3.Reflect(transform.forward, collision.contacts[0].normal);
        }
    }
}