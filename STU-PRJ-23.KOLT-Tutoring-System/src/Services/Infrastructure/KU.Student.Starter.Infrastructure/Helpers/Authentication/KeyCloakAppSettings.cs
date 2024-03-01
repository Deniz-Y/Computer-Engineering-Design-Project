using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KU.Student.Starter.Infrastructure.Helpers.Authentication
{
    public interface IKeyCloakAppSettings
    {
        public string ClientId { get; set; }
        public string Authority { get; set; }
        public string ClientSecret { get; set; }
    }

    public class KeyCloakAppSettings : IKeyCloakAppSettings
    {
        public string ClientId { get; set; }
        public string Authority { get; set; }
        public string ClientSecret { get; set; }
    }
}
