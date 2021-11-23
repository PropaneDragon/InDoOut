using InDoOut_Core.Instancing;
using System;

namespace InDoOut_Executable_Core.Localisation
{
    public class Branding : Singleton<Branding>
    {
        public string AppNameShort => "ido";
        public string AppNameNoArrows => "in do out";
        public string AppNameArrows => "in > do > out";

        public string WebsiteShort => "idoapp.net";
        public string Website => "https://idoapp.net";
        public Uri WebsiteUri => new(Website);
    }
}
