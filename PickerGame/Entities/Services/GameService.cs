using System;
using System.Linq;
using PickerGameModel.Entities.Game;
using PickerGameModel.Exceptions;
using PickerGameModel.Interfaces.Game;
using PickerGameModel.Interfaces.Player;
using PickerGameModel.Interfaces.Services;

namespace PickerGameModel.Entities.Services
{
    public class GameService : IGameService
    {
        private readonly Random _randomizer = new Random();

        private int _currnetMovePlayerIndex;
        public EventHandler<IPlayer> PlayerTurnChanged;
        public EventHandler<GameState> GameStateChanged;

        public IGame Game { get; set; }

        public GameService()
        {
            Game = new DefaultGame();
        }

        public IPlayer PlayerMoving => Game.Participiants[_currnetMovePlayerIndex];

        public void JoinPlayer(IPlayer player)
        {
            if (Game.Participiants.Contains(player))
                throw new PlayerAlraedyRegisteredException();

            if (Game.GameState != GameState.WaitinForPlayers && Game.GameState != GameState.ReadyToPlay)
                throw new JoinGameStateException(Game.GameState);

            if (Game.Participiants.Count >= Game.Settings.MaxPlayersAmount)
                throw new GameLobbyIsFullException();

            if (Game.Participiants.Count == 0)
                Game.Owner = player;

            Game.Participiants.Add(player);

            UpdateGameStatus();
        }


        public void Start(IPlayer owner)
        {
            if (!owner.Equals(Game.Owner))
                throw new GamePlayerAccessException();

            if (Game.GameState == GameState.ReadyToPlay || Game.GameState == GameState.LobbyIsFull)
                Game.GameState = GameState.Running;
            else
                throw new GameIsNotReadyException(Game.Participiants.Count, Game.Settings.MinPlayersAmount);

            FillParticipiantsLifes();

            PickSecretNumber();

            NextPlayerMove(true);
        }

        private void FillParticipiantsLifes()
        {
            Game.AvaibleTurns.Clear();
            foreach (var player in Game.Participiants)
            {
                Game.AvaibleTurns.Add(player, Game.Settings.DefaultLifesAmount);
            }
        }

        public int GetLifes(IPlayer player)
        {
            return Game.AvaibleTurns[player];
        }

        public void KickPlayer(IPlayer player)
        {
            if (Game.GameState == GameState.Running)
                throw new GameViolationException();

            if (player.Equals(Game.Owner))
                Game.Owner = null;

            UpdateGameStatus();

            Game.Participiants.Remove(player);
        }

        /// <summary>
        /// Make a player number pick
        /// </summary>
        /// <param name="caller">Player</param>
        /// <param name="move">Number</param>
        /// <returns>Hint: Aprox result of (Secret Number - move)</returns>
        public int Move(IPlayer caller, int move)
        {
            if (Game.GameState != GameState.Running)
                throw new GameViolationException();

            if (!Game.Participiants.Contains(caller))
                throw new GameViolationException();

            if (!caller.Equals(PlayerMoving))
                throw new TurnViolationException();

            if (GetLifes(caller) < 1)
                throw new PlayerDeadException();

            // TODO: Result Enums
            // TODO: TEST MORE
            int result;
            if (move > Game.SecretNumber)
                result = 1;
            else if (move < Game.SecretNumber)
                result = -1;
            else
            {
                Game.GameState = GameState.Finished;
                Game.Winners.Add(caller);
                return 0;
            }

            HitPlyaer(caller);
            NextPlayerMove();
            return result;
        }

        public void Reset(IPlayer owner)
        {
            if (!owner.Equals(Game.Owner))
                throw new GamePlayerAccessException();

            if (Game.GameState == GameState.Running && Game.Participiants.Count > 0)
                throw new GameViolationException();

            Game.GameState = GameState.WaitinForPlayers;
            Game.SecretNumber = -1;
            _currnetMovePlayerIndex = -1;
            //_participiants.Clear();
            Game.Winners.Clear();
            Game.AvaibleTurns.Clear();

            UpdateGameStatus();
        }

        private void UpdateGameStatus()
        {
            if (Game.GameState != GameState.Running || Game.GameState != GameState.Finished)
            {
                if (Game.Participiants.Count >= Game.Settings.MinPlayersAmount)
                    Game.GameState = GameState.ReadyToPlay;
                if (Game.Participiants.Count >= Game.Settings.MaxPlayersAmount)
                    Game.GameState = GameState.LobbyIsFull;
            }
            if (Game.Owner == null)
                Game.GameState = GameState.WaitinForPlayers;
        }

        private void PickSecretNumber()
        {
            Game.SecretNumber = _randomizer.Next(Game.Settings.MinSecretNumber, Game.Settings.MaxSecretNumber);
        }

        private void NextPlayerMove(bool init = false)
        {
            if (_currnetMovePlayerIndex >= 0 && _currnetMovePlayerIndex < Game.Participiants.Count - 1)
                _currnetMovePlayerIndex++;
            else
                _currnetMovePlayerIndex = 0;

            if (init)
                _currnetMovePlayerIndex = 0;

            if (Game.AvaibleTurns[PlayerMoving] <= 0)
            {
                if (!IsAnyPlayerAlive())
                {
                    Game.GameState = GameState.Draw;
                    return;
                }
            }

            PlayerTurnChanged?.Invoke(this, PlayerMoving);
        }

        private bool IsAnyPlayerAlive()
        {
            return Game.AvaibleTurns.Any(x => x.Value > 0);
        }

        private void HitPlyaer(IPlayer player)
        {
            var value = Game.AvaibleTurns[player];
            Game.AvaibleTurns[player] = --value;
        }

        public int TellMeSecret(IPlayer player)
        {
            GetAccess(player);

            return Game.SecretNumber;
        }

        public void SkipPlayerMove(IPlayer player)
        {
            GetAccess(player);

            NextPlayerMove();
        }

        private void GetAccess(IPlayer player)
        {
            var gm = player as IGameMaster;

            if (gm == null) throw new GamePlayerAccessException();

            if (!gm.Validate())
                throw new GameMasterValidationException();
        }
    }
}