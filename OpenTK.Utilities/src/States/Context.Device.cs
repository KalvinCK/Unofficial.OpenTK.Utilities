using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using NvxGpuMemoryInfo = OpenTK.Graphics.OpenGL.NvxGpuMemoryInfo;

namespace OpenTK.Utilities;

// font: https://github.com/JuanDiegoMontoya/Fwog/blob/main/include/Fwog/Context.h
public static partial class Context
{
    public static class Device
    {
        public static readonly IReadOnlyCollection<string> Extensions;

        public static readonly string Vendor;
        public static readonly string Renderer;
        public static readonly string Version;
        public static readonly string ShadingLanguageVersion;
        public static readonly double MajorMinorVersion;

        static Device()
        {
            Version = GL.GetString(StringName.Version);
            Renderer = GL.GetString(StringName.Renderer);
            Vendor = GL.GetString(StringName.Vendor);
            ShadingLanguageVersion = GL.GetString(StringName.ShadingLanguageVersion);
            MajorMinorVersion = Convert.ToDouble($"{GL.GetInteger(GetPName.MajorVersion)}{GL.GetInteger(GetPName.MinorVersion)}") / 10.0;

            var extensions = new HashSet<string>(GL.GetInteger(GetPName.NumExtensions));
            for (int i = 0; i < GL.GetInteger(GetPName.NumExtensions); i++)
            {
                string extensionString = GL.GetString(StringNameIndexed.Extensions, i);
                extensions.Add(extensionString);
            }

            string extensionsFilePath = "GLExtensions.txt";
            if (!File.Exists(extensionsFilePath))
            {
                using StreamWriter writer = new StreamWriter(extensionsFilePath);
                foreach (string text in extensions)
                {
                    writer.WriteLine(text);
                }
            }

            Extensions = extensions;
        }

        public static bool ContainsExtension(string extensionName)
        {
            return Extensions.Contains(extensionName);
        }

        public static class Limits
        {
            public static readonly int MaxTextureSize;     // GL_MAX_TEXTURE_SIZE
            public static readonly int MaxTextureSize3D;   // GL_MAX_3D_TEXTURE_SIZE
            public static readonly int MaxTextureSizeCube; // GL_MAX_CUBE_MAP_TEXTURE_SIZE

            public static readonly float MaxSamplerLodBias;       // GL_MAX_TEXTURE_LOD_BIAS
            public static readonly float MaxSamplerAnisotropy;    // GL_MAX_TEXTURE_MAX_ANISOTROPY
            public static readonly int MaxArrayTextureLayers; // GL_MAX_ARRAY_TEXTURE_LAYERS
            public static readonly Vector2i MaxViewportDims;    // GL_MAX_VIEWPORT_DIMS
            public static readonly int SubpixelBits;          // GL_SUBPIXEL_BITS

            public static readonly int MaxColorAttachments;  // GL_MAX_COLOR_ATTACHMENTS
            public static readonly int MaxSamples;              // GL_MAX_SAMPLES

            public static readonly Vector2 InterpolationOffsetRange; // GL_MIN_FRAGMENT_INTERPOLATION_OFFSET & GL_MAX_FRAGMENT_INTERPOLATION_OFFSET
            public static readonly float PointSizeGranularity;        // GL_POINT_SIZE_GRANULARITY
            public static readonly Vector2 PointSizeRange;           // GL_POINT_SIZE_RANGE
            public static readonly Vector2 LineWidthRange;           // GL_ALIASED_LINE_WIDTH_RANGE

            // Verts
            public static readonly int MaxElementIndices;               // GL_MAX_ELEMENT_INDEX

            public static readonly int MaxVertexAttribs;
            public static readonly int MaxVertexImageUniforms;
            public static readonly int MaxVertexOutputComponents;
            public static readonly int MaxVertexStreams;
            public static readonly int MaxVertexTextureImageUnits;
            public static readonly int MaxVertexUniformBlocks;
            public static readonly int MaxVertexUniformComponents;
            public static readonly int MaxVertexUniformVectors;
            public static readonly int MaxVertexVaryingComponents;
            public static readonly int MaxCombinedVertexUniformComponents;

