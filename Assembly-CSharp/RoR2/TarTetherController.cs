using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020003F3 RID: 1011
	[RequireComponent(typeof(BezierCurveLine))]
	public class TarTetherController : NetworkBehaviour
	{
		// Token: 0x0600161A RID: 5658 RVA: 0x00069C21 File Offset: 0x00067E21
		private void Awake()
		{
			this.bezierCurveLine = base.GetComponent<BezierCurveLine>();
		}

		// Token: 0x0600161B RID: 5659 RVA: 0x00069C30 File Offset: 0x00067E30
		private void DoDamageTick(bool mulch)
		{
			if (!this.targetHealthComponent)
			{
				this.targetHealthComponent = this.targetRoot.GetComponent<HealthComponent>();
			}
			if (!this.ownerHealthComponent)
			{
				this.ownerHealthComponent = this.ownerRoot.GetComponent<HealthComponent>();
			}
			if (!this.ownerBody)
			{
				this.ownerBody = this.ownerRoot.GetComponent<CharacterBody>();
			}
			if (this.ownerRoot)
			{
				DamageInfo damageInfo = new DamageInfo
				{
					position = this.targetRoot.transform.position,
					attacker = null,
					inflictor = null,
					damage = (mulch ? (this.damageCoefficientPerTick * this.mulchDamageScale) : this.damageCoefficientPerTick) * this.ownerBody.damage,
					damageColorIndex = DamageColorIndex.Default,
					damageType = DamageType.Generic,
					crit = false,
					force = Vector3.zero,
					procChainMask = default(ProcChainMask),
					procCoefficient = 0f
				};
				this.targetHealthComponent.TakeDamage(damageInfo);
				if (!damageInfo.rejected)
				{
					this.ownerHealthComponent.Heal(damageInfo.damage, default(ProcChainMask), true);
				}
				if (!this.targetHealthComponent.alive)
				{
					this.NetworktargetRoot = null;
				}
			}
		}

		// Token: 0x0600161C RID: 5660 RVA: 0x00069D78 File Offset: 0x00067F78
		private Vector3 GetTargetRootPosition()
		{
			if (this.targetRoot)
			{
				Vector3 result = this.targetRoot.transform.position;
				if (this.targetHealthComponent)
				{
					result = this.targetHealthComponent.body.corePosition;
				}
				return result;
			}
			return base.transform.position;
		}

		// Token: 0x0600161D RID: 5661 RVA: 0x00069DD0 File Offset: 0x00067FD0
		private void Update()
		{
			this.age += Time.deltaTime;
			Vector3 position = this.ownerRoot.transform.position;
			if (!this.beginSiphon)
			{
				Vector3 position2 = Vector3.Lerp(position, this.GetTargetRootPosition(), this.age / this.attachTime);
				this.bezierCurveLine.endTransform.position = position2;
				return;
			}
			if (this.targetRoot)
			{
				this.bezierCurveLine.endTransform.position = this.targetRoot.transform.position;
			}
		}

		// Token: 0x0600161E RID: 5662 RVA: 0x00069E64 File Offset: 0x00068064
		private void FixedUpdate()
		{
			this.fixedAge += Time.fixedDeltaTime;
			if (this.targetRoot && this.ownerRoot)
			{
				Vector3 targetRootPosition = this.GetTargetRootPosition();
				if (!this.beginSiphon && this.fixedAge >= this.attachTime)
				{
					this.beginSiphon = true;
					return;
				}
				Vector3 vector = this.ownerRoot.transform.position - targetRootPosition;
				if (NetworkServer.active)
				{
					float sqrMagnitude = vector.sqrMagnitude;
					bool flag = sqrMagnitude < this.mulchDistanceSqr;
					this.tickTimer -= Time.fixedDeltaTime;
					if (this.tickTimer <= 0f)
					{
						this.tickTimer += (flag ? (this.tickInterval * this.mulchTickIntervalScale) : this.tickInterval);
						this.DoDamageTick(flag);
					}
					if (sqrMagnitude > this.breakDistanceSqr)
					{
						UnityEngine.Object.Destroy(base.gameObject);
						return;
					}
				}
				if (Util.HasEffectiveAuthority(this.targetRoot))
				{
					Vector3 b = vector.normalized * (this.reelSpeed * Time.fixedDeltaTime);
					CharacterMotor component = this.targetRoot.GetComponent<CharacterMotor>();
					if (component)
					{
						component.rootMotion += b;
						return;
					}
					Rigidbody component2 = this.targetRoot.GetComponent<Rigidbody>();
					if (component2)
					{
						component2.velocity += b;
						return;
					}
				}
			}
			else if (NetworkServer.active)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		// Token: 0x06001620 RID: 5664 RVA: 0x00004507 File Offset: 0x00002707
		private void UNetVersion()
		{
		}

		// Token: 0x170001FD RID: 509
		// (get) Token: 0x06001621 RID: 5665 RVA: 0x00069FF8 File Offset: 0x000681F8
		// (set) Token: 0x06001622 RID: 5666 RVA: 0x0006A00B File Offset: 0x0006820B
		public GameObject NetworktargetRoot
		{
			get
			{
				return this.targetRoot;
			}
			set
			{
				base.SetSyncVarGameObject(value, ref this.targetRoot, 1u, ref this.___targetRootNetId);
			}
		}

		// Token: 0x170001FE RID: 510
		// (get) Token: 0x06001623 RID: 5667 RVA: 0x0006A028 File Offset: 0x00068228
		// (set) Token: 0x06001624 RID: 5668 RVA: 0x0006A03B File Offset: 0x0006823B
		public GameObject NetworkownerRoot
		{
			get
			{
				return this.ownerRoot;
			}
			set
			{
				base.SetSyncVarGameObject(value, ref this.ownerRoot, 2u, ref this.___ownerRootNetId);
			}
		}

		// Token: 0x06001625 RID: 5669 RVA: 0x0006A058 File Offset: 0x00068258
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write(this.targetRoot);
				writer.Write(this.ownerRoot);
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
				writer.Write(this.targetRoot);
			}
			if ((base.syncVarDirtyBits & 2u) != 0u)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write(this.ownerRoot);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x06001626 RID: 5670 RVA: 0x0006A104 File Offset: 0x00068304
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.___targetRootNetId = reader.ReadNetworkId();
				this.___ownerRootNetId = reader.ReadNetworkId();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.targetRoot = reader.ReadGameObject();
			}
			if ((num & 2) != 0)
			{
				this.ownerRoot = reader.ReadGameObject();
			}
		}

		// Token: 0x06001627 RID: 5671 RVA: 0x0006A16C File Offset: 0x0006836C
		public override void PreStartClient()
		{
			if (!this.___targetRootNetId.IsEmpty())
			{
				this.NetworktargetRoot = ClientScene.FindLocalObject(this.___targetRootNetId);
			}
			if (!this.___ownerRootNetId.IsEmpty())
			{
				this.NetworkownerRoot = ClientScene.FindLocalObject(this.___ownerRootNetId);
			}
		}

		// Token: 0x04001979 RID: 6521
		[SyncVar]
		public GameObject targetRoot;

		// Token: 0x0400197A RID: 6522
		[SyncVar]
		public GameObject ownerRoot;

		// Token: 0x0400197B RID: 6523
		public float reelSpeed = 12f;

		// Token: 0x0400197C RID: 6524
		[NonSerialized]
		public float mulchDistanceSqr;

		// Token: 0x0400197D RID: 6525
		[NonSerialized]
		public float breakDistanceSqr;

		// Token: 0x0400197E RID: 6526
		[NonSerialized]
		public float mulchDamageScale;

		// Token: 0x0400197F RID: 6527
		[NonSerialized]
		public float mulchTickIntervalScale;

		// Token: 0x04001980 RID: 6528
		[NonSerialized]
		public float damageCoefficientPerTick;

		// Token: 0x04001981 RID: 6529
		[NonSerialized]
		public float tickInterval;

		// Token: 0x04001982 RID: 6530
		[NonSerialized]
		public float tickTimer;

		// Token: 0x04001983 RID: 6531
		public float attachTime;

		// Token: 0x04001984 RID: 6532
		private float fixedAge;

		// Token: 0x04001985 RID: 6533
		private float age;

		// Token: 0x04001986 RID: 6534
		private bool beginSiphon;

		// Token: 0x04001987 RID: 6535
		private BezierCurveLine bezierCurveLine;

		// Token: 0x04001988 RID: 6536
		private HealthComponent targetHealthComponent;

		// Token: 0x04001989 RID: 6537
		private HealthComponent ownerHealthComponent;

		// Token: 0x0400198A RID: 6538
		private CharacterBody ownerBody;

		// Token: 0x0400198B RID: 6539
		private NetworkInstanceId ___targetRootNetId;

		// Token: 0x0400198C RID: 6540
		private NetworkInstanceId ___ownerRootNetId;
	}
}
