using System;
using Moq;
using PickerGameModel.Entities.Player;
using PickerGameModel.Entities.Services;
using PickerGameModel.Entities.Settings;
using PickerGameModel.Exceptions;
using PickerGameModel.Interfaces.Game;
using PickerGameModel.Interfaces.Player;
using PickerGameModel.Interfaces.Services;
using Xunit;

namespace GameUnitTests
{
    public class GameTests
    {
        [Fact]
        public void Should_place_new_player_in_participiants_on_join()
        {
            var gameService = new GameService();
            var playerMock = new Mock<IPlayer>();

            var player = playerMock.Object;

            gameService.JoinPlayer(player);

            Assert.Contains(player, gameService.Game.Participiants);
        }


        [Fact]
        public void Shouldnt_place_player_in_participiants_on_join_if_exist()
        {
            var gameService = new GameService();
            var playerMock = new Mock<IPlayer>();

            var player = playerMock.Object;

            gameService.JoinPlayer(player);

            Exception ex = Assert.Throws<PlayerAlraedyRegisteredException>(() => gameService.JoinPlayer(player));

            Assert.Same("You are already registered in the game", ex.Message);
        }

        [Fact]
        public void Should_be_owner_if_participiants_was_empty_on_join()
        {
            var gameService = new GameService();
            var playerMock = new Mock<IPlayer>();

            var player = playerMock.Object;

            gameService.JoinPlayer(player);

            Assert.Same(gameService.Game.Owner, player);
        }

        [Theory]
        [InlineData(3)]
        public void Should_have_expected_turn_attempts_on_start(int expectedAttempts)
        {
            var gameService = new GameService();
            var playerMock = new Mock<IPlayer>();

            var player = playerMock.Object;

            gameService.JoinPlayer(player);

            gameService.Start(player);

            Assert.Equal(expectedAttempts, gameService.GetLifes(player));
        }

        [Fact]
        public void Shouldnt_register_new_player_if_game_status_is_not_correct()
        {
            var gameService = new GameService();

            var player1 = new Mock<IPlayer>().Object;
            gameService.JoinPlayer(player1);

            gameService.Start(player1);

            var player2 = new Mock<IPlayer>().Object;

            Assert.Throws<JoinGameStateException>(() => gameService.JoinPlayer(player2));
        }

        [Fact]
        public void Should_change_status_after_join_if_required_player_amount_achived()
        {
            var gameService = new GameService();

            FillGameParticipiantsToPlayersAmount(gameService);

            Assert.Equal(gameService.Game.GameState, GameState.ReadyToPlay);
        }

        [Fact]
        public void Should_reset_game_if_caller_is_owner()
        {
            var gameService = new GameService();

            var player = new Mock<IPlayer>().Object;
            gameService.JoinPlayer(player);

            gameService.Reset(player);

            Assert.Equal(GameState.ReadyToPlay, gameService.Game.GameState);
        }

        [Fact]
        public void Shouldnt_reset_game_if_caller_is_not_owner()
        {
            var gameService = new GameService();

            var player1 = new Mock<IPlayer>().Object;
            gameService.JoinPlayer(player1);
            var player2 = new Mock<IPlayer>().Object;
            gameService.JoinPlayer(player2);

            Assert.Throws<GamePlayerAccessException>(() => gameService.Reset(player2));
        }

        [Fact]
        public void Shouldnt_reset_game_if_is_in_progress()
        {
            var gameService = new GameService();

            var player1 = new Mock<IPlayer>().Object;
            gameService.JoinPlayer(player1);
            var player2 = new Mock<IPlayer>().Object;
            gameService.JoinPlayer(player2);

            gameService.Start(player1);

            Assert.Throws<GameViolationException>(() => gameService.Reset(player1));
        }

        [Fact]
        public void Should_start_game_caller_is_owner()
        {
            var gameService = new GameService();

            var player = new Mock<IPlayer>().Object;
            gameService.JoinPlayer(player);

            gameService.Start(player);

            Assert.Equal(GameState.Running, gameService.Game.GameState);
        }

        [Fact]
        public void Shouldnt_start_game_caller_is_not_owner()
        {
            var gameService = new GameService();

            var player1 = new Mock<IPlayer>().Object;
            gameService.JoinPlayer(player1);
            var player2 = new Mock<IPlayer>().Object;
            gameService.JoinPlayer(player2);

            Assert.Throws<GamePlayerAccessException>(() => gameService.Start(player2));
        }

        [Fact]
        public void Should_start_game_if_required_player_amount_achived()
        {
            var gameService = new GameService();

            var owner = FillGameParticipiantsToPlayersAmount(gameService);

            gameService.Start(owner);

            Assert.Equal(gameService.Game.GameState, GameState.Running);
        }

        [Fact]
        public void Shouldnt_start_game_if_required_player_amount_is_not_achived()
        {
            var gameService = new GameService();

            var settings = new DefaultGameSettings() {MinPlayersAmount = 2};

            gameService.Game.Settings = settings;

            var player = new Mock<IPlayer>().Object;
            gameService.JoinPlayer(player);

            Assert.Throws<GameIsNotReadyException>(() => gameService.Start(player));
        }

