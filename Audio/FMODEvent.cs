using FMODUnity;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Bartox.Audio
{
    [CreateAssetMenu(menuName = "_Bartox/New Fmod Event Reference", fileName = "NewFMODEvent")]
    public class FMODEvent : ScriptableObject
    {
        [Space] [Title("Fmod Event")] [Required]
        public EventReference FmodEvent;

        [Title("Event Settings")] [Range(0f, 1f)]
        public float _volume = 1f;

        public bool _is3D = true;
    }
}   