using System;
using Moq;
using PickerGameModel.Entities.Game;
using PickerGameModel.Entities.Player;
using PickerGameModel.Entities.Settings;
using PickerGameModel.Exceptions;
using PickerGameModel.Interfaces.Game;
using PickerGameModel.Interfaces.Player;
using Xunit;

namespace GameTests
{
    public class GameTests
    {
        [Fact]
        public void Should_place_new_player_in_participiants_on_join()
        {
            var game = new DefaultGame();
            var playerMock = new Mock<IPlayer>();

            var player = playerMock.Object;

            game.JoinPlayer(player);

            Assert.Contains(player, game.Participiants);
        }


        [Fact]
        public void Shouldnt_place_player_in_participiants_on_join_if_exist()
        {
            var game = new DefaultGame();
            var playerMock = new Mock<IPlayer>();

            var player = playerMock.Object;

            game.JoinPlayer(player);

            Exception ex = Assert.Throws<PlayerAlraedyRegisteredException>(() => game.JoinPlayer(player));

            Assert.Same("You are already registered in the game", ex.Message);
        }

        [Fact]
        public void Should_be_owner_if_participiants_was_empty_on_join()
        {
            var game = new DefaultGame();
            var playerMock = new Mock<IPlayer>();

            var player = playerMock.Object;

            game.JoinPlayer(player);

            Assert.Same(game.Owner, player);
        }

        [Theory]
        [InlineData(3)]
        public void Should_have_expected_turn_attempts_on_start(int expectedAttempts)
        {
            var game = new DefaultGame();
            var playerMock = new Mock<IPlayer>();

            var player = playerMock.Object;

            game.JoinPlayer(player);

            game.Start(player);

            Assert.Equal(expectedAttempts, game.GetLifes(player));
        }

        [Fact]
        public void Shouldnt_register_new_player_if_game_status_is_not_correct()
        {
            var game = new DefaultGame();

            var player1 = new Mock<IPlayer>().Object;
            game.JoinPlayer(player1);

            game.Start(player1);

            var player2 = new Mock<IPlayer>().Object;

            Assert.Throws<JoinGameStateException>(() => game.JoinPlayer(player2));
        }

        [Fact]
        public void Should_change_status_after_join_if_required_player_amount_achived()
        {
            var game = new DefaultGame();

            FillGameParticipiantsToPlayersAmount(game);

            Assert.Equal(game.GameState, GameState.ReadyToPlay);
        }

        [Fact]
        public void Should_reset_game_if_caller_is_owner()
        {
            var game = new DefaultGame();

            var player = new Mock<IPlayer>().Object;
            game.JoinPlayer(player);

            game.Reset(player);

            Assert.Equal(GameState.ReadyToPlay, game.GameState);
        }

        [Fact]
        public void Shouldnt_reset_game_if_caller_is_not_owner()
        {
            var game = new DefaultGame();

            var player1 = new Mock<IPlayer>().Object;
            game.JoinPlayer(player1);
            var player2 = new Mock<IPlayer>().Object;
            game.JoinPlayer(player2);

            Assert.Throws<GamePlayerAccessException>(() => game.Reset(player2));
        }

        [Fact]
        public void Shouldnt_reset_game_if_is_in_progress()
        {
            var game = new DefaultGame();

            var player1 = new Mock<IPlayer>().Object;
            game.JoinPlayer(player1);
            var player2 = new Mock<IPlayer>().Object;
            game.JoinPlayer(player2);

            game.Start(player1);

            Assert.Throws<GameViolationException>(() => game.Reset(player1));
        }

        [Fact]
        public void Should_start_game_caller_is_owner()
        {
            var game = new DefaultGame();

            var player = new Mock<IPlayer>().Object;
            game.JoinPlayer(player);

            game.Start(player);

            Assert.Equal(GameState.Running, game.GameState);
        }

        [Fact]
        public void Shouldnt_start_game_caller_is_not_owner()
        {
            var game = new DefaultGame();

            var player1 = new Mock<IPlayer>().Object;
            game.JoinPlayer(player1);
            var player2 = new Mock<IPlayer>().Object;
            game.JoinPlayer(player2);

            Assert.Throws<GamePlayerAccessException>(() => game.Start(player2));
        }

        [Fact]
        public void Should_start_game_if_required_player_amount_achived()
        {
            var game = new DefaultGame();

            var owner = FillGameParticipiantsToPlayersAmount(game);

            game.Start(owner);

            Assert.Equal(game.GameState, GameState.Running);
        }

        [Fact]
        public void Shouldnt_start_game_if_required_player_amount_is_not_achived()
        {
            var game = new DefaultGame();

            var settings = new DefaultGameSettings {MinPlayersAmount = 2};

            game.Settings = settings;

            var player = new Mock<IPlayer>().Object;
            game.JoinPlayer(player);

            Assert.Throws<GameIsNotReadyException>(() => game.Start(player));
        }

