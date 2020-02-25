using System;
using System.Collections.ObjectModel;
using UnityEngine;

namespace RoR2.Projectile
{
	// Token: 0x020004F1 RID: 1265
	[RequireComponent(typeof(Rigidbody))]
	public class DaggerController : MonoBehaviour
	{
		// Token: 0x06001E17 RID: 7703 RVA: 0x00081A45 File Offset: 0x0007FC45
		private void Awake()
		{
			this.transform = base.transform;
			this.rigidbody = base.GetComponent<Rigidbody>();
			this.rigidbody.AddRelativeForce(UnityEngine.Random.insideUnitSphere * 50f);
		}

		// Token: 0x06001E18 RID: 7704 RVA: 0x00081A7C File Offset: 0x0007FC7C
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

		// Token: 0x06001E19 RID: 7705 RVA: 0x00081BB4 File Offset: 0x0007FDB4
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

		// Token: 0x04001B47 RID: 6983
		private new Transform transform;

		// Token: 0x04001B48 RID: 6984
		private Rigidbody rigidbody;

		// Token: 0x04001B49 RID: 6985
		public Transform target;

		// Token: 0x04001B4A RID: 6986
		public float acceleration;

		// Token: 0x04001B4B RID: 6987
		public float delayTimer;

		// Token: 0x04001B4C RID: 6988
		public float giveupTimer = 8f;

		// Token: 0x04001B4D RID: 6989
		public float deathTimer = 10f;

		// Token: 0x04001B4E RID: 6990
		private float timer;

		// Token: 0x04001B4F RID: 6991
		public float turbulence;

		// Token: 0x04001B50 RID: 6992
		private bool hasPlayedSound;
	}
}
