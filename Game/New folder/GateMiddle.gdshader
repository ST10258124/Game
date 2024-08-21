shader_type spatial;
render_mode blend_mix;

void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	// Called for every pixel the material is visible on.
    // Oscillate color between red and blue over time
    float t = sin(TIME * 16.0f) * 0.5 + 0.5;
    vec3 pink = vec3(2.0, 0.0, 2.0);
    vec3 aqua = vec3(0.0, 2.0, 2.0);
    vec3 color = mix(pink, aqua, t);

    // Set constant transparency (alpha)
    ALPHA = 0.1;

    // Assign color and alpha to ALBEDO
    ALBEDO = color;
}

//void light() {
	// Called for every pixel for every light affecting the material.
	// Uncomment to replace the default light processing function with this one.
//}
