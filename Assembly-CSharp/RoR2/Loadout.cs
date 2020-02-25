using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Linq;
using JetBrains.Annotations;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020003BB RID: 955
	public class Loadout
	{
		// Token: 0x0600171C RID: 5916 RVA: 0x000649C0 File Offset: 0x00062BC0
		public void Serialize(NetworkWriter writer)
		{
			this.bodyLoadoutManager.Serialize(writer);
		}

		// Token: 0x0600171D RID: 5917 RVA: 0x000649CE File Offset: 0x00062BCE
		public void Deserialize(NetworkReader reader)
		{
			this.bodyLoadoutManager.Deserialize(reader);
		}

		// Token: 0x0600171E RID: 5918 RVA: 0x000649DC File Offset: 0x00062BDC
		public void Copy(Loadout dest)
		{
			this.bodyLoadoutManager.Copy(dest.bodyLoadoutManager);
		}

		// Token: 0x0600171F RID: 5919 RVA: 0x000649EF File Offset: 0x00062BEF
		public bool ValueEquals(Loadout other)
		{
			return this == other || (other != null && this.bodyLoadoutManager.ValueEquals(other.bodyLoadoutManager));
		}

		// Token: 0x06001720 RID: 5920 RVA: 0x00064A0D File Offset: 0x00062C0D
		public XElement ToXml(string elementName)
		{
			XElement xelement = new XElement(elementName);
			xelement.Add(this.bodyLoadoutManager.ToXml("BodyLoadouts"));
			return xelement;
		}

		// Token: 0x06001721 RID: 5921 RVA: 0x00064A30 File Offset: 0x00062C30
		public bool FromXml(XElement element)
		{
			bool flag = true;
			XElement xelement = element.Element("BodyLoadouts");
			return xelement != null && (flag & this.bodyLoadoutManager.FromXml(xelement));
		}

		// Token: 0x06001722 RID: 5922 RVA: 0x00064A67 File Offset: 0x00062C67
		public void EnforceUnlockables(UserProfile userProfile)
		{
			this.bodyLoadoutManager.EnforceUnlockables(userProfile);
		}

		// Token: 0x06001723 RID: 5923 RVA: 0x00064A78 File Offset: 0x00062C78
		private static void GenerateViewables()
		{
			StringBuilder stringBuilder = new StringBuilder();
			ViewablesCatalog.Node node = new ViewablesCatalog.Node("Loadout", true, null);
			ViewablesCatalog.Node parent = new ViewablesCatalog.Node("Bodies", true, node);
			for (int i = 0; i < BodyCatalog.bodyCount; i++)
			{
				if (SurvivorCatalog.GetSurvivorIndexFromBodyIndex(i) != SurvivorIndex.None)
				{
					string bodyName = BodyCatalog.GetBodyName(i);
					GenericSkill[] bodyPrefabSkillSlots = BodyCatalog.GetBodyPrefabSkillSlots(i);
					ViewablesCatalog.Node parent2 = new ViewablesCatalog.Node(bodyName, true, parent);
					for (int j = 0; j < bodyPrefabSkillSlots.Length; j++)
					{
						SkillFamily skillFamily = bodyPrefabSkillSlots[j].skillFamily;
						if (skillFamily.variants.Length > 1)
						{
							string skillFamilyName = SkillCatalog.GetSkillFamilyName(skillFamily.catalogIndex);
							uint num = 0U;
							while ((ulong)num < (ulong)((long)skillFamily.variants.Length))
							{
								ref SkillFamily.Variant ptr = ref skillFamily.variants[(int)num];
								string unlockableName = ptr.unlockableName;
								if (!string.IsNullOrEmpty(unlockableName))
								{
									string skillName = SkillCatalog.GetSkillName(ptr.skillDef.skillIndex);
									stringBuilder.Append(skillFamilyName).Append(".").Append(skillName);
									string name = stringBuilder.ToString();
									stringBuilder.Clear();
									ViewablesCatalog.Node variantNode = new ViewablesCatalog.Node(name, false, parent2);
									ptr.viewableNode = variantNode;
									variantNode.shouldShowUnviewed = ((UserProfile userProfile) => !userProfile.HasViewedViewable(variantNode.fullName) && userProfile.HasUnlockable(unlockableName));
								}
								num += 1U;
							}
						}
					}
					SkinDef[] bodySkins = BodyCatalog.GetBodySkins(i);
					if (bodySkins.Length > 1)
					{
						ViewablesCatalog.Node parent3 = new ViewablesCatalog.Node("Skins", true, parent2);
						for (int k = 0; k < bodySkins.Length; k++)
						{
							string unlockableName = bodySkins[k].unlockableName;
							if (!string.IsNullOrEmpty(unlockableName))
							{
								ViewablesCatalog.Node skinNode = new ViewablesCatalog.Node(bodySkins[k].name, false, parent3);
								skinNode.shouldShowUnviewed = ((UserProfile userProfile) => !userProfile.HasViewedViewable(skinNode.fullName) && userProfile.HasUnlockable(unlockableName));
							}
						}
					}
				}
			}
			ViewablesCatalog.AddNodeToRoot(node);
		}

		// Token: 0x04001610 RID: 5648
		public readonly Loadout.BodyLoadoutManager bodyLoadoutManager = new Loadout.BodyLoadoutManager();

		// Token: 0x020003BC RID: 956
		public class BodyLoadoutManager
		{
			// Token: 0x06001725 RID: 5925 RVA: 0x00064CA0 File Offset: 0x00062EA0
			private int FindModifiedBodyLoadoutIndexByBodyIndex(int bodyIndex)
			{
				for (int i = 0; i < this.modifiedBodyLoadouts.Length; i++)
				{
					if (this.modifiedBodyLoadouts[i].bodyIndex == bodyIndex)
					{
						return i;
					}
				}
				return -1;
			}

			// Token: 0x06001726 RID: 5926 RVA: 0x00064CD4 File Offset: 0x00062ED4
			private Loadout.BodyLoadoutManager.BodyLoadout GetReadOnlyBodyLoadout(int bodyIndex)
			{
				int num = this.FindModifiedBodyLoadoutIndexByBodyIndex(bodyIndex);
				if (num == -1)
				{
					return Loadout.BodyLoadoutManager.defaultBodyLoadouts[bodyIndex];
				}
				return this.modifiedBodyLoadouts[num];
			}

			// Token: 0x06001727 RID: 5927 RVA: 0x00064D00 File Offset: 0x00062F00
			private Loadout.BodyLoadoutManager.BodyLoadout GetOrCreateModifiedBodyLoadout(int bodyIndex)
			{
				int num = this.FindModifiedBodyLoadoutIndexByBodyIndex(bodyIndex);
				if (num != -1)
				{
					return this.modifiedBodyLoadouts[num];
				}
				Loadout.BodyLoadoutManager.BodyLoadout result = Loadout.BodyLoadoutManager.GetDefaultLoadoutForBody(bodyIndex).Clone();
				HGArrayUtilities.ArrayAppend<Loadout.BodyLoadoutManager.BodyLoadout>(ref this.modifiedBodyLoadouts, ref result);
				return result;
			}

			// Token: 0x06001728 RID: 5928 RVA: 0x00064D3C File Offset: 0x00062F3C
			public uint GetSkillVariant(int bodyIndex, int skillSlot)
			{
				return this.GetReadOnlyBodyLoadout(bodyIndex).skillPreferences[skillSlot];
			}

			// Token: 0x06001729 RID: 5929 RVA: 0x00064D4C File Offset: 0x00062F4C
			public void SetSkillVariant(int bodyIndex, int skillSlot, uint skillVariant)
			{
				if (this.GetSkillVariant(bodyIndex, skillSlot) == skillVariant)
				{
					return;
				}
				Loadout.BodyLoadoutManager.BodyLoadout orCreateModifiedBodyLoadout = this.GetOrCreateModifiedBodyLoadout(bodyIndex);
				orCreateModifiedBodyLoadout.SetSkillVariant(skillSlot, skillVariant);
				this.RemoveBodyLoadoutIfDefault(orCreateModifiedBodyLoadout);
			}

			// Token: 0x0600172A RID: 5930 RVA: 0x00064D7D File Offset: 0x00062F7D
			public uint GetSkinIndex(int bodyIndex)
			{
				return this.GetReadOnlyBodyLoadout(bodyIndex).skinPreference;
			}

			// Token: 0x0600172B RID: 5931 RVA: 0x00064D8C File Offset: 0x00062F8C
			public void SetSkinIndex(int bodyIndex, uint skinIndex)
			{
				Loadout.BodyLoadoutManager.BodyLoadout orCreateModifiedBodyLoadout = this.GetOrCreateModifiedBodyLoadout(bodyIndex);
				orCreateModifiedBodyLoadout.skinPreference = skinIndex;
				this.RemoveBodyLoadoutIfDefault(orCreateModifiedBodyLoadout);
			}

			// Token: 0x0600172C RID: 5932 RVA: 0x00064DAF File Offset: 0x00062FAF
			private void RemoveBodyLoadoutIfDefault(Loadout.BodyLoadoutManager.BodyLoadout bodyLoadout)
			{
				this.RemoveBodyLoadoutIfDefault(this.FindModifiedBodyLoadoutIndexByBodyIndex(bodyLoadout.bodyIndex));
			}

			// Token: 0x0600172D RID: 5933 RVA: 0x00064DC3 File Offset: 0x00062FC3
			private void RemoveBodyLoadoutIfDefault(int modifiedBodyLoadoutIndex)
			{
				Loadout.BodyLoadoutManager.BodyLoadout bodyLoadout = this.modifiedBodyLoadouts[modifiedBodyLoadoutIndex];
				if (bodyLoadout.ValueEquals(Loadout.BodyLoadoutManager.GetDefaultLoadoutForBody(bodyLoadout.bodyIndex)))
				{
					this.RemoveBodyLoadoutAt(modifiedBodyLoadoutIndex);
				}
			}

			// Token: 0x0600172E RID: 5934 RVA: 0x00064DE6 File Offset: 0x00062FE6
			private void RemoveBodyLoadoutAt(int i)
			{
				HGArrayUtilities.ArrayRemoveAtAndResize<Loadout.BodyLoadoutManager.BodyLoadout>(ref this.modifiedBodyLoadouts, i, 1);
			}

			// Token: 0x0600172F RID: 5935 RVA: 0x00064DF5 File Offset: 0x00062FF5
			private static Loadout.BodyLoadoutManager.BodyLoadout GetDefaultLoadoutForBody(int bodyIndex)
			{
				return Loadout.BodyLoadoutManager.defaultBodyLoadouts[bodyIndex];
			}

			// Token: 0x06001730 RID: 5936 RVA: 0x00064DFE File Offset: 0x00062FFE
			private static int GetSkillSlotCountForBody(int bodyIndex)
			{
				return Loadout.BodyLoadoutManager.allBodyInfos[bodyIndex].skillSlotCount;
			}

			// Token: 0x06001731 RID: 5937 RVA: 0x00064E10 File Offset: 0x00063010
			[SystemInitializer(new Type[]
			{
				typeof(SkillCatalog),
				typeof(BodyCatalog)
			})]
			private static void Init()
			{
				Loadout.BodyLoadoutManager.defaultBodyLoadouts = new Loadout.BodyLoadoutManager.BodyLoadout[BodyCatalog.bodyCount];
				Loadout.BodyLoadoutManager.allBodyInfos = new Loadout.BodyLoadoutManager.BodyInfo[Loadout.BodyLoadoutManager.defaultBodyLoadouts.Length];
				for (int i = 0; i < Loadout.BodyLoadoutManager.defaultBodyLoadouts.Length; i++)
				{
					Loadout.BodyLoadoutManager.BodyInfo bodyInfo = default(Loadout.BodyLoadoutManager.BodyInfo);
					bodyInfo.prefabSkillSlots = BodyCatalog.GetBodyPrefabBodyComponent(i).GetComponents<GenericSkill>();
					bodyInfo.skillFamilyIndices = new int[bodyInfo.skillSlotCount];
					for (int j = 0; j < bodyInfo.prefabSkillSlots.Length; j++)
					{
						int[] skillFamilyIndices = bodyInfo.skillFamilyIndices;
						int num = j;
						SkillFamily skillFamily = bodyInfo.prefabSkillSlots[j].skillFamily;
						skillFamilyIndices[num] = ((skillFamily != null) ? skillFamily.catalogIndex : -1);
					}
					Loadout.BodyLoadoutManager.allBodyInfos[i] = bodyInfo;
					uint[] array = new uint[bodyInfo.skillSlotCount];
					for (int k = 0; k < bodyInfo.prefabSkillSlots.Length; k++)
					{
						array[k] = bodyInfo.prefabSkillSlots[k].skillFamily.defaultVariantIndex;
					}
					Loadout.BodyLoadoutManager.defaultBodyLoadouts[i] = new Loadout.BodyLoadoutManager.BodyLoadout
					{
						bodyIndex = i,
						skinPreference = 0U,
						skillPreferences = array
					};
				}
				Loadout.GenerateViewables();
			}

			// Token: 0x06001732 RID: 5938 RVA: 0x00064F24 File Offset: 0x00063124
			public void Copy(Loadout.BodyLoadoutManager dest)
			{
				Array.Resize<Loadout.BodyLoadoutManager.BodyLoadout>(ref dest.modifiedBodyLoadouts, this.modifiedBodyLoadouts.Length);
				for (int i = 0; i < this.modifiedBodyLoadouts.Length; i++)
				{
					dest.modifiedBodyLoadouts[i] = this.modifiedBodyLoadouts[i].Clone();
				}
			}

			// Token: 0x06001733 RID: 5939 RVA: 0x00064F6C File Offset: 0x0006316C
			public void Serialize(NetworkWriter writer)
			{
				writer.WritePackedUInt32((uint)this.modifiedBodyLoadouts.Length);
				for (int i = 0; i < this.modifiedBodyLoadouts.Length; i++)
				{
					this.modifiedBodyLoadouts[i].Serialize(writer);
				}
			}

			// Token: 0x06001734 RID: 5940 RVA: 0x00064FA8 File Offset: 0x000631A8
			public void Deserialize(NetworkReader reader)
			{
				try
				{
					int num = (int)reader.ReadPackedUInt32();
					if (num > BodyCatalog.bodyCount)
					{
						num = BodyCatalog.bodyCount;
					}
					Array.Resize<Loadout.BodyLoadoutManager.BodyLoadout>(ref this.modifiedBodyLoadouts, num);
					for (int i = 0; i < num; i++)
					{
						Loadout.BodyLoadoutManager.BodyLoadout bodyLoadout = new Loadout.BodyLoadoutManager.BodyLoadout();
						bodyLoadout.Deserialize(reader);
						this.modifiedBodyLoadouts[i] = bodyLoadout;
					}
				}
				catch (Exception ex)
				{
					this.modifiedBodyLoadouts = Array.Empty<Loadout.BodyLoadoutManager.BodyLoadout>();
					throw ex;
				}
			}

			// Token: 0x06001735 RID: 5941 RVA: 0x00065018 File Offset: 0x00063218
			public bool ValueEquals(Loadout.BodyLoadoutManager other)
			{
				if (this == other)
				{
					return true;
				}
				if (other == null)
				{
					return false;
				}
				if (this.modifiedBodyLoadouts.Length != other.modifiedBodyLoadouts.Length)
				{
					return false;
				}
				for (int i = 0; i < this.modifiedBodyLoadouts.Length; i++)
				{
					if (!this.modifiedBodyLoadouts[i].ValueEquals(other.modifiedBodyLoadouts[i]))
					{
						return false;
					}
				}
				return true;
			}

			// Token: 0x06001736 RID: 5942 RVA: 0x00065074 File Offset: 0x00063274
			public XElement ToXml(string elementName)
			{
				XElement xelement = new XElement(elementName);
				for (int i = 0; i < this.modifiedBodyLoadouts.Length; i++)
				{
					xelement.Add(this.modifiedBodyLoadouts[i].ToXml("BodyLoadout"));
				}
				return xelement;
			}

			// Token: 0x06001737 RID: 5943 RVA: 0x000650BC File Offset: 0x000632BC
			public bool FromXml(XElement element)
			{
				Loadout.BodyLoadoutManager.<>c__DisplayClass23_0 CS$<>8__locals1;
				CS$<>8__locals1.bodyLoadouts = new List<Loadout.BodyLoadoutManager.BodyLoadout>();
				foreach (XElement element2 in element.Elements("BodyLoadout"))
				{
					Loadout.BodyLoadoutManager.BodyLoadout bodyLoadout = new Loadout.BodyLoadoutManager.BodyLoadout();
					if (bodyLoadout.FromXml(element2) && !Loadout.BodyLoadoutManager.<FromXml>g__BodyLoadoutAlreadyDefined|23_0(bodyLoadout.bodyIndex, ref CS$<>8__locals1) && !bodyLoadout.ValueEquals(Loadout.BodyLoadoutManager.GetDefaultLoadoutForBody(bodyLoadout.bodyIndex)))
					{
						CS$<>8__locals1.bodyLoadouts.Add(bodyLoadout);
					}
				}
				this.modifiedBodyLoadouts = CS$<>8__locals1.bodyLoadouts.ToArray();
				return true;
			}

			// Token: 0x06001738 RID: 5944 RVA: 0x00065168 File Offset: 0x00063368
			public void EnforceUnlockables(UserProfile userProfile)
			{
				for (int i = this.modifiedBodyLoadouts.Length - 1; i >= 0; i--)
				{
					this.modifiedBodyLoadouts[i].EnforceUnlockables(userProfile);
					this.RemoveBodyLoadoutIfDefault(i);
				}
			}

			// Token: 0x0600173A RID: 5946 RVA: 0x000651B4 File Offset: 0x000633B4
			[CompilerGenerated]
			internal static bool <FromXml>g__BodyLoadoutAlreadyDefined|23_0(int bodyIndex, ref Loadout.BodyLoadoutManager.<>c__DisplayClass23_0 A_1)
			{
				for (int i = 0; i < A_1.bodyLoadouts.Count; i++)
				{
					if (A_1.bodyLoadouts[i].bodyIndex == bodyIndex)
					{
						return true;
					}
				}
				return false;
			}

			// Token: 0x04001611 RID: 5649
			private Loadout.BodyLoadoutManager.BodyLoadout[] modifiedBodyLoadouts = Array.Empty<Loadout.BodyLoadoutManager.BodyLoadout>();

			// Token: 0x04001612 RID: 5650
			private static Loadout.BodyLoadoutManager.BodyLoadout[] defaultBodyLoadouts;

			// Token: 0x04001613 RID: 5651
			private static Loadout.BodyLoadoutManager.BodyInfo[] allBodyInfos;

			// Token: 0x020003BD RID: 957
			private sealed class BodyLoadout
			{
				// Token: 0x0600173B RID: 5947 RVA: 0x000651EE File Offset: 0x000633EE
				[NotNull]
				public Loadout.BodyLoadoutManager.BodyLoadout Clone()
				{
					return new Loadout.BodyLoadoutManager.BodyLoadout
					{
						bodyIndex = this.bodyIndex,
						skinPreference = this.skinPreference,
						skillPreferences = (uint[])this.skillPreferences.Clone()
					};
				}

				// Token: 0x0600173C RID: 5948 RVA: 0x00065224 File Offset: 0x00063424
				public bool ValueEquals(Loadout.BodyLoadoutManager.BodyLoadout other)
				{
					return this == other || (other != null && this.bodyIndex.Equals(other.bodyIndex) && this.skinPreference.Equals(other.skinPreference) && ((IStructuralEquatable)this.skillPreferences).Equals(other.skillPreferences, EqualityComparer<uint>.Default));
				}

				// Token: 0x0600173D RID: 5949 RVA: 0x0006527C File Offset: 0x0006347C
				public uint GetSkillVariant(int skillSlotIndex)
				{
					if ((ulong)skillSlotIndex < (ulong)((long)this.skillPreferences.Length))
					{
						return this.skillPreferences[skillSlotIndex];
					}
					return 0U;
				}

				// Token: 0x0600173E RID: 5950 RVA: 0x00065295 File Offset: 0x00063495
				public bool SetSkillVariant(int skillSlotIndex, uint skillVariant)
				{
					if ((ulong)skillSlotIndex < (ulong)((long)this.skillPreferences.Length))
					{
						this.skillPreferences[skillSlotIndex] = HGMath.Clamp(skillVariant, 0U, (uint)this.LookUpMaxSkillVariants(skillSlotIndex));
						return true;
					}
					return false;
				}

				// Token: 0x0600173F RID: 5951 RVA: 0x000652C0 File Offset: 0x000634C0
				private bool IsSkillVariantValid(int skillSlotIndex)
				{
					SkillFamily skillFamily = this.GetSkillFamily(skillSlotIndex);
					return skillFamily && (ulong)this.GetSkillVariant(skillSlotIndex) < (ulong)((long)skillFamily.variants.Length);
				}

				// Token: 0x06001740 RID: 5952 RVA: 0x000652F8 File Offset: 0x000634F8
				private bool IsSkillVariantLocked(int skillSlotIndex, UserProfile userProfile)
				{
					SkillFamily skillFamily = this.GetSkillFamily(skillSlotIndex);
					if (!skillFamily)
					{
						return false;
					}
					uint skillVariant = this.GetSkillVariant(skillSlotIndex);
					string unlockableName = skillFamily.variants[(int)skillVariant].unlockableName;
					return !string.IsNullOrEmpty(unlockableName) && !userProfile.HasUnlockable(unlockableName);
				}

				// Token: 0x06001741 RID: 5953 RVA: 0x00065345 File Offset: 0x00063545
				private void ResetSkillVariant(int skillSlotIndex)
				{
					if ((ulong)skillSlotIndex < (ulong)((long)this.skillPreferences.Length))
					{
						uint[] array = this.skillPreferences;
						SkillFamily skillFamily = this.GetSkillFamily(skillSlotIndex);
						array[skillSlotIndex] = ((skillFamily != null) ? skillFamily.defaultVariantIndex : 0U);
					}
				}

				// Token: 0x06001742 RID: 5954 RVA: 0x00065370 File Offset: 0x00063570
				private bool IsSkinValid()
				{
					SkinDef[] bodySkins = BodyCatalog.GetBodySkins(this.bodyIndex);
					return (ulong)this.skinPreference < (ulong)((long)bodySkins.Length);
				}

				// Token: 0x06001743 RID: 5955 RVA: 0x00065398 File Offset: 0x00063598
				private bool IsSkinLocked(UserProfile userProfile)
				{
					SkinDef skinDef = BodyCatalog.GetBodySkins(this.bodyIndex)[(int)this.skinPreference];
					return !string.IsNullOrEmpty(skinDef.unlockableName) && !userProfile.HasUnlockable(skinDef.unlockableName);
				}

				// Token: 0x06001744 RID: 5956 RVA: 0x000653D6 File Offset: 0x000635D6
				private void ResetSkin()
				{
					this.skinPreference = 0U;
				}

				// Token: 0x06001745 RID: 5957 RVA: 0x000653E0 File Offset: 0x000635E0
				public void EnforceValidity()
				{
					for (int i = 0; i < this.skillPreferences.Length; i++)
					{
						if (!this.IsSkillVariantValid(i))
						{
							this.ResetSkillVariant(i);
						}
					}
					if (!this.IsSkinValid())
					{
						this.ResetSkin();
					}
				}

				// Token: 0x06001746 RID: 5958 RVA: 0x00065420 File Offset: 0x00063620
				public void EnforceUnlockables(UserProfile userProfile)
				{
					for (int i = 0; i < this.skillPreferences.Length; i++)
					{
						if (this.IsSkillVariantLocked(i, userProfile))
						{
							this.ResetSkillVariant(i);
						}
					}
					if (this.IsSkinLocked(userProfile))
					{
						this.ResetSkin();
					}
				}

				// Token: 0x06001747 RID: 5959 RVA: 0x00065460 File Offset: 0x00063660
				[CanBeNull]
				private SkillFamily GetSkillFamily(int skillSlotIndex)
				{
					return BodyCatalog.GetBodyPrefabSkillSlots(this.bodyIndex)[skillSlotIndex].skillFamily;
				}

				// Token: 0x06001748 RID: 5960 RVA: 0x00065474 File Offset: 0x00063674
				public int LookUpMaxSkillVariants(int skillSlotIndex)
				{
					if ((ulong)skillSlotIndex >= (ulong)((long)this.skillPreferences.Length))
					{
						return 0;
					}
					SkillFamily skillFamily = this.GetSkillFamily(skillSlotIndex);
					if (skillFamily == null)
					{
						return 0;
					}
					return skillFamily.variants.Length;
				}

				// Token: 0x06001749 RID: 5961 RVA: 0x0006549C File Offset: 0x0006369C
				public void Serialize(NetworkWriter writer)
				{
					writer.WriteBodyIndex(this.bodyIndex);
					writer.WritePackedUInt32(this.skinPreference);
					for (int i = 0; i < this.skillPreferences.Length; i++)
					{
						writer.WritePackedUInt32(this.skillPreferences[i]);
					}
				}

				// Token: 0x0600174A RID: 5962 RVA: 0x000654E4 File Offset: 0x000636E4
				public void Deserialize(NetworkReader reader)
				{
					this.bodyIndex = Mathf.Clamp(reader.ReadBodyIndex(), 0, BodyCatalog.bodyCount);
					this.skinPreference = reader.ReadPackedUInt32();
					Array.Resize<uint>(ref this.skillPreferences, Loadout.BodyLoadoutManager.GetSkillSlotCountForBody(this.bodyIndex));
					for (int i = 0; i < this.skillPreferences.Length; i++)
					{
						this.SetSkillVariant(i, reader.ReadPackedUInt32());
					}
				}

				// Token: 0x0600174B RID: 5963 RVA: 0x0006554C File Offset: 0x0006374C
				public XElement ToXml(string elementName)
				{
					XElement xelement = new XElement(elementName);
					xelement.SetAttributeValue("bodyName", BodyCatalog.GetBodyName(this.bodyIndex));
					xelement.Add(new XElement("Skin", this.skinPreference.ToString()));
					ref Loadout.BodyLoadoutManager.BodyInfo ptr = ref Loadout.BodyLoadoutManager.allBodyInfos[this.bodyIndex];
					for (int i = 0; i < ptr.prefabSkillSlots.Length; i++)
					{
						int skillFamilyIndex = ptr.skillFamilyIndices[i];
						SkillFamily skillFamily = SkillCatalog.GetSkillFamily(skillFamilyIndex);
						string skillFamilyName = SkillCatalog.GetSkillFamilyName(skillFamilyIndex);
						string variantName = skillFamily.GetVariantName((int)this.skillPreferences[i]);
						if (variantName != null)
						{
							XElement xelement2 = new XElement("SkillPreference", variantName);
							xelement2.SetAttributeValue("skillFamily", skillFamilyName);
							xelement.Add(xelement2);
						}
					}
					return xelement;
				}

				// Token: 0x0600174C RID: 5964 RVA: 0x00065620 File Offset: 0x00063820
				public bool FromXml(XElement element)
				{
					XElement xelement = element.Element("Skin");
					uint.TryParse(((xelement != null) ? xelement.Value : null) ?? string.Empty, out this.skinPreference);
					XAttribute xattribute = element.Attribute("bodyName");
					string text = (xattribute != null) ? xattribute.Value : null;
					if (text == null)
					{
						Debug.Log("bodyName=null");
						return false;
					}
					this.bodyIndex = BodyCatalog.FindBodyIndex(text);
					if (this.bodyIndex == -1)
					{
						Debug.LogFormat("Could not find body index for bodyName={0}", new object[]
						{
							text
						});
						return false;
					}
					ref Loadout.BodyLoadoutManager.BodyInfo ptr = ref Loadout.BodyLoadoutManager.allBodyInfos[this.bodyIndex];
					Loadout.BodyLoadoutManager.BodyLoadout.<>c__DisplayClass20_0 CS$<>8__locals1;
					CS$<>8__locals1.prefabSkillSlots = ptr.prefabSkillSlots;
					this.skillPreferences = new uint[CS$<>8__locals1.prefabSkillSlots.Length];
					foreach (XElement xelement2 in element.Elements("SkillPreference"))
					{
						XAttribute xattribute2 = xelement2.Attribute("skillFamily");
						string text2 = (xattribute2 != null) ? xattribute2.Value : null;
						string value = xelement2.Value;
						if (text2 != null)
						{
							int num = Loadout.BodyLoadoutManager.BodyLoadout.<FromXml>g__FindSkillSlotIndex|20_0(text2, ref CS$<>8__locals1);
							if (num != -1)
							{
								int variantIndex = CS$<>8__locals1.prefabSkillSlots[num].skillFamily.GetVariantIndex(value);
								if (variantIndex != -1)
								{
									this.skillPreferences[num] = (uint)variantIndex;
								}
								else
								{
									Debug.LogFormat("Could not find variant index for elementSkillFamilyName={0} elementSkillName={1}", new object[]
									{
										text2,
										value
									});
								}
							}
							else
							{
								Debug.LogFormat("Could not find skill slot index for elementSkillFamilyName={0} elementSkillName={1}", new object[]
								{
									text2,
									value
								});
							}
						}
					}
					return true;
				}

				// Token: 0x0600174E RID: 5966 RVA: 0x000657C8 File Offset: 0x000639C8
				[CompilerGenerated]
				internal static int <FromXml>g__FindSkillSlotIndex|20_0(string requestedSkillFamilyName, ref Loadout.BodyLoadoutManager.BodyLoadout.<>c__DisplayClass20_0 A_1)
				{
					for (int i = 0; i < A_1.prefabSkillSlots.Length; i++)
					{
						if (SkillCatalog.GetSkillFamilyName(A_1.prefabSkillSlots[i].skillFamily.catalogIndex).Equals(requestedSkillFamilyName, StringComparison.Ordinal))
						{
							return i;
						}
					}
					return -1;
				}

				// Token: 0x04001614 RID: 5652
				public int bodyIndex;

				// Token: 0x04001615 RID: 5653
				public uint skinPreference;

				// Token: 0x04001616 RID: 5654
				public uint[] skillPreferences;
			}

			// Token: 0x020003BF RID: 959
			private struct BodyInfo
			{
				// Token: 0x170002B2 RID: 690
				// (get) Token: 0x0600174F RID: 5967 RVA: 0x0006580B File Offset: 0x00063A0B
				public int skillSlotCount
				{
					get
					{
						return this.prefabSkillSlots.Length;
					}
				}

				// Token: 0x04001618 RID: 5656
				public int[] skillFamilyIndices;

				// Token: 0x04001619 RID: 5657
				public GenericSkill[] prefabSkillSlots;
			}
		}
	}
}
