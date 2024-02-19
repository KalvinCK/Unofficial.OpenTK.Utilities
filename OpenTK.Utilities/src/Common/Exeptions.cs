using OpenTK.Graphics.OpenGL4;

namespace OpenTK.Utilities;

public class ShaderCompilerException(ShaderType shaderType, string info)
    : Exception($"Failed to compile shader.\nType: {shaderType}.\nInfo: {info}.") { }

public class ShaderProgramException(string info)
    : Exception($"Failed to create shader program.\nInfo: {info}.") { }

public class AttributeNotFoundException(string Attrname)
    : Exception($"Attribute: {Attrname} not found.") { }

public class UniformNotFoundException(string uniName)
    : Exception($"Uniform: {uniName} not found.") { }

public class UnallocatedBufferException(string? message = null)
    : Exception(message ?? "The buffer was not allocated.") { }

public class EmptyAllocationException(string? message = null)
    : Exception(message ?? "Empty allocation attempt.") { }

public class StaticBufferAllocationException(string? message = null)
    : Exception(message ?? "Static buffer object cannot be relocated.") { }

public class UnallocatedTextureException(string? message = null)
    : Exception(message ?? "Texture was not allocated.") { }

public class EmptyCollectionException(string? message = null)
    : Exception(message ?? "The collection is empty, cannot get the first element.") { }

public class InvalidEnumException<TEnum>()
    : Exception($"Invalid operation on this enum: {nameof(TEnum)}")
    where TEnum : Enum { }

public class TextureUpdatePixelsException(string message)
    : Exception(message) { }
