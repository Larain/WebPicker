using PickerGameModel.Interfaces.Player;

namespace PickerGameModel.Entities.Player
{
    public class GameMaster : IGameMaster
    {
        public GameMaster(string playerId, string nickname)
        {
            PlayerId = playerId;
            Nickname = nickname;
        }

        public string PlayerId { get; }
        public string Nickname { get; }
        public bool Validate()
        {
            return true;
        }
    }
}