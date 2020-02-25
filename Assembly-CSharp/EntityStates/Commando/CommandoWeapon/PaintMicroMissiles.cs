using System;
using System.Collections.Generic;
using RoR2;
using UnityEngine;

namespace EntityStates.Commando.CommandoWeapon
{
	// Token: 0x020008C4 RID: 2244
	public class PaintMicroMissiles : BaseState
	{
		// Token: 0x06003252 RID: 12882 RVA: 0x000D9AD2 File Offset: 0x000D7CD2
		public override void OnEnter()
		{
			base.OnEnter();
			this.targetsList = new List<GameObject>();
			this.targetIndicators = new Dictionary<GameObject, GameObject>();
			this.retargetTimer = 0f;
			this.lastTarget = null;
		}

		// Token: 0x06003253 RID: 12883 RVA: 0x000D9B04 File Offset: 0x000D7D04
		public override void OnExit()
		{
			base.OnExit();
			foreach (KeyValuePair<GameObject, GameObject> keyValuePair in this.targetIndicators)
			{
				if (keyValuePair.Value)
				{
					EntityState.Destroy(keyValuePair.Value);
				}
			}
			this.targetIndicators = null;
		}

		// Token: 0x06003254 RID: 12884 RVA: 0x000D9B78 File Offset: 0x000D7D78
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.isAuthority)
			{
				Ray aimRay = base.GetAimRay();
				base.StartAimMode(aimRay, 2f, false);
				if (!base.inputBank || !base.inputBank.skill2.down)
				{
					if (this.targetsList.Count == 0)
					{
						this.outer.SetNextStateToMain();
						return;
					}
					this.outer.SetNextState(new FireMicroMissiles
					{
						targetsList = this.targetsList
					});
					return;
				}
				else
				{
					LayerMask mask = LayerIndex.world.mask | LayerIndex.entityPrecise.mask;
					float maxDistance = 100f;
					GameObject gameObject = null;
					RaycastHit raycastHit;
					if (Physics.Raycast(aimRay, out raycastHit, maxDistance, mask, QueryTriggerInteraction.Ignore))
					{
						HurtBox component = raycastHit.collider.GetComponent<HurtBox>();
						if (component)
						{
							HealthComponent healthComponent = component.healthComponent;
							if (healthComponent)
							{
								gameObject = healthComponent.gameObject;
							}
						}
					}
					int num = 10;
					if (this.targetsList.Count < num)
					{
						this.retargetTimer -= Time.fixedDeltaTime;
						if (this.lastTarget != gameObject || this.retargetTimer <= 0f)
						{
							if (gameObject)
							{
								this.AddTarget(gameObject);
							}
							this.retargetTimer = this.retargetInterval;
							this.lastTarget = gameObject;
						}
					}
				}
			}
		}

		// Token: 0x06003255 RID: 12885 RVA: 0x000D9CE0 File Offset: 0x000D7EE0
		private void AddTarget(GameObject target)
		{
			this.targetsList.Add(target);
			GameObject value = null;
			if (!this.targetIndicators.TryGetValue(target, out value))
			{
				value = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/ShieldTransferIndicator"), target.transform.position, Quaternion.identity, target.transform);
				this.targetIndicators[target] = value;
			}
		}

		// Token: 0x06003256 RID: 12886 RVA: 0x0000BDAE File Offset: 0x00009FAE
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x04003149 RID: 12617
		private List<GameObject> targetsList;

		// Token: 0x0400314A RID: 12618
		private Dictionary<GameObject, GameObject> targetIndicators;

		// Token: 0x0400314B RID: 12619
		private GameObject lastTarget;

		// Token: 0x0400314C RID: 12620
		private float retargetTimer;

		// Token: 0x0400314D RID: 12621
		private float retargetInterval = 0.2f;
	}
}
