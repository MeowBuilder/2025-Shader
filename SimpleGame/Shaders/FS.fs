#version 330

layout(location=0) out vec4 FragColor;
layout(location=1) out vec4 FragColor1;

in vec2 v_UV;

uniform sampler2D u_RGBTexture;

uniform sampler2D u_DigitTexture;
uniform sampler2D u_NUMTexture;

uniform float u_Time;

const float c_PI = 3.141592;

vec4 Test(){
	vec2 newUV = v_UV;
	float dx = sin(v_UV.y * 2 * c_PI * 4 + u_Time) * 0.1;
	float dy = sin(v_UV.x * 2 * c_PI * 4 + u_Time) * 0.2;

	newUV += vec2(dx,dy);

	vec4 sampledClor = texture(u_RGBTexture, newUV);
	return sampledClor;
}

vec4 Circles(){
	vec2 newUV = v_UV;
	vec2 center = vec2(0.5,0.5);
	float d = distance(newUV,center);
	vec4 newColor = vec4(0);

	float value = sin(d * 4 * c_PI * 4 + u_Time * 8);
	newColor = vec4(value);

	return newColor;
}

vec4 Flag()
{
	vec2 newUV = vec2(v_UV.x,1-v_UV.y - 0.5);
	vec4 newColor = vec4(0);

	float width = 0.25 * (1-newUV.x);
	float sinValue = 0.2 * sin(newUV.x * 2 * c_PI - u_Time * 2) * newUV.x * 1.2;
	float value = round(0.5+(sinValue + width - newUV.y));
	float value2 = round(0.5-(sinValue - width - newUV.y));

	newColor = vec4(value) * vec4(value2);


	//if (newUV.y < sinValue + width && newUV.y > sinValue - width) {
	//	newColor = vec4(1);
	//}
	//else{
	//	discard;
	//}

	return newColor;
}

vec4 Q1()
{
	vec2 newUV = vec2(v_UV.x, v_UV.y);
	float x = newUV.x;
	float y = 1 - abs(2 * (newUV.y - 0.5)); // newUV.y - 0.5 => -0.5 ~ 0.5 \ * 2 => -1 ~ 1 \ abs() => 1~0~1 \ 1- => 0~1~0
	vec4 newColor = texture(u_RGBTexture, vec2(x,y));
	return newColor;
}

vec4 Q2(){
	vec2 newUV = vec2(v_UV.x, v_UV.y);
	float x = fract(newUV.x * 3);
	float y = (2 - floor(newUV.x * 3))/3 + (newUV.y / 3); 

	vec4 newColor = texture(u_RGBTexture, vec2(x,y));
	return newColor;
}

vec4 Q3(){
	vec2 newUV = vec2(v_UV.x, v_UV.y);
	float x = fract(newUV.x * 3);
	float y = (floor(newUV.x * 3))/3 + (newUV.y / 3);

	vec4 newColor = texture(u_RGBTexture, vec2(x,y));
	return newColor;
}

vec4 Brick_Horizontal(){
	vec2 newUV = vec2(v_UV.x, v_UV.y);

	float rCount = 2;
	float sAmount = 0.5;

	float x = fract(newUV.x * rCount) 
	        + floor(newUV.y * rCount+1) * sAmount;// (0.5 * (1-round(newUV.y)));
	float y = fract(newUV.y * rCount);

	vec4 newColor = texture(u_RGBTexture, vec2(x,y));
	return newColor;
}

vec4 Brick_Vertical(){
	vec2 newUV = vec2(v_UV.x, v_UV.y);
	float x = fract(newUV.x * 2);
	float y = fract(newUV.y * 2) + floor(newUV.x*2) * 0.5;//(0.5 * (round(newUV.x)));

	vec4 newColor = texture(u_RGBTexture, vec2(x,y));
	return newColor;
}

float random (vec2 P) {
    return fract(sin(dot(P, vec2(12.9898, 78.233))) * 43758.5453);
}

float fractalNoise(vec2 P, float T) {
    // 텍스처 좌표 P에 시간 T를 더해 노이즈를 이동시킵니다.
    vec2 p1 = P * 4.0;
    vec2 p2 = P * 8.0;

    // 여러 주파수(4.0, 8.0)의 난수를 합산하여 프랙탈 느낌을 줍니다.
    float n = random(floor(p1 + T)) * 0.5;
    n += random(floor(p2 - T * 0.5)) * 0.25; 
    
    // 결과를 0.0에서 1.0 범위로 정규화합니다.
    return n * 1.5;
}

