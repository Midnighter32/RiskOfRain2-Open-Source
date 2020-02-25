using System;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000214 RID: 532
	public class GhostGunController : MonoBehaviour
	{
		// Token: 0x06000BB1 RID: 2993 RVA: 0x00032B6B File Offset: 0x00030D6B
		private void Start()
		{
			this.fireTimer = 0f;
			this.ammo = 6;
			this.kills = 0;
			this.timeoutTimer = this.timeout;
		}

		// Token: 0x06000BB2 RID: 2994 RVA: 0x00032B94 File Offset: 0x00030D94
		private void Fire(Vector3 origin, Vector3 aimDirection)
		{
			CharacterBody component = this.owner.GetComponent<CharacterBody>();
			int killCount = component.killCount;
			new BulletAttack
			{
				aimVector = aimDirection,
				bulletCount = 1U,
				damage = this.CalcDamage(),
				force = 2400f,
				maxSpread = 0f,
				minSpread = 0f,
				muzzleName = "muzzle",
				origin = origin,
				owner = this.owner,
				procCoefficient = 0f,
				tracerEffectPrefab = Resources.Load<GameObject>("Prefabs/Effects/Tracers/TracerSmokeChase"),
				hitEffectPrefab = Resources.Load<GameObject>("Prefabs/Effects/ImpactEffects/Hitspark1"),
				damageColorIndex = DamageColorIndex.Item
			}.Fire();
			this.kills += component.killCount - killCount;
		}

		// Token: 0x06000BB3 RID: 2995 RVA: 0x00032C60 File Offset: 0x00030E60
		private float CalcDamage()
		{
			float damage = this.owner.GetComponent<CharacterBody>().damage;
			return 5f * Mathf.Pow(2f, (float)this.kills) * damage;
		}

		// Token: 0x06000BB4 RID: 2996 RVA: 0x00032C98 File Offset: 0x00030E98
		private bool HasLoS(GameObject target)
		{
			Ray ray = new Ray(base.transform.position, target.transform.position - base.transform.position);
			RaycastHit raycastHit = default(RaycastHit);
			return !Physics.Raycast(ray, out raycastHit, this.maxRange, LayerIndex.defaultLayer.mask | LayerIndex.world.mask, QueryTriggerInteraction.Ignore) || raycastHit.collider.gameObject == target;
		}

		// Token: 0x06000BB5 RID: 2997 RVA: 0x00032D24 File Offset: 0x00030F24
		private bool WillHit(GameObject target)
		{
			Ray ray = new Ray(base.transform.position, base.transform.forward);
			RaycastHit raycastHit = default(RaycastHit);
			if (Physics.Raycast(ray, out raycastHit, this.maxRange, LayerIndex.entityPrecise.mask | LayerIndex.world.mask, QueryTriggerInteraction.Ignore))
			{
				HurtBox component = raycastHit.collider.GetComponent<HurtBox>();
				if (component)
				{
					HealthComponent healthComponent = component.healthComponent;
					if (healthComponent)
					{
						return healthComponent.gameObject == target;
					}
				}
			}
			return false;
		}

		// Token: 0x06000BB6 RID: 2998 RVA: 0x00032DBC File Offset: 0x00030FBC
		private GameObject FindTarget()
		{
			TeamIndex teamA = TeamIndex.Neutral;
			TeamComponent component = this.owner.GetComponent<TeamComponent>();
			if (component)
			{
				teamA = component.teamIndex;
			}
			Vector3 position = base.transform.position;
			float num = this.CalcDamage();
			float num2 = this.maxRange * this.maxRange;
			GameObject gameObject = null;
			GameObject result = null;
			float num3 = 0f;
			float num4 = float.PositiveInfinity;
			for (TeamIndex teamIndex = TeamIndex.Neutral; teamIndex < TeamIndex.Count; teamIndex += 1)
			{
				if (TeamManager.IsTeamEnemy(teamA, teamIndex))
				{
					ReadOnlyCollection<TeamComponent> teamMembers = TeamComponent.GetTeamMembers(teamIndex);
					for (int i = 0; i < teamMembers.Count; i++)
					{
						GameObject gameObject2 = teamMembers[i].gameObject;
						if ((gameObject2.transform.position - position).sqrMagnitude <= num2)
						{
							HealthComponent component2 = teamMembers[i].GetComponent<HealthComponent>();
							if (component2)
							{
								if (component2.health <= num)
								{
									if (component2.health > num3 && this.HasLoS(gameObject2))
									{
										gameObject = gameObject2;
										num3 = component2.health;
									}
								}
								else if (component2.health < num4 && this.HasLoS(gameObject2))
								{
									result = gameObject2;
									num4 = component2.health;
								}
							}
						}
					}
				}
			}
			if (!gameObject)
			{
				return result;
			}
			return gameObject;
		}

		// Token: 0x06000BB7 RID: 2999 RVA: 0x00032F0C File Offset: 0x0003110C
		private void FixedUpdate()
		{
			if (!NetworkServer.active)
			{
				return;
			}
			if (!this.owner)
			{
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			InputBankTest component = this.owner.GetComponent<InputBankTest>();
			Vector3 vector = component ? component.aimDirection : base.transform.forward;
			if (this.target)
			{
				vector = (this.target.transform.position - base.transform.position).normalized;
			}
			base.transform.forward = Vector3.RotateTowards(base.transform.forward, vector, 0.017453292f * this.turnSpeed * Time.fixedDeltaTime, 0f);
			Vector3 vector2 = this.owner.transform.position + base.transform.rotation * this.localOffset;
			base.transform.position = Vector3.SmoothDamp(base.transform.position, vector2, ref this.velocity, this.positionSmoothTime, float.PositiveInfinity, Time.fixedDeltaTime);
			this.fireTimer -= Time.fixedDeltaTime;
			this.timeoutTimer -= Time.fixedDeltaTime;
			if (this.fireTimer <= 0f)
			{
				this.target = this.FindTarget();
				this.fireTimer = this.interval;
			}
			if (this.target && this.WillHit(this.target))
			{
				Vector3 normalized = (this.target.transform.position - base.transform.position).normalized;
				this.Fire(base.transform.position, normalized);
				this.ammo--;
				this.target = null;
				this.timeoutTimer = this.timeout;
			}
			if (this.ammo <= 0 || this.timeoutTimer <= 0f)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		// Token: 0x04000BC6 RID: 3014
		public GameObject owner;

		// Token: 0x04000BC7 RID: 3015
		public float interval;

		// Token: 0x04000BC8 RID: 3016
		public float maxRange = 20f;

		// Token: 0x04000BC9 RID: 3017
		public float turnSpeed = 180f;

		// Token: 0x04000BCA RID: 3018
		public Vector3 localOffset = Vector3.zero;

		// Token: 0x04000BCB RID: 3019
		public float positionSmoothTime = 0.05f;

		// Token: 0x04000BCC RID: 3020
		public float timeout = 2f;

		// Token: 0x04000BCD RID: 3021
		private float fireTimer;

		// Token: 0x04000BCE RID: 3022
		private float timeoutTimer;

		// Token: 0x04000BCF RID: 3023
		private int ammo;

		// Token: 0x04000BD0 RID: 3024
		private int kills;

		// Token: 0x04000BD1 RID: 3025
		private GameObject target;

		// Token: 0x04000BD2 RID: 3026
		private Vector3 velocity;
	}
}
