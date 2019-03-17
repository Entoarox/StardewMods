using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.Characters;
using StardewValley.Menus;

using Entoarox.MorePetsAndAnimals.Framework;

namespace Entoarox.MorePetsAndAnimals
{
    internal class AdoptQuestion
    {
        /*********
        ** Fields
        *********/
        private readonly string Type;
        private readonly int Skin;
        private readonly AnimatedSprite Sprite;
        private Farmer Who;


        /*********
        ** Public methods
        *********/
        public AdoptQuestion(AnimalSkin skin)
        {
            this.Type = skin.AnimalType;
            this.Skin = skin.ID;

            this.Sprite = new AnimatedSprite(skin.AssetKey, 28, 32, 32)
            {
                loop = true
            };
        }

        public static void Show()
        {
            Random random = ModEntry.Random;
            string type=null;
            AnimalSkin skin=null;
            if (ModEntry.Config.BalancedPetTypes)
            {
                double totalType = ModEntry.Pets.Count;
                Dictionary<string, double> types = ModEntry.Pets.Where(a => a.Value.Count>0).ToDictionary(k => k.Key, v => totalType);
                foreach (Pet pet in ModEntry.GetAllPets().Where(p => ModEntry.PetTypesRev.ContainsKey(p.GetType()) && ModEntry.Pets[ModEntry.PetTypesRev[p.GetType()]].Count>0))
                    types[ModEntry.PetTypesRev[pet.GetType()]] *= 0.5;
                types = types.ToDictionary(k => k.Key, v => v.Value / totalType);
                double typeMax = types.Values.OrderByDescending(a => a).First();
                double typeChance = random.NextDouble() * typeMax;
                string[] validTypes = types.Where(a => a.Value >= typeChance).Select(a => a.Key).ToArray();
                if (validTypes.Length > 0)
                    type = validTypes[random.Next(validTypes.Length)];
            }
            if (string.IsNullOrEmpty(type))
            {
                string[] arr = ModEntry.Pets.Where(a => a.Value.Count > 0).Select(a => a.Key).ToArray();
                type = arr[random.Next(arr.Length)];
            }
            if (ModEntry.Config.BalancedPetSkins)
            {
                double totalSkin = ModEntry.Pets[type].Count;
                Dictionary<int, double> skins = ModEntry.Pets[type].ToDictionary(k => k.ID, v => totalSkin);
                foreach (Pet pet in ModEntry.GetAllPets().Where(pet => ModEntry.PetTypesRev.ContainsKey(pet.GetType()) && ModEntry.PetTypesRev[pet.GetType()].Equals(type) && skins.ContainsKey(pet.Manners)))
                    skins[pet.Manners] *= 0.5;
                skins = skins.ToDictionary(k => k.Key, v => v.Value / totalSkin);
                double skinMax = skins.Values.OrderByDescending(a => a).First();
                double skinChance = random.NextDouble();
                int[] validSkins = skins.Where(a => a.Value >= skinChance).Select(a => a.Key).ToArray();
                int id = 0;
                if (validSkins.Length > 0)
                    id = validSkins[random.Next(validSkins.Length)];
                skin = ModEntry.Pets[type].FirstOrDefault(a => a.ID == id);
            }
            if(skin==null)
                skin = ModEntry.Pets[type][random.Next(ModEntry.Pets[type].Count)];
            AdoptQuestion q = new AdoptQuestion(skin);
            ModEntry.SHelper.Events.Display.RenderedHud += q.Display;
            Game1.currentLocation.lastQuestionKey = "AdoptPetQuestion";
            Game1.currentLocation.createQuestionDialogue(
                ModEntry.SHelper.Translation.Get("AdoptMessage", new { petType = type, adoptionPrice = ModEntry.Config.AdoptionPrice }),
                Game1.player.money < ModEntry.Config.AdoptionPrice
                    ? new[]
                    {
                        new Response("n", ModEntry.SHelper.Translation.Get("AdoptNoGold", new { adoptionPrice = ModEntry.Config.AdoptionPrice }))
                    }
                    : new[]
                    {
                        new Response("y", ModEntry.SHelper.Translation.Get("AdoptYes")),
                        new Response("n", ModEntry.SHelper.Translation.Get("AdoptNo"))
                    },
                q.Resolver);
        }

        public void Display(object o, EventArgs e)
        {
            if (Game1.activeClickableMenu != null && Game1.activeClickableMenu is DialogueBox)
            {
                DialogueBox box = (DialogueBox)Game1.activeClickableMenu;
                Vector2 c = new Vector2(Game1.viewport.Width / 2 - 128 * Game1.pixelZoom, Game1.viewport.Height - box.height - 56 * Game1.pixelZoom);
                Vector2 p = new Vector2(36 * Game1.pixelZoom + c.X, 32 * Game1.pixelZoom + c.Y);
                IClickableMenu.drawTextureBox(Game1.spriteBatch, Game1.menuTexture, new Rectangle(0, 256, 60, 60), (int)c.X, (int)c.Y, 40 * Game1.pixelZoom, 40 * Game1.pixelZoom, Color.White, 1, true);
                Game1.spriteBatch.Draw(this.Sprite.Texture, p, this.Sprite.SourceRect, Color.White, 0, new Vector2(this.Sprite.SpriteWidth, this.Sprite.SpriteHeight), Game1.pixelZoom, SpriteEffects.None, 0.991f);
                this.Sprite.Animate(Game1.currentGameTime, 28, 2, 500);
            }
            else
                ModEntry.SHelper.Events.Display.RenderedHud -= this.Display;
        }

        public void Resolver(Farmer who, string answer)
        {
            ModEntry.SHelper.Events.Display.RenderedHud -= this.Display;
            if (answer == "n")
                return;
            this.Who = who;
            Game1.activeClickableMenu = new NamingMenu(this.Namer, ModEntry.SHelper.Translation.Get("ChooseName"));
        }

        public void Namer(string petName)
        {
            Pet pet;
            this.Who.Money -= ModEntry.Config.AdoptionPrice;
            Type type = ModEntry.PetTypes[this.Type];
            pet = (Pet)Activator.CreateInstance(type, (int)Game1.player.position.X, (int)Game1.player.position.Y);
            pet.Sprite = new AnimatedSprite(ModEntry.SHelper.Content.GetActualAssetKey($"assets/skins/{this.Type}_{this.Skin}.png"), 0, 32, 32);
            pet.Name = petName;
            pet.displayName = petName;
            pet.Manners = this.Skin;
            pet.Age = Game1.year * 1000 + Array.IndexOf(ModEntry.Seasons, Game1.currentSeason) * 100 + Game1.dayOfMonth;
            pet.Position = Game1.player.position;
            Game1.currentLocation.addCharacter(pet);
            pet.warpToFarmHouse(this.Who);
            Game1.drawObjectDialogue(ModEntry.SHelper.Translation.Get("Adopted", new { petName }));
        }
    }
}
