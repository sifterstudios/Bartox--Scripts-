using UnityEngine;

namespace Bartox.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Singleton;

        void Awake()
        {
            if (Singleton == null)
            {
                Singleton = this;
            }
            else if (Singleton != this)
            {
                Destroy(gameObject);
            }
        }
    }
}