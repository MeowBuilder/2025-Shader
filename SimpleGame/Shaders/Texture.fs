#version 330

layout(location=0) out vec4 FragColor;

in vec2 v_Tex;

uniform sampler2D u_TexID;
uniform float u_Time;


// 가상의 질량체(렌즈)의 중심점 (Normalized Device Coordinates, [0, 1])
const vec2 LENS_CENTER = vec2(0.5, 0.5); 
// 렌즈의 강도 또는 반경에 비례하는 상수. 값이 클수록 왜곡이 강해집니다.
const float LENS_STRENGTH = 0.05; 
// 텍스처 좌표계 변환을 위한 상수 (v_Tex는 보통 [0, 1] 범위입니다)
const vec2 SCREEN_CENTER = vec2(0.5, 0.5); 

vec4 Lens()
{
    // 1. 현재 픽셀의 화면 중심으로부터의 벡터를 계산 (화면 중심을 (0,0)으로)
    // v_Tex는 [0, 1] 범위, delta는 [-0.5, 0.5] 범위
    vec2 delta = v_Tex - LENS_CENTER; 
    
    // 2. 렌즈 중심으로부터의 거리 계산 (r)
    float r = length(delta);
    
    // 3. 중력 렌즈 왜곡량 계산
    // 렌즈 효과는 거리가 가까울수록 강하고, 멀수록 약해집니다.
    // 간단한 근사치로 (1.0 / r^2)에 비례하는 왜곡을 사용합니다.
    // r이 0에 가까워지는 것을 방지하기 위해 작은 상수(epsilon)를 더합니다.
    float epsilon = 0.001; 
    float distortion = LENS_STRENGTH / (r * r + epsilon);
    
    // 4. 왜곡된 텍스처 좌표 계산
    // 원본 텍스처 좌표(v_Tex)에, 중심을 향하는 방향(delta)으로 왜곡을 적용하여 
    // 새로운 텍스처 좌표를 얻습니다.
    // -delta를 사용하는 이유는, 빛이 렌즈를 통과하면서 *바깥쪽으로 휘는* 것처럼 
    // 보이기 위해, 텍스처를 *중심을 향해 당겨와야* 하기 때문입니다 (역방향 매핑).
    vec2 distorted_uv = v_Tex - delta * distortion;
    
    // 5. 왜곡된 좌표로 텍스처 샘플링 (OpenGL의 텍스처 Y축은 보통 반전되어 있으므로 1.0-y 적용)
    // 다만, v_Tex 자체가 이미 Y축이 뒤집혀 들어올 수도 있으므로, 기존 코드 주석을 참고하여 
    // v_Tex.y만 사용하거나 1.0 - v_Tex.y를 사용해야 합니다.
    // 예시에서는 원본 프래그먼트 쉐이더의 주석 처리된 코드를 따르지 않고, v_Tex.y를 그대로 사용합니다.
    // FragColor = texture(u_TexID,vec2(v_Tex.x,1-v_Tex.y)); <--- 이 주석을 기반으로
    // 여기서는 렌더링 파이프라인에서 이미 Y축을 뒤집는다고 가정하고, distorted_uv.y를 그대로 사용합니다.
    // 만약 Y축 반전이 필요하다면: vec2(distorted_uv.x, 1.0 - distorted_uv.y);
    
    // Y축 반전을 적용하여 샘플링하는 예시:
    // return texture(u_TexID, vec2(distorted_uv.x, 1.0 - distorted_uv.y));
    
    // Y축 반전 없이 샘플링하는 예시 (더 흔한 경우):
    return texture(u_TexID, distorted_uv); 
}

// 만화경 효과의 중심 (화면 중앙)
const vec2 KALEIDOSCOPE_CENTER = vec2(0.5, 0.5);
// 만화경 섹터의 수 (짝수여야 함, 6, 8, 10, 12 등)
// 섹터 수가 많을수록 더 복잡한 패턴이 됩니다.
const int KALEIDOSCOPE_SECTORS = 12; 

