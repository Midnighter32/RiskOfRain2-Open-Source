using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x020005A6 RID: 1446
	public class DebugStats : MonoBehaviour
	{
		// Token: 0x0600205E RID: 8286 RVA: 0x00098730 File Offset: 0x00096930
		private void Awake()
		{
			DebugStats.fpsTimestamps = new Queue<float>();
			this.fpsTimestampCount = (int)(this.fpsAverageTime / this.fpsAverageFrequency);
			DebugStats.budgetTimestamps = new Queue<float>();
			this.budgetTimestampCount = (int)(this.budgetAverageTime / this.budgetAverageFrequency);
		}

		// Token: 0x0600205F RID: 8287 RVA: 0x00098770 File Offset: 0x00096970
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

		// Token: 0x06002060 RID: 8288 RVA: 0x000987EC File Offset: 0x000969EC
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

		// Token: 0x06002061 RID: 8289 RVA: 0x00098868 File Offset: 0x00096A68
		private void FixedUpdate()
		{
			if (Input.GetKeyDown(KeyCode.F1))
			{
				DebugStats.showStats = !DebugStats.showStats;
			}
			this.statsRoot.SetActive(DebugStats.showStats);
		}

		// Token: 0x06002062 RID: 8290 RVA: 0x00098894 File Offset: 0x00096A94
		private void Update()
		{
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

		// Token: 0x040022EA RID: 8938
		public GameObject statsRoot;

		// Token: 0x040022EB RID: 8939
		public TextMeshProUGUI fpsAverageText;

		// Token: 0x040022EC RID: 8940
		public float fpsAverageFrequency = 1f;

		// Token: 0x040022ED RID: 8941
		public float fpsAverageTime = 60f;

		// Token: 0x040022EE RID: 8942
		public float fpsAverageDisplayUpdateFrequency = 1f;

		// Token: 0x040022EF RID: 8943
		private float fpsStopwatch;

		// Token: 0x040022F0 RID: 8944
		private float fpsDisplayStopwatch;

		// Token: 0x040022F1 RID: 8945
		private static Queue<float> fpsTimestamps;

		// Token: 0x040022F2 RID: 8946
		private int fpsTimestampCount;

		// Token: 0x040022F3 RID: 8947
		public TextMeshProUGUI budgetAverageText;

		// Token: 0x040022F4 RID: 8948
		public float budgetAverageFrequency = 1f;

		// Token: 0x040022F5 RID: 8949
		public float budgetAverageTime = 60f;

		// Token: 0x040022F6 RID: 8950
		private const float budgetAverageDisplayUpdateFrequency = 1f;

		// Token: 0x040022F7 RID: 8951
		private float budgetStopwatch;

		// Token: 0x040022F8 RID: 8952
		private float budgetDisplayStopwatch;

		// Token: 0x040022F9 RID: 8953
		private static Queue<float> budgetTimestamps;

		// Token: 0x040022FA RID: 8954
		private int budgetTimestampCount;

		// Token: 0x040022FB RID: 8955
		private static bool showStats;

		// Token: 0x040022FC RID: 8956
		public TextMeshProUGUI teamText;
	}
}
