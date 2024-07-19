using EmberToolkit.Common.Interfaces.Configuration;
using EmberToolkit.Common.Interfaces.Services;
using EmberToolkit.Unity.Services;

namespace EmberToolkit.Unity.Behaviours.Managers
{
    public abstract class IntegrationManager : EmberSingleton
    {
        private IEmberSettings _settings;

        protected override void Awake()
        {
            InitializeSettings();
            //Load Resources to be passed into DI for access by backend and ServiceConductor
            //TODO: Move Settings into a configuration/ini file
            SaveObject = false;
            RequestService(out _settings);

            IEmberServiceConnector coreService = new EmberServiceConnector(_settings);

            base.Awake();//Base awake needs to be called after DI and ServiceConductor are fully initilized.

        }

        protected abstract void InitializeSettings();

    }
}