vec4 Lens2(){
    // 1. 텍스처 좌표를 만화경 중심으로 이동 (-0.5 ~ 0.5 범위)
    vec2 uv = v_Tex - KALEIDOSCOPE_CENTER;

    // 2. 극좌표로 변환 (거리 r, 각도 theta)
    float r = length(uv);
    float angle = atan(uv.y, uv.x); // atan2와 유사 (범위 -PI ~ PI)

    // 3. 각도를 만화경 섹터에 맞게 정규화하고 반사 (핵심 부분)
    // 각도를 0 ~ (2*PI / KALEIDOSCOPE_SECTORS) 범위로 압축
    float sector_angle = (2.0 * 3.14159265359) / float(KALEIDOSCOPE_SECTORS); // 각 섹터의 각도 크기
    
    // 현재 각도를 섹터 각도 단위로 정규화
    float normalized_angle = mod(angle, sector_angle);

    // 반사 효과를 위해 각도를 조절
    // 예: 0 ~ sector_angle/2 는 그대로, sector_angle/2 ~ sector_angle 은 반전
    // 이 과정은 sin(normalized_angle / sector_angle * PI) 등을 사용하여 부드러운 반사도 가능하지만,
    // 여기서는 간단하게 삼각 섹터를 접는 방식으로 구현합니다.
    if (mod(floor(angle / sector_angle), 2.0) == 1.0) {
        // 홀수 섹터일 경우 반전
        normalized_angle = sector_angle - normalized_angle;
    }
    
    // 4. 새로운 극좌표 (r, normalized_angle)를 다시 직교 좌표로 변환
    vec2 new_uv;
    new_uv.x = r * cos(normalized_angle);
    new_uv.y = r * sin(normalized_angle);

    // 5. 다시 원래 화면 중앙으로 이동 (0.0 ~ 1.0 범위)
    new_uv += KALEIDOSCOPE_CENTER;

    // 6. 텍스처 샘플링 (OpenGL의 텍스처 Y축은 보통 반전되어 있으므로 1.0-y 적용)
    // 이전 주석과 마찬가지로, Y축 반전은 렌더링 파이프라인에 따라 다를 수 있습니다.
    // 여기서는 일반적으로 많이 사용되는 1.0 - new_uv.y를 적용합니다.
    return texture(u_TexID, vec2(new_uv.x, 1.0 - new_uv.y));
}

vec4 Kaleidoscope_lens()
{
    // 1. 텍스처 좌표를 만화경 중심으로 이동 (-0.5 ~ 0.5 범위)
    vec2 uv = v_Tex - KALEIDOSCOPE_CENTER;

    // 2. 극좌표로 변환 (거리 r, 각도 theta)
    float r = length(uv);
    float angle = atan(uv.y, uv.x); // -PI ~ PI 범위

    // 3. 각도를 만화경 섹터에 맞게 정규화하고 반사 (핵심 부분)
    float PI = 3.14159265359;
    float sector_angle = (2.0 * PI) / float(KALEIDOSCOPE_SECTORS);
    
    // 현재 각도를 섹터 각도 단위로 정규화
    float normalized_angle = mod(angle, sector_angle);

    // 홀수 섹터일 경우 반전 (대칭 거울 효과)
    if (mod(floor(angle / sector_angle), 2.0) == 1.0) {
        normalized_angle = sector_angle - normalized_angle;
    }
    
    // 4. 새로운 극좌표를 다시 직교 좌표로 변환
    vec2 new_uv;
    new_uv.x = r * cos(normalized_angle);
    new_uv.y = r * sin(normalized_angle);

    // 5. 다시 원래 화면 중앙으로 이동 (0.0 ~ 1.0 범위)
    new_uv += KALEIDOSCOPE_CENTER;

    // 6. 왜곡된 좌표로 텍스처 샘플링 (Y축 반전)
    // 이 함수는 체인의 첫 번째 필터이므로, 여기서 u_TexID를 직접 샘플링합니다.
    return texture(u_TexID, vec2(new_uv.x, 1.0 - new_uv.y));
}

