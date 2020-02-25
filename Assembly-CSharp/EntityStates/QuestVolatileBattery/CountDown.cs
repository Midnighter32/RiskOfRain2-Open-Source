using System;
using System.Collections.Generic;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.QuestVolatileBattery
{
	// Token: 0x020007A8 RID: 1960
	public class CountDown : QuestVolatileBatteryBaseState
	{
		// Token: 0x06002CCF RID: 11471 RVA: 0x000BCDE4 File Offset: 0x000BAFE4
		public override void OnEnter()
		{
			base.OnEnter();
			if (CountDown.vfxPrefab && base.attachedCharacterModel)
			{
				List<GameObject> equipmentDisplayObjects = base.attachedCharacterModel.GetEquipmentDisplayObjects(EquipmentIndex.QuestVolatileBattery);
				if (equipmentDisplayObjects.Count > 0)
				{
					this.vfxInstances = new GameObject[equipmentDisplayObjects.Count];
					for (int i = 0; i < this.vfxInstances.Length; i++)
					{
						GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(CountDown.vfxPrefab, equipmentDisplayObjects[i].transform);
						gameObject.transform.localPosition = Vector3.zero;
						gameObject.transform.localRotation = Quaternion.identity;
						this.vfxInstances[i] = gameObject;
					}
				}
			}
		}

		// Token: 0x06002CD0 RID: 11472 RVA: 0x000BCE90 File Offset: 0x000BB090
		public override void OnExit()
		{
			GameObject[] array = this.vfxInstances;
			for (int i = 0; i < array.Length; i++)
			{
				EntityState.Destroy(array[i]);
			}
			this.vfxInstances = Array.Empty<GameObject>();
			base.OnExit();
		}

		// Token: 0x06002CD1 RID: 11473 RVA: 0x000BCECB File Offset: 0x000BB0CB
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (NetworkServer.active)
			{
				this.FixedUpdateServer();
			}
		}

		// Token: 0x06002CD2 RID: 11474 RVA: 0x000BCEE0 File Offset: 0x000BB0E0
		private void FixedUpdateServer()
		{
			if (base.fixedAge >= CountDown.duration && !this.detonated)
			{
				this.detonated = true;
				this.Detonate();
			}
		}

		// Token: 0x06002CD3 RID: 11475 RVA: 0x000BCF04 File Offset: 0x000BB104
		public void Detonate()
		{
			if (!base.networkedBodyAttachment.attachedBody)
			{
				return;
			}
			Vector3 corePosition = base.networkedBodyAttachment.attachedBody.corePosition;
			float baseDamage = 0f;
			if (base.attachedHealthComponent)
			{
				baseDamage = base.attachedHealthComponent.fullCombinedHealth * 3f;
			}
			EffectManager.SpawnEffect(CountDown.explosionEffectPrefab, new EffectData
			{
				origin = corePosition,
				scale = CountDown.explosionRadius
			}, true);
			new BlastAttack
			{
				position = corePosition + UnityEngine.Random.onUnitSphere,
				radius = CountDown.explosionRadius,
				falloffModel = BlastAttack.FalloffModel.None,
				attacker = base.networkedBodyAttachment.attachedBodyObject,
				inflictor = base.networkedBodyAttachment.gameObject,
				damageColorIndex = DamageColorIndex.Item,
				baseDamage = baseDamage,
				baseForce = 5000f,
				bonusForce = Vector3.zero,
				canHurtAttacker = true,
				crit = false,
				procChainMask = default(ProcChainMask),
				procCoefficient = 0f,
				teamIndex = base.networkedBodyAttachment.attachedBody.teamComponent.teamIndex
			}.Fire();
			base.networkedBodyAttachment.attachedBody.inventory.SetEquipmentIndex(EquipmentIndex.None);
			this.outer.SetNextState(new Idle());
		}

		// Token: 0x040028E8 RID: 10472
		public static float duration;

		// Token: 0x040028E9 RID: 10473
		public static GameObject vfxPrefab;

		// Token: 0x040028EA RID: 10474
		public static float explosionRadius;

		// Token: 0x040028EB RID: 10475
		public static GameObject explosionEffectPrefab;

		// Token: 0x040028EC RID: 10476
		private GameObject[] vfxInstances = Array.Empty<GameObject>();

		// Token: 0x040028ED RID: 10477
		private bool detonated;
	}
}
