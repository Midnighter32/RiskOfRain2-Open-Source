using System;
using System.Linq;
using RoR2;
using UnityEngine;

namespace EntityStates.TitanMonster
{
	// Token: 0x02000850 RID: 2128
	public class ChargeMegaLaser : BaseState
	{
		// Token: 0x0600301D RID: 12317 RVA: 0x000CE590 File Offset: 0x000CC790
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = ChargeMegaLaser.baseDuration / this.attackSpeedStat;
			Transform modelTransform = base.GetModelTransform();
			Util.PlayScaledSound(ChargeMegaLaser.chargeAttackSoundString, base.gameObject, 2.1f / this.duration);
			Ray aimRay = base.GetAimRay();
			this.enemyFinder = new BullseyeSearch();
			this.enemyFinder.maxDistanceFilter = 2000f;
			this.enemyFinder.maxAngleFilter = ChargeMegaLaser.lockOnAngle;
			this.enemyFinder.searchOrigin = aimRay.origin;
			this.enemyFinder.searchDirection = aimRay.direction;
			this.enemyFinder.filterByLoS = false;
			this.enemyFinder.sortMode = BullseyeSearch.SortMode.Angle;
			this.enemyFinder.teamMaskFilter = TeamMask.allButNeutral;
			if (base.teamComponent)
			{
				this.enemyFinder.teamMaskFilter.RemoveTeam(base.teamComponent.teamIndex);
			}
			if (modelTransform)
			{
				ChildLocator component = modelTransform.GetComponent<ChildLocator>();
				if (component)
				{
					Transform transform = component.FindChild("MuzzleLaser");
					if (transform)
					{
						if (this.effectPrefab)
						{
							this.chargeEffect = UnityEngine.Object.Instantiate<GameObject>(this.effectPrefab, transform.position, transform.rotation);
							this.chargeEffect.transform.parent = transform;
							ScaleParticleSystemDuration component2 = this.chargeEffect.GetComponent<ScaleParticleSystemDuration>();
							if (component2)
							{
								component2.newDuration = this.duration;
							}
						}
						if (this.laserPrefab)
						{
							this.laserEffect = UnityEngine.Object.Instantiate<GameObject>(this.laserPrefab, transform.position, transform.rotation);
							this.laserEffect.transform.parent = transform;
							this.laserLineComponent = this.laserEffect.GetComponent<LineRenderer>();
						}
					}
				}
			}
			if (base.characterBody)
			{
				base.characterBody.SetAimTimer(this.duration);
			}
			this.flashTimer = 0f;
			this.laserOn = true;
		}

		// Token: 0x0600301E RID: 12318 RVA: 0x000CE78D File Offset: 0x000CC98D
		public override void OnExit()
		{
			base.OnExit();
			if (this.chargeEffect)
			{
				EntityState.Destroy(this.chargeEffect);
			}
			if (this.laserEffect)
			{
				EntityState.Destroy(this.laserEffect);
			}
		}

		// Token: 0x0600301F RID: 12319 RVA: 0x000CE7C8 File Offset: 0x000CC9C8
		public override void Update()
		{
			base.Update();
			if (this.laserEffect && this.laserLineComponent)
			{
				float num = 1000f;
				Ray aimRay = base.GetAimRay();
				this.enemyFinder.RefreshCandidates();
				this.lockedOnHurtBox = this.enemyFinder.GetResults().FirstOrDefault<HurtBox>();
				if (this.lockedOnHurtBox)
				{
					aimRay.direction = this.lockedOnHurtBox.transform.position - aimRay.origin;
				}
				Vector3 position = this.laserEffect.transform.parent.position;
				Vector3 point = aimRay.GetPoint(num);
				RaycastHit raycastHit;
				if (Physics.Raycast(aimRay, out raycastHit, num, LayerIndex.world.mask | LayerIndex.defaultLayer.mask))
				{
					point = raycastHit.point;
				}
				this.laserLineComponent.SetPosition(0, position);
				this.laserLineComponent.SetPosition(1, point);
				float num2;
				if (this.duration - base.age > 0.5f)
				{
					num2 = base.age / this.duration;
				}
				else
				{
					this.flashTimer -= Time.deltaTime;
					if (this.flashTimer <= 0f)
					{
						this.laserOn = !this.laserOn;
						this.flashTimer = 0.033333335f;
					}
					num2 = (this.laserOn ? 1f : 0f);
				}
				num2 *= ChargeMegaLaser.laserMaxWidth;
				this.laserLineComponent.startWidth = num2;
				this.laserLineComponent.endWidth = num2;
			}
		}

		// Token: 0x06003020 RID: 12320 RVA: 0x000CE968 File Offset: 0x000CCB68
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				FireMegaLaser nextState = new FireMegaLaser();
				this.outer.SetNextState(nextState);
				return;
			}
		}

		// Token: 0x06003021 RID: 12321 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04002E07 RID: 11783
		public static float baseDuration = 3f;

		// Token: 0x04002E08 RID: 11784
		public static float laserMaxWidth = 0.2f;

		// Token: 0x04002E09 RID: 11785
		[SerializeField]
		public GameObject effectPrefab;

		// Token: 0x04002E0A RID: 11786
		[SerializeField]
		public GameObject laserPrefab;

		// Token: 0x04002E0B RID: 11787
		public static string chargeAttackSoundString;

		// Token: 0x04002E0C RID: 11788
		public static float lockOnAngle;

		// Token: 0x04002E0D RID: 11789
		private HurtBox lockedOnHurtBox;

		// Token: 0x04002E0E RID: 11790
		public float duration;

		// Token: 0x04002E0F RID: 11791
		private GameObject chargeEffect;

		// Token: 0x04002E10 RID: 11792
		private GameObject laserEffect;

		// Token: 0x04002E11 RID: 11793
		private LineRenderer laserLineComponent;

		// Token: 0x04002E12 RID: 11794
		private Vector3 visualEndPosition;

		// Token: 0x04002E13 RID: 11795
		private float flashTimer;

		// Token: 0x04002E14 RID: 11796
		private bool laserOn;

		// Token: 0x04002E15 RID: 11797
		private BullseyeSearch enemyFinder;

		// Token: 0x04002E16 RID: 11798
		private const float originalSoundDuration = 2.1f;
	}
}
