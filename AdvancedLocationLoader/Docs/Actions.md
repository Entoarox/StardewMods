ALL Docs
==============
[Introduction](Introduction.md) / [Content Pack Format](Manifest.md) / **Custom Actions** / [Miscellaneous](Misc.md)

---------------------------------------------------------------------------------------------------------
Custom Actions
==============
ALL comes bundled with new custom actions that add extra functionality for mod makers to use.
Even mods that do not get loaded through ALL can use some of these actions as long as ALL is installed.

A full list of these actions, their argument list, and their description can be found below.
Actions that only work for location mods have a `*` at the end, this is not part of the Action, but a identifier for location-mod only actions.

| Action              | Arguments                                   | Description                                                                                                                                         |
|---------------------|---------------------------------------------|-----------------------------------------------------------------------------------------------------------------------------------------------------|
| `ALLReact`          | `ALLReact <Interval> <Tile1>,<Tile2>,<...>` | Animates through the list of tile indexes given once, with a delay of `Interval` between each tile.                                                 |
| `ALLShift`          | `ALLShift <X> <Y>`                          | Similar to `Warp` except that it moves you within the current location, meaning it works for any location.                                          |
| `ALLRawMessage`     | `ALLRawMessage <Message>`                   | Displays the raw text as given rather then looking up a translated message.                                                                         |
| `ALLRandomMessage`  | `ALLRandomMessage <Msg1>\|<Msg2>\|<...>`    | Displays one of the `\|` separated messages at random each time it is activated, *Localization not supported*.                                      |
| `ALLMessage`*       | `ALLMessage <ModID> <MessageKey>`           | Displays the localized message identified by the `ModID` of the location mod and `MessageKey` which matches the translation key in your i18n files. |
| `ALLShop`*          | `ALLShop <ShopID>`                          | Displays the shop for the `ShopID` given, or a empty shop if none exists. ([More info](Manifest.md))                                                    |
| `ALLTeleporter`*    | `ALLTeleporter <TeleporterID>`              | Displays the teleport destination menu for `TeleporterID` given. ([More info](Manifest.md))                                                         |
| `ALLConditional`*   | `ALLConditional <ConditionalID>`            | Displays the conditional completion menu for the `ConditionalID` given, if not already completed. ([More info](Manifest.md))                        |