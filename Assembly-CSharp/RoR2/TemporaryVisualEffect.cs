using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000354 RID: 852
	public class TemporaryVisualEffect : MonoBehaviour
	{
		// Token: 0x060014BB RID: 5307 RVA: 0x00058851 File Offset: 0x00056A51
		private void Start()
		{
			this.RebuildVisualComponents();
		}

		// Token: 0x060014BC RID: 5308 RVA: 0x00058859 File Offset: 0x00056A59
		private void FixedUpdate()
		{
			if (this.previousVisualState != this.visualState)
			{
				this.RebuildVisualComponents();
			}
			this.previousVisualState = this.visualState;
		}

		// Token: 0x060014BD RID: 5309 RVA: 0x0005887C File Offset: 0x00056A7C
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

		// Token: 0x060014BE RID: 5310 RVA: 0x00058914 File Offset: 0x00056B14
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

		// Token: 0x0400134A RID: 4938
		public float radius = 1f;

		// Token: 0x0400134B RID: 4939
		public Transform parentTransform;

		// Token: 0x0400134C RID: 4940
		public Transform visualTransform;

		// Token: 0x0400134D RID: 4941
		public MonoBehaviour[] enterComponents;

		// Token: 0x0400134E RID: 4942
		public MonoBehaviour[] exitComponents;

		// Token: 0x0400134F RID: 4943
		public TemporaryVisualEffect.VisualState visualState;

		// Token: 0x04001350 RID: 4944
		private TemporaryVisualEffect.VisualState previousVisualState;

		// Token: 0x04001351 RID: 4945
		[HideInInspector]
		public HealthComponent healthComponent;

		// Token: 0x02000355 RID: 853
		public enum VisualState
		{
			// Token: 0x04001353 RID: 4947
			Enter,
			// Token: 0x04001354 RID: 4948
			Exit
		}
	}
}
