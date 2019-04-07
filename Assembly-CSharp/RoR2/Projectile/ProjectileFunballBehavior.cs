using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x02000548 RID: 1352
	[RequireComponent(typeof(ProjectileController))]
	[RequireComponent(typeof(TeamFilter))]
	public class ProjectileFunballBehavior : NetworkBehaviour
	{
		// Token: 0x06001E2D RID: 7725 RVA: 0x0008E3B6 File Offset: 0x0008C5B6
		private void Awake()
		{
			this.projectileController = base.GetComponent<ProjectileController>();
		}

		// Token: 0x06001E2E RID: 7726 RVA: 0x0008E3C4 File Offset: 0x0008C5C4
		private void Start()
		{
			this.Networktimer = -1f;
		}

		// Token: 0x06001E2F RID: 7727 RVA: 0x0008E3D4 File Offset: 0x0008C5D4
		private void FixedUpdate()
		{
			if (NetworkServer.active && this.fuseStarted)
			{
				this.Networktimer = this.timer + Time.fixedDeltaTime;
				if (this.timer >= this.duration)
				{
					EffectManager.instance.SpawnEffect(this.explosionPrefab, new EffectData
					{
						origin = base.transform.position,
						scale = this.blastRadius
					}, true);
					new BlastAttack
					{
						attacker = this.projectileController.owner,
						inflictor = base.gameObject,
						teamIndex = this.projectileController.teamFilter.teamIndex,
						position = base.transform.position,
						procChainMask = this.projectileController.procChainMask,
						procCoefficient = this.projectileController.procCoefficient,
						radius = this.blastRadius,
						baseDamage = this.blastDamage,
						baseForce = this.blastForce,
						bonusForce = Vector3.zero,
						crit = false,
						damageType = DamageType.Generic
					}.Fire();
					UnityEngine.Object.Destroy(base.gameObject);
				}
			}
		}

		// Token: 0x06001E30 RID: 7728 RVA: 0x0008E504 File Offset: 0x0008C704
		private void OnCollisionEnter(Collision collision)
		{
			this.fuseStarted = true;
		}

		// Token: 0x06001E32 RID: 7730 RVA: 0x00004507 File Offset: 0x00002707
		private void UNetVersion()
		{
		}

		// Token: 0x170002A5 RID: 677
		// (get) Token: 0x06001E33 RID: 7731 RVA: 0x0008E538 File Offset: 0x0008C738
		// (set) Token: 0x06001E34 RID: 7732 RVA: 0x0008E54B File Offset: 0x0008C74B
		public float Networktimer
		{
			get
			{
				return this.timer;
			}
			set
			{
				base.SetSyncVar<float>(value, ref this.timer, 1u);
			}
		}

		// Token: 0x06001E35 RID: 7733 RVA: 0x0008E560 File Offset: 0x0008C760
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write(this.timer);
				return true;
			}
			bool flag = false;
			if ((base.syncVarDirtyBits & 1u) != 0u)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write(this.timer);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x06001E36 RID: 7734 RVA: 0x0008E5CC File Offset: 0x0008C7CC
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.timer = reader.ReadSingle();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.timer = reader.ReadSingle();
			}
		}

		// Token: 0x040020B3 RID: 8371
		[Tooltip("The effect to use for the explosion.")]
		public GameObject explosionPrefab;

		// Token: 0x040020B4 RID: 8372
		[Tooltip("How many seconds until detonation.")]
		public float duration;

		// Token: 0x040020B5 RID: 8373
		[Tooltip("Radius of blast in meters.")]
		public float blastRadius = 1f;

		// Token: 0x040020B6 RID: 8374
		[Tooltip("Maximum damage of blast.")]
		public float blastDamage = 1f;

		// Token: 0x040020B7 RID: 8375
		[Tooltip("Force of blast.")]
		public float blastForce = 1f;

		// Token: 0x040020B8 RID: 8376
		private ProjectileController projectileController;

		// Token: 0x040020B9 RID: 8377
		[SyncVar]
		private float timer;

		// Token: 0x040020BA RID: 8378
		private bool fuseStarted;
	}
}
