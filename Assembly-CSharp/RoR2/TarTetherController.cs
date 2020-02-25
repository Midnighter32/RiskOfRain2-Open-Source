using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200034A RID: 842
	[RequireComponent(typeof(BezierCurveLine))]
	public class TarTetherController : NetworkBehaviour
	{
		// Token: 0x0600140F RID: 5135 RVA: 0x00055B7D File Offset: 0x00053D7D
		private void Awake()
		{
			this.bezierCurveLine = base.GetComponent<BezierCurveLine>();
		}

		// Token: 0x06001410 RID: 5136 RVA: 0x00055B8C File Offset: 0x00053D8C
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

		// Token: 0x06001411 RID: 5137 RVA: 0x00055CD4 File Offset: 0x00053ED4
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

		// Token: 0x06001412 RID: 5138 RVA: 0x00055D2C File Offset: 0x00053F2C
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

		// Token: 0x06001413 RID: 5139 RVA: 0x00055DC0 File Offset: 0x00053FC0
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

		// Token: 0x06001415 RID: 5141 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x17000261 RID: 609
		// (get) Token: 0x06001416 RID: 5142 RVA: 0x00055F54 File Offset: 0x00054154
		// (set) Token: 0x06001417 RID: 5143 RVA: 0x00055F67 File Offset: 0x00054167
		public GameObject NetworktargetRoot
		{
			get
			{
				return this.targetRoot;
			}
			[param: In]
			set
			{
				base.SetSyncVarGameObject(value, ref this.targetRoot, 1U, ref this.___targetRootNetId);
			}
		}

		// Token: 0x17000262 RID: 610
		// (get) Token: 0x06001418 RID: 5144 RVA: 0x00055F84 File Offset: 0x00054184
		// (set) Token: 0x06001419 RID: 5145 RVA: 0x00055F97 File Offset: 0x00054197
		public GameObject NetworkownerRoot
		{
			get
			{
				return this.ownerRoot;
			}
			[param: In]
			set
			{
				base.SetSyncVarGameObject(value, ref this.ownerRoot, 2U, ref this.___ownerRootNetId);
			}
		}

		// Token: 0x0600141A RID: 5146 RVA: 0x00055FB4 File Offset: 0x000541B4
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write(this.targetRoot);
				writer.Write(this.ownerRoot);
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
				writer.Write(this.targetRoot);
			}
			if ((base.syncVarDirtyBits & 2U) != 0U)
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

		// Token: 0x0600141B RID: 5147 RVA: 0x00056060 File Offset: 0x00054260
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

		// Token: 0x0600141C RID: 5148 RVA: 0x000560C8 File Offset: 0x000542C8
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

		// Token: 0x040012D7 RID: 4823
		[SyncVar]
		public GameObject targetRoot;

		// Token: 0x040012D8 RID: 4824
		[SyncVar]
		public GameObject ownerRoot;

		// Token: 0x040012D9 RID: 4825
		public float reelSpeed = 12f;

		// Token: 0x040012DA RID: 4826
		[NonSerialized]
		public float mulchDistanceSqr;

		// Token: 0x040012DB RID: 4827
		[NonSerialized]
		public float breakDistanceSqr;

		// Token: 0x040012DC RID: 4828
		[NonSerialized]
		public float mulchDamageScale;

		// Token: 0x040012DD RID: 4829
		[NonSerialized]
		public float mulchTickIntervalScale;

		// Token: 0x040012DE RID: 4830
		[NonSerialized]
		public float damageCoefficientPerTick;

		// Token: 0x040012DF RID: 4831
		[NonSerialized]
		public float tickInterval;

		// Token: 0x040012E0 RID: 4832
		[NonSerialized]
		public float tickTimer;

		// Token: 0x040012E1 RID: 4833
		public float attachTime;

		// Token: 0x040012E2 RID: 4834
		private float fixedAge;

		// Token: 0x040012E3 RID: 4835
		private float age;

		// Token: 0x040012E4 RID: 4836
		private bool beginSiphon;

		// Token: 0x040012E5 RID: 4837
		private BezierCurveLine bezierCurveLine;

		// Token: 0x040012E6 RID: 4838
		private HealthComponent targetHealthComponent;

		// Token: 0x040012E7 RID: 4839
		private HealthComponent ownerHealthComponent;

		// Token: 0x040012E8 RID: 4840
		private CharacterBody ownerBody;

		// Token: 0x040012E9 RID: 4841
		private NetworkInstanceId ___targetRootNetId;

		// Token: 0x040012EA RID: 4842
		private NetworkInstanceId ___ownerRootNetId;
	}
}
