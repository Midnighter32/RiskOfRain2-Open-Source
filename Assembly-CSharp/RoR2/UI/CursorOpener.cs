using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x020005B0 RID: 1456
	[RequireComponent(typeof(MPEventSystemLocator))]
	public class CursorOpener : MonoBehaviour
	{
		// Token: 0x170003A1 RID: 929
		// (get) Token: 0x0600228C RID: 8844 RVA: 0x000958D4 File Offset: 0x00093AD4
		// (set) Token: 0x0600228D RID: 8845 RVA: 0x000958E4 File Offset: 0x00093AE4
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

		// Token: 0x0600228E RID: 8846 RVA: 0x00095A10 File Offset: 0x00093C10
		private void Awake()
		{
			this.eventSystemLocator = base.GetComponent<MPEventSystemLocator>();
			this.openForAllEventSystems = this.eventSystemLocator.eventSystemProvider.fallBackToMainEventSystem;
		}

		// Token: 0x0600228F RID: 8847 RVA: 0x00095A34 File Offset: 0x00093C34
		private void Start()
		{
			this.opening = true;
		}

		// Token: 0x06002290 RID: 8848 RVA: 0x00095A34 File Offset: 0x00093C34
		private void OnEnable()
		{
			this.opening = true;
		}

		// Token: 0x06002291 RID: 8849 RVA: 0x00095A3D File Offset: 0x00093C3D
		private void OnDisable()
		{
			this.opening = false;
		}

		// Token: 0x06002292 RID: 8850 RVA: 0x00095A46 File Offset: 0x00093C46
		[AssetCheck(typeof(CursorOpener))]
		private static void CheckCursorOpener(AssetCheckArgs args)
		{
			if (!((CursorOpener)args.asset).GetComponent<MPEventSystemLocator>())
			{
				args.Log("Missing MPEventSystemLocator.", null);
			}
		}

		// Token: 0x04001FF9 RID: 8185
		private MPEventSystemLocator eventSystemLocator;

		// Token: 0x04001FFA RID: 8186
		private MPEventSystem linkedEventSystem;

		// Token: 0x04001FFB RID: 8187
		private bool openForAllEventSystems;
	}
}
