using System.Collections.Generic;

namespace RPG.Stats
{
	public interface IModifierProvider
    {
        IEnumerable GetAdditiveModifier(Stat stat);
    }
}