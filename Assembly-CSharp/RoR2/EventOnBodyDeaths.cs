using System;
using UnityEngine;
using UnityEngine.Events;

namespace RoR2
{
	// Token: 0x020002EA RID: 746
	public class EventOnBodyDeaths : MonoBehaviour
	{
		// Token: 0x06000F14 RID: 3860 RVA: 0x0004A931 File Offset: 0x00048B31
		private void OnEnable()
		{
			GlobalEventManager.onCharacterDeathGlobal += this.OnCharacterDeath;
		}

		// Token: 0x06000F15 RID: 3861 RVA: 0x0004A944 File Offset: 0x00048B44
		private void OnDisable()
		{
			GlobalEventManager.onCharacterDeathGlobal -= this.OnCharacterDeath;
		}

		// Token: 0x06000F16 RID: 3862 RVA: 0x0004A958 File Offset: 0x00048B58
		private void OnCharacterDeath(DamageReport damageReport)
		{
			if (damageReport.victimBody)
			{
				for (int i = 0; i < this.bodyNames.Length; i++)
				{
					if (damageReport.victimBody.name.Contains(this.bodyNames[i]))
					{
						this.currentDeathCount++;
						break;
					}
				}
			}
			if (this.currentDeathCount >= this.targetDeathCount)
			{
				UnityEvent unityEvent = this.onAchieved;
				if (unityEvent == null)
				{
					return;
				}
				unityEvent.Invoke();
			}
		}

		// Token: 0x04001326 RID: 4902
		public string[] bodyNames;

		// Token: 0x04001327 RID: 4903
		private int currentDeathCount;

		// Token: 0x04001328 RID: 4904
		public int targetDeathCount;

		// Token: 0x04001329 RID: 4905
		public UnityEvent onAchieved;
	}
}
