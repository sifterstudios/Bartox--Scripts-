using System.Collections.Generic;
using System.Diagnostics.Tracing;

namespace Bartox.Core
{
    public class EventSystem
    {
        // create a singleton instance of the event system to be used throughout the game
        static EventSystem _singleton;

        public static EventSystem Singleton
        {
            get { return _singleton ??= new EventSystem(); }
        }
    }
}