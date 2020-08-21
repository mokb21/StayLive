using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StayLive.Helpers
{
    public enum MessageLocation
    {
        top_left = 1,
        top_right = 2,
        bottom_left = 3,
        bottom_right = 4,
        top_full = 5,
        bottom_full = 6
    }

    public enum MessageType
    {
        success = 1,
        error = 2,
        info = 3,
        warning = 4,
    }

    public sealed class MessageBg
    {

        private MessageBg() { }

        public const string success = "#5CB57C";
        public const string error = "#ff2b52";
        public const string info = "#1E88E5";
        public const string warning = "#FFB62B";
    }

    public static class InfoMessage
    {

        public static void MsgSuccess(this Controller controller, String title, String body)
        {
            message(MessageType.success, title, body, MessageBg.success, controller);
        }

        public static void MsgError(this Controller controller, String title, String body)
        {
            message(MessageType.error, title, body, MessageBg.error, controller);
        }

        public static void MsgInfo(this Controller controller, String title, String body)
        {
            message(MessageType.info, title, body, MessageBg.info, controller);
        }

        public static void MsgWarning(this Controller controller, String title, String body)
        {
            message(MessageType.warning, title, body, MessageBg.warning, controller);
        }

        public static void MsgSavedSuccessfuly(this Controller controller)
        {
            message(MessageType.success, Resources.General.Save, Resources.General.CompleteSuccessfuly, MessageBg.success, controller);
        }

        public static void MsgDeleteSuccessfuly(this Controller controller)
        {
            message(MessageType.success, Resources.General.Delete, Resources.General.DeletedSuccessfuly, MessageBg.success, controller);
        }

        public static void MsgNotValid(this Controller controller)
        {
            message(MessageType.error, "", "", MessageBg.error, controller);
        }

        public static void MsgNotFound(this Controller controller)
        {
            message(MessageType.error, "", "", MessageBg.error, controller);
        }

        private static void message(MessageType type, string title, string body, string color, Controller controller)
        {
            if (controller.TempData["im"] == null)
            {
                controller.TempData["im"] = "";
            }
            controller.TempData["im"] = "$.toast({heading: '" + title.Replace("'", "").Replace(Environment.NewLine, "") + "',text: '" + body.Replace("'", "").Replace(Environment.NewLine, "") + "',icon:'" + type.ToString() + "',bgColor:'" + color + "',position:'top-right',loaderBg: '#fff'});" + controller.TempData["im"].ToString();
        }
    }
}