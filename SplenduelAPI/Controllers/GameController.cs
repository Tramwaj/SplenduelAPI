using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Splenduel.Core.Game.Model;
using Splenduel.Core.Game.Services;
using Splenduel.Interfaces.DTOs;
using Splenduel.Interfaces.VMs;
using SplenduelAPI.Hubs;
using System.Security.Claims;

namespace SplenduelAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GameController : ControllerBase
    {
        private readonly GameManager _gameManager;
        private readonly IHubContext<GameHub> _gameHub;

        //todo: remove dependency from Model (interfaces)
        public GameController(GameManager gameManager, IHubContext<GameHub> gameHub)
        {
            _gameManager = gameManager;
            _gameHub = gameHub;
        }

        [HttpGet("getGameState")]
        //[HttpGet("{id:guid}")]
        [Authorize]
        public async Task<ActionResult<GameStateVM>> GetGameState(Guid id)
        {
            string playerName = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(playerName)) return BadRequest("Something wrong with your name?");
            try
            {
                var gameData = await _gameManager.GetGameData(id, playerName);
                return Ok(gameData);
            }
            catch (Exception ex) { return BadRequest(ex); }
        }
        [HttpPost("SendAction")]
        [Authorize]
        public async Task<ActionResult> SendAction([FromBody] ActionFromJson action)
        {
            string? playerName = User.FindFirst(ClaimTypes.Name)?.Value;
            var actionn = action.Action;
            //string playerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(playerName)) return BadRequest("Something wrong with your name?");
            try
            {
                var gameData = await _gameManager.ResolveAction(actionn, playerName);
                return Ok();
            }
            catch (Exception ex) { return BadRequest(ex); }
        }

    }
}