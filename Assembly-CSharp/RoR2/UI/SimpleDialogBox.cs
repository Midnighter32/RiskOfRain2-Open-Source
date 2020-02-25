using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace RoR2.UI
{
	// Token: 0x0200062B RID: 1579
	[RequireComponent(typeof(MPEventSystemLocator))]
	public class SimpleDialogBox : UIBehaviour
	{
		// Token: 0x0600254C RID: 9548 RVA: 0x000A25C3 File Offset: 0x000A07C3
		protected override void OnEnable()
		{
			base.OnEnable();
			SimpleDialogBox.instancesList.Add(this);
		}

		// Token: 0x0600254D RID: 9549 RVA: 0x000A25D6 File Offset: 0x000A07D6
		protected override void OnDisable()
		{
			SimpleDialogBox.instancesList.Remove(this);
			base.OnDisable();
		}

		// Token: 0x0600254E RID: 9550 RVA: 0x000A25EC File Offset: 0x000A07EC
		private static string GetString(SimpleDialogBox.TokenParamsPair pair)
		{
			string text = Language.GetString(pair.token);
			if (pair.formatParams != null && pair.formatParams.Length != 0)
			{
				text = string.Format(text, pair.formatParams);
			}
			return text;
		}

		// Token: 0x170003DA RID: 986
		// (set) Token: 0x0600254F RID: 9551 RVA: 0x000A2624 File Offset: 0x000A0824
		public SimpleDialogBox.TokenParamsPair headerToken
		{
			set
			{
				this.headerLabel.text = SimpleDialogBox.GetString(value);
			}
		}

		// Token: 0x170003DB RID: 987
		// (set) Token: 0x06002550 RID: 9552 RVA: 0x000A2637 File Offset: 0x000A0837
		public SimpleDialogBox.TokenParamsPair descriptionToken
		{
			set
			{
				this.descriptionLabel.text = SimpleDialogBox.GetString(value);
			}
		}

		// Token: 0x06002551 RID: 9553 RVA: 0x000A264C File Offset: 0x000A084C
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

		// Token: 0x06002552 RID: 9554 RVA: 0x000A26BC File Offset: 0x000A08BC
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

		// Token: 0x06002553 RID: 9555 RVA: 0x000A26F1 File Offset: 0x000A08F1
		public MPButton AddCancelButton(string displayToken, params object[] formatParams)
		{
			return this.AddButton(delegate
			{
				UnityEngine.Object.Destroy(this.rootObject);
			}, displayToken, formatParams);
		}

		// Token: 0x06002554 RID: 9556 RVA: 0x000A2708 File Offset: 0x000A0908
		public MPButton AddActionButton(UnityAction action, string displayToken, params object[] formatParams)
		{
			return this.AddButton(delegate
			{
				action();
				UnityEngine.Object.Destroy(this.rootObject);
			}, displayToken, formatParams);
		}

		// Token: 0x06002555 RID: 9557 RVA: 0x000A2740 File Offset: 0x000A0940
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

		// Token: 0x06002556 RID: 9558 RVA: 0x000A278A File Offset: 0x000A098A
		private void Update()
		{
			this.buttonContainer.gameObject.SetActive(this.buttonContainer.childCount > 0);
		}

		// Token: 0x06002557 RID: 9559 RVA: 0x000A27AC File Offset: 0x000A09AC
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

		// Token: 0x040022FF RID: 8959
		public static readonly List<SimpleDialogBox> instancesList = new List<SimpleDialogBox>();

		// Token: 0x04002300 RID: 8960
		public GameObject rootObject;

		// Token: 0x04002301 RID: 8961
		public GameObject buttonPrefab;

		// Token: 0x04002302 RID: 8962
		public RectTransform buttonContainer;

		// Token: 0x04002303 RID: 8963
		public TextMeshProUGUI headerLabel;

		// Token: 0x04002304 RID: 8964
		public TextMeshProUGUI descriptionLabel;

		// Token: 0x04002305 RID: 8965
		private MPButton defaultChoice;

		// Token: 0x04002306 RID: 8966
		public object[] descriptionFormatParameters;

		// Token: 0x0200062C RID: 1580
		public struct TokenParamsPair
		{
			// Token: 0x0600255B RID: 9563 RVA: 0x000A2814 File Offset: 0x000A0A14
			public TokenParamsPair(string token, params object[] formatParams)
			{
				this.token = token;
				this.formatParams = formatParams;
			}

			// Token: 0x04002307 RID: 8967
			public string token;

			// Token: 0x04002308 RID: 8968
			public object[] formatParams;
		}
	}
}
