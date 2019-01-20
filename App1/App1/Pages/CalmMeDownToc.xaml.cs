
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App1
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CalmMeDownToc : ContentPage
	{
		public CalmMeDownToc ()
        {
            InitializeComponent();
            webView.Source = "https://giphy.com/explore/cute-animals";
        }
    }
}