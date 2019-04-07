using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003A7 RID: 935
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(Collider))]
	public class RigidbodyStickOnImpact : MonoBehaviour
	{
		// Token: 0x060013CD RID: 5069 RVA: 0x00061017 File Offset: 0x0005F217
		private void Start()
		{
			this.rb = base.GetComponent<Rigidbody>();
		}

		// Token: 0x060013CE RID: 5070 RVA: 0x00061028 File Offset: 0x0005F228
		private void Update()
		{
			if (this.stuck)
			{
				this.stopwatchSinceStuck += Time.deltaTime;
				base.transform.position = this.transformPositionWhenContacted + this.embedDistanceCurve.Evaluate(this.stopwatchSinceStuck) * this.contactNormal;
			}
		}

		// Token: 0x060013CF RID: 5071 RVA: 0x00061084 File Offset: 0x0005F284
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
				EffectManager.instance.SpawnEffect(this.stickEffectPrefab, new EffectData
				{
					origin = this.contactPosition,
					rotation = Util.QuaternionSafeLookRotation(this.contactNormal)
				}, false);
				Util.PlaySound(this.stickSoundString, base.gameObject);
				this.rb.isKinematic = true;
				this.rb.velocity = Vector3.zero;
			}
		}

		// Token: 0x04001795 RID: 6037
		private Rigidbody rb;

		// Token: 0x04001796 RID: 6038
		public string stickSoundString;

		// Token: 0x04001797 RID: 6039
		public GameObject stickEffectPrefab;

		// Token: 0x04001798 RID: 6040
		public float minimumRelativeVelocityMagnitude;

		// Token: 0x04001799 RID: 6041
		public AnimationCurve embedDistanceCurve;

		// Token: 0x0400179A RID: 6042
		private bool stuck;

		// Token: 0x0400179B RID: 6043
		private float stopwatchSinceStuck;

		// Token: 0x0400179C RID: 6044
		private Vector3 contactNormal;

		// Token: 0x0400179D RID: 6045
		private Vector3 contactPosition;

		// Token: 0x0400179E RID: 6046
		private Vector3 transformPositionWhenContacted;
	}
}
