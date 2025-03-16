using LethalLevelLoader;
using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicMoonRatings.Modules
{
    internal class CustomModes
    {
        List<ExtendedLevel> moons = LethalLevelLoader.PatchedContent.ExtendedLevels;

        internal static int GetCustomRating(ExtendedLevel level)
        {
            return 666; //nothing done with this yet, config requiring setup first.
        }
    }
}
