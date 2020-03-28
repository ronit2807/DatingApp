using System.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DatingApp.API.Data;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;
using DatingApp.API.Data;
using System.Threading.Tasks;
namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;

        public AuthController(IAuthRepository repo)
        {
            _repo = repo;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(string username,string password)
        {
            username=username.ToLower();
            if(await _repo.UserExists(username))
                return BadRequest("username already exists");

            var userToCreate=new User
            {
                Username=username
            };

            User createdUser=await _repo.Register(userToCreate,password);

            return StatusCode(201);
        }
    }
}