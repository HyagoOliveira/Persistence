using UnityEngine;
using System.Collections;

namespace ActionCode.Persistence
{
    /// <summary>
    /// Abstract component to Show/Hide an UI GameObject when a <see cref="PersistenceSettings"/> event is fired.
    /// </summary>
    public abstract class AbstractUIHandler : MonoBehaviour
    {
        [SerializeField, Tooltip("The Persistence Settings asset.")]
        protected PersistenceSettings settings;
        [SerializeField, Min(0F), Tooltip("The minimum display time to Show/Hide the UI GameObject.")]
        private float minimumDisplayTime = 2F;
        [SerializeField, Tooltip("If enable, it will disable the UI GameObject on Awake.")]
        private bool disbaleOnAwake = true;
        [SerializeField, Tooltip("The UI GameObject to be displayed.")]
        private GameObject uiGameObject;

        private float beginShowTime;

        private void Awake()
        {
            if (disbaleOnAwake) Disable();
        }

        private void OnEnable() => BindActions();
        private void OnDisable() => UnbindActions();

        protected abstract void BindActions();
        protected abstract void UnbindActions();

        protected void HandleStartAction() => Show();
        protected void HandleEndAction() => Hide();

        private void Show()
        {
            beginShowTime = GetTime();
            Enable();
        }

        private void Hide()
        {
            var elapsedTime = GetTime() - beginShowTime;
            var hasMinimumDisplayTime = elapsedTime > minimumDisplayTime;

            if (hasMinimumDisplayTime) Disable();
            else
            {
                var remainingTime = minimumDisplayTime - elapsedTime;

                StopAllCoroutines();
                StartCoroutine(DisableRoutine(remainingTime));
            }
        }

        private void Enable() => uiGameObject.SetActive(true);
        private void Disable() => uiGameObject.SetActive(false);

        private IEnumerator DisableRoutine(float time)
        {
            yield return new WaitForSecondsRealtime(time);
            Disable();
        }

        private static float GetTime() => Time.realtimeSinceStartup;
    }
}