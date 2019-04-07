using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Engi.EngiWeapon
{
	// Token: 0x0200018D RID: 397
	internal class PlaceTurret : BaseState
	{
		// Token: 0x060007A7 RID: 1959 RVA: 0x00025DFC File Offset: 0x00023FFC
		public override void OnEnter()
		{
			base.OnEnter();
			if (base.isAuthority)
			{
				this.currentPlacementInfo = this.GetPlacementInfo();
				this.blueprints = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/EngiTurretBlueprints"), this.currentPlacementInfo.position, this.currentPlacementInfo.rotation).GetComponent<BlueprintController>();
			}
			base.PlayAnimation("Gesture", "PrepTurret");
			this.entryCountdown = 0.1f;
			this.exitCountdown = 0.25f;
			this.exitPending = false;
			if (base.modelLocator)
			{
				ChildLocator component = base.modelLocator.modelTransform.GetComponent<ChildLocator>();
				if (component)
				{
					Transform transform = component.FindChild("WristDisplay");
					if (transform)
					{
						this.wristDisplayObject = UnityEngine.Object.Instantiate<GameObject>(PlaceTurret.wristDisplayPrefab, transform);
					}
				}
			}
		}

		// Token: 0x060007A8 RID: 1960 RVA: 0x00025ECC File Offset: 0x000240CC
		private PlaceTurret.PlacementInfo GetPlacementInfo()
		{
			Ray aimRay = base.GetAimRay();
			Vector3 direction = aimRay.direction;
			direction.y = 0f;
			direction.Normalize();
			aimRay.direction = direction;
			PlaceTurret.PlacementInfo placementInfo = default(PlaceTurret.PlacementInfo);
			placementInfo.ok = false;
			placementInfo.rotation = Util.QuaternionSafeLookRotation(-direction);
			Ray ray = new Ray(aimRay.GetPoint(2f) + Vector3.up * 1f, Vector3.down);
			float num = 4f;
			float num2 = num;
			RaycastHit raycastHit;
			if (Physics.SphereCast(ray, 0.5f, out raycastHit, num, LayerIndex.world.mask) && raycastHit.normal.y > 0.5f)
			{
				num2 = raycastHit.distance;
				placementInfo.ok = true;
			}
			Vector3 point = ray.GetPoint(num2 + 0.5f);
			placementInfo.position = point;
			placementInfo.position.y = placementInfo.position.y + 0.91f;
			if (placementInfo.ok)
			{
				float d = Mathf.Max(0.82000005f, 0f) * 0.5f;
				if (Physics.CheckCapsule(placementInfo.position + Vector3.up * d, placementInfo.position + Vector3.down * d, 0.45f, LayerIndex.world.mask | LayerIndex.defaultLayer.mask))
				{
					placementInfo.ok = false;
				}
			}
			return placementInfo;
		}

		// Token: 0x060007A9 RID: 1961 RVA: 0x00026060 File Offset: 0x00024260
		private void DestroyBlueprints()
		{
			if (this.blueprints)
			{
				EntityState.Destroy(this.blueprints.gameObject);
				this.blueprints = null;
			}
		}

		// Token: 0x060007AA RID: 1962 RVA: 0x00026086 File Offset: 0x00024286
		public override void OnExit()
		{
			base.OnExit();
			base.PlayAnimation("Gesture", "PlaceTurret");
			if (this.wristDisplayObject)
			{
				EntityState.Destroy(this.wristDisplayObject);
			}
			this.DestroyBlueprints();
		}

		// Token: 0x060007AB RID: 1963 RVA: 0x000260BC File Offset: 0x000242BC
		public override void Update()
		{
			if (base.inputBank && !base.inputBank.skill4.down)
			{
				this.skill4Released = true;
			}
			base.Update();
			this.currentPlacementInfo = this.GetPlacementInfo();
			if (this.blueprints)
			{
				this.blueprints.PushState(this.currentPlacementInfo.position, this.currentPlacementInfo.rotation, this.currentPlacementInfo.ok);
			}
		}

		// Token: 0x060007AC RID: 1964 RVA: 0x0002613C File Offset: 0x0002433C
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.isAuthority)
			{
				this.entryCountdown -= Time.fixedDeltaTime;
				if (this.exitPending)
				{
					this.exitCountdown -= Time.fixedDeltaTime;
					if (this.exitCountdown <= 0f)
					{
						this.outer.SetNextStateToMain();
						return;
					}
				}
				else if (base.inputBank && this.entryCountdown <= 0f)
				{
					if ((base.inputBank.skill1.down || base.inputBank.skill4.justPressed) && this.currentPlacementInfo.ok)
					{
						if (base.characterBody)
						{
							base.characterBody.SendConstructTurret(base.characterBody, this.currentPlacementInfo.position, this.currentPlacementInfo.rotation);
							if (base.skillLocator)
							{
								GenericSkill skill = base.skillLocator.GetSkill(SkillSlot.Special);
								if (skill)
								{
									skill.DeductStock(1);
								}
							}
						}
						Util.PlaySound(PlaceTurret.placeSoundString, base.gameObject);
						this.DestroyBlueprints();
						this.exitPending = true;
					}
					if (base.inputBank.skill2.justPressed)
					{
						this.DestroyBlueprints();
						this.exitPending = true;
					}
				}
			}
		}

		// Token: 0x060007AD RID: 1965 RVA: 0x0000B306 File Offset: 0x00009506
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x040009E2 RID: 2530
		public static GameObject wristDisplayPrefab;

		// Token: 0x040009E3 RID: 2531
		public static string placeSoundString;

		// Token: 0x040009E4 RID: 2532
		private const float placementMaxUp = 1f;

		// Token: 0x040009E5 RID: 2533
		private const float placementMaxDown = 3f;

		// Token: 0x040009E6 RID: 2534
		private const float placementForwardDistance = 2f;

		// Token: 0x040009E7 RID: 2535
		private const float entryDelay = 0.1f;

		// Token: 0x040009E8 RID: 2536
		private const float exitDelay = 0.25f;

		// Token: 0x040009E9 RID: 2537
		private const float turretRadius = 0.5f;

		// Token: 0x040009EA RID: 2538
		private const float turretHeight = 1.82f;

		// Token: 0x040009EB RID: 2539
		private const float turretCenter = 0f;

		// Token: 0x040009EC RID: 2540
		private const float turretModelYOffset = -0.75f;

		// Token: 0x040009ED RID: 2541
		private GameObject wristDisplayObject;

		// Token: 0x040009EE RID: 2542
		private BlueprintController blueprints;

		// Token: 0x040009EF RID: 2543
		private float exitCountdown;

		// Token: 0x040009F0 RID: 2544
		private bool exitPending;

		// Token: 0x040009F1 RID: 2545
		private float entryCountdown;

		// Token: 0x040009F2 RID: 2546
		private PlaceTurret.PlacementInfo currentPlacementInfo;

		// Token: 0x040009F3 RID: 2547
		private bool skill4Released;

		// Token: 0x0200018E RID: 398
		private struct PlacementInfo
		{
			// Token: 0x040009F4 RID: 2548
			public bool ok;

			// Token: 0x040009F5 RID: 2549
			public Vector3 position;

			// Token: 0x040009F6 RID: 2550
			public Quaternion rotation;
		}
	}
}
