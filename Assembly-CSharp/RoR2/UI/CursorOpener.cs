using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x020005D1 RID: 1489
	[RequireComponent(typeof(MPEventSystemLocator))]
	public class CursorOpener : MonoBehaviour
	{
		// Token: 0x170002ED RID: 749
		// (get) Token: 0x0600215C RID: 8540 RVA: 0x0009C97C File Offset: 0x0009AB7C
		// (set) Token: 0x0600215D RID: 8541 RVA: 0x0009C98C File Offset: 0x0009AB8C
		private bool opening
		{
			get
			{
				return this.linkedEventSystem;
			}
			set
			{
				MPEventSystem eventSystem = this.eventSystemLocator.eventSystem;
				if (value && this.linkedEventSystem != eventSystem && this.linkedEventSystem)
				{
					this.opening = false;
				}
				if (this.linkedEventSystem != value)
				{
					if (value)
					{
						if (this.eventSystemLocator.eventSystem)
						{
							if (this.openForAllEventSystems)
							{
								using (IEnumerator<MPEventSystem> enumerator = MPEventSystem.readOnlyInstancesList.GetEnumerator())
								{
									while (enumerator.MoveNext())
									{
										MPEventSystem mpeventSystem = enumerator.Current;
										mpeventSystem.cursorOpenerCount++;
									}
									goto IL_A4;
								}
							}
							eventSystem.cursorOpenerCount++;
							IL_A4:
							this.linkedEventSystem = eventSystem;
							return;
						}
					}
					else
					{
						if (this.openForAllEventSystems)
						{
							using (IEnumerator<MPEventSystem> enumerator = MPEventSystem.readOnlyInstancesList.GetEnumerator())
							{
								while (enumerator.MoveNext())
								{
									MPEventSystem mpeventSystem2 = enumerator.Current;
									mpeventSystem2.cursorOpenerCount--;
								}
								goto IL_FB;
							}
						}
						this.linkedEventSystem.cursorOpenerCount--;
						IL_FB:
						this.linkedEventSystem = null;
					}
				}
			}
		}

		// Token: 0x0600215E RID: 8542 RVA: 0x0009CAB8 File Offset: 0x0009ACB8
		private void Awake()
		{
			this.eventSystemLocator = base.GetComponent<MPEventSystemLocator>();
			this.openForAllEventSystems = this.eventSystemLocator.eventSystemProvider.fallBackToMainEventSystem;
		}

		// Token: 0x0600215F RID: 8543 RVA: 0x0009CADC File Offset: 0x0009ACDC
		private void Start()
		{
			this.opening = true;
		}

		// Token: 0x06002160 RID: 8544 RVA: 0x0009CADC File Offset: 0x0009ACDC
		private void OnEnable()
		{
			this.opening = true;
		}

		// Token: 0x06002161 RID: 8545 RVA: 0x0009CAE5 File Offset: 0x0009ACE5
		private void OnDisable()
		{
			this.opening = false;
		}

		// Token: 0x06002162 RID: 8546 RVA: 0x0009CAEE File Offset: 0x0009ACEE
		[AssetCheck(typeof(CursorOpener))]
		private static void CheckCursorOpener(ProjectIssueChecker projectIssueChecker, UnityEngine.Object asset)
		{
			if (!((CursorOpener)asset).GetComponent<MPEventSystemLocator>())
			{
				projectIssueChecker.Log("Missing MPEventSystemLocator.", null);
			}
		}

		// Token: 0x040023EB RID: 9195
		private MPEventSystemLocator eventSystemLocator;

		// Token: 0x040023EC RID: 9196
		private MPEventSystem linkedEventSystem;

		// Token: 0x040023ED RID: 9197
		private bool openForAllEventSystems;
	}
}
