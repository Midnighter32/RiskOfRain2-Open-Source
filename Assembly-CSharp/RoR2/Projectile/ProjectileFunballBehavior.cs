using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x02000506 RID: 1286
	[RequireComponent(typeof(TeamFilter))]
	[RequireComponent(typeof(ProjectileController))]
	public class ProjectileFunballBehavior : NetworkBehaviour
	{
		// Token: 0x06001E80 RID: 7808 RVA: 0x00083BF2 File Offset: 0x00081DF2
		private void Awake()
		{
			this.projectileController = base.GetComponent<ProjectileController>();
		}

		// Token: 0x06001E81 RID: 7809 RVA: 0x00083C00 File Offset: 0x00081E00
		private void Start()
		{
			this.Networktimer = -1f;
		}

		// Token: 0x06001E82 RID: 7810 RVA: 0x00083C10 File Offset: 0x00081E10
		private void FixedUpdate()
		{
			if (NetworkServer.active && this.fuseStarted)
			{
				this.Networktimer = this.timer + Time.fixedDeltaTime;
				if (this.timer >= this.duration)
				{
					EffectManager.SpawnEffect(this.explosionPrefab, new EffectData
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

		// Token: 0x06001E83 RID: 7811 RVA: 0x00083D3C File Offset: 0x00081F3C
		private void OnCollisionEnter(Collision collision)
		{
			this.fuseStarted = true;
		}

		// Token: 0x06001E85 RID: 7813 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x1700033E RID: 830
		// (get) Token: 0x06001E86 RID: 7814 RVA: 0x00083D70 File Offset: 0x00081F70
		// (set) Token: 0x06001E87 RID: 7815 RVA: 0x00083D83 File Offset: 0x00081F83
		public float Networktimer
		{
			get
			{
				return this.timer;
			}
			[param: In]
			set
			{
				base.SetSyncVar<float>(value, ref this.timer, 1U);
			}
		}

		// Token: 0x06001E88 RID: 7816 RVA: 0x00083D98 File Offset: 0x00081F98
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write(this.timer);
				return true;
			}
			bool flag = false;
			if ((base.syncVarDirtyBits & 1U) != 0U)
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

		// Token: 0x06001E89 RID: 7817 RVA: 0x00083E04 File Offset: 0x00082004
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

		// Token: 0x04001BE7 RID: 7143
		[Tooltip("The effect to use for the explosion.")]
		public GameObject explosionPrefab;

		// Token: 0x04001BE8 RID: 7144
		[Tooltip("How many seconds until detonation.")]
		public float duration;

		// Token: 0x04001BE9 RID: 7145
		[Tooltip("Radius of blast in meters.")]
		public float blastRadius = 1f;

		// Token: 0x04001BEA RID: 7146
		[Tooltip("Maximum damage of blast.")]
		public float blastDamage = 1f;

		// Token: 0x04001BEB RID: 7147
		[Tooltip("Force of blast.")]
		public float blastForce = 1f;

		// Token: 0x04001BEC RID: 7148
		private ProjectileController projectileController;

		// Token: 0x04001BED RID: 7149
		[SyncVar]
		private float timer;

		// Token: 0x04001BEE RID: 7150
		private bool fuseStarted;
	}
}
