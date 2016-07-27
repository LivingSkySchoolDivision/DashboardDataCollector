using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LSKYDashboardDataCollector.FleetVision
{
    public class FleetVisionVehicle
    {
        public int ID { get; set; }
        public string Class { get; set; }
        public string VIN { get; set; }
        public string Description { get; set; }
        public string Plate { get; set; }
        public string Plate2 { get; set; }
        public string Driver { get; set; }
        public string FuelType { get; set; }
        public bool IsActive { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string ModelYear { get; set; }
        public DateTime LastUpdated { get; set; }
        public string LastUpdatedBy { get; set; }
    }
}