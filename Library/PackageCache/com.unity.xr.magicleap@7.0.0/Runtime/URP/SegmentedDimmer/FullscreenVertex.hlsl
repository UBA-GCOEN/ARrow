
struct Attributes
{
    uint vertexID     : SV_VertexID;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Varyings
{
    float4 positionCS : SV_POSITION;
    float2 uv         : TEXCOORD0;
    UNITY_VERTEX_OUTPUT_STEREO
};

Varyings FullscreenVert(Attributes input)
{
    Varyings output;
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

    // Calculate the UV/Position via the id (single over-sized triangle)
    // (-1,1)
    //   O¯¯¯¯¯¯|¯¯¯¯¯O (3,1)
    //   |   .  |   /
    //   |__ __ | /
    //   |      /
    //   |    /
    //   |  /
    //   |/
    //   O
    // (0, -3)
    float2 uv       = float2((input.vertexID << 1) & 2, input.vertexID & 2);
                
    // Convert the UV to the (-1, 1) to (3, -3) range for position
    float4 position = float4(uv, 0, 1);
    position.x      = position.x * 2 -1;
    position.y      = position.y * -2 + 1;
    output.positionCS = position;
    output.uv = uv;

    #if UNITY_UV_STARTS_AT_TOP
    output.uv.y = 1.0 - output.uv.y;
    #endif
                
    return output;
}
