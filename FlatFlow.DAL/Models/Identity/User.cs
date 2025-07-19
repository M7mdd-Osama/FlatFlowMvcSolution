using FlatFlow.DAL.Models.ApartmentModel;
using Microsoft.AspNetCore.Identity;

namespace FlatFlow.DAL.Models.Identity
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; } = null!;
        public string? LastName { get; set; }

        public ICollection<Apartment> Apartments { get; set; }
        public ICollection<Client> Clients { get; set; }
        public string? ResetPasswordCode { get; set; }
        public DateTime? ResetPasswordCodeExpiry { get; set; }

    }
}
