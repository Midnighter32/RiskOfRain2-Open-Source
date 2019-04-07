using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200031A RID: 794
	[RequireComponent(typeof(Collider))]
	public class HurtBox : MonoBehaviour
	{
		// Token: 0x17000167 RID: 359
		// (get) Token: 0x06001060 RID: 4192 RVA: 0x00052425 File Offset: 0x00050625
		// (set) Token: 0x06001061 RID: 4193 RVA: 0x0005242D File Offset: 0x0005062D
		public Collider collider { get; private set; }

		// Token: 0x17000168 RID: 360
		// (get) Token: 0x06001062 RID: 4194 RVA: 0x00052436 File Offset: 0x00050636
		// (set) Token: 0x06001063 RID: 4195 RVA: 0x0005243E File Offset: 0x0005063E
		public float volume { get; private set; }

		// Token: 0x17000169 RID: 361
		// (get) Token: 0x06001064 RID: 4196 RVA: 0x00052447 File Offset: 0x00050647
		public Vector3 randomVolumePoint
		{
			get
			{
				return Util.RandomColliderVolumePoint(this.collider);
			}
		}

		// Token: 0x06001065 RID: 4197 RVA: 0x00052454 File Offset: 0x00050654
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

		// Token: 0x06001066 RID: 4198 RVA: 0x000524D3 File Offset: 0x000506D3
		private void OnEnable()
		{
			if (this.isBullseye)
			{
				HurtBox.bullseyesList.Add(this);
			}
		}

		// Token: 0x06001067 RID: 4199 RVA: 0x000524E8 File Offset: 0x000506E8
		private void OnDisable()
		{
			if (this.isBullseye)
			{
				HurtBox.bullseyesList.Remove(this);
			}
		}

		// Token: 0x0400147C RID: 5244
		[Tooltip("The health component to which this hurtbox belongs.")]
		public HealthComponent healthComponent;

		// Token: 0x0400147D RID: 5245
		[Tooltip("Whether or not this hurtbox is considered a bullseye. Do not change this at runtime!")]
		public bool isBullseye;

		// Token: 0x0400147E RID: 5246
		public HurtBox.DamageModifier damageModifier;

		// Token: 0x0400147F RID: 5247
		[NonSerialized]
		public TeamIndex teamIndex = TeamIndex.None;

		// Token: 0x04001480 RID: 5248
		[SerializeField]
		[HideInInspector]
		public HurtBoxGroup hurtBoxGroup;

		// Token: 0x04001481 RID: 5249
		[SerializeField]
		[HideInInspector]
		public short indexInGroup = -1;

		// Token: 0x04001484 RID: 5252
		private static readonly List<HurtBox> bullseyesList = new List<HurtBox>();

		// Token: 0x04001485 RID: 5253
		public static readonly ReadOnlyCollection<HurtBox> readOnlyBullseyesList = HurtBox.bullseyesList.AsReadOnly();

		// Token: 0x0200031B RID: 795
		public enum DamageModifier
		{
			// Token: 0x04001487 RID: 5255
			Normal,
			// Token: 0x04001488 RID: 5256
			SniperTarget,
			// Token: 0x04001489 RID: 5257
			Weak,
			// Token: 0x0400148A RID: 5258
			Barrier
		}
	}
}