        [Fact]
        public void Should_set_move_to_one_of_players_after_game_start()
        {
            var gameService = new GameService();

            var owner = FillGameParticipiantsToPlayersAmount(gameService);

            gameService.Start(owner);

            Assert.NotNull(gameService.PlayerMoving);
        }

        [Fact]
        public void Should_hit_player_on_fail()
        {
            var gameService = new GameService();

            var player = new Mock<IPlayer>().Object;
            gameService.JoinPlayer(player);

            gameService.Start(player);

            var lifesAtStart = gameService.GetLifes(player);

            gameService.Move(player, -1);

            var lifesAfterFail = gameService.GetLifes(player);

            Assert.True(lifesAtStart == ++lifesAfterFail);
        }

        [Fact]
        public void Should_set_player_moving_at_start()
        {
            var gameService = new GameService();

            var player1 = new Mock<IPlayer>().Object;
            gameService.JoinPlayer(player1);
            var player2 = new Mock<IPlayer>().Object;
            gameService.JoinPlayer(player2);

            gameService.Start(player1);

            Assert.Same(player1, gameService.PlayerMoving);
        }

        [Fact]
        public void Should_change_player_moving_after_each_move()
        {
            var gameService = new GameService();

            var player1 = new Mock<IPlayer>().Object;
            gameService.JoinPlayer(player1);
            var player2 = new Mock<IPlayer>().Object;
            gameService.JoinPlayer(player2);

            gameService.Start(player1);

            gameService.Move(player1, -1);

            Assert.Same(player2, gameService.PlayerMoving);
        }

        [Fact]
        public void Should_reset_player_moving_on_each_loop()
        {
            var gameService = new GameService();

            var player1 = new Mock<IPlayer>().Object;
            gameService.JoinPlayer(player1);
            var player2 = new Mock<IPlayer>().Object;
            gameService.JoinPlayer(player2);

            gameService.Start(player1);

            gameService.Move(player1, -1);
            gameService.Move(player2, -1);

            Assert.Same(player1, gameService.PlayerMoving);
        }

        [Fact]
        public void Should_throw_exception_if_another_player_moving()
        {
            var gameService = new GameService();

            var player1 = new Mock<IPlayer>().Object;
            gameService.JoinPlayer(player1);
            var player2 = new Mock<IPlayer>().Object;
            gameService.JoinPlayer(player2);

            gameService.Start(player1);

            gameService.Move(player1, -1);

            Assert.Throws<TurnViolationException>(() => gameService.Move(player1, -1));
        }

        [Fact]
        public void Should_kill_player_if_no_lifes()
        {
            var gameService = new GameService();

            var player1 = new Mock<IPlayer>().Object;
            gameService.JoinPlayer(player1);
            var player2 = new Mock<IPlayer>().Object;
            gameService.JoinPlayer(player2);

            gameService.Start(player1);

            var gm = new GameMaster("0", "test");

            for (int i = 0; i < gameService.Game.Settings.DefaultLifesAmount; i++)
            {
                gameService.Move(player1, -1);
                gameService.SkipPlayerMove(gm);
            }

            Assert.Throws<PlayerDeadException>(() => gameService.Move(player1, -1));
        }

        [Fact]
        public void Secret_shouldnt_be_same_with_type_deafult_value()
        {
            var gameService = new GameService();

            var player = new Mock<IPlayer>().Object;
            gameService.JoinPlayer(player);

            gameService.Start(player);

            var gm = new GameMaster("0", "test");
            int secret = gameService.TellMeSecret(gm);

            // Secret shouldn't be same as default type value
            Assert.NotEqual(default(int), secret);
        }

        [Fact]
        public void Should_create_secret_in_range()
        {
            for (var i = 0; i < 10; i++)
            {
                var gameService = new GameService();

                var player = new Mock<IPlayer>().Object;
                gameService.JoinPlayer(player);

                gameService.Start(player);

                var gm = new GameMaster("0", "test");
                var secret = gameService.TellMeSecret(gm);

                Assert.InRange(secret, gameService.Game.Settings.MinSecretNumber, gameService.Game.Settings.MaxSecretNumber);
            }
        }

        [Fact]
        public void Should_stop_game_if_somebody_picked_secret()
        {
            var gameService = new GameService();

            var player = new Mock<IPlayer>().Object;
            gameService.JoinPlayer(player);

            gameService.Start(player);

            var gm = new GameMaster("0", "test");
            var secret = gameService.TellMeSecret(gm);

            gameService.Move(player, secret);

            Assert.Equal(GameState.Finished, gameService.Game.GameState);
        }

