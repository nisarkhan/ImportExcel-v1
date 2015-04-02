using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImportExcel_v1.Models
{
    public class FilterModel
    {

        public FilterModel()
        {
            DataModel = new List<DeviceViewModel>();
        }

        private int _pageSize = 10;
        private int _page = 1;
        public int PageSize
        {
            get { return _pageSize; }
            set { _pageSize = value; }
        }

        public int Page
        {
            get { return _page; }
            set { _page = value; }
        }
        public int TotalCount { get; set; }
        //public string SearchText { get; set; }
        //public string Sort { get; set; }
        //public string Sortdir { get; set; }

        public List<DeviceViewModel> DataModel { get; set; }
    }
}