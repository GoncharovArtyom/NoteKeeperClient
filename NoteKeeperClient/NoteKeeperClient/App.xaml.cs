using NoteKeeper.Model;
using NoteKeeperClient.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace NoteKeeperClient
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            var connectionString = "http://notekeeper.azurewebsites.net/api/";
            var client = new RestClient.RestClient(connectionString);
            var page = new NoteKeeperClient.Views.LoginScreen(client);
            MainPage = page;
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
