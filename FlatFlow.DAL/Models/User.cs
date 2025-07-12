namespace FlatFlow.DAL.Models
{
        public class User : BaseEntity
        {
            public string UserName { get; set; }
            public string Email { get; set; }
            public string Pass { get; set; }

            public ICollection<Apartment> Apartments { get; set; }
            public ICollection<Client> Clients { get; set; }
        }
    }
