shader_type spatial;

render_mode unshaded, blend_mix;

uniform vec3 uv1_scale = vec3(1.0, 1.0, 1.0);
uniform vec3 uv1_offset = vec3(0.0, 0.0, 0.0);
uniform float variation = 0.0;
varying float elapsed_time;
void vertex() {
	elapsed_time = TIME;
	UV = UV*uv1_scale.xy+uv1_offset.xy;
}
float rand(vec2 x) {
    return fract(cos(mod(dot(x, vec2(13.9898, 8.141)), 3.14)) * 43758.5453);
}
vec2 rand2(vec2 x) {
    return fract(cos(mod(vec2(dot(x, vec2(13.9898, 8.141)),
						      dot(x, vec2(3.4562, 17.398))), vec2(3.14))) * 43758.5453);
}
vec3 rand3(vec2 x) {
    return fract(cos(mod(vec3(dot(x, vec2(13.9898, 8.141)),
							  dot(x, vec2(3.4562, 17.398)),
                              dot(x, vec2(13.254, 5.867))), vec3(3.14))) * 43758.5453);
}
float param_rnd(float minimum, float maximum, float seed) {
	return minimum+(maximum-minimum)*rand(vec2(seed));
}
vec3 rgb2hsv(vec3 c) {
	vec4 K = vec4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
	vec4 p = c.g < c.b ? vec4(c.bg, K.wz) : vec4(c.gb, K.xy);
	vec4 q = c.r < p.x ? vec4(p.xyw, c.r) : vec4(c.r, p.yzx);
	float d = q.x - min(q.w, q.y);
	float e = 1.0e-10;
	return vec3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
}
vec3 hsv2rgb(vec3 c) {
	vec4 K = vec4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
	vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
	return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}
