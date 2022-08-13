using UnityEngine;

namespace ActionCode.Persistence
{
    /// <summary>
    /// Component to Show/Hide an UI GameObject when the saving process is happening.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class SavingUIHandler : AbstractUIHandler
    {
        protected override void BindActions()
        {
            settings.OnSaveStart += HandleStartAction;
            settings.OnSaveEnd += HandleEndAction;
        }

        protected override void UnbindActions()
        {
            settings.OnSaveStart -= HandleStartAction;
            settings.OnSaveEnd -= HandleEndAction;
        }
    }
}