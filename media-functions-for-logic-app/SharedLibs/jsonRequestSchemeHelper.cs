using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace media_functions_for_logic_app.SharedLibs
{
    class jsonRequestSchemeHelper
    {
        /* TODO move this in separate class :*/
        public class Rootobject
        {
            public Properties properties { get; set; }
            public string type { get; set; }
        }

        public class Properties
        {
            public Name name { get; set; }
            public PathToItem pathToItem { get; set; }
            public SourceContainerPath sourceContainerPath { get; set; }
        }

        public class Name
        {
            public string value { get; set; }
            public string type { get; set; }
        }
        public class PathToItem
        {
            public string value { get; set; }
        }
        public class SourceContainerPath
        {
            public string value { get; set; }
        }
    }
}
