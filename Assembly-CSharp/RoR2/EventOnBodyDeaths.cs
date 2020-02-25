using System;
using UnityEngine;
using UnityEngine.Events;

namespace RoR2
{
	// Token: 0x020001F8 RID: 504
	public class EventOnBodyDeaths : MonoBehaviour
	{
		// Token: 0x06000ABC RID: 2748 RVA: 0x0002F861 File Offset: 0x0002DA61
		private void OnEnable()
		{
			GlobalEventManager.onCharacterDeathGlobal += this.OnCharacterDeath;
		}

		// Token: 0x06000ABD RID: 2749 RVA: 0x0002F874 File Offset: 0x0002DA74
		private void OnDisable()
		{
			GlobalEventManager.onCharacterDeathGlobal -= this.OnCharacterDeath;
		}

		// Token: 0x06000ABE RID: 2750 RVA: 0x0002F888 File Offset: 0x0002DA88
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

		// Token: 0x04000B08 RID: 2824
		public string[] bodyNames;

		// Token: 0x04000B09 RID: 2825
		private int currentDeathCount;

		// Token: 0x04000B0A RID: 2826
		public int targetDeathCount;

		// Token: 0x04000B0B RID: 2827
		public UnityEvent onAchieved;
	}
}
