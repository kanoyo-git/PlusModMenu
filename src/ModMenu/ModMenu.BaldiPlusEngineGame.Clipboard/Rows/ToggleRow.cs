using ModMenu.Settings;
using UnityEngine;
using UnityEngine.UI;

namespace ModMenu.BaldiPlusEngineGame.Clipboard.Rows;

internal sealed class ToggleRow : SettingRow
{
	private Image _check;

	public bool Shown => _check.enabled;

	protected override void Build()
	{
		var box = UiFactory.Image("Box", Control, GameAssets.Sprite(AssetNames.CheckBox));
		box.rectTransform.Place(Vector2.zero, new Vector2(32f, 32f));
		var check = GameAssets.Sprite(AssetNames.Check);
		_check = UiFactory.Image("Check", box.rectTransform, check);
		_check.rectTransform.Place(new Vector2(5f, 6f), check.rect.size);
		var hit = UiFactory.Rect("HitArea", Rect);
		hit.Stretch();
		hit.SetSiblingIndex(0);
		UiFactory.Clickable(UiFactory.HitArea(hit), Flip).PressTarget = box.rectTransform;
	}

	protected override void OnLabelClick()
	{
		Flip();
	}

	public void Flip()
	{
		if (KeptManager.AltHeld && SettingKeep.CanKeep(Entry))
		{
			KeptManager.Flip(Entry);
			return;
		}
		if (Entry.Available)
		{
			Entry.SetValue(!(Entry.GetValue() is true));
		}
	}

	public override void ReadValue()
	{
		_check.enabled = Entry.GetValue() is true;
	}
}
