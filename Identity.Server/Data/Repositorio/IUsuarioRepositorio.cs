using Identity.Server.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.Server.Data.Repositorio
{
    /// <summary>
    /// Implementar esta interface para bbdd de i3
    /// </summary>
    public interface IUsuarioRepositorio
    {
        bool ExisteUsername(string user);

        void SaveUSer(ApplicationUser user);
    }
}
