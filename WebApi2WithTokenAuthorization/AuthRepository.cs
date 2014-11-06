using System;
using System.Configuration;
using System.Threading.Tasks;
using AdaptiveSystems.AspNetIdentity.AzureTableStorage;
using Microsoft.AspNet.Identity;
using WebApi2WithTokenAuthorization.Models;

namespace WebApi2WithTokenAuthorization
{
    public class AuthRepository : IDisposable
    {
        private readonly UserManager<User> _userManager;

        public AuthRepository()
        {
            var cs = ConfigurationManager.ConnectionStrings["UserStore-ConnectionString"].ConnectionString;
            _userManager = new UserManager<User>(new UserStore<User>(cs));
        }

        public async Task<IdentityResult> RegisterUser(UserModel userModel)
        {
            var user = new User
            {
                UserName = userModel.UserName, 
                Email = userModel.Email
            };

            return await _userManager.CreateAsync(user, userModel.Password);
        }

        public async Task<User> FindUser(string userName, string password)
        {
            return await _userManager.FindAsync(userName, password);
        }

        public void Dispose()
        {
            _userManager.Dispose();
        }
    }
}