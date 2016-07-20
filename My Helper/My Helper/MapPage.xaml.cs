using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.Connectivity;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Shared_Classes.ViewModels;
using Windows.Storage.Streams;
using Windows.Devices.Sensors;
using Windows.UI.Core;
using Shared_Classes.Models;
using Windows.System;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace My_Helper
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MapPage : Page
    {

        PointOfInterestManager poiManager;
        Geolocator _geolocator;
        MapIcon currentLocation = new MapIcon();
        public Point point = new Point(0.5, 1);
        private RandomAccessStreamReference mapIconStreamReference;
        bool isOpen = false;
        Geopoint location_arg = null;
        string place_arg = "";

        public MapPage()
        {
            this.InitializeComponent();
            mapIconStreamReference = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/mappin.png"));
            Map.Loaded += Map_Loaded;
            currentLocation.Title = "My Location";
            currentLocation.NormalizedAnchorPoint = new Point(0.5, 1);
            currentLocation.ZIndex = 0;
            currentLocation.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/my_location.png"));
            getPosition();
            Map.MapElements.Add(currentLocation);
            
        }

        public static double ToRadian(double val)
        {
            return val * (Math.PI / 180);
        }

        public static double ToDegree(double val)
        {
            return val * 180 / Math.PI;
        }

        private double getDistance(Geopoint p1, Geopoint p2)
        {
            var dLat = p1.Position.Latitude - p2.Position.Latitude;
            dLat = ToRadian(dLat);
            var dLon = p1.Position.Longitude - p2.Position.Longitude;
            dLon = ToRadian(dLon);
            var a = Math.Pow(Math.Sin(dLat / 2), 2) + Math.Cos(ToRadian(p1.Position.Latitude)) * Math.Cos(ToRadian(p2.Position.Latitude)) * Math.Pow(Math.Sin(dLon / 2), 2);
            var b = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return 6373 * b;
        }

        private async void getPosition()
        {
            var accessStatus = await Geolocator.RequestAccessAsync();

            switch (accessStatus)
            {
                case GeolocationAccessStatus.Allowed:
                    // Create Geolocator and define perodic-based tracking (2 second interval).
                    _geolocator = new Geolocator { ReportInterval = 2000 };

                    // Subscribe to the PositionChanged event to get location updates.
                    _geolocator.PositionChanged += OnPositionChanged;

                    // Subscribe to StatusChanged event to get updates of location status changes.
                    _geolocator.StatusChanged += OnStatusChanged;

                    break;

                case GeolocationAccessStatus.Denied:

                    break;

                case GeolocationAccessStatus.Unspecified:

                    break;
            }

        }

        private void OnStatusChanged(Geolocator sender, StatusChangedEventArgs args)
        {
        }

        private async void OnPositionChanged(Geolocator sender, PositionChangedEventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Geopoint current = new Geopoint(new BasicGeoposition()
                {
                    Latitude = e.Position.Coordinate.Latitude,
                    Longitude = e.Position.Coordinate.Longitude
                });
                currentLocation.Location = new Geopoint(new BasicGeoposition()
                {
                    Latitude = e.Position.Coordinate.Latitude,
                    Longitude = e.Position.Coordinate.Longitude
                });

                if(isOpen && location_arg != null)
                {
                    //DistanceTextBlock.Text = getDistance(current, location_arg).ToString() + "Km";
                }
            });

        }


        private void Map_Loaded(object sender, RoutedEventArgs e)
        {
            poiManager = new PointOfInterestManager();
            var poiPoints = poiManager.FetchPOIs();
            foreach (var poi in poiPoints)
            {
                MapIcon icon = new MapIcon();
                icon.Location = poi.Location;
                icon.Title = poi.DisplayName;
                icon.ZIndex = 0;
                icon.Image = poi.mapIconReference;
                icon.NormalizedAnchorPoint = poi.NormalizedAnchorPoint;
                Map.MapElements.Add(icon);
            }
            poiManager = new PointOfInterestManager();
            if (place_arg != "" && place_arg != null)
            {
                List<PointOfInterest> list = poiManager.FetchPOIs();
                foreach (var point in list)
                {
                    if (point.DisplayName.ToLower() == place_arg.ToLower())
                    {
                        DetailsView.Visibility = Visibility.Visible;
                        TitleTextBlock.Text = point.DisplayName;
                        MoreTextBlock.Text = point.MoreInfo;
                        LatitudeTextBlock.Text = "Lat:" + point.Location.Position.Latitude.ToString();
                        LongitudeTextBlock.Text = "Lon:" + point.Location.Position.Longitude.ToString();
                        location_arg = point.Location;
                    }
                }
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Map.Center = new Geopoint(new BasicGeoposition() { Latitude = 10.7611, Longitude = 78.8139 });
            Map.ZoomLevel = 16;
            if( checkInternet())
            {
                Map.Style = MapStyle.AerialWithRoads;
            }
            if( e.Parameter != null)
            {
                isOpen = true;
                place_arg = (string)e.Parameter;
                
            }
        }

        private bool checkInternet()
        {
            var conn = NetworkInformation.GetInternetConnectionProfile();
            if( conn != null)
            {
                return true;
            }
            return false;
        }

        private void Map_MapElementClick(MapControl sender, MapElementClickEventArgs args)
        {
            //var location = args.Location;
            var x = (MapIcon)args.MapElements.FirstOrDefault();
            var location = x.Location;
            poiManager = new PointOfInterestManager();
            List<PointOfInterest> list = poiManager.FetchPOIs();
            foreach (var point in list)
            {
                if (point.Location.Position.Latitude == location.Position.Latitude && point.Location.Position.Longitude == location.Position.Longitude)
                {
                    DetailsView.Visibility = Visibility.Visible;
                    TitleTextBlock.Text = point.DisplayName;
                    MoreTextBlock.Text = point.MoreInfo;
                    LatitudeTextBlock.Text = "Lat:" + point.Location.Position.Latitude.ToString();
                    LongitudeTextBlock.Text = "Lon:" + point.Location.Position.Longitude.ToString();
                    location_arg = point.Location;
                }
            }
        }

        private async void NavigateButton_Click(object sender, RoutedEventArgs e)
        {
            string uri = "ms-walk-to:?destination.latitude=" + location_arg.Position.Latitude.ToString() + "&destination.longitude=" + location_arg.Position.Longitude.ToString() + "&destination.name=" + TitleTextBlock.Text;
            Uri mapsUri = new Uri(uri);
            await Launcher.LaunchUriAsync(mapsUri);
        }
    }

}
