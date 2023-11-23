using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Client.Core;
using Client.MVVM.View;
using Client.MVVM.ViewModel;
using Client.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Client
{
    public partial class App : Application
    {
        private readonly ServiceProvider _serviceProvider;

        public App()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddSingleton<MainWindowView>(provider => new MainWindowView()
            {
                DataContext = provider.GetRequiredService<MainViewModel>()
            } );
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<RegisterViewModel>();
            services.AddSingleton<LoginViewModel>();
            services.AddSingleton<HomeViewModel>();
            services.AddSingleton<SettingsViewModel>();
            services.AddSingleton<RatingViewModel>();
            services.AddSingleton<CreateGameViewModel>();
            services.AddSingleton<JoinGameViewModel>();
            services.AddSingleton<ShipSelectionViewModel>();
            services.AddSingleton<IValueConverter, ActiveViewModelConverter>();
            services.AddSingleton<BooleanToPrivacyTextConverter>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<WaitingPageViewModel>();
            //Делегат
            // Працює типу var x = Func
            services.AddSingleton<Func<Type, ViewModel>>(serviceProvider =>
                viewModelType => (ViewModel)serviceProvider.GetRequiredService(viewModelType));
            
            _serviceProvider = services.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            var mainWindow = _serviceProvider.GetRequiredService<MainWindowView>();
            mainWindow.Show();
            base.OnStartup(e);
        }
    }
}