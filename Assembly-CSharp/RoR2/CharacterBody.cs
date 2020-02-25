using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using EntityStates;
using RoR2.Audio;
using RoR2.CharacterAI;
using RoR2.Networking;
using RoR2.Orbs;
using RoR2.Projectile;
using RoR2.Skills;
using RoR2.UI;
using Unity;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.Serialization;

namespace RoR2
{
	// Token: 0x02000177 RID: 375
	[RequireComponent(typeof(TeamComponent))]
	[RequireComponent(typeof(SkillLocator))]
	[DisallowMultipleComponent]
	public class CharacterBody : NetworkBehaviour, ILifeBehavior, IDisplayNameProvider, IOnTakeDamageServerReceiver, IOnKilledOtherServerReceiver
	{
		// Token: 0x0600070E RID: 1806 RVA: 0x0001E221 File Offset: 0x0001C421
		[RuntimeInitializeOnLoadMethod]
		private static void LoadCommonAssets()
		{
			CharacterBody.CommonAssets.Load();
		}

		// Token: 0x0600070F RID: 1807 RVA: 0x0001E228 File Offset: 0x0001C428
		public string GetDisplayName()
		{
			return Language.GetString(this.baseNameToken);
		}

		// Token: 0x06000710 RID: 1808 RVA: 0x0001E235 File Offset: 0x0001C435
		public string GetSubtitle()
		{
			return Language.GetString(this.subtitleNameToken);
		}

		// Token: 0x06000711 RID: 1809 RVA: 0x0001E244 File Offset: 0x0001C444
		public string GetUserName()
		{
			string text = "";
			if (this.master)
			{
				PlayerCharacterMasterController component = this.master.GetComponent<PlayerCharacterMasterController>();
				if (component)
				{
					text = component.GetDisplayName();
				}
			}
			if (string.IsNullOrEmpty(text))
			{
				text = this.GetDisplayName();
			}
			return text;
		}

		// Token: 0x06000712 RID: 1810 RVA: 0x0001E290 File Offset: 0x0001C490
		public string GetColoredUserName()
		{
			Color32 userColor = new Color32(127, 127, 127, byte.MaxValue);
			string text = null;
			if (this.master)
			{
				PlayerCharacterMasterController component = this.master.GetComponent<PlayerCharacterMasterController>();
				if (component)
				{
					GameObject networkUserObject = component.networkUserObject;
					if (networkUserObject)
					{
						NetworkUser component2 = networkUserObject.GetComponent<NetworkUser>();
						if (component2)
						{
							userColor = component2.userColor;
							text = component2.userName;
						}
					}
				}
			}
			if (text == null)
			{
				text = this.GetDisplayName();
			}
			return Util.GenerateColoredString(text, userColor);
		}

