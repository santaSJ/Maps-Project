using Shared_Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace My_Helper
{
    public sealed partial class FloatingButton : UserControl
    {
        

        public FloatingButton()
        {
            this.InitializeComponent();
        }

        public string Title
        {
            get
            {
                return TitleTextBlock.Text;
            }
            set
            {
                TitleTextBlock.Text = value;
            }
        }

        private double _distance { get; set; }
        public double Distance
        {
            get
            {
                return _distance;
            }
            set
            {
                DistanceTextBlock.Text = value.ToString() + " " + "Km" ;
                _distance = value;
            }
        }

        private Geopoint _location;
        public Geopoint Location
        {
            get
            {
                return _location;
            }
            set
            {
                _location = value;
            }
        }

        private string _placeType { get; set; }
        public string PlaceType
        {
            get
            {
                return _placeType;
            }
            set
            {
                TypeImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/" + value));
                _placeType = value;
                switch (value)
                {
                    case POI_Type.atm:
                        Pane.Background = new SolidColorBrush(Colors.ForestGreen);
                        break;
                    case POI_Type.food:
                        Pane.Background = new SolidColorBrush(Colors.DeepPink);
                        break;
                    case POI_Type.general:
                        Pane.Background = new SolidColorBrush(Colors.LightBlue);
                        break;
                    case POI_Type.guest_house:
                        Pane.Background = new SolidColorBrush(Colors.Lavender);
                        break;
                    case POI_Type.health:
                        Pane.Background = new SolidColorBrush(Colors.LightPink);
                        break;
                    case POI_Type.hostels:
                        Pane.Background = new SolidColorBrush(Colors.LimeGreen);
                        break;
                    case POI_Type.repair:
                        Pane.Background = new SolidColorBrush(Colors.Maroon);
                        break;
                    case POI_Type.shopping:
                        Pane.Background = new SolidColorBrush(Colors.Orange);
                        break;
                    case POI_Type.study:
                        Pane.Background = new SolidColorBrush(Colors.SteelBlue);
                        break;
                    case POI_Type.transport:
                        Pane.Background = new SolidColorBrush(Colors.MintCream);
                        break;

                }
            }
        }

        private double _left { get; set; }

        public double Left
        {
            get
            {
                return _left;
            }
            set
            {
                _left = value;
            }
        }

        private double _top { get; set; }

        public double Top
        {
            get
            {
                return _top;
            }
            set
            {
                _top = value;
            }
        }
    }
}
