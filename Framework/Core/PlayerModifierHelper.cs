using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entoarox.Framework.Core
{
#pragma warning disable CS0618
    internal class PlayerModifierHelper : IPlayerModifierHelper
    {
        private List<PlayerModifier> _Modifiers = new List<PlayerModifier>();
        private static PlayerModifier _Compound = new PlayerModifier(true)
        {
            ExperienceModifierCombat = 1,
            ExperienceModifierFarming = 1,
            ExperienceModifierFishing = 1,
            ExperienceModifierForaging = 1,
            ExperienceModifierMining = 1
        };
        private void _UpdateCompound()
        {
            _ResetModifiers();
            _Compound = new PlayerModifier(true)
            {
                ExperienceModifierCombat = 1,
                ExperienceModifierFarming = 1,
                ExperienceModifierFishing = 1,
                ExperienceModifierForaging = 1,
                ExperienceModifierMining = 1
            };
            foreach(PlayerModifier modifier in _Modifiers)
            {
                _Compound.ExperienceModifierCombat += modifier.ExperienceModifierCombat;
            }
            _Compound.Unlocked = false;
            _ApplyModifiers();
        }
        private void _ResetModifiers()
        {

        }
        private void _ApplyModifiers()
        {

        }
#pragma warning restore CS0618
        internal static void _UpdateModifiers()
        {

        }
        public int Count
        {
            get => _Modifiers.Count;
        }
        public void Add(PlayerModifier modifier)
        {
            _Modifiers.Add(modifier);
            _UpdateCompound();
        }
        public void AddRange(IEnumerable<PlayerModifier> modifiers)
        {
            _Modifiers.AddRange(modifiers);
            _UpdateCompound();
        }
        public void Remove(PlayerModifier modifier)
        {
            _Modifiers.Remove(modifier);
            _UpdateCompound();
        }
        public void RemoveAll(Predicate<PlayerModifier> predicate)
        {
            _Modifiers.RemoveAll(predicate);
            _UpdateCompound();
        }
        public void Clear()
        {
            _Modifiers.Clear();
            _UpdateCompound();
        }
        public bool Contains(PlayerModifier modifier) => _Modifiers.Contains(modifier);
        public bool Exists(Predicate<PlayerModifier> predicate) => _Modifiers.Exists(predicate);
        public bool TrueForAll(Predicate<PlayerModifier> predicate) => _Modifiers.TrueForAll(predicate);
    }
}
