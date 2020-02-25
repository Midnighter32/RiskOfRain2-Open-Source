using System;
using System.Collections.ObjectModel;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.TeleporterHealNovaController
{
	// Token: 0x02000778 RID: 1912
	public class TeleporterHealNovaGeneratorMain : BaseState
	{
		// Token: 0x06002BFF RID: 11263 RVA: 0x000B9ED4 File Offset: 0x000B80D4
		public override void OnEnter()
		{
			base.OnEnter();
			Transform parent = base.transform.parent;
			if (parent)
			{
				this.teleporter = parent.GetComponent<TeleporterInteraction>();
				this.previousPulseFraction = this.GetCurrentTeleporterChargeFraction();
			}
			TeamFilter component = base.GetComponent<TeamFilter>();
			this.teamIndex = (component ? component.teamIndex : TeamIndex.None);
		}

		// Token: 0x06002C00 RID: 11264 RVA: 0x000B9F31 File Offset: 0x000B8131
		private float GetCurrentTeleporterChargeFraction()
		{
			return Mathf.Min(1f - (this.teleporter ? (this.teleporter.remainingChargeTimer / this.teleporter.chargeDuration) : 0f), 1f);
		}

		// Token: 0x06002C01 RID: 11265 RVA: 0x000B9F70 File Offset: 0x000B8170
		public override void FixedUpdate()
		{
			if (NetworkServer.active && Time.fixedDeltaTime > 0f)
			{
				if (!this.teleporter || this.teleporter.isCharged)
				{
					EntityState.Destroy(this.outer.gameObject);
					return;
				}
				this.pulseCount = TeleporterHealNovaGeneratorMain.CalculatePulseCount(this.teamIndex);
				float num = TeleporterHealNovaGeneratorMain.CalculateNextPulseFraction(this.pulseCount, this.previousPulseFraction);
				float currentTeleporterChargeFraction = this.GetCurrentTeleporterChargeFraction();
				if (num < currentTeleporterChargeFraction)
				{
					this.Pulse();
					this.previousPulseFraction = num;
				}
			}
		}

		// Token: 0x06002C02 RID: 11266 RVA: 0x000B9FF8 File Offset: 0x000B81F8
		private static int CalculatePulseCount(TeamIndex teamIndex)
		{
			int num = 0;
			ReadOnlyCollection<CharacterMaster> readOnlyInstancesList = CharacterMaster.readOnlyInstancesList;
			for (int i = 0; i < readOnlyInstancesList.Count; i++)
			{
				CharacterMaster characterMaster = readOnlyInstancesList[i];
				if (characterMaster.teamIndex == teamIndex)
				{
					num += characterMaster.inventory.GetItemCount(ItemIndex.TPHealingNova);
				}
			}
			return num;
		}

		// Token: 0x06002C03 RID: 11267 RVA: 0x000BA040 File Offset: 0x000B8240
		private static float CalculateNextPulseFraction(int pulseCount, float previousPulseFraction)
		{
			float num = 1f / (float)(pulseCount + 1);
			for (int i = 1; i <= pulseCount; i++)
			{
				float num2 = (float)i * num;
				if (num2 > previousPulseFraction)
				{
					return num2;
				}
			}
			return 1f;
		}

		// Token: 0x06002C04 RID: 11268 RVA: 0x000BA074 File Offset: 0x000B8274
		protected void Pulse()
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(TeleporterHealNovaGeneratorMain.pulsePrefab, base.transform.position, base.transform.rotation, base.transform.parent);
			gameObject.GetComponent<TeamFilter>().teamIndex = this.teamIndex;
			NetworkServer.Spawn(gameObject);
		}

		// Token: 0x0400281E RID: 10270
		public static GameObject pulsePrefab;

		// Token: 0x0400281F RID: 10271
		private TeleporterInteraction teleporter;

		// Token: 0x04002820 RID: 10272
		private TeamIndex teamIndex;

		// Token: 0x04002821 RID: 10273
		private NetworkParent networkParent;

		// Token: 0x04002822 RID: 10274
		private float previousPulseFraction;

		// Token: 0x04002823 RID: 10275
		private int pulseCount;
	}
}
