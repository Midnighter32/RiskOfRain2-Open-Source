using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Mage.Weapon
{
	// Token: 0x02000111 RID: 273
	public class ChargeMeteor : BaseState
	{
		// Token: 0x06000536 RID: 1334 RVA: 0x0001717A File Offset: 0x0001537A
		public override void OnEnter()
		{
			base.OnEnter();
			this.chargeDuration = ChargeMeteor.baseChargeDuration / this.attackSpeedStat;
			this.duration = ChargeMeteor.baseDuration / this.attackSpeedStat;
			this.UpdateAreaIndicator();
		}

		// Token: 0x06000537 RID: 1335 RVA: 0x000171AC File Offset: 0x000153AC
		private void UpdateAreaIndicator()
		{
			if (this.areaIndicatorInstance)
			{
				float maxDistance = 1000f;
				RaycastHit raycastHit;
				if (Physics.Raycast(base.GetAimRay(), out raycastHit, maxDistance, LayerIndex.world.mask))
				{
					this.areaIndicatorInstance.transform.position = raycastHit.point;
					this.areaIndicatorInstance.transform.up = raycastHit.normal;
				}
			}
			else
			{
				this.areaIndicatorInstance = UnityEngine.Object.Instantiate<GameObject>(ChargeMeteor.areaIndicatorPrefab);
			}
			this.radius = Util.Remap(Mathf.Clamp01(this.stopwatch / this.chargeDuration), 0f, 1f, ChargeMeteor.minMeteorRadius, ChargeMeteor.maxMeteorRadius);
			this.areaIndicatorInstance.transform.localScale = new Vector3(this.radius, this.radius, this.radius);
		}

		// Token: 0x06000538 RID: 1336 RVA: 0x00017286 File Offset: 0x00015486
		public override void Update()
		{
			base.Update();
			this.UpdateAreaIndicator();
		}

		// Token: 0x06000539 RID: 1337 RVA: 0x00017294 File Offset: 0x00015494
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;
			if ((this.stopwatch >= this.duration || base.inputBank.skill2.justReleased) && base.isAuthority)
			{
				this.fireMeteor = true;
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x0600053A RID: 1338 RVA: 0x000172F4 File Offset: 0x000154F4
		public override void OnExit()
		{
			EffectManager.instance.SimpleMuzzleFlash(ChargeMeteor.muzzleflashEffect, base.gameObject, "Muzzle", false);
			if (this.areaIndicatorInstance)
			{
				if (this.fireMeteor)
				{
					float num = Util.Remap(Mathf.Clamp01(this.stopwatch / this.chargeDuration), 0f, 1f, ChargeMeteor.minDamageCoefficient, ChargeMeteor.maxDamageCoefficient);
					EffectManager.instance.SpawnEffect(ChargeMeteor.meteorEffect, new EffectData
					{
						origin = this.areaIndicatorInstance.transform.position,
						scale = this.radius
					}, true);
					BlastAttack blastAttack = new BlastAttack();
					blastAttack.radius = this.radius;
					blastAttack.procCoefficient = ChargeMeteor.procCoefficient;
					blastAttack.position = this.areaIndicatorInstance.transform.position;
					blastAttack.attacker = base.gameObject;
					blastAttack.crit = Util.CheckRoll(base.characterBody.crit, base.characterBody.master);
					blastAttack.baseDamage = base.characterBody.damage * num;
					blastAttack.falloffModel = BlastAttack.FalloffModel.SweetSpot;
					blastAttack.baseForce = ChargeMeteor.force;
					blastAttack.teamIndex = TeamComponent.GetObjectTeam(blastAttack.attacker);
					blastAttack.Fire();
				}
				EntityState.Destroy(this.areaIndicatorInstance.gameObject);
			}
			base.OnExit();
		}

		// Token: 0x04000572 RID: 1394
		public static float baseChargeDuration;

		// Token: 0x04000573 RID: 1395
		public static float baseDuration;

		// Token: 0x04000574 RID: 1396
		public static GameObject areaIndicatorPrefab;

		// Token: 0x04000575 RID: 1397
		public static float minMeteorRadius = 0f;

		// Token: 0x04000576 RID: 1398
		public static float maxMeteorRadius = 10f;

		// Token: 0x04000577 RID: 1399
		public static GameObject meteorEffect;

		// Token: 0x04000578 RID: 1400
		public static float minDamageCoefficient;

		// Token: 0x04000579 RID: 1401
		public static float maxDamageCoefficient;

		// Token: 0x0400057A RID: 1402
		public static float procCoefficient;

		// Token: 0x0400057B RID: 1403
		public static float force;

		// Token: 0x0400057C RID: 1404
		public static GameObject muzzleflashEffect;

		// Token: 0x0400057D RID: 1405
		private float stopwatch;

		// Token: 0x0400057E RID: 1406
		private GameObject areaIndicatorInstance;

		// Token: 0x0400057F RID: 1407
		private bool fireMeteor;

		// Token: 0x04000580 RID: 1408
		private float radius;

		// Token: 0x04000581 RID: 1409
		private float chargeDuration;

		// Token: 0x04000582 RID: 1410
		private float duration;
	}
}
