float4x4 World;
float4x4 View;
float4x4 Projection;

float3 LightDirection;
float Ambient;

Texture Texture0;
sampler TextureSampler0 = sampler_state { 
			texture   = <Texture0> ; 
			magfilter = LINEAR; 
			minfilter = LINEAR; 
			mipfilter = LINEAR; 
			AddressU  = mirror; 
			AddressV  = mirror;
		};

Texture Texture1;
sampler TextureSampler1 = sampler_state { 
			texture   = <Texture1> ; 
			magfilter = LINEAR; 
			minfilter = LINEAR; 
			mipfilter = LINEAR; 
			AddressU  = wrap; 
			AddressV  = wrap;
		};

Texture Texture2;
sampler TextureSampler2 = sampler_state { 
			texture   = <Texture2> ; 
			magfilter = LINEAR; 
			minfilter = LINEAR; 
			mipfilter = LINEAR; 
			AddressU  = mirror; 
			AddressV  = mirror;
		};

Texture Texture3;
sampler TextureSampler3 = sampler_state { 
			texture   = <Texture3> ; 
			magfilter = LINEAR; 
			minfilter = LINEAR; 
			mipfilter = LINEAR; 
			AddressU  = wrap; 
			AddressV  = wrap;
		};

struct VertexShaderInput
{
    float4 Position : POSITION0;
	float3 Normal : TEXCOORD0;
	float2 TexCoords : TEXCOORD1;
	float4 TexWeights : TEXCOORD2;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float3 Normal : TEXCOORD0;
	float2 TexCoords : TEXCOORD1;
	float4 TexWeights : TEXCOORD2;
    float4 Light : TEXCOORD3;
};


VertexShaderOutput MultitexturedVertexShader(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
	
	output.Normal = input.Normal;
	output.TexCoords = input.TexCoords;
	output.TexWeights = input.TexWeights;

    output.Light.xyz = -LightDirection;
    output.Light.w = 1;   

    return output;
}

float4 MultitexturedPixelShader(VertexShaderOutput input) : COLOR0
{
	float4 output;

	float lighting = saturate(saturate(dot(input.Normal, input.Light)) + Ambient);

    output  = tex2D(TextureSampler0, input.TexCoords)*input.TexWeights.x;
    output += tex2D(TextureSampler1, input.TexCoords)*input.TexWeights.y;
    output += tex2D(TextureSampler2, input.TexCoords)*input.TexWeights.z;
    output += tex2D(TextureSampler3, input.TexCoords)*input.TexWeights.w;  
	
    output.rgb *= lighting;

    return output;
}

technique Multitextured
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 MultitexturedVertexShader();
        PixelShader = compile ps_2_0 MultitexturedPixelShader();
    }
}
