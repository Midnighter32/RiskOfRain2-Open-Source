using System;
using System.Collections.Generic;
using RoR2;
using RoR2.Orbs;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Treebot.TreebotFlower
{
	// Token: 0x02000759 RID: 1881
	public class TreebotFlower2Projectile : BaseState
	{
		// Token: 0x06002B83 RID: 11139 RVA: 0x000B7804 File Offset: 0x000B5A04
		public override void OnEnter()
		{
			base.OnEnter();
			ProjectileController component = base.GetComponent<ProjectileController>();
			if (component)
			{
				this.owner = component.owner;
				this.procChainMask = component.procChainMask;
				this.procCoefficient = component.procCoefficient;
				this.teamIndex = component.teamFilter.teamIndex;
			}
			ProjectileDamage component2 = base.GetComponent<ProjectileDamage>();
			if (component2)
			{
				this.damage = component2.damage;
				this.damageType = component2.damageType;
				this.crit = component2.crit;
			}
			if (NetworkServer.active)
			{
				this.rootedBodies = new List<CharacterBody>();
			}
			base.PlayAnimation("Base", "SpawnToIdle");
			Util.PlaySound(TreebotFlower2Projectile.enterSoundString, base.gameObject);
			if (TreebotFlower2Projectile.enterEffectPrefab)
			{
				EffectManager.SimpleEffect(TreebotFlower2Projectile.enterEffectPrefab, base.transform.position, base.transform.rotation, false);
			}
			ChildLocator component3 = base.GetModelTransform().GetComponent<ChildLocator>();
			if (component3)
			{
				Transform transform = component3.FindChild("AreaIndicator");
				transform.localScale = new Vector3(TreebotFlower2Projectile.radius, TreebotFlower2Projectile.radius, TreebotFlower2Projectile.radius);
				transform.gameObject.SetActive(true);
			}
		}

		// Token: 0x06002B84 RID: 11140 RVA: 0x000B7934 File Offset: 0x000B5B34
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (NetworkServer.active)
			{
				this.rootPulseTimer -= Time.fixedDeltaTime;
				this.healTimer -= Time.fixedDeltaTime;
				if (this.rootPulseTimer <= 0f)
				{
					this.rootPulseTimer += TreebotFlower2Projectile.duration / TreebotFlower2Projectile.rootPulseCount;
					this.RootPulse();
				}
				if (this.healTimer <= 0f)
				{
					this.healTimer += TreebotFlower2Projectile.duration / TreebotFlower2Projectile.healPulseCount;
					this.HealPulse();
				}
				if (base.fixedAge >= TreebotFlower2Projectile.duration)
				{
					EntityState.Destroy(base.gameObject);
					return;
				}
			}
		}

		// Token: 0x06002B85 RID: 11141 RVA: 0x000B79E4 File Offset: 0x000B5BE4
		private void RootPulse()
		{
			Vector3 position = base.transform.position;
			foreach (HurtBox hurtBox in new SphereSearch
			{
				origin = position,
				radius = TreebotFlower2Projectile.radius,
				mask = LayerIndex.entityPrecise.mask
			}.RefreshCandidates().FilterCandidatesByHurtBoxTeam(TeamMask.AllExcept(this.teamIndex)).OrderCandidatesByDistance().FilterCandidatesByDistinctHurtBoxEntities().GetHurtBoxes())
			{
				CharacterBody body = hurtBox.healthComponent.body;
				if (!this.rootedBodies.Contains(body))
				{
					this.rootedBodies.Add(body);
					body.AddBuff(BuffIndex.Entangle);
					body.RecalculateStats();
					Vector3 a = hurtBox.transform.position - position;
					float magnitude = a.magnitude;
					Vector3 a2 = a / magnitude;
					Rigidbody component = hurtBox.healthComponent.GetComponent<Rigidbody>();
					float num = component ? component.mass : 1f;
					float num2 = magnitude - TreebotFlower2Projectile.yankIdealDistance;
					float num3 = TreebotFlower2Projectile.yankSuitabilityCurve.Evaluate(num);
					Vector3 vector = component ? component.velocity : Vector3.zero;
					if (HGMath.IsVectorNaN(vector))
					{
						vector = Vector3.zero;
					}
					Vector3 a3 = -vector;
					if (num2 > 0f)
					{
						a3 = a2 * -Trajectory.CalculateInitialYSpeedForHeight(num2, -body.acceleration);
					}
					Vector3 force = a3 * (num * num3);
					DamageInfo damageInfo = new DamageInfo
					{
						attacker = this.owner,
						inflictor = base.gameObject,
						crit = this.crit,
						damage = this.damage,
						damageColorIndex = DamageColorIndex.Default,
						damageType = this.damageType,
						force = force,
						position = hurtBox.transform.position,
						procChainMask = this.procChainMask,
						procCoefficient = this.procCoefficient
					};
					hurtBox.healthComponent.TakeDamage(damageInfo);
					HurtBox hurtBoxReference = hurtBox;
					HurtBoxGroup hurtBoxGroup = hurtBox.hurtBoxGroup;
					int num4 = 0;
					while ((float)num4 < Mathf.Min(4f, body.radius * 2f))
					{
						EffectData effectData = new EffectData
						{
							scale = 1f,
							origin = position,
							genericFloat = Mathf.Max(0.2f, TreebotFlower2Projectile.duration - base.fixedAge)
						};
						effectData.SetHurtBoxReference(hurtBoxReference);
						EffectManager.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/OrbEffects/EntangleOrbEffect"), effectData, true);
						hurtBoxReference = hurtBoxGroup.hurtBoxes[UnityEngine.Random.Range(0, hurtBoxGroup.hurtBoxes.Length)];
						num4++;
					}
				}
			}
		}

		// Token: 0x06002B86 RID: 11142 RVA: 0x000B7C98 File Offset: 0x000B5E98
		public override void OnExit()
		{
			if (this.rootedBodies != null)
			{
				foreach (CharacterBody characterBody in this.rootedBodies)
				{
					characterBody.RemoveBuff(BuffIndex.Entangle);
				}
				this.rootedBodies = null;
			}
			Util.PlaySound(TreebotFlower2Projectile.exitSoundString, base.gameObject);
			if (TreebotFlower2Projectile.exitEffectPrefab)
			{
				EffectManager.SimpleEffect(TreebotFlower2Projectile.exitEffectPrefab, base.transform.position, base.transform.rotation, false);
			}
			base.OnExit();
		}

		// Token: 0x06002B87 RID: 11143 RVA: 0x000B7D40 File Offset: 0x000B5F40
		private void HealPulse()
		{
			HealthComponent healthComponent = this.owner ? this.owner.GetComponent<HealthComponent>() : null;
			if (healthComponent && this.rootedBodies.Count > 0)
			{
				float num = 1f / TreebotFlower2Projectile.healPulseCount;
				HealOrb healOrb = new HealOrb();
				healOrb.origin = base.transform.position;
				healOrb.target = healthComponent.body.mainHurtBox;
				healOrb.healValue = num * TreebotFlower2Projectile.healthFractionYieldPerHit * healthComponent.fullHealth * (float)this.rootedBodies.Count;
				healOrb.overrideDuration = 0.3f;
				OrbManager.instance.AddOrb(healOrb);
			}
		}

		// Token: 0x04002789 RID: 10121
		public static float yankIdealDistance;

		// Token: 0x0400278A RID: 10122
		public static AnimationCurve yankSuitabilityCurve;

		// Token: 0x0400278B RID: 10123
		public static float healthFractionYieldPerHit;

		// Token: 0x0400278C RID: 10124
		public static float radius;

		// Token: 0x0400278D RID: 10125
		public static float healPulseCount;

		// Token: 0x0400278E RID: 10126
		public static float duration;

		// Token: 0x0400278F RID: 10127
		public static float rootPulseCount;

		// Token: 0x04002790 RID: 10128
		public static string enterSoundString;

		// Token: 0x04002791 RID: 10129
		public static string exitSoundString;

		// Token: 0x04002792 RID: 10130
		public static GameObject enterEffectPrefab;

		// Token: 0x04002793 RID: 10131
		public static GameObject exitEffectPrefab;

		// Token: 0x04002794 RID: 10132
		private List<CharacterBody> rootedBodies;

		// Token: 0x04002795 RID: 10133
		private float healTimer;

		// Token: 0x04002796 RID: 10134
		private float rootPulseTimer;

		// Token: 0x04002797 RID: 10135
		private GameObject owner;

		// Token: 0x04002798 RID: 10136
		private ProcChainMask procChainMask;

		// Token: 0x04002799 RID: 10137
		private float procCoefficient;

		// Token: 0x0400279A RID: 10138
		private TeamIndex teamIndex = TeamIndex.None;

		// Token: 0x0400279B RID: 10139
		private float damage;

		// Token: 0x0400279C RID: 10140
		private DamageType damageType;

		// Token: 0x0400279D RID: 10141
		private bool crit;

		// Token: 0x0400279E RID: 10142
		private float healPulseHealthFractionValue;
	}
}
