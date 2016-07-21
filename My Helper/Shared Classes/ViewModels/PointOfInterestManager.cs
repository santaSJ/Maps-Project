using Shared_Classes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Storage.Streams;

namespace Shared_Classes.ViewModels
{
    public class PointOfInterestManager
    {

        public List<PointOfInterest> FetchPOIs()
        {
            List<PointOfInterest> pois = new List<PointOfInterest>();
            
            
            pois.Add(new PointOfInterest()
            {
                DisplayName = "Garnet C",
                NormalizedAnchorPoint = new Windows.Foundation.Point(0.5, 1),
                Location = new Geopoint(new BasicGeoposition()
                {
                    Latitude = 10.763411,
                    Longitude = 78.812537
                }),
                mapIconReference = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/hostels.png")),
                Type = POI_Type.hostels,
                MoreInfo = "Hostel for NITT students"


            });
            
            pois.Add(new PointOfInterest()
            {
                DisplayName = "Garnet B",
                NormalizedAnchorPoint = new Windows.Foundation.Point(0.5, 1),
                Location = new Geopoint(new BasicGeoposition()
                { 
                    Latitude = 10.762979,
                    Longitude = 78.811539
                }),
                mapIconReference = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/hostels.png")),
                Type = POI_Type.hostels,
                MoreInfo = "Hostel for NITT students"


            });
            pois.Add(new PointOfInterest()
            {
                DisplayName = "Garnet A",
                NormalizedAnchorPoint = new Windows.Foundation.Point(0.5, 1),
                Location = new Geopoint(new BasicGeoposition()
                {
                    Latitude = 10.762629,
                    Longitude = 78.811543
                }),
                mapIconReference = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/hostels.png")),
                Type = POI_Type.hostels,
                MoreInfo = "Hostel for NITT students"


            });
            
            //10.759728, 
            
            pois.Add(new PointOfInterest()
            {
                DisplayName = "Orion",
                NormalizedAnchorPoint = new Windows.Foundation.Point(0.5, 1),
                Location = new Geopoint(new BasicGeoposition()
                {
                    Latitude = 10.759728,
                    Longitude = 78.811040
                }),
                mapIconReference = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/study.png")),
                Type = POI_Type.study,
                MoreInfo = "Lecture hall for seniors"


            });

            pois.Add(new PointOfInterest()
            {
                DisplayName = "Lecture Hall Complex",
                NormalizedAnchorPoint = new Windows.Foundation.Point(0.5, 1),
                Location = new Geopoint(new BasicGeoposition()
                {
                    Latitude = 10.761027,
                    Longitude = 78.814204
                }),
                mapIconReference = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/study.png")),
                Type = POI_Type.study,
                MoreInfo = "Lecture hall for first years"


            });

            pois.Add(new PointOfInterest()
            {
                DisplayName = "Octagon",
                NormalizedAnchorPoint = new Windows.Foundation.Point(0.5, 1),
                Location = new Geopoint(new BasicGeoposition()
                {
                    Latitude = 10.760545,
                    Longitude = 78.814673
                }),
                mapIconReference = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/study.png")),
                Type = POI_Type.study,
                MoreInfo = "Lecture hall for first years"


            });

            pois.Add(new PointOfInterest()
            {
                DisplayName = "NITT Hospital",
                NormalizedAnchorPoint = new Windows.Foundation.Point(0.5, 1),
                Location = new Geopoint(new BasicGeoposition()
                {
                    Latitude = 10.762398,
                    Longitude = 78.818537
                }),
                mapIconReference = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/health.png")),
                Type = POI_Type.health,
                MoreInfo = "Lecture hall for first years"


            });
            
            pois.Add(new PointOfInterest()
            {
                DisplayName = "Department of Computer Science",
                NormalizedAnchorPoint = new Windows.Foundation.Point(0.5, 1),
                Location = new Geopoint(new BasicGeoposition()
                {
                    Latitude = 10.760013,
                    Longitude = 78.818406
                }),
                mapIconReference = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/study.png")),
                Type = POI_Type.study,
                MoreInfo = "CSE Department"


            });

            pois.Add(new PointOfInterest()
            {
                DisplayName = "Agate Hostel",
                NormalizedAnchorPoint = new Windows.Foundation.Point(0.5, 1),
                Location = new Geopoint(new BasicGeoposition()
                {
                    Latitude = 10.762061,
                    Longitude = 78.813333
                }),
                mapIconReference = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/hostels.png")),
                Type = POI_Type.hostels,
                MoreInfo = "Hostel for first years"


            });
            //10.763076, 
            pois.Add(new PointOfInterest()
            {
                DisplayName = "Diamond Hostel",
                NormalizedAnchorPoint = new Windows.Foundation.Point(0.5, 1),
                Location = new Geopoint(new BasicGeoposition()
                {
                    Latitude = 10.763076,
                    Longitude = 78.814406
                }),
                mapIconReference = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/hostels.png")),
                Type = POI_Type.hostels,
                MoreInfo = "Hostel for first years"


            });

            //, 
            pois.Add(new PointOfInterest()
            {
                DisplayName = "Coral Hostel",
                NormalizedAnchorPoint = new Windows.Foundation.Point(0.5, 1),
                Location = new Geopoint(new BasicGeoposition()
                {
                    Latitude = 10.762161,
                    Longitude = 78.815552
                }),
                mapIconReference = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/hostels.png")),
                Type = POI_Type.hostels,
                MoreInfo = "Hostel for first years"


            });
            pois.Add(new PointOfInterest()
            {
                DisplayName = "Opal",
                NormalizedAnchorPoint = new Windows.Foundation.Point(0.5, 1),
                Location = new Geopoint(new BasicGeoposition()
                {
                    Latitude = 10.757885,
                    Longitude = 78.820690
                }),
                mapIconReference = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/hostels.png")),
                Type = POI_Type.hostels,
                MoreInfo = "Hostel first year girls"


            });

            pois.Add(new PointOfInterest()
            {
                DisplayName = "SBI ATM",
                NormalizedAnchorPoint = new Windows.Foundation.Point(0.5, 1),
                Location = new Geopoint(new BasicGeoposition()
                {
                    Latitude = 10.759389,
                    Longitude = 78.814166
                }),
                mapIconReference = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/atm.png")),
                Type = POI_Type.atm,
                MoreInfo = "ATM"


            });



            return pois;
        }
    }
}