vec4 Brick_Horizontal_AI(){
    // 1. 벽돌 패턴 정의 상수
    const float BRICK_WIDTH = 4.0;
    const float BRICK_HEIGHT = 8.0;
    const float MORTAR_SIZE = 0.05;
    const float CRACK_AMOUNT = 0.1; // 노이즈에 의한 변형의 강도 조절
    
    // 2. 반복되는 타일 맵 좌표 계산
    vec2 tileUV = v_UV * vec2(BRICK_WIDTH, BRICK_HEIGHT);
    vec2 brickIndex = floor(tileUV); 
    vec2 brickUV = fract(tileUV);    
    
    // 3. 벽돌 패턴의 오프셋 (엇갈림 효과) 적용 (짝수 행 오프셋 적용)
    float row = brickIndex.y;
    float offset = 0.0;
    if (mod(row, 2.0) < 0.5) { // 짝수 행
        offset = 0.5;
    }
    float offsetUV_x = fract(brickUV.x + offset);
    
    // 4. 줄눈(Mortar) 마스크 생성 (이전과 동일)
    float isBrick = 1.0; 
    if (brickUV.y > (1.0 - MORTAR_SIZE)) {
        isBrick = 0.0;
    }
    if (isBrick > 0.5 && offsetUV_x > (1.0 - MORTAR_SIZE)) {
        isBrick = 0.0;
    }
    
    // 5. 텍스처 샘플링 및 색상 결정
    vec4 finalColor;
    
    if (isBrick > 0.5) {
        // **⭐ 핵심 수정 부분: 노이즈를 이용한 UV 왜곡 적용**
        
        // 5-1. 현재 벽돌의 중심 위치를 기반으로 노이즈를 생성합니다.
        // brickIndex (정수 좌표)를 사용하면 각 벽돌마다 고유한 노이즈 패턴이 적용됩니다.
        vec2 noise_pos = brickIndex + vec2(offsetUV_x, brickUV.y);
        
        // 5-2. 시간에 따라 움직이는 프랙탈 노이즈 값을 얻습니다.
        // U_Time을 사용하여 시간에 따라 노이즈의 패턴이 계속 변화합니다.
        float noise_value_u = fractalNoise(noise_pos * 1.2, u_Time * 0.5);
        float noise_value_v = fractalNoise(noise_pos * 0.8, u_Time * 1.0);

        // 5-3. 텍스처 좌표를 노이즈 값으로 왜곡합니다.
        // noise_value는 0~1 사이이므로, 이를 -0.5 ~ +0.5 범위로 조절하고 강도를 곱합니다.
        vec2 distortion = vec2(
            (noise_value_u - 0.5) * CRACK_AMOUNT,
            (noise_value_v - 0.5) * CRACK_AMOUNT
        );
        
        // 5-4. 벽돌 내부의 좌표에 왜곡을 적용하여 새로운 샘플링 좌표를 만듭니다.
        vec2 brick_sample_UV = vec2(offsetUV_x, brickUV.y) + distortion;

        // 텍스처(u_RGBTexture)에서 왜곡된 좌표로 샘플링
        finalColor = texture(u_RGBTexture, brick_sample_UV); 

    } else {
        // 줄눈(Mortar)
        finalColor = vec4(0.2, 0.2, 0.2, 1.0);
    }
    
    // 6. 최종 색상 출력
    return finalColor;
}

vec4 Digit(){
    return texture(u_DigitTexture,v_UV);
}

vec4 Digit_Num(){
    int selectedNUM = int(u_Time) % 10;

    int tileIndex = (selectedNUM + 9)%10;

    float offX = float(tileIndex % 5) * 0.2;
    float offY = floor(tileIndex*0.2) * 0.5;

	float tx = (v_UV.x * 0.2) + offX;
	float ty = (v_UV.y * 0.5) + offY;

	return texture(u_NUMTexture, vec2(tx,ty));
}

// 격자무늬(Grid) 필터 함수
vec4 applyGridFilter(vec4 color, vec2 local_uv) {
    float grid_size = 15.0; // 격자 크기 (클수록 촘촘)
    vec2 p = fract(local_uv * grid_size);
    
    // 외곽선 두께
    float line_width = 0.05; 
    
    // 격자 선 마스크 (0.0: 선, 1.0: 내부)
    float mask = step(line_width, p.x) * step(line_width, p.y);
    
    // 선 색상 (어둡게)
    vec4 grid_color = vec4(color.rgb * 0.3, color.a);
    
    // 마스크를 이용해 색상을 혼합
    return mix(grid_color, color, mask);
}

