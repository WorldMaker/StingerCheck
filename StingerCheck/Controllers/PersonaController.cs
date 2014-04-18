using Microsoft.AspNet.Identity;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;

namespace StingerCheck.Controllers
{
    public class PersonaController : ApiController
    {
        // From: http://brockallen.com/2013/10/24/a-primer-on-owin-cookie-authentication-middleware-for-the-asp-net-developer/
        public const string PersonaAuthenticationType = DefaultAuthenticationTypes.ApplicationCookie;

        [HttpPost]
        public async Task<IHttpActionResult> Login([FromBody]string assertion)
        {
            var http = new HttpClient() { BaseAddress = new Uri(Properties.Settings.Default.PersonaVerificationBaseUrl), };
            var result = await http.PostAsJsonAsync("verify", new { assertion, audience = Properties.Settings.Default.PersonaAudienceUrl, });

            if (!result.IsSuccessStatusCode)
            {
                return Unauthorized();
            }

            dynamic response = JObject.Parse(await result.Content.ReadAsStringAsync());

            if (string.Equals(response.status, "okay", StringComparison.OrdinalIgnoreCase))
            {
                var claims = new Claim[]
                {
                    new Claim(ClaimTypes.Name, response.email),
                    new Claim(ClaimTypes.Email, response.email),
                    new Claim("persona-expires", response.expires),
                    new Claim("persona-audience", response.audience),
                    new Claim("persona-issuer", response.issuer),
                    new Claim(ClaimTypes.AuthenticationMethod, "Persona"),
                };
                var identity = new ClaimsIdentity(claims, PersonaAuthenticationType);
                var ctx = Request.GetOwinContext();
                ctx.Authentication.SignIn(identity);
            }
            else if (string.Equals(response.status, "failure", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest((string)response.reason);
            }

            return BadRequest();
        }

        [HttpPost]
        public IHttpActionResult Logout()
        {
            var ctx = Request.GetOwinContext();
            ctx.Authentication.SignOut(PersonaAuthenticationType);
            return Ok();
        }
    }
}
