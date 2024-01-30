#ifndef NINE_SLICE_INCLUDED
#define NINE_SLICE_INCLUDED

float map(float value, float originalMin, float originalMax, float newMin, float newMax) {
    return (value - originalMin) / (originalMax - originalMin) * (newMax - newMin) + newMin;
} 

float process_axis(float coordinate, float pixel, float texel, float start, float end) {
	if (coordinate > 1.0 - end * pixel) {
		return map(coordinate, 1.0 - end * pixel, 1.0, 1.0 - texel * end, 1.0);
	} else if (coordinate > start * pixel) {
		return map(coordinate, start * pixel, 1.0 - end * pixel, start * texel, 1.0 - end * texel);
	} else {
		return map(coordinate, 0.0, start * pixel, 0.0, start * texel);
	}
}

void nine_slice_uv_float(float2 uv, float2 scale, float2 texel, float4 border, out float2 out_uv)
{
	float2 pixel_size = texel / scale;

	out_uv = float2
	(
		process_axis(uv.x, pixel_size.x, texel.x, border.r, border.b),
		process_axis(uv.y, pixel_size.y, texel.y, border.g, border.a)
	);
}

void nine_slice_uv_half(half2 uv, half2 scale, half2 texel, half4 border, out half2 out_uv)
{
	half2 pixel_size = texel / scale;

	out_uv = half2
	(
		process_axis(uv.x, pixel_size.x, texel.x, border.r, border.b),
		process_axis(uv.y, pixel_size.y, texel.y, border.g, border.a)
	);
}

#endif