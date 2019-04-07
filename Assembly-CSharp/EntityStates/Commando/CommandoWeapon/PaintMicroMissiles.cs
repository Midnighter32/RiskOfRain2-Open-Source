using System;
using System.Collections.Generic;
using RoR2;
using UnityEngine;

namespace EntityStates.Commando.CommandoWeapon
{
	// Token: 0x020001AF RID: 431
	internal class PaintMicroMissiles : BaseState
	{
		// Token: 0x06000870 RID: 2160 RVA: 0x0002A552 File Offset: 0x00028752
		public override void OnEnter()
		{
			base.OnEnter();
			this.targetsList = new List<GameObject>();
			this.targetIndicators = new Dictionary<GameObject, GameObject>();
			this.retargetTimer = 0f;
			this.lastTarget = null;
		}

		// Token: 0x06000871 RID: 2161 RVA: 0x0002A584 File Offset: 0x00028784
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

		// Token: 0x06000872 RID: 2162 RVA: 0x0002A5F8 File Offset: 0x000287F8
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

		// Token: 0x06000873 RID: 2163 RVA: 0x0002A760 File Offset: 0x00028960
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

		// Token: 0x06000874 RID: 2164 RVA: 0x0000B306 File Offset: 0x00009506
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x04000B48 RID: 2888
		private List<GameObject> targetsList;

		// Token: 0x04000B49 RID: 2889
		private Dictionary<GameObject, GameObject> targetIndicators;

		// Token: 0x04000B4A RID: 2890
		private GameObject lastTarget;

		// Token: 0x04000B4B RID: 2891
		private float retargetTimer;

		// Token: 0x04000B4C RID: 2892
		private float retargetInterval = 0.2f;
	}
}
