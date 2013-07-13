﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AdoNet.Data;

namespace ADONET.WebApp
{
    public partial class RetrieveImagesFromSQL : System.Web.UI.Page
    {
        //private static void Main()
        //{
        //    SqlConnection dbConn = new SqlConnection(
        //        Settings.Default.DBConnectionString);
        //    dbConn.Open();
        //    using (dbConn)
        //    {
        //        SqlCommand cmd = new SqlCommand(
        //            "SELECT Picture FROM Categories ", dbConn);
        //        SqlDataReader reader = cmd.ExecuteReader();
        //        //using (reader)
        //        //{
        //        while (reader.Read())
        //        {
        //            byte[] imageFromDB = (byte[]) reader["Picture"];
        //            Console.WriteLine("Extracted first image from the DB.");
        //            WriteBinaryFile(DEST_IMAGE_FILE_NAME, imageFromDB);
        //            Console.WriteLine("Image saved to file {0}.",
        //                              DEST_IMAGE_FILE_NAME);
        //        }
        //        //}
        //    }
        //}

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Write(
                "Write a program that retrieves the images for all categories in the Northwind database and stores them as JPG files in the file system.<br>");

            RepairYourDatabase();

            string query = "SELECT Picture FROM [dbo].[Categories]";
            boFunctions.ExecuteSqlQueryReturnValue(query, null, CommandType.Text, delegate(SqlDataReader reader)
                {
                    List<string> images = new List<string>();
                    while (reader.Read())
                    {
                        var folder = Server.MapPath("\\Images\\Categories\\");
                        if (Helpers.DirectoryExist(folder))
                        {
                            var filePath = folder + images.Count + ".Jpeg";
                            byte[] data = (byte[]) reader["Picture"];
                            WriteBinaryFile(filePath, data);
                            images.Add(filePath);
                        }
                    }

                    grdResult.DataSource = images;
                    grdResult.DataBind();
                });
        }

        private void RepairYourDatabase()
        {
            var data = ReadBinaryFile(Server.MapPath(@"\Images\MainImage.png"));
            InsertImageToDB(data);
        }

        private void InsertImageToDB(byte[] data)
        {
            string query = "Update Categories";

            Dictionary<string, object> parametters = new Dictionary<string, object>();
            parametters.Add("Picture", data);

            boFunctions.ExecuteSqlQueryUpdate(query, parametters, delegate(SqlDataReader reader)
            {
                grdResult.DataSource = reader;
                grdResult.DataBind();
            });
        }

        private static byte[] ReadBinaryFile(string fileName)
        {
            FileStream stream = File.OpenRead(fileName);
            using (stream)
            {
                int pos = 0;
                int length = (int)stream.Length;
                byte[] buf = new byte[length];
                while (true)
                {
                    int bytesRead = stream.Read(buf, pos, length - pos);
                    if (bytesRead == 0)
                    {
                        break;
                    }
                    pos += bytesRead;
                }
                return buf;
            }
        }

        private static void WriteBinaryFile(string fileName, byte[] fileContents)
        {
            FileStream stream = File.OpenWrite(fileName);
            using (stream)
            {
                stream.Write(fileContents, 0, fileContents.Length);
            }
        }
    }
}