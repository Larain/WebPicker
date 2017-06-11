using Autofac;
using PickerGameModel.Entities;
using PickerGameModel.Entities.Game;
using PickerGameModel.Interfaces;
using PickerGameModel.Interfaces.Game;
using PickerGameModel.Interfaces.Services;
using WebPicker.Data.ADO.NET;
using WebPicker.Models;

namespace WebPicker.Helpers
{
    public static class DependencyInstallers
    {
        public static void InstallGame(this ContainerBuilder builder)
        {
            builder.RegisterType<DefaultGame>().As<IGame>();
            builder.RegisterType<GameRepository>().SingleInstance().As<IRepository<IGameService>>();
        }
        public static void InstallLogger(this ContainerBuilder builder)
        {
            builder.RegisterType<CustomLogger>().SingleInstance().As<ILogger>();
            builder.RegisterType<LogRepository>().As<IRepository<Log>>();
        }
    }
}