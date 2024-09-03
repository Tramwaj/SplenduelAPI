using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Splenduel.Core.Game;
using Splenduel.Core.Game.Services;
using Splenduel.Core.Game.Store;
using Splenduel.Core.Global;
using Splenduel.Core.Home;
using Splenduel.Core.Home.Interfaces;
using Splenduel.Core.Home.Model;
using SplenduelAPI.Hubs;
using System.Security.Claims;

namespace SplenduelAPI.Controllers
{
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly IHomeManager _homeManager;
        private readonly GameManager _gameManager;

        public HomeController(IHomeManager homeManager, GameManager gameManager)
        {
            this._homeManager = homeManager;
            this._gameManager = gameManager;
        }

        [HttpGet("test")]
        public async Task<IActionResult> Test()
        {
            return Ok("Ok");
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<HomeViewModel>> Get()
        {
            string user = User.FindFirst(ClaimTypes.Name)?.Value;
            var vm = await _homeManager.GetHomeViewModel(user);
            vm.UserName = user;
            return vm;
        }

        [HttpPost("invite")]
        [HttpPost("{invited}")]
        [HttpPost("{starts:bool}")]
        [Authorize]
        public async Task<IActionResult> CreateGameInvite(string invited, bool starts)
        {
            string? inviterUserName = User.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(inviterUserName))
            {
                return BadRequest();
            }
            var response = await _homeManager.CreateGameInvite(inviterUserName, invited, starts);
            if (!response.Success)
                return BadRequest(response.Message);
            else
                return Ok();
        }
        [HttpPost("acceptInvite")]
        [HttpPost("{id:Guid}")]
        [Authorize]
        public async Task<ActionResult<Guid>> AcceptGameInvite(Guid id)
        {
            Guid result;
            try
            {
                result = await this._homeManager.AcceptGameInvite(id);
                if (result == Guid.Empty) return BadRequest("Couldn't accept invite");
            }
            catch (Exception ex) { return BadRequest(ex); }
            return Ok(result);
        }
        [HttpPost("rejectInvite")]
        [HttpPost("{id:Guid}")]
        [Authorize]
        public async Task<IActionResult> RejectGameInvite(Guid id)
        {
            if (await this._homeManager.RejectGameInvite(id))
                return Ok();
            else
                return BadRequest("InviteId not found ");
        }
        //[Authorize]
        //[HttpGet("/notify")]
        //public async Task<IActionResult> Notify(string user, string message)
        //{
        //    var buffer = new byte[1024 * 4];
        //    //var webSocket = await Context
        //    return null;
        //}
    }
}
