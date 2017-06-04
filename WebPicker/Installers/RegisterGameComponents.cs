using Autofac;
using PickerGameModel.Entities;
using PickerGameModel.Entities.Game;
using PickerGameModel.Interfaces;
using PickerGameModel.Interfaces.Game;

namespace WebPicker.Installers
{
    public static class RegisterGameComponents
    {
        public static void InstallGame(this ContainerBuilder builder)
        {
            builder.RegisterType<DefaultGame>().As<IGame>();
            builder.RegisterType<GameRepository>().SingleInstance().As<IGameRepository>().As<GameRepository>();
        }
    }
}