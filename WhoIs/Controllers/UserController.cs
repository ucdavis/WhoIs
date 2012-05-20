using System;
using System.Linq;
using System.Web.Mvc;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Web.Controller;
using WhoIs.Models;
using WhoIs.Services;

namespace WhoIs.Controllers
{
    public class UserController : SuperController
    {
        private readonly IRepositoryWithTypedId<User, Guid> _userRepository;
        private readonly IUserSearchService _userSearchService;

        public UserController(IRepositoryWithTypedId<User,Guid> userRepository, IUserSearchService userSearchService)
        {
            _userRepository = userRepository;
            _userSearchService = userSearchService;
        }

        //
        // GET: /User/
        public ActionResult Index()
        {
            var users = _userRepository.GetAll();
            
            return View(users);
        }

        public ActionResult BuildIndex()
        {
            var users = _userRepository.GetAll();
            _userSearchService.BuildIndex(users.ToList());

            return RedirectToAction("Index");
        }

        public ActionResult Search(string id)
        {
            var results =  _userSearchService.Search("FirstName", id);

            return View(results);
        }
    }
}
