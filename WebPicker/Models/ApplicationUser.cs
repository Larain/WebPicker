using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using PickerGameModel.Interfaces.Player;

namespace WebPicker.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public sealed class ApplicationUser : IdentityUser, IPlayer, IEquatable<IPlayer>, IEquatable<ApplicationUser>
    {
        public ApplicationUser()
        {
            PlayerId = Id;
        }
        public string Nickname { get; set; }
        public string PlayerId { get; private set; }
        public bool Equals(IPlayer other)
        {
            return other.PlayerId == PlayerId;
        }

        public bool Equals(ApplicationUser other)
        {
            return other.PlayerId == PlayerId;
        }

        public override bool Equals(Object obj)
        {
            var other = obj as IPlayer;
            if (other == null) return false;

            return other.PlayerId == PlayerId;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Nickname != null ? Nickname.GetHashCode() : 0) * 397) ^ (PlayerId != null ? PlayerId.GetHashCode() : 0);
            }
        }
    }
}
