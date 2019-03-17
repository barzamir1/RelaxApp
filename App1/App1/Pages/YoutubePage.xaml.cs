
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App1.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class YoutubePage : ContentPage
	{
		public YoutubePage ()
		{
			InitializeComponent ();
            //webView.Source = "https://www.youtube.com/watch?v=1ZYbU82GVz4";
            webView.Source = "https://www.youtube.com/embed/1ZYbU82GVz4"; 
        }

    }
}