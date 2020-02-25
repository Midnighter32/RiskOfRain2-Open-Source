using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020001A1 RID: 417
	public class ChestRevealer : NetworkBehaviour
	{
		// Token: 0x060008F4 RID: 2292 RVA: 0x00026D24 File Offset: 0x00024F24
		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			RoR2Application.onFixedUpdate += ChestRevealer.StaticFixedUpdate;
			ChestRevealer.typesToCheck = (from t in typeof(ChestRevealer).Assembly.GetTypes()
			where typeof(IInteractable).IsAssignableFrom(t)
			select t).ToArray<Type>();
		}

		// Token: 0x060008F5 RID: 2293 RVA: 0x00026D84 File Offset: 0x00024F84
		private static void StaticFixedUpdate()
		{
			ChestRevealer.pendingReveals.Sort();
			while (ChestRevealer.pendingReveals.Count > 0)
			{
				ChestRevealer.PendingReveal pendingReveal = ChestRevealer.pendingReveals[0];
				if (!pendingReveal.time.hasPassed)
				{
					break;
				}
				if (pendingReveal.gameObject)
				{
					ChestRevealer.RevealedObject.RevealObject(pendingReveal.gameObject, pendingReveal.duration);
				}
				ChestRevealer.pendingReveals.RemoveAt(0);
			}
		}

		// Token: 0x060008F6 RID: 2294 RVA: 0x00026DF0 File Offset: 0x00024FF0
		public void Pulse()
		{
			ChestRevealer.<>c__DisplayClass12_0 CS$<>8__locals1;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.origin = base.transform.position;
			CS$<>8__locals1.radiusSqr = this.radius * this.radius;
			CS$<>8__locals1.invPulseTravelSpeed = 1f / this.pulseTravelSpeed;
			Type[] array = ChestRevealer.typesToCheck;
			for (int i = 0; i < array.Length; i++)
			{
				foreach (MonoBehaviour monoBehaviour in InstanceTracker.FindInstancesEnumerable(array[i]))
				{
					if (((IInteractable)monoBehaviour).ShouldShowOnScanner())
					{
						this.<Pulse>g__TryAddRevealable|12_0(monoBehaviour.transform, ref CS$<>8__locals1);
					}
				}
			}
			EffectManager.SpawnEffect(this.pulseEffectPrefab, new EffectData
			{
				origin = CS$<>8__locals1.origin,
				scale = this.radius * this.pulseEffectScale
			}, false);
		}

		// Token: 0x060008F7 RID: 2295 RVA: 0x00026EDC File Offset: 0x000250DC
		private void FixedUpdate()
		{
			if (this.nextPulse.hasPassed)
			{
				this.Pulse();
				this.nextPulse = Run.FixedTimeStamp.now + this.pulseInterval;
			}
		}

		// Token: 0x060008FA RID: 2298 RVA: 0x00026F54 File Offset: 0x00025154
		[CompilerGenerated]
		private void <Pulse>g__TryAddRevealable|12_0(Transform revealableTransform, ref ChestRevealer.<>c__DisplayClass12_0 A_2)
		{
			float sqrMagnitude = (revealableTransform.position - A_2.origin).sqrMagnitude;
			if (sqrMagnitude > A_2.radiusSqr)
			{
				return;
			}
			float b = Mathf.Sqrt(sqrMagnitude) * A_2.invPulseTravelSpeed;
			ChestRevealer.PendingReveal item = new ChestRevealer.PendingReveal
			{
				gameObject = revealableTransform.gameObject,
				time = Run.FixedTimeStamp.now + b,
				duration = this.revealDuration
			};
			ChestRevealer.pendingReveals.Add(item);
		}

		// Token: 0x060008FB RID: 2299 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x1700012E RID: 302
		// (get) Token: 0x060008FC RID: 2300 RVA: 0x00026FD8 File Offset: 0x000251D8
		// (set) Token: 0x060008FD RID: 2301 RVA: 0x00026FEB File Offset: 0x000251EB
		public float Networkradius
		{
			get
			{
				return this.radius;
			}
			[param: In]
			set
			{
				base.SetSyncVar<float>(value, ref this.radius, 1U);
			}
		}

		// Token: 0x060008FE RID: 2302 RVA: 0x00027000 File Offset: 0x00025200
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write(this.radius);
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
				writer.Write(this.radius);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x060008FF RID: 2303 RVA: 0x0002706C File Offset: 0x0002526C
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

		// Token: 0x04000947 RID: 2375
		[SyncVar]
		public float radius;

		// Token: 0x04000948 RID: 2376
		public float pulseTravelSpeed = 10f;

		// Token: 0x04000949 RID: 2377
		public float revealDuration = 10f;

		// Token: 0x0400094A RID: 2378
		public float pulseInterval = 1f;

		// Token: 0x0400094B RID: 2379
		private Run.FixedTimeStamp nextPulse = Run.FixedTimeStamp.negativeInfinity;

		// Token: 0x0400094C RID: 2380
		public GameObject pulseEffectPrefab;

		// Token: 0x0400094D RID: 2381
		public float pulseEffectScale = 1f;

		// Token: 0x0400094E RID: 2382
		private static readonly List<ChestRevealer.PendingReveal> pendingReveals = new List<ChestRevealer.PendingReveal>();

		// Token: 0x0400094F RID: 2383
		private static Type[] typesToCheck;

		// Token: 0x020001A2 RID: 418
		private struct PendingReveal : IComparable<ChestRevealer.PendingReveal>
		{
			// Token: 0x06000900 RID: 2304 RVA: 0x000270AD File Offset: 0x000252AD
			public int CompareTo(ChestRevealer.PendingReveal other)
			{
				return this.time.CompareTo(other.time);
			}

			// Token: 0x04000950 RID: 2384
			public GameObject gameObject;

			// Token: 0x04000951 RID: 2385
			public Run.FixedTimeStamp time;

			// Token: 0x04000952 RID: 2386
			public float duration;
		}

		// Token: 0x020001A3 RID: 419
		private class RevealedObject : MonoBehaviour
		{
			// Token: 0x06000901 RID: 2305 RVA: 0x000270C0 File Offset: 0x000252C0
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

			// Token: 0x06000902 RID: 2306 RVA: 0x000270F4 File Offset: 0x000252F4
			private void OnEnable()
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/PositionIndicators/PoiPositionIndicator"), base.transform.position, base.transform.rotation);
				this.positionIndicator = gameObject.GetComponent<PositionIndicator>();
				this.positionIndicator.targetTransform = base.transform;
				ChestRevealer.RevealedObject.currentlyRevealedObjects[base.gameObject] = this;
			}

			// Token: 0x06000903 RID: 2307 RVA: 0x00027155 File Offset: 0x00025355
			private void OnDisable()
			{
				ChestRevealer.RevealedObject.currentlyRevealedObjects.Remove(base.gameObject);
				if (this.positionIndicator)
				{
					UnityEngine.Object.Destroy(this.positionIndicator.gameObject);
				}
				this.positionIndicator = null;
			}

			// Token: 0x06000904 RID: 2308 RVA: 0x0002718C File Offset: 0x0002538C
			private void FixedUpdate()
			{
				this.lifetime -= Time.fixedDeltaTime;
				if (this.lifetime <= 0f)
				{
					UnityEngine.Object.Destroy(this);
				}
			}

			// Token: 0x04000953 RID: 2387
			private float lifetime;

			// Token: 0x04000954 RID: 2388
			private static readonly Dictionary<GameObject, ChestRevealer.RevealedObject> currentlyRevealedObjects = new Dictionary<GameObject, ChestRevealer.RevealedObject>();

			// Token: 0x04000955 RID: 2389
			private PositionIndicator positionIndicator;
		}
	}
}
