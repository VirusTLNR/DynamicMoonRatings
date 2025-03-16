# Dynamic Moon Ratings

Dynamic Moon Ratings aims to be the mod that gives moon ratings an actual point.

## Features
- Two Usage Modes:
    Risk -> Ratings calculation including scrap value. No moon scrap modification.
    Difficulty -> Ratings calculation excluding scrap value. Does modify moon scrap based off the rating provided.

- Three display Modes:
    'A' -> shows just a letter like vanilla moons (when speedrunning mode is enabled, this will be forced into A### mode)
    'A###' -> shows the rating, with the first number replaced with a vanilla style letter.
    '####' -> shows the rating only as a number. this is the only mode that shows the true rating value.
    
    Note: 'A' and 'A###' will show F- and S+ for ratings outside of the 0 to 7000 range.

- Version config selection for both ratings and scrap spawning (if in difficulty mode) so even when the mod updates, you can still revert to the settings you prefer, or use newer versions. Buggy versions of calculations maybe removed, or modified, which will be noted in the changelog.

- Configuration of base scrap generated level for a 0 rated moon (50/100 -> 1500/3000 for min/max allowing you to have extreme low scrap games, extreme high scrap games, as well as normal games (default value should be balanced for a vanilla game))

- [Coming Soon] Speedrunning Mode:
    Once released, speedrunning mode will force you into 'A###' or '####' display mode and will display all your settings for the mod on screen as "speed running" verification, including versions used and the moon rating for purposes of speedrunning.


## Ratings Calculation Details
- LLL -> Uses the original LLL moon rating calculation, applied to all moons. The displayed value is the LLL rating multiplied by 3.5 to fit it into the same scale as V1, the rating order and gap will be identical to how LLL rates them, without interference from moon creators.

- V1 -> Rating = ((((enemies + price + cruiser) - scrap) * weather) / time) .. this leads to vanilla configured Experimentation having a rating of 14342 in difficulty mode and 14032 in risk mode when tested (weather not checked), Difficulty Mode uses a fixed value of 1000 for SCRAP.
    Example(Risk Mode): [Moon]41 Experimentation       -> (([Enemies]18927.78 + [Price]0 + [Cruiser]0) - [Scrap]1387.5) x [Weather]0.8 / [Time]1 = [Total]14032.22
    Example(Difficulty Mode): [Moon]41 Experimentation -> (([Enemies]18927.78 + [Price]0 + [Cruiser]0) - [Scrap]1000)   x [Weather]0.8 / [Time]1 = [Total]14342.22
    
    Note: displayed rating is the above rating divided by 10. This is to be functional compatibility wise with the dictionary used to sort ratings (which often fails with lower numbers that often lead to the same numbers being used)

## Scrap Generation Details (Difficulty Mode Only)
- V1 -> ScrapGenMod = ((100 + (ratingNumber + quotaNumber + dayNumber)) / 100)
        ratingNumber = (rating / 200) // 1% per 200 rating.
        quotaNumber = (quota / 50) // 1% per 50 quota
        dayNumber = (current day) // 1% per 1 days spent

   In V1, day 1, 130 quota using 300/600 base scrap, should have scrap ranging from 300-2000 (estimate) depending on moon rating. (i got 572-1145 on Rainy Experimntation and 1355-2711 on Rainy StarlancerZero.)
-  In V1, day 37, 3900ish quota, using 300/600 base scrap, should have scrap ranging from 800-3500 (estimate) depending on moon rating. (i got 939-1878 on Stormy Experimentation and 1622-3245 on Clear Weathered StarlancerZero)