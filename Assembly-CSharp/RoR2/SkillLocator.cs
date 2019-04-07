using System;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;

namespace RoR2
{
	// Token: 0x020003E3 RID: 995
	[RequireComponent(typeof(NetworkIdentity))]
	public class SkillLocator : MonoBehaviour
	{
		// Token: 0x060015B2 RID: 5554 RVA: 0x00067EB8 File Offset: 0x000660B8
		private void Awake()
		{
			this.networkIdentity = base.GetComponent<NetworkIdentity>();
			this.allSkills = base.GetComponents<GenericSkill>();
		}

		// Token: 0x060015B3 RID: 5555 RVA: 0x00067ED4 File Offset: 0x000660D4
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

		// Token: 0x060015B4 RID: 5556 RVA: 0x00067F13 File Offset: 0x00066113
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

		// Token: 0x060015B5 RID: 5557 RVA: 0x00067F4C File Offset: 0x0006614C
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

		// Token: 0x060015B6 RID: 5558 RVA: 0x00067FA4 File Offset: 0x000661A4
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

		// Token: 0x060015B7 RID: 5559 RVA: 0x00068024 File Offset: 0x00066224
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

		// Token: 0x060015B8 RID: 5560 RVA: 0x00068104 File Offset: 0x00066304
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

		// Token: 0x060015B9 RID: 5561 RVA: 0x00068144 File Offset: 0x00066344
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

		// Token: 0x04001911 RID: 6417
		[FormerlySerializedAs("skill1")]
		public GenericSkill primary;

		// Token: 0x04001912 RID: 6418
		[FormerlySerializedAs("skill2")]
		public GenericSkill secondary;

		// Token: 0x04001913 RID: 6419
		[FormerlySerializedAs("skill3")]
		public GenericSkill utility;

		// Token: 0x04001914 RID: 6420
		[FormerlySerializedAs("skill4")]
		public GenericSkill special;

		// Token: 0x04001915 RID: 6421
		public SkillLocator.PassiveSkill passiveSkill;

		// Token: 0x04001916 RID: 6422
		private NetworkIdentity networkIdentity;

		// Token: 0x04001917 RID: 6423
		private GenericSkill[] allSkills;

		// Token: 0x020003E4 RID: 996
		[Serializable]
		public struct PassiveSkill
		{
			// Token: 0x04001918 RID: 6424
			public bool enabled;

			// Token: 0x04001919 RID: 6425
			public string skillNameToken;

			// Token: 0x0400191A RID: 6426
			public string skillDescriptionToken;

			// Token: 0x0400191B RID: 6427
			public Sprite icon;
		}
	}
}
