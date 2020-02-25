using System;
using RoR2.Projectile;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200037F RID: 895
	public class WinchControl : MonoBehaviour
	{
		// Token: 0x060015D5 RID: 5589 RVA: 0x0005D18A File Offset: 0x0005B38A
		private void Start()
		{
			this.attachmentTransform = this.FindAttachmentTransform();
			if (this.attachmentTransform)
			{
				this.tailTransform.position = this.attachmentTransform.position;
			}
		}

		// Token: 0x060015D6 RID: 5590 RVA: 0x0005D1BB File Offset: 0x0005B3BB
		private void Update()
		{
			if (!this.attachmentTransform)
			{
				this.attachmentTransform = this.FindAttachmentTransform();
			}
			if (this.attachmentTransform)
			{
				this.tailTransform.position = this.attachmentTransform.position;
			}
		}

		// Token: 0x060015D7 RID: 5591 RVA: 0x0005D1FC File Offset: 0x0005B3FC
		private Transform FindAttachmentTransform()
		{
			this.projectileGhostController = base.GetComponent<ProjectileGhostController>();
			if (this.projectileGhostController)
			{
				Transform authorityTransform = this.projectileGhostController.authorityTransform;
				if (authorityTransform)
				{
					ProjectileController component = authorityTransform.GetComponent<ProjectileController>();
					if (component)
					{
						GameObject owner = component.owner;
						if (owner)
						{
							ModelLocator component2 = owner.GetComponent<ModelLocator>();
							if (component2)
							{
								Transform modelTransform = component2.modelTransform;
								if (modelTransform)
								{
									ChildLocator component3 = modelTransform.GetComponent<ChildLocator>();
									if (component3)
									{
										return component3.FindChild(this.attachmentString);
									}
								}
							}
						}
					}
				}
			}
			return null;
		}

		// Token: 0x04001461 RID: 5217
		public Transform tailTransform;

		// Token: 0x04001462 RID: 5218
		public string attachmentString;

		// Token: 0x04001463 RID: 5219
		private ProjectileGhostController projectileGhostController;

		// Token: 0x04001464 RID: 5220
		private Transform attachmentTransform;
	}
}