            public static readonly int MaxTessellationControlPerVertexInputComponents; // GL_MAX_TESS_CONTROL_INPUT_COMPONENTS
            public static readonly int MaxTessellationControlPerVertexOutputComponents; // GL_MAX_TESS_CONTROL_OUTPUT_COMPONENTS
            public static readonly int MaxTessellationControlPerPatchOutputComponents; // GL_MAX_TESS_PATCH_COMPONENTS
            public static readonly int MaxTessellationControlTotalOutputComponents;    // GL_MAX_TESS_CONTROL_TOTAL_OUTPUT_COMPONENTS
            public static readonly int MaxTessellationEvaluationInputComponents;       // GL_MAX_TESS_EVALUATION_INPUT_COMPONENTS
            public static readonly int MaxTessellationEvaluationOutputComponents;      // GL_MAX_TESS_EVALUATION_OUTPUT_COMPONENTS

            public static readonly int MaxFragmentInputComponents;    // GL_MAX_FRAGMENT_INPUT_COMPONENTS
            public static readonly Vector2i TexelOffsetRange;           // GL_MIN_PROGRAM_TEXEL_OFFSET & GL_MAX_PROGRAM_TEXEL_OFFSET
            public static readonly Vector2i TextureGatherOffsetRange;   // GL_MIN_PROGRAM_TEXTURE_GATHER_OFFSET & GL_MAX_PROGRAM_TEXTURE_GATHER_OFFSET

            public static readonly int MaxTessellationGenerationLevel; // GL_MAX_TESS_GEN_LEVEL
            public static readonly int MaxPatchSize;                   // GL_MAX_PATCH_VERTICES

            public static readonly int MaxUniformBufferBindings;     // GL_MAX_UNIFORM_BUFFER_BINDINGS
            public static readonly int MaxUniformBlockSize;          // GL_MAX_UNIFORM_BLOCK_SIZE
            public static readonly int UniformBufferOffsetAlignment; // GL_UNIFORM_BUFFER_OFFSET_ALIGNMENT
            public static readonly int MaxCombinedUniformBlocks;     // GL_MAX_COMBINED_UNIFORM_BLOCKS

            public static readonly int MaxShaderStorageBufferBindings;     // GL_MAX_SHADER_STORAGE_BUFFER_BINDINGS
            public static readonly int MaxShaderStorageBlockSize;          // GL_MAX_SHADER_STORAGE_BLOCK_SIZE
            public static readonly int ShaderStorageBufferOffsetAlignment; // GL_SHADER_STORAGE_BUFFER_OFFSET_ALIGNMENT
            public static readonly int MaxCombinedShaderStorageBlocks;     // GL_MAX_COMBINED_SHADER_STORAGE_BLOCKS

            public static readonly int MaxCombinedShaderOutputResources; // GL_MAX_COMBINED_SHADER_OUTPUT_RESOURCES
            public static readonly int MaxCombinedTextureImageUnits;     // GL_MAX_COMBINED_TEXTURE_IMAGE_UNITS

            public static readonly int MaxComputeSharedMemorySize;     // GL_MAX_COMPUTE_SHARED_MEMORY_SIZE
            public static readonly int MaxComputeWorkGroupInvocations; // GL_MAX_COMPUTE_WORK_GROUP_INVOCATIONS
            public static readonly Vector3i MaxComputeWorkGroupCount;    // GL_MAX_COMPUTE_WORK_GROUP_COUNT
            public static readonly Vector3i MaxComputeWorkGroupSize;     // GL_MAX_COMPUTE_WORK_GROUP_SIZE

            public static readonly int MaxImageUnits;                      // GL_MAX_IMAGE_UNITS
            public static readonly int MaxFragmentCombinedOutputResources; // GL_MAX_COMBINED_IMAGE_UNITS_AND_FRAGMENT_OUTPUTS
            public static readonly int MaxCombinedImageUniforms;           // GL_MAX_COMBINED_IMAGE_UNIFORMS
            public static readonly int MaxServerWaitTimeout;               // GL_MAX_SERVER_WAIT_TIMEOUT