        [Fact]
        public void Should_set_move_to_one_of_players_after_game_start()
        {
            var game = new DefaultGame();

            var owner = FillGameParticipiantsToPlayersAmount(game);

            game.Start(owner);

            Assert.NotNull(game.PlayerMoving);
        }

        [Fact]
        public void Should_hit_player_on_fail()
        {
            var game = new DefaultGame();

            var player = new Mock<IPlayer>().Object;
            game.JoinPlayer(player);

            game.Start(player);

            var lifesAtStart = game.GetLifes(player);

            game.Move(player, -1);

            var lifesAfterFail = game.GetLifes(player);

            Assert.True(lifesAtStart == ++lifesAfterFail);
        }

        [Fact]
        public void Should_set_player_moving_at_start()
        {
            var game = new DefaultGame();

            var player1 = new Mock<IPlayer>().Object;
            game.JoinPlayer(player1);
            var player2 = new Mock<IPlayer>().Object;
            game.JoinPlayer(player2);

            game.Start(player1);

            Assert.Same(player1, game.PlayerMoving);
        }

        [Fact]
        public void Should_change_player_moving_after_each_move()
        {
            var game = new DefaultGame();

            var player1 = new Mock<IPlayer>().Object;
            game.JoinPlayer(player1);
            var player2 = new Mock<IPlayer>().Object;
            game.JoinPlayer(player2);

            game.Start(player1);

            game.Move(player1, -1);

            Assert.Same(player2, game.PlayerMoving);
        }

        [Fact]
        public void Should_reset_player_moving_on_each_loop()
        {
            var game = new DefaultGame();

            var player1 = new Mock<IPlayer>().Object;
            game.JoinPlayer(player1);
            var player2 = new Mock<IPlayer>().Object;
            game.JoinPlayer(player2);

            game.Start(player1);

            game.Move(player1, -1);
            game.Move(player2, -1);

            Assert.Same(player1, game.PlayerMoving);
        }

        [Fact]
        public void Should_throw_exception_if_another_player_moving()
        {
            var game = new DefaultGame();

            var player1 = new Mock<IPlayer>().Object;
            game.JoinPlayer(player1);
            var player2 = new Mock<IPlayer>().Object;
            game.JoinPlayer(player2);

            game.Start(player1);

            game.Move(player1, -1);

            Assert.Throws<TurnViolationException>(() => game.Move(player1, -1));
        }

        [Fact]
        public void Should_kill_player_if_no_lifes()
        {
            var game = new DefaultGame();

            var player1 = new Mock<IPlayer>().Object;
            game.JoinPlayer(player1);
            var player2 = new Mock<IPlayer>().Object;
            game.JoinPlayer(player2);

            game.Start(player1);

            var gm = new GameMaster("0", "test");

            for (int i = 0; i < game.Settings.DefaultLifesAmount; i++)
            {
                game.Move(player1, -1);
                game.SkipPlayerMove(gm);
            }

            Assert.Throws<PlayerDeadException>(() => game.Move(player1, -1));
        }

        [Fact]
        public void Secret_shouldnt_be_same_with_type_deafult_value()
        {
            var game = new DefaultGame();

            var player = new Mock<IPlayer>().Object;
            game.JoinPlayer(player);

            game.Start(player);

            var gm = new GameMaster("0", "test");
            int secret = game.TellMeSecret(gm);

            // Secret shouldn't be same as default type value
            Assert.NotEqual(default(int), secret);
        }

        [Fact]
        public void Should_create_secret_in_range()
        {
            for (var i = 0; i < 10; i++)
            {
                var game = new DefaultGame();

                var player = new Mock<IPlayer>().Object;
                game.JoinPlayer(player);

                game.Start(player);

                var gm = new GameMaster("0", "test");
                var secret = game.TellMeSecret(gm);

                Assert.InRange(secret, game.Settings.MinSecretNumber, game.Settings.MaxSecretNumber);
            }
        }

        [Fact]
        public void Should_stop_game_if_somebody_picked_secret()
        {
            var game = new DefaultGame();

            var player = new Mock<IPlayer>().Object;
            game.JoinPlayer(player);

            game.Start(player);

            var gm = new GameMaster("0", "test");
            var secret = game.TellMeSecret(gm);

            game.Move(player, secret);

            Assert.Equal(GameState.Finished, game.GameState);
        }

