﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using Caliburn.Micro;
using SimpleInjector;
using WatchingArchiver.ViewModels;

namespace WatchingArchiver.Bootstrap
{
    public class Bootstrapper : BootstrapperBase
    {
        /// <summary>
        ///     IoC Container instance
        /// </summary>
        public static readonly Container ContainerInstance = new Container();

        /// <inheritdoc />
        public Bootstrapper()
        {
            Initialize();
        }

        /// <inheritdoc />
        protected override void Configure()
        {
            ContainerInstance.Register<IWindowManager, WindowManager>();
            ContainerInstance.RegisterSingleton<IEventAggregator, EventAggregator>();
            //ContainerInstance.RegisterSingleton<IDialogCoordinator, DialogCoordinator>();

            ContainerInstance.Verify();
        }

        /// <inheritdoc />
        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<MainWindowViewModel>();
        }

        /// <inheritdoc />
        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            IServiceProvider provider = ContainerInstance;
            var collectionType = typeof(IEnumerable<>).MakeGenericType(service);
            var services = (IEnumerable<object>)provider.GetService(collectionType);
            return services ?? Enumerable.Empty<object>();
        }

        /// <inheritdoc />
        protected override object GetInstance(Type service, string key)
        {
            return ContainerInstance.GetInstance(service);
        }

        /// <inheritdoc />
        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            return new[]
            {
                Assembly.GetExecutingAssembly()
            };
        }

        /// <inheritdoc />
        protected override void BuildUp(object instance)
        {
            var registration = ContainerInstance.GetRegistration(instance.GetType(), true);
            registration.Registration.InitializeInstance(instance);
        }
    }
}