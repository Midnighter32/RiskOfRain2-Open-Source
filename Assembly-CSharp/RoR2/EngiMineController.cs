using System;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020002E3 RID: 739
	[RequireComponent(typeof(ProjectileStickOnImpact))]
	[RequireComponent(typeof(Deployable))]
	public class EngiMineController : MonoBehaviour
	{
		// Token: 0x06000ECC RID: 3788 RVA: 0x00048C74 File Offset: 0x00046E74
		private void Start()
		{
			this.projectileStick = base.GetComponent<ProjectileStickOnImpact>();
			this.projectileController = base.GetComponent<ProjectileController>();
			this.projectileDamage = base.GetComponent<ProjectileDamage>();
			base.GetComponent<Rigidbody>().AddTorque(UnityEngine.Random.insideUnitSphere * 90f);
			if (NetworkServer.active && this.projectileController.owner)
			{
				CharacterBody component = this.projectileController.owner.GetComponent<CharacterBody>();
				if (component)
				{
					CharacterMaster master = component.master;
					if (master)
					{
						master.AddDeployable(base.GetComponent<Deployable>(), DeployableSlot.EngiMine);
					}
				}
			}
		}

		// Token: 0x06000ECD RID: 3789 RVA: 0x00048D10 File Offset: 0x00046F10
		public void Explode()
		{
			if (NetworkServer.active && this.mineState != EngiMineController.MineState.Exploded)
			{
				this.mineState = EngiMineController.MineState.Exploded;
				new BlastAttack
				{
					procChainMask = this.projectileController.procChainMask,
					procCoefficient = this.projectileController.procCoefficient,
					attacker = this.projectileController.owner,
					inflictor = base.gameObject,
					teamIndex = this.projectileController.teamFilter.teamIndex,
					procCoefficient = this.projectileController.procCoefficient,
					baseDamage = this.projectileDamage.damage,
					baseForce = this.projectileDamage.force,
					falloffModel = BlastAttack.FalloffModel.SweetSpot,
					crit = this.projectileDamage.crit,
					radius = this.explosionRadius,
					position = base.transform.position,
					damageColorIndex = this.projectileDamage.damageColorIndex
				}.Fire();
				if (this.explosionSoundString.Length > 0)
				{
					Util.PlaySound(this.explosionSoundString, base.gameObject);
				}
				if (this.explosionEffectPrefab)
				{
					EffectManager.instance.SpawnEffect(this.explosionEffectPrefab, new EffectData
					{
						origin = base.transform.position,
						rotation = base.transform.rotation,
						scale = this.explosionRadius
					}, true);
				}
				if (this.wardPrefab)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.wardPrefab, base.transform.position, Quaternion.identity);
					BuffWard component = gameObject.GetComponent<BuffWard>();
					component.Networkradius = this.wardRadius;
					TeamFilter component2 = component.GetComponent<TeamFilter>();
					component2.teamIndex = ((component2.teamIndex == TeamIndex.Player) ? TeamIndex.Monster : TeamIndex.Player);
					NetworkServer.Spawn(gameObject);
				}
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		// Token: 0x06000ECE RID: 3790 RVA: 0x00048EE4 File Offset: 0x000470E4
		private void FixedUpdate()
		{
			switch (this.mineState)
			{
			case EngiMineController.MineState.Flying:
				if (NetworkServer.active)
				{
					this.proximityDetector.gameObject.SetActive(false);
				}
				if (this.projectileStick.stuck)
				{
					this.mineState = EngiMineController.MineState.Priming;
					return;
				}
				break;
			case EngiMineController.MineState.Priming:
				if (NetworkServer.active)
				{
					this.proximityDetector.gameObject.SetActive(false);
				}
				this.fixedAge += Time.fixedDeltaTime;
				if (!this.projectileStick.stuck)
				{
					this.mineState = EngiMineController.MineState.Flying;
					this.fixedAge = 0f;
				}
				if (this.fixedAge >= this.primingDelay)
				{
					this.mineState = EngiMineController.MineState.Sticking;
					this.fixedAge = 0f;
					if (this.primingSoundString.Length > 0)
					{
						Util.PlaySound(this.primingSoundString, base.gameObject);
						return;
					}
				}
				break;
			case EngiMineController.MineState.Sticking:
				if (NetworkServer.active)
				{
					this.proximityDetector.gameObject.SetActive(true);
				}
				this.fixedAge += Time.fixedDeltaTime;
				if (!this.projectileStick.stuck)
				{
					this.mineState = EngiMineController.MineState.Flying;
					this.fixedAge = 0f;
					return;
				}
				break;
			case EngiMineController.MineState.PrepForExplosion:
				if (NetworkServer.active)
				{
					this.proximityDetector.gameObject.SetActive(false);
					this.fixedAge += Time.fixedDeltaTime;
					if (this.fixedAge >= this.prepForExplosionDuration)
					{
						this.Explode();
					}
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x06000ECF RID: 3791 RVA: 0x00049058 File Offset: 0x00047258
		public void PrepForExplosion()
		{
			if (this.mineState < EngiMineController.MineState.PrepForExplosion)
			{
				this.mineState = EngiMineController.MineState.PrepForExplosion;
				this.fixedAge = 0f;
				this.prepForExplosionChildEffect.SetActive(true);
				this.projectileStick.enabled = false;
				Rigidbody component = base.GetComponent<Rigidbody>();
				component.isKinematic = false;
				component.collisionDetectionMode = CollisionDetectionMode.Continuous;
				component.AddForce(base.transform.forward * this.detatchForce);
				component.AddTorque(UnityEngine.Random.onUnitSphere * 200f);
			}
		}

		// Token: 0x040012DA RID: 4826
		private ProjectileStickOnImpact projectileStick;

		// Token: 0x040012DB RID: 4827
		public EngiMineController.MineState mineState;

		// Token: 0x040012DC RID: 4828
		private ProjectileController projectileController;

		// Token: 0x040012DD RID: 4829
		private ProjectileDamage projectileDamage;

		// Token: 0x040012DE RID: 4830
		public GameObject impactEffectPrefab;

		// Token: 0x040012DF RID: 4831
		public GameObject explosionEffectPrefab;

		// Token: 0x040012E0 RID: 4832
		public GameObject wardPrefab;

		// Token: 0x040012E1 RID: 4833
		public GameObject prepForExplosionChildEffect;

		// Token: 0x040012E2 RID: 4834
		public float primingDelay = 0.3f;

		// Token: 0x040012E3 RID: 4835
		public float prepForExplosionDuration = 1f;

		// Token: 0x040012E4 RID: 4836
		public float explosionRadius = 6f;

		// Token: 0x040012E5 RID: 4837
		public float detatchForce;

		// Token: 0x040012E6 RID: 4838
		public float wardRadius;

		// Token: 0x040012E7 RID: 4839
		public string primingSoundString;

		// Token: 0x040012E8 RID: 4840
		public string explosionSoundString;

		// Token: 0x040012E9 RID: 4841
		private float fixedAge;

		// Token: 0x040012EA RID: 4842
		public MineProximityDetonator proximityDetector;

		// Token: 0x020002E4 RID: 740
		public enum MineState
		{
			// Token: 0x040012EC RID: 4844
			Flying,
			// Token: 0x040012ED RID: 4845
			Priming,
			// Token: 0x040012EE RID: 4846
			Sticking,
			// Token: 0x040012EF RID: 4847
			PrepForExplosion,
			// Token: 0x040012F0 RID: 4848
			Exploded
		}
	}
}
