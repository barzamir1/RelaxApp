using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App1
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GamePage : ContentPage
    {
        bool measurementStarted = false;
        public GamePage()
        {
            InitializeComponent();
            webView.Source = "https://www.cubeslam.com/jfrvfj?play"; //"https://monsterofcookie.itch.io/thug-racer";
            webView.Navigating += WebView_Navigating;
            webView.Navigated += WebView_Navigated;
            
        }

        private async void WebView_Navigated(object sender, WebNavigatedEventArgs e)
        {
            String script = "var levelElem = document.getElementById('level');" +
                            "var callback = function(){" + //called when the level element is changed
                                              "if (levelElem.innerText == '2'){" + //if user finished the first level
                                                   "window.location.href = 'FinishedLevel1';" + //will be canceled by webView Navigation event
                                               "}" +
                                            "}; " +
                            "var config = { attributes: true, childList: true, subtree: true }; " +
                            "var observer = new MutationObserver(callback); " +
                            "observer.observe(levelElem, config); ";
            try
            {
                await webView.EvaluateJavaScriptAsync(script);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private void WebView_Navigating(object sender, WebNavigatingEventArgs e)
        {
            var navigateTarget = e.Url;
            if(navigateTarget.Equals("https://www.cubeslam.com/FinishedLevel1"))
            {
                e.Cancel = true;
                if (!measurementStarted)
                {
                    var thread = new Thread(() => { MeasurementHandler.GetStressResult(-1, null); });
                    thread.Start();
                    measurementStarted = true;
                }
            }
        }
    }
}