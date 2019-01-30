using System;
using System.Collections.Generic;
using System.Linq;
using Entoarox.MorePetsAndAnimals.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Menus;

namespace Entoarox.MorePetsAndAnimals
{
    internal class AdoptQuestion
    {
        /*********
        ** Fields
        *********/
        private readonly IModEvents Events;
        private readonly bool Cat;
        private readonly int Skin;
        private readonly AnimatedSprite Sprite;
        private Farmer Who;


        /*********
        ** Public methods
        *********/
        public AdoptQuestion(bool cat, int skin, IModEvents events)
        {
            this.Cat = cat;
            this.Skin = skin;
            this.Events = events;

            this.Sprite = new AnimatedSprite(ModEntry.SHelper.Content.GetActualAssetKey($"assets/skins/{(cat ? "cat" : "dog")}_{skin}.png"), 28, 32, 32)
            {
                loop = true
            };
        }

        /// <summary>Select a pet for adoption (if any) and show the adoption UI.</summary>
        /// <param name="events">The available mod events.</param>
        /// <param name="currentPets">The pets already owned by the player.</param>
        public static void Show(IModEvents events, Pet[] currentPets)
        {
            // choose pet & skin
            AnimalType type = AdoptQuestion.GetNextPet(currentPets);
            bool isCat = type == AnimalType.Cat;
            int skin = AdoptQuestion.GetNextSkin(isCat, currentPets);
            if (skin <= 0)
            {
                Game1.drawObjectDialogue("Just an empty box.");
                return;
            }

            // show adoption UI
            AdoptQuestion q = new AdoptQuestion(isCat, skin, events);
            events.Display.RenderedHud += q.Display;
            Game1.currentLocation.lastQuestionKey = "AdoptPetQuestion";
            Game1.currentLocation.createQuestionDialogue(
                $"Oh dear, it looks like someone has abandoned a poor {type} here! Perhaps you should pay Marnie {ModEntry.Config.AdoptionPrice} gold to give it a checkup so you can adopt it?",
                Game1.player.money < ModEntry.Config.AdoptionPrice
                    ? new[]
                    {
                        new Response("n", $"Unfortunately I do not have the required {ModEntry.Config.AdoptionPrice} gold in order to do this.")
                    }
                    : new[]
                    {
                        new Response("y", "Yes, I really should adopt the poor animal!"),
                        new Response("n", "No, I do not have the space to house it.")
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
                this.Events.Display.RenderedHud -= this.Display;
        }

        public void Resolver(Farmer who, string answer)
        {
            this.Events.Display.RenderedHud -= this.Display;
            if (answer == "n")
                return;
            this.Who = who;
            Game1.activeClickableMenu = new NamingMenu(this.Namer, "Choose a name");
        }

        public void Namer(string petName)
        {
            Pet pet;
            this.Who.Money -= ModEntry.Config.AdoptionPrice;
            if (this.Cat)
            {
                pet = new Cat((int)Game1.player.position.X, (int)Game1.player.position.Y)
                {
                    Sprite = new AnimatedSprite(ModEntry.SHelper.Content.GetActualAssetKey($"assets/skins/cat_{this.Skin}.png"), 0, 32, 32)
                };
            }
            else
            {
                pet = new Dog(Game1.player.getTileLocationPoint().X, Game1.player.getTileLocationPoint().Y)
                {
                    Sprite = new AnimatedSprite(ModEntry.SHelper.Content.GetActualAssetKey($"assets/skins/dog_{this.Skin}.png"), 0, 32, 32)
                };
            }

            pet.Name = petName;
            pet.displayName = petName;
            pet.Manners = this.Skin;
            pet.Age = Game1.year * 1000 + Array.IndexOf(ModEntry.Seasons, Game1.currentSeason) * 100 + Game1.dayOfMonth;
            pet.Position = Game1.player.position;
            Game1.currentLocation.addCharacter(pet);
            pet.warpToFarmHouse(this.Who);
            Game1.drawObjectDialogue($"Marnie will bring {petName} to your house once they have their shots and been given a grooming.");
        }

        /*********
        ** Private methods
        *********/
        /// <summary>Randomly choose a cat or dog, balanced to prefer the least allocated pet type.</summary>
        /// <param name="currentPets">The current pets in the game.</param>
        private static AnimalType GetNextPet(Pet[] currentPets)
        {
            int catSkins = ModEntry.Indexes[AnimalType.Cat].Length;
            int dogSkins = ModEntry.Indexes[AnimalType.Dog].Length;

            // choose whichever has skins
            if (catSkins == 0 || dogSkins == 0)
                return catSkins != 0 ? AnimalType.Cat : AnimalType.Dog;

            // else choose based on allocation
            int catCount = currentPets.OfType<Cat>().Count();
            int dogCount = currentPets.OfType<Dog>().Count();
            if (catCount < dogCount)
                return AnimalType.Cat;
            if (dogCount < catCount)
                return AnimalType.Dog;

            // else choose randomly
            return ModEntry.Random.NextDouble() < 0.5 ? AnimalType.Cat : AnimalType.Dog;
        }

        /// <summary>Randomly choose an animal skin, balanced to prefer the least allocated skins.</summary>
        /// <param name="isCat">Whether to get a cat skin (else dog).</param>
        /// <param name="currentPets">The current pets in the game.</param>
        private static int GetNextSkin(bool isCat, Pet[] currentPets)
        {
            // get available skins
            AnimalSkin[] skins = ModEntry.Indexes[isCat ? AnimalType.Cat : AnimalType.Dog];
            if (!skins.Any())
                return 0;

            // choose based on allocations
            IDictionary<int, int> allocations = skins.ToDictionary(p => p.ID, p => 0);
            foreach (Pet pet in currentPets)
            {
                if (isCat ? pet is Cat : pet is Dog && allocations.TryGetValue(pet.Manners, out int count))
                    allocations[pet.Manners]++;
            }

            // randomly choose from least allocated skins
            int minCount = allocations.Values.Min();
            int[] possibleSkins = allocations.Where(p => p.Value == minCount).Select(p => p.Key).ToArray();
            return possibleSkins[ModEntry.Random.Next(0, possibleSkins.Length)];
        }
    }
}
