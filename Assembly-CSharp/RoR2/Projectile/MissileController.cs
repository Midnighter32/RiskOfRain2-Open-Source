using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x020004F9 RID: 1273
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(ProjectileTargetComponent))]
	public class MissileController : MonoBehaviour
	{
		// Token: 0x06001E3F RID: 7743 RVA: 0x000827BC File Offset: 0x000809BC
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
			this.targetComponent = base.GetComponent<ProjectileTargetComponent>();
		}

		// Token: 0x06001E40 RID: 7744 RVA: 0x00082814 File Offset: 0x00080A14
		private void FixedUpdate()
		{
			this.timer += Time.fixedDeltaTime;
			if (this.timer < this.giveupTimer)
			{
				this.rigidbody.velocity = this.transform.forward * this.maxVelocity;
				if (this.targetComponent.target && this.timer >= this.delayTimer)
				{
					this.rigidbody.velocity = this.transform.forward * (this.maxVelocity + this.timer * this.acceleration);
					Vector3 vector = this.targetComponent.target.position + UnityEngine.Random.insideUnitSphere * this.turbulence - this.transform.position;
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
			if (!this.targetComponent.target)
			{
				this.targetComponent.target = this.FindTarget();
			}
			else
			{
				HealthComponent component = this.targetComponent.target.GetComponent<HealthComponent>();
				if (component && !component.alive)
				{
					this.targetComponent.target = this.FindTarget();
				}
			}
			if (this.timer > this.deathTimer)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		// Token: 0x06001E41 RID: 7745 RVA: 0x000829B0 File Offset: 0x00080BB0
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

		// Token: 0x04001B75 RID: 7029
		private new Transform transform;

		// Token: 0x04001B76 RID: 7030
		private Rigidbody rigidbody;

		// Token: 0x04001B77 RID: 7031
		private TeamFilter teamFilter;

		// Token: 0x04001B78 RID: 7032
		private ProjectileTargetComponent targetComponent;

		// Token: 0x04001B79 RID: 7033
		public float maxVelocity;

		// Token: 0x04001B7A RID: 7034
		public float rollVelocity;

		// Token: 0x04001B7B RID: 7035
		public float acceleration;

		// Token: 0x04001B7C RID: 7036
		public float delayTimer;

		// Token: 0x04001B7D RID: 7037
		public float giveupTimer = 8f;

		// Token: 0x04001B7E RID: 7038
		public float deathTimer = 10f;

		// Token: 0x04001B7F RID: 7039
		private float timer;

		// Token: 0x04001B80 RID: 7040
		private QuaternionPID torquePID;

		// Token: 0x04001B81 RID: 7041
		public float turbulence;

		// Token: 0x04001B82 RID: 7042
		public float maxSeekDistance = 40f;

		// Token: 0x04001B83 RID: 7043
		private BullseyeSearch search = new BullseyeSearch();
	}
}
