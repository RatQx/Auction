using Aukcionas.Auth.Model;
using Aukcionas.Data;
using Aukcionas.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Aukcionas.Controllers
{
    [ApiController]
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<ForumRestUser> _userManager;
        private readonly DataContext _dataContext;
        private readonly ILogger<AuthController> _logger;

        public AdminController(UserManager<ForumRestUser> userManager, ILogger<AuthController> logger, DataContext dataContext)
        {
            _userManager = userManager;
            _dataContext = dataContext;
            _logger = logger;
        }
    }
}
