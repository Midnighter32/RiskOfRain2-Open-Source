using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x0200053E RID: 1342
	[RequireComponent(typeof(Rigidbody))]
	public class MissileController : MonoBehaviour
	{
		// Token: 0x06001DFA RID: 7674 RVA: 0x0008D314 File Offset: 0x0008B514
		private void Awake()
		{
			if (!NetworkServer.active)
			{
				base.enabled = false;
				return;
			}
			this.transform = base.transform;
			this.rigidbody = base.GetComponent<Rigidbody>();
			this.torquePID = base.GetComponent<QuaternionPID>();
			this.teamFilter = base.GetComponent<TeamFilter>();
		}

		// Token: 0x06001DFB RID: 7675 RVA: 0x0008D360 File Offset: 0x0008B560
		private void FixedUpdate()
		{
			this.timer += Time.fixedDeltaTime;
			if (this.timer < this.giveupTimer)
			{
				this.rigidbody.velocity = this.transform.forward * this.maxVelocity;
				if (this.target && this.timer >= this.delayTimer)
				{
					this.rigidbody.velocity = this.transform.forward * (this.maxVelocity + this.timer * this.acceleration);
					Vector3 vector = this.target.transform.position + UnityEngine.Random.insideUnitSphere * this.turbulence - this.transform.position;
					if (vector != Vector3.zero)
					{
						Quaternion rotation = this.transform.rotation;
						Quaternion targetQuat = Util.QuaternionSafeLookRotation(vector);
						this.torquePID.inputQuat = rotation;
						this.torquePID.targetQuat = targetQuat;
						this.rigidbody.angularVelocity = this.torquePID.UpdatePID();
					}
				}
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

		// Token: 0x06001DFC RID: 7676 RVA: 0x0008D4E4 File Offset: 0x0008B6E4
		private Transform FindTarget()
		{
			this.search.searchOrigin = this.transform.position;
			this.search.searchDirection = this.transform.forward;
			this.search.teamMaskFilter.RemoveTeam(this.teamFilter.teamIndex);
			this.search.RefreshCandidates();
			HurtBox hurtBox = this.search.GetResults().FirstOrDefault<HurtBox>();
			if (hurtBox == null)
			{
				return null;
			}
			return hurtBox.transform;
		}

		// Token: 0x04002058 RID: 8280
		private new Transform transform;

		// Token: 0x04002059 RID: 8281
		private Rigidbody rigidbody;

		// Token: 0x0400205A RID: 8282
		private TeamFilter teamFilter;

		// Token: 0x0400205B RID: 8283
		public Transform target;

		// Token: 0x0400205C RID: 8284
		public float maxVelocity;

		// Token: 0x0400205D RID: 8285
		public float rollVelocity;

		// Token: 0x0400205E RID: 8286
		public float acceleration;

		// Token: 0x0400205F RID: 8287
		public float delayTimer;

		// Token: 0x04002060 RID: 8288
		public float giveupTimer = 8f;

		// Token: 0x04002061 RID: 8289
		public float deathTimer = 10f;

		// Token: 0x04002062 RID: 8290
		private float timer;

		// Token: 0x04002063 RID: 8291
		private QuaternionPID torquePID;

		// Token: 0x04002064 RID: 8292
		public float turbulence;

		// Token: 0x04002065 RID: 8293
		public float maxSeekDistance = 40f;

		// Token: 0x04002066 RID: 8294
		private BullseyeSearch search = new BullseyeSearch();
	}
}
