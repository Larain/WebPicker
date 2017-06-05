using System.ComponentModel.DataAnnotations;
using PickerGameModel.Interfaces.Game;

namespace WebPicker.Models.GameViewModels
{
    public class PlayViewModel
    {
        public ApplicationUser User { get; set; }
        public int GameId { get; set; }
        public GameState GameState { get; set; }
        public int Lifes { get; set; }
        public string Result { get; set; }
        [Required]
        public int Number { get; set; }

        public override string ToString()
        {
            var message = "";

            message += $"User = {User?.Nickname}-{User?.PlayerId}+\n";
            message += $"GameId = {GameId}+\n";
            message += $"GameState = {GameState}+\n";
            message += $"Lifes = {Lifes}+\n";
            message += $"Result = {Result}+\n";
            message += $"Number = {Number}+\n";

            return message;
        }
    }
}
