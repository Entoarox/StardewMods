Custom Condition System
=====================
This document contains a list of all conditions that are implemented into the EntoaroxFramework Custom Condition System (CCS).
Each condition comes with a explanation as to what situation makes it true

Please note that all conditions can be inverted by prefixing them with the `!` symbol

Weather Conditions
----------------------------
Weather conditions are all conditions related to the weather.

----
**`weatherSun weatherRain weatherStorm weatherSnow weatherFestival weatherWedding weatherFallDebris weatherSummerDebris`**

Each of these conditions is `true` if the specific weather condition they name is the current weather state.


**`weatherDebris`**

This condition is true if either `weatherFallDebris` or `weatherSummerDebris` would be true.


**`weatherSeasonal`**

This condition is `true` if the current weather state, `weatherSummerDebris` for summer, `weatherFallDebris` for fall and `weatherSnow` for winter can only occur in the current season.


**`weatherSpecial`**

This condition is `true` if the current weather state is either `weatherWedding` or `weatherFestival`.

Time conditions
-----------------------
Time conditions are those conditions that relate to time in some shape.

----
**`sunday monday tuesday wednesday thursday friday saturday`**

Each of these conditions is `true` only on the day in question.


**`weekend`**

This condition is `true` if either the `sunday` or `saturday` conditions would be.


**`spring summer fall winter`**

Each of these conditions is `true` only during the season in question.


**`time>%`**

This condition is `true` if the time, marked as `%` has happened on the current day.
Time is written in the 24 hour notation without a separator.
Thus, Stardew days begin at `600` and end at `200` with `700 800 900 1000 1100 etc.` in between.


**`year=%`**

This condition is `true` if the year, marked as `%` is the current year.


**`year>%`**

This condition is `true` if the year, marked as `%` has already passed.

Flag Conditions
----------------------
Flag conditions are conditions that become true after the player has completed a certain task.

----
**`earthquake`**

This condition is `true` from the day after the earthquake event has happened.


**`rustyKey`**

This condition is `true` if the player has been given the rusty key.


**`skullKey`**

This condition is `true` if the player has been given the skull key.


**`clubMember`**

This condition is `true` if the player has been given a club pass and is allowed into the club.


**`dwarfSpeak`**

This condition is `true` if the player is capable of speaking the dwarven language.

Relationship Conditions
----------------------------------
These are conditions that are applicable to relationships.

-----
**`married`**

This condition is `true` if the player is married to anyone.


**`engaged`**

This condition is `true` if the player is engaged to be married.

**`married%`**

This condition is `true` if the player is married to the NPC named by `%`.
For example, `marriedAlex` is `true` if the player is married to the Alex NPC.


**`engaged%`**

This condition is `true` if the player is engaged to be married to the NPC named by `%`.
For example, `engagedAlex` is `true` if the player is engaged to be married to the Alex NPC.

Miscellaneous conditions
------------------------------------
These are conditions that do not fit any of the specific categories above.

----
**`houseLevel=%`**

This condition is `true` if the players house is at the upgrade level identified by the `%` placeholder.
Currently, in Stardew valley 1.1 the level can be anywhere between `0` and `3` but future versions of Stardew might expand upon this.


**`farmType=%`**

This condition is `true` if the player has the given type `%` as their farm.
Currently in Stardew valley 1.1 the level can be anywhere between `0` and `4` but future versions of Stardew might expand upon this.
