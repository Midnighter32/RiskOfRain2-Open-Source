using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Wisp1Monster
{
	// Token: 0x02000722 RID: 1826
	public class ChargeEmbers : BaseState
	{
		// Token: 0x06002A80 RID: 10880 RVA: 0x000B2DC0 File Offset: 0x000B0FC0
		public override void OnEnter()
		{
			base.OnEnter();
			this.stopwatch = 0f;
			this.duration = ChargeEmbers.baseDuration / this.attackSpeedStat;
			Util.PlayScaledSound(ChargeEmbers.attackString, base.gameObject, this.attackSpeedStat);
			Transform modelTransform = base.GetModelTransform();
			if (modelTransform)
			{
				ChildLocator component = modelTransform.GetComponent<ChildLocator>();
				if (component)
				{
					Transform transform = component.FindChild("Muzzle");
					if (transform)
					{
						if (ChargeEmbers.chargeEffectPrefab)
						{
							this.chargeEffectInstance = UnityEngine.Object.Instantiate<GameObject>(ChargeEmbers.chargeEffectPrefab, transform.position, transform.rotation);
							this.chargeEffectInstance.transform.parent = transform;
							ScaleParticleSystemDuration component2 = this.chargeEffectInstance.GetComponent<ScaleParticleSystemDuration>();
							if (component2)
							{
								component2.newDuration = this.duration;
							}
						}
						if (ChargeEmbers.laserEffectPrefab)
						{
							this.laserEffectInstance = UnityEngine.Object.Instantiate<GameObject>(ChargeEmbers.laserEffectPrefab, transform.position, transform.rotation);
							this.laserEffectInstance.transform.parent = transform;
							this.laserEffectInstanceLineRenderer = this.laserEffectInstance.GetComponent<LineRenderer>();
						}
					}
				}
			}
			base.PlayAnimation("Body", "ChargeAttack1", "ChargeAttack1.playbackRate", this.duration);
			if (base.characterBody)
			{
				base.characterBody.SetAimTimer(this.duration);
			}
		}

		// Token: 0x06002A81 RID: 10881 RVA: 0x000B2F1E File Offset: 0x000B111E
		public override void OnExit()
		{
			base.OnExit();
			if (this.chargeEffectInstance)
			{
				EntityState.Destroy(this.chargeEffectInstance);
			}
			if (this.laserEffectInstance)
			{
				EntityState.Destroy(this.laserEffectInstance);
			}
		}

		// Token: 0x06002A82 RID: 10882 RVA: 0x000B2F58 File Offset: 0x000B1158
		public override void Update()
		{
			base.Update();
			Ray aimRay = base.GetAimRay();
			float distance = 50f;
			Vector3 origin = aimRay.origin;
			Vector3 point = aimRay.GetPoint(distance);
			this.laserEffectInstanceLineRenderer.SetPosition(0, origin);
			this.laserEffectInstanceLineRenderer.SetPosition(1, point);
			Color startColor = new Color(1f, 1f, 1f, this.stopwatch / this.duration);
			Color clear = Color.clear;
			this.laserEffectInstanceLineRenderer.startColor = startColor;
			this.laserEffectInstanceLineRenderer.endColor = clear;
		}

		// Token: 0x06002A83 RID: 10883 RVA: 0x000B2FE8 File Offset: 0x000B11E8
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;
			if (this.stopwatch >= this.duration && base.isAuthority)
			{
				this.outer.SetNextState(new FireEmbers());
				return;
			}
		}

		// Token: 0x06002A84 RID: 10884 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x0400265D RID: 9821
		public static float baseDuration = 3f;

		// Token: 0x0400265E RID: 9822
		public static GameObject chargeEffectPrefab;

		// Token: 0x0400265F RID: 9823
		public static GameObject laserEffectPrefab;

		// Token: 0x04002660 RID: 9824
		public static string attackString;

		// Token: 0x04002661 RID: 9825
		private float duration;

		// Token: 0x04002662 RID: 9826
		private float stopwatch;

		// Token: 0x04002663 RID: 9827
		private GameObject chargeEffectInstance;

		// Token: 0x04002664 RID: 9828
		private GameObject laserEffectInstance;

		// Token: 0x04002665 RID: 9829
		private LineRenderer laserEffectInstanceLineRenderer;
	}
}
