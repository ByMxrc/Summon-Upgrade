using Terraria.ModLoader;

namespace SummonUpgrade
{
	public class SummonUpgrade : Mod
	{
		public static SummonUpgrade Instance { get; private set; }

		public override void Load()
		{
			Instance = this;
		}

		public override void Unload()
		{
			Instance = null;
		}
	}
}
