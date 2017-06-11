using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PickerGameModel.Exceptions;
using PickerGameModel.Interfaces;
using WebPicker.Data;
using WebPicker.Models;
using WebPicker.Models.LobbyViewModels;
using AutoMapper;
using PickerGameModel.Interfaces.Services;
using WebPicker.Controllers.Base;
using WebPicker.Data.ADO.NET;
using WebPicker.Helpers;

namespace WebPicker.Controllers
{
    [Authorize]
    public class LobbyController : ControllerWithLogging
    {
        public LobbyController(IRepository<Log> logRepository, IRepository<IGameService> repository, ApplicationDbContext applicationDbContext, UserManager<ApplicationUser> userManager, ILogger logger) : base(logger)
        {
            var items = logRepository.Get();
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
        private IRepository<IGameService> GameRepository { get; }

        #endregion

        // GET: Game
        public ActionResult Index()
        {
            return View(GameRepository);
        }

        public async Task<IActionResult> Connect(int gameId)
        {
            var user = await UserManager.GetUserAsync(User);
            var game = GameRepository.Get(gameId);

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
                Logger.Log("Warning", user.ToString(), nameof(Connect), $"Connection attempt to game {gameId}", e.ToString());
            }
            catch (JoinGameStateException e)
            {
                Logger.Log("Error", user.ToString(), nameof(Connect), $"Connection attempt to game {gameId}", e.ToString());
                return BadRequest(e.Message);
            }

            return RedirectToAction("Details", new { gameId });
        }

        // GET: Game/Details/5
        public async Task<ActionResult> Details(int gameId)
        {
            var user = await UserManager.GetUserAsync(User);
            var game = GameRepository.Get(gameId);

            if (game == null)
            {
                return NotFound();
            }

            var gameDto = Mapper.Map<DetailsViewModel>(game.Game);
            Mapper.Map(game.Game.Settings, gameDto);

            gameDto.IsUserOwner = game.Game.Owner.Equals(user);

            return View(gameDto);
        }
    }
}