using System.Collections.Generic;
using FMOD;
using FMODUnity;
using Sirenix.OdinInspector;
using UnityEngine;
using FMOD.Studio;
using Debug = UnityEngine.Debug;

namespace Bartox.Audio
{
    public class SFXManager : MonoBehaviour
    {
        static SFXManager _singleton;

        [HorizontalGroup("StudioListener")] [SerializeField]
        StudioEventEmitter _defaultEventEmitter;

        [TabGroup("UI")] [AssetList(Path = "Audio/UI SFX", AutoPopulate = true)]
        public List<FMODEvent> _uiSFX;

        [TabGroup("Ambient")] [AssetList(Path = "Audio/Ambient SFX", AutoPopulate = true)]
        public List<FMODEvent> _ambientSFX;

        [TabGroup("Violin")] [AssetList(Path = "Audio/Violin SFX", AutoPopulate = true)]
        public List<FMODEvent> _violinSFX;

        [TabGroup("Bart")] [AssetList(Path = "Audio/Bart SFX", AutoPopulate = true)]
        public List<FMODEvent> _bartSFX;

        [TabGroup("Interaction")] [AssetList(Path = "Audio/Interaction SFX", AutoPopulate = true)]
        public List<FMODEvent> _interactionSFX;

        public string Test = "test";

        public static SFXManager Singleton
        {
            get
            {
                if (_singleton == null) _singleton = FindObjectOfType<SFXManager>();

                return _singleton;
            }
        }


        public static void PlayEvent(FMODEvent fEvent, bool waitToFinish = true, StudioEventEmitter emitter = null,
            bool isOneShot = false, Vector3 position = default)
        {
            if (emitter == null) emitter = Singleton._defaultEventEmitter;

            if (emitter == null)
            {
                Debug.LogError("You forgot to add a Studio Event Emitter!");
                return;
            }

            if (isOneShot) RuntimeManager.PlayOneShot(fEvent.FmodEvent, position);

            if (!emitter.IsPlaying() || !waitToFinish)

            {
                emitter.EventReference = fEvent.FmodEvent;
                emitter.Play();
            }
        }

        [HorizontalGroup("AudioSource")]
        [ShowIf("@_defaultEventEmitter == null")]
        [GUIColor(1f, 0.5f, 0.5f, 1f)]
        [Button]
        void AddEventEmitter()
        {
            _defaultEventEmitter = this.gameObject.GetComponent<StudioEventEmitter>();

            if (_defaultEventEmitter == null)
            {
                _defaultEventEmitter = this.gameObject.AddComponent<StudioEventEmitter>();
            }
        }

        public enum SFXType
        {
            UI,
            Ambient,
            Violin,
            Bart,
            Interaction,
        }

        public static void PlaySoundWithLabeledParameter(EventReference eventRef, PARAMETER_ID paramID, string
            label, Vector3 position)
        {
            RuntimeManager.PlayOneShotLabeledParam(eventRef, paramID, label, position);
            print($"Played {eventRef.ToString()} with parameter {paramID} set to {label}");
        }

        public static PARAMETER_ID GetLocalParameterID(GUID eventGuid, string paramName)
        {
            PARAMETER_DESCRIPTION pd;
            var eventDescription = RuntimeManager.GetEventDescription(eventGuid);
            eventDescription.getParameterDescriptionByName(paramName, out pd);
            return pd.id;
        }

        public static PARAMETER_ID GetGlobalParameterID(string paramName)
        {
            PARAMETER_DESCRIPTION pd;
            RuntimeManager.StudioSystem.getParameterDescriptionByName(paramName, out pd);
            return pd.id;
        }
    }
}