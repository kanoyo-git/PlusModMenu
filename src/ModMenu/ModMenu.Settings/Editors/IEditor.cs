namespace ModMenu.Settings.Editors;

public interface IEditor
{
	void Initialize(IMetadata metadata);

	void SetValueWithoutNotify(object input);
}
