using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Drone
{
	// Token: 0x02000897 RID: 2199
	public class DeathState : GenericCharacterDeath
	{
		// Token: 0x06003153 RID: 12627 RVA: 0x000D4498 File Offset: 0x000D2698
		public override void OnEnter()
		{
			base.OnEnter();
			Util.PlaySound(this.initialSoundString, base.gameObject);
			if (base.rigidbodyMotor)
			{
				base.rigidbodyMotor.forcePID.enabled = false;
				base.rigidbodyMotor.rigid.useGravity = true;
				base.rigidbodyMotor.rigid.AddForce(Vector3.up * this.forceAmount, ForceMode.Force);
				base.rigidbodyMotor.rigid.collisionDetectionMode = CollisionDetectionMode.Continuous;
			}
			if (base.rigidbodyDirection)
			{
				base.rigidbodyDirection.enabled = false;
			}
			if (this.initialExplosionEffect)
			{
				EffectManager.SpawnEffect(this.deathExplosionEffect, new EffectData
				{
					origin = base.characterBody.corePosition,
					scale = base.characterBody.radius + this.deathEffectRadius
				}, false);
			}
			if (base.isAuthority && this.destroyOnImpact)
			{
				this.rigidbodyCollisionListener = base.gameObject.AddComponent<DeathState.RigidbodyCollisionListener>();
				this.rigidbodyCollisionListener.deathState = this;
			}
		}

		// Token: 0x06003154 RID: 12628 RVA: 0x000D45AB File Offset: 0x000D27AB
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (NetworkServer.active && base.fixedAge > this.deathDuration)
			{
				this.Explode();
			}
		}

		// Token: 0x06003155 RID: 12629 RVA: 0x000D45CE File Offset: 0x000D27CE
		public void Explode()
		{
			EntityState.Destroy(base.gameObject);
		}

		// Token: 0x06003156 RID: 12630 RVA: 0x000D45DC File Offset: 0x000D27DC
		public virtual void OnImpactServer(Vector3 contactPoint)
		{
			string text = BodyCatalog.GetBodyName(base.characterBody.bodyIndex);
			text = text.Replace("Body", "");
			text = "iscBroken" + text;
			SpawnCard spawnCard = Resources.Load<SpawnCard>("SpawnCards/InteractableSpawnCard/" + text);
			if (spawnCard != null)
			{
				DirectorPlacementRule placementRule = new DirectorPlacementRule
				{
					placementMode = DirectorPlacementRule.PlacementMode.Direct,
					position = contactPoint
				};
				GameObject gameObject = DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(spawnCard, placementRule, new Xoroshiro128Plus(0UL)));
				if (gameObject)
				{
					PurchaseInteraction component = gameObject.GetComponent<PurchaseInteraction>();
					if (component && component.costType == CostTypeIndex.Money)
					{
						component.Networkcost = Run.instance.GetDifficultyScaledCost(component.cost);
					}
				}
			}
		}

		// Token: 0x06003157 RID: 12631 RVA: 0x000D469C File Offset: 0x000D289C
		public override void OnExit()
		{
			if (this.deathExplosionEffect)
			{
				EffectManager.SpawnEffect(this.deathExplosionEffect, new EffectData
				{
					origin = base.characterBody.corePosition,
					scale = base.characterBody.radius + this.deathEffectRadius
				}, false);
			}
			if (this.rigidbodyCollisionListener)
			{
				EntityState.Destroy(this.rigidbodyCollisionListener);
			}
			Util.PlaySound(this.deathSoundString, base.gameObject);
			base.OnExit();
		}

		// Token: 0x04002FA6 RID: 12198
		[SerializeField]
		public GameObject initialExplosionEffect;

		// Token: 0x04002FA7 RID: 12199
		[SerializeField]
		public GameObject deathExplosionEffect;

		// Token: 0x04002FA8 RID: 12200
		[SerializeField]
		public string initialSoundString;

		// Token: 0x04002FA9 RID: 12201
		[SerializeField]
		public string deathSoundString;

		// Token: 0x04002FAA RID: 12202
		[SerializeField]
		public float deathEffectRadius;

		// Token: 0x04002FAB RID: 12203
		[SerializeField]
		public float forceAmount = 20f;

		// Token: 0x04002FAC RID: 12204
		[SerializeField]
		public float deathDuration = 2f;

		// Token: 0x04002FAD RID: 12205
		[SerializeField]
		public bool destroyOnImpact;

		// Token: 0x04002FAE RID: 12206
		private DeathState.RigidbodyCollisionListener rigidbodyCollisionListener;

		// Token: 0x02000898 RID: 2200
		public class RigidbodyCollisionListener : MonoBehaviour
		{
			// Token: 0x06003159 RID: 12633 RVA: 0x000D4740 File Offset: 0x000D2940
			private void OnCollisionEnter(Collision collision)
			{
				this.deathState.OnImpactServer(collision.GetContact(0).point);
				this.deathState.Explode();
			}

			// Token: 0x04002FAF RID: 12207
			public DeathState deathState;
		}
	}
}
