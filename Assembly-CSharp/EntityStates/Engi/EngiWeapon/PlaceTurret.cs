using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Engi.EngiWeapon
{
	// Token: 0x0200088C RID: 2188
	public class PlaceTurret : BaseState
	{
		// Token: 0x06003131 RID: 12593 RVA: 0x000D3C24 File Offset: 0x000D1E24
		public override void OnEnter()
		{
			base.OnEnter();
			if (base.isAuthority)
			{
				this.currentPlacementInfo = this.GetPlacementInfo();
				this.blueprints = UnityEngine.Object.Instantiate<GameObject>(this.blueprintPrefab, this.currentPlacementInfo.position, this.currentPlacementInfo.rotation).GetComponent<BlueprintController>();
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
						this.wristDisplayObject = UnityEngine.Object.Instantiate<GameObject>(this.wristDisplayPrefab, transform);
					}
				}
			}
		}

		// Token: 0x06003132 RID: 12594 RVA: 0x000D3CF0 File Offset: 0x000D1EF0
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
			if (placementInfo.ok)
			{
				float num3 = Mathf.Max(1.82f, 0f);
				if (Physics.CheckCapsule(placementInfo.position + Vector3.up * (num3 - 0.5f), placementInfo.position + Vector3.up * 0.5f, 0.45f, LayerIndex.world.mask | LayerIndex.defaultLayer.mask))
				{
					placementInfo.ok = false;
				}
			}
			return placementInfo;
		}

		// Token: 0x06003133 RID: 12595 RVA: 0x000D3E72 File Offset: 0x000D2072
		private void DestroyBlueprints()
		{
			if (this.blueprints)
			{
				EntityState.Destroy(this.blueprints.gameObject);
				this.blueprints = null;
			}
		}

		// Token: 0x06003134 RID: 12596 RVA: 0x000D3E98 File Offset: 0x000D2098
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

		// Token: 0x06003135 RID: 12597 RVA: 0x000D3ED0 File Offset: 0x000D20D0
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

		// Token: 0x06003136 RID: 12598 RVA: 0x000D3F50 File Offset: 0x000D2150
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
							base.characterBody.SendConstructTurret(base.characterBody, this.currentPlacementInfo.position, this.currentPlacementInfo.rotation, MasterCatalog.FindMasterIndex(this.turretMasterPrefab));
							if (base.skillLocator)
							{
								GenericSkill skill = base.skillLocator.GetSkill(SkillSlot.Special);
								if (skill)
								{
									skill.DeductStock(1);
								}
							}
						}
						Util.PlaySound(this.placeSoundString, base.gameObject);
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

		// Token: 0x06003137 RID: 12599 RVA: 0x0000BDAE File Offset: 0x00009FAE
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x04002F7C RID: 12156
		[SerializeField]
		public GameObject wristDisplayPrefab;

		// Token: 0x04002F7D RID: 12157
		[SerializeField]
		public string placeSoundString;

		// Token: 0x04002F7E RID: 12158
		[SerializeField]
		public GameObject blueprintPrefab;

		// Token: 0x04002F7F RID: 12159
		[SerializeField]
		public GameObject turretMasterPrefab;

		// Token: 0x04002F80 RID: 12160
		private const float placementMaxUp = 1f;

		// Token: 0x04002F81 RID: 12161
		private const float placementMaxDown = 3f;

		// Token: 0x04002F82 RID: 12162
		private const float placementForwardDistance = 2f;

		// Token: 0x04002F83 RID: 12163
		private const float entryDelay = 0.1f;

		// Token: 0x04002F84 RID: 12164
		private const float exitDelay = 0.25f;

		// Token: 0x04002F85 RID: 12165
		private const float turretRadius = 0.5f;

		// Token: 0x04002F86 RID: 12166
		private const float turretHeight = 1.82f;

		// Token: 0x04002F87 RID: 12167
		private const float turretCenter = 0f;

		// Token: 0x04002F88 RID: 12168
		private const float turretModelYOffset = -0.75f;

		// Token: 0x04002F89 RID: 12169
		private GameObject wristDisplayObject;

		// Token: 0x04002F8A RID: 12170
		private BlueprintController blueprints;

		// Token: 0x04002F8B RID: 12171
		private float exitCountdown;

		// Token: 0x04002F8C RID: 12172
		private bool exitPending;

		// Token: 0x04002F8D RID: 12173
		private float entryCountdown;

		// Token: 0x04002F8E RID: 12174
		private PlaceTurret.PlacementInfo currentPlacementInfo;

		// Token: 0x04002F8F RID: 12175
		private bool skill4Released;

		// Token: 0x0200088D RID: 2189
		private struct PlacementInfo
		{
			// Token: 0x04002F90 RID: 12176
			public bool ok;

			// Token: 0x04002F91 RID: 12177
			public Vector3 position;

			// Token: 0x04002F92 RID: 12178
			public Quaternion rotation;
		}
	}
}