// 색수차 효과의 중심 (화면 중앙)
const vec2 CHROMATIC_CENTER = vec2(0.5, 0.5); 
// 색수차 강도 (클수록 색상 분리가 심해집니다)
const float CHROMATIC_STRENGTH = 0.08; 

vec4 Lens4()
{
    // 1. 현재 픽셀의 중심으로부터의 방향 벡터 계산
    // v_Tex는 (0,0) ~ (1,1) 범위이므로, 중심 (0.5, 0.5)을 기준으로 이동
    vec2 dir_to_center = CHROMATIC_CENTER - v_Tex; 
    
    // 2. 중심으로부터의 거리 계산 (정규화된 거리)
    // 거리가 멀수록 색수차 효과를 강하게 적용하기 위함
    float dist = length(dir_to_center);

    // 3. 각 색상 채널별 샘플링 좌표 계산
    // 중심으로부터의 방향(dir_to_center)으로 각 채널을 밀어냅니다.
    // 각 채널은 서로 다른 강도로 밀어냅니다.
    vec2 uv_red   = v_Tex + dir_to_center * dist * CHROMATIC_STRENGTH * 1.5; // 빨강은 가장 바깥쪽으로
    vec2 uv_green = v_Tex + dir_to_center * dist * CHROMATIC_STRENGTH * 0.5; // 초록은 중간
    vec2 uv_blue  = v_Tex + dir_to_center * dist * CHROMATIC_STRENGTH * -0.5; // 파랑은 가장 안쪽으로 (음의 방향)

    // 4. 각 채널에서 색상 샘플링 (Y축 반전)
    // OpenGL 텍스처 좌표계에 맞춰 Y축을 반전시킵니다.
    vec4 final_color;
    final_color.r = texture(u_TexID, vec2(uv_red.x, 1.0 - uv_red.y)).r;
    final_color.g = texture(u_TexID, vec2(uv_green.x, 1.0 - uv_green.y)).g;
    final_color.b = texture(u_TexID, vec2(uv_blue.x, 1.0 - uv_blue.y)).b;
    final_color.a = texture(u_TexID, vec2(v_Tex.x, 1.0 - v_Tex.y)).a; // 알파는 원본 UV에서 샘플링

    return final_color;
}

// 필터 강도 조절 상수들 (레트로 효과)
const float SEPIA_INTENSITY = 0.8;    // 세피아 톤의 강도
const float DESATURATION_FACTOR = 0.2; // 채도 감소 정도
const float CONTRAST_ADJUST = 1.05;   // 대비 조절
const float BRIGHTNESS_ADJUST = 0.05; // 밝기 조절

// CRT 스캔라인 효과 상수들
const float SCREEN_HEIGHT = 1000.0;     // 현재 렌더링 영역의 가상 높이 (물리적 픽셀 높이에 근접한 값 사용)
const float SCANLINE_STRENGTH = 0.3;    // 스캔라인의 어두움 정도 (0.0 ~ 1.0)

