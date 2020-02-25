using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200021E RID: 542
	[RequireComponent(typeof(GenericOwnership))]
	public class HealBeamController : NetworkBehaviour
	{
		// Token: 0x17000181 RID: 385
		// (get) Token: 0x06000BF8 RID: 3064 RVA: 0x0003589A File Offset: 0x00033A9A
		// (set) Token: 0x06000BF9 RID: 3065 RVA: 0x000358A2 File Offset: 0x00033AA2
		public GenericOwnership ownership { get; private set; }

		// Token: 0x17000182 RID: 386
		// (get) Token: 0x06000BFA RID: 3066 RVA: 0x000358AB File Offset: 0x00033AAB
		// (set) Token: 0x06000BFB RID: 3067 RVA: 0x000358B3 File Offset: 0x00033AB3
		public HurtBox target
		{
			get
			{
				return this.cachedHurtBox;
			}
			[Server]
			set
			{
				if (!NetworkServer.active)
				{
					Debug.LogWarning("[Server] function 'System.Void RoR2.HealBeamController::set_target(RoR2.HurtBox)' called on client");
					return;
				}
				this.NetworknetTarget = HurtBoxReference.FromHurtBox(value);
				this.UpdateCachedHurtBox();
			}
		}

		// Token: 0x17000183 RID: 387
		// (get) Token: 0x06000BFC RID: 3068 RVA: 0x000358DC File Offset: 0x00033ADC
		// (set) Token: 0x06000BFD RID: 3069 RVA: 0x000358E4 File Offset: 0x00033AE4
		public float healRate { get; set; }

		// Token: 0x06000BFE RID: 3070 RVA: 0x000358ED File Offset: 0x00033AED
		private void Awake()
		{
			this.ownership = base.GetComponent<GenericOwnership>();
			this.startPointTransform.SetParent(null, true);
			this.endPointTransform.SetParent(null, true);
		}

		// Token: 0x06000BFF RID: 3071 RVA: 0x00035915 File Offset: 0x00033B15
		public override void OnStartClient()
		{
			base.OnStartClient();
			this.UpdateCachedHurtBox();
		}

		// Token: 0x06000C00 RID: 3072 RVA: 0x00035923 File Offset: 0x00033B23
		private void OnDestroy()
		{
			if (this.startPointTransform)
			{
				UnityEngine.Object.Destroy(this.startPointTransform.gameObject);
			}
			if (this.endPointTransform)
			{
				UnityEngine.Object.Destroy(this.endPointTransform.gameObject);
			}
		}

		// Token: 0x06000C01 RID: 3073 RVA: 0x0003595F File Offset: 0x00033B5F
		private void OnEnable()
		{
			InstanceTracker.Add<HealBeamController>(this);
		}

		// Token: 0x06000C02 RID: 3074 RVA: 0x00035967 File Offset: 0x00033B67
		private void OnDisable()
		{
			InstanceTracker.Remove<HealBeamController>(this);
		}

		// Token: 0x06000C03 RID: 3075 RVA: 0x0003596F File Offset: 0x00033B6F
		private void LateUpdate()
		{
			this.UpdateHealBeamVisuals();
		}

		// Token: 0x06000C04 RID: 3076 RVA: 0x00035977 File Offset: 0x00033B77
		private void OnSyncTarget(HurtBoxReference newValue)
		{
			this.NetworknetTarget = newValue;
			this.UpdateCachedHurtBox();
		}

		// Token: 0x06000C05 RID: 3077 RVA: 0x00035986 File Offset: 0x00033B86
		private void FixedUpdate()
		{
			if (NetworkServer.active)
			{
				this.FixedUpdateServer();
			}
		}

		// Token: 0x06000C06 RID: 3078 RVA: 0x00035998 File Offset: 0x00033B98
		private void FixedUpdateServer()
		{
			if (!this.cachedHurtBox)
			{
				this.BreakServer();
				return;
			}
			if (this.tickInterval > 0f)
			{
				this.stopwatchServer += Time.fixedDeltaTime;
				while (this.stopwatchServer >= this.tickInterval)
				{
					this.stopwatchServer -= this.tickInterval;
					this.OnTickServer();
				}
			}
		}

		// Token: 0x06000C07 RID: 3079 RVA: 0x00035A04 File Offset: 0x00033C04
		private void OnTickServer()
		{
			if (!this.cachedHurtBox || !this.cachedHurtBox.healthComponent)
			{
				this.BreakServer();
				return;
			}
			this.cachedHurtBox.healthComponent.Heal(this.healRate * this.tickInterval, default(ProcChainMask), true);
			if (this.breakOnTargetFullyHealed && this.cachedHurtBox.healthComponent.health >= this.cachedHurtBox.healthComponent.fullHealth)
			{
				this.BreakServer();
				return;
			}
		}

		// Token: 0x06000C08 RID: 3080 RVA: 0x00035A90 File Offset: 0x00033C90
		private void UpdateCachedHurtBox()
		{
			if (!this.previousHurtBoxReference.Equals(this.netTarget))
			{
				this.cachedHurtBox = this.netTarget.ResolveHurtBox();
				this.previousHurtBoxReference = this.netTarget;
			}
		}

		// Token: 0x06000C09 RID: 3081 RVA: 0x00035AC2 File Offset: 0x00033CC2
		public static bool HealBeamAlreadyExists(GameObject owner, HurtBox target)
		{
			return HealBeamController.HealBeamAlreadyExists(owner, target.healthComponent);
		}

		// Token: 0x06000C0A RID: 3082 RVA: 0x00035AD0 File Offset: 0x00033CD0
		public static bool HealBeamAlreadyExists(GameObject owner, HealthComponent targetHealthComponent)
		{
			List<HealBeamController> instancesList = InstanceTracker.GetInstancesList<HealBeamController>();
			int i = 0;
			int count = instancesList.Count;
			while (i < count)
			{
				HealBeamController healBeamController = instancesList[i];
				if (healBeamController.target.healthComponent == targetHealthComponent && healBeamController.ownership.ownerObject == owner)
				{
					return true;
				}
				i++;
			}
			return false;
		}

		// Token: 0x06000C0B RID: 3083 RVA: 0x00035B20 File Offset: 0x00033D20
		public static int GetHealBeamCountForOwner(GameObject owner)
		{
			int num = 0;
			List<HealBeamController> instancesList = InstanceTracker.GetInstancesList<HealBeamController>();
			int i = 0;
			int count = instancesList.Count;
			while (i < count)
			{
				if (instancesList[i].ownership.ownerObject == owner)
				{
					num++;
				}
				i++;
			}
			return num;
		}

		// Token: 0x06000C0C RID: 3084 RVA: 0x00035B64 File Offset: 0x00033D64
		private void UpdateHealBeamVisuals()
		{
			float target = this.target ? 1f : 0f;
			this.scaleFactor = Mathf.SmoothDamp(this.scaleFactor, target, ref this.scaleFactorVelocity, this.smoothTime);
			Vector3 localScale = new Vector3(this.scaleFactor, this.scaleFactor, this.scaleFactor);
			this.startPointTransform.SetPositionAndRotation(base.transform.position, base.transform.rotation);
			this.startPointTransform.localScale = localScale;
			if (this.cachedHurtBox)
			{
				this.endPointTransform.position = this.cachedHurtBox.transform.position;
			}
			this.endPointTransform.localScale = localScale;
			this.lineRenderer.widthMultiplier = this.scaleFactor * this.maxLineWidth;
		}

		// Token: 0x06000C0D RID: 3085 RVA: 0x00035C3C File Offset: 0x00033E3C
		[Server]
		public void BreakServer()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.HealBeamController::BreakServer()' called on client");
				return;
			}
			if (this.broken)
			{
				return;
			}
			this.broken = true;
			this.target = null;
			base.transform.SetParent(null);
			this.ownership.ownerObject = null;
			UnityEngine.Object.Destroy(base.gameObject, this.lingerAfterBrokenDuration);
		}

		// Token: 0x06000C0F RID: 3087 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x17000184 RID: 388
		// (get) Token: 0x06000C10 RID: 3088 RVA: 0x00035CC8 File Offset: 0x00033EC8
		// (set) Token: 0x06000C11 RID: 3089 RVA: 0x00035CDB File Offset: 0x00033EDB
		public HurtBoxReference NetworknetTarget
		{
			get
			{
				return this.netTarget;
			}
			[param: In]
			set
			{
				uint dirtyBit = 1U;
				if (NetworkServer.localClientActive && !base.syncVarHookGuard)
				{
					base.syncVarHookGuard = true;
					this.OnSyncTarget(value);
					base.syncVarHookGuard = false;
				}
				base.SetSyncVar<HurtBoxReference>(value, ref this.netTarget, dirtyBit);
			}
		}

		// Token: 0x06000C12 RID: 3090 RVA: 0x00035D1C File Offset: 0x00033F1C
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				GeneratedNetworkCode._WriteHurtBoxReference_None(writer, this.netTarget);
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
				GeneratedNetworkCode._WriteHurtBoxReference_None(writer, this.netTarget);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x06000C13 RID: 3091 RVA: 0x00035D88 File Offset: 0x00033F88
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.netTarget = GeneratedNetworkCode._ReadHurtBoxReference_None(reader);
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.OnSyncTarget(GeneratedNetworkCode._ReadHurtBoxReference_None(reader));
			}
		}

		// Token: 0x04000C05 RID: 3077
		public Transform startPointTransform;

		// Token: 0x04000C06 RID: 3078
		public Transform endPointTransform;

		// Token: 0x04000C07 RID: 3079
		public float tickInterval = 1f;

		// Token: 0x04000C08 RID: 3080
		public bool breakOnTargetFullyHealed;

		// Token: 0x04000C09 RID: 3081
		public LineRenderer lineRenderer;

		// Token: 0x04000C0A RID: 3082
		public float lingerAfterBrokenDuration;

		// Token: 0x04000C0C RID: 3084
		[SyncVar(hook = "OnSyncTarget")]
		private HurtBoxReference netTarget;

		// Token: 0x04000C0E RID: 3086
		private float stopwatchServer;

		// Token: 0x04000C0F RID: 3087
		private bool broken;

		// Token: 0x04000C10 RID: 3088
		private HurtBoxReference previousHurtBoxReference;

		// Token: 0x04000C11 RID: 3089
		private HurtBox cachedHurtBox;

		// Token: 0x04000C12 RID: 3090
		private float scaleFactorVelocity;

		// Token: 0x04000C13 RID: 3091
		private float maxLineWidth = 0.3f;

		// Token: 0x04000C14 RID: 3092
		private float smoothTime = 0.1f;

		// Token: 0x04000C15 RID: 3093
		private float scaleFactor;
	}
}
