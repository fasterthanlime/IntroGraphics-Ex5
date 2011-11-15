uniform sampler2D texture; // the depth texture that need to be post processed with the prewitt operator

uniform float dx; // use this uniform to move 1 pixel in x
uniform float dy; // use this uniform to move 1 pixel in y
		
float I(int i, int j)
{
	// x = y = z in our linear depth texture anyway :)
	return texture2D( texture, vec2( float(i) * dx, float(j) * dy ) ).x;
}
		
float C(mat3 K, int i, int j)
{
	float sum = 0.0;
	for ( int m = 0; m < 2; m++ ) {
		for ( int n = 0; n < 2; n++ ) {
			sum += K[m][n] * I( i + m - 1, j + n - 1 );
		}
	}
	return sum;
}
		
void main()
{	
	//
	// calculate prewitt edges
	//                  -1  0  1
	// x derivative:    -1  0  1
	//                  -1  0  1
	// 
	//                   1  1  1
	// y derivative:     0  0  0
	//                  -1 -1 -1
	//
	
	mat3 Gx = mat3(
		-1.0, -1.0, -1.0,
		 0.0,  0.0,  0.0,
		 1.0,  1.0,  1.0
	);
	
	mat3 Gy = mat3(
		 1.0,  0.0, -1.0,
		 1.0,  0.0, -1.0,
		 1.0,  0.0, -1.0
	);
	
	int i = int(gl_FragCoord.x);
	int j = int(gl_FragCoord.y);
	
	float gx = C(Gx, i, j);
	float gy = C(Gy, i, j);
	float gnorm = 1.0 - 10.0 * sqrt( ( gx * gx ) + ( gy * gy ) );
	
	gl_FragColor = vec4( gx, gy, sqrt( ( gx * gx ) + ( gy * gy ) ), 1.0 );
}