ALL Docs
==============
[Introduction](Introduction.md) / **Content Pack Format** / [Custom Actions](Actions.md) / [Miscellaneous](Misc.md)

---------------------------------------------------------------------------------------------------------
Content Pack Format
===================
Each ALL content pack must have two files:
* a `manifest.json` for SMAPI (see [Modding:Content packs](https://stardewvalleywiki.com/Modding:Content_packs));
* a `locations.json` which describes the location changes.

This `locations.json` contains the basic layouts for both custom locations and shops.
For a list of valid conditions that are usable in the various "Conditions" fields see [the Custom Condition System documentation](../../Framework/Docs/Conditions.md) over in the Entoarox Framework github.

To use a `Conditional` in your conditions, use the name of the conditional prefixed with `ALLCondition_`.

**Location Config (Version 1.2)**
```javascript
{
	/* What config loader ALL should use to load the file, for the 1.2 branch should always be `1.2` */
	"LoaderVersion":"1.2",
	/* The locations section is where new locations are registered for addition to the game */
	"Locations":[
		{
			/* The name to use for this new location in warps and such, if not unique then ALL will report a error */
			"MapName":"",
			/* The path to the xnb file, relative to this locations.json, of the tbin that contains the map for this location, without the xnb extension */
			"FileName":"",
			/* If set to `true` then this new location is a outdoor location and exhibits all behaviour outdoor locations have */
			"Outdoor":false,
			/* If set to `true` then this new location is a farmable location and crops will be able to be planted here */
			"Farmable":false,
			/* What special Type of location this is, or `Default` if a generic location, special location types have unique behaviour that no other location type exhibits */
			"Type":"Default"
			/*
				The following types are currently supported:
				Default			- Generic location
				Cellar			- Location where Cask objects are able to work
				Greenhouse		- Location that is not affected by seasons similar to the greenhouse
				Sewer			- Location that has the same green fog effect as the vanilla Sewer location
				BathHousePool	- Location that has the same white fog effect as the vanilla BathHousePool location
				Desert			- Location that exhibits the same 'always sunny' behaviour that the vanilla Desert exhibits
				Decoratable		- Location that allows for Furniture objects to be placed within

				More types may be added in the future, ALL will report if a type is unknown and fall back to the Default type in that case
			*/
		},
		...
	],
	/* The overrides section is where location maps can be overridden by name */
	"Overrides":[
		{
			/* The name that SDV uses for the location you wish to override */
			"MapName":"",
			/* The path to the xnb file, relative to this locations.json file, of the tbin that contains the map for this location, without the xnb extension */
			"FileName":""
		},
		...
	],
	/* The redirects section is where you can redirect the loading of any xnb file relative to Content to a xnb file relative to the locations.json instead */
	"Redirects":[
		{
			/* The path (Without the c:/blabla/../Content/ prefix) to the xnb file that should be redirected, without the xnb extension */
			"FromFile":"",
			/* The path (Relative to the locations.json) to the xnb file that should be loaded instead, without the xnb extension */
			"ToFile":""
		},
		...
	],
	/* The tilesheets section is where you register all custom tilesheets so that ALL can properly handle them */
	"Tilesheets":[
		{
			/* The name of the location that this tilesheet is attached to and needs to be edited on */
			"MapName":"",
			/* The path to the xnb file, relative to this locations.json, of the tbin that contains the map for this location, without the xnb extension */
			"FileName":"",
			/* The name given to the tilesheet in tIDE */
			"SheetId":"",
			/* If `true` then ALL will treat this tilesheet as seasonal, in such a case, it appends `_<season>` to the filename given above before adding the xnb extension */
			"Seasonal":false
		},
		...
	],
	/* The tiles section is where you can set or remove specific tiles on any location */
	"":[
		/* This first example is for setting a static tile */
		{
			/* The name of the location where you wish to set this tile */
			"MapName":"",
			/* The name given to the tilesheet in tIDE, if omitted, the first tilesheet assigned to the map is assumed */
			"SheetId":"",
			/* The X coordinate of the tile you wish to set */
			"TileX":0,
			/* The Y coordinate of the tile you wish to set */
			"TileY":0,
			/* The layer you wish to place this tile on, must be a valid SDV layer (Back, Buildings, Paths, Front, AlwaysFront) */
			"LayerId":"",
			/* What conditions must apply (if any) for this edit to be applied, omit or leave empty for no conditions */
			"Conditions":"",
			/* If set to `true` this edit is optional, and if the location it is meant to edit does not exist then ALL will not report an error as it usually would */
			"Optional":false,
			/* The tile index for the tile to set */
			"TileIndex":0
		},
		/* This second example is for setting a animated tile */
		{
			/* The name of the location where you wish to set this tile */
			"MapName":"",
			/* The name given to the tilesheet in tIDE, if omitted, the first tilesheet assigned to the map is assumed */
			"SheetId":"",
			/* The X coordinate of the tile you wish to set */
			"TileX":0,
			/* The Y coordinate of the tile you wish to set */
			"TileY":0,
			/* The layer you wish to place this tile on, must be a valid SDV layer (Back, Buildings, Paths, Front, AlwaysFront) */
			"LayerId":"",
			/* What conditions must apply (if any) for this edit to be applied, omit or leave empty for no conditions */
			"Conditions":"",
			/* If set to `true` this edit is optional, and if the location it is meant to edit does not exist then ALL will not report an error as it usually would */
			"Optional":false,
			/* The list of tile indexes for the tile to animate through */
			"TileIndexes":[0,1,1,2,2,0],
			/* The interval between each frame in the animation */
			"Interval":3
		},
		/* This third example is for removing a existing tile */
		{
			/* The name of the location where you wish to set this tile */
			"MapName":"",
			/* The X coordinate of the tile you wish to set */
			"TileX":0,
			/* The Y coordinate of the tile you wish to set */
			"TileY":0,
			/* The layer you wish to place this tile on, must be a valid SDV layer (Back, Buildings, Paths, Front, AlwaysFront) */
			"LayerId":"",
			/* What conditions must apply (if any) for this edit to be applied, omit or leave empty for no conditions */
			"Conditions":"",
			/* If set to `true` this edit is optional, and if the location it is meant to edit does not exist then ALL will not report an error as it usually would */
			"Optional":false,
			/* Setting the tile index to -1 tells ALL to remove any pre-existing tiles rather then setting a tile */
			"TileIndex":-1
		},
		...
	],
	/* The properties section is where you can modify tile properties for any specific tile on any location */
	"Properties":[
		{
			/* The name of the location where you wish to set this tile */
			"MapName":"",
			/* The X coordinate of the tile you wish to set */
			"TileX":0,
			/* The Y coordinate of the tile you wish to set */
			"TileY":0,
			/* The layer you wish to place this tile on, must be a valid SDV layer (Back, Buildings, Paths, Front, AlwaysFront) */
			"LayerId":"",
			/* What conditions must apply (if any) for this edit to be applied, omit or leave empty for no conditions */
			"Conditions":"",
			/* If set to `true` this edit is optional, and if the location it is meant to edit does not exist then ALL will not report an error as it usually would */
			"Optional":false,
			/* The key of the tile-property you wish to set */
			"Key":"",
			/* The value you wish to assign to the tile-property */
			"Value":""
		},
		...
	],
	/* The warps section lets you add or override warps, warps are overridden if a pre-existing warp is in the same x,y position, otherwise a new warp is added */
	"Warps":[
		{
			/* The name of the location where you wish to set this tile */
			"MapName":"",
			/* The X coordinate of the tile you wish to set */
			"TileX":0,
			/* The Y coordinate of the tile you wish to set */
			"TileY":0,
			/* The layer you wish to place this tile on, must be a valid SDV layer (Back, Buildings, Paths, Front, AlwaysFront) */
			"LayerId":"",
			/* What conditions must apply (if any) for this edit to be applied, omit or leave empty for no conditions */
			"Conditions":"",
			/* If set to `true` this edit is optional, and if the location it is meant to edit does not exist then ALL will not report an error as it usually would */
			"Optional":false,
			/* The name of the target location that this warp leads to */
			"TargetName":"",
			/* The X position on the target location this warp leads to */
			"TargetX":0,
			/* The Y position on the target location this warp leads to */
			"TargetY":0
		},
		...
	],
	/* The conditionals section lets you add custom "pay X of Y" requirements to the game using the `ALLConditional` tile action */
	"Conditionals":[
		{
			/* The unique name for this conditional */
			"Name":"",
			/* The Id of the item that is required to pay in order to complete this conditional, use `-1` for Gold */
			"Item":-1,
			/* The amount of the item is required to pay in order to complete this conditional */
			"Amount":0,
			/* The text that is displayed above the yes/no answers to complete this conditional */
			"Question":""
		},
		...
	],
	/* The teleporters section lets you add custom teleportation networks similar to the vanilla Minecart system to the game */
	"Teleporters":[
		{
			/* The name of this unique teleporter list */
			"Name":"",
			/* The list of destinations, if the previously given name has been used by another content pack, then both destination lists are merged */
			"Destinations":[
				{
					/* The name of the map that this specific destination will teleport the player to */
					"MapName":"",
					/* The X position on the map this specific destination will teleport the player to */
					"TileX":0,
					/* The Y position on the map this specific destination will teleport the player to */
					"TileY":0,
					/* The text to display as the selectable option for this destination */
					"ItemText":""
				},
				...
			]
		},
		...
	],
	/* The shops section references the names of the individual <shopname>.json files relative to the locations.json, without the json extension */
	"Shops":["","",...]
}
```

Shops have their own format that can be found below:

**Shop Config (Version 1)**
```javascript
{
	/* What version of the shop config parser ALL should use to read this file */
	"ParserVersion":1,
	/* Path relative to the shop config to a png to be used as a owner portrait for the shop */
	"Portrait":"",
	/* Name of the NPC who owns this shop, if no NPC exists then a placeholder NPC is created */
	"Owner":"",
	/* A list of messages, each time the shop is opened one is selected at random to be displayed */
	"Messages":["","",...],
	/* The list of items sold by this shop */
	"Items":[
		/* Each entry in the shop has its own item definition */
		{
			/* The numeric ID of the item */
			"Id":0,
			/* Optional, defaults to `false`, if true then this is a BigCraftable rather then a normal Object */
			"BigCraftable":false,
			/* Optional, defaults to the default sale price assigned to the item */
			"Price":0,
			/* Optional, defaults to `1`, how much of the item is bought per purchase */
			"Stack":1,
			/* Optional, defaults to unlimited stock, when set this limits how much of the item the shop sells at once */
			"Stock":1,
			/* What conditions must apply (if any) for this item to be in the shop, omit or leave empty for no conditions */
			"Conditions":""
		}
	]
	
}
```
