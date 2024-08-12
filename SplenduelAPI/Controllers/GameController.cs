using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Splenduel.Core.Game.Model;
using Splenduel.Core.Game.Services;
using System.Security.Claims;

namespace SplenduelAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GameController : ControllerBase
    {
        readonly GameManager _gameManager;

        //todo: remove dependency from Model (interfaces)
        public GameController(GameManager gameManager)
        {
            _gameManager = gameManager;
        }

        [HttpGet("getGameState")]
        //[HttpGet("{id:guid}")]
        [Authorize]
        public async Task<ActionResult<GameState>> GetGameState(Guid id)
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

    }
}