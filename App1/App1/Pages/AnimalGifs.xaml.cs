using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App1.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AnimalGifs : ContentPage
	{
		public AnimalGifs ()
		{
			InitializeComponent ();
            webView.Source = "https://giphy.com/explore/cute-animals";
        }
    }
}