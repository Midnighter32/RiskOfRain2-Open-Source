using System;
using System.Linq;
using RoR2;
using UnityEngine;

namespace EntityStates.TitanMonster
{
	// Token: 0x0200016A RID: 362
	internal class ChargeMegaLaser : BaseState
	{
		// Token: 0x06000702 RID: 1794 RVA: 0x00021798 File Offset: 0x0001F998
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

		// Token: 0x06000703 RID: 1795 RVA: 0x00021995 File Offset: 0x0001FB95
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

		// Token: 0x06000704 RID: 1796 RVA: 0x000219D0 File Offset: 0x0001FBD0
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

		// Token: 0x06000705 RID: 1797 RVA: 0x00021B70 File Offset: 0x0001FD70
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

		// Token: 0x06000706 RID: 1798 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x0400089B RID: 2203
		public static float baseDuration = 3f;

		// Token: 0x0400089C RID: 2204
		public static float laserMaxWidth = 0.2f;

		// Token: 0x0400089D RID: 2205
		[SerializeField]
		public GameObject effectPrefab;

		// Token: 0x0400089E RID: 2206
		[SerializeField]
		public GameObject laserPrefab;

		// Token: 0x0400089F RID: 2207
		public static string chargeAttackSoundString;

		// Token: 0x040008A0 RID: 2208
		public static float lockOnAngle;

		// Token: 0x040008A1 RID: 2209
		private HurtBox lockedOnHurtBox;

		// Token: 0x040008A2 RID: 2210
		public float duration;

		// Token: 0x040008A3 RID: 2211
		private GameObject chargeEffect;

		// Token: 0x040008A4 RID: 2212
		private GameObject laserEffect;

		// Token: 0x040008A5 RID: 2213
		private LineRenderer laserLineComponent;

		// Token: 0x040008A6 RID: 2214
		private Vector3 visualEndPosition;

		// Token: 0x040008A7 RID: 2215
		private float flashTimer;

		// Token: 0x040008A8 RID: 2216
		private bool laserOn;

		// Token: 0x040008A9 RID: 2217
		private BullseyeSearch enemyFinder;

		// Token: 0x040008AA RID: 2218
		private const float originalSoundDuration = 2.1f;
	}
}