            static Limits()
            {
                GL.GetInteger(GetPName.MaxTextureSize, out MaxTextureSize);
                GL.GetInteger(GetPName.Max3DTextureSize, out MaxTextureSize3D);
                GL.GetInteger(GetPName.MaxCubeMapTextureSize, out MaxTextureSizeCube);

                GL.GetFloat(GetPName.MaxTextureLodBias, out MaxSamplerLodBias);
                GL.GetFloat(GetPName.MaxTextureMaxAnisotropy, out MaxSamplerAnisotropy);
                GL.GetInteger(GetPName.MaxArrayTextureLayers, out MaxArrayTextureLayers);
                GetVec2i(GetPName.MaxViewportDims, ref MaxViewportDims);
                GL.GetInteger(GetPName.SubpixelBits, out SubpixelBits);

                GL.GetInteger(GetPName.MaxColorAttachments, out MaxColorAttachments);
                GL.GetInteger(GetPName.MaxSamples, out MaxSamples);

                GL.GetFloat(GetPName.MinFragmentInterpolationOffset, out InterpolationOffsetRange.X);
                GL.GetFloat(GetPName.MaxFragmentInterpolationOffset, out InterpolationOffsetRange.Y);
                GL.GetFloat(GetPName.PointSizeGranularity, out PointSizeGranularity);
                GL.GetFloat(GetPName.PointSizeRange, out PointSizeRange.X);
                GL.GetFloat(GetPName.LineWidthRange, out LineWidthRange.Y);

                GL.GetInteger(GetPName.MaxElementsIndices, out MaxElementIndices);

                GL.GetInteger(GetPName.MaxVertexAttribs, out MaxVertexAttribs);
                GL.GetInteger(GetPName.MaxVertexImageUniforms, out MaxVertexImageUniforms);
                GL.GetInteger(GetPName.MaxVertexOutputComponents, out MaxVertexOutputComponents);
                GL.GetInteger(GetPName.MaxVertexStreams, out MaxVertexStreams);
                GL.GetInteger(GetPName.MaxVertexTextureImageUnits, out MaxVertexTextureImageUnits);
                GL.GetInteger(GetPName.MaxVertexUniformBlocks, out MaxVertexUniformBlocks);
                GL.GetInteger(GetPName.MaxVertexUniformComponents, out MaxVertexUniformComponents);
                GL.GetInteger(GetPName.MaxVertexUniformVectors, out MaxVertexUniformVectors);
                GL.GetInteger(GetPName.MaxVertexVaryingComponents, out MaxVertexVaryingComponents);
                GL.GetInteger(GetPName.MaxCombinedVertexUniformComponents, out MaxCombinedVertexUniformComponents);

                GL.GetInteger(GetPName.MaxTessControlInputComponents, out MaxTessellationControlPerVertexInputComponents);
                GL.GetInteger(GetPName.MaxTessControlOutputComponents, out MaxTessellationControlPerVertexOutputComponents);
                GL.GetInteger(GetPName.MaxTessPatchComponents, out MaxTessellationControlPerPatchOutputComponents);
                GL.GetInteger(GetPName.MaxTessControlTotalOutputComponents, out MaxTessellationControlTotalOutputComponents);
                GL.GetInteger(GetPName.MaxTessEvaluationInputComponents, out MaxTessellationEvaluationInputComponents);
                GL.GetInteger(GetPName.MaxTessEvaluationOutputComponents, out MaxTessellationEvaluationOutputComponents);

                GL.GetInteger(GetPName.MaxFragmentInputComponents, out MaxFragmentInputComponents);
                GL.GetInteger(GetPName.MinProgramTexelOffset, out TexelOffsetRange.X);
                GL.GetInteger(GetPName.MaxProgramTexelOffset, out TexelOffsetRange.Y);
                GL.GetInteger(GetPName.MinProgramTextureGatherOffset, out TextureGatherOffsetRange.X);
                GL.GetInteger(GetPName.MaxProgramTextureGatherOffset, out TextureGatherOffsetRange.Y);

                GL.GetInteger(GetPName.MaxTessGenLevel, out MaxTessellationGenerationLevel);
                GL.GetInteger(GetPName.MaxPatchVertices, out MaxPatchSize);

                GL.GetInteger(GetPName.MaxUniformBufferBindings, out MaxUniformBufferBindings);
                GL.GetInteger(GetPName.MaxUniformBlockSize, out MaxUniformBlockSize);
                GL.GetInteger(GetPName.UniformBufferOffsetAlignment, out UniformBufferOffsetAlignment);
                GL.GetInteger(GetPName.MaxCombinedUniformBlocks, out MaxCombinedUniformBlocks);

                GL.GetInteger((GetPName)All.MaxShaderStorageBufferBindings, out MaxShaderStorageBufferBindings);
                GL.GetInteger((GetPName)All.MaxShaderStorageBufferBindings, out MaxShaderStorageBufferBindings);
                GL.GetInteger((GetPName)All.MaxShaderStorageBlockSize, out MaxShaderStorageBlockSize);
                GL.GetInteger((GetPName)All.ShaderStorageBufferOffsetAlignment, out ShaderStorageBufferOffsetAlignment);
                GL.GetInteger((GetPName)All.MaxCombinedShaderStorageBlocks, out MaxCombinedShaderStorageBlocks);

                GL.GetInteger((GetPName)All.MaxCombinedShaderOutputResources, out MaxCombinedShaderOutputResources);
                GL.GetInteger(GetPName.MaxCombinedTextureImageUnits, out MaxCombinedTextureImageUnits);

                GL.GetInteger((GetPName)All.MaxComputeSharedMemorySize, out MaxComputeSharedMemorySize);
                GL.GetInteger((GetPName)All.MaxComputeWorkGroupInvocations, out MaxComputeWorkGroupInvocations);

                GL.GetInteger((GetIndexedPName)All.MaxComputeWorkGroupCount, 0, out MaxComputeWorkGroupCount.X);
                GL.GetInteger((GetIndexedPName)All.MaxComputeWorkGroupCount, 1, out MaxComputeWorkGroupCount.Y);
                GL.GetInteger((GetIndexedPName)All.MaxComputeWorkGroupCount, 2, out MaxComputeWorkGroupCount.Z);
                GL.GetInteger((GetIndexedPName)All.MaxComputeWorkGroupSize, 0, out MaxComputeWorkGroupSize.X);
                GL.GetInteger((GetIndexedPName)All.MaxComputeWorkGroupSize, 1, out MaxComputeWorkGroupSize.Y);
                GL.GetInteger((GetIndexedPName)All.MaxComputeWorkGroupSize, 2, out MaxComputeWorkGroupSize.Z);

                GL.GetInteger((GetPName)All.MaxImageUnits, out MaxImageUnits);
                GL.GetInteger((GetPName)All.MaxCombinedImageUnitsAndFragmentOutputs, out MaxFragmentCombinedOutputResources);
                GL.GetInteger(GetPName.MaxCombinedImageUniforms, out MaxCombinedImageUniforms);
                GL.GetInteger((GetPName)All.MaxServerWaitTimeout, out MaxServerWaitTimeout);
            }

