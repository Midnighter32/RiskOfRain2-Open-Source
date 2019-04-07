using System;
using RoR2;
using UnityEngine;

namespace EntityStates.GolemMonster
{
	// Token: 0x02000176 RID: 374
	internal class ChargeLaser : BaseState
	{
		// Token: 0x06000736 RID: 1846 RVA: 0x000230D0 File Offset: 0x000212D0
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = ChargeLaser.baseDuration / this.attackSpeedStat;
			Transform modelTransform = base.GetModelTransform();
			this.chargePlayID = Util.PlayScaledSound(ChargeLaser.attackSoundString, base.gameObject, this.attackSpeedStat);
			if (modelTransform)
			{
				ChildLocator component = modelTransform.GetComponent<ChildLocator>();
				if (component)
				{
					Transform transform = component.FindChild("MuzzleLaser");
					if (transform)
					{
						if (ChargeLaser.effectPrefab)
						{
							this.chargeEffect = UnityEngine.Object.Instantiate<GameObject>(ChargeLaser.effectPrefab, transform.position, transform.rotation);
							this.chargeEffect.transform.parent = transform;
							ScaleParticleSystemDuration component2 = this.chargeEffect.GetComponent<ScaleParticleSystemDuration>();
							if (component2)
							{
								component2.newDuration = this.duration;
							}
						}
						if (ChargeLaser.laserPrefab)
						{
							this.laserEffect = UnityEngine.Object.Instantiate<GameObject>(ChargeLaser.laserPrefab, transform.position, transform.rotation);
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

		// Token: 0x06000737 RID: 1847 RVA: 0x00023220 File Offset: 0x00021420
		public override void OnExit()
		{
			AkSoundEngine.StopPlayingID(this.chargePlayID);
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

		// Token: 0x06000738 RID: 1848 RVA: 0x00023270 File Offset: 0x00021470
		public override void Update()
		{
			base.Update();
			if (this.laserEffect && this.laserLineComponent)
			{
				float num = 1000f;
				Ray aimRay = base.GetAimRay();
				Vector3 position = this.laserEffect.transform.parent.position;
				Vector3 point = aimRay.GetPoint(num);
				this.laserDirection = point - position;
				RaycastHit raycastHit;
				if (Physics.Raycast(aimRay, out raycastHit, num, LayerIndex.world.mask | LayerIndex.entityPrecise.mask))
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
				num2 *= ChargeLaser.laserMaxWidth;
				this.laserLineComponent.startWidth = num2;
				this.laserLineComponent.endWidth = num2;
			}
		}

		// Token: 0x06000739 RID: 1849 RVA: 0x000233CC File Offset: 0x000215CC
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				FireLaser fireLaser = new FireLaser();
				fireLaser.laserDirection = this.laserDirection;
				this.outer.SetNextState(fireLaser);
				return;
			}
		}

		// Token: 0x0600073A RID: 1850 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x0400090C RID: 2316
		public static float baseDuration = 3f;

		// Token: 0x0400090D RID: 2317
		public static float laserMaxWidth = 0.2f;

		// Token: 0x0400090E RID: 2318
		public static GameObject effectPrefab;

		// Token: 0x0400090F RID: 2319
		public static GameObject laserPrefab;

		// Token: 0x04000910 RID: 2320
		public static string attackSoundString;

		// Token: 0x04000911 RID: 2321
		private float duration;

		// Token: 0x04000912 RID: 2322
		private uint chargePlayID;

		// Token: 0x04000913 RID: 2323
		private GameObject chargeEffect;

		// Token: 0x04000914 RID: 2324
		private GameObject laserEffect;

		// Token: 0x04000915 RID: 2325
		private LineRenderer laserLineComponent;

		// Token: 0x04000916 RID: 2326
		private Vector3 laserDirection;

		// Token: 0x04000917 RID: 2327
		private Vector3 visualEndPosition;

		// Token: 0x04000918 RID: 2328
		private float flashTimer;

		// Token: 0x04000919 RID: 2329
		private bool laserOn;
	}
}
