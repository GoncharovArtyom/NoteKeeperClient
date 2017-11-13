using NoteKeeper.Model;
using NoteKeeperClient.RestClient;
using NoteKeeperClient.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NoteKeeperClient.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SharedNoteScreen : ContentPage
	{
		public SharedNoteScreen (IRestClient client, SharedNote sharedNote)
		{
			InitializeComponent ();
            BindingContext = new SharedNoteScreenViewModel(client, sharedNote);
		}
	}
}