using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x020004FA RID: 1274
	[RequireComponent(typeof(ProjectileController))]
	public class ProjectileCallOnOwnerNearby : MonoBehaviour
	{
		// Token: 0x06001E43 RID: 7747 RVA: 0x00082A60 File Offset: 0x00080C60
		private void Awake()
		{
			this.projectileController = base.GetComponent<ProjectileController>();
			ProjectileCallOnOwnerNearby.Filter filter = ProjectileCallOnOwnerNearby.Filter.None;
			if (NetworkServer.active)
			{
				filter |= ProjectileCallOnOwnerNearby.Filter.Server;
			}
			if (NetworkClient.active)
			{
				filter |= ProjectileCallOnOwnerNearby.Filter.Client;
			}
			if ((this.filter & filter) == ProjectileCallOnOwnerNearby.Filter.None)
			{
				base.enabled = false;
			}
		}

		// Token: 0x06001E44 RID: 7748 RVA: 0x00082AA2 File Offset: 0x00080CA2
		private void OnDisable()
		{
			this.SetState(ProjectileCallOnOwnerNearby.State.Outside);
		}

		// Token: 0x06001E45 RID: 7749 RVA: 0x00082AAB File Offset: 0x00080CAB
		private void SetState(ProjectileCallOnOwnerNearby.State newState)
		{
			if (this.state == newState)
			{
				return;
			}
			this.state = newState;
			if (this.state == ProjectileCallOnOwnerNearby.State.Inside)
			{
				UnityEvent unityEvent = this.onOwnerEnter;
				if (unityEvent == null)
				{
					return;
				}
				unityEvent.Invoke();
				return;
			}
			else
			{
				UnityEvent unityEvent2 = this.onOwnerExit;
				if (unityEvent2 == null)
				{
					return;
				}
				unityEvent2.Invoke();
				return;
			}
		}

		// Token: 0x06001E46 RID: 7750 RVA: 0x00082AE8 File Offset: 0x00080CE8
		private void FixedUpdate()
		{
			ProjectileCallOnOwnerNearby.State state = ProjectileCallOnOwnerNearby.State.Outside;
			if (this.projectileController.owner)
			{
				float num = this.radius * this.radius;
				if ((base.transform.position - this.projectileController.owner.transform.position).sqrMagnitude < num)
				{
					state = ProjectileCallOnOwnerNearby.State.Inside;
				}
			}
			this.SetState(state);
		}

		// Token: 0x04001B84 RID: 7044
		public ProjectileCallOnOwnerNearby.Filter filter;

		// Token: 0x04001B85 RID: 7045
		public float radius;

		// Token: 0x04001B86 RID: 7046
		public UnityEvent onOwnerEnter;

		// Token: 0x04001B87 RID: 7047
		public UnityEvent onOwnerExit;

		// Token: 0x04001B88 RID: 7048
		private ProjectileCallOnOwnerNearby.State state;

		// Token: 0x04001B89 RID: 7049
		private bool ownerInRadius;

		// Token: 0x04001B8A RID: 7050
		private ProjectileController projectileController;

		// Token: 0x020004FB RID: 1275
		[Flags]
		public enum Filter
		{
			// Token: 0x04001B8C RID: 7052
			None = 0,
			// Token: 0x04001B8D RID: 7053
			Server = 1,
			// Token: 0x04001B8E RID: 7054
			Client = 2
		}

		// Token: 0x020004FC RID: 1276
		private enum State
		{
			// Token: 0x04001B90 RID: 7056
			Outside,
			// Token: 0x04001B91 RID: 7057
			Inside
		}
	}
}
