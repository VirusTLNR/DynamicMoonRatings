﻿using LethalLevelLoader;

namespace DynamicMoonRatings.Modules.Calculations
{
    internal class Price
    {
        internal static float PriceCalc(ExtendedLevel level)
        {
            int routePrice = level.RoutePrice;
            float priceModifier;
            /*if (routePrice > 0)
            {
                priceModifier = 1 + (routePrice / 100);
            }
            else
            {
                priceModifier = 1;
            }*/
            priceModifier = routePrice;
            return priceModifier;
        }
    }
}
