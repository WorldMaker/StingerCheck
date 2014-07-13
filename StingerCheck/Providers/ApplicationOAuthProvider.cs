/* 
 * OWIN Provider for Mozilla Persona <http://persona.org>.
 * 
 * This file is explicitly exempted from the project's reciprocal
 * license and presented for use in other projects under the license
 * described here.
 * 
 * Copyright 2014 Max Batther
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using System.Threading.Tasks;
using Microsoft.Owin.Security.OAuth;
using System.Net.Http;
using System.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Security.Claims;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security;
using System.Collections.Generic;

namespace StingerCheck.Providers
{
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        private readonly string _publicClientId;

        public ApplicationOAuthProvider(string publicClientId)
        {
            if (publicClientId == null)
            {
                throw new ArgumentNullException("publicClientId");
            }

            _publicClientId = publicClientId;
        }

        public async override Task GrantCustomExtension(OAuthGrantCustomExtensionContext context)
        {
            if (context.GrantType.Equals("persona", StringComparison.OrdinalIgnoreCase))
            {
                var http = new HttpClient() { BaseAddress = new Uri(ConfigurationManager.AppSettings["PersonaVerificationBaseUrl"]), };
                var body = await JsonConvert.SerializeObjectAsync(new
                {
                    assertion = (string)context.Parameters["assertion"],
                    audience = ConfigurationManager.AppSettings["PersonaAudienceUrl"],
                });
                var result = await http.PostAsync("verify", new StringContent(body, System.Text.Encoding.UTF8, "application/json"));

                var response = JObject.Parse(await result.Content.ReadAsStringAsync());

                var status = (string)response["status"];
                if (result.IsSuccessStatusCode && string.Equals(status, "okay", StringComparison.OrdinalIgnoreCase))
                {
                    var email = (string)response["email"];
                    var claims = new Claim[]
                    {
                        new Claim(ClaimTypes.Name, email),
                        new Claim(ClaimTypes.Email, email),
                        new Claim("persona-expires", (string)response["expires"]),
                        new Claim("persona-audience", (string)response["audience"]),
                        new Claim("persona-issuer", (string)response["issuer"]),
                        new Claim(ClaimTypes.AuthenticationMethod, "Persona"),
                    };
                    var oauthIdentity = new ClaimsIdentity(claims, context.Options.AuthenticationType);
                    var ticket = new AuthenticationTicket(oauthIdentity, new AuthenticationProperties(new Dictionary<string, string> {
                        {"email", email},
                    }));
                    context.Validated(ticket);
                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationType);
                    context.OwinContext.Authentication.SignIn(identity);
                    return;
                }

                context.SetError("invalid_grant", (string)response.SelectToken("reason"));
                return;
            }

            await base.GrantCustomExtension(context);
        }


        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            // Resource owner password credentials does not provide a client ID.
            if (context.ClientId == null)
            {
                context.Validated();
            }

            return Task.FromResult<object>(null);
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            if (context.ClientId == _publicClientId)
            {
                Uri expectedRootUri = new Uri(context.Request.Uri, "/");

                if (expectedRootUri.AbsoluteUri == context.RedirectUri)
                {
                    context.Validated();
                }
                else if (context.ClientId == "web")
                {
                    var expectedUri = new Uri(context.Request.Uri, "/");
                    context.Validated(expectedUri.AbsoluteUri);
                }
            }

            return Task.FromResult<object>(null);
        }
    }
}