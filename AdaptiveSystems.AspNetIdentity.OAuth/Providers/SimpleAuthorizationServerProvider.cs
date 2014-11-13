using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AdaptiveSystems.AspNetIdentity.OAuth.Entities;
using AdaptiveSystems.AspNetIdentity.OAuth.Models;
using log4net;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using NExtensions;

namespace AdaptiveSystems.AspNetIdentity.OAuth.Providers
{
    public class SimpleAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        private readonly ILog log = LogManager.GetLogger(typeof (SimpleAuthorizationServerProvider));

        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            string clientId;
            string clientSecret;
            Client client;

            log.Debug("Attempting to get basic credentials from query string");
            if (!context.TryGetBasicCredentials(out clientId, out clientSecret))
            {
                log.Debug("Attempting to get basic credentials from form collection");
                context.TryGetFormCredentials(out clientId, out clientSecret);
            }

            if (context.ClientId == null)
            {
                //Remove the comments from the below line context.SetError, and invalidate context 
                //if you want to force sending clientId/secrects once obtain access tokens. 
                log.Debug("Could not find client_id, validating context");
                context.Validated();
                //context.SetError("invalid_clientId", "ClientId should be sent.");
                return;
            }

            using (var repo = new OAuthRepository())
            {
                client = await repo.FindClient(context.ClientId);
            }

            if (client == null)
            {
                log.DebugFormat("Could not find client with id: {0}, returning 'invliad_clientId' Client {0} is not registered in the system", context.ClientId);
                context.SetError("invalid_clientId", string.Format("Client '{0}' is not registered in the system.", context.ClientId));
                return;
            }

            if (client.ApplicationType == ApplicationType.NativeConfidential)
            {
                if (clientSecret.IsNullOrWhiteSpace())
                {
                    log.Debug("Client secret is null or empty, returning 'invalid_clientSecret' Client secret should be sent"); 
                    context.SetError("invalid_clientSecret", "Client secret should be sent.");
                    return;
                }
                if (client.Secret != clientSecret.Hash())
                {
                    log.Debug("client secret not the same as hashed client secret, returning 'invalid_clientSecret' Client secret is invalid"); 
                    context.SetError("invalid_clientSecret", "Client secret is invalid.");
                    return;
                }
            }

            if (!client.Active)
            {
                log.Debug("Client is not active, returning 'invalid_clientId' Client is inactive");
                context.SetError("invalid_clientId", "Client is inactive.");
                return;
            }

            context.OwinContext.Set("as:clientAllowedOrigin", client.AllowedOrigin);
            context.OwinContext.Set("as:clientRefreshTokenLifeTime", client.RefreshTokenLifeTime.ToString());

            context.Validated();
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var allowedOrigin = context.OwinContext.Get<string>("as:clientAllowedOrigin") ?? "*";

            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });

            using (var repo = new OAuthRepository())
            {
                var user = await repo.FindUser(context.UserName, context.Password);

                if (user == null)
                {
                    log.DebugFormat("Could not find user: {0}, returning 'invalid_grant' the username or password is incorrect", context.UserName);
                    context.SetError("invalid_grant", "The user name or password is incorrect.");
                    return;
                }
            }

            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
            identity.AddClaim(new Claim("sub", context.UserName));
            identity.AddClaim(new Claim("role", "user"));

            var props = new AuthenticationProperties(new Dictionary<string, string>
                {
                    { 
                        "as:client_id", context.ClientId ?? string.Empty
                    },
                    { 
                        "userName", context.UserName
                    }
                });

            var ticket = new AuthenticationTicket(identity, props);
            context.Validated(ticket);

        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (var property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        public override Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
        {
            var originalClient = context.Ticket.Properties.Dictionary["as:client_id"];
            var currentClient = context.ClientId;


            if (originalClient != currentClient)
            {
                log.Debug("Original client not the same as current client, returning 'invalid_clientId' Refresh tokenb is issued to a different clientId");
                context.SetError("invalid_clientId", "Refresh token is issued to a different clientId.");
                return Task.FromResult(0);
            }

            // Change auth ticket for refresh token requests
            var newIdentity = new ClaimsIdentity(context.Ticket.Identity);
            //add/remove claims here if neccessary
            //newIdentity.AddClaim(new Claim("newClaim", "newValue"));

            var newTicket = new AuthenticationTicket(newIdentity, context.Ticket.Properties);
            context.Validated(newTicket);

            return Task.FromResult(0);
        }

    }
}