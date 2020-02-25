using System;
using RoR2;
using UnityEngine;

namespace EntityStates.EngiTurret.EngiTurretWeapon
{
	// Token: 0x02000868 RID: 2152
	public class FireBeam : BaseState
	{
		// Token: 0x06003084 RID: 12420 RVA: 0x000D1224 File Offset: 0x000CF424
		public override void OnEnter()
		{
			base.OnEnter();
			Util.PlaySound(this.attackSoundString, base.gameObject);
			this.fireTimer = 0f;
			this.modelTransform = base.GetModelTransform();
			if (this.modelTransform)
			{
				ChildLocator component = this.modelTransform.GetComponent<ChildLocator>();
				if (component)
				{
					Transform transform = component.FindChild(this.muzzleString);
					if (transform && this.laserPrefab)
					{
						this.laserEffectInstance = UnityEngine.Object.Instantiate<GameObject>(this.laserPrefab, transform.position, transform.rotation);
						this.laserEffectInstance.transform.parent = transform;
						this.laserEffectInstanceEndTransform = this.laserEffectInstance.GetComponent<ChildLocator>().FindChild("LaserEnd");
					}
				}
			}
		}

		// Token: 0x06003085 RID: 12421 RVA: 0x000D12EC File Offset: 0x000CF4EC
		public override void OnExit()
		{
			if (this.laserEffectInstance)
			{
				EntityState.Destroy(this.laserEffectInstance);
			}
			base.OnExit();
		}

		// Token: 0x06003086 RID: 12422 RVA: 0x000D130C File Offset: 0x000CF50C
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.laserRay = this.GetLaserRay();
			this.fireTimer += Time.fixedDeltaTime;
			float num = this.fireFrequency * base.characterBody.attackSpeed;
			float num2 = 1f / num;
			if (this.fireTimer > num2)
			{
				this.FireBullet(this.modelTransform, this.laserRay, this.muzzleString);
				this.fireTimer = 0f;
			}
			if (this.laserEffectInstance && this.laserEffectInstanceEndTransform)
			{
				float distance = this.maxDistance;
				Vector3 position = this.laserEffectInstance.transform.parent.position;
				Vector3 point = this.laserRay.GetPoint(distance);
				RaycastHit raycastHit;
				if (Util.CharacterRaycast(base.gameObject, this.laserRay, out raycastHit, distance, LayerIndex.world.mask | LayerIndex.entityPrecise.mask, QueryTriggerInteraction.UseGlobal))
				{
					point = raycastHit.point;
				}
				this.laserEffectInstanceEndTransform.position = point;
			}
			if (base.isAuthority && !this.ShouldFireLaser())
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x06003087 RID: 12423 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x06003088 RID: 12424 RVA: 0x000D1443 File Offset: 0x000CF643
		public virtual void ModifyBullet(BulletAttack bulletAttack)
		{
			bulletAttack.damageType |= DamageType.SlowOnHit;
		}

		// Token: 0x06003089 RID: 12425 RVA: 0x000D1453 File Offset: 0x000CF653
		public virtual bool ShouldFireLaser()
		{
			return base.inputBank && base.inputBank.skill1.down;
		}

		// Token: 0x0600308A RID: 12426 RVA: 0x000D1474 File Offset: 0x000CF674
		public virtual Ray GetLaserRay()
		{
			return base.GetAimRay();
		}

		// Token: 0x0600308B RID: 12427 RVA: 0x000D147C File Offset: 0x000CF67C
		private void FireBullet(Transform modelTransform, Ray laserRay, string targetMuzzle)
		{
			if (this.effectPrefab)
			{
				EffectManager.SimpleMuzzleFlash(this.effectPrefab, base.gameObject, targetMuzzle, false);
			}
			if (base.isAuthority)
			{
				BulletAttack bulletAttack = new BulletAttack();
				bulletAttack.owner = base.gameObject;
				bulletAttack.weapon = base.gameObject;
				bulletAttack.origin = laserRay.origin;
				bulletAttack.aimVector = laserRay.direction;
				bulletAttack.minSpread = this.minSpread;
				bulletAttack.maxSpread = this.maxSpread;
				bulletAttack.bulletCount = 1U;
				bulletAttack.damage = this.damageCoefficient * this.damageStat / this.fireFrequency;
				bulletAttack.procCoefficient = this.procCoefficient / this.fireFrequency;
				bulletAttack.force = this.force;
				bulletAttack.muzzleName = targetMuzzle;
				bulletAttack.hitEffectPrefab = this.hitEffectPrefab;
				bulletAttack.isCrit = Util.CheckRoll(this.critStat, base.characterBody.master);
				bulletAttack.HitEffectNormal = false;
				bulletAttack.radius = 0f;
				bulletAttack.maxDistance = this.maxDistance;
				this.ModifyBullet(bulletAttack);
				bulletAttack.Fire();
			}
		}

		// Token: 0x04002ED4 RID: 11988
		[SerializeField]
		public GameObject effectPrefab;

		// Token: 0x04002ED5 RID: 11989
		[SerializeField]
		public GameObject hitEffectPrefab;

		// Token: 0x04002ED6 RID: 11990
		[SerializeField]
		public GameObject laserPrefab;

		// Token: 0x04002ED7 RID: 11991
		[SerializeField]
		public string muzzleString;

		// Token: 0x04002ED8 RID: 11992
		[SerializeField]
		public string attackSoundString;

		// Token: 0x04002ED9 RID: 11993
		[SerializeField]
		public float damageCoefficient;

		// Token: 0x04002EDA RID: 11994
		[SerializeField]
		public float procCoefficient;

		// Token: 0x04002EDB RID: 11995
		[SerializeField]
		public float force;

		// Token: 0x04002EDC RID: 11996
		[SerializeField]
		public float minSpread;

		// Token: 0x04002EDD RID: 11997
		[SerializeField]
		public float maxSpread;

		// Token: 0x04002EDE RID: 11998
		[SerializeField]
		public int bulletCount;

		// Token: 0x04002EDF RID: 11999
		[SerializeField]
		public float fireFrequency;

		// Token: 0x04002EE0 RID: 12000
		[SerializeField]
		public float maxDistance;

		// Token: 0x04002EE1 RID: 12001
		private float fireTimer;

		// Token: 0x04002EE2 RID: 12002
		private Ray laserRay;

		// Token: 0x04002EE3 RID: 12003
		private Transform modelTransform;

		// Token: 0x04002EE4 RID: 12004
		private GameObject laserEffectInstance;

		// Token: 0x04002EE5 RID: 12005
		private Transform laserEffectInstanceEndTransform;

		// Token: 0x04002EE6 RID: 12006
		public int bulletCountCurrent = 1;
	}
}
