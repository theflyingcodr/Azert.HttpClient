using Azert.HttpClient.Clients;
using Azert.HttpClient.Clients.Interfaces;
using Azert.HttpClient.Services;
using Azert.HttpClient.Services.Interfaces;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Azert.HttpClient.Windsor {
    public class Installer : IWindsorInstaller {

        public void Install(IWindsorContainer container, IConfigurationStore store) {
            container.Register(
                               Component.For<IHttpService>().ImplementedBy<HttpService>().LifestyleSingleton(),
                               Component.For<IHttpAsyncClient>().ImplementedBy<HttpAsyncClient>().LifestyleSingleton()
                );
        }
    }
}