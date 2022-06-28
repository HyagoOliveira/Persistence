using UnityEditor;
using ActionCode.ScriptableSettingsProvider.Editor;

namespace ActionCode.Persistence.Editor
{
    /// <summary>
    /// Asset Provider for <see cref="PersistenceSettings"/> asset handling its asset creation.
    /// <para>Creates the ActionCode/Persistence menu in the Project Settings windows.</para>
    /// </summary>
    public sealed class PersistenceSettingsProvider :
        AbstractScriptableSettingsProvider<PersistenceSettings>
    {
        /// <summary>
        /// Creates the Project Settings menu.
        /// </summary>
        public PersistenceSettingsProvider()
            : base("ActionCode/Persistence")
        {
        }

        [SettingsProvider]
        private static SettingsProvider CreateProjectSettingsMenu() =>
            new PersistenceSettingsProvider();

        protected override string GetConfigName() => "com.actioncode.persistence.settings";
    }
}