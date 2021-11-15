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
            var users = new List<Usuario>();
            users.Add(new Usuario { Id = 1, Username = "Homem-Aranha", Password = "ironman", Role = "employee" });
            users.Add(new Usuario { Id = 2, Username = "Peter Parker", Password = "tonystark", Role = "client" });
            return users.Where(x => x.Username.ToLower() == username.ToLower() && x.Password == password).FirstOrDefault();
        }
    }
}