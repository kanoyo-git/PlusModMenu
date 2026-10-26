namespace ModMenu.Settings;

public enum EscAction
{
	ClosePopup,
	CancelInput,
	ClearSearch,
	CloseMenu
}

public static class MenuEscape
{
	public static EscAction Next(bool popupOpen, bool inputFocused, bool searchHasText)
	{
		if (popupOpen)
		{
			return EscAction.ClosePopup;
		}
		if (inputFocused)
		{
			return EscAction.CancelInput;
		}
		return searchHasText ? EscAction.ClearSearch : EscAction.CloseMenu;
	}
}
