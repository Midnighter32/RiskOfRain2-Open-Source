using System;
using RoR2;
using UnityEngine;

namespace EntityStates.AncientWispMonster
{
	// Token: 0x020000D0 RID: 208
	internal class ChargeBomb : BaseState
	{
		// Token: 0x06000410 RID: 1040 RVA: 0x00010D0C File Offset: 0x0000EF0C
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

		// Token: 0x06000411 RID: 1041 RVA: 0x00010E54 File Offset: 0x0000F054
		public override void OnExit()
		{
			base.OnExit();
			EntityState.Destroy(this.chargeEffectLeft);
			EntityState.Destroy(this.chargeEffectRight);
		}

		// Token: 0x06000412 RID: 1042 RVA: 0x0000DDD0 File Offset: 0x0000BFD0
		public override void Update()
		{
			base.Update();
		}

		// Token: 0x06000413 RID: 1043 RVA: 0x00010E74 File Offset: 0x0000F074
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

		// Token: 0x06000414 RID: 1044 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x040003D1 RID: 977
		public static float baseDuration = 3f;

		// Token: 0x040003D2 RID: 978
		public static GameObject effectPrefab;

		// Token: 0x040003D3 RID: 979
		public static GameObject delayPrefab;

		// Token: 0x040003D4 RID: 980
		public static float radius = 10f;

		// Token: 0x040003D5 RID: 981
		public static float damageCoefficient = 1f;

		// Token: 0x040003D6 RID: 982
		private float duration;

		// Token: 0x040003D7 RID: 983
		private GameObject chargeEffectLeft;

		// Token: 0x040003D8 RID: 984
		private GameObject chargeEffectRight;

		// Token: 0x040003D9 RID: 985
		private Vector3 startLine = Vector3.zero;

		// Token: 0x040003DA RID: 986
		private Vector3 endLine = Vector3.zero;

		// Token: 0x040003DB RID: 987
		private bool hasFired;
	}
}
