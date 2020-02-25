using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005CD RID: 1485
	public class HUDBossHealthBarController : MonoBehaviour
	{
		// Token: 0x06002322 RID: 8994 RVA: 0x00099A50 File Offset: 0x00097C50
		private void FixedUpdate()
		{
			List<BossGroup> instancesList = InstanceTracker.GetInstancesList<BossGroup>();
			int num = 0;
			for (int i = 0; i < instancesList.Count; i++)
			{
				if (instancesList[i].shouldDisplayHealthBarOnHud)
				{
					num++;
				}
			}
			this.SetListeningForClientDamageNotified(num > 1);
			if (num == 1)
			{
				for (int j = 0; j < instancesList.Count; j++)
				{
					if (instancesList[j].shouldDisplayHealthBarOnHud)
					{
						this.currentBossGroup = instancesList[j];
						return;
					}
				}
				return;
			}
			if (instancesList.Count == 0)
			{
				this.currentBossGroup = null;
			}
		}

		// Token: 0x06002323 RID: 8995 RVA: 0x00099AD3 File Offset: 0x00097CD3
		private void OnDisable()
		{
			this.SetListeningForClientDamageNotified(false);
		}

		// Token: 0x06002324 RID: 8996 RVA: 0x00099ADC File Offset: 0x00097CDC
		private void OnClientDamageNotified(DamageDealtMessage damageDealtMessage)
		{
			if (!this.nextAllowedSourceUpdateTime.hasPassed)
			{
				return;
			}
			if (!damageDealtMessage.victim)
			{
				return;
			}
			CharacterBody component = damageDealtMessage.victim.GetComponent<CharacterBody>();
			if (!component)
			{
				return;
			}
			if (component.isBoss && damageDealtMessage.attacker == this.hud.targetBodyObject)
			{
				BossGroup bossGroup = BossGroup.FindBossGroup(component);
				if (bossGroup && bossGroup.shouldDisplayHealthBarOnHud)
				{
					this.currentBossGroup = bossGroup;
					this.nextAllowedSourceUpdateTime = Run.TimeStamp.now + 1f;
				}
			}
		}

		// Token: 0x06002325 RID: 8997 RVA: 0x00099B68 File Offset: 0x00097D68
		private void SetListeningForClientDamageNotified(bool newListeningForClientDamageNotified)
		{
			if (newListeningForClientDamageNotified == this.listeningForClientDamageNotified)
			{
				return;
			}
			this.listeningForClientDamageNotified = newListeningForClientDamageNotified;
			if (this.listeningForClientDamageNotified)
			{
				GlobalEventManager.onClientDamageNotified += this.OnClientDamageNotified;
				return;
			}
			GlobalEventManager.onClientDamageNotified -= this.OnClientDamageNotified;
		}

		// Token: 0x06002326 RID: 8998 RVA: 0x00099BA8 File Offset: 0x00097DA8
		private void LateUpdate()
		{
			bool flag = this.currentBossGroup && this.currentBossGroup.totalObservedHealth > 0f;
			this.container.SetActive(flag);
			if (flag)
			{
				float totalObservedHealth = this.currentBossGroup.totalObservedHealth;
				float totalMaxObservedMaxHealth = this.currentBossGroup.totalMaxObservedMaxHealth;
				float num = (totalMaxObservedMaxHealth == 0f) ? 0f : Mathf.Clamp01(totalObservedHealth / totalMaxObservedMaxHealth);
				this.delayedTotalHealthFraction = Mathf.Clamp(Mathf.SmoothDamp(this.delayedTotalHealthFraction, num, ref this.healthFractionVelocity, 0.1f, float.PositiveInfinity, Time.deltaTime), num, 1f);
				this.fillRectImage.fillAmount = num;
				this.delayRectImage.fillAmount = this.delayedTotalHealthFraction;
				HUDBossHealthBarController.sharedStringBuilder.Clear().AppendInt(Mathf.FloorToInt(totalObservedHealth), 0U, uint.MaxValue).Append("/").AppendInt(Mathf.FloorToInt(totalMaxObservedMaxHealth), 0U, uint.MaxValue);
				this.healthLabel.SetText(HUDBossHealthBarController.sharedStringBuilder);
				this.bossNameLabel.SetText(this.currentBossGroup.bestObservedName);
				this.bossSubtitleLabel.SetText(this.currentBossGroup.bestObservedSubtitle);
			}
		}

		// Token: 0x04002108 RID: 8456
		public HUD hud;

		// Token: 0x04002109 RID: 8457
		public GameObject container;

		// Token: 0x0400210A RID: 8458
		public Image fillRectImage;

		// Token: 0x0400210B RID: 8459
		public Image delayRectImage;

		// Token: 0x0400210C RID: 8460
		public TextMeshProUGUI healthLabel;

		// Token: 0x0400210D RID: 8461
		public TextMeshProUGUI bossNameLabel;

		// Token: 0x0400210E RID: 8462
		public TextMeshProUGUI bossSubtitleLabel;

		// Token: 0x0400210F RID: 8463
		private BossGroup currentBossGroup;

		// Token: 0x04002110 RID: 8464
		private float delayedTotalHealthFraction;

		// Token: 0x04002111 RID: 8465
		private float healthFractionVelocity;

		// Token: 0x04002112 RID: 8466
		private static readonly StringBuilder sharedStringBuilder = new StringBuilder();

		// Token: 0x04002113 RID: 8467
		private Run.TimeStamp nextAllowedSourceUpdateTime = Run.TimeStamp.negativeInfinity;

		// Token: 0x04002114 RID: 8468
		private bool listeningForClientDamageNotified;
	}
}
