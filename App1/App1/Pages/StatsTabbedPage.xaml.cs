using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkingWithMaps;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App1.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StatsTabbedPage : TabbedPage
    {
        public StatsTabbedPage()
        {
            InitializeComponent();
            Title = "Statistics";

            Children.Add(new CalendarStats() { Title = "Calendar", Icon = "calendar.png" });
            Children.Add(new PinPage() { Title = "Map", Icon = "map.png" });
            //TODO: consider removing these pages:
            Children.Add(new LastMeasurementsListPage() { Title = "All", Icon = "list.png" });
            Children.Add(new ActivitiesListPage() { Title = "Activities", Icon = "activity.png" });
        }
    }
}