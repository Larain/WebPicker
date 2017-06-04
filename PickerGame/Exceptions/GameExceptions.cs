using System;
using PickerGameModel.Interfaces.Game;

namespace PickerGameModel.Exceptions
{
    public class JoinGameStateException : Exception
    {
        private readonly string _message;
        public JoinGameStateException(string message)
        {
            _message = message;
        }
        public JoinGameStateException(GameState state)
        {
            CurrentGameState = state;
        }
        public JoinGameStateException(string message, GameState state)
        {
            _message = message;
            CurrentGameState = state;
        }

        private GameState CurrentGameState { get; }
        public override string Message => string.IsNullOrEmpty(_message) ? $"Unavailable to join the game. Game state is {CurrentGameState}" : _message;
    }
    public class PlayerAlraedyRegisteredException : Exception
    {
        private readonly string _message;

        public PlayerAlraedyRegisteredException() { }
        public PlayerAlraedyRegisteredException(string message)
        {
            _message = message;
        }
        public override string Message => string.IsNullOrEmpty(_message) ? $"You are already registered in the game" : _message;
    }   
    public class TurnViolationException : Exception
    {
        private readonly string _message;

        public TurnViolationException() { }
        public TurnViolationException(string message)
        {
            _message = message;
        }
        public override string Message => string.IsNullOrEmpty(_message) ? $"Another player moving. Wait for your turn" : _message;
    }
    public class GameViolationException : Exception
    {
        private readonly string _message;

        public GameViolationException() { }

        public GameViolationException(string message)
        {
            _message = message;
        }
        public override string Message => string.IsNullOrEmpty(_message) ? $"You cannot interact with this game" : _message;
    }
    public class GameIsNotReadyException : Exception
    {
        private readonly string _message;

        public GameIsNotReadyException(string message)
        {
            _message = message;
        }
        public GameIsNotReadyException(int playersAmount, int playersRequired)
        {
            PlayersAmount = playersAmount;
            PlayersRequired = playersRequired;
        }
        public GameIsNotReadyException(string message, int playersAmount, int playersRequired)
        {
            _message = message;
            PlayersAmount = playersAmount;
            PlayersRequired = playersRequired;
        }
        int PlayersAmount { get; }
        int PlayersRequired { get; }
        public override string Message => string.IsNullOrEmpty(_message) ? $"Not enough players: {PlayersAmount}. Required {PlayersRequired}" : _message;

    }
    public class PlayerDeadException : Exception
    {
        private readonly string _message;

        public PlayerDeadException() { }
        public PlayerDeadException(string message)
        {
            _message = message;
        }
        public override string Message => string.IsNullOrEmpty(_message) ? $"You are dead, dude" : _message;

    }
    public class GamePlayerAccessException : Exception
    {
        private readonly string _message;

        public GamePlayerAccessException() { }
        public GamePlayerAccessException(string message)
        {
            _message = message;
        }
        public override string Message => string.IsNullOrEmpty(_message) ? $"You do not have permissions to do that" : _message;

    }
    public class GameMasterValidationException : Exception
    {
        private readonly string _message;

        public GameMasterValidationException() { }
        public GameMasterValidationException(string message)
        {
            _message = message;
        }
        public override string Message => string.IsNullOrEmpty(_message) ? $"Your account is not valid" : _message;

    }

    public class GameLobbyIsFullException : Exception
    {
        private readonly string _message;

        public GameLobbyIsFullException() { }
        public GameLobbyIsFullException(string message)
        {
            _message = message;
        }
        public override string Message => string.IsNullOrEmpty(_message) ? $"Max players number achived" : _message;

    }
    
}