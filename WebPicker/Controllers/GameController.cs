using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PickerGameModel.Exceptions;
using PickerGameModel.Interfaces;
using PickerGameModel.Interfaces.Services;
using WebPicker.Controllers.Base;
using WebPicker.Data;
using WebPicker.Helpers;
using WebPicker.Models;
using WebPicker.Models.GameViewModels;

namespace WebPicker.Controllers
{
    [Authorize]
    public class GameController : ControllerWithLogging
    {
        public GameController(IRepository<IGameService> repository, ApplicationDbContext applicationDbContext, UserManager<ApplicationUser> userManager, ILogger logger) : base(logger)
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
        private IRepository<IGameService> GameRepository { get; }

        #endregion

        // GET: Game
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> Start(int gameId)
        {
            var game = GameRepository.Get(gameId);

            if (game == null)
                return NotFound();

            var user = await UserManager.GetUserAsync(User);

            try
            {
                game.Start(user);
            }
            catch (GamePlayerAccessException e)
            {
                Logger.Log("Error", user.ToString(), nameof(Start), $"Start attempt of game {gameId}", e.ToString());
                return BadRequest(e.Message);
            }
            catch (GameIsNotReadyException e)
            {
                Logger.Log("Error", user.ToString(), nameof(Start), $"Start attempt of game {gameId}", e.ToString());
                return BadRequest(e.Message);
            }

            return RedirectToAction("Play", new { gameId });
        }
        public async Task<ActionResult> Reset(int gameId)
        {
            var game = GameRepository.Get(gameId);

            if (game == null)
                return NotFound();

            var user = await UserManager.GetUserAsync(User);

            try
            {
                game.Reset(user);
            }
            catch (GamePlayerAccessException e)
            {
                Logger.Log("Error", user.ToString(), nameof(Reset), $"Reset attempt of game {gameId}", e.ToString());
                return BadRequest(e.Message);
            }
            catch (GameViolationException e)
            {
                Logger.Log("Error", user.ToString(), nameof(Reset), $"Reset attempt of game {gameId}", e.ToString());
                return BadRequest(e.Message);
            }

            return RedirectToAction("Details", "Lobby", new { gameId });
        }

        public async Task<ActionResult> Play(int gameId, string turnResult = "")
        {
            var game = GameRepository.Get(gameId);

            if (game == null)
                return NotFound();

            var user = await UserManager.GetUserAsync(User);

            var pwm = new PlayViewModel
            {
                User = user,
                GameId = gameId,
                GameState = game.Game.GameState,
                Lifes = game.GetLifes(user),
                Result = turnResult
            };

            return View(pwm);
        }
        // POST: Game/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Play(PlayViewModel playViewModel)
        {
            if (ModelState.IsValid)
            {
                var game = GameRepository.Get(playViewModel.GameId);

                if (game == null)
                    return NotFound();

                //if (game.GameState == GameState.Finished || game.GameState == GameState.Draw)
                //    return RedirectToAction("Details", "Lobby", new {gameId = game.GameId});

                var user = await UserManager.GetUserAsync(User);

                string answer;
                try
                {
                    var result = game.Move(user, playViewModel.Number);
                    answer = ParseTurnResult(result, playViewModel.Number);
                }
                catch (GameViolationException e)
                {
                    Logger.Log("Error", user.ToString(), nameof(Reset), $"Play of game {playViewModel.GameId}", e.ToString());
                    return RedirectToAction("Index", "Lobby");
                }
                catch (TurnViolationException e)
                {
                    Logger.Log("Warning", user.ToString(), nameof(Reset), $"Play of game {playViewModel.GameId}", e.ToString());
                    answer = e.Message;
                }
                catch (PlayerDeadException e)
                {
                    Logger.Log("Error", user.ToString(), nameof(Reset), $"Play of game {playViewModel.GameId}", e.ToString());
                    return RedirectToAction("Index", "Lobby");
                }
                return RedirectToAction("Play", new { gameId = game.Game.GameId, turnResult = answer });
            }
            return BadRequest();
        }

        private string ParseTurnResult(int result, int move)
        {
            switch (result)
            {
                case -1:
                    return $"Your number {move} is lower than secret!";
                case 0:
                    return $"Congritilations! You Won! Secret number was {move}";
                case 1:
                    return $"Your number {move} is bigger than secret!";
                default:
                    return "Oh shit";
            }
        }

        // POST: Game/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}