using AutoMapper;
using PickerGameModel.Entities.Game.Base;
using PickerGameModel.Interfaces.Game;
using PickerGameModel.Interfaces.Settings;
using WebPicker.Models.LobbyViewModels;

namespace WebPicker.Helpers
{
    public static class AutoMapper
    {
        public static void MapViewModels()
        {
            Mapper.Initialize(cfg => {
                cfg.CreateMap<IGame, DetailsViewModel>();
                cfg.CreateMap<IGameSettings, DetailsViewModel>();
            });
        }
    }
}
