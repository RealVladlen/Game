using System;
using DG.Tweening;
using UnityEngine;

namespace UIAnimations
{
    [RequireComponent(typeof(CanvasGroup))]
    public class Fader : MonoBehaviour
    {
        public static Fader Instance;
        
        private CanvasGroup _fade;
        private Tween _animation;

        private void Awake()
        {
            if (Instance) Destroy(gameObject);
            else Instance = this;

            _fade = GetComponent<CanvasGroup>();
        }

        public void ShowFade()
        {
            if (_animation != null)
                _animation?.Kill();

            _animation = _fade.DOFade(1, 0.5f).SetUpdate(true).OnComplete(() => { _animation = null; });
        }

        public void ShowFade(Action method)
        {
            if (_animation != null)
                _animation?.Kill();

            _animation = _fade.DOFade(1, 0.5f).SetUpdate(true).OnComplete(() => { method?.Invoke(); _animation = null; });
        }

        public void HideFade(float duration = 1f)
        {
            if (_animation != null)
                _animation?.Kill();

            _animation = _fade.DOFade(0, duration).SetUpdate(true).OnComplete(() => _animation = null);
        }

        public void HideFade(Action method)
        {
            if (_animation != null)
                _animation?.Kill();

            _animation = _fade.DOFade(0, 0.5f).SetUpdate(true).SetDelay(1).OnComplete(() => { method.Invoke(); _animation = null; });
        }

        private void OnDisable()
        {
            _animation?.Kill();
        }
    }
}