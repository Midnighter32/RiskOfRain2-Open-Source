using System;
using RoR2.Projectile;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000423 RID: 1059
	public class WinchControl : MonoBehaviour
	{
		// Token: 0x0600179B RID: 6043 RVA: 0x0006FE6E File Offset: 0x0006E06E
		private void Start()
		{
			this.attachmentTransform = this.FindAttachmentTransform();
			if (this.attachmentTransform)
			{
				this.tailTransform.position = this.attachmentTransform.position;
			}
		}

		// Token: 0x0600179C RID: 6044 RVA: 0x0006FE9F File Offset: 0x0006E09F
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

		// Token: 0x0600179D RID: 6045 RVA: 0x0006FEE0 File Offset: 0x0006E0E0
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

		// Token: 0x04001ACB RID: 6859
		public Transform tailTransform;

		// Token: 0x04001ACC RID: 6860
		public string attachmentString;

		// Token: 0x04001ACD RID: 6861
		private ProjectileGhostController projectileGhostController;

		// Token: 0x04001ACE RID: 6862
		private Transform attachmentTransform;
	}
}
