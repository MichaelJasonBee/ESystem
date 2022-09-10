using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVoucherAndStoreAPI.Models
{
    public class UserConstant
    {
        public static List<UserModel> Users = new List<UserModel>()
        {
            new UserModel() { Username = "admin", EmailAddress = "jason.admin@email.com", Password = "123", GivenName = "Jason", Surname = "Bryant", Role = "Administrator" },
            new UserModel() { Username = "buyer", EmailAddress = "elyse.seller@email.com", Password = "123", GivenName = "Elyse", Surname = "Lambert", Role = "Buyer" },
        };
    }
}
