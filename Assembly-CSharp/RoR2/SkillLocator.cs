using System;
using RoR2.Networking;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;

namespace RoR2
{
	// Token: 0x02000337 RID: 823
	[RequireComponent(typeof(NetworkIdentity))]
	public class SkillLocator : NetworkBehaviour
	{
		// Token: 0x17000257 RID: 599
		// (get) Token: 0x06001395 RID: 5013 RVA: 0x00053AAC File Offset: 0x00051CAC
		public int skillSlotCount
		{
			get
			{
				return this.allSkills.Length;
			}
		}

		// Token: 0x06001396 RID: 5014 RVA: 0x00053AB6 File Offset: 0x00051CB6
		private void Awake()
		{
			this.networkIdentity = base.GetComponent<NetworkIdentity>();
			this.allSkills = base.GetComponents<GenericSkill>();
		}

		// Token: 0x06001397 RID: 5015 RVA: 0x00053AD0 File Offset: 0x00051CD0
		public override void OnStartAuthority()
		{
			base.OnStartAuthority();
			this.UpdateAuthority();
		}

		// Token: 0x06001398 RID: 5016 RVA: 0x00053ADE File Offset: 0x00051CDE
		public override void OnStopAuthority()
		{
			base.OnStopAuthority();
			this.UpdateAuthority();
		}

		// Token: 0x06001399 RID: 5017 RVA: 0x00053AEC File Offset: 0x00051CEC
		private void UpdateAuthority()
		{
			this.hasEffectiveAuthority = Util.HasEffectiveAuthority(this.networkIdentity);
		}

		// Token: 0x0600139A RID: 5018 RVA: 0x00053B00 File Offset: 0x00051D00
		public override bool OnSerialize(NetworkWriter writer, bool initialState)
		{
			uint num = base.syncVarDirtyBits;
			if (initialState)
			{
				for (int i = 0; i < this.allSkills.Length; i++)
				{
					GenericSkill genericSkill = this.allSkills[i];
					if (genericSkill.baseSkill != genericSkill.defaultSkillDef)
					{
						num |= 1U << i;
					}
				}
			}
			writer.WritePackedUInt32(num);
			for (int j = 0; j < this.allSkills.Length; j++)
			{
				if ((num & 1U << j) != 0U)
				{
					GenericSkill genericSkill2 = this.allSkills[j];
					SkillDef baseSkill = genericSkill2.baseSkill;
					writer.WritePackedUInt32((uint)(((baseSkill != null) ? baseSkill.skillIndex : -1) + 1));
				}
			}
			return num > 0U;
		}

		// Token: 0x0600139B RID: 5019 RVA: 0x00053B9C File Offset: 0x00051D9C
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			this.inDeserialize = true;
			uint num = reader.ReadPackedUInt32();
			for (int i = 0; i < this.allSkills.Length; i++)
			{
				if ((num & 1U << i) != 0U)
				{
					GenericSkill genericSkill = this.allSkills[i];
					SkillDef skillDef = SkillCatalog.GetSkillDef((int)(reader.ReadPackedUInt32() - 1U));
					if (initialState || !this.hasEffectiveAuthority)
					{
						genericSkill.SetBaseSkill(skillDef);
					}
				}
			}
			this.inDeserialize = false;
		}

		// Token: 0x0600139C RID: 5020 RVA: 0x00053C04 File Offset: 0x00051E04
		public GenericSkill FindSkill(string skillName)
		{
			for (int i = 0; i < this.allSkills.Length; i++)
			{
				if (this.allSkills[i].skillName == skillName)
				{
					return this.allSkills[i];
				}
			}
			return null;
		}

		// Token: 0x0600139D RID: 5021 RVA: 0x00053C44 File Offset: 0x00051E44
		public GenericSkill FindSkillByFamilyName(string skillFamilyName)
		{
			for (int i = 0; i < this.allSkills.Length; i++)
			{
				if (SkillCatalog.GetSkillFamilyName(this.allSkills[i].skillFamily.catalogIndex) == skillFamilyName)
				{
					return this.allSkills[i];
				}
			}
			return null;
		}

		// Token: 0x0600139E RID: 5022 RVA: 0x00053C8D File Offset: 0x00051E8D
		public GenericSkill GetSkill(SkillSlot skillSlot)
		{
			switch (skillSlot)
			{
			case SkillSlot.Primary:
				return this.primary;
			case SkillSlot.Secondary:
				return this.secondary;
			case SkillSlot.Utility:
				return this.utility;
			case SkillSlot.Special:
				return this.special;
			default:
				return null;
			}
		}

		// Token: 0x0600139F RID: 5023 RVA: 0x00053CC4 File Offset: 0x00051EC4
		public GenericSkill GetSkillAtIndex(int index)
		{
			return HGArrayUtilities.GetSafe<GenericSkill>(this.allSkills, index);
		}

		// Token: 0x060013A0 RID: 5024 RVA: 0x00053CD2 File Offset: 0x00051ED2
		public int GetSkillSlotIndex(GenericSkill skillSlot)
		{
			return Array.IndexOf<GenericSkill>(this.allSkills, skillSlot);
		}

		// Token: 0x060013A1 RID: 5025 RVA: 0x00053CE0 File Offset: 0x00051EE0
		public SkillSlot FindSkillSlot(GenericSkill skillComponent)
		{
			if (!skillComponent)
			{
				return SkillSlot.None;
			}
			if (skillComponent == this.primary)
			{
				return SkillSlot.Primary;
			}
			if (skillComponent == this.secondary)
			{
				return SkillSlot.Secondary;
			}
			if (skillComponent == this.utility)
			{
				return SkillSlot.Utility;
			}
			if (skillComponent == this.special)
			{
				return SkillSlot.Special;
			}
			return SkillSlot.None;
		}

