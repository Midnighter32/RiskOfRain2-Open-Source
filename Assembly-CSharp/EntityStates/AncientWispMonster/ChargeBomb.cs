using System;
using RoR2;
using UnityEngine;

namespace EntityStates.AncientWispMonster
{
	// Token: 0x0200072F RID: 1839
	public class ChargeBomb : BaseState
	{
		// Token: 0x06002AB9 RID: 10937 RVA: 0x000B3D58 File Offset: 0x000B1F58
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = ChargeBomb.baseDuration / this.attackSpeedStat;
			base.PlayAnimation("Gesture", "ChargeBomb", "ChargeBomb.playbackRate", this.duration);
			Transform modelTransform = base.GetModelTransform();
			if (modelTransform)
			{
				ChildLocator component = modelTransform.GetComponent<ChildLocator>();
				if (component && ChargeBomb.effectPrefab)
				{
					Transform transform = component.FindChild("MuzzleLeft");
					Transform transform2 = component.FindChild("MuzzleRight");
					if (transform)
					{
						this.chargeEffectLeft = UnityEngine.Object.Instantiate<GameObject>(ChargeBomb.effectPrefab, transform.position, transform.rotation);
						this.chargeEffectLeft.transform.parent = transform;
					}
					if (transform2)
					{
						this.chargeEffectRight = UnityEngine.Object.Instantiate<GameObject>(ChargeBomb.effectPrefab, transform2.position, transform2.rotation);
						this.chargeEffectRight.transform.parent = transform2;
					}
				}
			}
			if (base.characterBody)
			{
				base.characterBody.SetAimTimer(this.duration);
			}
			RaycastHit raycastHit;
			if (Physics.Raycast(base.GetAimRay(), out raycastHit, (float)LayerIndex.world.mask))
			{
				this.startLine = raycastHit.point;
			}
		}

		// Token: 0x06002ABA RID: 10938 RVA: 0x000B3EA0 File Offset: 0x000B20A0
		public override void OnExit()
		{
			base.OnExit();
			EntityState.Destroy(this.chargeEffectLeft);
			EntityState.Destroy(this.chargeEffectRight);
		}

		// Token: 0x06002ABB RID: 10939 RVA: 0x000B02F8 File Offset: 0x000AE4F8
		public override void Update()
		{
			base.Update();
		}

		// Token: 0x06002ABC RID: 10940 RVA: 0x000B3EC0 File Offset: 0x000B20C0
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			float num = 0f;
			if (base.fixedAge >= num && !this.hasFired)
			{
				this.hasFired = true;
				Ray aimRay = base.GetAimRay();
				RaycastHit raycastHit;
				if (Physics.Raycast(aimRay, out raycastHit, (float)LayerIndex.world.mask))
				{
					this.endLine = raycastHit.point;
				}
				Vector3 normalized = (this.endLine - this.startLine).normalized;
				normalized.y = 0f;
				normalized.Normalize();
				for (int i = 0; i < 1; i++)
				{
					Vector3 vector = this.endLine;
					Ray ray = default(Ray);
					ray.origin = aimRay.origin;
					ray.direction = vector - aimRay.origin;
					Debug.DrawLine(ray.origin, vector, Color.red, 5f);
					if (Physics.Raycast(ray, out raycastHit, 500f, LayerIndex.world.mask))
					{
						Vector3 point = raycastHit.point;
						Quaternion rotation = Util.QuaternionSafeLookRotation(raycastHit.normal);
						GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(ChargeBomb.delayPrefab, point, rotation);
						DelayBlast component = gameObject.GetComponent<DelayBlast>();
						component.position = point;
						component.baseDamage = base.characterBody.damage * ChargeBomb.damageCoefficient;
						component.baseForce = 2000f;
						component.bonusForce = Vector3.up * 1000f;
						component.radius = ChargeBomb.radius;
						component.attacker = base.gameObject;
						component.inflictor = null;
						component.crit = Util.CheckRoll(this.critStat, base.characterBody.master);
						component.maxTimer = this.duration;
						gameObject.GetComponent<TeamFilter>().teamIndex = TeamComponent.GetObjectTeam(component.attacker);
						gameObject.transform.localScale = new Vector3(ChargeBomb.radius, ChargeBomb.radius, 1f);
						ScaleParticleSystemDuration component2 = gameObject.GetComponent<ScaleParticleSystemDuration>();
						if (component2)
						{
							component2.newDuration = this.duration;
						}
					}
				}
			}
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextState(new FireBomb());
				return;
			}
		}

		// Token: 0x06002ABD RID: 10941 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04002697 RID: 9879
		public static float baseDuration = 3f;

		// Token: 0x04002698 RID: 9880
		public static GameObject effectPrefab;

		// Token: 0x04002699 RID: 9881
		public static GameObject delayPrefab;

		// Token: 0x0400269A RID: 9882
		public static float radius = 10f;

		// Token: 0x0400269B RID: 9883
		public static float damageCoefficient = 1f;

		// Token: 0x0400269C RID: 9884
		private float duration;

		// Token: 0x0400269D RID: 9885
		private GameObject chargeEffectLeft;

		// Token: 0x0400269E RID: 9886
		private GameObject chargeEffectRight;

		// Token: 0x0400269F RID: 9887
		private Vector3 startLine = Vector3.zero;

		// Token: 0x040026A0 RID: 9888
		private Vector3 endLine = Vector3.zero;

		// Token: 0x040026A1 RID: 9889
		private bool hasFired;
	}
}
