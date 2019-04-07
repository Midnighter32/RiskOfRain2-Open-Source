using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Wisp1Monster
{
	// Token: 0x020000C6 RID: 198
	public class ChargeEmbers : BaseState
	{
		// Token: 0x060003DA RID: 986 RVA: 0x0000FDC8 File Offset: 0x0000DFC8
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

		// Token: 0x060003DB RID: 987 RVA: 0x0000FF26 File Offset: 0x0000E126
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

		// Token: 0x060003DC RID: 988 RVA: 0x0000FF60 File Offset: 0x0000E160
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

		// Token: 0x060003DD RID: 989 RVA: 0x0000FFF0 File Offset: 0x0000E1F0
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

		// Token: 0x060003DE RID: 990 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04000398 RID: 920
		public static float baseDuration = 3f;

		// Token: 0x04000399 RID: 921
		public static GameObject chargeEffectPrefab;

		// Token: 0x0400039A RID: 922
		public static GameObject laserEffectPrefab;

		// Token: 0x0400039B RID: 923
		public static string attackString;

		// Token: 0x0400039C RID: 924
		private float duration;

		// Token: 0x0400039D RID: 925
		private float stopwatch;

		// Token: 0x0400039E RID: 926
		private GameObject chargeEffectInstance;

		// Token: 0x0400039F RID: 927
		private GameObject laserEffectInstance;

		// Token: 0x040003A0 RID: 928
		private LineRenderer laserEffectInstanceLineRenderer;
	}
}
