using PickerGameModel.Interfaces.Player;

namespace PickerGameModel.Entities.Player
{
    public class Player : IPlayer
    {
        public string PlayerId { get; set; }
        public string Nickname { get; set; }
    }
}
