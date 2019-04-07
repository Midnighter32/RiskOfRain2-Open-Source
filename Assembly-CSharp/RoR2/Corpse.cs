using System;
using System.Collections.Generic;
using RoR2.ConVar;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002BA RID: 698
	public class Corpse : MonoBehaviour
	{
		// Token: 0x06000E2D RID: 3629 RVA: 0x00045DE2 File Offset: 0x00043FE2
		private void CollectRenderers()
		{
			if (this.renderers == null)
			{
				this.renderers = base.GetComponentsInChildren<Renderer>();
			}
		}

		// Token: 0x06000E2E RID: 3630 RVA: 0x00045DF8 File Offset: 0x00043FF8
		private void OnEnable()
		{
			Corpse.instancesList.Add(this);
			if (Corpse.disposalMode == Corpse.DisposalMode.OutOfSight)
			{
				this.CollectRenderers();
			}
		}

		// Token: 0x06000E2F RID: 3631 RVA: 0x00045E13 File Offset: 0x00044013
		private void OnDisable()
		{
			Corpse.instancesList.Remove(this);
		}

		// Token: 0x06000E30 RID: 3632 RVA: 0x00045E21 File Offset: 0x00044021
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		private static void StaticInit()
		{
			RoR2Application.onUpdate += Corpse.StaticUpdate;
		}

		// Token: 0x06000E31 RID: 3633 RVA: 0x00045E34 File Offset: 0x00044034
		private static void IncrementCurrentCheckIndex()
		{
			Corpse.currentCheckIndex++;
			if (Corpse.currentCheckIndex >= Corpse.instancesList.Count)
			{
				Corpse.currentCheckIndex = 0;
			}
		}

		// Token: 0x06000E32 RID: 3634 RVA: 0x00045E5C File Offset: 0x0004405C
		private static bool CheckCorpseOutOfSight(Corpse corpse)
		{
			foreach (Renderer renderer in corpse.renderers)
			{
				if (renderer && renderer.isVisible)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06000E33 RID: 3635 RVA: 0x00045E98 File Offset: 0x00044098
		private static void StaticUpdate()
		{
			if (Corpse.maxCorpses < 0)
			{
				return;
			}
			int num = Corpse.instancesList.Count - Corpse.maxCorpses;
			int num2 = Math.Min(Math.Min(num, Corpse.maxChecksPerUpdate), Corpse.instancesList.Count);
			Corpse.DisposalMode disposalMode = Corpse.disposalMode;
			if (disposalMode == Corpse.DisposalMode.Hard)
			{
				for (int i = num - 1; i >= 0; i--)
				{
					Corpse.DestroyCorpse(Corpse.instancesList[i]);
				}
				return;
			}
			if (disposalMode != Corpse.DisposalMode.OutOfSight)
			{
				return;
			}
			for (int j = 0; j < num2; j++)
			{
				Corpse.IncrementCurrentCheckIndex();
				if (Corpse.CheckCorpseOutOfSight(Corpse.instancesList[Corpse.currentCheckIndex]))
				{
					Corpse.DestroyCorpse(Corpse.instancesList[Corpse.currentCheckIndex]);
				}
			}
		}

		// Token: 0x06000E34 RID: 3636 RVA: 0x00045F46 File Offset: 0x00044146
		private static void DestroyCorpse(Corpse corpse)
		{
			if (corpse)
			{
				UnityEngine.Object.Destroy(corpse.gameObject);
			}
		}

		// Token: 0x04001217 RID: 4631
		private static readonly List<Corpse> instancesList = new List<Corpse>();

		// Token: 0x04001218 RID: 4632
		private Renderer[] renderers;

		// Token: 0x04001219 RID: 4633
		private static int maxCorpses = 25;

		// Token: 0x0400121A RID: 4634
		private static Corpse.DisposalMode disposalMode = Corpse.DisposalMode.OutOfSight;

		// Token: 0x0400121B RID: 4635
		private static int maxChecksPerUpdate = 3;

		// Token: 0x0400121C RID: 4636
		private static int currentCheckIndex = 0;

		// Token: 0x020002BB RID: 699
		private class CorpsesMaxConVar : BaseConVar
		{
			// Token: 0x06000E37 RID: 3639 RVA: 0x00037E38 File Offset: 0x00036038
			private CorpsesMaxConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x06000E38 RID: 3640 RVA: 0x00045F80 File Offset: 0x00044180
			public override void SetString(string newValue)
			{
				int maxCorpses;
				if (TextSerialization.TryParseInvariant(newValue, out maxCorpses))
				{
					Corpse.maxCorpses = maxCorpses;
				}
			}

			// Token: 0x06000E39 RID: 3641 RVA: 0x00045F9D File Offset: 0x0004419D
			public override string GetString()
			{
				return TextSerialization.ToStringInvariant(Corpse.maxCorpses);
			}

			// Token: 0x0400121D RID: 4637
			private static Corpse.CorpsesMaxConVar instance = new Corpse.CorpsesMaxConVar("corpses_max", ConVarFlags.Archive | ConVarFlags.Engine, "25", "The maximum number of corpses allowed.");
		}

		// Token: 0x020002BC RID: 700
		public enum DisposalMode
		{
			// Token: 0x0400121F RID: 4639
			Hard,
			// Token: 0x04001220 RID: 4640
			OutOfSight
		}

		// Token: 0x020002BD RID: 701
		private class CorpseDisposalConVar : BaseConVar
		{
			// Token: 0x06000E3B RID: 3643 RVA: 0x00037E38 File Offset: 0x00036038
			private CorpseDisposalConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x06000E3C RID: 3644 RVA: 0x00045FC8 File Offset: 0x000441C8
			public override void SetString(string newValue)
			{
				try
				{
					Corpse.DisposalMode disposalMode = (Corpse.DisposalMode)Enum.Parse(typeof(Corpse.DisposalMode), newValue, true);
					if (disposalMode != Corpse.disposalMode)
					{
						Corpse.disposalMode = disposalMode;
						if (disposalMode != Corpse.DisposalMode.Hard && disposalMode == Corpse.DisposalMode.OutOfSight)
						{
							foreach (Corpse corpse in Corpse.instancesList)
							{
								corpse.CollectRenderers();
							}
						}
					}
				}
				catch (ArgumentException)
				{
					Console.ShowHelpText(this.name);
				}
			}

			// Token: 0x06000E3D RID: 3645 RVA: 0x00046064 File Offset: 0x00044264
			public override string GetString()
			{
				return Corpse.disposalMode.ToString();
			}

			// Token: 0x04001221 RID: 4641
			private static Corpse.CorpseDisposalConVar instance = new Corpse.CorpseDisposalConVar("corpses_disposal", ConVarFlags.Archive | ConVarFlags.Engine, null, "The corpse disposal mode. Choices are Hard and OutOfSight.");
		}
	}
}
