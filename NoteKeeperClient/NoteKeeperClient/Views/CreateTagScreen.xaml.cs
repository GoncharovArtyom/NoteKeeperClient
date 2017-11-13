using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using NoteKeeper.Model;
using NoteKeeperClient.ViewModels;
using NoteKeeperClient.RestClient;
using System.Collections.ObjectModel;

namespace NoteKeeperClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CreateTagScreen : ContentPage
    {
        public CreateTagScreen(IRestClient client, User user, Action<Tag> applyLocalChanges)
        {
            InitializeComponent();
            BindingContext = new CreateTagScreenViewModel(client, user, applyLocalChanges, async () =>
            {
                await Navigation.PopModalAsync();
            });
        }
    }
}