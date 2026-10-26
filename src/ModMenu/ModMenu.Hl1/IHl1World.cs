namespace ModMenu.Hl1;

public interface IHl1World
{
	float? GroundUnder(float feet, float hullHeight);

	float CeilingAbove(float feet, float hullHeight);

	bool HullClear(float feet, float hullHeight);

	bool EdgeAhead(float feet, Hl1Vec flatDirection);
}
