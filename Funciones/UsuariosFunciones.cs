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
        public static void auth(string user, string password)
        {
            PrincipalContext principalcontext = new PrincipalContext(ContextType.Domain, "25.77.144.99");

            bool userValid = principalcontext.ValidateCredentials(user, password);
            if (userValid == true)
                Console.WriteLine("Autenticado");
            else
                Console.WriteLine("No autenticado");
        }

        public static void register()
        {
            using (PrincipalContext principalContext = new PrincipalContext(ContextType.Domain, "25.77.144.99", "Administrador", "Una2021"))
            {
                using (UserPrincipal userPrincipal = new UserPrincipal(principalContext))
                {
                    userPrincipal.Name = "CharlesBarker";
                    userPrincipal.SamAccountName = "CharlesBarker";
                    userPrincipal.GivenName = "Charles";
                    userPrincipal.Surname = "Barker";
                    userPrincipal.DisplayName = "CharlesBarker";
                    userPrincipal.UserPrincipalName = "CharlesBarker";
                    userPrincipal.SetPassword("Una2021");
                    userPrincipal.Enabled = true;
                    userPrincipal.Save();
                }
            }
        }
    }
}
