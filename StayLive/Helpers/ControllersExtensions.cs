using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace StayLive.Helpers
{
    public static class ControllersExtensions
    {
        public static ViewResult NotFound(this Controller controller)
        {
            return new ViewResult { ViewName = "~/Views/Shared/NotFound.cshtml" };
        }

        public static Boolean IsValidImage(this HttpPostedFileBase image)
        {
            if (!(new string[] { ".png", ".jpg", ".jpeg" }).Contains((new System.IO.FileInfo(image.FileName)).Extension.ToLower()))
                return false;
            return true;
        }

        public static byte[] GetImageBytes(this HttpPostedFileBase image)
        {
            if (image != null)
            {
                using (var binaryReader = new System.IO.BinaryReader(image.InputStream))
                {
                    return binaryReader.ReadBytes(image.ContentLength);
                }
            }
            return null;
        }

        public static byte[] HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password)) return null;
            byte[] ret = new byte[0];
            byte[] bytes = Encoding.Unicode.GetBytes(password);
            SHA1 algorithm = SHA1Managed.Create();
            ret = algorithm.ComputeHash(bytes);
            return ret;
        }

        public static string RenderPartialToString(this Controller controller,string viewName,object model = null)
        {
            controller.ViewData.Model = model;

            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName);
                ViewContext viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }
    }
}