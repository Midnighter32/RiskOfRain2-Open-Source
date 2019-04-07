using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000298 RID: 664
	public class ChestRevealer : NetworkBehaviour
	{
		// Token: 0x06000D89 RID: 3465 RVA: 0x00042B46 File Offset: 0x00040D46
		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			RoR2Application.onFixedUpdate += ChestRevealer.StaticFixedUpdate;
		}

		// Token: 0x06000D8A RID: 3466 RVA: 0x00042B5C File Offset: 0x00040D5C
		private static void StaticFixedUpdate()
		{
			ChestRevealer.pendingReveals.Sort();
			while (ChestRevealer.pendingReveals.Count > 0 && ChestRevealer.pendingReveals[0].time.hasPassed)
			{
				if (ChestRevealer.pendingReveals[0].gameObject)
				{
					ChestRevealer.RevealedObject.RevealObject(ChestRevealer.pendingReveals[0].gameObject, ChestRevealer.pendingReveals[0].duration);
				}
				ChestRevealer.pendingReveals.RemoveAt(0);
			}
		}

		// Token: 0x06000D8B RID: 3467 RVA: 0x00042BE4 File Offset: 0x00040DE4
		public void Pulse()
		{
			Vector3 position = base.transform.position;
			float num = this.radius * this.radius;
			foreach (PurchaseInteraction purchaseInteraction in PurchaseInteraction.readOnlyInstancesList)
			{
				float sqrMagnitude = (purchaseInteraction.transform.position - position).sqrMagnitude;
				if (sqrMagnitude <= num && purchaseInteraction.available)
				{
					float b = Mathf.Sqrt(sqrMagnitude) / this.pulseTravelSpeed;
					ChestRevealer.PendingReveal item = new ChestRevealer.PendingReveal
					{
						gameObject = purchaseInteraction.gameObject,
						time = Run.FixedTimeStamp.now + b,
						duration = this.revealDuration
					};
					ChestRevealer.pendingReveals.Add(item);
				}
			}
			EffectManager.instance.SpawnEffect(this.pulseEffectPrefab, new EffectData
			{
				origin = position,
				scale = this.radius * this.pulseEffectScale
			}, false);
		}

		// Token: 0x06000D8C RID: 3468 RVA: 0x00042CF8 File Offset: 0x00040EF8
		private void FixedUpdate()
		{
			if (this.nextPulse.hasPassed)
			{
				this.Pulse();
				this.nextPulse = Run.FixedTimeStamp.now + this.pulseInterval;
			}
		}

		// Token: 0x06000D8F RID: 3471 RVA: 0x00004507 File Offset: 0x00002707
		private void UNetVersion()
		{
		}

		// Token: 0x17000126 RID: 294
		// (get) Token: 0x06000D90 RID: 3472 RVA: 0x00042D70 File Offset: 0x00040F70
		// (set) Token: 0x06000D91 RID: 3473 RVA: 0x00042D83 File Offset: 0x00040F83
		public float Networkradius
		{
			get
			{
				return this.radius;
			}
			set
			{
				base.SetSyncVar<float>(value, ref this.radius, 1u);
			}
		}

		// Token: 0x06000D92 RID: 3474 RVA: 0x00042D98 File Offset: 0x00040F98
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write(this.radius);
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
				writer.Write(this.radius);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x06000D93 RID: 3475 RVA: 0x00042E04 File Offset: 0x00041004
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.radius = reader.ReadSingle();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.radius = reader.ReadSingle();
			}
		}

		// Token: 0x0400117F RID: 4479
		[SyncVar]
		public float radius;

		// Token: 0x04001180 RID: 4480
		public float pulseTravelSpeed = 10f;

		// Token: 0x04001181 RID: 4481
		public float revealDuration = 10f;

		// Token: 0x04001182 RID: 4482
		public float pulseInterval = 1f;

		// Token: 0x04001183 RID: 4483
		private Run.FixedTimeStamp nextPulse = Run.FixedTimeStamp.negativeInfinity;

		// Token: 0x04001184 RID: 4484
		public GameObject pulseEffectPrefab;

		// Token: 0x04001185 RID: 4485
		public float pulseEffectScale = 1f;

		// Token: 0x04001186 RID: 4486
		private static readonly List<ChestRevealer.PendingReveal> pendingReveals = new List<ChestRevealer.PendingReveal>();

		// Token: 0x02000299 RID: 665
		private struct PendingReveal : IComparable<ChestRevealer.PendingReveal>
		{
			// Token: 0x06000D94 RID: 3476 RVA: 0x00042E45 File Offset: 0x00041045
			public int CompareTo(ChestRevealer.PendingReveal other)
			{
				return this.time.CompareTo(other.time);
			}

			// Token: 0x04001187 RID: 4487
			public GameObject gameObject;

			// Token: 0x04001188 RID: 4488
			public Run.FixedTimeStamp time;

			// Token: 0x04001189 RID: 4489
			public float duration;
		}

		// Token: 0x0200029A RID: 666
		private class RevealedObject : MonoBehaviour
		{
			// Token: 0x06000D95 RID: 3477 RVA: 0x00042E58 File Offset: 0x00041058
			public static void RevealObject(GameObject gameObject, float duration)
			{
				ChestRevealer.RevealedObject revealedObject;
				if (!ChestRevealer.RevealedObject.currentlyRevealedObjects.TryGetValue(gameObject, out revealedObject))
				{
					revealedObject = gameObject.AddComponent<ChestRevealer.RevealedObject>();
				}
				if (revealedObject.lifetime < duration)
				{
					revealedObject.lifetime = duration;
				}
			}

			// Token: 0x06000D96 RID: 3478 RVA: 0x00042E8C File Offset: 0x0004108C
			private void OnEnable()
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/PositionIndicators/PoiPositionIndicator"), base.transform.position, base.transform.rotation);
				this.positionIndicator = gameObject.GetComponent<PositionIndicator>();
				this.positionIndicator.targetTransform = base.transform;
				ChestRevealer.RevealedObject.currentlyRevealedObjects[base.gameObject] = this;
			}

			// Token: 0x06000D97 RID: 3479 RVA: 0x00042EED File Offset: 0x000410ED
			private void OnDisable()
			{
				ChestRevealer.RevealedObject.currentlyRevealedObjects.Remove(base.gameObject);
				UnityEngine.Object.Destroy(this.positionIndicator.gameObject);
				this.positionIndicator = null;
			}

			// Token: 0x06000D98 RID: 3480 RVA: 0x00042F17 File Offset: 0x00041117
			private void FixedUpdate()
			{
				this.lifetime -= Time.fixedDeltaTime;
				if (this.lifetime <= 0f)
				{
					UnityEngine.Object.Destroy(this);
				}
			}

			// Token: 0x0400118A RID: 4490
			private float lifetime;

			// Token: 0x0400118B RID: 4491
			private static readonly Dictionary<GameObject, ChestRevealer.RevealedObject> currentlyRevealedObjects = new Dictionary<GameObject, ChestRevealer.RevealedObject>();

			// Token: 0x0400118C RID: 4492
			private PositionIndicator positionIndicator;
		}
	}
}
