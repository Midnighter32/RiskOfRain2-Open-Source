using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x02000501 RID: 1281
	[RequireComponent(typeof(ProjectileController))]
	[RequireComponent(typeof(Deployable))]
	public class ProjectileDeployToOwner : MonoBehaviour
	{
		// Token: 0x06001E6C RID: 7788 RVA: 0x00083273 File Offset: 0x00081473
		private void Start()
		{
			if (NetworkServer.active)
			{
				this.DeployToOwner();
			}
		}

		// Token: 0x06001E6D RID: 7789 RVA: 0x00083284 File Offset: 0x00081484
		private void DeployToOwner()
		{
			GameObject owner = base.GetComponent<ProjectileController>().owner;
			if (owner)
			{
				CharacterBody component = owner.GetComponent<CharacterBody>();
				if (component)
				{
					CharacterMaster master = component.master;
					if (master)
					{
						master.AddDeployable(base.GetComponent<Deployable>(), this.deployableSlot);
					}
				}
			}
		}

		// Token: 0x04001BB4 RID: 7092
		public DeployableSlot deployableSlot;
	}
}
