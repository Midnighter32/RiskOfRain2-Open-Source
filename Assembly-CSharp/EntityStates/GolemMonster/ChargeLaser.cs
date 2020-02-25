using System;
using RoR2;
using UnityEngine;

namespace EntityStates.GolemMonster
{
	// Token: 0x0200085C RID: 2140
	public class ChargeLaser : BaseState
	{
		// Token: 0x06003051 RID: 12369 RVA: 0x000CFED0 File Offset: 0x000CE0D0
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

		// Token: 0x06003052 RID: 12370 RVA: 0x000D0020 File Offset: 0x000CE220
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

		// Token: 0x06003053 RID: 12371 RVA: 0x000D0070 File Offset: 0x000CE270
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

		// Token: 0x06003054 RID: 12372 RVA: 0x000D01CC File Offset: 0x000CE3CC
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

		// Token: 0x06003055 RID: 12373 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04002E79 RID: 11897
		public static float baseDuration = 3f;

		// Token: 0x04002E7A RID: 11898
		public static float laserMaxWidth = 0.2f;

		// Token: 0x04002E7B RID: 11899
		public static GameObject effectPrefab;

		// Token: 0x04002E7C RID: 11900
		public static GameObject laserPrefab;

		// Token: 0x04002E7D RID: 11901
		public static string attackSoundString;

		// Token: 0x04002E7E RID: 11902
		private float duration;

		// Token: 0x04002E7F RID: 11903
		private uint chargePlayID;

		// Token: 0x04002E80 RID: 11904
		private GameObject chargeEffect;

		// Token: 0x04002E81 RID: 11905
		private GameObject laserEffect;

		// Token: 0x04002E82 RID: 11906
		private LineRenderer laserLineComponent;

		// Token: 0x04002E83 RID: 11907
		private Vector3 laserDirection;

		// Token: 0x04002E84 RID: 11908
		private Vector3 visualEndPosition;

		// Token: 0x04002E85 RID: 11909
		private float flashTimer;

		// Token: 0x04002E86 RID: 11910
		private bool laserOn;
	}
}