            public static class SubgroupLimits
            {
                public static readonly int SubgroupSize; // GL_SUBGROUP_SIZE_KHR

                // ShaderObject stage support
                public static readonly bool VertexShaderSupported;                 // GL_VERTEX_SHADER_BIT
                public static readonly bool TessellationControlShaderSupported;    // GL_TESS_CONTROL_SHADER_BIT
                public static readonly bool TessellationEvaluationShaderSupported; // GL_TESS_EVALUATION_SHADER_BIT
                public static readonly bool FragmentShaderSupported;               // GL_FRAGMENT_SHADER_BIT
                public static readonly bool ComputeShaderSupported;                // GL_COMPUTE_SHADER_BIT
                public static readonly bool GeometryShaderSupported;               // GL_COMPUTE_SHADER_BIT

                // Features
                public static readonly bool VoteSupported;            // GL_SUBGROUP_FEATURE_VOTE_BIT_KHR
                public static readonly bool ArithmeticSupported;      // GL_SUBGROUP_FEATURE_ARITHMETIC_BIT_KHR
                public static readonly bool BallotSupported;          // GL_SUBGROUP_FEATURE_BALLOT_BIT_KHR
                public static readonly bool ShuffleSupported;         // GL_SUBGROUP_FEATURE_SHUFFLE_BIT_KHR
                public static readonly bool ShuffleRelativeSupported; // GL_SUBGROUP_FEATURE_SHUFFLE_RELATIVE_BIT_KHR
                public static readonly bool ClusteredSupported;       // GL_SUBGROUP_FEATURE_CLUSTERED_BIT_KHR
                public static readonly bool QuadSupported;            // GL_SUBGROUP_FEATURE_QUAD_BIT_KHR

