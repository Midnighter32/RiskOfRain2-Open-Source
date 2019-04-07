using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace RoR2.UI
{
	// Token: 0x02000638 RID: 1592
	[RequireComponent(typeof(MPEventSystemLocator))]
	public class SimpleDialogBox : UIBehaviour
	{
		// Token: 0x060023B3 RID: 9139 RVA: 0x000A7D4B File Offset: 0x000A5F4B
		protected override void OnEnable()
		{
			base.OnEnable();
			SimpleDialogBox.instancesList.Add(this);
		}

		// Token: 0x060023B4 RID: 9140 RVA: 0x000A7D5E File Offset: 0x000A5F5E
		protected override void OnDisable()
		{
			SimpleDialogBox.instancesList.Remove(this);
			base.OnDisable();
		}

		// Token: 0x060023B5 RID: 9141 RVA: 0x000A7D74 File Offset: 0x000A5F74
		private static string GetString(SimpleDialogBox.TokenParamsPair pair)
		{
			string text = Language.GetString(pair.token);
			if (pair.formatParams != null && pair.formatParams.Length != 0)
			{
				text = string.Format(text, pair.formatParams);
			}
			return text;
		}

		// Token: 0x1700031B RID: 795
		// (set) Token: 0x060023B6 RID: 9142 RVA: 0x000A7DAC File Offset: 0x000A5FAC
		public SimpleDialogBox.TokenParamsPair headerToken
		{
			set
			{
				this.headerLabel.text = SimpleDialogBox.GetString(value);
			}
		}

		// Token: 0x1700031C RID: 796
		// (set) Token: 0x060023B7 RID: 9143 RVA: 0x000A7DBF File Offset: 0x000A5FBF
		public SimpleDialogBox.TokenParamsPair descriptionToken
		{
			set
			{
				this.descriptionLabel.text = SimpleDialogBox.GetString(value);
			}
		}

		// Token: 0x060023B8 RID: 9144 RVA: 0x000A7DD4 File Offset: 0x000A5FD4
		private MPButton AddButton(UnityAction callback, string displayToken, params object[] formatParams)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.buttonPrefab, this.buttonContainer);
			string text = Language.GetString(displayToken);
			if (formatParams != null && formatParams.Length != 0)
			{
				text = string.Format(text, formatParams);
			}
			gameObject.GetComponentInChildren<TextMeshProUGUI>().text = text;
			MPButton component = gameObject.GetComponent<MPButton>();
			component.onClick.AddListener(callback);
			gameObject.SetActive(true);
			if (!this.defaultChoice)
			{
				this.defaultChoice = component;
			}
			return component;
		}

		// Token: 0x060023B9 RID: 9145 RVA: 0x000A7E44 File Offset: 0x000A6044
		public MPButton AddCommandButton(string consoleString, string displayToken, params object[] formatParams)
		{
			return this.AddButton(delegate
			{
				if (!string.IsNullOrEmpty(consoleString))
				{
					Console.instance.SubmitCmd(null, consoleString, true);
				}
				UnityEngine.Object.Destroy(this.rootObject);
			}, displayToken, formatParams);
		}

		// Token: 0x060023BA RID: 9146 RVA: 0x000A7E79 File Offset: 0x000A6079
		public MPButton AddCancelButton(string displayToken, params object[] formatParams)
		{
			return this.AddButton(delegate
			{
				UnityEngine.Object.Destroy(this.rootObject);
			}, displayToken, formatParams);
		}

		// Token: 0x060023BB RID: 9147 RVA: 0x000A7E90 File Offset: 0x000A6090
		public MPButton AddActionButton(UnityAction action, string displayToken, params object[] formatParams)
		{
			return this.AddButton(delegate
			{
				action();
				UnityEngine.Object.Destroy(this.rootObject);
			}, displayToken, formatParams);
		}

		// Token: 0x060023BC RID: 9148 RVA: 0x000A7EC8 File Offset: 0x000A60C8
		protected override void Start()
		{
			base.Start();
			if (this.defaultChoice)
			{
				MPEventSystemLocator component = base.GetComponent<MPEventSystemLocator>();
				if (component.eventSystem)
				{
					component.eventSystem.SetSelectedGameObject(this.defaultChoice.gameObject);
				}
			}
		}

		// Token: 0x060023BD RID: 9149 RVA: 0x000A7F12 File Offset: 0x000A6112
		private void Update()
		{
			this.buttonContainer.gameObject.SetActive(this.buttonContainer.childCount > 0);
		}

		// Token: 0x060023BE RID: 9150 RVA: 0x000A7F34 File Offset: 0x000A6134
		public static SimpleDialogBox Create(MPEventSystem owner = null)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/UI/SimpleDialogRoot"));
			if (owner)
			{
				MPEventSystemProvider component = gameObject.GetComponent<MPEventSystemProvider>();
				component.eventSystem = owner;
				component.fallBackToMainEventSystem = false;
				component.eventSystem.SetSelectedGameObject(null);
			}
			return gameObject.transform.GetComponentInChildren<SimpleDialogBox>();
		}

		// Token: 0x040026A2 RID: 9890
		public static readonly List<SimpleDialogBox> instancesList = new List<SimpleDialogBox>();

		// Token: 0x040026A3 RID: 9891
		public GameObject rootObject;

		// Token: 0x040026A4 RID: 9892
		public GameObject buttonPrefab;

		// Token: 0x040026A5 RID: 9893
		public RectTransform buttonContainer;

		// Token: 0x040026A6 RID: 9894
		public TextMeshProUGUI headerLabel;

		// Token: 0x040026A7 RID: 9895
		public TextMeshProUGUI descriptionLabel;

		// Token: 0x040026A8 RID: 9896
		private MPButton defaultChoice;

		// Token: 0x040026A9 RID: 9897
		public object[] descriptionFormatParameters;

		// Token: 0x02000639 RID: 1593
		public struct TokenParamsPair
		{
			// Token: 0x060023C2 RID: 9154 RVA: 0x000A7F9C File Offset: 0x000A619C
			public TokenParamsPair(string token, params object[] formatParams)
			{
				this.token = token;
				this.formatParams = formatParams;
			}

			// Token: 0x040026AA RID: 9898
			public string token;

			// Token: 0x040026AB RID: 9899
			public object[] formatParams;
		}
	}
}