		// Token: 0x060013A2 RID: 5026 RVA: 0x00053D38 File Offset: 0x00051F38
		[Server]
		public void ApplyLoadoutServer(Loadout loadout, int bodyIndex)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.SkillLocator::ApplyLoadoutServer(RoR2.Loadout,System.Int32)' called on client");
				return;
			}
			if (bodyIndex == -1)
			{
				return;
			}
			for (int i = 0; i < this.allSkills.Length; i++)
			{
				uint skillVariant = loadout.bodyLoadoutManager.GetSkillVariant(bodyIndex, i);
				GenericSkill genericSkill = this.allSkills[i];
				genericSkill.SetBaseSkill(genericSkill.skillFamily.variants[(int)skillVariant].skillDef);
			}
		}

		// Token: 0x060013A3 RID: 5027 RVA: 0x00053DA4 File Offset: 0x00051FA4
		public void ResetSkills()
		{
			if (NetworkServer.active && this.networkIdentity.clientAuthorityOwner != null)
			{
				NetworkWriter networkWriter = new NetworkWriter();
				networkWriter.StartMessage(56);
				networkWriter.Write(base.gameObject);
				networkWriter.FinishMessage();
				this.networkIdentity.clientAuthorityOwner.SendWriter(networkWriter, QosChannelIndex.defaultReliable.intVal);
			}
			for (int i = 0; i < this.allSkills.Length; i++)
			{
				this.allSkills[i].Reset();
			}
		}

		// Token: 0x060013A4 RID: 5028 RVA: 0x00053E24 File Offset: 0x00052024
		public void ApplyAmmoPack()
		{
			if (NetworkServer.active && !this.networkIdentity.hasAuthority)
			{
				NetworkWriter networkWriter = new NetworkWriter();
				networkWriter.StartMessage(63);
				networkWriter.Write(base.gameObject);
				networkWriter.FinishMessage();
				NetworkConnection clientAuthorityOwner = this.networkIdentity.clientAuthorityOwner;
				if (clientAuthorityOwner != null)
				{
					clientAuthorityOwner.SendWriter(networkWriter, QosChannelIndex.defaultReliable.intVal);
					return;
				}
			}
			else
			{
				GenericSkill[] array = new GenericSkill[]
				{
					this.primary,
					this.secondary,
					this.utility,
					this.special
				};
				Util.ShuffleArray<GenericSkill>(array);
				foreach (GenericSkill genericSkill in array)
				{
					if (genericSkill && genericSkill.CanApplyAmmoPack())
					{
						Debug.LogFormat("Resetting skill {0}", new object[]
						{
							genericSkill.skillName
						});
						genericSkill.AddOneStock();
					}
				}
			}
		}

		// Token: 0x060013A5 RID: 5029 RVA: 0x00053F04 File Offset: 0x00052104
		[NetworkMessageHandler(msgType = 56, client = true)]
		private static void HandleResetSkills(NetworkMessage netMsg)
		{
			GameObject gameObject = netMsg.reader.ReadGameObject();
			if (!NetworkServer.active && gameObject)
			{
				SkillLocator component = gameObject.GetComponent<SkillLocator>();
				if (component)
				{
					component.ResetSkills();
				}
			}
		}

		// Token: 0x060013A6 RID: 5030 RVA: 0x00053F44 File Offset: 0x00052144
		[NetworkMessageHandler(msgType = 63, client = true)]
		private static void HandleAmmoPackPickup(NetworkMessage netMsg)
		{
			GameObject gameObject = netMsg.reader.ReadGameObject();
			if (!NetworkServer.active && gameObject)
			{
				SkillLocator component = gameObject.GetComponent<SkillLocator>();
				if (component)
				{
					component.ApplyAmmoPack();
				}
			}
		}

		// Token: 0x060013A8 RID: 5032 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x04001264 RID: 4708
		[FormerlySerializedAs("skill1")]
		public GenericSkill primary;

		// Token: 0x04001265 RID: 4709
		[FormerlySerializedAs("skill2")]
		public GenericSkill secondary;

		// Token: 0x04001266 RID: 4710
		[FormerlySerializedAs("skill3")]
		public GenericSkill utility;

		// Token: 0x04001267 RID: 4711
		[FormerlySerializedAs("skill4")]
		public GenericSkill special;

		// Token: 0x04001268 RID: 4712
		public SkillLocator.PassiveSkill passiveSkill;

		// Token: 0x04001269 RID: 4713
		private NetworkIdentity networkIdentity;

		// Token: 0x0400126A RID: 4714
		private GenericSkill[] allSkills;

		// Token: 0x0400126B RID: 4715
		private bool hasEffectiveAuthority;

		// Token: 0x0400126C RID: 4716
		private uint skillDefDirtyFlags;

		// Token: 0x0400126D RID: 4717
		private bool inDeserialize;

		// Token: 0x02000338 RID: 824
		[Serializable]
		public struct PassiveSkill
		{
			// Token: 0x0400126E RID: 4718
			public bool enabled;

			// Token: 0x0400126F RID: 4719
			public string skillNameToken;

			// Token: 0x04001270 RID: 4720
			public string skillDescriptionToken;

			// Token: 0x04001271 RID: 4721
			public Sprite icon;
		}
	}
}
