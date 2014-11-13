using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using AdaptiveSystems.AspNetIdentity.AzureTableStorage;
using AdaptiveSystems.AspNetIdentity.OAuth.Data;
using AdaptiveSystems.AspNetIdentity.OAuth.Entities;
using AdaptiveSystems.AspNetIdentity.OAuth.Models;
using Microsoft.AspNet.Identity;
using Microsoft.WindowsAzure.Storage.Table;

namespace AdaptiveSystems.AspNetIdentity.OAuth
{
    public class OAuthRepository : IDisposable
    {
        private readonly UserManager<User> _userManager;
        private readonly AzureTables _tables;

        public OAuthRepository()
        {
            var cs = ConfigurationManager.ConnectionStrings["UserStore-ConnectionString"].ConnectionString;
            _userManager = new UserManager<User>(new UserStore<User>(cs));
            _tables = new AzureTables();
        }

        public async Task<User> FindAsync(UserLoginInfo loginInfo)
        {
            var user = await _userManager.FindAsync(loginInfo);

            return user;
        }

        public async Task<IdentityResult> CreateAsync(User user)
        {
            var result = await _userManager.CreateAsync(user);

            return result;
        }

        public async Task<IdentityResult> AddLoginAsync(string userId, UserLoginInfo login)
        {
            var result = await _userManager.AddLoginAsync(userId, login);

            return result;
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

        public async Task<User> FindUser(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task UpdateUser(User user)
        {
            await _userManager.UpdateAsync(user);
        }

        public async Task<Client> FindClient(string clientId)
        {
            var result = await _tables.ClientsTable.ExecuteAsync(TableOperation.Retrieve<Client>(Client.PartitionKeyValue, clientId));

            return (Client)result.Result;
        }

        public async Task<bool> AddRefreshToken(RefreshToken token)
        {
            var existingTokens = await GetRefreshTokenBy(token.User, token.ClientId);

            if (existingTokens != null && existingTokens.Any())
            {
                await RemoveRefreshToken(existingTokens.First());
            }

            token.Timestamp = DateTime.UtcNow;
            token.ETag = "*";
            token.SetPartionAndRowKeys();
            var insertOperation = TableOperation.Insert(token);
            var result = await _tables.RefreshTokenTable.ExecuteAsync(insertOperation);

            return result != null;//TODO: need to understand what the result of a delete operation is in order to determine success or not
        }

        private async Task<IEnumerable<RefreshToken>> GetRefreshTokenBy(string user, string clientId)
        {
            var allRefreshTokens = await GetAllRefreshTokens();
            return allRefreshTokens.Where(rt => rt.User == user && rt.ClientId == clientId);
        }

        public async Task<bool> RemoveRefreshToken(string refreshTokenId)
        {
            var refreshToken = await FindRefreshToken(refreshTokenId);

            if (refreshToken != null)
            {
                return await RemoveRefreshToken(refreshToken);
            }

            return false;
        }

        public async Task<bool> RemoveRefreshToken(RefreshToken refreshToken)
        {
            var deleteOperation = TableOperation.Delete(refreshToken);
            var result = await _tables.RefreshTokenTable.ExecuteAsync(deleteOperation);
            return result != null;//TODO: need to understand what the result of a delete operation is in order to determine success or not
        }

        public async Task<RefreshToken> FindRefreshToken(string refreshTokenId)
        {
            var result = await _tables.RefreshTokenTable.ExecuteAsync(TableOperation.Retrieve<RefreshToken>(RefreshToken.PartitionKeyValue, refreshTokenId));

            return (RefreshToken)result.Result;
        }

        public Task<List<RefreshToken>> GetAllRefreshTokens()
        {
            return Task.Factory.StartNew(() =>
            {
                var tokens = _tables.RefreshTokenTable.ExecuteQuery(new TableQuery<RefreshToken>());
                return tokens.ToList();
            });
        }

        public void Dispose()
        {
            _userManager.Dispose();
        }
    }
}