vec4 Lens5()
{
    // 1. 원본 텍스처 색상 샘플링 (Y축 반전)
    vec4 color = texture(u_TexID, vec2(v_Tex.x, 1.0 - v_Tex.y));
    vec3 mixed_color = color.rgb;
    
    // --- 레트로 필터 효과 (이전과 동일) ---
    
    // 2. 밝기 (Luminance) 계산
    float luminance = dot(mixed_color, vec3(0.299, 0.587, 0.114));

    // 3. 세피아 톤 및 채도 감소 적용
    vec3 sepia_color = vec3(
        luminance * 1.07,
        luminance * 0.90,
        luminance * 0.70
    );
    mixed_color = mix(mixed_color, sepia_color, SEPIA_INTENSITY);
    mixed_color = mix(mixed_color, vec3(luminance), DESATURATION_FACTOR);

    // 4. 대비 및 밝기 조절
    mixed_color = (mixed_color - 0.5) * CONTRAST_ADJUST + 0.5;
    mixed_color += BRIGHTNESS_ADJUST;
    
    // 5. 클램프
    mixed_color = clamp(mixed_color, 0.0, 1.0);
    
    // --- CRT 스캔라인 효과 추가 ---
    
    // 6. 스캔라인 마스크 계산
    // 현재 픽셀의 Y 좌표를 화면 높이(SCREEN_HEIGHT) 기준으로 변환하여 정수값으로 만듭니다.
    // v_Tex.y (0.0 ~ 1.0)에 SCREEN_HEIGHT를 곱하면 0 ~ SCREEN_HEIGHT 사이의 픽셀 위치 근사치가 나옵니다.
    // mod(..., 2.0)을 사용하여 픽셀 라인이 홀수인지 짝수인지 구분합니다 (두 라인마다 한 번씩 어둡게).
    float pixel_y = v_Tex.y * SCREEN_HEIGHT; 
    
    // 픽셀 Y 좌표의 정수 부분만 사용하여 줄을 구분합니다.
    float scanline_mask = 1.0;
    if (mod(floor(pixel_y), 2.0) == 0.0) {
        // 짝수 번째 줄(또는 홀수, 설정에 따라)을 SCANLINE_STRENGTH만큼 어둡게 만듭니다.
        scanline_mask = 1.0 - SCANLINE_STRENGTH;
    }
    
    // 7. 최종 색상에 마스크 적용
    mixed_color *= scanline_mask;

    // 8. 최종 색상 반환
    return vec4(mixed_color, color.a);
}

vec4 Color_lens(vec4 input_color) 
{
    // 입력 색상의 RGB 채널만 사용
    vec3 mixed_color = input_color.rgb;
    
    // 1. 밝기 (Luminance) 계산
    float luminance = dot(mixed_color, vec3(0.299, 0.587, 0.114));

    // 2. 세피아 톤 및 채도 감소 적용
    vec3 sepia_color = vec3(
        luminance * 1.07,
        luminance * 0.90,
        luminance * 0.70
    );
    mixed_color = mix(mixed_color, sepia_color, SEPIA_INTENSITY);
    mixed_color = mix(mixed_color, vec3(luminance), DESATURATION_FACTOR);

    // 3. 대비 및 밝기 조절
    mixed_color = (mixed_color - 0.5) * CONTRAST_ADJUST + 0.5;
    mixed_color += BRIGHTNESS_ADJUST;
    
    // 4. 클램프
    mixed_color = clamp(mixed_color, 0.0, 1.0);
    
    // 5. CRT 스캔라인 효과 추가
    float pixel_y = v_Tex.y * SCREEN_HEIGHT; 
    float scanline_mask = 1.0;
    if (mod(floor(pixel_y), 2.0) == 0.0) {
        scanline_mask = 1.0 - SCANLINE_STRENGTH;
    }
    
    mixed_color *= scanline_mask;

    // 6. 최종 색상 반환 (입력 알파 값 사용)
    return vec4(mixed_color, input_color.a);
}

vec4 Pixelization(){
    float resol = (sin(u_Time)+1)*100;

    float tx = floor(v_Tex.x * resol) / resol;
    float ty = floor(v_Tex.y * resol) / resol;

    return texture(u_TexID,vec2(tx,ty));
}

void main()
{
	//FragColor = texture(u_TexID,vec2(v_Tex.x,1-v_Tex.y));

    //FragColor = Pixelization();

	FragColor = Lens4();
    FragColor = Color_lens(FragColor);
}
