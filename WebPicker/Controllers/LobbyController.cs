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

namespace WebPicker.Controllers
{
    [Authorize]
    public class LobbyController : Controller
    {
        public LobbyController(IGameRepository repository, ApplicationDbContext applicationDbContext, UserManager<ApplicationUser> userManager)
        {
            GameRepository = repository;
            ApplicationDbContext = applicationDbContext;
            UserManager = userManager;
        }

        #region Properties

        /// <summary>
        /// Application DB context
        /// </summary>
        protected ApplicationDbContext ApplicationDbContext { get; set; }

        /// <summary>
        /// User manager - attached to application DB context
        /// </summary>
        protected UserManager<ApplicationUser> UserManager { get; set; }

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

            if (game == null) return BadRequest("Game not found");

            try
            {
                game.JoinPlayer(user);
            }
            catch (PlayerAlraedyRegisteredException) { }
            catch (JoinGameStateException e)
            {
                return BadRequest(e.Message);
            }

            return RedirectToAction("Details", new { gameId });
        }

        // GET: Game/Details/5
        public async Task<ActionResult> Details(int gameId)
        {
            var game = GameRepository.Games.FirstOrDefault(x => x.GameId == gameId);

            if (game == null)
                return NotFound();

            var user = await UserManager.GetUserAsync(User);

            var dvm = new DetailsViewModel
            {
                GameId = game.GameId,
                CreatedAt = game.CreatedAt,
                GameState = game.GameState,
                MaxPlayersLimit = game.Settings.MaxPlayersAmount,
                PlayersAmout = game.Participiants.Length,
                Owner = game.Owner,
                IsUserOwner = game.Owner?.PlayerId == user.PlayerId
            };

            return View(dvm);
        }
    }
}