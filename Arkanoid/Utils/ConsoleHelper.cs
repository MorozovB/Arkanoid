using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arkanoid.Utils
{
    // Provides helper methods and objects for console operations.
    // Currently includes a lock object to ensure thread-safe console access.
    internal static class ConsoleHelper
    {
        // Lock object for synchronizing console operations.
        internal static readonly object ConsoleLock = new object();

        // Additional helper methods for console operations can be added here if needed.
    }
}
