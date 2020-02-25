using System;
using System.Collections.Generic;
using RoR2.Skills;

namespace RoR2
{
	// Token: 0x0200040C RID: 1036
	[Serializable]
	public class SerializableLoadout
	{
		// Token: 0x170002F2 RID: 754
		// (get) Token: 0x06001929 RID: 6441 RVA: 0x0006CBE7 File Offset: 0x0006ADE7
		public bool isEmpty
		{
			get
			{
				return this.bodyLoadouts.Length == 0;
			}
		}

		// Token: 0x0600192A RID: 6442 RVA: 0x0006CBF3 File Offset: 0x0006ADF3
		public void Apply(Loadout loadout)
		{
			if (this.loadoutBuilder == null)
			{
				this.loadoutBuilder = new SerializableLoadout.LoadoutBuilder(this);
			}
			this.loadoutBuilder.Apply(loadout);
		}

		// Token: 0x0400177B RID: 6011
		public SerializableLoadout.BodyLoadout[] bodyLoadouts = Array.Empty<SerializableLoadout.BodyLoadout>();

		// Token: 0x0400177C RID: 6012
		private SerializableLoadout.LoadoutBuilder loadoutBuilder;

		// Token: 0x0200040D RID: 1037
		[Serializable]
		public struct BodyLoadout
		{
			// Token: 0x0400177D RID: 6013
			public CharacterBody body;

			// Token: 0x0400177E RID: 6014
			public SerializableLoadout.BodyLoadout.SkillChoice[] skillChoices;

			// Token: 0x0400177F RID: 6015
			public SkinDef skinChoice;

			// Token: 0x0200040E RID: 1038
			[Serializable]
			public struct SkillChoice
			{
				// Token: 0x04001780 RID: 6016
				public SkillFamily skillFamily;

				// Token: 0x04001781 RID: 6017
				public SkillDef variant;
			}
		}

		// Token: 0x0200040F RID: 1039
		private class LoadoutBuilder
		{
			// Token: 0x0600192C RID: 6444 RVA: 0x0006CC28 File Offset: 0x0006AE28
			public LoadoutBuilder(SerializableLoadout serializedLoadout)
			{
				SerializableLoadout.BodyLoadout[] bodyLoadouts = serializedLoadout.bodyLoadouts;
				List<SerializableLoadout.LoadoutBuilder.SkillSetter> list = new List<SerializableLoadout.LoadoutBuilder.SkillSetter>(8);
				List<SerializableLoadout.LoadoutBuilder.SkinSetter> list2 = new List<SerializableLoadout.LoadoutBuilder.SkinSetter>(bodyLoadouts.Length);
				for (int i = 0; i < bodyLoadouts.Length; i++)
				{
					ref SerializableLoadout.BodyLoadout ptr = ref bodyLoadouts[i];
					CharacterBody body = ptr.body;
					if (body)
					{
						int bodyIndex = body.bodyIndex;
						GenericSkill[] bodyPrefabSkillSlots = BodyCatalog.GetBodyPrefabSkillSlots(bodyIndex);
						SerializableLoadout.BodyLoadout.SkillChoice[] skillChoices = ptr.skillChoices;
						int j = 0;
						while (j < skillChoices.Length)
						{
							ref SerializableLoadout.BodyLoadout.SkillChoice ptr2 = ref skillChoices[j];
							int num = SerializableLoadout.LoadoutBuilder.FindSkillSlotIndex(bodyPrefabSkillSlots, ptr2.skillFamily);
							int num2 = SerializableLoadout.LoadoutBuilder.FindSkillVariantIndex(ptr2.skillFamily, ptr2.variant);
							if (num != -1 && num2 != -1)
							{
								list.Add(new SerializableLoadout.LoadoutBuilder.SkillSetter(bodyIndex, num, (uint)num2));
							}
							i++;
						}
						int num3 = Array.IndexOf<SkinDef>(BodyCatalog.GetBodySkins(bodyIndex), ptr.skinChoice);
						if (num3 != -1)
						{
							list2.Add(new SerializableLoadout.LoadoutBuilder.SkinSetter(bodyIndex, (uint)num3));
						}
					}
				}
				this.skillSetters = list.ToArray();
				this.skinSetters = list2.ToArray();
			}

			// Token: 0x0600192D RID: 6445 RVA: 0x0006CD3C File Offset: 0x0006AF3C
			public void Apply(Loadout loadout)
			{
				for (int i = 0; i < this.skillSetters.Length; i++)
				{
					ref SerializableLoadout.LoadoutBuilder.SkillSetter ptr = ref this.skillSetters[i];
					loadout.bodyLoadoutManager.SetSkillVariant(ptr.bodyIndex, ptr.skillSlotIndex, ptr.skillVariantIndex);
				}
				for (int j = 0; j < this.skinSetters.Length; j++)
				{
					ref SerializableLoadout.LoadoutBuilder.SkinSetter ptr2 = ref this.skinSetters[j];
					loadout.bodyLoadoutManager.SetSkinIndex(ptr2.bodyIndex, ptr2.skinIndex);
				}
			}

			// Token: 0x0600192E RID: 6446 RVA: 0x0006CDC0 File Offset: 0x0006AFC0
			private static int FindSkillSlotIndex(GenericSkill[] skillSlots, SkillFamily skillFamily)
			{
				for (int i = 0; i < skillSlots.Length; i++)
				{
					if (skillSlots[i].skillFamily == skillFamily)
					{
						return i;
					}
				}
				return -1;
			}

			// Token: 0x0600192F RID: 6447 RVA: 0x0006CDEC File Offset: 0x0006AFEC
			private static int FindSkillVariantIndex(SkillFamily skillFamily, SkillDef skillDef)
			{
				for (int i = 0; i < skillFamily.variants.Length; i++)
				{
					if (skillFamily.variants[i].skillDef == skillDef)
					{
						return i;
					}
				}
				return -1;
			}

			// Token: 0x04001782 RID: 6018
			private readonly SerializableLoadout.LoadoutBuilder.SkillSetter[] skillSetters;

			// Token: 0x04001783 RID: 6019
			private readonly SerializableLoadout.LoadoutBuilder.SkinSetter[] skinSetters;

			// Token: 0x02000410 RID: 1040
			private struct SkillSetter
			{
				// Token: 0x06001930 RID: 6448 RVA: 0x0006CE23 File Offset: 0x0006B023
				public SkillSetter(int bodyIndex, int skillSlotIndex, uint skillVariantIndex)
				{
					this.bodyIndex = bodyIndex;
					this.skillSlotIndex = skillSlotIndex;
					this.skillVariantIndex = skillVariantIndex;
				}

				// Token: 0x04001784 RID: 6020
				public readonly int bodyIndex;

				// Token: 0x04001785 RID: 6021
				public readonly int skillSlotIndex;

				// Token: 0x04001786 RID: 6022
				public readonly uint skillVariantIndex;
			}

			// Token: 0x02000411 RID: 1041
			private struct SkinSetter
			{
				// Token: 0x06001931 RID: 6449 RVA: 0x0006CE3A File Offset: 0x0006B03A
				public SkinSetter(int bodyIndex, uint skinIndex)
				{
					this.bodyIndex = bodyIndex;
					this.skinIndex = skinIndex;
				}

				// Token: 0x04001787 RID: 6023
				public readonly int bodyIndex;

				// Token: 0x04001788 RID: 6024
				public readonly uint skinIndex;
			}
		}
	}
}
