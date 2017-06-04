using PickerGameModel.Interfaces.Settings;

namespace PickerGameModel.Entities.Settings
{
    public class DefaultGameSettings : IGameSettings
    {
        public int MaxSecretNumber { get; set; } = 10;
        public int MinSecretNumber { get; set; } = 1;
        public int MaxPlayersAmount { get; set; } = 3;
        public int MinPlayersAmount { get; set; } = 1;
        public int DefaultLifesAmount { get; set; } = 3;
    }
}