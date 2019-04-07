using System;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x02000610 RID: 1552
	public class Nameplate : MonoBehaviour
	{
		// Token: 0x06002304 RID: 8964 RVA: 0x000A4F08 File Offset: 0x000A3108
		public void SetBody(CharacterBody body)
		{
			this.body = body;
		}

		// Token: 0x06002305 RID: 8965 RVA: 0x000A4F14 File Offset: 0x000A3114
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

		// Token: 0x040025EF RID: 9711
		public TextMeshPro label;

		// Token: 0x040025F0 RID: 9712
		private CharacterBody body;

		// Token: 0x040025F1 RID: 9713
		public GameObject aliveObject;

		// Token: 0x040025F2 RID: 9714
		public GameObject deadObject;

		// Token: 0x040025F3 RID: 9715
		public SpriteRenderer criticallyHurtSpriteRenderer;

		// Token: 0x040025F4 RID: 9716
		public SpriteRenderer[] coloredSprites;

		// Token: 0x040025F5 RID: 9717
		public Color baseColor;

		// Token: 0x040025F6 RID: 9718
		public Color combatColor;
	}
}
