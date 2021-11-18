using LetsShop.Repository.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LetsShop.Repository
{
    public static class UsuarioRepository
    {
        public static Usuario Get(string username, string password)
        {
            var users = new List<Usuario>
            {
                new Usuario { Id = 1, Username = "Homem-Aranha", Password = "ironman", Role = "funcionario" },
                new Usuario { Id = 2, Username = "Peter Parker", Password = "tonystark", Role = "cliente" }
            };
            return users.Find(x => string.Equals(x.Username, username, StringComparison.OrdinalIgnoreCase) && x.Password == password);
        }
    }
}