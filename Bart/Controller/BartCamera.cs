using Cinemachine;
using UnityEngine;

namespace Bartox.Bart.Controller
{
    public class BartCamera : MonoBehaviour
    {
        [SerializeField] CinemachineFreeLook _mainFreeLookCam;
        [SerializeField] CinemachineFreeLook _aimCam;

        [SerializeField] int _defaultPriorityValue = 10;
        [SerializeField] int _defaultDeactivateValue = 4;


        public void ActivateMainFreeLookCam()
        {
            _mainFreeLookCam.m_Priority = _defaultPriorityValue;
            _aimCam.m_Priority = _defaultDeactivateValue;
        }


        public void ActivateAimCam()
        {
            _aimCam.m_Priority = _defaultPriorityValue;
            _mainFreeLookCam.m_Priority = _defaultDeactivateValue;
        }
    }
}