using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.DirectoryServices.AccountManagement;
namespace Funciones
{
    public class UsuariosFunciones
    {
        public static bool auth(string user, string password)
        {
            PrincipalContext principalcontext = new PrincipalContext(ContextType.Domain, "25.77.144.99");

            bool userValid = principalcontext.ValidateCredentials(user, password);
            if (userValid == true)
                Console.WriteLine("Autenticado");
            else
                Console.WriteLine("No autenticado");

            return userValid;
        }

        public static void register(string usuario, string password)
        {
            using (PrincipalContext principalContext = new PrincipalContext(ContextType.Domain, "25.77.144.99", "Administrador", "Una2021"))
            {
                using (UserPrincipal userPrincipal = new UserPrincipal(principalContext))
                {
                    userPrincipal.Name = usuario;
                    userPrincipal.SamAccountName = usuario;
                    userPrincipal.DisplayName = usuario;
                    userPrincipal.UserPrincipalName = usuario;
                    userPrincipal.SetPassword(password);
                    userPrincipal.Enabled = true;
                    userPrincipal.Save();
                }
            }
        }
    }
}
