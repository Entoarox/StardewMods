using System;
using System.Collections.Generic;

namespace Entoarox.Framework
{
    public interface IPlayerModifierHelper
    {
        int Count { get; }
        void Add(PlayerModifier modifier);
        void AddRange(IEnumerable<PlayerModifier> modifiers);
        void Remove(PlayerModifier modifier);
        void RemoveAll(Predicate<PlayerModifier> predicate);
        void Clear();
        bool Contains(PlayerModifier modifier);
        bool Exists(Predicate<PlayerModifier> predicate);
        bool TrueForAll(Predicate<PlayerModifier> predicate);
    }
}
