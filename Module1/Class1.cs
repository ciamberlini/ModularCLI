using CLI.Shared;

namespace Module1
{
    [Class(Name = "Configurazione Active Directory",
           Author = "Simone Ciamberlini",
           ReleaseDate = "08-03-2016",
           Description = "Modulo per la configurazione dei parametri di active directory",
           Version = "1.0.0.0")]
    public class Class1 : ICli
    {
        private readonly ICliLogger _logger;
        public Class1(ICliLogger logger)
        {
            _logger = logger;
        }

        [Method("Imposta parametri di Active Directory", "configuread")]
        public void ConfigureActiveDirectory(
              [Param("Inserire il nome di dominio DNS", "-d")]               string dnsDomainName,
              [Param("Inserire la porta LDAP", "-p")]                        int ldapPort,
              [Param("Inserire un username valido per il bind ad AD", "-u")] string bindUsername,
              [Param("Inserire la password valida per il bind ad AD", "-s")] string bindPassword)
        {
            _logger.WarnFormat("You entered bind for {0} on port {1} with user {2} and password {3}", dnsDomainName, ldapPort, bindUsername, bindPassword);
        }
    }
}
