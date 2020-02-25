using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace RoR2.Skills
{
	// Token: 0x020004BE RID: 1214
	[CreateAssetMenu(menuName = "RoR2/SkillFamily")]
	public class SkillFamily : ScriptableObject
	{
		// Token: 0x1700032B RID: 811
		// (get) Token: 0x06001D4D RID: 7501 RVA: 0x0000AC7F File Offset: 0x00008E7F
		[Obsolete("Accessing UnityEngine.Object.Name causes allocations on read. Look up the name from the catalog instead. If absolutely necessary to perform direct access, cast to ScriptableObject first.")]
		public new string name
		{
			get
			{
				return null;
			}
		}

		// Token: 0x1700032C RID: 812
		// (get) Token: 0x06001D4E RID: 7502 RVA: 0x0007CF1A File Offset: 0x0007B11A
		// (set) Token: 0x06001D4F RID: 7503 RVA: 0x0007CF22 File Offset: 0x0007B122
		public int catalogIndex { get; set; }

		// Token: 0x1700032D RID: 813
		// (get) Token: 0x06001D50 RID: 7504 RVA: 0x0007CF2B File Offset: 0x0007B12B
		public SkillDef defaultSkillDef
		{
			get
			{
				return this.variants[(int)this.defaultVariantIndex].skillDef;
			}
		}

		// Token: 0x06001D51 RID: 7505 RVA: 0x0007CF43 File Offset: 0x0007B143
		public void OnValidate()
		{
			if ((ulong)this.defaultVariantIndex >= (ulong)((long)this.variants.Length))
			{
				Debug.LogErrorFormat("Skill Family \"{0}\" defaultVariantIndex is outside the bounds of the variants array.", Array.Empty<object>());
			}
		}

		// Token: 0x06001D52 RID: 7506 RVA: 0x0007CF66 File Offset: 0x0007B166
		public string GetVariantName(int variantIndex)
		{
			return SkillCatalog.GetSkillName(this.variants[variantIndex].skillDef.skillIndex);
		}

		// Token: 0x06001D53 RID: 7507 RVA: 0x0007CF84 File Offset: 0x0007B184
		public int GetVariantIndex(string variantName)
		{
			for (int i = 0; i < this.variants.Length; i++)
			{
				if (this.GetVariantName(i).Equals(variantName, StringComparison.Ordinal))
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x04001A4D RID: 6733
		[FormerlySerializedAs("Entries")]
		public SkillFamily.Variant[] variants;

		// Token: 0x04001A4E RID: 6734
		[FormerlySerializedAs("defaultEntryIndex")]
		public uint defaultVariantIndex;

		// Token: 0x020004BF RID: 1215
		[Serializable]
		public struct Variant
		{
			// Token: 0x1700032E RID: 814
			// (get) Token: 0x06001D55 RID: 7509 RVA: 0x0007CFB7 File Offset: 0x0007B1B7
			// (set) Token: 0x06001D56 RID: 7510 RVA: 0x0007CFBF File Offset: 0x0007B1BF
			public ViewablesCatalog.Node viewableNode { get; set; }

			// Token: 0x04001A4F RID: 6735
			public SkillDef skillDef;

			// Token: 0x04001A50 RID: 6736
			public string unlockableName;
		}
	}
}
