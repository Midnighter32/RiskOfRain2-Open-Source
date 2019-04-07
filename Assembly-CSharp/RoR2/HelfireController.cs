using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;

namespace RoR2
{
	// Token: 0x0200030C RID: 780
	[RequireComponent(typeof(NetworkedBodyAttachment))]
	public class HelfireController : NetworkBehaviour
	{
		// Token: 0x17000164 RID: 356
		// (get) Token: 0x0600102E RID: 4142 RVA: 0x000514AF File Offset: 0x0004F6AF
		// (set) Token: 0x0600102F RID: 4143 RVA: 0x000514B7 File Offset: 0x0004F6B7
		public NetworkedBodyAttachment networkedBodyAttachment { get; private set; }

		// Token: 0x06001030 RID: 4144 RVA: 0x000514C0 File Offset: 0x0004F6C0
		private void Awake()
		{
			this.networkedBodyAttachment = base.GetComponent<NetworkedBodyAttachment>();
			this.auraEffectTransform.SetParent(null);
		}

		// Token: 0x06001031 RID: 4145 RVA: 0x000514DA File Offset: 0x0004F6DA
		private void OnDestroy()
		{
			if (this.auraEffectTransform)
			{
				UnityEngine.Object.Destroy(this.auraEffectTransform.gameObject);
				this.auraEffectTransform = null;
			}
			if (this.cameraTargetParams)
			{
				this.cameraTargetParams.aimMode = CameraTargetParams.AimType.Standard;
			}
		}

		// Token: 0x06001032 RID: 4146 RVA: 0x00051519 File Offset: 0x0004F719
		private void Start()
		{
			if (NetworkServer.active)
			{
				this.bullseyeSearch = new BullseyeSearch
				{
					teamMaskFilter = TeamMask.all
				};
			}
		}

		// Token: 0x06001033 RID: 4147 RVA: 0x00051538 File Offset: 0x0004F738
		private void FixedUpdate()
		{
			this.radius = this.baseRadius * (1f + (float)(this.stack - 1) * 0.5f);
			if (NetworkServer.active)
			{
				this.timer -= Time.fixedDeltaTime;
				if (this.timer <= 0f)
				{
					float damageMultiplier = 1f + (float)(this.stack - 1) * 0.5f;
					this.timer = this.interval;
					this.bullseyeSearch.searchOrigin = base.transform.position;
					this.bullseyeSearch.maxDistanceFilter = this.radius;
					this.bullseyeSearch.RefreshCandidates();
					foreach (GameObject victimObject in (from hurtBox in this.bullseyeSearch.GetResults()
					where hurtBox.healthComponent
					select hurtBox.healthComponent.gameObject).Distinct<GameObject>())
					{
						DotController.InflictDot(victimObject, this.networkedBodyAttachment.attachedBodyObject, DotController.DotIndex.Helfire, this.dotDuration, damageMultiplier);
					}
				}
			}
		}

		// Token: 0x06001034 RID: 4148 RVA: 0x0005168C File Offset: 0x0004F88C
		private void LateUpdate()
		{
			CharacterBody attachedBody = this.networkedBodyAttachment.attachedBody;
			if (attachedBody)
			{
				this.auraEffectTransform.position = this.networkedBodyAttachment.attachedBody.corePosition;
				this.auraEffectTransform.localScale = new Vector3(this.radius, this.radius, this.radius);
				if (!this.cameraTargetParams)
				{
					this.cameraTargetParams = attachedBody.GetComponent<CameraTargetParams>();
					return;
				}
				this.cameraTargetParams.aimMode = CameraTargetParams.AimType.Aura;
			}
		}

		// Token: 0x06001036 RID: 4150 RVA: 0x00004507 File Offset: 0x00002707
		private void UNetVersion()
		{
		}

		// Token: 0x17000165 RID: 357
		// (get) Token: 0x06001037 RID: 4151 RVA: 0x00051720 File Offset: 0x0004F920
		// (set) Token: 0x06001038 RID: 4152 RVA: 0x00051733 File Offset: 0x0004F933
		public int Networkstack
		{
			get
			{
				return this.stack;
			}
			set
			{
				base.SetSyncVar<int>(value, ref this.stack, 1u);
			}
		}

		// Token: 0x06001039 RID: 4153 RVA: 0x00051748 File Offset: 0x0004F948
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.WritePackedUInt32((uint)this.stack);
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
				writer.WritePackedUInt32((uint)this.stack);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x0600103A RID: 4154 RVA: 0x000517B4 File Offset: 0x0004F9B4
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.stack = (int)reader.ReadPackedUInt32();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.stack = (int)reader.ReadPackedUInt32();
			}
		}

		// Token: 0x04001423 RID: 5155
		[SyncVar]
		public int stack = 1;

		// Token: 0x04001424 RID: 5156
		[FormerlySerializedAs("radius")]
		public float baseRadius;

		// Token: 0x04001425 RID: 5157
		public float dotDuration;

		// Token: 0x04001426 RID: 5158
		public float interval;

		// Token: 0x04001427 RID: 5159
		public Transform auraEffectTransform;

		// Token: 0x04001428 RID: 5160
		private float timer;

		// Token: 0x04001429 RID: 5161
		private float radius;

		// Token: 0x0400142B RID: 5163
		private BullseyeSearch bullseyeSearch;

		// Token: 0x0400142C RID: 5164
		private CameraTargetParams cameraTargetParams;
	}
}
