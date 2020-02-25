using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Mage.Weapon
{
	// Token: 0x020007CF RID: 1999
	public class ChargeMeteor : BaseState
	{
		// Token: 0x06002D95 RID: 11669 RVA: 0x000C14BE File Offset: 0x000BF6BE
		public override void OnEnter()
		{
			base.OnEnter();
			this.chargeDuration = ChargeMeteor.baseChargeDuration / this.attackSpeedStat;
			this.duration = ChargeMeteor.baseDuration / this.attackSpeedStat;
			this.UpdateAreaIndicator();
		}

		// Token: 0x06002D96 RID: 11670 RVA: 0x000C14F0 File Offset: 0x000BF6F0
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

		// Token: 0x06002D97 RID: 11671 RVA: 0x000C15CA File Offset: 0x000BF7CA
		public override void Update()
		{
			base.Update();
			this.UpdateAreaIndicator();
		}

		// Token: 0x06002D98 RID: 11672 RVA: 0x000C15D8 File Offset: 0x000BF7D8
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

		// Token: 0x06002D99 RID: 11673 RVA: 0x000C1638 File Offset: 0x000BF838
		public override void OnExit()
		{
			EffectManager.SimpleMuzzleFlash(ChargeMeteor.muzzleflashEffect, base.gameObject, "Muzzle", false);
			if (this.areaIndicatorInstance)
			{
				if (this.fireMeteor)
				{
					float num = Util.Remap(Mathf.Clamp01(this.stopwatch / this.chargeDuration), 0f, 1f, ChargeMeteor.minDamageCoefficient, ChargeMeteor.maxDamageCoefficient);
					EffectManager.SpawnEffect(ChargeMeteor.meteorEffect, new EffectData
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

		// Token: 0x04002A2E RID: 10798
		public static float baseChargeDuration;

		// Token: 0x04002A2F RID: 10799
		public static float baseDuration;

		// Token: 0x04002A30 RID: 10800
		public static GameObject areaIndicatorPrefab;

		// Token: 0x04002A31 RID: 10801
		public static float minMeteorRadius = 0f;

		// Token: 0x04002A32 RID: 10802
		public static float maxMeteorRadius = 10f;

		// Token: 0x04002A33 RID: 10803
		public static GameObject meteorEffect;

		// Token: 0x04002A34 RID: 10804
		public static float minDamageCoefficient;

		// Token: 0x04002A35 RID: 10805
		public static float maxDamageCoefficient;

		// Token: 0x04002A36 RID: 10806
		public static float procCoefficient;

		// Token: 0x04002A37 RID: 10807
		public static float force;

		// Token: 0x04002A38 RID: 10808
		public static GameObject muzzleflashEffect;

		// Token: 0x04002A39 RID: 10809
		private float stopwatch;

		// Token: 0x04002A3A RID: 10810
		private GameObject areaIndicatorInstance;

		// Token: 0x04002A3B RID: 10811
		private bool fireMeteor;

		// Token: 0x04002A3C RID: 10812
		private float radius;

		// Token: 0x04002A3D RID: 10813
		private float chargeDuration;

		// Token: 0x04002A3E RID: 10814
		private float duration;
	}
}
