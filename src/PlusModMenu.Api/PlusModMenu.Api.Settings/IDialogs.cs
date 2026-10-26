using System;
using System.Collections.Generic;

namespace PlusModMenu.Api.Settings;

public interface IDialogs
{
	void Message(UIText title, UIText text);

	void Confirm(UIText title, UIText text, UIText confirmLabel, Action onConfirm);

	void Prompt(UIText title, UIText text, string initialValue, UIText confirmLabel, Action<string> onConfirm, Action onCancel = null);

	void Choose(UIText title, IReadOnlyList<UIText> options, Action<int> onChoose);
}
