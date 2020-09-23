#ifdef GL_ES
precision mediump float;
#endif

#define LIMIT 300.0
#define MAGNIFICATION 100.0
#define OFFSET vec2(-0.11, 0.89895)
#define ZOOM_V 10.0

uniform vec2 u_resolution;
uniform float u_time;

vec2 mul_complex(in vec2 a, in vec2 b) {
	return vec2(a.x * b.x - a.y * b.y, a.y * b.x + a.x * b.y);
}

vec3 hsv_to_rgb(in vec3 hsv) {
    return ((clamp(abs(fract(hsv.x + vec3(0, 2, 1) / 3.0) * 6.0 - 3.0) - 1.0, 0.0, 1.0) - 1.0) * hsv.y + 1.0) * hsv.z;
}

void main(){
    float mag = 1.0 / MAGNIFICATION;

#ifdef ZOOM_V
    mag *= ((1.0 / ZOOM_V) / u_time);
#endif

    vec2 p = (gl_FragCoord.xy * 2. - u_resolution.xy) / min(u_resolution.x, u_resolution.y);
    vec2 c = (mag * p) + OFFSET;

    vec2 z = vec2(0.0, 0.0);
    float conv = LIMIT;
    for (float n = 0.0; n < LIMIT; n++) {
        z = mul_complex(z, z) + c;
        if (dot(z, z) > 4.0) {
            conv = n;
            break;
        }
    }

    vec3 rgb = hsv_to_rgb(vec3(abs(sin(u_time * 0.01)), conv * 0.01, conv * 0.015 * sin(u_time)));
    float alpha = 1.0 - step(LIMIT, conv);
    gl_FragColor = vec4(rgb, alpha);
}
