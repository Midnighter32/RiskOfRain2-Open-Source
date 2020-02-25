using System;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x020005FF RID: 1535
	public class Nameplate : MonoBehaviour
	{
		// Token: 0x06002474 RID: 9332 RVA: 0x0009F078 File Offset: 0x0009D278
		public void SetBody(CharacterBody body)
		{
			this.body = body;
		}

		// Token: 0x06002475 RID: 9333 RVA: 0x0009F084 File Offset: 0x0009D284
		private void LateUpdate()
		{
			string text = "";
			Color color = this.baseColor;
			bool flag = true;
			bool flag2 = false;
			bool flag3 = false;
			if (this.body)
			{
				text = this.body.GetDisplayName();
				flag = this.body.healthComponent.alive;
				flag2 = (!this.body.outOfCombat || !this.body.outOfDanger);
				flag3 = (this.body.healthComponent.combinedHealthFraction < HealthBar.criticallyHurtThreshold);
				CharacterMaster master = this.body.master;
				if (master)
				{
					PlayerCharacterMasterController component = master.GetComponent<PlayerCharacterMasterController>();
					if (component)
					{
						GameObject networkUserObject = component.networkUserObject;
						if (networkUserObject)
						{
							NetworkUser component2 = networkUserObject.GetComponent<NetworkUser>();
							if (component2)
							{
								text = component2.userName;
							}
						}
					}
					else
					{
						text = Language.GetString(this.body.baseNameToken);
					}
				}
			}
			color = (flag2 ? this.combatColor : this.baseColor);
			this.aliveObject.SetActive(flag);
			this.deadObject.SetActive(!flag);
			if (this.criticallyHurtSpriteRenderer)
			{
				this.criticallyHurtSpriteRenderer.enabled = (flag3 && flag);
				this.criticallyHurtSpriteRenderer.color = HealthBar.GetCriticallyHurtColor();
			}
			if (this.label)
			{
				this.label.text = text;
				this.label.color = color;
			}
			SpriteRenderer[] array = this.coloredSprites;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].color = color;
			}
		}

		// Token: 0x04002233 RID: 8755
		public TextMeshPro label;

		// Token: 0x04002234 RID: 8756
		private CharacterBody body;

		// Token: 0x04002235 RID: 8757
		public GameObject aliveObject;

		// Token: 0x04002236 RID: 8758
		public GameObject deadObject;

		// Token: 0x04002237 RID: 8759
		public SpriteRenderer criticallyHurtSpriteRenderer;

		// Token: 0x04002238 RID: 8760
		public SpriteRenderer[] coloredSprites;

		// Token: 0x04002239 RID: 8761
		public Color baseColor;

		// Token: 0x0400223A RID: 8762
		public Color combatColor;
	}
}
