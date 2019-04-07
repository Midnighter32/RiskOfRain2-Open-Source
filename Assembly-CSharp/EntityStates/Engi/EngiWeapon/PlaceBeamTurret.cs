using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Engi.EngiWeapon
{
	// Token: 0x0200018B RID: 395
	internal class PlaceBeamTurret : BaseState
	{
		// Token: 0x0600079F RID: 1951 RVA: 0x000259D8 File Offset: 0x00023BD8
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
						this.wristDisplayObject = UnityEngine.Object.Instantiate<GameObject>(PlaceBeamTurret.wristDisplayPrefab, transform);
					}
				}
			}
		}

		// Token: 0x060007A0 RID: 1952 RVA: 0x00025AA8 File Offset: 0x00023CA8
		private PlaceBeamTurret.PlacementInfo GetPlacementInfo()
		{
			Ray aimRay = base.GetAimRay();
			Vector3 direction = aimRay.direction;
			direction.y = 0f;
			direction.Normalize();
			aimRay.direction = direction;
			PlaceBeamTurret.PlacementInfo placementInfo = default(PlaceBeamTurret.PlacementInfo);
			placementInfo.ok = false;
			placementInfo.rotation = Util.QuaternionSafeLookRotation(-direction);
			RaycastHit raycastHit = default(RaycastHit);
			Ray ray = new Ray(aimRay.GetPoint(2f) + Vector3.up * 1f, Vector3.down);
			float num = 4f;
			float num2 = num;
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

		// Token: 0x060007A1 RID: 1953 RVA: 0x00025C44 File Offset: 0x00023E44
		private void DestroyBlueprints()
		{
			if (this.blueprints)
			{
				EntityState.Destroy(this.blueprints.gameObject);
				this.blueprints = null;
			}
		}

		// Token: 0x060007A2 RID: 1954 RVA: 0x00025C6A File Offset: 0x00023E6A
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

		// Token: 0x060007A3 RID: 1955 RVA: 0x0000DDD0 File Offset: 0x0000BFD0
		public override void Update()
		{
			base.Update();
		}

		// Token: 0x060007A4 RID: 1956 RVA: 0x00025CA0 File Offset: 0x00023EA0
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.currentPlacementInfo = this.GetPlacementInfo();
			if (this.blueprints)
			{
				this.blueprints.PushState(this.currentPlacementInfo.position, this.currentPlacementInfo.rotation, this.currentPlacementInfo.ok);
			}
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
					if (base.inputBank.skill1.down && this.currentPlacementInfo.ok)
					{
						if (base.characterBody)
						{
							base.characterBody.SendConstructTurret(base.characterBody, this.currentPlacementInfo.position, this.currentPlacementInfo.rotation);
						}
						Util.PlaySound(PlaceBeamTurret.placeSoundString, base.gameObject);
						this.DestroyBlueprints();
						this.exitPending = true;
					}
					if (base.inputBank.skill4.justPressed)
					{
						this.DestroyBlueprints();
						this.exitPending = true;
					}
				}
			}
		}

		// Token: 0x060007A5 RID: 1957 RVA: 0x0000B306 File Offset: 0x00009506
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x040009CE RID: 2510
		public static GameObject wristDisplayPrefab;

		// Token: 0x040009CF RID: 2511
		public static string placeSoundString;

		// Token: 0x040009D0 RID: 2512
		private const float placementMaxUp = 1f;

		// Token: 0x040009D1 RID: 2513
		private const float placementMaxDown = 3f;

		// Token: 0x040009D2 RID: 2514
		private const float placementForwardDistance = 2f;

		// Token: 0x040009D3 RID: 2515
		private const float entryDelay = 0.1f;

		// Token: 0x040009D4 RID: 2516
		private const float exitDelay = 0.25f;

		// Token: 0x040009D5 RID: 2517
		private const float turretRadius = 0.5f;

		// Token: 0x040009D6 RID: 2518
		private const float turretHeight = 1.82f;

		// Token: 0x040009D7 RID: 2519
		private const float turretCenter = 0f;

		// Token: 0x040009D8 RID: 2520
		private const float turretModelYOffset = -0.75f;

		// Token: 0x040009D9 RID: 2521
		private GameObject wristDisplayObject;

		// Token: 0x040009DA RID: 2522
		private BlueprintController blueprints;

		// Token: 0x040009DB RID: 2523
		private float exitCountdown;

		// Token: 0x040009DC RID: 2524
		private bool exitPending;

		// Token: 0x040009DD RID: 2525
		private float entryCountdown;

		// Token: 0x040009DE RID: 2526
		private PlaceBeamTurret.PlacementInfo currentPlacementInfo;

		// Token: 0x0200018C RID: 396
		private struct PlacementInfo
		{
			// Token: 0x040009DF RID: 2527
			public bool ok;

			// Token: 0x040009E0 RID: 2528
			public Vector3 position;

			// Token: 0x040009E1 RID: 2529
			public Quaternion rotation;
		}
	}
}
