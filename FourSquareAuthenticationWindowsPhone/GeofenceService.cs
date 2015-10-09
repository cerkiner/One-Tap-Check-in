using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Windows.Devices.Geolocation;
using Windows.Devices.Geolocation.Geofencing;

namespace GeofenceUniversalApp
{
    public static class GeofenceService
    {
        public static IList<Geofence> GetGeofences()
        {
            return GeofenceMonitor.Current.Geofences;
        }

        public static void CreateGeofence(string id, double lat, double lon, double radius)
        {
            if (GeofenceMonitor.Current.Geofences.SingleOrDefault(g => g.Id == id) != null) return;

            var position = new BasicGeoposition();
            position.Latitude = lat;
            position.Longitude = lon;

            var geocircle = new Geocircle(position, radius);

            MonitoredGeofenceStates mask = 0;
            mask |= MonitoredGeofenceStates.Entered;
            // Uncomment these to monitor other states
            mask |= MonitoredGeofenceStates.Exited;
            mask |= MonitoredGeofenceStates.Removed;

            // Create Geofence with the supplied id, geocircle and mask, not for single use
            // and with a dwell time of 5 seconds

            //MessageBox.Show(string.Format("Geofence {0} created!", id.ToString()));
            var geofence = new Geofence(id, geocircle, mask, false, new TimeSpan(0, 0, 5));
            GeofenceMonitor.Current.Geofences.Add(geofence);
        }

        public static void RemoveGeofence(string id)
        {
            var geofence = GeofenceMonitor.Current.Geofences.SingleOrDefault(g => g.Id == id);

            if (geofence != null)
                GeofenceMonitor.Current.Geofences.Remove(geofence);
        }
    }
}
