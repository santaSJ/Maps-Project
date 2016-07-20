using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Sensors;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


namespace My_Helper
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Accelerometer _accelerometer;
        private bool isCamera;
        string arg_map = "";

        public MainPage()
        {
            this.InitializeComponent();
            _accelerometer = Accelerometer.GetDefault();
            isCamera = false;
            if (_accelerometer != null)
            {
                uint minReportInterval = _accelerometer.MinimumReportInterval;
                uint reportInterval = minReportInterval > 500 ? minReportInterval : 500;
                _accelerometer.ReportInterval = reportInterval;

                _accelerometer.ReadingChanged += _accelerometer_ReadingChanged;
            }
            ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if(e.Parameter != null)
            {
                arg_map = (string)e.Parameter;
                MainViewSelector.Navigate(typeof(MapPage), arg_map);
            }

        }

        private async void _accelerometer_ReadingChanged(Accelerometer sender, AccelerometerReadingChangedEventArgs args)
        {
            var reading = args.Reading;
            if (Math.Abs(reading.AccelerationZ) < 0.4  && isCamera == false)
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    MainViewSelector.BackStack.Clear();
                    MainViewSelector.Navigate(typeof(CameraView));
                    arg_map = "";

                });
                isCamera = true;
            }
            else if(Math.Abs(reading.AccelerationZ) > 0.4  && isCamera == true)
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    MainViewSelector.BackStack.Clear();
                    MainViewSelector.Navigate(typeof(MapPage), arg_map);

                });
                isCamera = false;
            }
        }
        /*
        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            HamburgerLayout.IsPaneOpen = !HamburgerLayout.IsPaneOpen;
        }
        */
        /*
        private void ScenarioSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(ScenarioSelector.SelectedItem == Map)
            {
                isMap = true;
                if(_accelerometer != null)
                {
                    var reading = _accelerometer.GetCurrentReading();
                    if( Math.Abs(reading.AccelerationZ) < 0.4)
                    {
                        MainViewSelector.Navigate(typeof(CameraView));
                        isCamera = true;
                        return;
                    }
                }
                MainViewSelector.BackStack.Clear();
                MainViewSelector.Navigate(typeof(MapPage));
                isCamera = false;
            }
            else
            {
                isMap = false;
            }
        }
        */
    }
}
