using System;
using EntityStates.VagrantMonster;
using RoR2;
using UnityEngine;

namespace EntityStates.VagrantNovaItem
{
	// Token: 0x02000742 RID: 1858
	public class ChargeState : BaseVagrantNovaItemState
	{
		// Token: 0x06002B18 RID: 11032 RVA: 0x000B5608 File Offset: 0x000B3808
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = ChargeState.baseDuration / (base.attachedBody ? base.attachedBody.attackSpeed : 1f);
			base.SetChargeSparkEmissionRateMultiplier(1f);
			if (base.attachedBody)
			{
				Vector3 position = base.transform.position;
				Quaternion rotation = base.transform.rotation;
				this.chargeVfxInstance = UnityEngine.Object.Instantiate<GameObject>(ChargeMegaNova.chargingEffectPrefab, position, rotation);
				this.chargeVfxInstance.transform.localScale = Vector3.one * 0.25f;
				Util.PlaySound(ChargeState.chargeSound, base.gameObject);
				this.areaIndicatorVfxInstance = UnityEngine.Object.Instantiate<GameObject>(ChargeMegaNova.areaIndicatorPrefab, position, rotation);
				ObjectScaleCurve component = this.areaIndicatorVfxInstance.GetComponent<ObjectScaleCurve>();
				component.timeMax = this.duration;
				component.baseScale = Vector3.one * DetonateState.blastRadius;
				this.areaIndicatorVfxInstance.GetComponent<AnimateShaderAlpha>().timeMax = this.duration;
			}
			RoR2Application.onLateUpdate += this.OnLateUpdate;
		}

		// Token: 0x06002B19 RID: 11033 RVA: 0x000B5720 File Offset: 0x000B3920
		public override void OnExit()
		{
			RoR2Application.onLateUpdate -= this.OnLateUpdate;
			if (this.chargeVfxInstance != null)
			{
				EntityState.Destroy(this.chargeVfxInstance);
				this.chargeVfxInstance = null;
			}
			if (this.areaIndicatorVfxInstance != null)
			{
				EntityState.Destroy(this.areaIndicatorVfxInstance);
				this.areaIndicatorVfxInstance = null;
			}
			base.OnExit();
		}

		// Token: 0x06002B1A RID: 11034 RVA: 0x000B5778 File Offset: 0x000B3978
		private void OnLateUpdate()
		{
			if (this.chargeVfxInstance)
			{
				this.chargeVfxInstance.transform.position = base.transform.position;
			}
			if (this.areaIndicatorVfxInstance)
			{
				this.areaIndicatorVfxInstance.transform.position = base.transform.position;
			}
		}

		// Token: 0x06002B1B RID: 11035 RVA: 0x000B57D5 File Offset: 0x000B39D5
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.isAuthority && base.fixedAge >= this.duration)
			{
				this.outer.SetNextState(new DetonateState());
			}
		}

		// Token: 0x040026EF RID: 9967
		public static float baseDuration = 3f;

		// Token: 0x040026F0 RID: 9968
		public static string chargeSound;

		// Token: 0x040026F1 RID: 9969
		private float duration;

		// Token: 0x040026F2 RID: 9970
		private GameObject chargeVfxInstance;

		// Token: 0x040026F3 RID: 9971
		private GameObject areaIndicatorVfxInstance;
	}
}