        [Fact]
        public void Should_stop_game_if_nobody_has_lifes()
        {
            var game = new DefaultGame();

            var player1 = new Mock<IPlayer>().Object;
            game.JoinPlayer(player1);
            var player2 = new Mock<IPlayer>().Object;
            game.JoinPlayer(player2);

            game.Start(player1);

            for (int i = 0; i < game.Settings.DefaultLifesAmount; i++)
            {
                game.Move(player1, -1);
                game.Move(player2, -1);
            }

            Assert.Equal(GameState.Draw, game.GameState);
        }

        [Fact]
        public void Should_return_BiggerMoveResult_if_secret_is_less()
        {
            var game = new DefaultGame();

            var player1 = new Mock<IPlayer>().Object;
            game.JoinPlayer(player1);

            game.Start(player1);

            var gm = new GameMaster("0", "test");
            var secret = game.TellMeSecret(gm);

            var result = game.Move(player1, ++secret);

            Assert.Equal(1, result);
        }

        [Fact]
        public void Should_return_LessMoveResult_if_secret_is_less()
        {
            var game = new DefaultGame();

            var player1 = new Mock<IPlayer>().Object;
            game.JoinPlayer(player1);

            game.Start(player1);

            var gm = new GameMaster("0", "test");
            var secret = game.TellMeSecret(gm);

            var result = game.Move(player1, --secret);

            Assert.Equal(-1, result);
        }

        [Fact]
        public void Should_return_WinMoveResult_if_secret_is_same()
        {
            var game = new DefaultGame();

            var player1 = new Mock<IPlayer>().Object;
            game.JoinPlayer(player1);

            game.Start(player1);

            var gm = new GameMaster("0", "test");
            var secret = game.TellMeSecret(gm);

            var result = game.Move(player1, secret);

            Assert.Equal(0, result);
        }

        [Fact]
        public void Shouldnt_reject_user_on_TellMeSecret_if_have_access()
        {
            var game = new DefaultGame();

            var player1 = new Mock<IPlayer>().Object;
            game.JoinPlayer(player1);

            game.Start(player1);

            var gm = new GameMaster("0", "test");
            var secret = game.TellMeSecret(gm);

            Assert.NotEqual(default(int), secret);
        }

        [Fact]
        public void Should_reject_user_on_TellMeSecret_if_dont_have_access()
        {
            var game = new DefaultGame();

            var player1 = new Mock<IPlayer>().Object;
            game.JoinPlayer(player1);

            game.Start(player1);

            var player3 = new Mock<IPlayer>().Object;

            Assert.Throws<GamePlayerAccessException>(() => game.TellMeSecret(player3));
        }

        [Fact]
        public void Shouldnt_reject_user_on_SkipPlayerMove_if_have_access()
        {
            var game = new DefaultGame();

            var player1 = new Mock<IPlayer>().Object;
            game.JoinPlayer(player1);
            var player2 = new Mock<IPlayer>().Object;
            game.JoinPlayer(player2);

            game.Start(player1);

            var firstPlayerMove = game.PlayerMoving;

            var gm = new GameMaster("0", "test");
            game.SkipPlayerMove(gm);

            var secondPlayerMove = game.PlayerMoving;

            Assert.NotSame(firstPlayerMove, secondPlayerMove);
        }

        [Fact]
        public void Should_reject_user_on_SkipPlayerMove_if_dont_have_access()
        {
            var game = new DefaultGame();

            var player1 = new Mock<IPlayer>().Object;
            game.JoinPlayer(player1);
            var player2 = new Mock<IPlayer>().Object;
            game.JoinPlayer(player2);

            game.Start(player1);

            var player3 = new Mock<IPlayer>().Object;

            Assert.Throws<GamePlayerAccessException>(() => game.SkipPlayerMove(player3));
        }

        

        #region Helpers

        private IPlayer FillGameParticipiantsToPlayersAmount(IGame game, int? playersAmount = null)
        {
            var requiredAmount = playersAmount ?? game.Settings.MinPlayersAmount;
            IPlayer owner = null;
            for (int i = 0; i < requiredAmount; i++)
            {
                var newPlayer = new Mock<IPlayer>().Object;
                if (i == 0)
                    owner = newPlayer;
                game.JoinPlayer(newPlayer);
            }
            return owner;
        }

        private void FailPlayerTurns(IGame game, IPlayer player, int? turnAmount = null)
        {
            for (int i = 0; i < game.Settings.DefaultLifesAmount; i++)
                game.Move(player, -1);
        }

        private void DrawGame(IGame game)
        {
            for (int i = 0; i < game.Settings.DefaultLifesAmount; i++)
            {
                foreach (IPlayer player in game.Participiants)
                {
                    game.Move(player, -1);
                }
            }
        }
        private IPlayer WinGame(IGame game)
        {
            var gm = new GameMaster("0", "test");
            var secret = game.TellMeSecret(gm);

            var playerWon = game.PlayerMoving;

            game.Move(playerWon, secret);

            return playerWon;
        }

        #endregion
    }
}