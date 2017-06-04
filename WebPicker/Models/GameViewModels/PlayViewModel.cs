using System.ComponentModel.DataAnnotations;
using PickerGameModel.Interfaces.Game;

namespace WebPicker.Models.GameViewModels
{
    public class PlayViewModel
    {
        public int GameId { get; set; }
        public GameState GameState { get; set; }
        public int Lifes { get; set; }
        public string Result { get; set; }
        [Required]
        public int Number { get; set; }
    }
}