// 웨이브(Wave) 필터 함수
vec4 applyWaveFilter(vec4 color, vec2 local_uv) {
    // 시간에 따른 웨이브 애니메이션 추가
    float wave_speed = 5.0; 
    float wave_strength = 0.05; // 웨이브 강도
    
    // Y축 좌표를 사인파로 왜곡 (수평 웨이브)
    float distortion = sin((local_uv.x + u_Time * 0.5) * wave_speed) * wave_strength;
    
    // 왜곡된 Y 좌표를 사용하여 텍스처를 샘플링하는 것처럼 처리
    // 여기서는 간단히 색상에 영향을 줍니다.
    float wave_factor = smoothstep(0.0, 1.0, 0.5 + distortion * 5.0);
    
    // 웨이브 팩터에 따라 색상 톤을 변경 (예: 파란색/녹색 톤 혼합)
    vec3 wave_tint = mix(vec3(1.0, 1.0, 1.0), vec3(0.5, 0.8, 1.0), wave_factor);
    
    return vec4(color.rgb * wave_tint, color.a);
}

vec4 Digit_Five_Nums() {
    // 1. 현재 시간에서 5자리 숫자를 계산 (00000 ~ 99999)
    int currentNumber = int(u_Time * 100.0) % 100000;
    
    // 2. 텍스처 좌표 (v_UV)를 5개의 영역으로 분할
    int digitIndex = int(v_UV.x * 5.0); 
    
    // 3. 자릿수별 값 추출
    int selectedNUM; 
    int divisor = 10000;
    
    if (digitIndex == 1) divisor = 1000;
    else if (digitIndex == 2) divisor = 100;
    else if (digitIndex == 3) divisor = 10;
    else if (digitIndex == 4) divisor = 1;

    selectedNUM = (currentNumber / divisor) % 10;

    // 4. 텍스처 레이아웃 조정 및 오프셋 계산 (생략하지 않음)
    int tileIndex;
    if (selectedNUM == 0) {
        tileIndex = 9;
    } else {
        tileIndex = selectedNUM - 1;
    }
    
    float offX_sheet = float(tileIndex % 5) * 0.2;
    float offY_sheet = floor(float(tileIndex) * 0.2) * 0.5;

    // 5. 시간에 따른 효과 계산
    
    // A. 10 단위 변경 감지 (랜덤 색상 필터링)
    int colorChangeValue = (currentNumber / 10);
    
    // B. 100 단위 변경 감지 (미러링/상하좌우 반전)
    int mirrorToggle = (currentNumber / 100) % 2; 
    
    // C. 1000 단위 변경 감지 (랜덤 필터 선택)
    int filterToggle = (currentNumber / 1000); 

    // 6. 최종 텍스처 좌표 계산 및 미러링 적용
    float local_x = fract(v_UV.x * 5.0);
    float local_y = v_UV.y;
    vec2 local_uv = vec2(local_x, local_y); // 필터링을 위해 local_uv 저장

    // 100 단위 변화 시 상하좌우 반전 적용
    if (mirrorToggle == 1) {
        local_x = 1.0 - local_x;
        local_y = 1.0 - local_y;
    }

    // 최종 샘플링 좌표
    float tx = (local_x * 0.2) + offX_sheet;
    float ty = (local_y * 0.5) + offY_sheet;

    // 7. 색상 샘플링 및 필터 효과 적용
    vec4 finalColor = texture(u_NUMTexture, vec2(tx, ty));

    // ⭐ 1000 단위 변화 시 랜덤 필터 효과 적용 ⭐
    // 필터 선택을 위한 시드: 1000 단위의 몫을 시드로 사용
    float filterSeed = float(filterToggle);
    // 0~1 사이의 랜덤 값으로 필터를 선택 (0.5 기준)
    float filterChoice = random(vec2(filterSeed, 0.0)); 
    
    if (filterChoice > 0.5) {
        // 랜덤 선택 1: 격자무늬 필터
        finalColor = applyGridFilter(finalColor, local_uv);
    } else {
        // 랜덤 선택 2: 웨이브 필터
        finalColor = applyWaveFilter(finalColor, local_uv);
    }
    
    // ⭐ 10 단위 변화 시 랜덤 색상 필터링 적용 ⭐
    // 색상 시드: 10 단위의 몫을 시드로 사용
    float colorSeed = float(colorChangeValue);
    
    // 10 단위가 바뀔 때마다 3개의 독립적인 랜덤 값 (RGB) 생성
    float r = random(vec2(colorSeed, 1.0));
    float g = random(vec2(colorSeed, 2.0));
    float b = random(vec2(colorSeed, 3.0));
    
    // 랜덤 색상 틴트 생성 (0.5 ~ 1.0 사이의 랜덤 값)
    vec3 random_tint = vec3(r, g, b) * 0.5 + 0.5;
    
    // 최종 색상에 랜덤 틴트 곱하기 (필터링 효과)
    finalColor.rgb *= random_tint;

    // 8. 최종 출력
    return finalColor;
}

void main()
{
	//Test();
	//Circles();
	//Flag();
	//Q1();
	//Brick_Horizontal();
	//Brick_Vertical();
	//Brick_Horizontal_AI();
    //Digit();
    //Digit_Num();
    //Digit_Five_Nums();

    FragColor = Circles();
    FragColor1 = Flag();
}
