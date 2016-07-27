﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LSKYDashboardDataCollector.FleetVision
{
    public class FleetVisionWorkOrder
    {
        public int ID { get; set; }
        public string WorkOrderNumber { get; set; }
        public string RequestBy { get; set; }
        public int VehicleID { get; set; }
        public string WorkRequested { get; set; }
        public string Status { get; set; }
        public DateTime EstDateTime { get; set; }
        public string WorkPerformed { get; set; }
        public decimal PartsTotal { get; set; }
        public decimal LaborTotal { get; set; }
        public string CreatedBy { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime LastUpdated { get; set; }
        public decimal ShopFee { get; set; }
        public string InvoiceNumber { get; set; }

        public bool IsClosed => this.Status.ToLower().Equals("completed");
    }
}