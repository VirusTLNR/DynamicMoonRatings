# Dynamic Moon Ratings

Dynamic Moon Ratings aims to be the mod that gives moon ratings an actual point. All clients need this mod installed.

if you are having issues, please report them here: https://discord.com/channels/1168655651455639582/1352724025750585505

## Features
- Two Usage Modes:
    Risk -> Ratings calculation including scrap value. No moon scrap modification.
    Difficulty -> Ratings calculation excluding scrap value. Scrap spawned is modified based off the rating displayed, days spent ingame and the quota you require.

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
- CUSTOM -> Configure your own ratings and tiers in the config, must load into a game save at least once before configuring the moon ratings otherwise they will display as "UNKNOWN" (so new moons added and not configured will show as UNKNOWN too). (Tiers may also require a game save load to update for more configuration)

- LLL -> Uses the original LLL moon rating calculation, applied to all moons. The displayed value is the LLL rating multiplied by 3.5 to fit it into the same scale as V1, the rating order and gap will be identical to how LLL rates them, without interference from moon creators.

- V1 -> Rating = ((((enemies + price + cruiser) - scrap) * weather) / time) .., Difficulty Mode uses a fixed value of 1000 for SCRAP.
    Note: displayed rating is the above rating divided by 10. This is so the moons can fit in a more suitable scale. (0-7000 in V1)

## Scrap Generation Details (Difficulty Mode Only)
- V1 -> ScrapGenMod = ((100 + (ratingNumber + quotaNumber + dayNumber)) / 100)
        ratingNumber = (rating / 200) // 1% per 200 rating.
        quotaNumber = (quota / 50) // 1% per 50 quota
        dayNumber = (current day) // 1% per 1 days spent
    Note: this is made for a custom rating range of 0-7000.