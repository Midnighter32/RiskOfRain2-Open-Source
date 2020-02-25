using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002E7 RID: 743
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(Collider))]
	public class RigidbodyStickOnImpact : MonoBehaviour
	{
		// Token: 0x060010FE RID: 4350 RVA: 0x0004ACCC File Offset: 0x00048ECC
		private void Start()
		{
			this.rb = base.GetComponent<Rigidbody>();
		}

		// Token: 0x060010FF RID: 4351 RVA: 0x0004ACDC File Offset: 0x00048EDC
		private void Update()
		{
			if (this.stuck)
			{
				this.stopwatchSinceStuck += Time.deltaTime;
				base.transform.position = this.transformPositionWhenContacted + this.embedDistanceCurve.Evaluate(this.stopwatchSinceStuck) * this.contactNormal;
			}
		}

		// Token: 0x06001100 RID: 4352 RVA: 0x0004AD38 File Offset: 0x00048F38
		private void OnCollisionEnter(Collision collision)
		{
			if (this.stuck || this.rb.isKinematic)
			{
				return;
			}
			if (collision.transform.gameObject.layer != LayerIndex.world.intVal)
			{
				return;
			}
			if (collision.relativeVelocity.sqrMagnitude > this.minimumRelativeVelocityMagnitude * this.minimumRelativeVelocityMagnitude)
			{
				this.stuck = true;
				ContactPoint contact = collision.GetContact(0);
				this.contactNormal = contact.normal;
				this.contactPosition = contact.point;
				this.transformPositionWhenContacted = base.transform.position;
				EffectManager.SpawnEffect(this.stickEffectPrefab, new EffectData
				{
					origin = this.contactPosition,
					rotation = Util.QuaternionSafeLookRotation(this.contactNormal)
				}, false);
				Util.PlaySound(this.stickSoundString, base.gameObject);
				this.rb.isKinematic = true;
				this.rb.velocity = Vector3.zero;
			}
		}

		// Token: 0x04001072 RID: 4210
		private Rigidbody rb;

		// Token: 0x04001073 RID: 4211
		public string stickSoundString;

		// Token: 0x04001074 RID: 4212
		public GameObject stickEffectPrefab;

		// Token: 0x04001075 RID: 4213
		public float minimumRelativeVelocityMagnitude;

		// Token: 0x04001076 RID: 4214
		public AnimationCurve embedDistanceCurve;

		// Token: 0x04001077 RID: 4215
		private bool stuck;

		// Token: 0x04001078 RID: 4216
		private float stopwatchSinceStuck;

		// Token: 0x04001079 RID: 4217
		private Vector3 contactNormal;

		// Token: 0x0400107A RID: 4218
		private Vector3 contactPosition;

		// Token: 0x0400107B RID: 4219
		private Vector3 transformPositionWhenContacted;
	}
}
