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
using Windows.UI.Xaml.Shapes;

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
            //CurrentPosition = new Geopoint(new BasicGeoposition() { Latitude = 10.759728, Longitude = 78.811040 });
            getPosition();
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
                    var lines = VirtualLayer.Children.OfType<Line>().ToList();
                    foreach (var line in lines)
                    {
                        VirtualLayer.Children.Remove(line);
                    }
                    var texts = VirtualLayer.Children.OfType<TextBlock>().ToList();
                    foreach (var text in texts)
                    {
                        VirtualLayer.Children.Remove(text);
                    }

                    Debug.WriteLine(VirtualLayer.Children.Count);
                    // New Code

                    List<PointOfInterest> points_to_display = new List<PointOfInterest>();
                    foreach (var poi in POIs)
                    {
                        var bearing = getBearing(CurrentPosition, poi.Location);
                        var diff = Math.Abs((double)compassReading - bearing);
                        if (diff < 30 || diff > 330)
                        {
                            var point = poi;
                            point.Left = getLeft(VirtualLayer.ActualWidth, compassReading, bearing);
                            point.Top = VirtualLayer.ActualHeight/2 - 70;
                            points_to_display.Add(point);

                        }
                    }
                    foreach (var poi in points_to_display)
                    {
                        
                        for( int i = 0; i < points_to_display.Count; i++)
                        {
                            var _poi = points_to_display[i];
                            if(Math.Abs(poi.Left-_poi.Left) < 80 && poi.Top == _poi.Top && poi != _poi)
                            {
                                if( getDistance(CurrentPosition, _poi.Location) > getDistance(CurrentPosition, poi.Location))
                                {
                                    points_to_display[i].Top -= 90;
                                }
                                else
                                {
                                    points_to_display[i].Top += 90;
                                }
                                i = 0;
                            }
                        }
                    }

                    for ( int i = 0; i < points_to_display.Count-1; i++)
                    {
                        var poi = points_to_display[i];
                        for (int j = i+1; j < points_to_display.Count; j++)
                        {
                            var _poi = points_to_display[j];
                            if( getDistance( poi.Location , _poi.Location) < 0.5 && Math.Abs( poi.Left - _poi.Left) < 50)
                            {
                                Line l = new Line();
                                l.Stroke = new SolidColorBrush(Colors.Black);
                                l.StrokeThickness = 3;
                                if (getDistance(CurrentPosition, _poi.Location) > getDistance(CurrentPosition, poi.Location))
                                {
                                    l.X1 = poi.Left + 40;
                                    l.Y1 = poi.Top;
                                    l.X2 = _poi.Left + 40;
                                    l.Y2 = _poi.Top + 80;

                                }
                                else
                                {
                                    l.X1 = poi.Left + 40;
                                    l.Y1 = poi.Top + 80;
                                    l.X2 = _poi.Left + 40;
                                    l.Y2 = _poi.Top ;
                                }
                                l.StrokeEndLineCap = PenLineCap.Triangle;
                                VirtualLayer.Children.Add(l);
                                TextBlock distText = new TextBlock();
                                distText.FontSize = 10;
                                distText.Text = Math.Round(getDistance(_poi.Location, poi.Location), 3).ToString() + " Km";
                                Canvas.SetLeft(distText, (l.X1 + l.X2) / 2 + 4);
                                Canvas.SetTop(distText, (l.Y1 + l.Y2) / 2 - 6);
                                VirtualLayer.Children.Add(distText);
                            }
                        }
                    }
                    foreach (var poi in points_to_display)
                    {
                        FloatingButton place = new FloatingButton();
                        place.Title = poi.DisplayName;
                        place.Location = poi.Location;
                        place.Distance = Math.Round(getDistance(CurrentPosition, poi.Location), 3);
                        place.PlaceType = poi.Type;
                        place.Tapped += Place_Tapped;
                        VirtualLayer.Children.Add(place);
                        VirtualLayer.HorizontalAlignment = HorizontalAlignment.Stretch;
                        VirtualLayer.VerticalAlignment = VerticalAlignment.Stretch;
                        Canvas.SetLeft(place, poi.Left);
                        Canvas.SetTop(place, poi.Top);

                    }


                });

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
