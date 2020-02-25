using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x02000579 RID: 1401
	public class DebugStats : MonoBehaviour
	{
		// Token: 0x0600214F RID: 8527 RVA: 0x000902FE File Offset: 0x0008E4FE
		private void Awake()
		{
			DebugStats.fpsTimestamps = new Queue<float>();
			this.fpsTimestampCount = (int)(this.fpsAverageTime / this.fpsAverageFrequency);
			DebugStats.budgetTimestamps = new Queue<float>();
			this.budgetTimestampCount = (int)(this.budgetAverageTime / this.budgetAverageFrequency);
		}

		// Token: 0x06002150 RID: 8528 RVA: 0x0009033C File Offset: 0x0008E53C
		private float GetAverageFPS()
		{
			if (DebugStats.fpsTimestamps.Count == 0)
			{
				return 0f;
			}
			float num = 0f;
			foreach (float num2 in DebugStats.fpsTimestamps)
			{
				num += num2;
			}
			num /= (float)DebugStats.fpsTimestamps.Count;
			return Mathf.Round(num);
		}

		// Token: 0x06002151 RID: 8529 RVA: 0x000903B8 File Offset: 0x0008E5B8
		private float GetAverageParticleCost()
		{
			if (DebugStats.budgetTimestamps.Count == 0)
			{
				return 0f;
			}
			float num = 0f;
			foreach (float num2 in DebugStats.budgetTimestamps)
			{
				num += num2;
			}
			num /= (float)DebugStats.budgetTimestamps.Count;
			return Mathf.Round(num);
		}

		// Token: 0x06002152 RID: 8530 RVA: 0x00090434 File Offset: 0x0008E634
		private void Update()
		{
			this.statsRoot.SetActive(DebugStats.showStats);
			if (!DebugStats.showStats)
			{
				return;
			}
			this.fpsStopwatch += Time.unscaledDeltaTime;
			this.fpsDisplayStopwatch += Time.unscaledDeltaTime;
			float num = 1f / Time.unscaledDeltaTime;
			if (this.fpsStopwatch >= 1f / this.fpsAverageFrequency)
			{
				this.fpsStopwatch = 0f;
				DebugStats.fpsTimestamps.Enqueue(num);
				if (DebugStats.fpsTimestamps.Count > this.fpsTimestampCount)
				{
					DebugStats.fpsTimestamps.Dequeue();
				}
			}
			if (this.fpsDisplayStopwatch > this.fpsAverageDisplayUpdateFrequency)
			{
				this.fpsDisplayStopwatch = 0f;
				float averageFPS = this.GetAverageFPS();
				this.fpsAverageText.text = string.Concat(new string[]
				{
					"FPS: ",
					Mathf.Round(num).ToString(),
					" (",
					averageFPS.ToString(),
					")"
				});
				TextMeshProUGUI textMeshProUGUI = this.fpsAverageText;
				textMeshProUGUI.text = string.Concat(new string[]
				{
					textMeshProUGUI.text,
					"\n ms: ",
					(Mathf.Round(100000f / num) / 100f).ToString(),
					"(",
					(Mathf.Round(100000f / averageFPS) / 100f).ToString(),
					")"
				});
			}
			this.budgetStopwatch += Time.unscaledDeltaTime;
			this.budgetDisplayStopwatch += Time.unscaledDeltaTime;
			float num2 = (float)VFXBudget.totalCost;
			if (this.budgetStopwatch >= 1f / this.budgetAverageFrequency)
			{
				this.budgetStopwatch = 0f;
				DebugStats.budgetTimestamps.Enqueue(num2);
				if (DebugStats.budgetTimestamps.Count > this.budgetTimestampCount)
				{
					DebugStats.budgetTimestamps.Dequeue();
				}
			}
			if (this.budgetDisplayStopwatch > 1f)
			{
				this.budgetDisplayStopwatch = 0f;
				float averageParticleCost = this.GetAverageParticleCost();
				this.budgetAverageText.text = string.Concat(new string[]
				{
					"Particle Cost: ",
					Mathf.Round(num2).ToString(),
					" (",
					averageParticleCost.ToString(),
					")"
				});
			}
			if (this.teamText)
			{
				string str = "Allies: " + TeamComponent.GetTeamMembers(TeamIndex.Player).Count + "\n";
				string str2 = "Enemies: " + TeamComponent.GetTeamMembers(TeamIndex.Monster).Count + "\n";
				string str3 = "Bosses: " + BossGroup.GetTotalBossCount() + "\n";
				string text = "Directors:";
				foreach (CombatDirector combatDirector in CombatDirector.instancesList)
				{
					string text2 = "\n==[" + combatDirector.gameObject.name + "]==";
					if (combatDirector.enabled)
					{
						string text3 = "\n - Credit: " + combatDirector.monsterCredit.ToString();
						string text4 = "\n - Target: " + (combatDirector.currentSpawnTarget ? combatDirector.currentSpawnTarget.name : "null");
						string text5 = "\n - Last Spawn Card: ";
						if (combatDirector.lastAttemptedMonsterCard != null && combatDirector.lastAttemptedMonsterCard.spawnCard)
						{
							text5 += combatDirector.lastAttemptedMonsterCard.spawnCard.name;
						}
						string text6 = "\n - Reroll Timer: " + Mathf.Round(combatDirector.monsterSpawnTimer);
						text2 = string.Concat(new string[]
						{
							text2,
							text3,
							text4,
							text5,
							text6
						});
					}
					else
					{
						text2 += " (Off)";
					}
					text = text + text2 + "\n \n";
				}
				this.teamText.text = str + str2 + str3 + text;
			}
		}

		// Token: 0x04001EBD RID: 7869
		public GameObject statsRoot;

		// Token: 0x04001EBE RID: 7870
		public TextMeshProUGUI fpsAverageText;

		// Token: 0x04001EBF RID: 7871
		public float fpsAverageFrequency = 1f;

		// Token: 0x04001EC0 RID: 7872
		public float fpsAverageTime = 60f;

		// Token: 0x04001EC1 RID: 7873
		public float fpsAverageDisplayUpdateFrequency = 1f;

		// Token: 0x04001EC2 RID: 7874
		private float fpsStopwatch;

		// Token: 0x04001EC3 RID: 7875
		private float fpsDisplayStopwatch;

		// Token: 0x04001EC4 RID: 7876
		private static Queue<float> fpsTimestamps;

		// Token: 0x04001EC5 RID: 7877
		private int fpsTimestampCount;

		// Token: 0x04001EC6 RID: 7878
		public TextMeshProUGUI budgetAverageText;

		// Token: 0x04001EC7 RID: 7879
		public float budgetAverageFrequency = 1f;

		// Token: 0x04001EC8 RID: 7880
		public float budgetAverageTime = 60f;

		// Token: 0x04001EC9 RID: 7881
		private const float budgetAverageDisplayUpdateFrequency = 1f;

		// Token: 0x04001ECA RID: 7882
		private float budgetStopwatch;

		// Token: 0x04001ECB RID: 7883
		private float budgetDisplayStopwatch;

		// Token: 0x04001ECC RID: 7884
		private static Queue<float> budgetTimestamps;

		// Token: 0x04001ECD RID: 7885
		private int budgetTimestampCount;

		// Token: 0x04001ECE RID: 7886
		private static bool showStats;

		// Token: 0x04001ECF RID: 7887
		public TextMeshProUGUI teamText;
	}
}
