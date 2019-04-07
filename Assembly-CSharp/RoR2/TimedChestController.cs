using System;
using System.Text;
using EntityStates.TimedChest;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000400 RID: 1024
	public class TimedChestController : NetworkBehaviour, IInteractable
	{
		// Token: 0x060016C7 RID: 5831 RVA: 0x0006C7DF File Offset: 0x0006A9DF
		public string GetContextString(Interactor activator)
		{
			return Language.GetString(this.contextString);
		}

		// Token: 0x060016C8 RID: 5832 RVA: 0x0006C7EC File Offset: 0x0006A9EC
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

		// Token: 0x060016C9 RID: 5833 RVA: 0x0006C803 File Offset: 0x0006AA03
		public void OnInteractionBegin(Interactor activator)
		{
			base.GetComponent<EntityStateMachine>().SetNextState(new Opening());
		}

		// Token: 0x060016CA RID: 5834 RVA: 0x0000A1ED File Offset: 0x000083ED
		public bool ShouldIgnoreSpherecastForInteractibility(Interactor activator)
		{
			return false;
		}

		// Token: 0x17000218 RID: 536
		// (get) Token: 0x060016CB RID: 5835 RVA: 0x0006C815 File Offset: 0x0006AA15
		private int remainingTime
		{
			get
			{
				return (int)(this.lockTime - Run.instance.time);
			}
		}

		// Token: 0x17000219 RID: 537
		// (get) Token: 0x060016CC RID: 5836 RVA: 0x0006C829 File Offset: 0x0006AA29
		private bool locked
		{
			get
			{
				return this.remainingTime <= 0;
			}
		}

		// Token: 0x060016CD RID: 5837 RVA: 0x0006C838 File Offset: 0x0006AA38
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
					uint value = (uint)(num2 - (int)(num3 * 60u));
					TimedChestController.sharedStringBuilder.Clear();
					TimedChestController.sharedStringBuilder.Append("<mspace=2.5em>");
					if (flag2)
					{
						uint num4 = 2u;
						if (!flag)
						{
							TimedChestController.sharedStringBuilder.Append("-");
							num4 = 1u;
						}
						TimedChestController.sharedStringBuilder.AppendUint(num3, num4, num4);
						TimedChestController.sharedStringBuilder.Append(":");
						TimedChestController.sharedStringBuilder.AppendUint(value, 2u, 2u);
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

		// Token: 0x060016D0 RID: 5840 RVA: 0x00004507 File Offset: 0x00002707
		private void UNetVersion()
		{
		}

		// Token: 0x060016D1 RID: 5841 RVA: 0x0006C994 File Offset: 0x0006AB94
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool result;
			return result;
		}

		// Token: 0x060016D2 RID: 5842 RVA: 0x00004507 File Offset: 0x00002707
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
		}

		// Token: 0x040019F4 RID: 6644
		public float lockTime = 600f;

		// Token: 0x040019F5 RID: 6645
		public TextMeshPro displayTimer;

		// Token: 0x040019F6 RID: 6646
		public ObjectScaleCurve displayScaleCurve;

		// Token: 0x040019F7 RID: 6647
		public string contextString;

		// Token: 0x040019F8 RID: 6648
		public Color displayIsAvailableColor;

		// Token: 0x040019F9 RID: 6649
		public Color displayIsLockedColor;

		// Token: 0x040019FA RID: 6650
		public bool purchased;

		// Token: 0x040019FB RID: 6651
		private const int minTime = -599;

		// Token: 0x040019FC RID: 6652
		private static readonly StringBuilder sharedStringBuilder = new StringBuilder();
	}
}
