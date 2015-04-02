using ImportExcel_v1.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ImportExcel_v1
{
    public class BulkUpload2Controller : Controller
    {
        FilterModel storedevices = new FilterModel();
        List<ImportDeviceViewModel>  gridToDBRecordList = new List<ImportDeviceViewModel>();
          
        public ActionResult Index()
        {
            if (Session["storedevices"] == null)
            {
                FilterModel fm = new FilterModel();
                fm = storedevices;
                Session["storedevices"] = fm;
            }

            return View(Session["storedevices"]);
        }

        //[HttpPost]
        //public ActionResult PageSize(FormCollection formCollection)
        //{
        //    if (Session["storedevices"] == null) { return View("Index"); }

        //    foreach (string _formData in formCollection)
        //    {
        //        if (_formData == "PageSize")
        //        {
        //            Int16 pageSize = Convert.ToInt16(formCollection[_formData]);
        //            ((FilterModel)(Session["storedevices"])).PageSize = pageSize;
        //            break;
        //        }
        //    }
        //    return View("Index", Session["storedevices"]);
        //}

         
        public void UpdateDb(FormCollection formCollection)
        {
            
            int i = 0;
            ImportDeviceViewModel _gridData = new ImportDeviceViewModel();
            foreach (string _formData in formCollection)
            {
                if (_formData == "location_" + i)
                {
                    _gridData.Location = formCollection[_formData];
                }
                else if (_formData == "rackShelf_" + i)
                {
                    _gridData.RackShelf = formCollection[_formData];
                }
                else if (_formData == "dcLocation_" + i)
                {
                    _gridData.DCLocation = formCollection[_formData];
                }
                else if (_formData == "customer_" + i)
                {
                    _gridData.Customer = formCollection[_formData];
                }
                else if (_formData == "serialNumber_" + i)
                {
                    _gridData.SerialNumber = formCollection[_formData];
                }
                else if (_formData == "model_" + i)
                {
                    _gridData.Model = formCollection[_formData];
                }
                else if (_formData == "useState_" + i)
                {
                    _gridData.UseState = formCollection[_formData];
                }
                else if (_formData == "localName_" + i)
                {
                    _gridData.LocalName = formCollection[_formData];
                }
                else if (_formData == "assetTag_" + i)
                {
                    _gridData.AssetTag = formCollection[_formData];
                    i++;
                    //gridDataList.Add(_gridData);
                    gridToDBRecordList.Add(_gridData);
                    _gridData = new ImportDeviceViewModel();
                }
            }
             
        }



        [HttpPost]
        public ActionResult Index(HttpPostedFileBase file, FormCollection formCollection, string pageSizeDDL, string saveToDB)
        {
            List<DeviceViewModel> devices = new List<DeviceViewModel>();
            if (file != null)
            {
                if (Request.Files["file"].ContentLength > 0)
                {
                    string fileLocation = Server.MapPath("~/Excel/") + Request.Files["file"].FileName;
                    Session.Add("fileLocation", fileLocation);

                    string fileExtension = System.IO.Path.GetExtension(fileLocation);
                    devices = ExtractDataFromExcel(fileLocation);
                    FilterModel fm = new FilterModel();
                    fm.DataModel = devices;
                    Session["storedevices"] = fm;
                }
            } 
            if(saveToDB == "Save to DB")
            { 
                if (formCollection.Count > 0 && formCollection != null)
                {
                    if (Session["storedevices"] == null) { return View("Index"); }

                    //foreach (string _formData in formCollection)
                    //{
                        //if (_formData == "PageSize")
                        //{
                        //    Int16 pageSize = Convert.ToInt16(formCollection[_formData]);
                        //    ((FilterModel)(Session["storedevices"])).PageSize = pageSize;
                        //    //break;
                        //}
                        //else if (_formData != "PageSize")
                        //{
                            UpdateDb(formCollection);
                        //}
                    //}
                }
            }
          else if (!string.IsNullOrEmpty(pageSizeDDL))
            {
                if (Session["storedevices"] == null) { return View("Index"); }

                Int16 pageSize = Convert.ToInt16(pageSizeDDL);
                ((FilterModel)(Session["storedevices"])).PageSize = pageSize;
                pageSizeDDL = null;
            }

            return View(Session["storedevices"]);
        }

        public static List<DeviceViewModel> ExtractDataFromExcel(string fileLocation)
        {
            List<DeviceViewModel> devices = new List<DeviceViewModel>();

            OleDbConnection oledbConn = new OleDbConnection();
            OleDbCommand cmd = new OleDbCommand();
            OleDbDataAdapter oleda = new OleDbDataAdapter();
            DataSet ds = new DataSet();

            int totalRowsCount = 0;
            string fileExtension = System.IO.Path.GetExtension(fileLocation);

            //need to pass relative path after deploying on server
            //string path = Request.Files["file"].FileName; //System.IO.Path.GetFullPath(Server.MapPath("~/InformationNew.xlsx"));
            /*connection string  to work with excel file. HDR=Yes - indicates 
            that the first row contains columnnames, not data. HDR=No - indicates 
            the opposite. "IMEX=1;" tells the driver to always read "intermixed" 
            (numbers, dates, strings etc) data columns as text. 
            Note that this option might affect excel sheet write access negative. */

            using (OleDbConnection connection = new OleDbConnection())
            {
                if (Path.GetExtension(fileExtension) == ".xls")
                {
                    connection.ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                }
                else if (Path.GetExtension(fileExtension) == ".xls")
                {
                    connection.ConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties='Excel 12.0;HDR=YES;IMEX=1;';";
                }

                connection.Open();

                string sql = "SELECT [location],[rack_shelf],[dc_location],[customer],[serialnumber],[model],[use_state],[localname],[asset_tag] FROM [device$]";

                using (OleDbCommand command = new OleDbCommand(sql, connection))
                {
                    try
                    {
                        command.CommandType = CommandType.Text;
                        oleda = new OleDbDataAdapter(command);
                        oleda.Fill(ds);

                        DataTable table = ds.Tables[0];
                        totalRowsCount = table.Rows.Count;
                        if (totalRowsCount > 0)
                        {
                            int i = 0;
                            foreach (DataRow row in table.Rows) // Loop over the rows.
                            {
                                DeviceViewModel device = new DeviceViewModel();
                                for (int j = 0; j < row.ItemArray.Count(); j++)
                                {
                                    device.Location = row.Table.Rows[i]["location"].ToString();
                                    device.RackShelf = row.Table.Rows[i]["rack_shelf"].ToString();
                                    device.DCLocation = row.Table.Rows[i]["dc_location"].ToString();
                                    device.Customer = row.Table.Rows[i]["customer"].ToString();
                                    device.SerialNumber = row.Table.Rows[i]["serialnumber"].ToString();
                                    device.Model = row.Table.Rows[i]["model"].ToString();
                                    device.UseState = row.Table.Rows[i]["use_state"].ToString();
                                    device.LocalName = row.Table.Rows[i]["localname"].ToString();
                                    device.AssetTag = row.Table.Rows[i]["asset_tag"].ToString();
                                    devices.Add(device);
                                    break;
                                }
                                i++;
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        // MessageBox.Show("Error: " + ex.Message);
                    }
                }
            }

            return devices;
        }

        public List<ImportDeviceViewModel> GridToDBRecordList { get; set; }
    }
}