uniform sampler2D texture_1;
const float texture_1_size = 1024.0;
float white_noise(vec2 uv, float size, float seed) {
	vec2 seed2 = rand2(vec2(seed, 1.0-seed));
	uv /= size;
	vec2 point_pos = floor(uv)+vec2(0.5);
	float color = rand(seed2+point_pos);
	return color;
}
vec2 transform2_clamp(vec2 uv) {
	return clamp(uv, vec2(0.0), vec2(1.0));
}
vec2 transform2(vec2 uv, vec2 translate, float rotate, vec2 scale) {
 	vec2 rv;
	uv -= translate;
	uv -= vec2(0.5);
	rv.x = cos(rotate)*uv.x + sin(rotate)*uv.y;
	rv.y = -sin(rotate)*uv.x + cos(rotate)*uv.y;
	rv /= scale;
	rv += vec2(0.5);
	return rv;	
}
float pingpong(float a, float b)
{
  return (b != 0.0) ? abs(fract((a - b) / (b * 2.0)) * b * 2.0 - b) : 0.0;
}
vec3 blend_normal(vec2 uv, vec3 c1, vec3 c2, float opacity) {
	return opacity*c1 + (1.0-opacity)*c2;
}
vec3 blend_dissolve(vec2 uv, vec3 c1, vec3 c2, float opacity) {
	if (rand(uv) < opacity) {
		return c1;
	} else {
		return c2;
	}
}
vec3 blend_multiply(vec2 uv, vec3 c1, vec3 c2, float opacity) {
	return opacity*c1*c2 + (1.0-opacity)*c2;
}
vec3 blend_screen(vec2 uv, vec3 c1, vec3 c2, float opacity) {
	return opacity*(1.0-(1.0-c1)*(1.0-c2)) + (1.0-opacity)*c2;
}
float blend_overlay_f(float c1, float c2) {
	return (c1 < 0.5) ? (2.0*c1*c2) : (1.0-2.0*(1.0-c1)*(1.0-c2));
}
vec3 blend_overlay(vec2 uv, vec3 c1, vec3 c2, float opacity) {
	return opacity*vec3(blend_overlay_f(c1.x, c2.x), blend_overlay_f(c1.y, c2.y), blend_overlay_f(c1.z, c2.z)) + (1.0-opacity)*c2;
}
vec3 blend_hard_light(vec2 uv, vec3 c1, vec3 c2, float opacity) {
	return opacity*0.5*(c1*c2+blend_overlay(uv, c1, c2, 1.0)) + (1.0-opacity)*c2;
}
float blend_soft_light_f(float c1, float c2) {
	return (c2 < 0.5) ? (2.0*c1*c2+c1*c1*(1.0-2.0*c2)) : 2.0*c1*(1.0-c2)+sqrt(c1)*(2.0*c2-1.0);
}
vec3 blend_soft_light(vec2 uv, vec3 c1, vec3 c2, float opacity) {
	return opacity*vec3(blend_soft_light_f(c1.x, c2.x), blend_soft_light_f(c1.y, c2.y), blend_soft_light_f(c1.z, c2.z)) + (1.0-opacity)*c2;
}
float blend_burn_f(float c1, float c2) {
	return (c1==0.0)?c1:max((1.0-((1.0-c2)/c1)),0.0);
}
vec3 blend_burn(vec2 uv, vec3 c1, vec3 c2, float opacity) {
	return opacity*vec3(blend_burn_f(c1.x, c2.x), blend_burn_f(c1.y, c2.y), blend_burn_f(c1.z, c2.z)) + (1.0-opacity)*c2;
}
float blend_dodge_f(float c1, float c2) {
	return (c1==1.0)?c1:min(c2/(1.0-c1),1.0);
}
vec3 blend_dodge(vec2 uv, vec3 c1, vec3 c2, float opacity) {
	return opacity*vec3(blend_dodge_f(c1.x, c2.x), blend_dodge_f(c1.y, c2.y), blend_dodge_f(c1.z, c2.z)) + (1.0-opacity)*c2;
}
vec3 blend_lighten(vec2 uv, vec3 c1, vec3 c2, float opacity) {
	return opacity*max(c1, c2) + (1.0-opacity)*c2;
}
vec3 blend_darken(vec2 uv, vec3 c1, vec3 c2, float opacity) {
	return opacity*min(c1, c2) + (1.0-opacity)*c2;
}
vec3 blend_difference(vec2 uv, vec3 c1, vec3 c2, float opacity) {
	return opacity*clamp(c2-c1, vec3(0.0), vec3(1.0)) + (1.0-opacity)*c2;
}
vec3 blend_additive(vec2 uv, vec3 c1, vec3 c2, float oppacity) {
	return c2 + c1 * oppacity;
}
vec3 blend_addsub(vec2 uv, vec3 c1, vec3 c2, float oppacity) {
	return c2 + (c1 - .5) * 2.0 * oppacity;
}
float blend_linear_light_f(float c1, float c2) {
	return (c1 + 2.0 * c2) - 1.0;
}
vec3 blend_linear_light(vec2 uv, vec3 c1, vec3 c2, float opacity) {
return opacity*vec3(blend_linear_light_f(c1.x, c2.x), blend_linear_light_f(c1.y, c2.y), blend_linear_light_f(c1.z, c2.z)) + (1.0-opacity)*c2;
}
float blend_vivid_light_f(float c1, float c2) {
	return (c1 < 0.5) ? 1.0 - (1.0 - c2) / (2.0 * c1) : c2 / (2.0 * (1.0 - c1));
}
vec3 blend_vivid_light(vec2 uv, vec3 c1, vec3 c2, float opacity) {
	return opacity*vec3(blend_vivid_light_f(c1.x, c2.x), blend_vivid_light_f(c1.y, c2.y), blend_vivid_light_f(c1.z, c2.z)) + (1.0-opacity)*c2;
}
float blend_pin_light_f( float c1, float c2) {
	return (2.0 * c1 - 1.0 > c2) ? 2.0 * c1 - 1.0 : ((c1 < 0.5 * c2) ? 2.0 * c1 : c2);
}
vec3 blend_pin_light(vec2 uv, vec3 c1, vec3 c2, float opacity) {
	return opacity*vec3(blend_pin_light_f(c1.x, c2.x), blend_pin_light_f(c1.y, c2.y), blend_pin_light_f(c1.z, c2.z)) + (1.0-opacity)*c2;
}
float blend_hard_mix_f(float c1, float c2) {
	return floor(c1 + c2);
}
vec3 blend_hard_mix(vec2 uv, vec3 c1, vec3 c2, float opacity) {
		return opacity*vec3(blend_hard_mix_f(c1.x, c2.x), blend_hard_mix_f(c1.y, c2.y), blend_hard_mix_f(c1.z, c2.z)) + (1.0-opacity)*c2;
}
float blend_exclusion_f(float c1, float c2) {
	return c1 + c2 - 2.0 * c1 * c2;
}
vec3 blend_exclusion(vec2 uv, vec3 c1, vec3 c2, float opacity) {
	return opacity*vec3(blend_exclusion_f(c1.x, c2.x), blend_exclusion_f(c1.y, c2.y), blend_exclusion_f(c1.z, c2.z)) + (1.0-opacity)*c2;
}
const float p_o88535_amount1 = 0.250000000;
const float seed_o88530 = 0.154530495;
const float p_o88530_v1 = 0.000000000;
const float p_o88530_v2 = 0.100000000;
const float p_o88530_v3 = 0.200000000;
const float p_o88530_v4 = 0.300000000;
const float p_o88530_v5 = 0.400000000;
const float p_o88528_translate_x = 0.001000000;
const float p_o88528_translate_y = 0.050000001;
const float p_o88528_rotate = 0.000000000;
const float p_o88528_scale_x = 1.000000000;
const float p_o88528_scale_y = 1.000000000;
const float seed_o88527 = 0.091868803;
const float p_o88527_v1 = 0.000000000;
const float p_o88527_v2 = 0.100000000;
const float p_o88527_v3 = 0.200000000;
const float p_o88527_v4 = 0.300000000;
const float p_o88527_v5 = 0.400000000;
const float p_o88534_default_in1 = 0.000000000;
const float p_o88534_default_in2 = 0.000000000;
const float p_o88525_translate_x = 1.000000000;
const float p_o88525_translate_y = 1.000000000;
const float p_o88525_rotate = 0.000000000;
const float p_o88525_scale_x = 32.000000000;
const float p_o88525_scale_y = 1.000000000;
const float seed_o88529 = 0.000000000;
const float p_o88532_steps = 15.000000000;
const float seed_o88536 = 0.000000000;
float o88536_f(vec2 uv, float _seed_variation_) {
	return white_noise(uv, 1.0/16.000000000, (seed_o88536+fract(_seed_variation_)));
}const float p_o88533_in_min = 0.000000000;
const float p_o88533_in_max = 1.000000000;
const float p_o88533_out_min = -4.000000000;
const float p_o88533_out_max = 8.000000000;
float o88527_input_in(vec2 uv, float _seed_variation_) {
float o88526_0_1_f = (elapsed_time);
vec4 o88532_0_1_rgba = vec4(floor(vec4(vec3(o88526_0_1_f), 1.0).rgb*p_o88532_steps)/p_o88532_steps, vec4(vec3(o88526_0_1_f), 1.0).a);
vec3 o88529_0_rand = ((o88532_0_1_rgba).rgb);vec3 o88529_0_1_rgb = rand3( vec2( float((seed_o88529+fract(_seed_variation_))) + o88529_0_rand.x, rand( o88529_0_rand.yz ) ) );
float o88536_0_1_f = o88536_f((transform2((uv), vec2(p_o88525_translate_x*(2.0*1.0-1.0), p_o88525_translate_y*(2.0*(dot(o88529_0_1_rgb, vec3(1.0))/3.0)-1.0)), p_o88525_rotate*0.01745329251*(2.0*1.0-1.0), vec2(p_o88525_scale_x*(2.0*1.0-1.0), p_o88525_scale_y*(2.0*1.0-1.0)))), _seed_variation_);
vec4 o88525_0_1_rgba = vec4(vec3(o88536_0_1_f), 1.0);
vec4 o88533_0_1_rgba = vec4(vec3(p_o88533_out_min)+(vec4(o88529_0_1_rgb, 1.0).rgb-vec3(p_o88533_in_min))*vec3((p_o88533_out_max-(p_o88533_out_min))/(p_o88533_in_max-(p_o88533_in_min))), vec4(o88529_0_1_rgb, 1.0).a);
float o88534_0_clamp_false = (dot((o88525_0_1_rgba).rgb, vec3(1.0))/3.0)*(dot((o88533_0_1_rgba).rgb, vec3(1.0))/3.0);
float o88534_0_clamp_true = clamp(o88534_0_clamp_false, 0.0, 1.0);
float o88534_0_1_f = o88534_0_clamp_false;
return o88534_0_1_f;
}
float o88530_input_in(vec2 uv, float _seed_variation_) {
float o88527_0_1_f = o88527_input_in((uv), (seed_o88527+fract(_seed_variation_))+p_o88527_v1);
float o88527_1_2_f = o88527_input_in((uv), (seed_o88527+fract(_seed_variation_))+p_o88527_v2);
vec4 o88523_0 = textureLod(texture_1, transform2_clamp(transform2((uv), vec2(p_o88528_translate_x*(2.0*o88527_0_1_f-1.0), p_o88528_translate_y*(2.0*o88527_1_2_f-1.0)), p_o88528_rotate*0.01745329251*(2.0*1.0-1.0), vec2(p_o88528_scale_x*(2.0*1.0-1.0), p_o88528_scale_y*(2.0*1.0-1.0)))), 0.0);
vec4 o88528_0_1_rgba = o88523_0;
return (dot((o88528_0_1_rgba).rgb, vec3(1.0))/3.0);
}
void fragment() {
	float _seed_variation_ = variation;
	vec2 uv = fract(UV);
float o88530_0_1_f = o88530_input_in((uv), (seed_o88530+fract(_seed_variation_))+p_o88530_v1);
float o88530_1_2_f = o88530_input_in((uv), (seed_o88530+fract(_seed_variation_))+p_o88530_v2);
float o88530_2_3_f = o88530_input_in((uv), (seed_o88530+fract(_seed_variation_))+p_o88530_v3);
float o88530_3_4_f = o88530_input_in((uv), (seed_o88530+fract(_seed_variation_))+p_o88530_v4);
vec4 o88531_0_1_rgba = vec4(o88530_0_1_f, o88530_1_2_f, o88530_2_3_f, o88530_3_4_f);
float o88527_2_1_f = o88527_input_in((uv), (seed_o88527+fract(_seed_variation_))+p_o88527_v3);
vec4 o88535_0_b = o88531_0_1_rgba;
vec4 o88535_0_l;
float o88535_0_a;

o88535_0_l = vec4(vec3(o88527_2_1_f), 1.0);
o88535_0_a = p_o88535_amount1*1.0;
o88535_0_b = vec4(blend_difference((uv), o88535_0_l.rgb, o88535_0_b.rgb, o88535_0_a*o88535_0_l.a), min(1.0, o88535_0_b.a+o88535_0_a*o88535_0_l.a));

vec4 o88535_0_2_rgba = o88535_0_b;

	vec4 color_tex = o88535_0_2_rgba;
	color_tex = mix(pow((color_tex + vec4(0.055)) * (1.0 / (1.0 + 0.055)),vec4(2.4)),color_tex * (1.0 / 12.92),lessThan(color_tex,vec4(0.04045)));
	ALBEDO = color_tex.rgb;
	ALPHA = color_tex.a;

}



