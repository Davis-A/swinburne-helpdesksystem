﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Helpdesk.Common.Requests.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Helpdesk.Website.Controllers.api
{
    //[Authorize]
    [Route("api/users")]
    [ApiController]
    public class UsersController : BaseApiController
    {
        [HttpGet]
        [Route("{id}")]
        public IActionResult GetUser([FromRoute] int id)
        {
            if (!IsAuthorized())
                return Unauthorized();

            throw new NotImplementedException();
        }

        [HttpGet]
        [Route("")]
        public IActionResult GetUsers()
        {
            if (!IsAuthorized())
                return Unauthorized();

            throw new NotImplementedException();
        }

        [HttpPost]
        [Route("{id}")]
        public IActionResult AddUser([FromBody] AddUserRequest request)
        {
            if (!IsAuthorized())
                return Unauthorized();

            throw new NotImplementedException();
        }

        [HttpPatch]
        [Route("{id}")]
        public IActionResult UpdateUser([FromRoute] int id, [FromBody] UpdateUserRequest request)
        {
            if (!IsAuthorized())
                return Unauthorized();

            throw new NotImplementedException();
        }

        [HttpDelete]
        [Route("{id}")]
        public IActionResult DeleteUser ([FromRoute] int id)
        {
            if (!IsAuthorized())
                return Unauthorized();

            throw new NotImplementedException();
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (!IsAuthorized())
                return Unauthorized();

            throw new NotImplementedException();
        }
    }
}