using CLI.Shared;

namespace Module1
{
    [Class(Name = "Test configurazione Active Directory",
           Author = "Simone Ciamberlini",
           ReleaseDate = "08-03-2016",
           Description =  "All about the calendars",
           Version = "1.0.0.0" )]
    public class Class2 : ICli
    {
        private readonly ICliLogger _logger;
        public Class2(ICliLogger logger)
        {
            _logger = logger;
        }
        [Method("Test parametri di Active Directory")]
        public void TestActiveDirectory(
              [Param("Inserire il nome del dominio DNS")] string dnsDomainName)
        {
            _logger.Warn(Properties.Settings.Default.Server);
        }
    }
}
