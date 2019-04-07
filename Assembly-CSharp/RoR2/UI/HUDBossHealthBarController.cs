using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005E8 RID: 1512
	public class HUDBossHealthBarController : MonoBehaviour
	{
		// Token: 0x060021E1 RID: 8673 RVA: 0x000A05BC File Offset: 0x0009E7BC
		private void LateUpdate()
		{
			this.container.SetActive(HUDBossHealthBarController.shouldBeActive);
			if (HUDBossHealthBarController.shouldBeActive)
			{
				this.fillRectImage.fillAmount = HUDBossHealthBarController.totalHealthFraction;
				this.delayRectImage.fillAmount = HUDBossHealthBarController.delayedTotalHealthFraction;
				this.healthLabel.text = HUDBossHealthBarController.healthString;
				this.bossNameLabel.text = HUDBossHealthBarController.bossNameString;
				this.bossSubtitleLabel.text = HUDBossHealthBarController.bossSubtitleResolvedString;
			}
		}

		// Token: 0x060021E2 RID: 8674 RVA: 0x000A0630 File Offset: 0x0009E830
		private static HUDBossHealthBarController.BossMemory GetBossMemory(CharacterMaster bossMaster)
		{
			if (!bossMaster)
			{
				return null;
			}
			for (int i = 0; i < HUDBossHealthBarController.bossMemoryList.Count; i++)
			{
				if (HUDBossHealthBarController.bossMemoryList[i].master == bossMaster)
				{
					return HUDBossHealthBarController.bossMemoryList[i];
				}
			}
			HUDBossHealthBarController.BossMemory bossMemory = new HUDBossHealthBarController.BossMemory
			{
				master = bossMaster
			};
			HUDBossHealthBarController.bossMemoryList.Add(bossMemory);
			if (HUDBossHealthBarController.bossMemoryList.Count == 1)
			{
				HUDBossHealthBarController.bossNameString = Language.GetString(bossMaster.bodyPrefab.GetComponent<CharacterBody>().baseNameToken);
				string text = bossMaster.bodyPrefab.GetComponent<CharacterBody>().GetSubtitle();
				if (text.Length == 0)
				{
					text = Language.GetString("NULL_SUBTITLE");
				}
				HUDBossHealthBarController.bossSubtitleResolvedString = "<sprite name=\"CloudLeft\" tint=1> " + text + "<sprite name=\"CloudRight\" tint=1>";
				EliteIndex eliteIndex = EliteCatalog.IsEquipmentElite(bossMaster.inventory.currentEquipmentIndex);
				if (eliteIndex != EliteIndex.None)
				{
					HUDBossHealthBarController.bossNameString = EliteCatalog.GetEliteDef(eliteIndex).prefix + HUDBossHealthBarController.bossNameString;
				}
			}
			return bossMemory;
		}

		// Token: 0x060021E3 RID: 8675 RVA: 0x000A072C File Offset: 0x0009E92C
		private static HealthComponent GetCharacterHealthComponent(CharacterMaster master)
		{
			if (master)
			{
				GameObject bodyObject = master.GetBodyObject();
				if (bodyObject)
				{
					return bodyObject.GetComponent<HealthComponent>();
				}
			}
			return null;
		}

		// Token: 0x060021E4 RID: 8676 RVA: 0x000A0758 File Offset: 0x0009E958
		private static void Recalculate()
		{
			HUDBossHealthBarController.totalBossHealth = 0f;
			HUDBossHealthBarController.totalMaxBossHealth = 0f;
			if (BossGroup.instance)
			{
				ReadOnlyCollection<CharacterMaster> readOnlyMembersList = BossGroup.instance.readOnlyMembersList;
				for (int i = 0; i < readOnlyMembersList.Count; i++)
				{
					HUDBossHealthBarController.GetBossMemory(readOnlyMembersList[i]);
				}
			}
			for (int j = 0; j < HUDBossHealthBarController.bossMemoryList.Count; j++)
			{
				HUDBossHealthBarController.BossMemory bossMemory = HUDBossHealthBarController.bossMemoryList[j];
				bossMemory.UpdateLastKnownHealth();
				HUDBossHealthBarController.totalBossHealth += bossMemory.lastKnownHealth;
				HUDBossHealthBarController.totalMaxBossHealth += bossMemory.lastKnownMaxHealth;
			}
			HUDBossHealthBarController.shouldBeActive = (HUDBossHealthBarController.totalBossHealth != 0f);
			if (HUDBossHealthBarController.shouldBeActive)
			{
				HUDBossHealthBarController.totalHealthFraction = ((HUDBossHealthBarController.totalMaxBossHealth == 0f) ? 0f : Mathf.Clamp01(HUDBossHealthBarController.totalBossHealth / HUDBossHealthBarController.totalMaxBossHealth));
				HUDBossHealthBarController.delayedTotalHealthFraction = Mathf.SmoothDamp(HUDBossHealthBarController.delayedTotalHealthFraction, HUDBossHealthBarController.totalHealthFraction, ref HUDBossHealthBarController.healthFractionVelocity, 0.1f);
				HUDBossHealthBarController.healthString = Mathf.FloorToInt(HUDBossHealthBarController.totalBossHealth) + "/" + Mathf.FloorToInt(HUDBossHealthBarController.totalMaxBossHealth);
				return;
			}
			HUDBossHealthBarController.bossMemoryList.Clear();
		}

		// Token: 0x060021E5 RID: 8677 RVA: 0x000A0890 File Offset: 0x0009EA90
		private void OnEnable()
		{
			if (HUDBossHealthBarController.enabledCount++ == 0)
			{
				RoR2Application.onUpdate += HUDBossHealthBarController.Recalculate;
			}
		}

		// Token: 0x060021E6 RID: 8678 RVA: 0x000A08B2 File Offset: 0x0009EAB2
		private void OnDisable()
		{
			if (--HUDBossHealthBarController.enabledCount == 0)
			{
				RoR2Application.onUpdate -= HUDBossHealthBarController.Recalculate;
			}
		}

		// Token: 0x040024E2 RID: 9442
		public GameObject container;

		// Token: 0x040024E3 RID: 9443
		public Image fillRectImage;

		// Token: 0x040024E4 RID: 9444
		public Image delayRectImage;

		// Token: 0x040024E5 RID: 9445
		public TextMeshProUGUI healthLabel;

		// Token: 0x040024E6 RID: 9446
		public TextMeshProUGUI bossNameLabel;

		// Token: 0x040024E7 RID: 9447
		public TextMeshProUGUI bossSubtitleLabel;

		// Token: 0x040024E8 RID: 9448
		private static List<HUDBossHealthBarController.BossMemory> bossMemoryList = new List<HUDBossHealthBarController.BossMemory>();

		// Token: 0x040024E9 RID: 9449
		private static bool shouldBeActive = false;

		// Token: 0x040024EA RID: 9450
		private static float totalBossHealth = 0f;

		// Token: 0x040024EB RID: 9451
		private static float totalMaxBossHealth = 0f;

		// Token: 0x040024EC RID: 9452
		private static float totalHealthFraction;

		// Token: 0x040024ED RID: 9453
		private static float delayedTotalHealthFraction;

		// Token: 0x040024EE RID: 9454
		private static string healthString = "";

		// Token: 0x040024EF RID: 9455
		private static string bossNameString = "";

		// Token: 0x040024F0 RID: 9456
		private static string bossSubtitleResolvedString = "";

		// Token: 0x040024F1 RID: 9457
		private static float healthFractionVelocity = 0f;

		// Token: 0x040024F2 RID: 9458
		private static int enabledCount = 0;

		// Token: 0x020005E9 RID: 1513
		private class BossMemory
		{
			// Token: 0x170002F6 RID: 758
			// (get) Token: 0x060021E9 RID: 8681 RVA: 0x000A0934 File Offset: 0x0009EB34
			public HealthComponent healthComponent
			{
				get
				{
					if (!this.foundBodyObject && this.master)
					{
						GameObject bodyObject = this.master.GetBodyObject();
						if (bodyObject)
						{
							this._healthComponent = bodyObject.GetComponent<HealthComponent>();
							this.foundBodyObject = true;
						}
					}
					return this._healthComponent;
				}
			}

			// Token: 0x060021EA RID: 8682 RVA: 0x000A0984 File Offset: 0x0009EB84
			public void UpdateLastKnownHealth()
			{
				HealthComponent healthComponent = this.healthComponent;
				if (healthComponent)
				{
					this.lastKnownHealth = Mathf.Max(healthComponent.health + healthComponent.shield, 0f);
					this.lastKnownMaxHealth = healthComponent.fullHealth + healthComponent.fullShield;
					return;
				}
				this.lastKnownHealth = 0f;
			}

			// Token: 0x040024F3 RID: 9459
			public CharacterMaster master;

			// Token: 0x040024F4 RID: 9460
			private bool foundBodyObject;

			// Token: 0x040024F5 RID: 9461
			private HealthComponent _healthComponent;

			// Token: 0x040024F6 RID: 9462
			public float lastKnownHealth;

			// Token: 0x040024F7 RID: 9463
			public float lastKnownMaxHealth;
		}
	}
}
