using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using PickerGameModel.Exceptions;
using PickerGameModel.Interfaces.Game;
using PickerGameModel.Interfaces.Player;
using PickerGameModel.Interfaces.Settings;

namespace PickerGameModel.Entities.Game.Base
{
    public abstract class Game : IGame
    {
        private readonly Dictionary<IPlayer, int> _avaibleTurns;
        private readonly List<IPlayer> _participiants;
        private readonly List<IPlayer> _winners;
        private readonly Random _randomizer = new Random();

        private Timer _timeoutTimer;

        private int _currnetMovePlayerIndex;
        private int _secretNumber;

        public EventHandler<IPlayer> PlayerTurnChanged;
        public EventHandler<GameState> GameStateChanged;
        private GameState _gameState;

        protected Game()
        {
            GameId = GetHashCode();

            CreatedAt = DateTime.Now;

            _avaibleTurns = new Dictionary<IPlayer, int>();
            _participiants = new List<IPlayer>();
            _winners = new List<IPlayer>();

            GameState = GameState.WaitinForPlayers;

            //_timeoutTimer = new Timer(null, null, 0, 30);
        }

        #region Properties

        public abstract IGameSettings Settings { get; set; }

        public int GameId { get; }
        public DateTime CreatedAt { get; }
        public IPlayer[] Participiants => _participiants.ToArray();
        public IPlayer[] Winners => _winners.ToArray();
        public IPlayer Owner { get; private set; }
        public IPlayer PlayerMoving => _participiants[_currnetMovePlayerIndex];

        public GameState GameState
        {
            get { return _gameState; }
            private set
            {
                if (value != _gameState)
                {
                    _gameState = value;
                    GameStateChanged?.Invoke(this, _gameState);
                }
            }
        }

        #endregion

        #region Methods

        public void JoinPlayer(IPlayer player)
        {
            if (_participiants.Contains(player))
                throw new PlayerAlraedyRegisteredException();

            if (GameState != GameState.WaitinForPlayers && GameState != GameState.ReadyToPlay)
                throw new JoinGameStateException(GameState);

            if (_participiants.Count >= Settings.MaxPlayersAmount)
                throw new GameLobbyIsFullException();

            if (_participiants.Count == 0)
                Owner = player;

            _participiants.Add(player);

            UpdateGameStatus();
        }


        public void Start(IPlayer owner)
        {
            if (!owner.Equals(Owner))
                throw new GamePlayerAccessException();

            if (GameState == GameState.ReadyToPlay || GameState == GameState.LobbyIsFull)
                GameState = GameState.Running;
            else
                throw new GameIsNotReadyException(_participiants.Count, Settings.MinPlayersAmount);

            FillParticipiantsLifes();

            PickSecretNumber();

            NextPlayerMove(true);
        }

        private void FillParticipiantsLifes()
        {
            _avaibleTurns.Clear();
            foreach (var player in _participiants)
            {
                _avaibleTurns.Add(player, Settings.DefaultLifesAmount);
            }
        }

        public int GetLifes(IPlayer player)
        {
            return _avaibleTurns[player];
        }

        public void KickPlayer(IPlayer player)
        {
            if (GameState == GameState.Running)
                throw new GameViolationException();

            if (player.Equals(Owner))
                Owner = null;

            UpdateGameStatus();

            _participiants.Remove(player);
        }

        /// <summary>
        /// Make a player number pick
        /// </summary>
        /// <param name="caller">Player</param>
        /// <param name="move">Number</param>
        /// <returns>Hint: Aprox result of (Secret Number - move)</returns>
        public int Move(IPlayer caller, int move)
        {
            if (GameState != GameState.Running)
                throw new GameViolationException();

            if (!_participiants.Contains(caller))
                throw new GameViolationException();

            if (!caller.Equals(PlayerMoving))
                throw new TurnViolationException();

            if (GetLifes(caller) < 1)
                throw new PlayerDeadException();

            // TODO: Result Enums
            // TODO: TEST MORE
            int result;
            if (move > _secretNumber)
                result = 1;
            else if (move < _secretNumber)
                result = -1;
            else
            {
                GameState = GameState.Finished;
                _winners.Add(caller);
                return 0;
            }

            HitPlyaer(caller);
            NextPlayerMove();
            return result;
        }

        public void Reset(IPlayer owner)
        {
            if (!owner.Equals(Owner))
                throw new GamePlayerAccessException();

            if (GameState == GameState.Running && _participiants.Count > 0)
                throw new GameViolationException();

            GameState = GameState.WaitinForPlayers;
            _secretNumber = -1;
            _currnetMovePlayerIndex = -1;
            //_participiants.Clear();
            _winners.Clear();
            _avaibleTurns.Clear();

            UpdateGameStatus();
        }

        #endregion

        #region Private Methods

        private void UpdateGameStatus()
        {
            if (GameState != GameState.Running || GameState != GameState.Finished)
            {
                if (_participiants.Count >= Settings.MinPlayersAmount)
                    GameState = GameState.ReadyToPlay;
                if (_participiants.Count >= Settings.MaxPlayersAmount)
                    GameState = GameState.LobbyIsFull;
            }
            if (Owner == null)
                GameState = GameState.WaitinForPlayers;
        }

        private void PickSecretNumber()
        {
            _secretNumber = _randomizer.Next(Settings.MinSecretNumber, Settings.MaxSecretNumber);
        }

        private void NextPlayerMove(bool init = false)
        {
            if (_currnetMovePlayerIndex >= 0 && _currnetMovePlayerIndex < _participiants.Count - 1)
                _currnetMovePlayerIndex++;
            else
                _currnetMovePlayerIndex = 0;

            if (init)
                _currnetMovePlayerIndex = 0;

            if (_avaibleTurns[PlayerMoving] <= 0)
            {
                if (!IsAnyPlayerAlive())
                {
                    GameState = GameState.Draw;
                    return;
                }
            }

            PlayerTurnChanged?.Invoke(this, PlayerMoving);
        }

        private bool IsAnyPlayerAlive()
        {
            return _avaibleTurns.Any(x => x.Value > 0);
        }

        private void HitPlyaer(IPlayer player)
        {
            var value = _avaibleTurns[player];
            _avaibleTurns[player] = --value;
        }

        public int TellMeSecret(IPlayer player)
        {
            GetAccess(player);

            return _secretNumber;
        }

        public void SkipPlayerMove(IPlayer player)
        {
            GetAccess(player);

            NextPlayerMove();
        }

        #endregion

        private void GetAccess(IPlayer player)
        {
            var gm = player as IGameMaster;

            if (gm == null) throw new GamePlayerAccessException();

            if (!gm.Validate())
                throw new GameMasterValidationException();
        }
    }
}