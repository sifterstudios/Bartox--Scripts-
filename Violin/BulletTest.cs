using UnityEngine;

namespace Bartox.Violin
{
    public class BulletTest : MonoBehaviour
    {
        public float _speed;
        Rigidbody _rb;

        void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _rb.velocity = transform.forward * _speed;
        }

        void OnCollisionEnter(Collision collision)
        {
            print("Collided!!");
        }
    }
}