using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entoarox.Framework.Abstraction
{
    public class Character
    {
        private StardewValley.NPC Char;
        internal Character(StardewValley.NPC character)
        {
            Char = character;
        }
        public string Name => Char.getName();
        public GameLocation CurrentLocation => GameLocation.Wrap(Char.currentLocation);
        public bool IsDateable => Char.datable;
        public bool IsDatingFarmer => Char.datingFarmer;
        public bool IsDivorcedFromFarmer => Char.divorcedFromFarmer;
        public int DaysMarried => Char.daysMarried;
        public int Birthday_Day => Char.birthday_Day;
        public string Birthday_Season => Char.birthday_Season;
        public Genders Gender => (Genders)Char.gender;
        public CharacterAges Age => (CharacterAges)Char.age;
        public CharacterManners Manners => (CharacterManners)Char.manners;
        public CharacterAnxieties Anxiety => (CharacterAnxieties)Char.socialAnxiety;
        public CharacterOptimisms Optimism => (CharacterOptimisms)Char.optimism;
        public HomeRegions HomeRegion => (HomeRegions)Char.homeRegion;
        public int DaysUntilChild
        {
            get
            {
                return Char.daysUntilBirthing;
            }
            set
            {
                Char.daysUntilBirthing = value;
            }
        }
        public int DaysBeforeNextChild
        {
            get
            {
                return Char.daysAfterLastBirth+1;
            }
            set
            {
                Char.daysAfterLastBirth = value-1;
            }
        }
        public void ShowTextAboveHead(string text, int spriteTextColor = -1, int style = 2, int duration = 3000, int delay = 0)
        {
            Char.showTextAboveHead(text, spriteTextColor, style, duration, delay);
        }
        public bool CanReceiveItemAsGift(StardewValley.Item item)
        {
            return item is StardewValley.Object || item is StardewValley.Objects.Ring || item is StardewValley.Objects.Hat || item is StardewValley.Objects.Boots || item is StardewValley.Tools.MeleeWeapon;
        }
        public GiftLikes GetGiftTasteForItem(StardewValley.Item item)
        {
            return (GiftLikes)Char.getGiftTasteForThisItem(item);
        }
        public void AddDialogue(string dialogue)
        {
            Char.setNewDialogue(dialogue, true);
        }
        public void SetDialogue(string dialogue)
        {
            Char.setNewDialogue(dialogue);
        }
        public StardewValley.Object GetFavoriteItem()
        {
            return Char.getFavoriteItem();
        }
        public Microsoft.Xna.Framework.Rectangle GetMugshotRectangle()
        {
            return Char.getMugShotSourceRect();
        }
        public bool CanHaveChild()
        {
            return Char.canGetPregnant();
        }
    }
}
