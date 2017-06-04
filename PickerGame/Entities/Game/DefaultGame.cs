using PickerGameModel.Entities.Settings;
using PickerGameModel.Interfaces.Settings;

namespace PickerGameModel.Entities.Game {
    public class DefaultGame : Base.Game
    {
        public override IGameSettings Settings { get; set; } = new DefaultGameSettings();
    }
}