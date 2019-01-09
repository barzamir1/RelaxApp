using Android.Gms.Location;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(App1.Droid.Location))]
namespace App1.Droid
{
    class Location:ILocation
    {
        //Location provider
        FusedLocationProviderClient fusedLocationProviderClient = Droid.MainActivity.fusedLocationProviderClient;


        //Getting location for measurement
        public async Task<Android.Locations.Location> GetLastLocationFromDevice()
        {
            // This method assumes that the necessary run-time permission checks have succeeded.
            Android.Locations.Location location = await fusedLocationProviderClient.GetLastLocationAsync();

            if (location == null)
            {
                return new Android.Locations.Location("Location didn't work");
            }
            else
            {
                return location;
            }
        }
    }
}