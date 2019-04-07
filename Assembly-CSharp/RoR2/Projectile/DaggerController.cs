using System;
using System.Collections.ObjectModel;
using UnityEngine;

namespace RoR2.Projectile
{
	// Token: 0x02000537 RID: 1335
	[RequireComponent(typeof(Rigidbody))]
	public class DaggerController : MonoBehaviour
	{
		// Token: 0x06001DE2 RID: 7650 RVA: 0x0008C75D File Offset: 0x0008A95D
		private void Awake()
		{
			this.transform = base.transform;
			this.rigidbody = base.GetComponent<Rigidbody>();
			this.rigidbody.AddRelativeForce(UnityEngine.Random.insideUnitSphere * 50f);
		}

		// Token: 0x06001DE3 RID: 7651 RVA: 0x0008C794 File Offset: 0x0008A994
		private void FixedUpdate()
		{
			this.timer += Time.fixedDeltaTime;
			if (this.timer < this.giveupTimer)
			{
				if (this.target)
				{
					Vector3 vector = this.target.transform.position - this.transform.position;
					if (vector != Vector3.zero)
					{
						this.transform.rotation = Util.QuaternionSafeLookRotation(vector);
					}
					if (this.timer >= this.delayTimer)
					{
						this.rigidbody.AddForce(this.transform.forward * this.acceleration);
						if (!this.hasPlayedSound)
						{
							Util.PlaySound("Play_item_proc_dagger_fly", base.gameObject);
							this.hasPlayedSound = true;
						}
					}
				}
			}
			else
			{
				this.rigidbody.useGravity = true;
			}
			if (!this.target)
			{
				this.target = this.FindTarget();
			}
			else
			{
				HealthComponent component = this.target.GetComponent<HealthComponent>();
				if (component && !component.alive)
				{
					this.target = this.FindTarget();
				}
			}
			if (this.timer > this.deathTimer)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		// Token: 0x06001DE4 RID: 7652 RVA: 0x0008C8CC File Offset: 0x0008AACC
		private Transform FindTarget()
		{
			ReadOnlyCollection<TeamComponent> teamMembers = TeamComponent.GetTeamMembers(TeamIndex.Monster);
			float num = 99999f;
			Transform result = null;
			for (int i = 0; i < teamMembers.Count; i++)
			{
				float num2 = Vector3.SqrMagnitude(teamMembers[i].transform.position - this.transform.position);
				if (num2 < num)
				{
					num = num2;
					result = teamMembers[i].transform;
				}
			}
			return result;
		}

		// Token: 0x0400202E RID: 8238
		private new Transform transform;

		// Token: 0x0400202F RID: 8239
		private Rigidbody rigidbody;

		// Token: 0x04002030 RID: 8240
		public Transform target;

		// Token: 0x04002031 RID: 8241
		public float acceleration;

		// Token: 0x04002032 RID: 8242
		public float delayTimer;

		// Token: 0x04002033 RID: 8243
		public float giveupTimer = 8f;

		// Token: 0x04002034 RID: 8244
		public float deathTimer = 10f;

		// Token: 0x04002035 RID: 8245
		private float timer;

		// Token: 0x04002036 RID: 8246
		public float turbulence;

		// Token: 0x04002037 RID: 8247
		private bool hasPlayedSound;
	}
}
