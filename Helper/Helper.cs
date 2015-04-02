using ImportExcel_v1.Models;
 
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Web;

namespace Midrange.Helper
{
    public class Helper
    {
        public List<ImportDeviceViewModel> ExtractDataFromExcel(string fileLocation)
        {
            List<ImportDeviceViewModel> devices = new List<ImportDeviceViewModel>();
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
                        //if (ds.Tables[0].DefaultView.Count > 0)
                        {
                            int i = 0;
                            foreach (DataRow row in table.Rows) // Loop over the rows.
                            {
                                ImportDeviceViewModel device = new ImportDeviceViewModel();
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
    }
}