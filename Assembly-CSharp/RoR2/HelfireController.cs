using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;

namespace RoR2
{
	// Token: 0x0200022D RID: 557
	[RequireComponent(typeof(NetworkedBodyAttachment))]
	public class HelfireController : NetworkBehaviour
	{
		// Token: 0x1700019C RID: 412
		// (get) Token: 0x06000C74 RID: 3188 RVA: 0x000382EA File Offset: 0x000364EA
		// (set) Token: 0x06000C75 RID: 3189 RVA: 0x000382F2 File Offset: 0x000364F2
		public NetworkedBodyAttachment networkedBodyAttachment { get; private set; }

		// Token: 0x06000C76 RID: 3190 RVA: 0x000382FB File Offset: 0x000364FB
		private void Awake()
		{
			this.networkedBodyAttachment = base.GetComponent<NetworkedBodyAttachment>();
			this.auraEffectTransform.SetParent(null);
		}

		// Token: 0x06000C77 RID: 3191 RVA: 0x00038315 File Offset: 0x00036515
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

		// Token: 0x06000C78 RID: 3192 RVA: 0x00038354 File Offset: 0x00036554
		private void FixedUpdate()
		{
			this.radius = this.baseRadius * (1f + (float)(this.stack - 1) * 0.5f);
			if (NetworkServer.active)
			{
				this.ServerFixedUpdate();
			}
		}

		// Token: 0x06000C79 RID: 3193 RVA: 0x00038388 File Offset: 0x00036588
		private void ServerFixedUpdate()
		{
			this.timer -= Time.fixedDeltaTime;
			if (this.timer <= 0f)
			{
				float damageMultiplier = 1f + (float)(this.stack - 1) * 0.5f;
				this.timer = this.interval;
				Collider[] array = Physics.OverlapSphere(base.transform.position, this.radius, LayerIndex.entityPrecise.mask, QueryTriggerInteraction.Collide);
				GameObject[] array2 = new GameObject[array.Length];
				int count = 0;
				for (int i = 0; i < array.Length; i++)
				{
					GameObject gameObject = Util.HurtBoxColliderToBody(array[i]).gameObject;
					if (gameObject && Array.IndexOf<GameObject>(array2, gameObject, 0, count) == -1)
					{
						DotController.InflictDot(gameObject, this.networkedBodyAttachment.attachedBodyObject, DotController.DotIndex.Helfire, this.dotDuration, damageMultiplier);
						array2[count++] = gameObject;
					}
				}
			}
		}

		// Token: 0x06000C7A RID: 3194 RVA: 0x0003846C File Offset: 0x0003666C
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

		// Token: 0x06000C7C RID: 3196 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x1700019D RID: 413
		// (get) Token: 0x06000C7D RID: 3197 RVA: 0x00038500 File Offset: 0x00036700
		// (set) Token: 0x06000C7E RID: 3198 RVA: 0x00038513 File Offset: 0x00036713
		public int Networkstack
		{
			get
			{
				return this.stack;
			}
			[param: In]
			set
			{
				base.SetSyncVar<int>(value, ref this.stack, 1U);
			}
		}

		// Token: 0x06000C7F RID: 3199 RVA: 0x00038528 File Offset: 0x00036728
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.WritePackedUInt32((uint)this.stack);
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
				writer.WritePackedUInt32((uint)this.stack);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x06000C80 RID: 3200 RVA: 0x00038594 File Offset: 0x00036794
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

		// Token: 0x04000C65 RID: 3173
		[SyncVar]
		public int stack = 1;

		// Token: 0x04000C66 RID: 3174
		[FormerlySerializedAs("radius")]
		public float baseRadius;

		// Token: 0x04000C67 RID: 3175
		public float dotDuration;

		// Token: 0x04000C68 RID: 3176
		public float interval;

		// Token: 0x04000C69 RID: 3177
		public Transform auraEffectTransform;

		// Token: 0x04000C6A RID: 3178
		private float timer;

		// Token: 0x04000C6B RID: 3179
		private float radius;

		// Token: 0x04000C6D RID: 3181
		private CameraTargetParams cameraTargetParams;
	}
}
