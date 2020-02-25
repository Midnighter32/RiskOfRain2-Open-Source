using System;
using EntityStates.Headstompers;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020001FF RID: 511
	public class FallBootsLights : MonoBehaviour
	{
		// Token: 0x06000AE7 RID: 2791 RVA: 0x0003013E File Offset: 0x0002E33E
		private void Start()
		{
			this.characterModel = base.GetComponentInParent<CharacterModel>();
			this.FindSourceStateMachine();
		}

		// Token: 0x06000AE8 RID: 2792 RVA: 0x00030154 File Offset: 0x0002E354
		private void FindSourceStateMachine()
		{
			if (!this.characterModel || !this.characterModel.body)
			{
				return;
			}
			BaseHeadstompersState baseHeadstompersState = BaseHeadstompersState.FindForBody(this.characterModel.body);
			this.sourceStateMachine = ((baseHeadstompersState != null) ? baseHeadstompersState.outer : null);
		}

		// Token: 0x06000AE9 RID: 2793 RVA: 0x000301A4 File Offset: 0x0002E3A4
		private void Update()
		{
			if (!this.sourceStateMachine)
			{
				this.FindSourceStateMachine();
			}
			bool flag = this.sourceStateMachine && !(this.sourceStateMachine.state is HeadstompersCooldown);
			if (flag != this.isReady)
			{
				if (flag)
				{
					this.readyEffectInstance = UnityEngine.Object.Instantiate<GameObject>(this.readyEffect, base.transform.position, base.transform.rotation, base.transform);
					Util.PlaySound("Play_item_proc_fallboots_activate", base.gameObject);
				}
				else if (this.readyEffectInstance)
				{
					UnityEngine.Object.Destroy(this.readyEffectInstance);
				}
				this.isReady = flag;
			}
			bool flag2 = this.sourceStateMachine && this.sourceStateMachine.state is HeadstompersFall;
			if (flag2 != this.isTriggered)
			{
				if (flag2)
				{
					this.triggerEffectInstance = UnityEngine.Object.Instantiate<GameObject>(this.triggerEffect, base.transform.position, base.transform.rotation, base.transform);
					Util.PlaySound("Play_item_proc_fallboots_activate", base.gameObject);
				}
				else if (this.triggerEffectInstance)
				{
					UnityEngine.Object.Destroy(this.triggerEffectInstance);
				}
				this.isTriggered = flag2;
			}
			bool flag3 = this.sourceStateMachine && this.sourceStateMachine.state is HeadstompersCharge;
			if (flag3 != this.isCharging)
			{
				if (flag3)
				{
					this.chargingEffectInstance = UnityEngine.Object.Instantiate<GameObject>(this.chargingEffect, base.transform.position, base.transform.rotation, base.transform);
				}
				else if (this.chargingEffectInstance)
				{
					UnityEngine.Object.Destroy(this.chargingEffectInstance);
				}
				this.isCharging = flag3;
			}
		}

		// Token: 0x04000B30 RID: 2864
		public GameObject readyEffect;

		// Token: 0x04000B31 RID: 2865
		public GameObject triggerEffect;

		// Token: 0x04000B32 RID: 2866
		public GameObject chargingEffect;

		// Token: 0x04000B33 RID: 2867
		private GameObject readyEffectInstance;

		// Token: 0x04000B34 RID: 2868
		private GameObject triggerEffectInstance;

		// Token: 0x04000B35 RID: 2869
		private GameObject chargingEffectInstance;

		// Token: 0x04000B36 RID: 2870
		private bool isReady;

		// Token: 0x04000B37 RID: 2871
		private bool isTriggered;

		// Token: 0x04000B38 RID: 2872
		private bool isCharging;

		// Token: 0x04000B39 RID: 2873
		private CharacterModel characterModel;

		// Token: 0x04000B3A RID: 2874
		private EntityStateMachine sourceStateMachine;
	}
}
