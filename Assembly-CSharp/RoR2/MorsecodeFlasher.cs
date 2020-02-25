using System;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200028E RID: 654
	public class MorsecodeFlasher : MonoBehaviour
	{
		// Token: 0x06000E8A RID: 3722 RVA: 0x0004080B File Offset: 0x0003EA0B
		private void FixedUpdate()
		{
			this.age -= Time.fixedDeltaTime;
			if (this.age <= 0f)
			{
				this.age = this.messageRepeatDelay;
				this.PlayMorseCodeMessage(this.morsecodeMessage);
			}
		}

		// Token: 0x06000E8B RID: 3723 RVA: 0x00040844 File Offset: 0x0003EA44
		public void PlayMorseCodeMessage(string message)
		{
			base.StartCoroutine("_PlayMorseCodeMessage", message);
		}

		// Token: 0x06000E8C RID: 3724 RVA: 0x00040853 File Offset: 0x0003EA53
		private IEnumerator _PlayMorseCodeMessage(string message)
		{
			Regex regex = new Regex("[^A-z0-9 ]");
			message = regex.Replace(message.ToUpper(), "");
			foreach (char c in message)
			{
				if (c == ' ')
				{
					yield return new WaitForSeconds(this.spaceDelay);
				}
				else
				{
					int num = (int)(c - 'A');
					if (num < 0)
					{
						num = (int)(c - '0' + '\u001a');
					}
					string text2 = this.alphabet[num];
					foreach (int num2 in text2)
					{
						float num3 = this.dotDuration;
						if (num2 == 45)
						{
							num3 = this.dashDuration;
						}
						base.StartCoroutine("_FlashMorseCodeObject", num3);
						yield return new WaitForSeconds(num3 + this.delayBetweenCharacters);
					}
					string text3 = null;
				}
			}
			string text = null;
			yield break;
		}

		// Token: 0x06000E8D RID: 3725 RVA: 0x00040869 File Offset: 0x0003EA69
		private IEnumerator _FlashMorseCodeObject(float duration)
		{
			this.flashRootObject.SetActive(true);
			yield return new WaitForSeconds(duration);
			this.flashRootObject.SetActive(false);
			yield break;
		}

		// Token: 0x04000E6E RID: 3694
		private string[] alphabet = new string[]
		{
			".-",
			"-...",
			"-.-.",
			"-..",
			".",
			"..-.",
			"--.",
			"....",
			"..",
			".---",
			"-.-",
			".-..",
			"--",
			"-.",
			"---",
			".--.",
			"--.-",
			".-.",
			"...",
			"-",
			"..-",
			"...-",
			".--",
			"-..-",
			"-.--",
			"--..",
			"-----",
			".----",
			"..---",
			"...--",
			"....-",
			".....",
			"-....",
			"--...",
			"---..",
			"----."
		};

		// Token: 0x04000E6F RID: 3695
		public string morsecodeMessage;

		// Token: 0x04000E70 RID: 3696
		public float spaceDelay;

		// Token: 0x04000E71 RID: 3697
		public float delayBetweenCharacters;

		// Token: 0x04000E72 RID: 3698
		public float dotDuration;

		// Token: 0x04000E73 RID: 3699
		public float dashDuration;

		// Token: 0x04000E74 RID: 3700
		public float messageRepeatDelay;

		// Token: 0x04000E75 RID: 3701
		public GameObject flashRootObject;

		// Token: 0x04000E76 RID: 3702
		private float age;
	}
}