        [Fact]
        public void Should_stop_game_if_nobody_has_lifes()
        {
            var gameService = new GameService();

            var player1 = new Mock<IPlayer>().Object;
            gameService.JoinPlayer(player1);
            var player2 = new Mock<IPlayer>().Object;
            gameService.JoinPlayer(player2);

            gameService.Start(player1);

            for (int i = 0; i < gameService.Game.Settings.DefaultLifesAmount; i++)
            {
                gameService.Move(player1, -1);
                gameService.Move(player2, -1);
            }

            Assert.Equal(GameState.Draw, gameService.Game.GameState);
        }

        [Fact]
        public void Should_return_BiggerMoveResult_if_secret_is_less()
        {
            var gameService = new GameService();

            var player1 = new Mock<IPlayer>().Object;
            gameService.JoinPlayer(player1);

            gameService.Start(player1);

            var gm = new GameMaster("0", "test");
            var secret = gameService.TellMeSecret(gm);

            var result = gameService.Move(player1, ++secret);

            Assert.Equal(1, result);
        }

        [Fact]
        public void Should_return_LessMoveResult_if_secret_is_less()
        {
            var gameService = new GameService();

            var player1 = new Mock<IPlayer>().Object;
            gameService.JoinPlayer(player1);

            gameService.Start(player1);

            var gm = new GameMaster("0", "test");
            var secret = gameService.TellMeSecret(gm);

            var result = gameService.Move(player1, --secret);

            Assert.Equal(-1, result);
        }

        [Fact]
        public void Should_return_WinMoveResult_if_secret_is_same()
        {
            var gameService = new GameService();

            var player1 = new Mock<IPlayer>().Object;
            gameService.JoinPlayer(player1);

            gameService.Start(player1);

            var gm = new GameMaster("0", "test");
            var secret = gameService.TellMeSecret(gm);

            var result = gameService.Move(player1, secret);

            Assert.Equal(0, result);
        }

        [Fact]
        public void Shouldnt_reject_user_on_TellMeSecret_if_have_access()
        {
            var gameService = new GameService();

            var player1 = new Mock<IPlayer>().Object;
            gameService.JoinPlayer(player1);

            gameService.Start(player1);

            var gm = new GameMaster("0", "test");
            var secret = gameService.TellMeSecret(gm);

            Assert.NotEqual(default(int), secret);
        }

        [Fact]
        public void Should_reject_user_on_TellMeSecret_if_dont_have_access()
        {
            var gameService = new GameService();

            var player1 = new Mock<IPlayer>().Object;
            gameService.JoinPlayer(player1);

            gameService.Start(player1);

            var player3 = new Mock<IPlayer>().Object;

            Assert.Throws<GamePlayerAccessException>(() => gameService.TellMeSecret(player3));
        }

        [Fact]
        public void Shouldnt_reject_user_on_SkipPlayerMove_if_have_access()
        {
            var gameService = new GameService();

            var player1 = new Mock<IPlayer>().Object;
            gameService.JoinPlayer(player1);
            var player2 = new Mock<IPlayer>().Object;
            gameService.JoinPlayer(player2);

            gameService.Start(player1);

            var firstPlayerMove = gameService.PlayerMoving;

            var gm = new GameMaster("0", "test");
            gameService.SkipPlayerMove(gm);

            var secondPlayerMove = gameService.PlayerMoving;

            Assert.NotSame(firstPlayerMove, secondPlayerMove);
        }

        [Fact]
        public void Should_reject_user_on_SkipPlayerMove_if_dont_have_access()
        {
            var gameService = new GameService();

            var player1 = new Mock<IPlayer>().Object;
            gameService.JoinPlayer(player1);
            var player2 = new Mock<IPlayer>().Object;
            gameService.JoinPlayer(player2);

            gameService.Start(player1);

            var player3 = new Mock<IPlayer>().Object;

            Assert.Throws<GamePlayerAccessException>(() => gameService.SkipPlayerMove(player3));
        }

        

        #region Helpers

        private IPlayer FillGameParticipiantsToPlayersAmount(IGameService gameService, int? playersAmount = null)
        {
            var requiredAmount = playersAmount ?? gameService.Game.Settings.MinPlayersAmount;
            IPlayer owner = null;
            for (int i = 0; i < requiredAmount; i++)
            {
                var newPlayer = new Mock<IPlayer>().Object;
                if (i == 0)
                    owner = newPlayer;
                gameService.JoinPlayer(newPlayer);
            }
            return owner;
        }

        private void FailPlayerTurns(IGameService gameService, IPlayer player, int? turnAmount = null)
        {
            for (int i = 0; i < gameService.Game.Settings.DefaultLifesAmount; i++)
                gameService.Move(player, -1);
        }

        private void DrawGame(IGameService gameService)
        {
            for (int i = 0; i < gameService.Game.Settings.DefaultLifesAmount; i++)
            {
                foreach (IPlayer player in gameService.Game.Participiants)
                {
                    gameService.Move(player, -1);
                }
            }
        }
        private IPlayer WinGame(IGameService gameService)
        {
            var gm = new GameMaster("0", "test");
            var secret = gameService.TellMeSecret(gm);

            var playerWon = gameService.PlayerMoving;

            gameService.Move(playerWon, secret);

            return playerWon;
        }

        #endregion
    }
}