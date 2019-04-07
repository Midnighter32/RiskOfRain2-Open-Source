using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003FC RID: 1020
	public class TemporaryVisualEffect : MonoBehaviour
	{
		// Token: 0x060016BC RID: 5820 RVA: 0x0006C58A File Offset: 0x0006A78A
		private void Start()
		{
			this.RebuildVisualComponents();
		}

		// Token: 0x060016BD RID: 5821 RVA: 0x0006C592 File Offset: 0x0006A792
		private void FixedUpdate()
		{
			if (this.previousVisualState != this.visualState)
			{
				this.RebuildVisualComponents();
			}
			this.previousVisualState = this.visualState;
		}

		// Token: 0x060016BE RID: 5822 RVA: 0x0006C5B4 File Offset: 0x0006A7B4
		private void RebuildVisualComponents()
		{
			TemporaryVisualEffect.VisualState visualState = this.visualState;
			MonoBehaviour[] array;
			if (visualState == TemporaryVisualEffect.VisualState.Enter)
			{
				array = this.enterComponents;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].enabled = true;
				}
				array = this.exitComponents;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].enabled = false;
				}
				return;
			}
			if (visualState != TemporaryVisualEffect.VisualState.Exit)
			{
				return;
			}
			array = this.enterComponents;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enabled = false;
			}
			array = this.exitComponents;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enabled = true;
			}
		}

		// Token: 0x060016BF RID: 5823 RVA: 0x0006C64C File Offset: 0x0006A84C
		private void LateUpdate()
		{
			bool flag = this.healthComponent;
			if (this.parentTransform)
			{
				base.transform.position = this.parentTransform.position;
			}
			else
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
			if (!flag || (flag && !this.healthComponent.alive))
			{
				this.visualState = TemporaryVisualEffect.VisualState.Exit;
			}
			if (this.visualTransform)
			{
				this.visualTransform.localScale = new Vector3(this.radius, this.radius, this.radius);
			}
		}

		// Token: 0x040019E3 RID: 6627
		public float radius = 1f;

		// Token: 0x040019E4 RID: 6628
		public Transform parentTransform;

		// Token: 0x040019E5 RID: 6629
		public Transform visualTransform;

		// Token: 0x040019E6 RID: 6630
		public MonoBehaviour[] enterComponents;

		// Token: 0x040019E7 RID: 6631
		public MonoBehaviour[] exitComponents;

		// Token: 0x040019E8 RID: 6632
		public TemporaryVisualEffect.VisualState visualState;

		// Token: 0x040019E9 RID: 6633
		private TemporaryVisualEffect.VisualState previousVisualState;

		// Token: 0x040019EA RID: 6634
		[HideInInspector]
		public HealthComponent healthComponent;

		// Token: 0x020003FD RID: 1021
		public enum VisualState
		{
			// Token: 0x040019EC RID: 6636
			Enter,
			// Token: 0x040019ED RID: 6637
			Exit
		}
	}
}
