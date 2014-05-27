using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.Cookies;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;

namespace StingerCheck.Controllers
{
    [RoutePrefix("api/Persona")]
    public class PersonaController : ApiController
    {
        [HttpPost]
        [Route("logout")]
        public IHttpActionResult Logout()
        {
            var ctx = Request.GetOwinContext();
            ctx.Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
            return Ok();
        }
    }
}
