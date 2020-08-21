using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using StayLive.Models;

namespace StayLive.Helpers
{
    /// <summary>
    /// Summary description for DrawImage
    /// </summary>
    public class DrawImage : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            StayLiveEntities dbService = new StayLiveEntities();
            string type = "undefined";
            Byte[] Image = null;
            int id = 0;
            try
            {
                type = context.Request.QueryString["T"];
                switch (type)
                {
                    case "u":
                        id = int.Parse(context.Request.QueryString["Id"].ToString());
                        if (id == 0)
                            Image = GetBinaryImage(AppDomain.CurrentDomain.BaseDirectory + "/assets/images/anonymous.png");
                        else
                            Image = dbService.Users.Find(id).ProfilePhoto;
                        if (Image == null || Image.Length == 0)
                        Image = GetBinaryImage(AppDomain.CurrentDomain.BaseDirectory + "/assets/images/anonymous.png");
                        break;
                    case "c":
                        id = int.Parse(context.Request.QueryString["Id"].ToString());
                        if (id == 0)
                            Image = GetBinaryImage(AppDomain.CurrentDomain.BaseDirectory + "/assets/images/company_default.png");
                        else
                            Image = dbService.Companies.Find(id).Logo;
                        if (Image == null || Image.Length == 0)
                            Image = GetBinaryImage(AppDomain.CurrentDomain.BaseDirectory + "/assets/images/company_default.png");
                        break;
                }

                if (Image != null)
                    DrawImg(Image, "image/gif", context);
            }
            catch (Exception ex)
            {
                
            }
            finally
            {
                dbService.Dispose();
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        protected Byte[] GetBinaryImage(String Path)
        {
            BinaryReader Reader = new BinaryReader(new StreamReader(Path).BaseStream);
            if (Reader.BaseStream != null && Reader.BaseStream.CanRead)
            {
                return Reader.ReadBytes((int)Reader.BaseStream.Length);
            }
            return new Byte[0];
        }

        private Boolean DrawImg(Byte[] image, String imageType, HttpContext context)
        {
            if (image == null || image.Length == 0)
                return false;

            context.Response.Clear();

            context.Response.ContentType = imageType;
            context.Response.BinaryWrite(image);
            context.Response.Flush(); // Sends all currently buffered output to the client.
            context.Response.SuppressContent = true;  // Gets or sets a value indicating whether to send HTTP content to the client.
            context.ApplicationInstance.CompleteRequest(); // Causes ASP.NET to bypass all events and filtering in the HTTP pipeline chain of execution and directly execute the EndRequest event.
            return true;
        }
    }
}