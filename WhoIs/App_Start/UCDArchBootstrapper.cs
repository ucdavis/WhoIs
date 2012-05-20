using System.Web.Mvc;
using Castle.Windsor;
using Microsoft.Practices.ServiceLocation;
using WhoIs.App_Start;
using WhoIs.Controllers;
using UCDArch.Data.NHibernate;
using UCDArch.Web.IoC;
using UCDArch.Web.ModelBinder;
using WhoIs.Models;

[assembly: WebActivator.PreApplicationStartMethod(typeof(UCDArchBootstrapper), "PreStart")]
namespace WhoIs.App_Start
{
    public class UCDArchBootstrapper
    {
        /// <summary>
        /// PreStart for the UCDArch Application configures the model binding, db, and IoC container
        /// </summary>
        public static void PreStart()
        {
            ModelBinders.Binders.DefaultBinder = new UCDArchModelBinder();

            NHibernateSessionConfiguration.Mappings.UseFluentMappings(typeof(User).Assembly);

            IWindsorContainer container = InitializeServiceLocator();
        }

        private static IWindsorContainer InitializeServiceLocator()
        {
            IWindsorContainer container = new WindsorContainer();

            ControllerBuilder.Current.SetControllerFactory(new WindsorControllerFactory(container));

            container.RegisterControllers(typeof(HomeController).Assembly);
            ComponentRegistrar.AddComponentsTo(container);

            ServiceLocator.SetLocatorProvider(() => new WindsorServiceLocator(container));

            return container;
        }
    }
}