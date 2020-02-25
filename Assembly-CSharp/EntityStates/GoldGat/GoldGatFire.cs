using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.GoldGat
{
	// Token: 0x02000863 RID: 2147
	public class GoldGatFire : BaseGoldGatState
	{
		// Token: 0x06003071 RID: 12401 RVA: 0x000D0B2F File Offset: 0x000CED2F
		public override void OnEnter()
		{
			base.OnEnter();
			this.loopSoundID = Util.PlaySound(GoldGatFire.windUpSoundString, base.gameObject);
			this.FireBullet();
		}

		// Token: 0x06003072 RID: 12402 RVA: 0x000D0B54 File Offset: 0x000CED54
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
					bulletCount = 1U,
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
			EffectManager.SimpleMuzzleFlash(GoldGatFire.muzzleFlashEffectPrefab, base.gameObject, "Muzzle", false);
		}

		// Token: 0x06003073 RID: 12403 RVA: 0x000D0D5C File Offset: 0x000CEF5C
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

		// Token: 0x06003074 RID: 12404 RVA: 0x000D0DED File Offset: 0x000CEFED
		public override void OnExit()
		{
			AkSoundEngine.StopPlayingID(this.loopSoundID);
			base.OnExit();
		}

		// Token: 0x04002EAF RID: 11951
		public static float minFireFrequency;

		// Token: 0x04002EB0 RID: 11952
		public static float maxFireFrequency;

		// Token: 0x04002EB1 RID: 11953
		public static float minSpread;

		// Token: 0x04002EB2 RID: 11954
		public static float maxSpread;

		// Token: 0x04002EB3 RID: 11955
		public static float windUpDuration;

		// Token: 0x04002EB4 RID: 11956
		public static float force;

		// Token: 0x04002EB5 RID: 11957
		public static float damageCoefficient;

		// Token: 0x04002EB6 RID: 11958
		public static float procCoefficient;

		// Token: 0x04002EB7 RID: 11959
		public static string attackSoundString;

		// Token: 0x04002EB8 RID: 11960
		public static GameObject tracerEffectPrefab;

		// Token: 0x04002EB9 RID: 11961
		public static GameObject impactEffectPrefab;

		// Token: 0x04002EBA RID: 11962
		public static GameObject muzzleFlashEffectPrefab;

		// Token: 0x04002EBB RID: 11963
		public static int baseMoneyCostPerBullet;

		// Token: 0x04002EBC RID: 11964
		public static string windUpSoundString;

		// Token: 0x04002EBD RID: 11965
		public static string windUpRTPC;

		// Token: 0x04002EBE RID: 11966
		public float totalStopwatch;

		// Token: 0x04002EBF RID: 11967
		private float stopwatch;

		// Token: 0x04002EC0 RID: 11968
		private float fireFrequency;

		// Token: 0x04002EC1 RID: 11969
		private uint loopSoundID;
	}
}
