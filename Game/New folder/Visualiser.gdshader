shader_type spatial;
render_mode blend_mix;

void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	// Called for every pixel the material is visible on.
    // Oscillate color between red and blue over time
    float t = sin(TIME * 5.0f) * 0.5 + 0.5;
    vec3 blue = vec3(0.0, 1.0, 4.0);
    vec3 light_blue = vec3(0.18, 1.12, 4.0);
    vec3 color = mix(blue, light_blue, t);

    // Set constant transparency (alpha)
    ALPHA = 1.0;

    // Assign color and alpha to ALBEDO
    ALBEDO = color;
}

//void light() {
	// Called for every pixel for every light affecting the material.
	// Uncomment to replace the default light processing function with this one.
//}
