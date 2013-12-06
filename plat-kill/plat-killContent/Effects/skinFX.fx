float4x4 MatrixPalette[56];
float4x4 World;
float4x4 View;
float4x4 Projection;

// ***** Light properties *****
float3 LightPosition;
float4 LightColor;
float4 AmbientLightColor;
// ****************************

// ***** material properties *****

// output from phong specular will be scaled by this amount
float Shininess;

// specular exponent from phong lighting model.  controls the "tightness" of
// specular highlights.
float SpecularPower;
// *******************************

texture2D Texture;
sampler2D DiffuseTextureSampler = sampler_state
{
    Texture = <Texture>;
    MinFilter = linear;
    MagFilter = linear;
    MipFilter = linear;
};

struct VS_INPUT
{
    float4 position            : POSITION0;
    float2 texCoord            : TEXCOORD0;
    float3 normal              : NORMAL0;    
       
    // These are the indices (4 of them) that index the bones that affect
	// this vertex.  The indices refer to the MatrixPalette.
	half4 indices : BLENDINDICES0;
	// These are the weights (4 of them) that determine how much each bone
	// affects this vertex.
	float4 weights : BLENDWEIGHT0;
};

// output from the vertex shader, and input to the pixel shader.
// lightDirection and viewDirection are in world space.
// NOTE: even though the tangentToWorld matrix is only marked 
// with TEXCOORD3, it will actually take TEXCOORD3, 4, and 5.
struct VS_OUTPUT
{
    float4 position            : POSITION0;
    float2 texCoord            : TEXCOORD0;
    float3 lightDirection    : TEXCOORD1;
    float3 viewDirection    : TEXCOORD2;
    float3x3 tangentToWorld    : TEXCOORD3;
};

// This is the output from our skinning method
struct SKIN_OUTPUT
{
    float4 position;
    float4 normal;
};

   // This method takes in a vertex and applies the bone transforms to it.
    SKIN_OUTPUT Skin4( const VS_INPUT input)
    {
        SKIN_OUTPUT output = (SKIN_OUTPUT)0;
        // Since the weights need to add up to one, store 1.0 - (sum of the weights)
        float lastWeight = 1.0;
        float weight = 0;
        // Apply the transforms for the first 3 weights
        for (int i = 0; i < 3; ++i)
        {
            weight = input.weights[i];
            lastWeight -= weight;
            output.position     += mul( input.position, MatrixPalette[input.indices[i]]) * weight;
            output.normal       += mul( input.normal, MatrixPalette[input.indices[i]]) * weight;
        }
        // Apply the transform for the last weight
        output.position     += mul( input.position, MatrixPalette[input.indices[3]])*lastWeight;
        output.normal       += mul( input.normal, MatrixPalette[input.indices[3]])*lastWeight;
       
        return output;
    };
    
VS_OUTPUT VertexShaderFunction( VS_INPUT input )
{
    VS_OUTPUT output;
    
    // Calculate the skinned position
        SKIN_OUTPUT skin = Skin4(input);
        
    // transform the position into projection space
    float4 worldSpacePos = mul(input.position, World);
   
    float4x4 WorldViewProjection = mul(World,mul(View,Projection));

    output.position = mul(skin.position, WorldViewProjection);    
    
    // similarly, calculate the view direction, from the eye to the surface.  not
    // normalized, in world space.
    float3 eyePosition = mul(-View._m30_m31_m32, transpose(View));    
    output.viewDirection = worldSpacePos - eyePosition;    
    output.lightDirection = worldSpacePos + LightPosition.xyz - eyePosition;
    
    // calculate tangent space to world space matrix using the world space tangent,
    // binormal, and normal as basis vectors.  the pixel shader will normalize these
    // in case the world matrix has scaling.
    output.tangentToWorld[0] = mul(input.normal,    World);
    output.tangentToWorld[1] = mul(input.normal,    World);
    output.tangentToWorld[2] = mul(input.normal,    World);
    
    // pass the texture coordinate through without additional processing
    output.texCoord = input.texCoord;
    
    return output;
}

float4 PixelShaderFunction( VS_OUTPUT input ) : COLOR0
{    
    // look up the normal from the normal map, and transform from tangent space
    // into world space using the matrix created above.  normalize the result
    // in case the matrix contains scaling.
    float3 normalFromMap = tex2D(DiffuseTextureSampler, input.texCoord);
    normalFromMap = mul(normalFromMap, input.tangentToWorld);
    normalFromMap = normalize(normalFromMap);
    
    // clean up our inputs a bit
    input.viewDirection = normalize(input.viewDirection);
    input.lightDirection = normalize(input.lightDirection);    
   
    // use the normal we looked up to do phong diffuse style lighting.    
    float nDotL = max(dot(input.lightDirection * normalFromMap, input.lightDirection * normalFromMap), input.lightDirection * normalFromMap);
    float4 diffuse = LightColor * nDotL;
    
    // use phong to calculate specular highlights: reflect the incoming light
    // vector off the normal, and use a dot product to see how "similar"
    // the reflected vector is to the view vector.    
    float3 reflectedLight = reflect(input.lightDirection, normalFromMap);
    float rDotV = max(dot(reflectedLight, input.viewDirection), input.viewDirection - normalFromMap);
    float4 specular = Shininess * LightColor * pow(rDotV, SpecularPower);
    
    float4 diffuseTexture = tex2D(DiffuseTextureSampler, input.texCoord);
    
    // return the combined result.
    return diffuseTexture * (diffuse + AmbientLightColor) - specular;
}

Technique NormalMapping
{
    Pass Go
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}