		// Token: 0x06000713 RID: 1811 RVA: 0x0001E314 File Offset: 0x0001C514
		[Server]
		private void WriteBuffs(NetworkWriter writer)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.CharacterBody::WriteBuffs(UnityEngine.Networking.NetworkWriter)' called on client");
				return;
			}
			writer.Write((byte)this.activeBuffsListCount);
			for (int i = 0; i < this.activeBuffsListCount; i++)
			{
				BuffIndex buffIndex = this.activeBuffsList[i];
				BuffDef buffDef = BuffCatalog.GetBuffDef(buffIndex);
				writer.WriteBuffIndex(buffIndex);
				if (buffDef.canStack)
				{
					writer.WritePackedUInt32((uint)this.buffs[(int)buffIndex]);
				}
			}
		}

		// Token: 0x06000714 RID: 1812 RVA: 0x0001E380 File Offset: 0x0001C580
		[Client]
		private void ReadBuffs(NetworkReader reader)
		{
			if (!NetworkClient.active)
			{
				Debug.LogWarning("[Client] function 'System.Void RoR2.CharacterBody::ReadBuffs(UnityEngine.Networking.NetworkReader)' called on server");
				return;
			}
			CharacterBody.<>c__DisplayClass17_0 CS$<>8__locals1;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.activeBuffsIndexToCheck = 0;
			int num = (int)reader.ReadByte();
			BuffIndex buffIndex = BuffIndex.None;
			for (int i = 0; i < num; i++)
			{
				BuffIndex buffIndex2 = reader.ReadBuffIndex();
				BuffDef buffDef = BuffCatalog.GetBuffDef(buffIndex2);
				int num2 = 1;
				if (buffDef.canStack)
				{
					num2 = (int)reader.ReadPackedUInt32();
				}
				if (num2 > 0 && !NetworkServer.active)
				{
					this.<ReadBuffs>g__ZeroBuffIndexRange|17_0(buffIndex + 1, buffIndex2, ref CS$<>8__locals1);
					this.SetBuffCount(buffIndex2, num2);
				}
				buffIndex = buffIndex2;
			}
			if (!NetworkServer.active)
			{
				this.<ReadBuffs>g__ZeroBuffIndexRange|17_0(buffIndex + 1, (BuffIndex)BuffCatalog.buffCount, ref CS$<>8__locals1);
			}
		}

		// Token: 0x06000715 RID: 1813 RVA: 0x0001E425 File Offset: 0x0001C625
		[Server]
		public void AddBuff(BuffIndex buffType)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.CharacterBody::AddBuff(RoR2.BuffIndex)' called on client");
				return;
			}
			this.SetBuffCount(buffType, this.buffs[(int)buffType] + 1);
		}

		// Token: 0x06000716 RID: 1814 RVA: 0x0001E450 File Offset: 0x0001C650
		[Server]
		public void RemoveBuff(BuffIndex buffType)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.CharacterBody::RemoveBuff(RoR2.BuffIndex)' called on client");
				return;
			}
			this.SetBuffCount(buffType, this.buffs[(int)buffType] - 1);
			if (buffType != BuffIndex.MedkitHeal)
			{
				if (buffType != BuffIndex.TonicBuff)
				{
					return;
				}
				if (this.inventory && this.GetBuffCount(BuffIndex.TonicBuff) == 0 && this.pendingTonicAfflictionCount > 0)
				{
					this.inventory.GiveItem(ItemIndex.TonicAffliction, this.pendingTonicAfflictionCount);
					ReadOnlyCollection<NotificationQueue> readOnlyCollection = NotificationQueue.readOnlyInstancesList;
					for (int i = 0; i < readOnlyCollection.Count; i++)
					{
						readOnlyCollection[i].OnPickup(this.master, new PickupIndex(ItemIndex.TonicAffliction));
					}
					this.pendingTonicAfflictionCount = 0;
				}
			}
			else if (this.GetBuffCount(BuffIndex.MedkitHeal) == 0)
			{
				int itemCount = this.inventory.GetItemCount(ItemIndex.Medkit);
				this.healthComponent.Heal((float)itemCount * 10f, default(ProcChainMask), true);
				EffectData effectData = new EffectData
				{
					origin = this.transform.position
				};
				effectData.SetNetworkedObjectReference(base.gameObject);
				EffectManager.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/MedkitHealEffect"), effectData, true);
				return;
			}
		}

		// Token: 0x06000717 RID: 1815 RVA: 0x0001E56C File Offset: 0x0001C76C
		private void SetBuffCount(BuffIndex buffType, int newCount)
		{
			ref int ptr = ref this.buffs[(int)buffType];
			if (newCount == ptr)
			{
				return;
			}
			int num = ptr;
			ptr = newCount;
			BuffDef buffDef = BuffCatalog.GetBuffDef(buffType);
			bool flag = true;
			if (!buffDef.canStack)
			{
				flag = (num == 0 != (newCount == 0));
			}
			if (flag)
			{
				if (newCount == 0)
				{
					HGArrayUtilities.ArrayRemoveAt<BuffIndex>(ref this.activeBuffsList, ref this.activeBuffsListCount, Array.IndexOf<BuffIndex>(this.activeBuffsList, buffType), 1);
					this.OnBuffFinalStackLost(buffDef);
				}
				else if (num == 0)
				{
					int num2 = 0;
					while (num2 < this.activeBuffsListCount && buffType >= this.activeBuffsList[num2])
					{
						num2++;
					}
					HGArrayUtilities.ArrayInsert<BuffIndex>(ref this.activeBuffsList, ref this.activeBuffsListCount, num2, ref buffType);
					this.OnBuffFirstStackGained(buffDef);
				}
				if (NetworkServer.active)
				{
					base.SetDirtyBit(2U);
				}
			}
			this.statsDirty = true;
			if (NetworkClient.active)
			{
				this.OnClientBuffsChanged();
			}
		}

		// Token: 0x06000718 RID: 1816 RVA: 0x0001E640 File Offset: 0x0001C840
		private void OnBuffFirstStackGained(BuffDef buffDef)
		{
			if (buffDef.isElite)
			{
				this.eliteBuffCount++;
			}
			BuffIndex buffIndex = buffDef.buffIndex;
			if (buffIndex == BuffIndex.Intangible)
			{
				this.UpdateHurtBoxesEnabled();
			}
		}

		// Token: 0x06000719 RID: 1817 RVA: 0x0001E678 File Offset: 0x0001C878
		private void OnBuffFinalStackLost(BuffDef buffDef)
		{
			if (buffDef.isElite)
			{
				this.eliteBuffCount--;
			}
			BuffIndex buffIndex = buffDef.buffIndex;
			if (buffIndex == BuffIndex.Intangible)
			{
				this.UpdateHurtBoxesEnabled();
			}
		}

		// Token: 0x0600071A RID: 1818 RVA: 0x0001E6AD File Offset: 0x0001C8AD
		public int GetBuffCount(BuffIndex buffType)
		{
			return this.buffs[(int)buffType];
		}

		// Token: 0x0600071B RID: 1819 RVA: 0x0001E6B7 File Offset: 0x0001C8B7
		public bool HasBuff(BuffIndex buffType)
		{
			return this.GetBuffCount(buffType) > 0;
		}

		// Token: 0x0600071C RID: 1820 RVA: 0x0001E6C4 File Offset: 0x0001C8C4
		[Server]
		public void AddTimedBuff(BuffIndex buffType, float duration)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.CharacterBody::AddTimedBuff(RoR2.BuffIndex,System.Single)' called on client");
				return;
			}
			CharacterBody.<>c__DisplayClass25_0 CS$<>8__locals1;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.buffType = buffType;
			CS$<>8__locals1.duration = duration;
			CS$<>8__locals1.buffDef = BuffCatalog.GetBuffDef(CS$<>8__locals1.buffType);
			BuffIndex buffType2 = CS$<>8__locals1.buffType;
			if (buffType2 != BuffIndex.AttackSpeedOnCrit)
			{
				if (buffType2 != BuffIndex.BeetleJuice)
				{
					switch (buffType2)
					{
					case BuffIndex.AffixHauntedRecipient:
						if (!this.HasBuff(BuffIndex.AffixHaunted))
						{
							this.<AddTimedBuff>g__DefaultBehavior|25_0(ref CS$<>8__locals1);
							return;
						}
						return;
					case BuffIndex.NullifyStack:
					{
						if (this.HasBuff(BuffIndex.Nullified))
						{
							return;
						}
						int num = 0;
						for (int i = 0; i < this.timedBuffs.Count; i++)
						{
							if (this.timedBuffs[i].buffIndex == CS$<>8__locals1.buffType)
							{
								num++;
								if (this.timedBuffs[i].timer < CS$<>8__locals1.duration)
								{
									this.timedBuffs[i].timer = CS$<>8__locals1.duration;
								}
							}
						}
						if (num < 2)
						{
							this.timedBuffs.Add(new CharacterBody.TimedBuff
							{
								buffIndex = CS$<>8__locals1.buffType,
								timer = CS$<>8__locals1.duration
							});
							this.AddBuff(CS$<>8__locals1.buffType);
							return;
						}
						this.ClearTimedBuffs(BuffIndex.NullifyStack);
						this.AddTimedBuff(BuffIndex.Nullified, 3f);
						return;
					}
					case BuffIndex.Nullified:
						EntitySoundManager.EmitSoundServer(CharacterBody.CommonAssets.nullifiedBuffAppliedSound.index, this.networkIdentity);
						this.<AddTimedBuff>g__DefaultBehavior|25_0(ref CS$<>8__locals1);
						return;
					case BuffIndex.MeatRegenBoost:
						EntitySoundManager.EmitSoundServer((AkEventIdArg)"Play_item_proc_regenOnKill", base.gameObject);
						this.<AddTimedBuff>g__DefaultBehavior|25_0(ref CS$<>8__locals1);
						return;
					}
					this.<AddTimedBuff>g__DefaultBehavior|25_0(ref CS$<>8__locals1);
				}
				else
				{
					int num2 = 0;
					for (int j = 0; j < this.timedBuffs.Count; j++)
					{
						if (this.timedBuffs[j].buffIndex == CS$<>8__locals1.buffType)
						{
							num2++;
							if (this.timedBuffs[j].timer < CS$<>8__locals1.duration)
							{
								this.timedBuffs[j].timer = CS$<>8__locals1.duration;
							}
						}
					}
					if (num2 < 10)
					{
						this.timedBuffs.Add(new CharacterBody.TimedBuff
						{
							buffIndex = CS$<>8__locals1.buffType,
							timer = CS$<>8__locals1.duration
						});
						this.AddBuff(CS$<>8__locals1.buffType);
						return;
					}
				}
				return;
			}
			int num3 = this.inventory ? this.inventory.GetItemCount(ItemIndex.AttackSpeedOnCrit) : 0;
			int num4 = 0;
			int num5 = -1;
			float num6 = 999f;
			for (int k = 0; k < this.timedBuffs.Count; k++)
			{
				if (this.timedBuffs[k].buffIndex == CS$<>8__locals1.buffType)
				{
					num4++;
					if (this.timedBuffs[k].timer < num6)
					{
						num5 = k;
						num6 = this.timedBuffs[k].timer;
					}
				}
			}
			if (num4 < 1 + num3 * 2)
			{
				this.timedBuffs.Add(new CharacterBody.TimedBuff
				{
					buffIndex = CS$<>8__locals1.buffType,
					timer = CS$<>8__locals1.duration
				});
				this.AddBuff(CS$<>8__locals1.buffType);
				ChildLocator component = this.modelLocator.modelTransform.GetComponent<ChildLocator>();
				if (component)
				{
					UnityEngine.Object exists = component.FindChild("HandL");
					Transform exists2 = component.FindChild("HandR");
					GameObject effectPrefab = Resources.Load<GameObject>("Prefabs/Effects/WolfProcEffect");
					if (exists)
					{
						EffectManager.SimpleMuzzleFlash(effectPrefab, base.gameObject, "HandL", true);
					}
					if (exists2)
					{
						EffectManager.SimpleMuzzleFlash(effectPrefab, base.gameObject, "HandR", true);
					}
				}
			}
			else if (num5 > -1)
			{
				this.timedBuffs[num5].timer = CS$<>8__locals1.duration;
			}
			EntitySoundManager.EmitSoundServer(CharacterBody.CommonAssets.procCritAttackSpeedSounds[Mathf.Min(CharacterBody.CommonAssets.procCritAttackSpeedSounds.Length - 1, num4)].index, this.networkIdentity);
		}

		// Token: 0x0600071D RID: 1821 RVA: 0x0001EAA8 File Offset: 0x0001CCA8
		[Server]
		public void ClearTimedBuffs(BuffIndex buffType)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.CharacterBody::ClearTimedBuffs(RoR2.BuffIndex)' called on client");
				return;
			}
			for (int i = this.timedBuffs.Count - 1; i >= 0; i--)
			{
				if (this.timedBuffs[i].buffIndex == buffType)
				{
					this.RemoveBuff(this.timedBuffs[i].buffIndex);
					this.timedBuffs.RemoveAt(i);
				}
			}
		}

		// Token: 0x0600071E RID: 1822 RVA: 0x0001EB1C File Offset: 0x0001CD1C
		[Server]
		private void UpdateBuffs(float deltaTime)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.CharacterBody::UpdateBuffs(System.Single)' called on client");
				return;
			}
			for (int i = this.timedBuffs.Count - 1; i >= 0; i--)
			{
				this.timedBuffs[i].timer -= deltaTime;
				if (this.timedBuffs[i].timer <= 0f)
				{
					this.RemoveBuff(this.timedBuffs[i].buffIndex);
					this.timedBuffs.RemoveAt(i);
				}
			}
		}

		// Token: 0x0600071F RID: 1823 RVA: 0x0001EBAC File Offset: 0x0001CDAC
		[Client]
		private void OnClientBuffsChanged()
		{
			if (!NetworkClient.active)
			{
				Debug.LogWarning("[Client] function 'System.Void RoR2.CharacterBody::OnClientBuffsChanged()' called on server");
				return;
			}
			bool flag = this.HasBuff(BuffIndex.WarCryBuff);
			if (!flag && this.warCryEffectInstance)
			{
				UnityEngine.Object.Destroy(this.warCryEffectInstance);
			}
			if (flag && !this.warCryEffectInstance)
			{
				Transform transform = this.mainHurtBox ? this.mainHurtBox.transform : this.transform;
				if (transform)
				{
					this.warCryEffectInstance = UnityEngine.Object.Instantiate<GameObject>((GameObject)Resources.Load("Prefabs/Effects/WarCryEffect"), transform.position, Quaternion.identity, transform);
				}
			}
		}

		// Token: 0x170000D2 RID: 210
		// (get) Token: 0x06000720 RID: 1824 RVA: 0x0001EC4F File Offset: 0x0001CE4F
		public CharacterMaster master
		{
			get
			{
				if (!this.masterObject)
				{
					return null;
				}
				return this._master;
			}
		}

		// Token: 0x170000D3 RID: 211
		// (get) Token: 0x06000721 RID: 1825 RVA: 0x0001EC66 File Offset: 0x0001CE66
		// (set) Token: 0x06000722 RID: 1826 RVA: 0x0001EC6E File Offset: 0x0001CE6E
		public Inventory inventory { get; private set; }

		// Token: 0x170000D4 RID: 212
		// (get) Token: 0x06000723 RID: 1827 RVA: 0x0001EC77 File Offset: 0x0001CE77
		// (set) Token: 0x06000724 RID: 1828 RVA: 0x0001EC7F File Offset: 0x0001CE7F
		public bool isPlayerControlled { get; private set; }

		// Token: 0x170000D5 RID: 213
		// (get) Token: 0x06000725 RID: 1829 RVA: 0x0001EC88 File Offset: 0x0001CE88
		// (set) Token: 0x06000726 RID: 1830 RVA: 0x0001EC90 File Offset: 0x0001CE90
		public float executeEliteHealthFraction { get; private set; }

		// Token: 0x06000727 RID: 1831 RVA: 0x0001EC9C File Offset: 0x0001CE9C
		private void UpdateHurtBoxesEnabled()
		{
			bool flag = this.inventory.GetItemCount(ItemIndex.Ghost) > 0 || this.HasBuff(BuffIndex.Intangible);
			if (flag == this.disablingHurtBoxes)
			{
				return;
			}
			if (this.hurtBoxGroup)
			{
				if (flag)
				{
					HurtBoxGroup hurtBoxGroup = this.hurtBoxGroup;
					int hurtBoxesDeactivatorCounter = hurtBoxGroup.hurtBoxesDeactivatorCounter + 1;
					hurtBoxGroup.hurtBoxesDeactivatorCounter = hurtBoxesDeactivatorCounter;
				}
				else
				{
					HurtBoxGroup hurtBoxGroup2 = this.hurtBoxGroup;
					int hurtBoxesDeactivatorCounter = hurtBoxGroup2.hurtBoxesDeactivatorCounter - 1;
					hurtBoxGroup2.hurtBoxesDeactivatorCounter = hurtBoxesDeactivatorCounter;
				}
			}
			this.disablingHurtBoxes = flag;
		}

		// Token: 0x06000728 RID: 1832 RVA: 0x0001ED14 File Offset: 0x0001CF14
		private void OnInventoryChanged()
		{
			EquipmentIndex currentEquipmentIndex = this.inventory.currentEquipmentIndex;
			if (currentEquipmentIndex != this.previousEquipmentIndex)
			{
				EquipmentDef equipmentDef = EquipmentCatalog.GetEquipmentDef(this.previousEquipmentIndex);
				EquipmentDef equipmentDef2 = EquipmentCatalog.GetEquipmentDef(currentEquipmentIndex);
				if (equipmentDef != null)
				{
					this.OnEquipmentLost(equipmentDef);
				}
				if (equipmentDef2 != null)
				{
					this.OnEquipmentGained(equipmentDef2);
				}
				this.previousEquipmentIndex = currentEquipmentIndex;
			}
			this.statsDirty = true;
			this.UpdateHurtBoxesEnabled();
			this.AddItemBehavior<CharacterBody.MushroomItemBehavior>(this.inventory.GetItemCount(ItemIndex.Mushroom));
			this.AddItemBehavior<CharacterBody.IcicleItemBehavior>(this.inventory.GetItemCount(ItemIndex.Icicle));
			this.AddItemBehavior<CharacterBody.HeadstomperItemBehavior>(this.inventory.GetItemCount(ItemIndex.FallBoots));
			this.AddItemBehavior<CharacterBody.AffixHauntedBehavior>(this.HasBuff(BuffIndex.AffixHaunted) ? 1 : 0);
			this.AddItemBehavior<CharacterBody.LaserTurbineBehavior>(this.inventory.GetItemCount(ItemIndex.LaserTurbine));
			if (NetworkServer.active)
			{
				this.AddItemBehavior<CharacterBody.RedWhipBehavior>(this.inventory.GetItemCount(ItemIndex.SprintOutOfCombat));
				this.AddItemBehavior<CharacterBody.SprintWispBehavior>(this.inventory.GetItemCount(ItemIndex.SprintWisp));
				this.AddItemBehavior<CharacterBody.NovaOnLowHealthItemBehaviorServer>(this.inventory.GetItemCount(ItemIndex.NovaOnLowHealth));
				this.AddItemBehavior<CharacterBody.QuestVolatileBatteryBehaviorServer>((this.inventory.GetEquipment((uint)this.inventory.activeEquipmentSlot).equipmentIndex == EquipmentIndex.QuestVolatileBattery) ? 1 : 0);
			}
			this.executeEliteHealthFraction = Util.ConvertAmplificationPercentageIntoReductionPercentage(20f * (float)this.inventory.GetItemCount(ItemIndex.ExecuteLowHealthElite)) / 100f;
			if (this.skillLocator)
			{
				if (this.skillLocator.primary)
				{
					if (this.inventory.GetItemCount(ItemIndex.LunarPrimaryReplacement) > 0)
					{
						this.skillLocator.primary.SetSkillOverride(this, CharacterBody.CommonAssets.lunarPrimaryReplacementSkillDef, GenericSkill.SkillOverridePriority.Replacement);
					}
					else
					{
						this.skillLocator.primary.UnsetSkillOverride(this, CharacterBody.CommonAssets.lunarPrimaryReplacementSkillDef, GenericSkill.SkillOverridePriority.Replacement);
					}
				}
				if (this.skillLocator.utility)
				{
					if (this.inventory.GetItemCount(ItemIndex.LunarUtilityReplacement) > 0)
					{
						this.skillLocator.utility.SetSkillOverride(this, CharacterBody.CommonAssets.lunarUtilityReplacementSkillDef, GenericSkill.SkillOverridePriority.Replacement);
					}
					else
					{
						this.skillLocator.utility.UnsetSkillOverride(this, CharacterBody.CommonAssets.lunarUtilityReplacementSkillDef, GenericSkill.SkillOverridePriority.Replacement);
					}
				}
			}
			Action action = this.onInventoryChanged;
			if (action == null)
			{
				return;
			}
			action();
		}

		// Token: 0x06000729 RID: 1833 RVA: 0x0001EF2C File Offset: 0x0001D12C
		private void OnEquipmentLost(EquipmentDef equipmentDef)
		{
			if (NetworkServer.active && equipmentDef.passiveBuff != BuffIndex.None)
			{
				this.RemoveBuff(equipmentDef.passiveBuff);
			}
		}

		// Token: 0x0600072A RID: 1834 RVA: 0x0001EF4A File Offset: 0x0001D14A
		private void OnEquipmentGained(EquipmentDef equipmentDef)
		{
			if (NetworkServer.active && equipmentDef.passiveBuff != BuffIndex.None)
			{
				this.AddBuff(equipmentDef.passiveBuff);
			}
		}

		// Token: 0x1400000B RID: 11
		// (add) Token: 0x0600072B RID: 1835 RVA: 0x0001EF68 File Offset: 0x0001D168
		// (remove) Token: 0x0600072C RID: 1836 RVA: 0x0001EFA0 File Offset: 0x0001D1A0
		public event Action onInventoryChanged;

		// Token: 0x170000D6 RID: 214
		// (get) Token: 0x0600072D RID: 1837 RVA: 0x0001EFD8 File Offset: 0x0001D1D8
		// (set) Token: 0x0600072E RID: 1838 RVA: 0x0001F0D7 File Offset: 0x0001D2D7
		public GameObject masterObject
		{
			get
			{
				if (!this._masterObject)
				{
					if (NetworkServer.active)
					{
						this._masterObject = NetworkServer.FindLocalObject(this.masterObjectId);
					}
					else if (NetworkClient.active)
					{
						this._masterObject = ClientScene.FindLocalObject(this.masterObjectId);
					}
					this._master = (this._masterObject ? this._masterObject.GetComponent<CharacterMaster>() : null);
					if (this._master)
					{
						this.isPlayerControlled = this._masterObject.GetComponent<PlayerCharacterMasterController>();
						if (this.inventory)
						{
							this.inventory.onInventoryChanged -= this.OnInventoryChanged;
						}
						this.inventory = this._master.inventory;
						if (this.inventory)
						{
							this.inventory.onInventoryChanged += this.OnInventoryChanged;
							this.OnInventoryChanged();
						}
						this.statsDirty = true;
					}
				}
				return this._masterObject;
			}
			set
			{
				this.masterObjectId = value.GetComponent<NetworkIdentity>().netId;
				this.statsDirty = true;
			}
		}

		// Token: 0x0600072F RID: 1839 RVA: 0x0001F0F4 File Offset: 0x0001D2F4
		private void UpdateMasterLink()
		{
			if (!this.linkedToMaster && this.master && this.master)
			{
				this.master.OnBodyStart(this);
				this.linkedToMaster = true;
				this.skinIndex = this.master.loadout.bodyLoadoutManager.GetSkinIndex(this.bodyIndex);
			}
		}

		// Token: 0x170000D7 RID: 215
		// (get) Token: 0x06000730 RID: 1840 RVA: 0x0001F157 File Offset: 0x0001D357
		// (set) Token: 0x06000731 RID: 1841 RVA: 0x0001F15F File Offset: 0x0001D35F
		public NetworkIdentity networkIdentity { get; private set; }

		// Token: 0x170000D8 RID: 216
		// (get) Token: 0x06000732 RID: 1842 RVA: 0x0001F168 File Offset: 0x0001D368
		// (set) Token: 0x06000733 RID: 1843 RVA: 0x0001F170 File Offset: 0x0001D370
		public CharacterMotor characterMotor { get; private set; }

		// Token: 0x170000D9 RID: 217
		// (get) Token: 0x06000734 RID: 1844 RVA: 0x0001F179 File Offset: 0x0001D379
		// (set) Token: 0x06000735 RID: 1845 RVA: 0x0001F181 File Offset: 0x0001D381
		public CharacterDirection characterDirection { get; private set; }

		// Token: 0x170000DA RID: 218
		// (get) Token: 0x06000736 RID: 1846 RVA: 0x0001F18A File Offset: 0x0001D38A
		// (set) Token: 0x06000737 RID: 1847 RVA: 0x0001F192 File Offset: 0x0001D392
		public TeamComponent teamComponent { get; private set; }

		// Token: 0x170000DB RID: 219
		// (get) Token: 0x06000738 RID: 1848 RVA: 0x0001F19B File Offset: 0x0001D39B
		// (set) Token: 0x06000739 RID: 1849 RVA: 0x0001F1A3 File Offset: 0x0001D3A3
		public HealthComponent healthComponent { get; private set; }

		// Token: 0x170000DC RID: 220
		// (get) Token: 0x0600073A RID: 1850 RVA: 0x0001F1AC File Offset: 0x0001D3AC
		// (set) Token: 0x0600073B RID: 1851 RVA: 0x0001F1B4 File Offset: 0x0001D3B4
		public EquipmentSlot equipmentSlot { get; private set; }

		// Token: 0x170000DD RID: 221
		// (get) Token: 0x0600073C RID: 1852 RVA: 0x0001F1BD File Offset: 0x0001D3BD
		// (set) Token: 0x0600073D RID: 1853 RVA: 0x0001F1C5 File Offset: 0x0001D3C5
		public InputBankTest inputBank { get; private set; }

		// Token: 0x170000DE RID: 222
		// (get) Token: 0x0600073E RID: 1854 RVA: 0x0001F1CE File Offset: 0x0001D3CE
		// (set) Token: 0x0600073F RID: 1855 RVA: 0x0001F1D6 File Offset: 0x0001D3D6
		public SkillLocator skillLocator { get; private set; }

		// Token: 0x170000DF RID: 223
		// (get) Token: 0x06000740 RID: 1856 RVA: 0x0001F1DF File Offset: 0x0001D3DF
		// (set) Token: 0x06000741 RID: 1857 RVA: 0x0001F1E7 File Offset: 0x0001D3E7
		public ModelLocator modelLocator { get; private set; }

		// Token: 0x170000E0 RID: 224
		// (get) Token: 0x06000742 RID: 1858 RVA: 0x0001F1F0 File Offset: 0x0001D3F0
		// (set) Token: 0x06000743 RID: 1859 RVA: 0x0001F1F8 File Offset: 0x0001D3F8
		public HurtBoxGroup hurtBoxGroup { get; private set; }

		// Token: 0x170000E1 RID: 225
		// (get) Token: 0x06000744 RID: 1860 RVA: 0x0001F201 File Offset: 0x0001D401
		// (set) Token: 0x06000745 RID: 1861 RVA: 0x0001F209 File Offset: 0x0001D409
		public HurtBox mainHurtBox { get; private set; }

		// Token: 0x170000E2 RID: 226
		// (get) Token: 0x06000746 RID: 1862 RVA: 0x0001F212 File Offset: 0x0001D412
		// (set) Token: 0x06000747 RID: 1863 RVA: 0x0001F21A File Offset: 0x0001D41A
		public Transform coreTransform { get; private set; }

		// Token: 0x06000748 RID: 1864 RVA: 0x0001F224 File Offset: 0x0001D424
		private void Awake()
		{
			this.transform = base.transform;
			this.networkIdentity = base.GetComponent<NetworkIdentity>();
			this.teamComponent = base.GetComponent<TeamComponent>();
			this.healthComponent = base.GetComponent<HealthComponent>();
			this.equipmentSlot = base.GetComponent<EquipmentSlot>();
			this.skillLocator = base.GetComponent<SkillLocator>();
			this.modelLocator = base.GetComponent<ModelLocator>();
			this.characterMotor = base.GetComponent<CharacterMotor>();
			this.characterDirection = base.GetComponent<CharacterDirection>();
			this.inputBank = base.GetComponent<InputBankTest>();
			this.sfxLocator = base.GetComponent<SfxLocator>();
			this.activeBuffsList = BuffCatalog.GetPerBuffBuffer<BuffIndex>();
			this.buffs = BuffCatalog.GetPerBuffBuffer<int>();
			if (this.modelLocator)
			{
				this.modelLocator.onModelChanged += this.OnModelChanged;
				this.OnModelChanged(this.modelLocator.modelTransform);
			}
			this.radius = 1f;
			CapsuleCollider component = base.GetComponent<CapsuleCollider>();
			if (component)
			{
				this.radius = component.radius;
				return;
			}
			SphereCollider component2 = base.GetComponent<SphereCollider>();
			if (component2)
			{
				this.radius = component2.radius;
			}
		}

		// Token: 0x06000749 RID: 1865 RVA: 0x0001F344 File Offset: 0x0001D544
		private void OnModelChanged(Transform modelTransform)
		{
			this.hurtBoxGroup = null;
			this.mainHurtBox = null;
			this.coreTransform = this.transform;
			if (modelTransform)
			{
				this.hurtBoxGroup = modelTransform.GetComponent<HurtBoxGroup>();
				if (this.hurtBoxGroup)
				{
					this.mainHurtBox = this.hurtBoxGroup.mainHurtBox;
					if (this.mainHurtBox)
					{
						this.coreTransform = this.mainHurtBox.transform;
					}
				}
			}
		}

		// Token: 0x0600074A RID: 1866 RVA: 0x0001F3BC File Offset: 0x0001D5BC
		private void Start()
		{
			this.localStartTime = Run.FixedTimeStamp.now;
			bool flag = (this.bodyFlags & CharacterBody.BodyFlags.Masterless) > CharacterBody.BodyFlags.None;
			this.outOfCombatStopwatch = float.PositiveInfinity;
			this.outOfDangerStopwatch = float.PositiveInfinity;
			this.notMovingStopwatch = 0f;
			this.warCryTimer = 30f;
			if (NetworkServer.active)
			{
				this.outOfCombat = true;
				this.outOfDanger = true;
			}
			GlobalEventManager.instance.OnCharacterBodyStart(this);
			this.RecalculateStats();
			this.UpdateMasterLink();
			if (flag)
			{
				this.healthComponent.Networkhealth = this.maxHealth;
			}
		}

		// Token: 0x0600074B RID: 1867 RVA: 0x0001F44A File Offset: 0x0001D64A
		public void Update()
		{
			this.UpdateSpreadBloom(Time.deltaTime);
			this.UpdateAllTemporaryVisualEffects();
		}

		// Token: 0x0600074C RID: 1868 RVA: 0x0001F460 File Offset: 0x0001D660
		public void FixedUpdate()
		{
			this.outOfCombatStopwatch += Time.fixedDeltaTime;
			this.outOfDangerStopwatch += Time.fixedDeltaTime;
			this.aimTimer = Mathf.Max(this.aimTimer - Time.fixedDeltaTime, 0f);
			if (NetworkServer.active)
			{
				this.UpdateMultiKill(Time.fixedDeltaTime);
			}
			this.UpdateMasterLink();
			bool outOfCombat = this.outOfCombat;
			bool flag = outOfCombat;
			if (NetworkServer.active || base.hasAuthority)
			{
				flag = (this.outOfCombatStopwatch >= 5f);
				if (this.outOfCombat != flag)
				{
					if (NetworkServer.active)
					{
						base.SetDirtyBit(4U);
					}
					this.outOfCombat = flag;
					this.statsDirty = true;
				}
			}
			if (NetworkServer.active)
			{
				this.UpdateBuffs(Time.fixedDeltaTime);
				bool flag2 = this.outOfDangerStopwatch >= 7f;
				bool outOfDanger = this.outOfDanger;
				bool flag3 = outOfCombat && outOfDanger;
				bool flag4 = flag && flag2;
				if (this.outOfDanger != flag2)
				{
					base.SetDirtyBit(8U);
					this.outOfDanger = flag2;
					this.statsDirty = true;
				}
				if (flag4 && !flag3)
				{
					this.OnOutOfCombatAndDangerServer();
				}
				Vector3 position = this.transform.position;
				float num = 0.1f * Time.fixedDeltaTime;
				if ((position - this.previousPosition).sqrMagnitude <= num * num)
				{
					this.notMovingStopwatch += Time.fixedDeltaTime;
				}
				else
				{
					this.notMovingStopwatch = 0f;
				}
				this.previousPosition = position;
				this.UpdateTeslaCoil();
				this.UpdateBeetleGuardAllies();
				this.UpdateHelfire();
				this.UpdateAffixPoison(Time.fixedDeltaTime);
				int num2 = 0;
				if (this.inventory)
				{
					num2 = this.inventory.GetItemCount(ItemIndex.WarCryOnCombat);
				}
				if (num2 > 0)
				{
					this.warCryTimer -= Time.fixedDeltaTime;
					this.warCryReady = (this.warCryTimer <= 0f);
					if (this.warCryReady && (!this.outOfCombat && outOfCombat))
					{
						this.warCryTimer = 30f;
						this.ActivateWarCryAura(num2);
					}
				}
			}
			if (this.statsDirty)
			{
				this.RecalculateStats();
			}
			this.UpdateFireTrail();
		}

		// Token: 0x0600074D RID: 1869 RVA: 0x0001F678 File Offset: 0x0001D878
		public void OnDeathStart()
		{
			base.enabled = false;
			if (NetworkServer.active && this.currentVehicle)
			{
				this.currentVehicle.EjectPassenger(base.gameObject);
			}
			if (this.master)
			{
				this.master.OnBodyDeath(this);
			}
		}

		// Token: 0x0600074E RID: 1870 RVA: 0x0001F6CA File Offset: 0x0001D8CA
		public void OnTakeDamageServer(DamageReport damageReport)
		{
			this.outOfDangerStopwatch = 0f;
			if (this.master)
			{
				this.master.OnBodyDamaged(damageReport);
			}
		}

		// Token: 0x0600074F RID: 1871 RVA: 0x0001F6F0 File Offset: 0x0001D8F0
		public void OnSkillActivated(GenericSkill skill)
		{
			if (skill.isCombatSkill)
			{
				this.outOfCombatStopwatch = 0f;
			}
			if (!NetworkServer.active)
			{
				this.CallCmdOnSkillActivated((sbyte)this.skillLocator.FindSkillSlot(skill));
				return;
			}
		}

		// Token: 0x06000750 RID: 1872 RVA: 0x0001F720 File Offset: 0x0001D920
		public void OnDestroy()
		{
			if (this.modelLocator != null)
			{
				this.modelLocator.onModelChanged -= this.OnModelChanged;
			}
			if (this.inventory)
			{
				this.inventory.onInventoryChanged -= this.OnInventoryChanged;
			}
			if (this.master)
			{
				this.master.OnBodyDestroyed(this);
			}
		}

		// Token: 0x06000751 RID: 1873 RVA: 0x0001F78C File Offset: 0x0001D98C
		public float GetNormalizedThreatValue()
		{
			if (Run.instance)
			{
				return (this.master ? this.master.money : 0f) / Mathf.Pow(Run.instance.compensatedDifficultyCoefficient, 2f);
			}
			return 0f;
		}

		// Token: 0x06000752 RID: 1874 RVA: 0x0001F7E2 File Offset: 0x0001D9E2
		private void OnEnable()
		{
			CharacterBody.instancesList.Add(this);
		}

		// Token: 0x06000753 RID: 1875 RVA: 0x0001F7EF File Offset: 0x0001D9EF
		private void OnDisable()
		{
			CharacterBody.instancesList.Remove(this);
		}

		// Token: 0x06000754 RID: 1876 RVA: 0x0001F7FD File Offset: 0x0001D9FD
		private void OnValidate()
		{
			if (this.autoCalculateLevelStats)
			{
				this.PerformAutoCalculateLevelStats();
			}
			if (!Application.isPlaying && this.bodyIndex != -1)
			{
				this.bodyIndex = -1;
			}
		}

		// Token: 0x170000E3 RID: 227
		// (get) Token: 0x06000755 RID: 1877 RVA: 0x0001F824 File Offset: 0x0001DA24
		// (set) Token: 0x06000756 RID: 1878 RVA: 0x0001F82C File Offset: 0x0001DA2C
		public bool hasEffectiveAuthority { get; private set; }

		// Token: 0x06000757 RID: 1879 RVA: 0x0001F835 File Offset: 0x0001DA35
		private void UpdateAuthority()
		{
			this.hasEffectiveAuthority = Util.HasEffectiveAuthority(base.gameObject);
		}

		// Token: 0x06000758 RID: 1880 RVA: 0x0001F848 File Offset: 0x0001DA48
		public override void OnStartAuthority()
		{
			this.UpdateAuthority();
		}

		// Token: 0x06000759 RID: 1881 RVA: 0x0001F848 File Offset: 0x0001DA48
		public override void OnStopAuthority()
		{
			this.UpdateAuthority();
		}

		// Token: 0x170000E4 RID: 228
		// (get) Token: 0x0600075A RID: 1882 RVA: 0x0001F850 File Offset: 0x0001DA50
		// (set) Token: 0x0600075B RID: 1883 RVA: 0x0001F858 File Offset: 0x0001DA58
		public bool isSprinting
		{
			get
			{
				return this._isSprinting;
			}
			set
			{
				if (this._isSprinting != value)
				{
					this._isSprinting = value;
					this.RecalculateStats();
					if (value)
					{
						this.OnSprintStart();
					}
					else
					{
						this.OnSprintStop();
					}
					if (NetworkServer.active)
					{
						base.SetDirtyBit(16U);
						return;
					}
					if (base.hasAuthority)
					{
						this.CallCmdUpdateSprint(value);
					}
				}
			}
		}

		// Token: 0x0600075C RID: 1884 RVA: 0x0000409B File Offset: 0x0000229B
		private void OnSprintStart()
		{
		}

		// Token: 0x0600075D RID: 1885 RVA: 0x0000409B File Offset: 0x0000229B
		private void OnSprintStop()
		{
		}

		// Token: 0x0600075E RID: 1886 RVA: 0x0001F8AB File Offset: 0x0001DAAB
		[Command]
		private void CmdUpdateSprint(bool newIsSprinting)
		{
			this.isSprinting = newIsSprinting;
		}

		// Token: 0x0600075F RID: 1887 RVA: 0x0001F8B4 File Offset: 0x0001DAB4
		[Command]
		private void CmdOnSkillActivated(sbyte skillIndex)
		{
			this.OnSkillActivated(this.skillLocator.GetSkill((SkillSlot)skillIndex));
		}

		// Token: 0x170000E5 RID: 229
		// (get) Token: 0x06000760 RID: 1888 RVA: 0x0001F8C8 File Offset: 0x0001DAC8
		// (set) Token: 0x06000761 RID: 1889 RVA: 0x0001F8D0 File Offset: 0x0001DAD0
		public bool outOfCombat { get; private set; } = true;

		// Token: 0x170000E6 RID: 230
		// (get) Token: 0x06000762 RID: 1890 RVA: 0x0001F8D9 File Offset: 0x0001DAD9
		// (set) Token: 0x06000763 RID: 1891 RVA: 0x0001F8E1 File Offset: 0x0001DAE1
		public bool outOfDanger
		{
			get
			{
				return this._outOfDanger;
			}
			private set
			{
				if (this._outOfDanger == value)
				{
					return;
				}
				this._outOfDanger = value;
				this.OnOutOfDangerChanged();
			}
		}

		// Token: 0x06000764 RID: 1892 RVA: 0x0001F8FA File Offset: 0x0001DAFA
		private void OnOutOfDangerChanged()
		{
			if (this.outOfDanger && this.healthComponent.shield != this.healthComponent.fullShield)
			{
				Util.PlaySound("Play_item_proc_personal_shield_recharge", base.gameObject);
			}
		}

		// Token: 0x06000765 RID: 1893 RVA: 0x0001F92D File Offset: 0x0001DB2D
		[Server]
		private void OnOutOfCombatAndDangerServer()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.CharacterBody::OnOutOfCombatAndDangerServer()' called on client");
				return;
			}
		}

		// Token: 0x06000766 RID: 1894 RVA: 0x0001F944 File Offset: 0x0001DB44
		[Server]
		public bool GetNotMoving()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Boolean RoR2.CharacterBody::GetNotMoving()' called on client");
				return false;
			}
			return this.notMovingStopwatch >= 2f;
		}

		// Token: 0x06000767 RID: 1895 RVA: 0x0001F96C File Offset: 0x0001DB6C
		public void PerformAutoCalculateLevelStats()
		{
			this.levelMaxHealth = Mathf.Round(this.baseMaxHealth * 0.3f);
			this.levelMaxShield = Mathf.Round(this.baseMaxShield * 0.3f);
			this.levelRegen = this.baseRegen * 0.2f;
			this.levelMoveSpeed = 0f;
			this.levelJumpPower = 0f;
			this.levelDamage = this.baseDamage * 0.2f;
			this.levelAttackSpeed = 0f;
			this.levelCrit = 0f;
			this.levelArmor = 0f;
		}

		// Token: 0x170000E7 RID: 231
		// (get) Token: 0x06000768 RID: 1896 RVA: 0x0001FA02 File Offset: 0x0001DC02
		// (set) Token: 0x06000769 RID: 1897 RVA: 0x0001FA0A File Offset: 0x0001DC0A
		public float experience { get; private set; }

		// Token: 0x170000E8 RID: 232
		// (get) Token: 0x0600076A RID: 1898 RVA: 0x0001FA13 File Offset: 0x0001DC13
		// (set) Token: 0x0600076B RID: 1899 RVA: 0x0001FA1B File Offset: 0x0001DC1B
		public float level { get; private set; }

		// Token: 0x170000E9 RID: 233
		// (get) Token: 0x0600076C RID: 1900 RVA: 0x0001FA24 File Offset: 0x0001DC24
		// (set) Token: 0x0600076D RID: 1901 RVA: 0x0001FA2C File Offset: 0x0001DC2C
		public float maxHealth { get; private set; }

		// Token: 0x170000EA RID: 234
		// (get) Token: 0x0600076E RID: 1902 RVA: 0x0001FA35 File Offset: 0x0001DC35
		// (set) Token: 0x0600076F RID: 1903 RVA: 0x0001FA3D File Offset: 0x0001DC3D
		public float maxBarrier { get; private set; }

		// Token: 0x170000EB RID: 235
		// (get) Token: 0x06000770 RID: 1904 RVA: 0x0001FA46 File Offset: 0x0001DC46
		// (set) Token: 0x06000771 RID: 1905 RVA: 0x0001FA4E File Offset: 0x0001DC4E
		public float barrierDecayRate { get; private set; }

		// Token: 0x170000EC RID: 236
		// (get) Token: 0x06000772 RID: 1906 RVA: 0x0001FA57 File Offset: 0x0001DC57
		// (set) Token: 0x06000773 RID: 1907 RVA: 0x0001FA5F File Offset: 0x0001DC5F
		public float regen { get; private set; }

		// Token: 0x170000ED RID: 237
		// (get) Token: 0x06000774 RID: 1908 RVA: 0x0001FA68 File Offset: 0x0001DC68
		// (set) Token: 0x06000775 RID: 1909 RVA: 0x0001FA70 File Offset: 0x0001DC70
		public float maxShield { get; private set; }

		// Token: 0x170000EE RID: 238
		// (get) Token: 0x06000776 RID: 1910 RVA: 0x0001FA79 File Offset: 0x0001DC79
		// (set) Token: 0x06000777 RID: 1911 RVA: 0x0001FA81 File Offset: 0x0001DC81
		public float moveSpeed { get; private set; }

		// Token: 0x170000EF RID: 239
		// (get) Token: 0x06000778 RID: 1912 RVA: 0x0001FA8A File Offset: 0x0001DC8A
		// (set) Token: 0x06000779 RID: 1913 RVA: 0x0001FA92 File Offset: 0x0001DC92
		public float acceleration { get; private set; }

		// Token: 0x170000F0 RID: 240
		// (get) Token: 0x0600077A RID: 1914 RVA: 0x0001FA9B File Offset: 0x0001DC9B
		// (set) Token: 0x0600077B RID: 1915 RVA: 0x0001FAA3 File Offset: 0x0001DCA3
		public float jumpPower { get; private set; }

		// Token: 0x170000F1 RID: 241
		// (get) Token: 0x0600077C RID: 1916 RVA: 0x0001FAAC File Offset: 0x0001DCAC
		// (set) Token: 0x0600077D RID: 1917 RVA: 0x0001FAB4 File Offset: 0x0001DCB4
		public int maxJumpCount { get; private set; }

		// Token: 0x170000F2 RID: 242
		// (get) Token: 0x0600077E RID: 1918 RVA: 0x0001FABD File Offset: 0x0001DCBD
		// (set) Token: 0x0600077F RID: 1919 RVA: 0x0001FAC5 File Offset: 0x0001DCC5
		public float maxJumpHeight { get; private set; }

		// Token: 0x170000F3 RID: 243
		// (get) Token: 0x06000780 RID: 1920 RVA: 0x0001FACE File Offset: 0x0001DCCE
		// (set) Token: 0x06000781 RID: 1921 RVA: 0x0001FAD6 File Offset: 0x0001DCD6
		public float damage { get; private set; }

		// Token: 0x170000F4 RID: 244
		// (get) Token: 0x06000782 RID: 1922 RVA: 0x0001FADF File Offset: 0x0001DCDF
		// (set) Token: 0x06000783 RID: 1923 RVA: 0x0001FAE7 File Offset: 0x0001DCE7
		public float attackSpeed { get; private set; }

		// Token: 0x170000F5 RID: 245
		// (get) Token: 0x06000784 RID: 1924 RVA: 0x0001FAF0 File Offset: 0x0001DCF0
		// (set) Token: 0x06000785 RID: 1925 RVA: 0x0001FAF8 File Offset: 0x0001DCF8
		public float crit { get; private set; }

		// Token: 0x170000F6 RID: 246
		// (get) Token: 0x06000786 RID: 1926 RVA: 0x0001FB01 File Offset: 0x0001DD01
		// (set) Token: 0x06000787 RID: 1927 RVA: 0x0001FB09 File Offset: 0x0001DD09
		public float armor { get; private set; }

		// Token: 0x170000F7 RID: 247
		// (get) Token: 0x06000788 RID: 1928 RVA: 0x0001FB12 File Offset: 0x0001DD12
		// (set) Token: 0x06000789 RID: 1929 RVA: 0x0001FB1A File Offset: 0x0001DD1A
		public float critHeal { get; private set; }

		// Token: 0x170000F8 RID: 248
		// (get) Token: 0x0600078A RID: 1930 RVA: 0x0001FB23 File Offset: 0x0001DD23
		// (set) Token: 0x0600078B RID: 1931 RVA: 0x0001FB2B File Offset: 0x0001DD2B
		public float cursePenalty { get; private set; }

		// Token: 0x0600078C RID: 1932 RVA: 0x0001FB34 File Offset: 0x0001DD34
		public void RecalculateStats()
		{
			this.experience = TeamManager.instance.GetTeamExperience(this.teamComponent.teamIndex);
			this.level = TeamManager.instance.GetTeamLevel(this.teamComponent.teamIndex);
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			int num6 = 0;
			int num7 = 0;
			int num8 = 0;
			int num9 = 0;
			int num10 = 0;
			int num11 = 0;
			int num12 = 0;
			int num13 = 0;
			int num14 = 0;
			int num15 = 0;
			int num16 = 0;
			int num17 = 0;
			int num18 = 0;
			int bonusStockFromBody = 0;
			int num19 = 0;
			int num20 = 0;
			int num21 = 0;
			int num22 = 0;
			int num23 = 0;
			int num24 = 0;
			int num25 = 0;
			int num26 = 0;
			int num27 = 0;
			EquipmentIndex equipmentIndex = EquipmentIndex.None;
			uint num28 = 0U;
			if (this.inventory)
			{
				this.level += (float)this.inventory.GetItemCount(ItemIndex.LevelBonus);
				num = this.inventory.GetItemCount(ItemIndex.Infusion);
				num2 = this.inventory.GetItemCount(ItemIndex.HealWhileSafe);
				num3 = this.inventory.GetItemCount(ItemIndex.PersonalShield);
				num4 = this.inventory.GetItemCount(ItemIndex.Hoof);
				num5 = this.inventory.GetItemCount(ItemIndex.SprintOutOfCombat);
				num6 = this.inventory.GetItemCount(ItemIndex.Feather);
				num7 = this.inventory.GetItemCount(ItemIndex.Syringe);
				num8 = this.inventory.GetItemCount(ItemIndex.CritGlasses);
				num9 = this.inventory.GetItemCount(ItemIndex.AttackSpeedOnCrit);
				num10 = this.inventory.GetItemCount(ItemIndex.CooldownOnCrit);
				num11 = this.inventory.GetItemCount(ItemIndex.HealOnCrit);
				num12 = this.GetBuffCount(BuffIndex.BeetleJuice);
				num13 = this.inventory.GetItemCount(ItemIndex.ShieldOnly);
				num14 = this.inventory.GetItemCount(ItemIndex.AlienHead);
				num15 = this.inventory.GetItemCount(ItemIndex.Knurl);
				num16 = this.inventory.GetItemCount(ItemIndex.BoostHp);
				num17 = this.inventory.GetItemCount(ItemIndex.CritHeal);
				num18 = this.inventory.GetItemCount(ItemIndex.SprintBonus);
				bonusStockFromBody = this.inventory.GetItemCount(ItemIndex.SecondarySkillMagazine);
				num19 = this.inventory.GetItemCount(ItemIndex.SprintArmor);
				num20 = this.inventory.GetItemCount(ItemIndex.UtilitySkillMagazine);
				num21 = this.inventory.GetItemCount(ItemIndex.HealthDecay);
				num23 = this.inventory.GetItemCount(ItemIndex.TonicAffliction);
				num24 = this.inventory.GetItemCount(ItemIndex.LunarDagger);
				num22 = this.inventory.GetItemCount(ItemIndex.DrizzlePlayerHelper);
				num25 = this.inventory.GetItemCount(ItemIndex.MonsoonPlayerHelper);
				num26 = this.inventory.GetItemCount(ItemIndex.Pearl);
				num27 = this.inventory.GetItemCount(ItemIndex.ShinyPearl);
				equipmentIndex = this.inventory.currentEquipmentIndex;
				num28 = this.inventory.infusionBonus;
			}
			float num29 = this.level - 1f;
			this.isElite = (this.eliteBuffCount > 0);
			bool flag = this.HasBuff(BuffIndex.TonicBuff);
			bool flag2 = this.HasBuff(BuffIndex.Entangle);
			bool flag3 = this.HasBuff(BuffIndex.Nullified);
			float maxHealth = this.maxHealth;
			float maxShield = this.maxShield;
			float num30 = this.baseMaxHealth + this.levelMaxHealth * num29;
			float num31 = 1f;
			num31 += (float)num16 * 0.1f;
			num31 += (float)(num26 + num27) * 0.1f;
			if (num > 0)
			{
				num30 += num28;
			}
			num30 += (float)num15 * 40f;
			num30 *= num31;
			this.maxHealth = num30;
			float num32 = this.baseMaxShield + this.levelMaxShield * num29;
			num32 += (float)num3 * 0.08f * this.maxHealth;
			if (this.HasBuff(BuffIndex.EngiShield))
			{
				num32 += this.maxHealth * 1f;
			}
			if (this.HasBuff(BuffIndex.EngiTeamShield))
			{
				num32 += this.maxHealth * 0.5f;
			}
			if (num13 > 0)
			{
				num32 += this.maxHealth * (1.5f + (float)(num13 - 1) * 0.25f);
				this.maxHealth = 1f;
			}
			if (this.HasBuff(BuffIndex.AffixBlue))
			{
				float num33 = this.maxHealth * 0.5f;
				this.maxHealth -= num33;
				num32 += this.maxHealth;
			}
			this.maxShield = num32;
			float num34 = this.baseRegen + this.levelRegen * num29;
			float num35 = 1f + num29 * 0.2f;
			float num36 = (float)num15 * 1.6f * num35;
			float num37 = ((this.outOfDanger && num2 > 0) ? (3f * (float)num2) : 0f) * num35;
			float num38 = (this.HasBuff(BuffIndex.MeatRegenBoost) ? 2f : 0f) * num35;
			float num39 = (float)num27 * 0.1f * num35;
			float num40 = 1f;
			if (num22 > 0)
			{
				num40 += 0.5f;
			}
			if (num25 > 0)
			{
				num40 -= 0.4f;
			}
			if (this.HasBuff(BuffIndex.OnFire))
			{
				num40 = 0f;
			}
			float num41 = (num34 + num36 + num37 + num38 + num39) * num40;
			if (num21 > 0)
			{
				num41 = Mathf.Min(num41, 0f) - this.maxHealth / (float)num21;
			}
			this.regen = num41;
			float num42 = this.baseMoveSpeed + this.levelMoveSpeed * num29;
			float num43 = 1f;
			if (Run.instance.enabledArtifacts.HasArtifact(ArtifactIndex.Spirit))
			{
				float num44 = 1f;
				if (this.healthComponent)
				{
					num44 = this.healthComponent.combinedHealthFraction;
				}
				num43 += 1f - num44;
			}
			if (equipmentIndex == EquipmentIndex.AffixYellow)
			{
				num42 += 2f;
			}
			if (this.isSprinting)
			{
				num42 *= this.sprintingSpeedMultiplier;
			}
			num43 += (float)num4 * 0.14f;
			num43 += (float)num27 * 0.1f;
			if (this.isSprinting && num18 > 0)
			{
				num43 += (0.1f + 0.2f * (float)num18) / this.sprintingSpeedMultiplier;
			}
			if (num5 > 0 && this.HasBuff(BuffIndex.WhipBoost))
			{
				num43 += (float)num5 * 0.3f;
			}
			if (this.HasBuff(BuffIndex.BugWings))
			{
				num43 += 0.2f;
			}
			if (this.HasBuff(BuffIndex.Warbanner))
			{
				num43 += 0.3f;
			}
			if (this.HasBuff(BuffIndex.EnrageAncientWisp))
			{
				num43 += 0.4f;
			}
			if (this.HasBuff(BuffIndex.CloakSpeed))
			{
				num43 += 0.4f;
			}
			if (this.HasBuff(BuffIndex.WarCryBuff))
			{
				num43 += 0.5f;
			}
			if (this.HasBuff(BuffIndex.EngiTeamShield))
			{
				num43 += 0.3f;
			}
			float num45 = 1f;
			if (this.HasBuff(BuffIndex.Slow50))
			{
				num45 += 0.5f;
			}
			if (this.HasBuff(BuffIndex.Slow60))
			{
				num45 += 0.6f;
			}
			if (this.HasBuff(BuffIndex.Slow80))
			{
				num45 += 0.8f;
			}
			if (this.HasBuff(BuffIndex.ClayGoo))
			{
				num45 += 0.5f;
			}
			if (this.HasBuff(BuffIndex.Slow30))
			{
				num45 += 0.3f;
			}
			if (this.HasBuff(BuffIndex.Cripple))
			{
				num45 += 1f;
			}
			num42 *= num43 / num45;
			if (num12 > 0)
			{
				num42 *= 1f - 0.05f * (float)num12;
			}
			this.moveSpeed = num42;
			this.acceleration = this.moveSpeed / this.baseMoveSpeed * this.baseAcceleration;
			if (flag2 || flag3)
			{
				this.moveSpeed = 0f;
				this.acceleration = 80f;
			}
			float jumpPower = this.baseJumpPower + this.levelJumpPower * num29;
			this.jumpPower = jumpPower;
			this.maxJumpHeight = Trajectory.CalculateApex(this.jumpPower);
			this.maxJumpCount = this.baseJumpCount + num6;
			float num46 = this.baseDamage + this.levelDamage * num29;
			float num47 = 1f;
			int num48 = this.inventory ? this.inventory.GetItemCount(ItemIndex.BoostDamage) : 0;
			if (num48 > 0)
			{
				num47 += (float)num48 * 0.1f;
			}
			if (num12 > 0)
			{
				num47 -= 0.05f * (float)num12;
			}
			if (this.HasBuff(BuffIndex.GoldEmpowered))
			{
				num47 += 1f;
			}
			num47 += (float)num27 * 0.1f;
			num47 += Mathf.Pow(2f, (float)num24) - 1f;
			num46 *= num47;
			this.damage = num46;
			float num49 = this.baseAttackSpeed + this.levelAttackSpeed * num29;
			float num50 = 1f;
			num50 += (float)num7 * 0.15f;
			if (equipmentIndex == EquipmentIndex.AffixYellow)
			{
				num50 += 0.5f;
			}
			num50 += (float)this.buffs[2] * 0.12f;
			if (this.HasBuff(BuffIndex.Warbanner))
			{
				num50 += 0.3f;
			}
			if (this.HasBuff(BuffIndex.Energized))
			{
				num50 += 0.7f;
			}
			if (this.HasBuff(BuffIndex.WarCryBuff))
			{
				num50 += 1f;
			}
			num50 += (float)num27 * 0.1f;
			num49 *= num50;
			if (num12 > 0)
			{
				num49 *= 1f - 0.05f * (float)num12;
			}
			this.attackSpeed = num49;
			float num51 = this.baseCrit + this.levelCrit * num29;
			num51 += (float)num8 * 10f;
			if (num9 > 0)
			{
				num51 += 5f;
			}
			if (num10 > 0)
			{
				num51 += 5f;
			}
			if (num11 > 0)
			{
				num51 += 5f;
			}
			if (num17 > 0)
			{
				num51 += 5f;
			}
			if (this.HasBuff(BuffIndex.FullCrit))
			{
				num51 += 100f;
			}
			num51 += (float)num27 * 10f;
			this.crit = num51;
			this.armor = this.baseArmor + this.levelArmor * num29;
			if (num27 > 0)
			{
				this.armor *= 1f + 0.1f * (float)num27;
			}
			this.armor += (float)num22 * 70f;
			this.armor += (this.HasBuff(BuffIndex.ArmorBoost) ? 200f : 0f);
			this.armor += (this.HasBuff(BuffIndex.ElephantArmorBoost) ? 500f : 0f);
			if (this.HasBuff(BuffIndex.Cripple))
			{
				this.armor -= 20f;
			}
			if (this.HasBuff(BuffIndex.Pulverized))
			{
				this.armor -= 60f;
			}
			if (this.isSprinting && num19 > 0)
			{
				this.armor += (float)(num19 * 30);
			}
			float num52 = 1f;
			if (this.HasBuff(BuffIndex.GoldEmpowered))
			{
				num52 *= 0.25f;
			}
			for (int i = 0; i < num14; i++)
			{
				num52 *= 0.75f;
			}
			if (this.HasBuff(BuffIndex.NoCooldowns))
			{
				num52 = 0f;
			}
			if (this.skillLocator.primary)
			{
				this.skillLocator.primary.cooldownScale = num52;
			}
			if (this.skillLocator.secondary)
			{
				this.skillLocator.secondary.cooldownScale = num52;
				this.skillLocator.secondary.SetBonusStockFromBody(bonusStockFromBody);
			}
			if (this.skillLocator.utility)
			{
				float num53 = num52;
				if (num20 > 0)
				{
					num53 *= 0.6666667f;
				}
				this.skillLocator.utility.cooldownScale = num53;
				this.skillLocator.utility.SetBonusStockFromBody(num20 * 2);
			}
			if (this.skillLocator.special)
			{
				this.skillLocator.special.cooldownScale = num52;
			}
			this.critHeal = 0f;
			if (num17 > 0)
			{
				float crit = this.crit;
				this.crit /= (float)(num17 + 1);
				this.critHeal = crit - this.crit;
			}
			this.cursePenalty = 1f;
			if (num24 > 0)
			{
				this.cursePenalty = Mathf.Pow(2f, (float)num24);
			}
			if (this.HasBuff(BuffIndex.Weak))
			{
				this.armor -= 30f;
				this.damage *= 0.6f;
				this.moveSpeed *= 0.6f;
			}
			if (flag)
			{
				this.maxHealth *= 1.5f;
				this.maxShield *= 1.5f;
				this.attackSpeed *= 1.7f;
				this.moveSpeed *= 1.3f;
				this.armor += 20f;
				this.damage *= 2f;
				this.regen *= 4f;
			}
			else if (num23 > 0)
			{
				float num54 = Mathf.Pow(0.95f, (float)num23);
				this.attackSpeed *= num54;
				this.moveSpeed *= num54;
				this.damage *= num54;
				this.regen *= num54;
				this.cursePenalty += 0.1f * (float)num23;
			}
			this.maxHealth /= this.cursePenalty;
			this.maxShield /= this.cursePenalty;
			this.maxBarrier = this.maxHealth + this.maxShield;
			this.barrierDecayRate = this.maxBarrier / 30f;
			if (NetworkServer.active)
			{
				float num55 = this.maxHealth - maxHealth;
				float num56 = this.maxShield - maxShield;
				if (num55 > 0f)
				{
					this.healthComponent.Heal(num55, default(ProcChainMask), false);
				}
				else if (this.healthComponent.health > this.maxHealth)
				{
					this.healthComponent.Networkhealth = Mathf.Max(this.healthComponent.health + num55, this.maxHealth);
				}
				if (num56 > 0f)
				{
					this.healthComponent.RechargeShield(num56);
				}
				else if (this.healthComponent.shield > this.maxShield)
				{
					this.healthComponent.Networkshield = Mathf.Max(this.healthComponent.shield + num56, this.maxShield);
				}
			}
			this.statsDirty = false;
		}

		// Token: 0x0600078D RID: 1933 RVA: 0x000208EE File Offset: 0x0001EAEE
		public void OnLevelChanged()
		{
			this.statsDirty = true;
		}

		// Token: 0x0600078E RID: 1934 RVA: 0x000208F7 File Offset: 0x0001EAF7
		public void SetAimTimer(float duration)
		{
			this.aimTimer = duration;
		}

		// Token: 0x170000F9 RID: 249
		// (get) Token: 0x0600078F RID: 1935 RVA: 0x00020900 File Offset: 0x0001EB00
		public bool shouldAim
		{
			get
			{
				return this.aimTimer > 0f && !this.isSprinting;
			}
		}

		// Token: 0x06000790 RID: 1936 RVA: 0x0002091C File Offset: 0x0001EB1C
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			byte b = reader.ReadByte();
			if ((b & 1) != 0)
			{
				NetworkInstanceId c = reader.ReadNetworkId();
				if (c != this.masterObjectId)
				{
					this.masterObjectId = c;
					this.statsDirty = true;
				}
			}
			if ((b & 2) != 0)
			{
				this.ReadBuffs(reader);
			}
			if ((b & 4) != 0)
			{
				bool flag = reader.ReadBoolean();
				if (!base.hasAuthority && flag != this.outOfCombat)
				{
					this.outOfCombat = flag;
					this.statsDirty = true;
				}
			}
			if ((b & 8) != 0)
			{
				bool flag2 = reader.ReadBoolean();
				if (flag2 != this.outOfDanger)
				{
					this.outOfDanger = flag2;
					this.statsDirty = true;
				}
			}
			if ((b & 16) != 0)
			{
				bool flag3 = reader.ReadBoolean();
				if (flag3 != this.isSprinting && !base.hasAuthority)
				{
					this.statsDirty = true;
					this.isSprinting = flag3;
				}
			}
		}

		// Token: 0x06000791 RID: 1937 RVA: 0x000209DC File Offset: 0x0001EBDC
		public override bool OnSerialize(NetworkWriter writer, bool initialState)
		{
			uint num = base.syncVarDirtyBits;
			if (initialState)
			{
				num = 31U;
			}
			bool flag = (num & 1U) > 0U;
			bool flag2 = (num & 2U) > 0U;
			bool flag3 = (num & 4U) > 0U;
			bool flag4 = (num & 8U) > 0U;
			bool flag5 = (num & 16U) > 0U;
			writer.Write((byte)num);
			if (flag)
			{
				writer.Write(this.masterObjectId);
			}
			if (flag2)
			{
				this.WriteBuffs(writer);
			}
			if (flag3)
			{
				writer.Write(this.outOfCombat);
			}
			if (flag4)
			{
				writer.Write(this.outOfDanger);
			}
			if (flag5)
			{
				writer.Write(this.isSprinting);
			}
			return !initialState && num > 0U;
		}

		// Token: 0x06000792 RID: 1938 RVA: 0x00020A74 File Offset: 0x0001EC74
		public T AddItemBehavior<T>(int stack) where T : CharacterBody.ItemBehavior
		{
			T t = base.GetComponent<T>();
			if (stack > 0)
			{
				if (!t)
				{
					t = base.gameObject.AddComponent<T>();
					t.body = this;
				}
				t.stack = stack;
				return t;
			}
			if (t)
			{
				UnityEngine.Object.Destroy(t);
			}
			return default(T);
		}

		// Token: 0x170000FA RID: 250
		// (get) Token: 0x06000793 RID: 1939 RVA: 0x00020AE0 File Offset: 0x0001ECE0
		// (set) Token: 0x06000794 RID: 1940 RVA: 0x00020AE8 File Offset: 0x0001ECE8
		public bool warCryReady
		{
			get
			{
				return this._warCryReady;
			}
			private set
			{
				if (this._warCryReady != value)
				{
					this._warCryReady = value;
					if (NetworkServer.active)
					{
						this.CallRpcSyncWarCryReady(value);
					}
				}
			}
		}

		// Token: 0x06000795 RID: 1941 RVA: 0x00020B08 File Offset: 0x0001ED08
		[Server]
		private void ActivateWarCryAura(int stacks)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.CharacterBody::ActivateWarCryAura(System.Int32)' called on client");
				return;
			}
			if (this.warCryAuraController)
			{
				UnityEngine.Object.Destroy(this.warCryAuraController);
			}
			this.warCryAuraController = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/WarCryAura"), this.transform.position, this.transform.rotation, this.transform);
			this.warCryAuraController.GetComponent<TeamFilter>().teamIndex = this.teamComponent.teamIndex;
			BuffWard component = this.warCryAuraController.GetComponent<BuffWard>();
			component.expireDuration = 2f + 4f * (float)stacks;
			component.Networkradius = 8f + 4f * (float)stacks;
			this.warCryAuraController.GetComponent<NetworkedBodyAttachment>().AttachToGameObjectAndSpawn(base.gameObject);
		}

		// Token: 0x06000796 RID: 1942 RVA: 0x00020BD6 File Offset: 0x0001EDD6
		[ClientRpc]
		private void RpcSyncWarCryReady(bool value)
		{
			if (!NetworkServer.active)
			{
				this.warCryReady = value;
			}
		}

		// Token: 0x06000797 RID: 1943 RVA: 0x00020BE8 File Offset: 0x0001EDE8
		public void OnKilledOtherServer(DamageReport damageReport)
		{
			this.killCount++;
			this.AddMultiKill(1);
			CharacterBody.IcicleItemBehavior component = base.GetComponent<CharacterBody.IcicleItemBehavior>();
			if (component)
			{
				component.OnOwnerKillOther();
			}
		}

		// Token: 0x06000798 RID: 1944 RVA: 0x00020C20 File Offset: 0x0001EE20
		[Server]
		private void UpdateTeslaCoil()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.CharacterBody::UpdateTeslaCoil()' called on client");
				return;
			}
			if (this.inventory)
			{
				int itemCount = this.inventory.GetItemCount(ItemIndex.ShockNearby);
				if (itemCount > 0)
				{
					this.teslaBuffRollTimer += Time.fixedDeltaTime;
					if (this.teslaBuffRollTimer >= 10f)
					{
						this.teslaBuffRollTimer = 0f;
						this.teslaCrit = Util.CheckRoll(this.crit, this.master);
						if (!this.HasBuff(BuffIndex.TeslaField))
						{
							this.AddBuff(BuffIndex.TeslaField);
						}
						else
						{
							this.RemoveBuff(BuffIndex.TeslaField);
						}
					}
					if (this.HasBuff(BuffIndex.TeslaField))
					{
						this.teslaFireTimer += Time.fixedDeltaTime;
						this.teslaResetListTimer += Time.fixedDeltaTime;
						if (this.teslaFireTimer >= 0.083333336f)
						{
							this.teslaFireTimer = 0f;
							LightningOrb lightningOrb = new LightningOrb
							{
								origin = this.corePosition,
								damageValue = this.damage * 2f,
								isCrit = this.teslaCrit,
								bouncesRemaining = 2 * itemCount,
								teamIndex = this.teamComponent.teamIndex,
								attacker = base.gameObject,
								procCoefficient = 0.3f,
								bouncedObjects = this.previousTeslaTargetList,
								lightningType = LightningOrb.LightningType.Tesla,
								damageColorIndex = DamageColorIndex.Item,
								range = 35f
							};
							HurtBox hurtBox = lightningOrb.PickNextTarget(this.transform.position);
							if (hurtBox)
							{
								this.previousTeslaTargetList.Add(hurtBox.healthComponent);
								lightningOrb.target = hurtBox;
								OrbManager.instance.AddOrb(lightningOrb);
							}
						}
						if (this.teslaResetListTimer >= this.teslaResetListInterval)
						{
							this.teslaResetListTimer -= this.teslaResetListInterval;
							this.previousTeslaTargetList.Clear();
						}
					}
				}
			}
		}

		// Token: 0x06000799 RID: 1945 RVA: 0x00020DFF File Offset: 0x0001EFFF
		public void AddHelfireDuration(float duration)
		{
			this.helfireLifetime = duration;
		}

		// Token: 0x0600079A RID: 1946 RVA: 0x00020E08 File Offset: 0x0001F008
		[Server]
		private void UpdateHelfire()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.CharacterBody::UpdateHelfire()' called on client");
				return;
			}
			this.helfireLifetime -= Time.fixedDeltaTime;
			bool flag = false;
			if (this.inventory)
			{
				flag = (this.inventory.GetItemCount(ItemIndex.BurnNearby) > 0 || this.helfireLifetime > 0f);
			}
			if (this.helfireController != flag)
			{
				if (flag)
				{
					this.helfireController = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/HelfireController")).GetComponent<HelfireController>();
					this.helfireController.networkedBodyAttachment.AttachToGameObjectAndSpawn(base.gameObject);
					return;
				}
				UnityEngine.Object.Destroy(this.helfireController.gameObject);
				this.helfireController = null;
			}
		}

		// Token: 0x0600079B RID: 1947 RVA: 0x00020EC8 File Offset: 0x0001F0C8
		private void UpdateFireTrail()
		{
			bool flag = this.HasBuff(BuffIndex.AffixRed);
			if (flag != this.fireTrail)
			{
				if (flag)
				{
					this.fireTrail = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/FireTrail"), this.transform).GetComponent<DamageTrail>();
					this.fireTrail.transform.position = this.footPosition;
					this.fireTrail.owner = base.gameObject;
					this.fireTrail.radius *= this.radius;
				}
				else
				{
					UnityEngine.Object.Destroy(this.fireTrail.gameObject);
					this.fireTrail = null;
				}
			}
			if (this.fireTrail)
			{
				this.fireTrail.damagePerSecond = this.damage * 1.5f;
			}
		}

		// Token: 0x0600079C RID: 1948 RVA: 0x00020F8C File Offset: 0x0001F18C
		private void UpdateBeetleGuardAllies()
		{
			if (NetworkServer.active)
			{
				int num = this.inventory ? this.inventory.GetItemCount(ItemIndex.BeetleGland) : 0;
				if (num > 0)
				{
					int deployableCount = this.master.GetDeployableCount(DeployableSlot.BeetleGuardAlly);
					if (deployableCount < num)
					{
						this.guardResummonCooldown -= Time.fixedDeltaTime;
						if (this.guardResummonCooldown <= 0f)
						{
							DirectorSpawnRequest directorSpawnRequest = new DirectorSpawnRequest((SpawnCard)Resources.Load("SpawnCards/CharacterSpawnCards/cscBeetleGuardAlly"), new DirectorPlacementRule
							{
								placementMode = DirectorPlacementRule.PlacementMode.Approximate,
								minDistance = 3f,
								maxDistance = 40f,
								spawnOnTarget = this.transform
							}, RoR2Application.rng);
							directorSpawnRequest.summonerBodyObject = base.gameObject;
							GameObject gameObject = DirectorCore.instance.TrySpawnObject(directorSpawnRequest);
							if (gameObject)
							{
								CharacterMaster component = gameObject.GetComponent<CharacterMaster>();
								AIOwnership component2 = gameObject.GetComponent<AIOwnership>();
								BaseAI component3 = gameObject.GetComponent<BaseAI>();
								if (component)
								{
									component.inventory.GiveItem(ItemIndex.BoostDamage, 30);
									component.inventory.GiveItem(ItemIndex.BoostHp, 10);
									GameObject bodyObject = component.GetBodyObject();
									if (bodyObject)
									{
										Deployable component4 = bodyObject.GetComponent<Deployable>();
										this.master.AddDeployable(component4, DeployableSlot.BeetleGuardAlly);
									}
								}
								if (component2)
								{
									component2.ownerMaster = this.master;
								}
								if (component3)
								{
									component3.leader.gameObject = base.gameObject;
								}
							}
							if (deployableCount < num)
							{
								this.guardResummonCooldown = 1f;
								return;
							}
							this.guardResummonCooldown = 30f;
						}
					}
				}
			}
		}

		// Token: 0x0600079D RID: 1949 RVA: 0x00021120 File Offset: 0x0001F320
		private void UpdateAffixPoison(float deltaTime)
		{
			if (this.HasBuff(BuffIndex.AffixPoison))
			{
				this.poisonballTimer += deltaTime;
				if (this.poisonballTimer >= 6f)
				{
					int num = 3 + (int)this.radius;
					this.poisonballTimer = 0f;
					Vector3 up = Vector3.up;
					float num2 = 360f / (float)num;
					Vector3 normalized = Vector3.ProjectOnPlane(this.transform.forward, up).normalized;
					Vector3 point = Vector3.RotateTowards(up, normalized, 0.43633232f, float.PositiveInfinity);
					for (int i = 0; i < num; i++)
					{
						Vector3 forward = Quaternion.AngleAxis(num2 * (float)i, up) * point;
						ProjectileManager.instance.FireProjectile(Resources.Load<GameObject>("Prefabs/Projectiles/PoisonOrbProjectile"), this.corePosition, Util.QuaternionSafeLookRotation(forward), base.gameObject, this.damage * 1f, 0f, Util.CheckRoll(this.crit, this.master), DamageColorIndex.Default, null, -1f);
					}
				}
			}
		}

		// Token: 0x170000FB RID: 251
		// (get) Token: 0x0600079E RID: 1950 RVA: 0x0002121E File Offset: 0x0001F41E
		public float bestFitRadius
		{
			get
			{
				return Mathf.Max(this.radius, this.characterMotor ? this.characterMotor.capsuleHeight : 1f);
			}
		}

		// Token: 0x0600079F RID: 1951 RVA: 0x0002124C File Offset: 0x0001F44C
		private void UpdateAllTemporaryVisualEffects()
		{
			this.UpdateSingleTemporaryVisualEffect(ref this.engiShieldTempEffect, "Prefabs/TemporaryVisualEffects/EngiShield", this.bestFitRadius, this.healthComponent.shield > 0f && this.HasBuff(BuffIndex.EngiShield), "");
			this.UpdateSingleTemporaryVisualEffect(ref this.bucklerShieldTempEffect, "Prefabs/TemporaryVisualEffects/BucklerDefense", this.radius, this.isSprinting && this.inventory.GetItemCount(ItemIndex.SprintArmor) > 0, "");
			this.UpdateSingleTemporaryVisualEffect(ref this.slowDownTimeTempEffect, "Prefabs/TemporaryVisualEffects/SlowDownTime", this.radius, this.HasBuff(BuffIndex.Slow60), "");
			this.UpdateSingleTemporaryVisualEffect(ref this.crippleEffect, "Prefabs/TemporaryVisualEffects/CrippleEffect", this.radius, this.HasBuff(BuffIndex.Cripple), "");
			this.UpdateSingleTemporaryVisualEffect(ref this.tonicBuffEffect, "Prefabs/TemporaryVisualEffects/TonicBuffEffect", this.radius, this.HasBuff(BuffIndex.TonicBuff), "");
			this.UpdateSingleTemporaryVisualEffect(ref this.weakTempEffect, "Prefabs/TemporaryVisualEffects/WeakEffect", this.radius, this.HasBuff(BuffIndex.Weak), "");
			this.UpdateSingleTemporaryVisualEffect(ref this.energizedTempEffect, "Prefabs/TemporaryVisualEffects/EnergizedEffect", this.radius, this.HasBuff(BuffIndex.Energized), "");
			this.UpdateSingleTemporaryVisualEffect(ref this.barrierTempEffect, "Prefabs/TemporaryVisualEffects/BarrierEffect", this.bestFitRadius, this.healthComponent.barrier > 0f, "");
			this.UpdateSingleTemporaryVisualEffect(ref this.regenBoostEffect, "Prefabs/TemporaryVisualEffects/RegenBoostEffect", this.bestFitRadius, this.HasBuff(BuffIndex.MeatRegenBoost), "");
			this.UpdateSingleTemporaryVisualEffect(ref this.nullSafeZoneEffect, "Prefabs/TemporaryVisualEffects/NullSafeZoneEffect", this.radius, this.HasBuff(BuffIndex.NullSafeZone), "");
			this.UpdateSingleTemporaryVisualEffect(ref this.elephantDefenseEffect, "Prefabs/TemporaryVisualEffects/ElephantDefense", this.radius, this.HasBuff(BuffIndex.ElephantArmorBoost), "");
			this.UpdateSingleTemporaryVisualEffect(ref this.healingDisabledEffect, "Prefabs/TemporaryVisualEffects/HealingDisabledEffect", this.radius, this.HasBuff(BuffIndex.HealingDisabled), "");
			this.UpdateSingleTemporaryVisualEffect(ref this.noCooldownEffect, "Prefabs/TemporaryVisualEffects/NoCooldownEffect", this.radius, this.HasBuff(BuffIndex.NoCooldowns), "Head");
			int buffCount = this.GetBuffCount(BuffIndex.NullifyStack);
			this.UpdateSingleTemporaryVisualEffect(ref this.nullifyStack1Effect, "Prefabs/TemporaryVisualEffects/NullifyStack1Effect", this.radius, buffCount == 1, "");
			this.UpdateSingleTemporaryVisualEffect(ref this.nullifyStack2Effect, "Prefabs/TemporaryVisualEffects/NullifyStack2Effect", this.radius, buffCount == 2, "");
			this.UpdateSingleTemporaryVisualEffect(ref this.nullifyStack3Effect, "Prefabs/TemporaryVisualEffects/NullifyStack3Effect", this.radius, this.HasBuff(BuffIndex.Nullified), "");
		}

		// Token: 0x060007A0 RID: 1952 RVA: 0x000214CC File Offset: 0x0001F6CC
		private void UpdateSingleTemporaryVisualEffect(ref TemporaryVisualEffect tempEffect, string resourceString, float effectRadius, bool active, string childLocatorOverride = "")
		{
			bool flag = tempEffect != null;
			if (flag != active)
			{
				if (active)
				{
					if (!flag)
					{
						GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>(resourceString), this.corePosition, Quaternion.identity);
						tempEffect = gameObject.GetComponent<TemporaryVisualEffect>();
						tempEffect.parentTransform = this.coreTransform;
						tempEffect.visualState = TemporaryVisualEffect.VisualState.Enter;
						tempEffect.healthComponent = this.healthComponent;
						tempEffect.radius = effectRadius;
						LocalCameraEffect component = gameObject.GetComponent<LocalCameraEffect>();
						if (component)
						{
							component.targetCharacter = base.gameObject;
						}
						if (!string.IsNullOrEmpty(childLocatorOverride))
						{
							ModelLocator modelLocator = this.modelLocator;
							ChildLocator childLocator;
							if (modelLocator == null)
							{
								childLocator = null;
							}
							else
							{
								Transform modelTransform = modelLocator.modelTransform;
								childLocator = ((modelTransform != null) ? modelTransform.GetComponent<ChildLocator>() : null);
							}
							ChildLocator childLocator2 = childLocator;
							if (childLocator2)
							{
								Transform transform = childLocator2.FindChild(childLocatorOverride);
								if (transform)
								{
									tempEffect.parentTransform = transform;
									return;
								}
							}
						}
					}
				}
				else if (tempEffect)
				{
					tempEffect.visualState = TemporaryVisualEffect.VisualState.Exit;
				}
			}
		}

		// Token: 0x060007A1 RID: 1953 RVA: 0x000215BC File Offset: 0x0001F7BC
		public VisibilityLevel GetVisibilityLevel(CharacterBody observer)
		{
			if (!this.HasBuff(BuffIndex.Cloak))
			{
				return VisibilityLevel.Visible;
			}
			if (!observer)
			{
				return VisibilityLevel.Revealed;
			}
			TeamIndex teamIndex = this.teamComponent ? this.teamComponent.teamIndex : TeamIndex.Neutral;
			TeamIndex teamIndex2 = observer.teamComponent ? observer.teamComponent.teamIndex : TeamIndex.Neutral;
			if (teamIndex != teamIndex2)
			{
				return VisibilityLevel.Cloaked;
			}
			return VisibilityLevel.Revealed;
		}

		// Token: 0x060007A2 RID: 1954 RVA: 0x0002161B File Offset: 0x0001F81B
		public void AddSpreadBloom(float value)
		{
			this.spreadBloomInternal = Mathf.Min(this.spreadBloomInternal + value, 1f);
		}

		// Token: 0x060007A3 RID: 1955 RVA: 0x00021635 File Offset: 0x0001F835
		public void SetSpreadBloom(float value, bool canOnlyIncreaseBloom = true)
		{
			if (canOnlyIncreaseBloom)
			{
				this.spreadBloomInternal = Mathf.Clamp(value, this.spreadBloomInternal, 1f);
				return;
			}
			this.spreadBloomInternal = Mathf.Min(value, 1f);
		}

		// Token: 0x060007A4 RID: 1956 RVA: 0x00021664 File Offset: 0x0001F864
		private void UpdateSpreadBloom(float dt)
		{
			float num = 1f / this.spreadBloomDecayTime;
			this.spreadBloomInternal = Mathf.Max(this.spreadBloomInternal - num * dt, 0f);
		}

		// Token: 0x170000FC RID: 252
		// (get) Token: 0x060007A5 RID: 1957 RVA: 0x00021698 File Offset: 0x0001F898
		public float spreadBloomAngle
		{
			get
			{
				return this.spreadBloomCurve.Evaluate(this.spreadBloomInternal);
			}
		}

		// Token: 0x060007A6 RID: 1958 RVA: 0x000216AC File Offset: 0x0001F8AC
		[Client]
		public void SendConstructTurret(CharacterBody builder, Vector3 position, Quaternion rotation, MasterCatalog.MasterIndex masterIndex)
		{
			if (!NetworkClient.active)
			{
				Debug.LogWarning("[Client] function 'System.Void RoR2.CharacterBody::SendConstructTurret(RoR2.CharacterBody,UnityEngine.Vector3,UnityEngine.Quaternion,RoR2.MasterCatalog/MasterIndex)' called on server");
				return;
			}
			CharacterBody.ConstructTurretMessage msg = new CharacterBody.ConstructTurretMessage
			{
				builder = builder.gameObject,
				position = position,
				rotation = rotation,
				turretMasterIndex = masterIndex
			};
			ClientScene.readyConnection.Send(62, msg);
		}

		// Token: 0x060007A7 RID: 1959 RVA: 0x0002170C File Offset: 0x0001F90C
		[NetworkMessageHandler(msgType = 62, server = true)]
		private static void HandleConstructTurret(NetworkMessage netMsg)
		{
			CharacterBody.ConstructTurretMessage constructTurretMessage = netMsg.ReadMessage<CharacterBody.ConstructTurretMessage>();
			if (constructTurretMessage.builder)
			{
				CharacterBody component = constructTurretMessage.builder.GetComponent<CharacterBody>();
				if (component)
				{
					CharacterMaster master = component.master;
					if (master)
					{
						CharacterMaster characterMaster = new MasterSummon
						{
							masterPrefab = MasterCatalog.GetMasterPrefab(constructTurretMessage.turretMasterIndex),
							position = constructTurretMessage.position,
							rotation = constructTurretMessage.rotation,
							summonerBodyObject = component.gameObject,
							ignoreTeamMemberLimit = true
						}.Perform();
						Inventory inventory = characterMaster.inventory;
						inventory.CopyItemsFrom(master.inventory);
						inventory.ResetItem(ItemIndex.WardOnLevel);
						inventory.ResetItem(ItemIndex.BeetleGland);
						inventory.ResetItem(ItemIndex.CrippleWardOnLevel);
						inventory.ResetItem(ItemIndex.TPHealingNova);
						Deployable deployable = characterMaster.gameObject.AddComponent<Deployable>();
						deployable.onUndeploy = new UnityEvent();
						deployable.onUndeploy.AddListener(new UnityAction(characterMaster.TrueKill));
						master.AddDeployable(deployable, DeployableSlot.EngiTurret);
					}
				}
			}
		}

		// Token: 0x170000FD RID: 253
		// (get) Token: 0x060007A8 RID: 1960 RVA: 0x00021810 File Offset: 0x0001FA10
		// (set) Token: 0x060007A9 RID: 1961 RVA: 0x00021818 File Offset: 0x0001FA18
		public int multiKillCount { get; private set; }

		// Token: 0x060007AA RID: 1962 RVA: 0x00021824 File Offset: 0x0001FA24
		[Server]
		public void AddMultiKill(int kills)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.CharacterBody::AddMultiKill(System.Int32)' called on client");
				return;
			}
			this.multiKillTimer = 1f;
			this.multiKillCount += kills;
			int num = this.inventory ? this.inventory.GetItemCount(ItemIndex.WarCryOnMultiKill) : 0;
			if (num > 0 && this.multiKillCount >= 4)
			{
				this.AddTimedBuff(BuffIndex.WarCryBuff, 2f + 4f * (float)num);
			}
		}

		// Token: 0x060007AB RID: 1963 RVA: 0x000218A0 File Offset: 0x0001FAA0
		[Server]
		private void UpdateMultiKill(float deltaTime)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.CharacterBody::UpdateMultiKill(System.Single)' called on client");
				return;
			}
			this.multiKillTimer -= deltaTime;
			if (this.multiKillTimer <= 0f)
			{
				this.multiKillTimer = 0f;
				this.multiKillCount = 0;
			}
		}

		// Token: 0x170000FE RID: 254
		// (get) Token: 0x060007AC RID: 1964 RVA: 0x000218EF File Offset: 0x0001FAEF
		public Vector3 corePosition
		{
			get
			{
				if (!this.coreTransform)
				{
					return this.transform.position;
				}
				return this.coreTransform.position;
			}
		}

		// Token: 0x170000FF RID: 255
		// (get) Token: 0x060007AD RID: 1965 RVA: 0x00021918 File Offset: 0x0001FB18
		public Vector3 footPosition
		{
			get
			{
				Vector3 position = this.transform.position;
				if (this.characterMotor)
				{
					position.y -= this.characterMotor.capsuleHeight * 0.5f;
				}
				return position;
			}
		}

		// Token: 0x17000100 RID: 256
		// (get) Token: 0x060007AE RID: 1966 RVA: 0x0002195B File Offset: 0x0001FB5B
		// (set) Token: 0x060007AF RID: 1967 RVA: 0x00021963 File Offset: 0x0001FB63
		public float radius { get; private set; }

		// Token: 0x17000101 RID: 257
		// (get) Token: 0x060007B0 RID: 1968 RVA: 0x0002196C File Offset: 0x0001FB6C
		public Vector3 aimOrigin
		{
			get
			{
				if (!this.aimOriginTransform)
				{
					return this.corePosition;
				}
				return this.aimOriginTransform.position;
			}
		}

		// Token: 0x17000102 RID: 258
		// (get) Token: 0x060007B1 RID: 1969 RVA: 0x0002198D File Offset: 0x0001FB8D
		// (set) Token: 0x060007B2 RID: 1970 RVA: 0x00021995 File Offset: 0x0001FB95
		public bool isElite { get; private set; }

		// Token: 0x17000103 RID: 259
		// (get) Token: 0x060007B3 RID: 1971 RVA: 0x0002199E File Offset: 0x0001FB9E
		public bool isBoss
		{
			get
			{
				return this.master && this.master.isBoss;
			}
		}

		// Token: 0x17000104 RID: 260
		// (get) Token: 0x060007B4 RID: 1972 RVA: 0x000219BA File Offset: 0x0001FBBA
		public bool isFlying
		{
			get
			{
				return !this.characterMotor || this.characterMotor.isFlying;
			}
		}

		// Token: 0x060007B5 RID: 1973 RVA: 0x000219D6 File Offset: 0x0001FBD6
		[ClientRpc]
		public void RpcBark()
		{
			if (this.sfxLocator)
			{
				Util.PlaySound(this.sfxLocator.barkSound, base.gameObject);
			}
		}

		// Token: 0x060007B6 RID: 1974 RVA: 0x000219FC File Offset: 0x0001FBFC
		[Command]
		public void CmdRequestVehicleEjection()
		{
			if (this.currentVehicle)
			{
				this.currentVehicle.EjectPassenger(base.gameObject);
			}
		}

		// Token: 0x060007B7 RID: 1975 RVA: 0x00021A1C File Offset: 0x0001FC1C
		public bool RollCrit()
		{
			return this.master && Util.CheckRoll(this.crit, this.master);
		}

		// Token: 0x060007B8 RID: 1976 RVA: 0x00021A3E File Offset: 0x0001FC3E
		[ClientRpc]
		private void RpcUsePreferredInitialStateType()
		{
			if (this.hasEffectiveAuthority)
			{
				this.SetBodyStateToPreferredInitialState();
			}
		}

		// Token: 0x060007B9 RID: 1977 RVA: 0x00021A4E File Offset: 0x0001FC4E
		public void SetBodyStateToPreferredInitialState()
		{
			if (!this.hasEffectiveAuthority)
			{
				if (NetworkServer.active)
				{
					this.CallRpcUsePreferredInitialStateType();
				}
				return;
			}
			EntityStateMachine entityStateMachine = EntityStateMachine.FindByCustomName(base.gameObject, "Body");
			if (entityStateMachine == null)
			{
				return;
			}
			entityStateMachine.SetState(EntityState.Instantiate(this.preferredInitialStateType));
		}

		// Token: 0x060007BA RID: 1978 RVA: 0x00021A8B File Offset: 0x0001FC8B
		[Server]
		public void SetLoadoutServer(Loadout loadout)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.CharacterBody::SetLoadoutServer(RoR2.Loadout)' called on client");
				return;
			}
			this.skillLocator.ApplyLoadoutServer(loadout, this.bodyIndex);
		}

		// Token: 0x17000105 RID: 261
		// (get) Token: 0x060007BB RID: 1979 RVA: 0x00021AB4 File Offset: 0x0001FCB4
		// (set) Token: 0x060007BC RID: 1980 RVA: 0x00021ABC File Offset: 0x0001FCBC
		public Run.FixedTimeStamp localStartTime { get; private set; } = Run.FixedTimeStamp.positiveInfinity;

		// Token: 0x060007BE RID: 1982 RVA: 0x00021B50 File Offset: 0x0001FD50
		static CharacterBody()
		{
			NetworkBehaviour.RegisterCommandDelegate(typeof(CharacterBody), CharacterBody.kCmdCmdUpdateSprint, new NetworkBehaviour.CmdDelegate(CharacterBody.InvokeCmdCmdUpdateSprint));
			CharacterBody.kCmdCmdOnSkillActivated = 384138986;
			NetworkBehaviour.RegisterCommandDelegate(typeof(CharacterBody), CharacterBody.kCmdCmdOnSkillActivated, new NetworkBehaviour.CmdDelegate(CharacterBody.InvokeCmdCmdOnSkillActivated));
			CharacterBody.kCmdCmdRequestVehicleEjection = 1803737791;
			NetworkBehaviour.RegisterCommandDelegate(typeof(CharacterBody), CharacterBody.kCmdCmdRequestVehicleEjection, new NetworkBehaviour.CmdDelegate(CharacterBody.InvokeCmdCmdRequestVehicleEjection));
			CharacterBody.kRpcRpcSyncWarCryReady = 1893254821;
			NetworkBehaviour.RegisterRpcDelegate(typeof(CharacterBody), CharacterBody.kRpcRpcSyncWarCryReady, new NetworkBehaviour.CmdDelegate(CharacterBody.InvokeRpcRpcSyncWarCryReady));
			CharacterBody.kRpcRpcBark = -76716871;
			NetworkBehaviour.RegisterRpcDelegate(typeof(CharacterBody), CharacterBody.kRpcRpcBark, new NetworkBehaviour.CmdDelegate(CharacterBody.InvokeRpcRpcBark));
			CharacterBody.kRpcRpcUsePreferredInitialStateType = 638695010;
			NetworkBehaviour.RegisterRpcDelegate(typeof(CharacterBody), CharacterBody.kRpcRpcUsePreferredInitialStateType, new NetworkBehaviour.CmdDelegate(CharacterBody.InvokeRpcRpcUsePreferredInitialStateType));
			NetworkCRC.RegisterBehaviour("CharacterBody", 0);
		}

		// Token: 0x060007BF RID: 1983 RVA: 0x00021C84 File Offset: 0x0001FE84
		[CompilerGenerated]
		private void <ReadBuffs>g__ZeroBuffIndexRange|17_0(BuffIndex start, BuffIndex end, ref CharacterBody.<>c__DisplayClass17_0 A_3)
		{
			while (A_3.activeBuffsIndexToCheck < this.activeBuffsListCount)
			{
				BuffIndex buffIndex = this.activeBuffsList[A_3.activeBuffsIndexToCheck];
				if (end <= buffIndex)
				{
					return;
				}
				int activeBuffsIndexToCheck;
				if (start <= buffIndex)
				{
					this.SetBuffCount(buffIndex, 0);
					activeBuffsIndexToCheck = A_3.activeBuffsIndexToCheck - 1;
					A_3.activeBuffsIndexToCheck = activeBuffsIndexToCheck;
				}
				activeBuffsIndexToCheck = A_3.activeBuffsIndexToCheck + 1;
				A_3.activeBuffsIndexToCheck = activeBuffsIndexToCheck;
			}
		}

		// Token: 0x060007C0 RID: 1984 RVA: 0x00021CE0 File Offset: 0x0001FEE0
		[CompilerGenerated]
		private void <AddTimedBuff>g__DefaultBehavior|25_0(ref CharacterBody.<>c__DisplayClass25_0 A_1)
		{
			bool flag = false;
			if (!A_1.buffDef.canStack)
			{
				for (int i = 0; i < this.timedBuffs.Count; i++)
				{
					if (this.timedBuffs[i].buffIndex == A_1.buffType)
					{
						flag = true;
						this.timedBuffs[i].timer = Mathf.Max(this.timedBuffs[i].timer, A_1.duration);
						break;
					}
				}
			}
			if (!flag)
			{
				this.timedBuffs.Add(new CharacterBody.TimedBuff
				{
					buffIndex = A_1.buffType,
					timer = A_1.duration
				});
				this.AddBuff(A_1.buffType);
			}
		}

		// Token: 0x060007C1 RID: 1985 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x060007C2 RID: 1986 RVA: 0x00021D93 File Offset: 0x0001FF93
		protected static void InvokeCmdCmdUpdateSprint(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("Command CmdUpdateSprint called on client.");
				return;
			}
			((CharacterBody)obj).CmdUpdateSprint(reader.ReadBoolean());
		}

		// Token: 0x060007C3 RID: 1987 RVA: 0x00021DBC File Offset: 0x0001FFBC
		protected static void InvokeCmdCmdOnSkillActivated(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("Command CmdOnSkillActivated called on client.");
				return;
			}
			((CharacterBody)obj).CmdOnSkillActivated((sbyte)reader.ReadPackedUInt32());
		}

		// Token: 0x060007C4 RID: 1988 RVA: 0x00021DE5 File Offset: 0x0001FFE5
		protected static void InvokeCmdCmdRequestVehicleEjection(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("Command CmdRequestVehicleEjection called on client.");
				return;
			}
			((CharacterBody)obj).CmdRequestVehicleEjection();
		}

		// Token: 0x060007C5 RID: 1989 RVA: 0x00021E08 File Offset: 0x00020008
		public void CallCmdUpdateSprint(bool newIsSprinting)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("Command function CmdUpdateSprint called on server.");
				return;
			}
			if (base.isServer)
			{
				this.CmdUpdateSprint(newIsSprinting);
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.Write(0);
			networkWriter.Write((short)((ushort)5));
			networkWriter.WritePackedUInt32((uint)CharacterBody.kCmdCmdUpdateSprint);
			networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
			networkWriter.Write(newIsSprinting);
			base.SendCommandInternal(networkWriter, 0, "CmdUpdateSprint");
		}

		// Token: 0x060007C6 RID: 1990 RVA: 0x00021E94 File Offset: 0x00020094
		public void CallCmdOnSkillActivated(sbyte skillIndex)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("Command function CmdOnSkillActivated called on server.");
				return;
			}
			if (base.isServer)
			{
				this.CmdOnSkillActivated(skillIndex);
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.Write(0);
			networkWriter.Write((short)((ushort)5));
			networkWriter.WritePackedUInt32((uint)CharacterBody.kCmdCmdOnSkillActivated);
			networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
			networkWriter.WritePackedUInt32((uint)skillIndex);
			base.SendCommandInternal(networkWriter, 0, "CmdOnSkillActivated");
		}

		// Token: 0x060007C7 RID: 1991 RVA: 0x00021F20 File Offset: 0x00020120
		public void CallCmdRequestVehicleEjection()
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("Command function CmdRequestVehicleEjection called on server.");
				return;
			}
			if (base.isServer)
			{
				this.CmdRequestVehicleEjection();
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.Write(0);
			networkWriter.Write((short)((ushort)5));
			networkWriter.WritePackedUInt32((uint)CharacterBody.kCmdCmdRequestVehicleEjection);
			networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
			base.SendCommandInternal(networkWriter, 0, "CmdRequestVehicleEjection");
		}

		// Token: 0x060007C8 RID: 1992 RVA: 0x00021F9C File Offset: 0x0002019C
		protected static void InvokeRpcRpcSyncWarCryReady(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("RPC RpcSyncWarCryReady called on server.");
				return;
			}
			((CharacterBody)obj).RpcSyncWarCryReady(reader.ReadBoolean());
		}

		// Token: 0x060007C9 RID: 1993 RVA: 0x00021FC5 File Offset: 0x000201C5
		protected static void InvokeRpcRpcBark(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("RPC RpcBark called on server.");
				return;
			}
			((CharacterBody)obj).RpcBark();
		}

		// Token: 0x060007CA RID: 1994 RVA: 0x00021FE8 File Offset: 0x000201E8
		protected static void InvokeRpcRpcUsePreferredInitialStateType(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("RPC RpcUsePreferredInitialStateType called on server.");
				return;
			}
			((CharacterBody)obj).RpcUsePreferredInitialStateType();
		}

		// Token: 0x060007CB RID: 1995 RVA: 0x0002200C File Offset: 0x0002020C
		public void CallRpcSyncWarCryReady(bool value)
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("RPC Function RpcSyncWarCryReady called on client.");
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.Write(0);
			networkWriter.Write((short)((ushort)2));
			networkWriter.WritePackedUInt32((uint)CharacterBody.kRpcRpcSyncWarCryReady);
			networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
			networkWriter.Write(value);
			this.SendRPCInternal(networkWriter, 0, "RpcSyncWarCryReady");
		}

		// Token: 0x060007CC RID: 1996 RVA: 0x00022080 File Offset: 0x00020280
		public void CallRpcBark()
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("RPC Function RpcBark called on client.");
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.Write(0);
			networkWriter.Write((short)((ushort)2));
			networkWriter.WritePackedUInt32((uint)CharacterBody.kRpcRpcBark);
			networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
			this.SendRPCInternal(networkWriter, 0, "RpcBark");
		}

		// Token: 0x060007CD RID: 1997 RVA: 0x000220EC File Offset: 0x000202EC
		public void CallRpcUsePreferredInitialStateType()
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("RPC Function RpcUsePreferredInitialStateType called on client.");
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.Write(0);
			networkWriter.Write((short)((ushort)2));
			networkWriter.WritePackedUInt32((uint)CharacterBody.kRpcRpcUsePreferredInitialStateType);
			networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
			this.SendRPCInternal(networkWriter, 0, "RpcUsePreferredInitialStateType");
		}

		// Token: 0x0400079B RID: 1947
		[Tooltip("This is assigned to the prefab automatically by BodyCatalog at runtime. Do not set this value manually.")]
		[HideInInspector]
		public int bodyIndex = -1;

		// Token: 0x0400079C RID: 1948
		[Tooltip("The language token to use as the base name of this character.")]
		public string baseNameToken;

		// Token: 0x0400079D RID: 1949
		public string subtitleNameToken;

		// Token: 0x0400079E RID: 1950
		private BuffIndex[] activeBuffsList;

		// Token: 0x0400079F RID: 1951
		private int activeBuffsListCount;

		// Token: 0x040007A0 RID: 1952
		private int[] buffs;

		// Token: 0x040007A1 RID: 1953
		private int eliteBuffCount;

		// Token: 0x040007A2 RID: 1954
		private List<CharacterBody.TimedBuff> timedBuffs = new List<CharacterBody.TimedBuff>();

		// Token: 0x040007A3 RID: 1955
		[NonSerialized]
		public int pendingTonicAfflictionCount;

		// Token: 0x040007A4 RID: 1956
		private GameObject warCryEffectInstance;

		// Token: 0x040007A5 RID: 1957
		[EnumMask(typeof(CharacterBody.BodyFlags))]
		public CharacterBody.BodyFlags bodyFlags;

		// Token: 0x040007A6 RID: 1958
		private NetworkInstanceId masterObjectId;

		// Token: 0x040007A7 RID: 1959
		private GameObject _masterObject;

		// Token: 0x040007A8 RID: 1960
		private CharacterMaster _master;

		// Token: 0x040007AA RID: 1962
		private bool linkedToMaster;

		// Token: 0x040007AC RID: 1964
		private bool disablingHurtBoxes;

		// Token: 0x040007AD RID: 1965
		private EquipmentIndex previousEquipmentIndex;

		// Token: 0x040007B0 RID: 1968
		private new Transform transform;

		// Token: 0x040007B9 RID: 1977
		private SfxLocator sfxLocator;

		// Token: 0x040007BE RID: 1982
		private static List<CharacterBody> instancesList = new List<CharacterBody>();

		// Token: 0x040007BF RID: 1983
		public static readonly ReadOnlyCollection<CharacterBody> readOnlyInstancesList = new ReadOnlyCollection<CharacterBody>(CharacterBody.instancesList);

		// Token: 0x040007C1 RID: 1985
		private bool _isSprinting;

		// Token: 0x040007C2 RID: 1986
		private float sprintingSpeedMultiplier = 1.45f;

		// Token: 0x040007C3 RID: 1987
		private const float outOfCombatDelay = 5f;

		// Token: 0x040007C4 RID: 1988
		private const float outOfDangerDelay = 7f;

		// Token: 0x040007C5 RID: 1989
		private float outOfCombatStopwatch;

		// Token: 0x040007C6 RID: 1990
		private float outOfDangerStopwatch;

		// Token: 0x040007C8 RID: 1992
		private bool _outOfDanger = true;

		// Token: 0x040007C9 RID: 1993
		private Vector3 previousPosition;

		// Token: 0x040007CA RID: 1994
		private const float notMovingWait = 2f;

		// Token: 0x040007CB RID: 1995
		private float notMovingStopwatch;

		// Token: 0x040007CC RID: 1996
		public bool rootMotionInMainState;

		// Token: 0x040007CD RID: 1997
		public float mainRootSpeed;

		// Token: 0x040007CE RID: 1998
		public float baseMaxHealth;

		// Token: 0x040007CF RID: 1999
		public float baseRegen;

		// Token: 0x040007D0 RID: 2000
		public float baseMaxShield;

		// Token: 0x040007D1 RID: 2001
		public float baseMoveSpeed;

		// Token: 0x040007D2 RID: 2002
		public float baseAcceleration;

		// Token: 0x040007D3 RID: 2003
		public float baseJumpPower;

		// Token: 0x040007D4 RID: 2004
		public float baseDamage;

		// Token: 0x040007D5 RID: 2005
		public float baseAttackSpeed;

		// Token: 0x040007D6 RID: 2006
		public float baseCrit;

		// Token: 0x040007D7 RID: 2007
		public float baseArmor;

		// Token: 0x040007D8 RID: 2008
		public int baseJumpCount = 1;

		// Token: 0x040007D9 RID: 2009
		public bool autoCalculateLevelStats;

		// Token: 0x040007DA RID: 2010
		public float levelMaxHealth;

		// Token: 0x040007DB RID: 2011
		public float levelRegen;

		// Token: 0x040007DC RID: 2012
		public float levelMaxShield;

		// Token: 0x040007DD RID: 2013
		public float levelMoveSpeed;

		// Token: 0x040007DE RID: 2014
		public float levelJumpPower;

		// Token: 0x040007DF RID: 2015
		public float levelDamage;

		// Token: 0x040007E0 RID: 2016
		public float levelAttackSpeed;

		// Token: 0x040007E1 RID: 2017
		public float levelCrit;

		// Token: 0x040007E2 RID: 2018
		public float levelArmor;

		// Token: 0x040007F5 RID: 2037
		private bool statsDirty;

		// Token: 0x040007F6 RID: 2038
		private float aimTimer;

		// Token: 0x040007F7 RID: 2039
		private const uint masterDirtyBit = 1U;

		// Token: 0x040007F8 RID: 2040
		private const uint buffsDirtyBit = 2U;

		// Token: 0x040007F9 RID: 2041
		private const uint outOfCombatBit = 4U;

		// Token: 0x040007FA RID: 2042
		private const uint outOfDangerBit = 8U;

		// Token: 0x040007FB RID: 2043
		private const uint sprintingBit = 16U;

		// Token: 0x040007FC RID: 2044
		private const uint allDirtyBits = 31U;

		// Token: 0x040007FD RID: 2045
		private GameObject warCryAuraController;

		// Token: 0x040007FE RID: 2046
		private float warCryTimer;

		// Token: 0x040007FF RID: 2047
		private const float warCryChargeDuration = 30f;

		// Token: 0x04000800 RID: 2048
		private bool _warCryReady;

		// Token: 0x04000801 RID: 2049
		[HideInInspector]
		public int killCount;

		// Token: 0x04000802 RID: 2050
		private float teslaBuffRollTimer;

		// Token: 0x04000803 RID: 2051
		private const float teslaRollInterval = 10f;

		// Token: 0x04000804 RID: 2052
		private float teslaFireTimer;

		// Token: 0x04000805 RID: 2053
		private float teslaResetListTimer;

		// Token: 0x04000806 RID: 2054
		private float teslaResetListInterval = 0.5f;

		// Token: 0x04000807 RID: 2055
		private const float teslaFireInterval = 0.083333336f;

		// Token: 0x04000808 RID: 2056
		private bool teslaCrit;

		// Token: 0x04000809 RID: 2057
		private List<HealthComponent> previousTeslaTargetList = new List<HealthComponent>();

		// Token: 0x0400080A RID: 2058
		private HelfireController helfireController;

		// Token: 0x0400080B RID: 2059
		private float helfireLifetime;

		// Token: 0x0400080C RID: 2060
		private DamageTrail fireTrail;

		// Token: 0x0400080D RID: 2061
		public bool wasLucky;

		// Token: 0x0400080E RID: 2062
		private const float timeBetweenGuardResummons = 30f;

		// Token: 0x0400080F RID: 2063
		private const float timeBetweenGuardRetryResummons = 1f;

		// Token: 0x04000810 RID: 2064
		private float guardResummonCooldown;

		// Token: 0x04000811 RID: 2065
		private const float poisonballAngle = 25f;

		// Token: 0x04000812 RID: 2066
		private const float poisonballDamageCoefficient = 1f;

		// Token: 0x04000813 RID: 2067
		private const float poisonballRefreshTime = 6f;

		// Token: 0x04000814 RID: 2068
		private float poisonballTimer;

		// Token: 0x04000815 RID: 2069
		private GameObject timeBubbleWardInstance;

		// Token: 0x04000816 RID: 2070
		private TemporaryVisualEffect engiShieldTempEffect;

		// Token: 0x04000817 RID: 2071
		private TemporaryVisualEffect bucklerShieldTempEffect;

		// Token: 0x04000818 RID: 2072
		private TemporaryVisualEffect slowDownTimeTempEffect;

		// Token: 0x04000819 RID: 2073
		private TemporaryVisualEffect crippleEffect;

		// Token: 0x0400081A RID: 2074
		private TemporaryVisualEffect tonicBuffEffect;

		// Token: 0x0400081B RID: 2075
		private TemporaryVisualEffect weakTempEffect;

		// Token: 0x0400081C RID: 2076
		private TemporaryVisualEffect energizedTempEffect;

		// Token: 0x0400081D RID: 2077
		private TemporaryVisualEffect barrierTempEffect;

		// Token: 0x0400081E RID: 2078
		private TemporaryVisualEffect nullifyStack1Effect;

		// Token: 0x0400081F RID: 2079
		private TemporaryVisualEffect nullifyStack2Effect;

		// Token: 0x04000820 RID: 2080
		private TemporaryVisualEffect nullifyStack3Effect;

		// Token: 0x04000821 RID: 2081
		private TemporaryVisualEffect regenBoostEffect;

		// Token: 0x04000822 RID: 2082
		private TemporaryVisualEffect nullSafeZoneEffect;

		// Token: 0x04000823 RID: 2083
		private TemporaryVisualEffect elephantDefenseEffect;

		// Token: 0x04000824 RID: 2084
		private TemporaryVisualEffect healingDisabledEffect;

		// Token: 0x04000825 RID: 2085
		private TemporaryVisualEffect noCooldownEffect;

		// Token: 0x04000826 RID: 2086
		[Tooltip("How long it takes for spread bloom to reset from full.")]
		public float spreadBloomDecayTime = 0.45f;

		// Token: 0x04000827 RID: 2087
		[Tooltip("The spread bloom interpretation curve.")]
		public AnimationCurve spreadBloomCurve;

		// Token: 0x04000828 RID: 2088
		private float spreadBloomInternal;

		// Token: 0x04000829 RID: 2089
		[Tooltip("The crosshair prefab used for this body.")]
		public GameObject crosshairPrefab;

		// Token: 0x0400082A RID: 2090
		[HideInInspector]
		public bool hideCrosshair;

		// Token: 0x0400082B RID: 2091
		private const float multiKillMaxInterval = 1f;

		// Token: 0x0400082C RID: 2092
		private float multiKillTimer;

		// Token: 0x0400082E RID: 2094
		private const int multiKillThresholdForWarcry = 4;

		// Token: 0x04000830 RID: 2096
		[Tooltip("The child transform to be used as the aiming origin.")]
		public Transform aimOriginTransform;

		// Token: 0x04000831 RID: 2097
		[Tooltip("The hull size to use when pathfinding for this object.")]
		public HullClassification hullClassification;

		// Token: 0x04000832 RID: 2098
		[Tooltip("The icon displayed for ally healthbars")]
		public Texture portraitIcon;

		// Token: 0x04000833 RID: 2099
		[Tooltip("Whether or not this is a boss for dropping items on death.")]
		[FormerlySerializedAs("isBoss")]
		public bool isChampion;

		// Token: 0x04000835 RID: 2101
		public VehicleSeat currentVehicle;

		// Token: 0x04000836 RID: 2102
		[Tooltip("The pod prefab to use for handling this character's first-time spawn animation.")]
		public GameObject preferredPodPrefab;

		// Token: 0x04000837 RID: 2103
		[Tooltip("The preferred state to use for handling the character's first-time spawn animation. Only used with no preferred pod prefab.")]
		public SerializableEntityStateType preferredInitialStateType = new SerializableEntityStateType(typeof(Uninitialized));

		// Token: 0x04000838 RID: 2104
		public uint skinIndex;

		// Token: 0x0400083A RID: 2106
		private static int kCmdCmdUpdateSprint = -1006016914;

		// Token: 0x0400083B RID: 2107
		private static int kCmdCmdOnSkillActivated;

		// Token: 0x0400083C RID: 2108
		private static int kRpcRpcSyncWarCryReady;

		// Token: 0x0400083D RID: 2109
		private static int kRpcRpcBark;

		// Token: 0x0400083E RID: 2110
		private static int kCmdCmdRequestVehicleEjection;

		// Token: 0x0400083F RID: 2111
		private static int kRpcRpcUsePreferredInitialStateType;

		// Token: 0x02000178 RID: 376
		private static class CommonAssets
		{
			// Token: 0x060007CE RID: 1998 RVA: 0x00022158 File Offset: 0x00020358
			public static void Load()
			{
				CharacterBody.CommonAssets.nullifiedBuffAppliedSound = Resources.Load<NetworkSoundEventDef>("NetworkSoundEventDefs/nseNullifiedBuffApplied");
				CharacterBody.CommonAssets.procCritAttackSpeedSounds = new NetworkSoundEventDef[]
				{
					Resources.Load<NetworkSoundEventDef>("NetworkSoundEventDefs/nseProcCritAttackSpeed1"),
					Resources.Load<NetworkSoundEventDef>("NetworkSoundEventDefs/nseProcCritAttackSpeed2"),
					Resources.Load<NetworkSoundEventDef>("NetworkSoundEventDefs/nseProcCritAttackSpeed3")
				};
				SkillCatalog.skillsDefined.CallWhenAvailable(delegate
				{
					CharacterBody.CommonAssets.lunarUtilityReplacementSkillDef = SkillCatalog.GetSkillDef(SkillCatalog.FindSkillIndexByName("LunarUtilityReplacement"));
					CharacterBody.CommonAssets.lunarPrimaryReplacementSkillDef = SkillCatalog.GetSkillDef(SkillCatalog.FindSkillIndexByName("LunarPrimaryReplacement"));
				});
			}

			// Token: 0x04000840 RID: 2112
			public static SkillDef lunarUtilityReplacementSkillDef;

			// Token: 0x04000841 RID: 2113
			public static SkillDef lunarPrimaryReplacementSkillDef;

			// Token: 0x04000842 RID: 2114
			public static NetworkSoundEventDef nullifiedBuffAppliedSound;

			// Token: 0x04000843 RID: 2115
			public static NetworkSoundEventDef[] procCritAttackSpeedSounds;
		}

		// Token: 0x0200017A RID: 378
		private class TimedBuff
		{
			// Token: 0x04000846 RID: 2118
			public BuffIndex buffIndex;

			// Token: 0x04000847 RID: 2119
			public float timer;
		}

		// Token: 0x0200017B RID: 379
		[Flags]
		public enum BodyFlags : byte
		{
			// Token: 0x04000849 RID: 2121
			None = 0,
			// Token: 0x0400084A RID: 2122
			IgnoreFallDamage = 1,
			// Token: 0x0400084B RID: 2123
			Mechanical = 2,
			// Token: 0x0400084C RID: 2124
			Masterless = 4,
			// Token: 0x0400084D RID: 2125
			ImmuneToGoo = 8,
			// Token: 0x0400084E RID: 2126
			ImmuneToExecutes = 16,
			// Token: 0x0400084F RID: 2127
			SprintAnyDirection = 32,
			// Token: 0x04000850 RID: 2128
			ResistantToAOE = 64
		}

		// Token: 0x0200017C RID: 380
		public class ItemBehavior : MonoBehaviour
		{
			// Token: 0x04000851 RID: 2129
			public CharacterBody body;

			// Token: 0x04000852 RID: 2130
			public int stack;
		}

		// Token: 0x0200017D RID: 381
		public class MushroomItemBehavior : CharacterBody.ItemBehavior
		{
			// Token: 0x060007D4 RID: 2004 RVA: 0x00022208 File Offset: 0x00020408
			private void FixedUpdate()
			{
				if (!NetworkServer.active)
				{
					return;
				}
				int stack = this.stack;
				bool flag = stack > 0 && this.body.GetNotMoving();
				float networkradius = this.body.radius + 1.5f + 1.5f * (float)stack;
				if (this.mushroomWard != flag)
				{
					if (flag)
					{
						this.mushroomWard = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/MushroomWard"), this.body.footPosition, Quaternion.identity);
						this.mushroomWard.GetComponent<TeamFilter>().teamIndex = this.body.teamComponent.teamIndex;
						NetworkServer.Spawn(this.mushroomWard);
					}
					else
					{
						UnityEngine.Object.Destroy(this.mushroomWard);
						this.mushroomWard = null;
					}
				}
				if (this.mushroomWard)
				{
					HealingWard component = this.mushroomWard.GetComponent<HealingWard>();
					component.healFraction = 0.0225f + 0.0225f * (float)stack;
					component.healPoints = 0f;
					component.Networkradius = networkradius;
				}
			}

			// Token: 0x060007D5 RID: 2005 RVA: 0x00022304 File Offset: 0x00020504
			private void OnDisable()
			{
				if (this.mushroomWard)
				{
					UnityEngine.Object.Destroy(this.mushroomWard);
				}
			}

			// Token: 0x04000853 RID: 2131
			private GameObject mushroomWard;
		}

		// Token: 0x0200017E RID: 382
		public class AffixHauntedBehavior : CharacterBody.ItemBehavior
		{
			// Token: 0x060007D7 RID: 2007 RVA: 0x00022328 File Offset: 0x00020528
			private void FixedUpdate()
			{
				if (!NetworkServer.active)
				{
					return;
				}
				bool flag = this.stack > 0;
				if (this.affixHauntedWard != flag)
				{
					if (flag)
					{
						this.affixHauntedWard = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/AffixHauntedWard"));
						this.affixHauntedWard.GetComponent<TeamFilter>().teamIndex = this.body.teamComponent.teamIndex;
						this.affixHauntedWard.GetComponent<BuffWard>().Networkradius = 30f + this.body.radius;
						this.affixHauntedWard.GetComponent<NetworkedBodyAttachment>().AttachToGameObjectAndSpawn(this.body.gameObject);
						return;
					}
					UnityEngine.Object.Destroy(this.affixHauntedWard);
					this.affixHauntedWard = null;
				}
			}

			// Token: 0x060007D8 RID: 2008 RVA: 0x000223DF File Offset: 0x000205DF
			private void OnDisable()
			{
				if (this.affixHauntedWard)
				{
					UnityEngine.Object.Destroy(this.affixHauntedWard);
				}
			}

			// Token: 0x04000854 RID: 2132
			private GameObject affixHauntedWard;
		}

		// Token: 0x0200017F RID: 383
		public class RedWhipBehavior : CharacterBody.ItemBehavior
		{
			// Token: 0x060007DA RID: 2010 RVA: 0x000223FC File Offset: 0x000205FC
			private void SetProvidingBuff(bool shouldProvideBuff)
			{
				if (shouldProvideBuff == this.providingBuff)
				{
					return;
				}
				this.providingBuff = shouldProvideBuff;
				if (this.providingBuff)
				{
					this.body.AddBuff(BuffIndex.WhipBoost);
					EffectData effectData = new EffectData();
					effectData.origin = this.body.corePosition;
					CharacterDirection characterDirection = this.body.characterDirection;
					bool flag = false;
					if (characterDirection && characterDirection.moveVector != Vector3.zero)
					{
						effectData.rotation = Util.QuaternionSafeLookRotation(characterDirection.moveVector);
						flag = true;
					}
					if (!flag)
					{
						effectData.rotation = this.body.transform.rotation;
					}
					EffectManager.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/SprintActivate"), effectData, true);
					return;
				}
				this.body.RemoveBuff(BuffIndex.WhipBoost);
			}

			// Token: 0x060007DB RID: 2011 RVA: 0x000224BC File Offset: 0x000206BC
			private void FixedUpdate()
			{
				this.SetProvidingBuff(this.body.outOfCombat);
			}

			// Token: 0x04000855 RID: 2133
			private bool providingBuff;
		}

		// Token: 0x02000180 RID: 384
		public class IcicleItemBehavior : CharacterBody.ItemBehavior
		{
			// Token: 0x060007DD RID: 2013 RVA: 0x000224D0 File Offset: 0x000206D0
			private void FixedUpdate()
			{
				if (!NetworkServer.active)
				{
					return;
				}
				bool flag = this.stack > 0;
				if (this.icicleAura != flag)
				{
					if (flag)
					{
						GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/IcicleAura"), base.transform.position, Quaternion.identity);
						this.icicleAura = gameObject.GetComponent<IcicleAuraController>();
						this.icicleAura.Networkowner = base.gameObject;
						NetworkServer.Spawn(gameObject);
						return;
					}
					UnityEngine.Object.Destroy(this.icicleAura.gameObject);
					this.icicleAura = null;
				}
			}

			// Token: 0x060007DE RID: 2014 RVA: 0x0002255B File Offset: 0x0002075B
			public void OnOwnerKillOther()
			{
				if (this.icicleAura)
				{
					this.icicleAura.OnOwnerKillOther();
				}
			}

			// Token: 0x060007DF RID: 2015 RVA: 0x00022575 File Offset: 0x00020775
			private void OnDisable()
			{
				if (this.icicleAura)
				{
					UnityEngine.Object.Destroy(this.icicleAura);
				}
			}

			// Token: 0x04000856 RID: 2134
			private IcicleAuraController icicleAura;
		}

		// Token: 0x02000181 RID: 385
		public class HeadstomperItemBehavior : CharacterBody.ItemBehavior
		{
			// Token: 0x060007E1 RID: 2017 RVA: 0x00022590 File Offset: 0x00020790
			private void FixedUpdate()
			{
				bool flag = this.stack > 0;
				if (flag != this.headstompersControllerObject)
				{
					if (flag)
					{
						this.headstompersControllerObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/HeadstompersController"));
						this.headstompersControllerObject.GetComponent<NetworkedBodyAttachment>().AttachToGameObjectAndSpawn(this.body.gameObject);
						return;
					}
					UnityEngine.Object.Destroy(this.headstompersControllerObject);
				}
			}

			// Token: 0x060007E2 RID: 2018 RVA: 0x000225F4 File Offset: 0x000207F4
			private void OnDisable()
			{
				if (this.headstompersControllerObject)
				{
					UnityEngine.Object.Destroy(this.headstompersControllerObject);
				}
			}

			// Token: 0x04000857 RID: 2135
			private GameObject headstompersControllerObject;
		}

		// Token: 0x02000182 RID: 386
		public class SprintWispBehavior : CharacterBody.ItemBehavior
		{
			// Token: 0x060007E4 RID: 2020 RVA: 0x00022610 File Offset: 0x00020810
			private void FixedUpdate()
			{
				if (this.body.isSprinting)
				{
					this.fireTimer -= Time.fixedDeltaTime;
				}
				if (this.fireTimer <= 0f)
				{
					this.fireTimer += 1f / CharacterBody.SprintWispBehavior.fireRate;
					this.Fire();
				}
			}

			// Token: 0x060007E5 RID: 2021 RVA: 0x00022668 File Offset: 0x00020868
			private void Fire()
			{
				DevilOrb devilOrb = new DevilOrb
				{
					origin = this.body.corePosition,
					damageValue = this.body.damage * CharacterBody.SprintWispBehavior.damageCoefficient * (float)this.stack,
					teamIndex = this.body.teamComponent.teamIndex,
					attacker = base.gameObject,
					damageColorIndex = DamageColorIndex.Item,
					scale = 1f,
					effectType = DevilOrb.EffectType.Wisp,
					procCoefficient = 1f
				};
				if (devilOrb.target = devilOrb.PickNextTarget(devilOrb.origin, CharacterBody.SprintWispBehavior.searchRadius))
				{
					devilOrb.isCrit = Util.CheckRoll(this.body.crit, this.body.master);
					OrbManager.instance.AddOrb(devilOrb);
				}
			}

			// Token: 0x04000858 RID: 2136
			private static readonly float fireRate = 2f;

			// Token: 0x04000859 RID: 2137
			private static readonly float searchRadius = 30f;

			// Token: 0x0400085A RID: 2138
			private static readonly float damageCoefficient = 1f;

			// Token: 0x0400085B RID: 2139
			private float fireTimer;
		}

		// Token: 0x02000183 RID: 387
		public class QuestVolatileBatteryBehaviorServer : CharacterBody.ItemBehavior
		{
			// Token: 0x060007E8 RID: 2024 RVA: 0x0002275E File Offset: 0x0002095E
			private void Start()
			{
				this.attachment = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/QuestVolatileBatteryAttachment")).GetComponent<NetworkedBodyAttachment>();
				this.attachment.AttachToGameObjectAndSpawn(this.body.gameObject);
			}

			// Token: 0x060007E9 RID: 2025 RVA: 0x00022790 File Offset: 0x00020990
			private void OnDestroy()
			{
				if (this.attachment)
				{
					UnityEngine.Object.Destroy(this.attachment.gameObject);
					this.attachment = null;
				}
			}

			// Token: 0x0400085C RID: 2140
			private NetworkedBodyAttachment attachment;
		}

		// Token: 0x02000184 RID: 388
		public class LaserTurbineBehavior : CharacterBody.ItemBehavior
		{
			// Token: 0x060007EB RID: 2027 RVA: 0x000227B8 File Offset: 0x000209B8
			private void Start()
			{
				if (NetworkServer.active)
				{
					this.laserTurbineControllerInstance = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/LaserTurbineController"), this.body.corePosition, Quaternion.identity);
					this.laserTurbineControllerInstance.GetComponent<GenericOwnership>().ownerObject = base.gameObject;
					this.laserTurbineControllerInstance.GetComponent<NetworkedBodyAttachment>().AttachToGameObjectAndSpawn(base.gameObject);
				}
			}

			// Token: 0x060007EC RID: 2028 RVA: 0x0002281D File Offset: 0x00020A1D
			private void OnDestroy()
			{
				if (this.laserTurbineControllerInstance)
				{
					UnityEngine.Object.Destroy(this.laserTurbineControllerInstance);
				}
			}

			// Token: 0x0400085D RID: 2141
			private GameObject laserTurbineControllerInstance;
		}

		// Token: 0x02000185 RID: 389
		public class NovaOnLowHealthItemBehaviorServer : CharacterBody.ItemBehavior
		{
			// Token: 0x060007EE RID: 2030 RVA: 0x00022837 File Offset: 0x00020A37
			private void Start()
			{
				this.attachment = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/BodyAttachments/VagrantNovaItemBodyAttachment")).GetComponent<NetworkedBodyAttachment>();
				this.attachment.AttachToGameObjectAndSpawn(this.body.gameObject);
			}

			// Token: 0x060007EF RID: 2031 RVA: 0x00022869 File Offset: 0x00020A69
			private void FixedUpdate()
			{
				if (!this.body.healthComponent.alive)
				{
					UnityEngine.Object.Destroy(this);
				}
			}

			// Token: 0x060007F0 RID: 2032 RVA: 0x00022883 File Offset: 0x00020A83
			private void OnDestroy()
			{
				if (this.attachment)
				{
					UnityEngine.Object.Destroy(this.attachment.gameObject);
					this.attachment = null;
				}
			}

			// Token: 0x0400085E RID: 2142
			private NetworkedBodyAttachment attachment;
		}

		// Token: 0x02000186 RID: 390
		public class TimeBubbleItemBehaviorServer : CharacterBody.ItemBehavior
		{
			// Token: 0x060007F2 RID: 2034 RVA: 0x000228A9 File Offset: 0x00020AA9
			private void OnDestroy()
			{
				if (this.body.timeBubbleWardInstance)
				{
					UnityEngine.Object.Destroy(this.body.timeBubbleWardInstance);
				}
			}
		}

		// Token: 0x02000187 RID: 391
		private class ConstructTurretMessage : MessageBase
		{
			// Token: 0x060007F5 RID: 2037 RVA: 0x000228CD File Offset: 0x00020ACD
			public override void Serialize(NetworkWriter writer)
			{
				writer.Write(this.builder);
				writer.Write(this.position);
				writer.Write(this.rotation);
				GeneratedNetworkCode._WriteNetworkMasterIndex_MasterCatalog(writer, this.turretMasterIndex);
			}

			// Token: 0x060007F6 RID: 2038 RVA: 0x000228FF File Offset: 0x00020AFF
			public override void Deserialize(NetworkReader reader)
			{
				this.builder = reader.ReadGameObject();
				this.position = reader.ReadVector3();
				this.rotation = reader.ReadQuaternion();
				this.turretMasterIndex = GeneratedNetworkCode._ReadNetworkMasterIndex_MasterCatalog(reader);
			}

			// Token: 0x0400085F RID: 2143
			public GameObject builder;

			// Token: 0x04000860 RID: 2144
			public Vector3 position;

			// Token: 0x04000861 RID: 2145
			public Quaternion rotation;

			// Token: 0x04000862 RID: 2146
			public MasterCatalog.NetworkMasterIndex turretMasterIndex;
		}
	}
}
