using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PickerGameModel.Entities.Game;
using PickerGameModel.Entities.Game.Base;
using PickerGameModel.Exceptions;
using PickerGameModel.Interfaces;
using PickerGameModel.Interfaces.Game;
using WebPicker.Data;
using WebPicker.Models;
using WebPicker.Models.LobbyViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.Filters;
using PickerGameModel.Interfaces.Settings;
using WebPicker.Controllers.Base;
using WebPicker.Helpers;

namespace WebPicker.Controllers
{
    [Authorize]
    public class LobbyController : ControllerWithLogging
    {
        public LobbyController(IGameRepository repository, ApplicationDbContext applicationDbContext, UserManager<ApplicationUser> userManager, ILogger logger) : base(logger)
        {
            GameRepository = repository;
            ApplicationDbContext = applicationDbContext;
            UserManager = userManager;
        }

        #region Properties

        /// <summary>
        /// Application DB context
        /// </summary>
        private ApplicationDbContext ApplicationDbContext { get; set; }

        /// <summary>
        /// User manager - attached to application DB context
        /// </summary>
        private UserManager<ApplicationUser> UserManager { get; set; }

        /// <summary>
        /// List of active game rooms
        /// </summary>
        private IGameRepository GameRepository { get; }

        #endregion

        // GET: Game
        public ActionResult Index()
        {
            return View(GameRepository);
        }

        public async Task<IActionResult> Connect(int gameId)
        {
            var user = await UserManager.GetUserAsync(User);
            var game = GameRepository.Games.First(x => x.GameId == gameId);

            if (game == null)
            {
                return BadRequest("Game not found");
            }
            try
            {
                game.JoinPlayer(user);
            }
            catch (PlayerAlraedyRegisteredException e)
            {
                Logger.LogAsync("Warning", user.ToString(), nameof(Connect), $"Connection attempt to game {gameId}", e.ToString());
            }
            catch (JoinGameStateException e)
            {
                Logger.LogAsync("Error", user.ToString(), nameof(Connect), $"Connection attempt to game {gameId}", e.ToString());
                return BadRequest(e.Message);
            }

            return RedirectToAction("Details", new { gameId });
        }

        // GET: Game/Details/5
        public async Task<ActionResult> Details(int gameId)
        {
            var user = await UserManager.GetUserAsync(User);
            var game = GameRepository.Games.FirstOrDefault(x => x.GameId == gameId);

            if (game == null)
            {
                return NotFound();
            }

            var gameDto = Mapper.Map<IGame, DetailsViewModel>(game);
            Mapper.Map(game.Settings, gameDto);

            gameDto.IsUserOwner = game.Owner.Equals(user);

            return View(gameDto);
        }
    }
}