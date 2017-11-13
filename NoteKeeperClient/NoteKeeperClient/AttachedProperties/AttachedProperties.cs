using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace NoteKeeperClient.AttachedProperties
{
    public static class AttachedProperties
    {
        public static BindableProperty IsBusyProperty =
      BindableProperty.CreateAttached("IsBusy", typeof(bool), typeof(ProgressBar), false,
        BindingMode.OneWay, null, IsBusyChanged);

        public static BindableProperty ErrorProperty =
      BindableProperty.CreateAttached("Error", typeof(string), typeof(ContentPage), null,
        BindingMode.TwoWay, null, ErrorChanged);

        private static void IsBusyChanged(BindableObject obj, object oldValue, object newValue)
        {
            var progressBar = obj as ProgressBar;
            if (progressBar != null)
            {
                switch ((bool)newValue)
                {
                    case true:
                        {
                            progressBar.Progress = 0;
                            progressBar.IsVisible = true;
                            progressBar.ProgressTo(1, 1000, Easing.Linear);
                            break;
                        }
                    case false:
                        {
                            progressBar.IsVisible = false;
                            break;
                        }
                }
            }
        }

        private static async void ErrorChanged(BindableObject obj, object oldValue, object newValue)
        {
            if (newValue as string == null)
            {
                return;
            }
            var page = obj as ContentPage;
            if (page != null)
            {
                await page.DisplayAlert("Ошибка", (string)newValue, "Ok");
                page.SetValue(ErrorProperty, null);
            }
        }
    }
}
