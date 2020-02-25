using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using JetBrains.Annotations;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000239 RID: 569
	[RequireComponent(typeof(Collider))]
	public class HurtBox : MonoBehaviour
	{
		// Token: 0x1700019F RID: 415
		// (get) Token: 0x06000C9F RID: 3231 RVA: 0x000391A3 File Offset: 0x000373A3
		// (set) Token: 0x06000CA0 RID: 3232 RVA: 0x000391AB File Offset: 0x000373AB
		public TeamIndex teamIndex { get; set; } = TeamIndex.None;

		// Token: 0x170001A0 RID: 416
		// (get) Token: 0x06000CA1 RID: 3233 RVA: 0x000391B4 File Offset: 0x000373B4
		// (set) Token: 0x06000CA2 RID: 3234 RVA: 0x000391BC File Offset: 0x000373BC
		public Collider collider { get; private set; }

		// Token: 0x170001A1 RID: 417
		// (get) Token: 0x06000CA3 RID: 3235 RVA: 0x000391C5 File Offset: 0x000373C5
		// (set) Token: 0x06000CA4 RID: 3236 RVA: 0x000391CD File Offset: 0x000373CD
		public float volume { get; private set; }

		// Token: 0x170001A2 RID: 418
		// (get) Token: 0x06000CA5 RID: 3237 RVA: 0x000391D6 File Offset: 0x000373D6
		public Vector3 randomVolumePoint
		{
			get
			{
				return Util.RandomColliderVolumePoint(this.collider);
			}
		}

		// Token: 0x06000CA6 RID: 3238 RVA: 0x000391E4 File Offset: 0x000373E4
		private void Awake()
		{
			this.collider = base.GetComponent<Collider>();
			this.collider.isTrigger = false;
			Rigidbody rigidbody = base.GetComponent<Rigidbody>();
			if (!rigidbody)
			{
				rigidbody = base.gameObject.AddComponent<Rigidbody>();
			}
			rigidbody.isKinematic = true;
			Vector3 lossyScale = base.transform.lossyScale;
			this.volume = lossyScale.x * 2f * (lossyScale.y * 2f) * (lossyScale.z * 2f);
		}

		// Token: 0x06000CA7 RID: 3239 RVA: 0x00039263 File Offset: 0x00037463
		private void OnEnable()
		{
			if (this.isBullseye)
			{
				HurtBox.bullseyesList.Add(this);
			}
		}

		// Token: 0x06000CA8 RID: 3240 RVA: 0x00039278 File Offset: 0x00037478
		private void OnDisable()
		{
			if (this.isBullseye)
			{
				HurtBox.bullseyesList.Remove(this);
			}
		}

		// Token: 0x06000CA9 RID: 3241 RVA: 0x0003928E File Offset: 0x0003748E
		public static GameObject FindEntityObject([NotNull] HurtBox hurtBox)
		{
			if (!hurtBox.healthComponent)
			{
				return null;
			}
			return hurtBox.healthComponent.gameObject;
		}

		// Token: 0x06000CAA RID: 3242 RVA: 0x000392AA File Offset: 0x000374AA
		public static bool HurtBoxesShareEntity([NotNull] HurtBox a, [NotNull] HurtBox b)
		{
			return HurtBox.FindEntityObject(a) == HurtBox.FindEntityObject(b);
		}

		// Token: 0x04000CB9 RID: 3257
		[Tooltip("The health component to which this hurtbox belongs.")]
		public HealthComponent healthComponent;

		// Token: 0x04000CBA RID: 3258
		[Tooltip("Whether or not this hurtbox is considered a bullseye. Do not change this at runtime!")]
		public bool isBullseye;

		// Token: 0x04000CBB RID: 3259
		public HurtBox.DamageModifier damageModifier;

		// Token: 0x04000CBD RID: 3261
		[SerializeField]
		[HideInInspector]
		public HurtBoxGroup hurtBoxGroup;

		// Token: 0x04000CBE RID: 3262
		[SerializeField]
		[HideInInspector]
		public short indexInGroup = -1;

		// Token: 0x04000CC1 RID: 3265
		private static readonly List<HurtBox> bullseyesList = new List<HurtBox>();

		// Token: 0x04000CC2 RID: 3266
		public static readonly ReadOnlyCollection<HurtBox> readOnlyBullseyesList = HurtBox.bullseyesList.AsReadOnly();

		// Token: 0x0200023A RID: 570
		public enum DamageModifier
		{
			// Token: 0x04000CC4 RID: 3268
			Normal,
			// Token: 0x04000CC5 RID: 3269
			SniperTarget,
			// Token: 0x04000CC6 RID: 3270
			Weak,
			// Token: 0x04000CC7 RID: 3271
			Barrier
		}

		// Token: 0x0200023B RID: 571
		public struct EntityEqualityComparer : IEqualityComparer<HurtBox>
		{
			// Token: 0x06000CAD RID: 3245 RVA: 0x000392EB File Offset: 0x000374EB
			public bool Equals(HurtBox a, HurtBox b)
			{
				return HurtBox.HurtBoxesShareEntity(a, b);
			}

			// Token: 0x06000CAE RID: 3246 RVA: 0x000392F4 File Offset: 0x000374F4
			public int GetHashCode(HurtBox hurtBox)
			{
				return HurtBox.FindEntityObject(hurtBox).GetHashCode();
			}
		}
	}
}
