using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NoteKeeperClient.RestClient;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using NoteKeeperClient.ViewModels;
using NoteKeeper.Model;

namespace NoteKeeperClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginScreen : TabbedPage
    {
        public LoginScreen(IRestClient client)
        {
            InitializeComponent();
            this.BindingContext = new LoginScreenViewModel(client, async (User user) =>
            {
                App.Current.MainPage = new MainScreen(client, user);
            });
        }
    }
}