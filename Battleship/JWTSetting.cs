using Microsoft.Extensions.Configuration;
using System.Configuration;

namespace battleship_my
{
    internal class JWTSetting : System.Configuration.ConfigurationSection
    {
        [ConfigurationProperty("JWT", DefaultValue = "")]
        public string JWT
        {
            get { return (string) this["JWT"]; }
            set { this["JWT"] = value; }
        }
    }
}
