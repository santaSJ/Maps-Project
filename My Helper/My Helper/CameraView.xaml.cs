using Shared_Classes;
using Shared_Classes.Models;
using Shared_Classes.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Devices.Sensors;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Capture;
using Windows.Services.Maps;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace My_Helper
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CameraView : Page
    {
        MediaCapture mediaCapture;
        Geolocator _geolocator;
        Compass _compass;
        Geopoint CurrentPosition;
        PointOfInterestManager poiManager = new PointOfInterestManager();

        public CameraView()
        {
            this.InitializeComponent();
            StartPreviewAsync();
            CurrentPosition = new Geopoint(new BasicGeoposition() { Latitude = 10.759728, Longitude = 78.811040 });
            //getPosition();
            _compass = Compass.GetDefault();
            if( _compass != null)
            {
                uint minReportInterval = _compass.MinimumReportInterval;
                uint reportInterval = minReportInterval > 500 ? minReportInterval : 500;
                _compass.ReportInterval = reportInterval;
                _compass.ReadingChanged += _compass_ReadingChanged;

            }
            
        }

        private async void _compass_ReadingChanged(Compass sender, CompassReadingChangedEventArgs e)
        {
            var compassReading = e.Reading.HeadingMagneticNorth;
            if( compassReading < 270)
            {
                compassReading += 90;
            }
            else
            {
                compassReading += 90;
                compassReading %= 360;
            }
            var POIs = poiManager.FetchPOIs();
            if ( CurrentPosition != null)
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
                {
                    var images = VirtualLayer.Children.OfType<FloatingButton>().ToList();
                    foreach (var image in images)
                    {
                        VirtualLayer.Children.Remove(image);
                    }


                    Debug.WriteLine(VirtualLayer.Children.Count);
                    // New Code
                    foreach (var poi in POIs)
                    {
                        var bearing = getBearing(CurrentPosition, poi.Location);
                        var diff = Math.Abs((double)compassReading - bearing);
                        if (diff < 30 || diff > 330)
                        {
                            FloatingButton place = new FloatingButton();
                            place.Title = poi.DisplayName;
                            place.Location = poi.Location;
                            place.Distance = getDistance(CurrentPosition, poi.Location);
                            place.PlaceType = poi.Type;
                            place.Left = getLeft(VirtualLayer.ActualWidth, compassReading, bearing);
                            place.Top = 50;
                            place.Tapped += Place_Tapped;
                            List<LayoutHelper> points = new List<LayoutHelper>();
                            var buttons = VirtualLayer.Children.OfType<FloatingButton>().ToList();
                            foreach (var button in images)
                            {
                                points.Add(new LayoutHelper() { Left = button.Left, Top = button.Top, Width = button.ActualWidth });
                            }
                            Canvas.SetLeft(place, place.Left);
                            Debug.WriteLine(points.Count);
                            place.Top = getTop(points, place.Left, place.Top, place.ActualWidth);
                            //Debug.WriteLine(place.Top);
                            Canvas.SetTop(place, place.Top);
                            VirtualLayer.Children.Add(place);
                            Canvas.SetZIndex(place, 1);
                        }
                    }

                    // Remove all places out of range
                    /*
                    foreach (var child in VirtualLayer.Children)
                    {
                        
                        var button = child as FloatingButton;
                        var bearing = getBearing(CurrentPosition, button.Location);
                        var diff = Math.Abs((double)compassReading - bearing);
                        if( !(diff < 30 || diff > 330))
                        {
                            VirtualLayer.Children.Remove(child);
                        }
                    }

                    
                    // Change positions of places in range and already rendered
                    foreach (var child in VirtualLayer.Children)
                    {
                        var button = child as FloatingButton;
                        var bearing = getBearing(CurrentPosition, button.Location);
                        var diff = Math.Abs((double)compassReading - bearing);
                        button.Distance = getDistance(CurrentPosition, button.Location);
                        button.Left = getLeft(VirtualLayer.ActualWidth, compassReading, bearing);
                        Canvas.SetLeft(button, button.Left);

                    }
                    */
                });

                /*
                List<PointOfInterest> list = new List<PointOfInterest>();
                foreach (var point in POIs)
                {
                    var bearing = getBearing(CurrentPosition, point.Location);
                    var diff = Math.Abs((double)compassReading - bearing);
                    if( diff < 30 || diff > 330)
                    {
                        list.Add(point);
                    }
                }
                Debug.WriteLine(list.Count);
                foreach (var poi in POIs)
                {
                    var bearing = getBearing(CurrentPosition, poi.Location);
                    var diff = Math.Abs((double)compassReading - bearing);

                    if ( diff < 30 || diff > 330)
                    {
                        await Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
                        {
                            // Skip places already rendered
                            foreach (var child in VirtualLayer.Children)
                            {
                                var button = child as FloatingButton;
                                if( button.Location == poi.Location)
                                {
                                    return;
                                }
                            }
                            // Add new Place
                            FloatingButton place = new FloatingButton();
                            place.Title = poi.DisplayName;
                            place.Location = poi.Location;
                            place.Distance = getDistance(CurrentPosition, poi.Location);
                            place.PlaceType = poi.Type;
                            place.Left = getLeft(VirtualLayer.ActualWidth, compassReading, bearing);
                            place.Top = 50;
                            place.Tapped += Place_Tapped;
                            List<LayoutHelper> points = new List<LayoutHelper>();
                            foreach (var POI in VirtualLayer.Children)
                            {
                                var button = POI as FloatingButton;
                                points.Add(new LayoutHelper() { Left = button.Left, Top = button.Top, Width = button.ActualWidth });
                            }
                            Canvas.SetLeft(place, place.Left);
                            Debug.WriteLine(points.Count);
                            Canvas.SetTop(place, getTop( points , place.Left , place.Top , place.ActualWidth));
                            VirtualLayer.Children.Add(place);
                            Canvas.SetZIndex(place, 1);
                        });

                    }
                    
                }
                */
            }
            
        }

        private double getDistance( Geopoint p1 , Geopoint p2)
        {
            var dLat = p1.Position.Latitude - p2.Position.Latitude;
            dLat = ToRadian(dLat);
            var dLon = p1.Position.Longitude - p2.Position.Longitude;
            dLon = ToRadian(dLon);
            var a = Math.Pow(Math.Sin(dLat / 2), 2) + Math.Cos(ToRadian(p1.Position.Latitude)) * Math.Cos(ToRadian(p2.Position.Latitude)) * Math.Pow(Math.Sin(dLon / 2), 2);
            var b = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return 6373 * b;
        }

        private double getTop( List<LayoutHelper> points , double left , double top, double width)
        {
            foreach (var point in points)
            {
                double _left = point.Left, _top = point.Top, _width = point.Width, right = left + width;
                if( _top == top && ((left >= _left && left <= _left + _width) || (right >= _left && right <= _left+_width)))
                {
                    return getTop(points, left, top + 50, width);
                }
            }
            return top;
        }

        private async void Place_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var place = sender as FloatingButton;
            string uri = "ms-walk-to:?destination.latitude="+place.Location.Position.Latitude.ToString()+"&destination.longitude="+place.Location.Position.Longitude.ToString()+"&destination.name="+place.Title;
            Uri mapsUri = new Uri(uri);
            await Launcher.LaunchUriAsync(mapsUri);
        }

        private double getLeft(double actualWidth, double compass, double bearing)
        {
            var mid = actualWidth / 2;
            var div = mid / 30;
            if( compass < 180)
            {
                bearing -= compass;
            }
            else
            {
                bearing += (360 - compass);
                if(bearing > 90)
                {
                    bearing = -(360 - bearing);
                }
            }
            if( bearing > 0)
            {
                var dist = mid + div * bearing - 35;
                return dist;
            }
            else
            {
                var dist =  mid - div *(-bearing) - 35;
                return dist;
            }
        }

        private double getBearing( Geopoint myLocation , Geopoint destination)
        {
            
            var dLon = destination.Position.Longitude - myLocation.Position.Longitude;

            var x = Math.Cos(ToRadian(destination.Position.Latitude)) * Math.Sin(ToRadian(dLon));
            var y = Math.Cos(ToRadian(myLocation.Position.Latitude)) * Math.Sin(ToRadian(destination.Position.Latitude)) - Math.Sin(ToRadian(myLocation.Position.Latitude)) * Math.Cos(ToRadian(destination.Position.Latitude)) * Math.Cos(ToRadian(dLon));
            var value = ToDegree(Math.Atan2(x, y));
            if (value < 0)
            {
                value = 360 + value;
            }
            return value;
        }
        private async void getPosition()
        {
            var accessStatus = await Geolocator.RequestAccessAsync();

            switch (accessStatus)
            {
                case GeolocationAccessStatus.Allowed:
                    // Create Geolocator and define perodic-based tracking (2 second interval).
                    _geolocator = new Geolocator { ReportInterval = 1000 };

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

        public static double ToRadian(double val)
        {
            return val * (Math.PI / 180);
        }
        public static double ToDegree(double val)
        {
            return val * 180 / Math.PI;
        }

        private void OnStatusChanged(Geolocator sender, StatusChangedEventArgs args)
        {
        }

        private void OnPositionChanged(Geolocator sender, PositionChangedEventArgs e)
        {
            CurrentPosition = new Geopoint(new BasicGeoposition() { Latitude = e.Position.Coordinate.Latitude, Longitude = e.Position.Coordinate.Longitude });
            
        }

        private async void StartPreviewAsync()
        {
            try
            {
                mediaCapture = new MediaCapture();
                await mediaCapture.InitializeAsync();
                CameraStream.Source = mediaCapture;
                await mediaCapture.StartPreviewAsync();
            }
            catch (UnauthorizedAccessException)
            {
                // This will be thrown if the user denied access to the camera in privacy settings
                System.Diagnostics.Debug.WriteLine("The app was denied access to the camera");
            }
            catch (Exception)
            {
                System.Diagnostics.Debug.WriteLine("MediaCapture initialization failed.");
            }
        }

    }
}
