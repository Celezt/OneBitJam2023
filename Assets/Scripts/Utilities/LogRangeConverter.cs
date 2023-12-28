using Unity.Mathematics;

/// <summary>
/// Tool to convert a range from 0-1 into a logarithmic range with a user defined center.
/// </summary>
/// <see cref="https://gist.github.com/bartofzo/6ad28a05ba9fc82e10a64f0c121c5c24"/>
public readonly struct LogRangeConverter
{
    public readonly float MinValue;
    public readonly float MaxValue;

    private readonly float _a;
    private readonly float _b;
    private readonly float _c;

    /// <summary>
    /// Set up a scaler
    /// </summary>
    /// <param name="minValue">Value for t = 0.</param>
    /// <param name="centerValue">Value for t = 0.5.</param>
    /// <param name="maxValue">Value for t = 1.0.</param>
    public LogRangeConverter(float minValue, float centerValue, float maxValue)
    {
        this.MinValue = minValue;
        this.MaxValue = maxValue;

        _a = (minValue * maxValue - (centerValue * centerValue)) / (minValue - 2 * centerValue + maxValue);
        _b = ((centerValue - minValue) * (centerValue - minValue)) / (minValue - 2 * centerValue + maxValue);
        _c = 2 * math.log((maxValue - centerValue) / (centerValue - minValue));
    }

    /// <summary>
    /// Convers the value in range 0 - 1 to the value in range of minValue - maxValue.
    /// </summary>
    public float ToRange(float value01)
    {
        return _a + _b * math.exp(_c * value01);
    }

    /// <summary>
    /// Converts the value in range min-max to a value between 0 and 1 that can be used for a slider.
    /// </summary>
    public float ToNormalized(float rangeValue)
    {
        return math.log((rangeValue - _a) / _b) / _c;
    }
}
