using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImportExcel_v1.Models
{
    public class DeviceViewModel
    {
        public string Location { get; set; }
        public string RackShelf { get; set; }
        public string DCLocation { get; set; }
        public string Customer { get; set; }
        public string SerialNumber { get; set; }
        public string Model { get; set; }
        public string UseState { get; set; }
        public string LocalName { get; set; }
        public string AssetTag { get; set; }
    }
}