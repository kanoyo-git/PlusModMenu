namespace ModMenu.Hl1;

public sealed class Hl1Stepper
{
	private double _accumulated;

	public int Advance(float frameSeconds)
	{
		_accumulated += frameSeconds;
		var ticks = (int)(_accumulated / Hl1Constants.TickSeconds + 1e-6);
		if (ticks > Hl1Constants.MaxTicksPerFrame)
		{
			_accumulated = 0;
			return Hl1Constants.MaxTicksPerFrame;
		}
		_accumulated -= ticks * (double)Hl1Constants.TickSeconds;
		return ticks;
	}

	public void Reset() => _accumulated = 0;
}
