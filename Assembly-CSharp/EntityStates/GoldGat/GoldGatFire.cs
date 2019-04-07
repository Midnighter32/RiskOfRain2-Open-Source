using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.GoldGat
{
	// Token: 0x0200017D RID: 381
	public class GoldGatFire : BaseGoldGatState
	{
		// Token: 0x06000754 RID: 1876 RVA: 0x00023CB3 File Offset: 0x00021EB3
		public override void OnEnter()
		{
			base.OnEnter();
			this.loopSoundID = Util.PlaySound(GoldGatFire.windUpSoundString, base.gameObject);
			this.FireBullet();
		}

		// Token: 0x06000755 RID: 1877 RVA: 0x00023CD8 File Offset: 0x00021ED8
		private void FireBullet()
		{
			this.body.SetAimTimer(2f);
			float t = Mathf.Clamp01(this.totalStopwatch / GoldGatFire.windUpDuration);
			this.fireFrequency = Mathf.Lerp(GoldGatFire.minFireFrequency, GoldGatFire.maxFireFrequency, t);
			float num = Mathf.Lerp(GoldGatFire.minSpread, GoldGatFire.maxSpread, t);
			Util.PlaySound(GoldGatFire.attackSoundString, base.gameObject);
			int num2 = (int)((float)GoldGatFire.baseMoneyCostPerBullet * (1f + (TeamManager.instance.GetTeamLevel(this.bodyMaster.teamIndex) - 1f) * 0.25f));
			if (base.isAuthority && this.gunChildLocator && this.gunChildLocator.FindChild("Muzzle"))
			{
				new BulletAttack
				{
					owner = this.networkedBodyAttachment.attachedBodyObject,
					aimVector = this.bodyInputBank.aimDirection,
					origin = this.bodyInputBank.aimOrigin,
					falloffModel = BulletAttack.FalloffModel.DefaultBullet,
					force = GoldGatFire.force,
					damage = this.body.damage * GoldGatFire.damageCoefficient,
					damageColorIndex = DamageColorIndex.Item,
					bulletCount = 1u,
					minSpread = 0f,
					maxSpread = num,
					tracerEffectPrefab = GoldGatFire.tracerEffectPrefab,
					isCrit = Util.CheckRoll(this.body.crit, this.bodyMaster),
					procCoefficient = GoldGatFire.procCoefficient,
					muzzleName = "Muzzle",
					weapon = base.gameObject
				}.Fire();
				this.gunAnimator.Play("Fire");
			}
			if (NetworkServer.active)
			{
				this.bodyMaster.money = (uint)Mathf.Max(0f, (float)((ulong)this.bodyMaster.money - (ulong)((long)num2)));
			}
			this.gunAnimator.SetFloat("Crank.playbackRate", this.fireFrequency);
			EffectManager.instance.SimpleMuzzleFlash(GoldGatFire.muzzleFlashEffectPrefab, base.gameObject, "Muzzle", false);
		}

		// Token: 0x06000756 RID: 1878 RVA: 0x00023EE4 File Offset: 0x000220E4
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.totalStopwatch += Time.fixedDeltaTime;
			this.stopwatch += Time.fixedDeltaTime;
			AkSoundEngine.SetRTPCValueByPlayingID(GoldGatFire.windUpRTPC, Mathf.InverseLerp(GoldGatFire.minFireFrequency, GoldGatFire.maxFireFrequency, this.fireFrequency) * 100f, this.loopSoundID);
			if (base.CheckReturnToIdle())
			{
				return;
			}
			if (this.stopwatch > 1f / this.fireFrequency)
			{
				this.stopwatch = 0f;
				this.FireBullet();
			}
		}

		// Token: 0x06000757 RID: 1879 RVA: 0x00023F75 File Offset: 0x00022175
		public override void OnExit()
		{
			AkSoundEngine.StopPlayingID(this.loopSoundID);
			base.OnExit();
		}

		// Token: 0x04000941 RID: 2369
		public static float minFireFrequency;

		// Token: 0x04000942 RID: 2370
		public static float maxFireFrequency;

		// Token: 0x04000943 RID: 2371
		public static float minSpread;

		// Token: 0x04000944 RID: 2372
		public static float maxSpread;

		// Token: 0x04000945 RID: 2373
		public static float windUpDuration;

		// Token: 0x04000946 RID: 2374
		public static float force;

		// Token: 0x04000947 RID: 2375
		public static float damageCoefficient;

		// Token: 0x04000948 RID: 2376
		public static float procCoefficient;

		// Token: 0x04000949 RID: 2377
		public static string attackSoundString;

		// Token: 0x0400094A RID: 2378
		public static GameObject tracerEffectPrefab;

		// Token: 0x0400094B RID: 2379
		public static GameObject impactEffectPrefab;

		// Token: 0x0400094C RID: 2380
		public static GameObject muzzleFlashEffectPrefab;

		// Token: 0x0400094D RID: 2381
		public static int baseMoneyCostPerBullet;

		// Token: 0x0400094E RID: 2382
		public static string windUpSoundString;

		// Token: 0x0400094F RID: 2383
		public static string windUpRTPC;

		// Token: 0x04000950 RID: 2384
		public float totalStopwatch;

		// Token: 0x04000951 RID: 2385
		private float stopwatch;

		// Token: 0x04000952 RID: 2386
		private float fireFrequency;

		// Token: 0x04000953 RID: 2387
		private uint loopSoundID;
	}
}
