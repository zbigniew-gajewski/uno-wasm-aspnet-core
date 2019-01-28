using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Windows.Input;
using Breeze.Sharp;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Uno.Extensions;
using Uno.Logging;
using UnoTest.Web.Data;

namespace UnoTest.Shared.ViewModels
{

#if __WASM__
    [Bindable(true)]
#endif
    public class MainPageViewModel : ViewModelBase
    {
        private string result;

        public MainPageViewModel()
        {
            GetDataUsingHttpClientCommand = new RelayCommand(OnGetDataUsingHttpClient);
            GetDataUsingBreezeSharpCommand = new RelayCommand(OnGetDataUsingBreezeSharp);

            OnGetDataUsingBreezeSharp();
        }

    
        public ICommand GetDataUsingHttpClientCommand { get; }
        public ICommand GetDataUsingBreezeSharpCommand { get; }

        public string Result
        {
            get => result;
            set
            {
                result = value;
                RaisePropertyChanged(nameof(Result));
            }
        }

        private async void OnGetDataUsingHttpClient()
        {
            #region simple HttpClient

            var handler = new Uno.UI.Wasm.WasmHttpHandler();

            var httpClient = new System.Net.Http.HttpClient(handler);

            // var httpClient = new Windows.Web.HttpClient(handler); for UWP

            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/json"));

            var response = await httpClient.GetAsync("http://localhost:53333/breeze/Customer/Customers");

            var jsonString = await response.Content.ReadAsStringAsync();

            // to do: better deserialization here:
            var jsonStringSplitted = jsonString
                .Replace('}',' ')
                .Replace(']', ' ')
                .Split(',')
                .Where(l => l.Contains("First") || l.Contains("Last") || l.Contains("Descrip"));

            var stringBuilder = new StringBuilder();
            foreach (var line in jsonStringSplitted)
            {
                stringBuilder.Append(line + "\n");
            }

            Result = stringBuilder.ToString();

            #endregion
        }

        private async void OnGetDataUsingBreezeSharp()
        {
            //this.Log().Debug("%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%");
            //Console.WriteLine("$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$");

            try
            {
                var serviceAddress = "http://localhost:53333/breeze/Customer/";
                var assembly = typeof(Customer).Assembly;
                var rslt = Configuration.Instance.ProbeAssemblies(assembly);

                DataService.DefaultHttpMessageHandler = new Uno.UI.Wasm.WasmHttpHandler();

                var entityManager = new EntityManager(serviceAddress);

                var query = new EntityQuery<Customer>();
                var customers = await entityManager.ExecuteQuery(query);

                var stringBuilder = new StringBuilder();
                foreach (var customer in customers)
                {
                    stringBuilder.AppendLine($"{customer.FirstName} - {customer.LastName} - {customer.Description} !");
                }

                Result = stringBuilder.ToString();
                //Result = result?.FirstOrDefault()?.FirstName;
            }
            catch (Exception ex)
            {
                Result = ex.ToString();
            }
        }








        ////private int numberOfClicks = 0;
        ////private string customerName = "Customer Name here!";
        ////private ObservableCollection<int> numbers = new ObservableCollection<int>();
        ////private ObservableCollection<string> navigationItems = new ObservableCollection<string>();
        ////private string selectedNavigationItem;

        ////private EntityManager entityManager;
        ////private string errorMessage;

        ////public MainPageViewModel()
        ////{
        ////    ClickMeButtonCommand = new RelayCommand(OnClickMeButton);
        ////    NavigationItems.Add("Counter");
        ////    NavigationItems.Add("Data");
        ////}

        ////public ICommand GetDataUsingHttpClientCommand { get; }
        

        ////public ICommand ClickMeButtonCommand { get; } 

        ////public int NumberOfClicks
        ////{
        ////    get => numberOfClicks;
        ////    set
        ////    {
        ////        numberOfClicks = value;
        ////        RaisePropertyChanged(nameof(NumberOfClicks));
        ////    }
        ////}


        ////public string CustomerName
        ////{
        ////    get => customerName;
        ////    set
        ////    {
        ////        customerName = value;
        ////        RaisePropertyChanged(nameof(CustomerName));
        ////    }
        ////}

        ////public string ErrorMessage
        ////{
        ////    get => errorMessage;
        ////    set
        ////    {
        ////        errorMessage = value;
        ////        RaisePropertyChanged(nameof(ErrorMessage));
        ////    }
        ////}

        ////public string SelectedNavigationItem
        ////{
        ////    get => selectedNavigationItem;
        ////    set
        ////    {
        ////        selectedNavigationItem = value;
        ////        ErrorMessage = selectedNavigationItem; // temp
        ////        RaisePropertyChanged(nameof(SelectedNavigationItem));
        ////    }
        ////}


        ////public ObservableCollection<int> Numbers => numbers;
        ////public ObservableCollection<string> NavigationItems => navigationItems;

        ////private void OnClickMeButton()
        ////{
        ////    NumberOfClicks += 1;
        ////    Numbers.Add(NumberOfClicks);



      
        ////    GetCustomers();

        ////}

        ////private async void GetCustomers()
        ////{

        ////    #region simple HttpClient

        ////    //var handler = new Uno.UI.Wasm.WasmHttpHandler();

        ////    //var httpClient = new System.Net.Http.HttpClient(handler);

        ////    //// var httpClient = new Windows.Web.HttpClient(handler); for UWP

        ////    //httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        ////    //httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/json"));

        ////    //var response = await httpClient.GetAsync("http://localhost:53333/breeze/Customer/Customers");

        ////    //var jsonString = await response.Content.ReadAsStringAsync();

        ////    ////some deserialization here
        ////    //ErrorMessage = jsonString.ToString();

        ////    #endregion

        ////    #region BreezeSharp

        ////    // Breeze.Sharp token authentication https://stackoverflow.com/questions/24104452/get-access-token-with-breezesharp
        ////    // https://stackoverflow.com/questions/21146587/passing-authentication-token-with-breeze-query
        ////    // Breeze.Sharp examples:  https://github.com/Breeze/breeze.sharp.samples/tree/master/ToDo

        ////    try
        ////    {
        ////        var serviceAddress = "http://localhost:53333/breeze/Customer/";
        ////        var assembly = typeof(Customer).Assembly;
        ////        var rslt = Configuration.Instance.ProbeAssemblies(assembly);

        ////        DataService.DefaultHttpMessageHandler = new Uno.UI.Wasm.WasmHttpHandler();

        ////        entityManager = new EntityManager(serviceAddress);

        ////        var query = new EntityQuery<Customer>();
        ////        var result = await entityManager.ExecuteQuery(query);

        ////        // CustomerName = result?.FirstOrDefault()?.FirstName;
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        ErrorMessage = ex.ToString();
        ////    }

        ////    #endregion
        ////}
    }
}
