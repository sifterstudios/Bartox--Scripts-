using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Bartox.UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] GameObject _aimingReticle;
        [SerializeField] GameObject _normalReticle;
        public static UIManager Singleton;
        [SerializeField] float _reticleFadeDuration = .2f;


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

        public void ActivateAim()
        {
            _aimingReticle.SetActive(true);
            _aimingReticle.GetComponent<RawImage>().DOFade(1f, _reticleFadeDuration);
            _normalReticle.SetActive(false);
        }

        public void ActivateNormal()
        {
            _aimingReticle.GetComponent<RawImage>().DOFade(0, _reticleFadeDuration);
            _aimingReticle.SetActive(false);
            _normalReticle.SetActive(true);
        }
    }
}