using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StayLive.Models;
using StayLive.Helpers;
using StayLive.Controllers;
using StayLive.areas.Levels.Level.Models;

namespace StayLive.areas.Levels.Controllers
{
    public class LevelController : BaseController
    {
        #region Action
        [RoleFilter(Role = areas.Users.Models.UserRoles.Admin)]
        public ActionResult Index()
        {
            LevelVM vm = new LevelVM();
            vm = FillLevelDetails();
            if (vm == null)
                return View("~/Views/Shared/NotFound.cshtml");

            return View(vm);
        }

        [HttpPost]
        [RoleFilter(Role = areas.Users.Models.UserRoles.Admin)]
        public ActionResult Index(LevelVM vm)
        {
            int Id = 0;
            var level = new StayLive.Models.Level();
            if (vm.Id >= 0)
            {
                level.Id = vm.Id;
                level.FirstName = vm.First;
                level.FirstHours = vm.FirstTime;
                level.SecondName = vm.Second;
                level.SecondHours = vm.SecondTime;
                level.ThirdName = vm.Third;
                Id = EditLevel(level);
            }

            if (Id == -1)
            {
                this.MsgError(StayLive.Resources.General.SaveLevel, StayLive.Resources.General.SomethingWentWorng);
                return View(vm);
            }
            else
            {
                this.MsgSavedSuccessfuly();
                return RedirectToAction("Index");
            }
        }
        #endregion

        #region GetterMethods
        [HttpGet]
        [RoleFilter(Role = areas.Users.Models.UserRoles.Admin)]
        public ActionResult LevelsTree()
        {
            var levelTree = new List<TreeModel>();

            var level = dbService.Levels.Where(a => a.CompanyId == SessionHelper.CompanyId.Value).FirstOrDefault();

            if (level != null)
            {
                levelTree.Add(new TreeModel() { id = "level1", text = level.FirstName, parent = "#", state = new NodeState() });
                levelTree.AddRange(GetUsersSubTree((byte)StayLive.areas.Levels.Models.LevelInfo.LevelOrder.First, "level1"));
                levelTree.Add(new TreeModel() { id = "level2", text = level.SecondName, parent = "#", state = new NodeState() });
                levelTree.AddRange(GetUsersSubTree((byte)StayLive.areas.Levels.Models.LevelInfo.LevelOrder.Second, "level2"));
                levelTree.Add(new TreeModel() { id = "level3", text = level.ThirdName, parent = "#", state = new NodeState() });
                levelTree.AddRange(GetUsersSubTree((byte)StayLive.areas.Levels.Models.LevelInfo.LevelOrder.Third, "level3"));
            }

            return Json(new { Data = levelTree }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region PrivateMethodes
        private LevelVM FillLevelDetails()
        {
            LevelVM vm = new LevelVM();
            var level = dbService.Levels.Where(a => a.CompanyId == SessionHelper.CompanyId.Value).FirstOrDefault();

            if (level == null)
                return null;

            vm.Id = level.Id;
            vm.First = level.FirstName;
            vm.FirstTime = level.FirstHours;
            vm.Second = level.SecondName;
            vm.SecondTime = level.SecondHours;
            vm.Third = level.ThirdName;
            vm.ThirdTime = null;
            return vm;
        }

        private int EditLevel(StayLive.Models.Level level)
        {
            try
            {
                var lvl = dbService.Levels.Find(level.Id);
                if (lvl != null)
                {
                    lvl.FirstName = level.FirstName;
                    lvl.FirstHours = level.FirstHours;
                    lvl.SecondName = level.SecondName;
                    lvl.SecondHours = level.SecondHours;
                    lvl.ThirdName = level.ThirdName;
                    dbService.SaveChanges();
                    return level.Id;
                }
                return -1;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        private List<TreeModel> GetUsersSubTree(byte LevelId,string FatherId)
        {
            var usersTree = new List<TreeModel>();
            var users = dbService.Users.Where(a => a.Level == LevelId && a.CompanyId == SessionHelper.CompanyId).ToList();

            usersTree.AddRange(users.Select(a => new TreeModel()
            {
                id = a.Id.ToString(),
                parent = FatherId,
                state = new NodeState(),
                text = a.Name
            }));

            usersTree = usersTree.OrderBy(a => a.text).ToList();
            return usersTree;
        }
        #endregion
    }
}
