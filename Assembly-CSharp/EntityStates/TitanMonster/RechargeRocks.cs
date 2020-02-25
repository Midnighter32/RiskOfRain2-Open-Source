using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.TitanMonster
{
	// Token: 0x0200085A RID: 2138
	public class RechargeRocks : BaseState
	{
		// Token: 0x06003046 RID: 12358 RVA: 0x000CFC9C File Offset: 0x000CDE9C
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

		// Token: 0x06003047 RID: 12359 RVA: 0x000CFDAF File Offset: 0x000CDFAF
		public override void OnExit()
		{
			base.OnExit();
			if (this.chargeEffect)
			{
				EntityState.Destroy(this.chargeEffect);
			}
		}

		// Token: 0x06003048 RID: 12360 RVA: 0x000CFDCF File Offset: 0x000CDFCF
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

		// Token: 0x06003049 RID: 12361 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04002E6C RID: 11884
		public static float baseDuration = 3f;

		// Token: 0x04002E6D RID: 11885
		public static float baseRechargeDuration = 2f;

		// Token: 0x04002E6E RID: 11886
		public static GameObject effectPrefab;

		// Token: 0x04002E6F RID: 11887
		public static string attackSoundString;

		// Token: 0x04002E70 RID: 11888
		public static GameObject rockControllerPrefab;

		// Token: 0x04002E71 RID: 11889
		private int rocksFired;

		// Token: 0x04002E72 RID: 11890
		private float duration;

		// Token: 0x04002E73 RID: 11891
		private float stopwatch;

		// Token: 0x04002E74 RID: 11892
		private float rechargeStopwatch;

		// Token: 0x04002E75 RID: 11893
		private GameObject chargeEffect;
	}
}
