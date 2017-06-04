namespace PickerGameModel.Interfaces.Settings
{
    public interface IGameSettings
    {
        int MaxSecretNumber { get; }
        int MinSecretNumber { get; }
        int MaxPlayersAmount { get; }
        int MinPlayersAmount { get; }
        int DefaultLifesAmount { get; }
    }
}