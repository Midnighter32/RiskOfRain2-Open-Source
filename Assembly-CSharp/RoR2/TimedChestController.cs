using System;
using System.Text;
using EntityStates.TimedChest;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000358 RID: 856
	public sealed class TimedChestController : NetworkBehaviour, IInteractable
	{
		// Token: 0x060014C6 RID: 5318 RVA: 0x00058AA7 File Offset: 0x00056CA7
		public string GetContextString(Interactor activator)
		{
			return Language.GetString(this.contextString);
		}

		// Token: 0x060014C7 RID: 5319 RVA: 0x00058AB4 File Offset: 0x00056CB4
		public Interactability GetInteractability(Interactor activator)
		{
			if (this.purchased)
			{
				return Interactability.Disabled;
			}
			if (!this.locked)
			{
				return Interactability.Available;
			}
			return Interactability.ConditionsNotMet;
		}

		// Token: 0x060014C8 RID: 5320 RVA: 0x00058ACB File Offset: 0x00056CCB
		public void OnInteractionBegin(Interactor activator)
		{
			base.GetComponent<EntityStateMachine>().SetNextState(new Opening());
		}

		// Token: 0x060014C9 RID: 5321 RVA: 0x0000AC89 File Offset: 0x00008E89
		public bool ShouldIgnoreSpherecastForInteractibility(Interactor activator)
		{
			return false;
		}

		// Token: 0x1700027D RID: 637
		// (get) Token: 0x060014CA RID: 5322 RVA: 0x00058AE0 File Offset: 0x00056CE0
		private int remainingTime
		{
			get
			{
				float num = 0f;
				if (Run.instance)
				{
					num = Run.instance.GetRunStopwatch();
				}
				return (int)(this.lockTime - num);
			}
		}

		// Token: 0x1700027E RID: 638
		// (get) Token: 0x060014CB RID: 5323 RVA: 0x00058B13 File Offset: 0x00056D13
		private bool locked
		{
			get
			{
				return this.remainingTime <= 0;
			}
		}

		// Token: 0x060014CC RID: 5324 RVA: 0x00058B24 File Offset: 0x00056D24
		public void FixedUpdate()
		{
			if (NetworkClient.active)
			{
				if (!this.purchased)
				{
					int num = this.remainingTime;
					bool flag = num >= 0;
					bool flag2 = true;
					if (num < -599)
					{
						flag2 = ((num & 1) != 0);
						num = -599;
					}
					int num2 = flag ? num : (-num);
					uint num3 = (uint)(num2 / 60);
					uint value = (uint)(num2 - (int)(num3 * 60U));
					TimedChestController.sharedStringBuilder.Clear();
					TimedChestController.sharedStringBuilder.Append("<mspace=0.75em>");
					if (flag2)
					{
						uint num4 = 2U;
						if (!flag)
						{
							TimedChestController.sharedStringBuilder.Append("-");
							num4 = 1U;
						}
						TimedChestController.sharedStringBuilder.AppendUint(num3, num4, num4);
						TimedChestController.sharedStringBuilder.Append(":");
						TimedChestController.sharedStringBuilder.AppendUint(value, 2U, 2U);
					}
					else
					{
						TimedChestController.sharedStringBuilder.Append("--:--");
					}
					TimedChestController.sharedStringBuilder.Append("</mspace>");
					this.displayTimer.SetText(TimedChestController.sharedStringBuilder);
					this.displayTimer.color = (this.locked ? this.displayIsLockedColor : this.displayIsAvailableColor);
					this.displayTimer.SetText(TimedChestController.sharedStringBuilder);
					this.displayScaleCurve.enabled = false;
					return;
				}
				this.displayScaleCurve.enabled = true;
			}
		}

		// Token: 0x060014CD RID: 5325 RVA: 0x00058C62 File Offset: 0x00056E62
		private void OnEnable()
		{
			InstanceTracker.Add<TimedChestController>(this);
		}

		// Token: 0x060014CE RID: 5326 RVA: 0x00058C6A File Offset: 0x00056E6A
		private void OnDisable()
		{
			InstanceTracker.Remove<TimedChestController>(this);
		}

		// Token: 0x060014CF RID: 5327 RVA: 0x00058C72 File Offset: 0x00056E72
		public bool ShouldShowOnScanner()
		{
			return !this.purchased;
		}

		// Token: 0x060014D2 RID: 5330 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x060014D3 RID: 5331 RVA: 0x00058C9C File Offset: 0x00056E9C
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool result;
			return result;
		}

		// Token: 0x060014D4 RID: 5332 RVA: 0x0000409B File Offset: 0x0000229B
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
		}

		// Token: 0x0400135B RID: 4955
		public float lockTime = 600f;

		// Token: 0x0400135C RID: 4956
		public TextMeshPro displayTimer;

		// Token: 0x0400135D RID: 4957
		public ObjectScaleCurve displayScaleCurve;

		// Token: 0x0400135E RID: 4958
		public string contextString;

		// Token: 0x0400135F RID: 4959
		public Color displayIsAvailableColor;

		// Token: 0x04001360 RID: 4960
		public Color displayIsLockedColor;

		// Token: 0x04001361 RID: 4961
		public bool purchased;

		// Token: 0x04001362 RID: 4962
		private const int minTime = -599;

		// Token: 0x04001363 RID: 4963
		private static readonly StringBuilder sharedStringBuilder = new StringBuilder();
	}
}
