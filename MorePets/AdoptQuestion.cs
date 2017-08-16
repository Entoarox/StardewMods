using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Menus;

namespace MorePets
{
    internal class AdoptQuestion
    {
        private bool Cat;
        private int Skin;
        private StardewValley.Farmer Who = null;
        private AnimatedSprite Sprite;
        internal AdoptQuestion(bool cat, int skin)
        {
            this.Cat = cat;
            this.Skin = skin;

            var textures = this.Cat ? MorePetsMod.CatTextures : MorePetsMod.DogTextures;

            this.Sprite = new AnimatedSprite(textures[this.Skin], 28, 32, 32);
            this.Sprite.loop = true;
        }
        internal static void Show()
        {
            Random rnd = MorePetsMod.random;

            int catLimit = MorePetsMod.CatTextures.Length;
            int dogLimit = MorePetsMod.DogTextures.Length;

            bool cat = catLimit != 0 && (dogLimit == 0 || rnd.NextDouble() < 0.5);
            AdoptQuestion q = new AdoptQuestion(cat, rnd.Next(1, cat ? catLimit : dogLimit));
            GraphicsEvents.OnPostRenderHudEvent += q.Display;
            Game1.currentLocation.lastQuestionKey = "AdoptPetQuestion";
            Game1.currentLocation.createQuestionDialogue(
                "Oh dear, it looks like someone has abandoned a poor " + (cat ? "Cat" : "Dog") + " here! Perhaps you should pay Marnie " + MorePetsMod.Config.AdoptionPrice + " gold to give it a checkup so you can adopt it?",
                Game1.player.money < MorePetsMod.Config.AdoptionPrice ?
                    new Response[] {
                        new Response("n","Unfortunately I do not have the required "+MorePetsMod.Config.AdoptionPrice+" gold in order to do this.")
                    } :
                    new Response[] {
                        new Response("y","Yes, I really should adopt the poor animal!"),
                        new Response("n","No, I do not have the space to house it.")
                    },
                q.Resolver, null);
        }
        internal void Display(object o, EventArgs e)
        {
            if (Game1.activeClickableMenu != null && Game1.activeClickableMenu is DialogueBox)
            {
                DialogueBox box = (DialogueBox)Game1.activeClickableMenu;
                Vector2 c = new Vector2(Game1.viewport.Width / 2 - 128 * Game1.pixelZoom, Game1.viewport.Height - box.height - (56 * Game1.pixelZoom));
                Vector2 p = new Vector2(36 * Game1.pixelZoom + c.X, 32 * Game1.pixelZoom + c.Y);
                IClickableMenu.drawTextureBox(Game1.spriteBatch, Game1.menuTexture, new Rectangle(0, 256, 60, 60), (int)c.X, (int)c.Y, 40 * Game1.pixelZoom, 40 * Game1.pixelZoom, Color.White, 1, true);
                Game1.spriteBatch.Draw(this.Sprite.Texture, p, this.Sprite.SourceRect, Color.White, 0, new Vector2(this.Sprite.spriteWidth, this.Sprite.spriteHeight), Game1.pixelZoom, SpriteEffects.None, 0.991f);
                this.Sprite.Animate(Game1.currentGameTime, 28, 2, 500);
            }
            else
                GraphicsEvents.OnPostRenderHudEvent -= this.Display;
        }
        internal void Resolver(StardewValley.Farmer who, string answer)
        {
            GraphicsEvents.OnPostRenderHudEvent -= this.Display;
            if (answer == "n")
                return;
            this.Who = who;
            Game1.activeClickableMenu = new NamingMenu(this.Namer, "Choose a name");
        }
        internal void Namer(string petName)
        {
            NPC pet;
            this.Who.Money -= MorePetsMod.Config.AdoptionPrice;
            if (this.Cat)
            {
                pet = new Cat((int)Game1.player.position.X, (int)Game1.player.position.Y);
                pet.sprite = new AnimatedSprite(MorePetsMod.CatTextures[this.Skin], 0, 32, 32);
            }
            else
            {
                pet = new Dog(Game1.player.getTileLocationPoint().X, Game1.player.getTileLocationPoint().Y);
                pet.sprite = new AnimatedSprite(MorePetsMod.DogTextures[this.Skin], 0, 32, 32);
            }
            pet.name = petName;
            pet.manners = this.Skin;
            pet.age = Game1.year * 1000 + MorePetsMod.seasons.IndexOf(Game1.currentSeason) * 100 + Game1.dayOfMonth;
            pet.position = Game1.player.position;
            Game1.currentLocation.addCharacter(pet);
            (pet as Pet).warpToFarmHouse(this.Who);
            Game1.drawObjectDialogue("Marnie will bring " + petName + " to your house once they have their shots and been given a grooming.");
        }
    }
}
