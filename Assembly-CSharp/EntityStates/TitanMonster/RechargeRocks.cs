using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.TitanMonster
{
	// Token: 0x02000174 RID: 372
	internal class RechargeRocks : BaseState
	{
		// Token: 0x0600072B RID: 1835 RVA: 0x00022E9C File Offset: 0x0002109C
		public override void OnEnter()
		{
			base.OnEnter();
			this.stopwatch = 0f;
			this.duration = RechargeRocks.baseDuration / this.attackSpeedStat;
			Transform modelTransform = base.GetModelTransform();
			Util.PlaySound(RechargeRocks.attackSoundString, base.gameObject);
			base.PlayCrossfade("Body", "RechargeRocks", "RechargeRocks.playbackRate", this.duration, 0.2f);
			if (modelTransform)
			{
				ChildLocator component = modelTransform.GetComponent<ChildLocator>();
				if (component)
				{
					Transform transform = component.FindChild("LeftFist");
					if (transform && RechargeRocks.effectPrefab)
					{
						this.chargeEffect = UnityEngine.Object.Instantiate<GameObject>(RechargeRocks.effectPrefab, transform.position, transform.rotation);
						this.chargeEffect.transform.parent = transform;
						ScaleParticleSystemDuration component2 = this.chargeEffect.GetComponent<ScaleParticleSystemDuration>();
						if (component2)
						{
							component2.newDuration = this.duration;
						}
					}
				}
			}
			if (NetworkServer.active)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(RechargeRocks.rockControllerPrefab);
				gameObject.GetComponent<TitanRockController>().SetOwner(base.gameObject);
				NetworkServer.Spawn(gameObject);
			}
		}

		// Token: 0x0600072C RID: 1836 RVA: 0x00022FAF File Offset: 0x000211AF
		public override void OnExit()
		{
			base.OnExit();
			if (this.chargeEffect)
			{
				EntityState.Destroy(this.chargeEffect);
			}
		}

		// Token: 0x0600072D RID: 1837 RVA: 0x00022FCF File Offset: 0x000211CF
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;
			if (this.stopwatch >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x0600072E RID: 1838 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x040008FF RID: 2303
		public static float baseDuration = 3f;

		// Token: 0x04000900 RID: 2304
		public static float baseRechargeDuration = 2f;

		// Token: 0x04000901 RID: 2305
		public static GameObject effectPrefab;

		// Token: 0x04000902 RID: 2306
		public static string attackSoundString;

		// Token: 0x04000903 RID: 2307
		public static GameObject rockControllerPrefab;

		// Token: 0x04000904 RID: 2308
		private int rocksFired;

		// Token: 0x04000905 RID: 2309
		private float duration;

		// Token: 0x04000906 RID: 2310
		private float stopwatch;

		// Token: 0x04000907 RID: 2311
		private float rechargeStopwatch;

		// Token: 0x04000908 RID: 2312
		private GameObject chargeEffect;
	}
}
