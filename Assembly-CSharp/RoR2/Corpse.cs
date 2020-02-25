using System;
using System.Collections.Generic;
using RoR2.ConVar;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020001C9 RID: 457
	public class Corpse : MonoBehaviour
	{
		// Token: 0x060009CC RID: 2508 RVA: 0x0002AD62 File Offset: 0x00028F62
		private void CollectRenderers()
		{
			if (this.renderers == null)
			{
				this.renderers = base.GetComponentsInChildren<Renderer>();
			}
		}

		// Token: 0x060009CD RID: 2509 RVA: 0x0002AD78 File Offset: 0x00028F78
		private void OnEnable()
		{
			Corpse.instancesList.Add(this);
			if (Corpse.disposalMode == Corpse.DisposalMode.OutOfSight)
			{
				this.CollectRenderers();
			}
		}

		// Token: 0x060009CE RID: 2510 RVA: 0x0002AD93 File Offset: 0x00028F93
		private void OnDisable()
		{
			Corpse.instancesList.Remove(this);
		}

		// Token: 0x060009CF RID: 2511 RVA: 0x0002ADA1 File Offset: 0x00028FA1
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		private static void StaticInit()
		{
			RoR2Application.onUpdate += Corpse.StaticUpdate;
		}

		// Token: 0x060009D0 RID: 2512 RVA: 0x0002ADB4 File Offset: 0x00028FB4
		private static void IncrementCurrentCheckIndex()
		{
			Corpse.currentCheckIndex++;
			if (Corpse.currentCheckIndex >= Corpse.instancesList.Count)
			{
				Corpse.currentCheckIndex = 0;
			}
		}

		// Token: 0x060009D1 RID: 2513 RVA: 0x0002ADDC File Offset: 0x00028FDC
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

		// Token: 0x060009D2 RID: 2514 RVA: 0x0002AE18 File Offset: 0x00029018
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

		// Token: 0x060009D3 RID: 2515 RVA: 0x0002AEC6 File Offset: 0x000290C6
		private static void DestroyCorpse(Corpse corpse)
		{
			if (corpse)
			{
				UnityEngine.Object.Destroy(corpse.gameObject);
			}
		}

		// Token: 0x040009FF RID: 2559
		private static readonly List<Corpse> instancesList = new List<Corpse>();

		// Token: 0x04000A00 RID: 2560
		private Renderer[] renderers;

		// Token: 0x04000A01 RID: 2561
		private static int maxCorpses = 25;

		// Token: 0x04000A02 RID: 2562
		private static Corpse.DisposalMode disposalMode = Corpse.DisposalMode.OutOfSight;

		// Token: 0x04000A03 RID: 2563
		private static int maxChecksPerUpdate = 3;

		// Token: 0x04000A04 RID: 2564
		private static int currentCheckIndex = 0;

		// Token: 0x020001CA RID: 458
		private class CorpsesMaxConVar : BaseConVar
		{
			// Token: 0x060009D6 RID: 2518 RVA: 0x0000972B File Offset: 0x0000792B
			private CorpsesMaxConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x060009D7 RID: 2519 RVA: 0x0002AF00 File Offset: 0x00029100
			public override void SetString(string newValue)
			{
				int maxCorpses;
				if (TextSerialization.TryParseInvariant(newValue, out maxCorpses))
				{
					Corpse.maxCorpses = maxCorpses;
				}
			}

			// Token: 0x060009D8 RID: 2520 RVA: 0x0002AF1D File Offset: 0x0002911D
			public override string GetString()
			{
				return TextSerialization.ToStringInvariant(Corpse.maxCorpses);
			}

			// Token: 0x04000A05 RID: 2565
			private static Corpse.CorpsesMaxConVar instance = new Corpse.CorpsesMaxConVar("corpses_max", ConVarFlags.Archive | ConVarFlags.Engine, "25", "The maximum number of corpses allowed.");
		}

		// Token: 0x020001CB RID: 459
		public enum DisposalMode
		{
			// Token: 0x04000A07 RID: 2567
			Hard,
			// Token: 0x04000A08 RID: 2568
			OutOfSight
		}

		// Token: 0x020001CC RID: 460
		private class CorpseDisposalConVar : BaseConVar
		{
			// Token: 0x060009DA RID: 2522 RVA: 0x0000972B File Offset: 0x0000792B
			private CorpseDisposalConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x060009DB RID: 2523 RVA: 0x0002AF48 File Offset: 0x00029148
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

			// Token: 0x060009DC RID: 2524 RVA: 0x0002AFE4 File Offset: 0x000291E4
			public override string GetString()
			{
				return Corpse.disposalMode.ToString();
			}

			// Token: 0x04000A09 RID: 2569
			private static Corpse.CorpseDisposalConVar instance = new Corpse.CorpseDisposalConVar("corpses_disposal", ConVarFlags.Archive | ConVarFlags.Engine, null, "The corpse disposal mode. Choices are Hard and OutOfSight.");
		}
	}
}
