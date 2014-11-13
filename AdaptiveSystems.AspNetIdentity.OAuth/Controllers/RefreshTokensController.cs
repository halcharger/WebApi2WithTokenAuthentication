using System.Threading.Tasks;
using System.Web.Http;

namespace AdaptiveSystems.AspNetIdentity.OAuth.Controllers
{
    [RoutePrefix("api/RefreshTokens")]
    public class RefreshTokensController : ApiController
    {
        private readonly OAuthRepository _repo;

        public RefreshTokensController()
        {
            _repo = new OAuthRepository();
        }

        [Authorize()]
        [Route("")]
        public async Task<IHttpActionResult> Get()
        {
            var tokens = await _repo.GetAllRefreshTokens();
            return Ok(tokens);
        }

        //[Authorize(Users = "Admin")]
        [AllowAnonymous]
        [Route("")]
        public async Task<IHttpActionResult> Delete(string tokenId)
        {
            var result = await _repo.RemoveRefreshToken(tokenId);
            if (result)
            {
                return Ok();
            }
            return BadRequest("Token Id does not exist");

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _repo.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}