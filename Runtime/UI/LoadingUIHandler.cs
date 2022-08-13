using UnityEngine;

namespace ActionCode.Persistence
{
    /// <summary>
    /// Component to Show/Hide an UI GameObject when the loading process is happening.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class LoadingUIHandler : AbstractUIHandler
    {
        protected override void BindActions()
        {
            settings.OnLoadStart += HandleStartAction;
            settings.OnLoadEnd += HandleEndAction;
        }

        protected override void UnbindActions()
        {
            settings.OnLoadStart -= HandleStartAction;
            settings.OnLoadEnd -= HandleEndAction;
        }
    }
}