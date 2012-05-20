using System;
using System.Web.Mvc;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Web.Controller;
using WhoIs.Models;

namespace WhoIs.Controllers
{
    public class UserController : SuperController
    {
        private readonly IRepositoryWithTypedId<User, Guid> _userRepository;

        public UserController(IRepositoryWithTypedId<User,Guid> userRepository)
        {
            _userRepository = userRepository;
        }

        //
        // GET: /User/
        public ActionResult Index()
        {
            var users = _userRepository.GetAll();
            
            return View(users);
        }

    }
}
