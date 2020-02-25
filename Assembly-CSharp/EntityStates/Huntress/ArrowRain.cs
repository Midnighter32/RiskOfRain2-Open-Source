using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.Huntress
{
	// Token: 0x0200082B RID: 2091
	public class ArrowRain : BaseArrowBarrage
	{
		// Token: 0x06002F58 RID: 12120 RVA: 0x000CA178 File Offset: 0x000C8378
		public override void OnEnter()
		{
			base.OnEnter();
			base.PlayAnimation("FullBody, Override", "LoopArrowRain");
			if (ArrowRain.areaIndicatorPrefab)
			{
				this.areaIndicatorInstance = UnityEngine.Object.Instantiate<GameObject>(ArrowRain.areaIndicatorPrefab);
				this.areaIndicatorInstance.transform.localScale = new Vector3(ArrowRain.arrowRainRadius, ArrowRain.arrowRainRadius, ArrowRain.arrowRainRadius);
			}
		}

		// Token: 0x06002F59 RID: 12121 RVA: 0x000CA1DC File Offset: 0x000C83DC
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
		}

		// Token: 0x06002F5A RID: 12122 RVA: 0x000CA24C File Offset: 0x000C844C
		public override void Update()
		{
			base.Update();
			this.UpdateAreaIndicator();
		}

		// Token: 0x06002F5B RID: 12123 RVA: 0x000CA25A File Offset: 0x000C845A
		protected override void HandlePrimaryAttack()
		{
			base.HandlePrimaryAttack();
			this.shouldFireArrowRain = true;
			this.outer.SetNextStateToMain();
		}

		// Token: 0x06002F5C RID: 12124 RVA: 0x000CA274 File Offset: 0x000C8474
		protected void DoFireArrowRain()
		{
			EffectManager.SimpleMuzzleFlash(ArrowRain.muzzleFlashEffect, base.gameObject, "Muzzle", false);
			if (this.areaIndicatorInstance && this.shouldFireArrowRain)
			{
				ProjectileManager.instance.FireProjectile(ArrowRain.projectilePrefab, this.areaIndicatorInstance.transform.position, this.areaIndicatorInstance.transform.rotation, base.gameObject, this.damageStat * ArrowRain.damageCoefficient, 0f, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
			}
		}

		// Token: 0x06002F5D RID: 12125 RVA: 0x000CA30F File Offset: 0x000C850F
		public override void OnExit()
		{
			if (this.shouldFireArrowRain && !this.outer.destroying)
			{
				this.DoFireArrowRain();
			}
			if (this.areaIndicatorInstance)
			{
				EntityState.Destroy(this.areaIndicatorInstance.gameObject);
			}
			base.OnExit();
		}

		// Token: 0x04002CDA RID: 11482
		public static float arrowRainRadius;

		// Token: 0x04002CDB RID: 11483
		public static float damageCoefficient;

		// Token: 0x04002CDC RID: 11484
		public static GameObject projectilePrefab;

		// Token: 0x04002CDD RID: 11485
		public static GameObject areaIndicatorPrefab;

		// Token: 0x04002CDE RID: 11486
		public static GameObject muzzleFlashEffect;

		// Token: 0x04002CDF RID: 11487
		private GameObject areaIndicatorInstance;

		// Token: 0x04002CE0 RID: 11488
		private bool shouldFireArrowRain;
	}
}
