using System.Data;
using AutoMapper;
using PickerGameModel.Interfaces.Game;
using PickerGameModel.Interfaces.Settings;
using WebPicker.Models;
using WebPicker.Models.LobbyViewModels;

namespace WebPicker.Helpers
{
    public static class AutoMapper
    {
        public static void Initialize()
        {
            Mapper.Initialize(cfg =>
            {
                ConfigViewModels(cfg);
                ConfigLogsSqlReader(cfg);
            });
        }
        private static void ConfigViewModels(IProfileExpression cfg)
        {
            cfg.CreateMap<IGame, DetailsViewModel>();
            cfg.CreateMap<IGameSettings, DetailsViewModel>();
        }
        private static void ConfigLogsSqlReader(IProfileExpression cfg)
        {
            cfg.CreateMap<IDataReader, Log>()
                .ForMember(log => log.Id, mapper => mapper.MapFrom(dr => dr.GetInt32(0)))
                .ForMember(log => log.Thread, mapper => mapper.MapFrom(dr => dr.GetString(2)))
                .ForMember(log => log.Level, mapper => mapper.MapFrom(dr => dr.GetString(3)))
                .ForMember(log => log.Logger, mapper => mapper.MapFrom(dr => dr.IsDBNull(4) ? null : dr.GetString(4)))
                .ForMember(log => log.User, mapper => mapper.MapFrom(dr => dr.IsDBNull(5) ? null : dr.GetString(5)))
                .ForMember(log => log.Action, mapper => mapper.MapFrom(dr => dr.IsDBNull(6) ? null : dr.GetString(6)))
                .ForMember(log => log.Message, mapper => mapper.MapFrom(dr => dr.GetString(7)))
                .ForMember(log => log.Exception, mapper => mapper.MapFrom(dr => dr.IsDBNull(8) ? null : dr.GetString(8)));
        }
    }
}
