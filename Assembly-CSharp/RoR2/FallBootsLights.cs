using System;
using EntityStates.Headstompers;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002F1 RID: 753
	public class FallBootsLights : MonoBehaviour
	{
		// Token: 0x06000F39 RID: 3897 RVA: 0x0004B1CE File Offset: 0x000493CE
		private void Start()
		{
			this.characterModel = base.GetComponentInParent<CharacterModel>();
			this.FindSourceStateMachine();
		}

		// Token: 0x06000F3A RID: 3898 RVA: 0x0004B1E4 File Offset: 0x000493E4
		private void FindSourceStateMachine()
		{
			if (!this.characterModel || !this.characterModel.body)
			{
				return;
			}
			foreach (object obj in this.characterModel.body.transform)
			{
				EntityStateMachine component = ((Transform)obj).GetComponent<EntityStateMachine>();
				if (component && component.state is BaseHeadstompersState)
				{
					this.sourceStateMachine = component;
					break;
				}
			}
		}

		// Token: 0x06000F3B RID: 3899 RVA: 0x0004B284 File Offset: 0x00049484
		private void Update()
		{
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

		// Token: 0x0400134E RID: 4942
		public GameObject readyEffect;

		// Token: 0x0400134F RID: 4943
		public GameObject triggerEffect;

		// Token: 0x04001350 RID: 4944
		public GameObject chargingEffect;

		// Token: 0x04001351 RID: 4945
		private GameObject readyEffectInstance;

		// Token: 0x04001352 RID: 4946
		private GameObject triggerEffectInstance;

		// Token: 0x04001353 RID: 4947
		private GameObject chargingEffectInstance;

		// Token: 0x04001354 RID: 4948
		private bool isReady;

		// Token: 0x04001355 RID: 4949
		private bool isTriggered;

		// Token: 0x04001356 RID: 4950
		private bool isCharging;

		// Token: 0x04001357 RID: 4951
		private CharacterModel characterModel;

		// Token: 0x04001358 RID: 4952
		private EntityStateMachine sourceStateMachine;
	}
}
