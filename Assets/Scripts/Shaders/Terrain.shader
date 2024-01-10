Shader "Universal Render Pipeline/Terrain"
{
    Properties
    {
        float _minHeight;
        float _maxHeight;
    }
    SubShader
    {
        

            Tags { "RenderType"="Opaque" }
            LOD 200

            CGPROGRAM
            // Physically based Standard lighting model, and enable shadows on all light types
            #pragma surface surf Standard fullforwardshadows

            // Use shader model 3.0 target, to get nicer looking lighting
            #pragma target 3.0

           

            struct Input
            {
               float3 worldPos;
            };

            float inverseLerp(float a, float b, float value)
            {
                //saturate means to clamp it in the range of 0 to 1
                return saturate((value - a) / (b - a));
            }

            // Add instancing support for this shader. 
            // You need to check 'Enable Instancing' on materials 
            // that use the shader.
            // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
            // #pragma instancing_options assumeuniformscaling
            UNITY_INSTANCING_BUFFER_START(Props)
                // put more per-instance properties here
            UNITY_INSTANCING_BUFFER_END(Props)

            void surf (Input IN, inout SurfaceOutputStandard o)
            {
            
                float heightPercent = inverseLerp(_minHeight, _maxHeight, IN.worldPos.y);
                o.Albedo = heightPercent;
            }
            ENDCG
        
    }
    FallBack "Diffuse"
}
