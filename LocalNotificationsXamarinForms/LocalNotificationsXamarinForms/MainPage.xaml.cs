using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace LocalNotificationsXamarinForms
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        void fire_Clicked(System.Object sender, System.EventArgs e)
        {
            var date = new DateTime(2020, 6, 22, 17, 12, 0);
            DependencyService.Get<INotifierService>().Notify(titleentry.Text, bodyentry.Text, 573);
        }
    }
}
