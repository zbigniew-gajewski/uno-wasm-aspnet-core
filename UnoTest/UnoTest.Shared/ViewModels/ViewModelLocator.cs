using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;

#if __WASM__
using Microsoft.Practices.ServiceLocation;
using System.ComponentModel;
#else
using CommonServiceLocator;
#endif

namespace UnoTest.Shared.ViewModels
{
#if __WASM__
    [Bindable(true)]
#endif
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default as IServiceLocator);
            if (ViewModelBase.IsInDesignModeStatic)
            {
                // Create design time view services and models
            }
            else
            {
                // Create run time view services and models
            }

            //Register your services used here
            //  SimpleIoc.Default.Register<INavigationService, NavigationService>();
            SimpleIoc.Default.Register<MainPageViewModel>();

        }

        public MainPageViewModel MainPageViewModelInstance => ServiceLocator.Current.GetInstance<MainPageViewModel>();

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}