                static SubgroupLimits()
                {
                    if (ContainsExtension("GL_KHR_shader_subgroup"))
                    {
                        GL.GetInteger((GetPName)All.SubgroupSizeKhr, out SubgroupSize);

                        GL.GetInteger((GetPName)All.SubgroupSupportedStagesKhr, out int subgroupStages);
                        const int GL_VERTEX_SHADER_BIT = 0x00000001;
                        const int GL_FRAGMENT_SHADER_BIT = 0x00000002;
                        const int GL_GEOMETRY_SHADER_BIT = 0x00000004;
                        const int GL_TESS_CONTROL_SHADER_BIT = 0x00000008;
                        const int GL_TESS_EVALUATION_SHADER_BIT = 0x00000010;
                        const int GL_COMPUTE_SHADER_BIT = 0x00000020;

                        Func<int, bool> checkCondition = value => value != 0;

                        VertexShaderSupported = checkCondition(subgroupStages & GL_VERTEX_SHADER_BIT);
                        TessellationControlShaderSupported = checkCondition(subgroupStages & GL_TESS_CONTROL_SHADER_BIT);
                        TessellationEvaluationShaderSupported = checkCondition(subgroupStages & GL_TESS_EVALUATION_SHADER_BIT);
                        FragmentShaderSupported = checkCondition(subgroupStages & GL_FRAGMENT_SHADER_BIT);
                        GeometryShaderSupported = checkCondition(subgroupStages & GL_GEOMETRY_SHADER_BIT);
                        ComputeShaderSupported = checkCondition(subgroupStages & GL_COMPUTE_SHADER_BIT);

                        const int GL_SUBGROUP_FEATURE_VOTE_BIT_KHR = 0x00000002;
                        const int GL_SUBGROUP_FEATURE_ARITHMETIC_BIT_KHR = 0x00000004;
                        const int GL_SUBGROUP_FEATURE_BALLOT_BIT_KHR = 0x00000008;
                        const int GL_SUBGROUP_FEATURE_SHUFFLE_BIT_KHR = 0x00000010;
                        const int GL_SUBGROUP_FEATURE_SHUFFLE_RELATIVE_BIT_KHR = 0x00000020;
                        const int GL_SUBGROUP_FEATURE_CLUSTERED_BIT_KHR = 0x00000040;
                        const int GL_SUBGROUP_FEATURE_QUAD_BIT_KHR = 0x00000080;

                        GL.GetInteger((GetPName)All.SubgroupSupportedStagesKhr, out int subgroupFeatures);
                        VoteSupported = checkCondition(subgroupFeatures & GL_SUBGROUP_FEATURE_VOTE_BIT_KHR);
                        ArithmeticSupported = checkCondition(subgroupFeatures & GL_SUBGROUP_FEATURE_ARITHMETIC_BIT_KHR);
                        BallotSupported = checkCondition(subgroupFeatures & GL_SUBGROUP_FEATURE_BALLOT_BIT_KHR);
                        ShuffleSupported = checkCondition(subgroupFeatures & GL_SUBGROUP_FEATURE_SHUFFLE_BIT_KHR);
                        ShuffleRelativeSupported = checkCondition(subgroupFeatures & GL_SUBGROUP_FEATURE_SHUFFLE_RELATIVE_BIT_KHR);
                        ClusteredSupported = checkCondition(subgroupFeatures & GL_SUBGROUP_FEATURE_CLUSTERED_BIT_KHR);
                        QuadSupported = checkCondition(subgroupFeatures & GL_SUBGROUP_FEATURE_QUAD_BIT_KHR);
                    }
                }
            }
        }

        public static class Features
        {
            public static readonly bool BindlessTextures = ContainsExtension("GL_ARB_bindless_texture"); // GL_ARB_bindless_texture
            public static readonly bool ShaderSubGroup = ContainsExtension("GL_KHR_shader_subgroup"); // GL_KHR_shader_subgroup
        }

        public static class GpuMemory
        {
            public static int MemoryTotal => GL.GetInteger((GetPName)NvxGpuMemoryInfo.GpuMemoryInfoTotalAvailableMemoryNvx) / 1024;

            public static int MemoryAvailable => GL.GetInteger((GetPName)NvxGpuMemoryInfo.GpuMemoryInfoCurrentAvailableVidmemNvx) / 1024;

            public static int MemoryUsage => MemoryTotal - MemoryAvailable;

            public static int MemoryDedicated => GL.GetInteger((GetPName)NvxGpuMemoryInfo.GpuMemoryInfoDedicatedVidmemNvx) / 1024;

            public static int MemoryEvicted => GL.GetInteger((GetPName)NvxGpuMemoryInfo.GpuMemoryInfoEvictedMemoryNvx) / 1024;

            public static int MemoryEvictionCount => GL.GetInteger((GetPName)NvxGpuMemoryInfo.GpuMemoryInfoEvictionCountNvx) / 1024;
        }
    }
}
