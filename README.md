
# OpenTK.Utility Extension Library

##### This C# library was developed to provide an intuitive and efficient interface for graphics rendering using OpenGL through the OpenTK library. It offers a wide range of functionalities to create and manipulate OpenGL objects, buffers, textures of various types, and more. This project allows developers to create graphic applications with ease.

#### Key Features
* Structured and Organized OpenGL Objects leveraging Modern OpenGL and extensive use of Direct State Access (DSA).

* Full support for creating and managing well-structured buffers that are easy to use and ensure complete security in data manipulation and access.

* Support for a variety of texture types, including 1D, 2D, 3D, cube textures, and array textures, enabling a wide range of visual effects in your applications.

* This library includes built-in support for Reading and writing images using StbImageSharp and StbimageWriterSharp, seamlessly integrated into the codebase. This feature allows for effortless loading of various image formats directly within the library without external dependencies

* The development was designed to be extensible, so almost everything has interfaces, so you can use your own codes if the project structure doesn't suit you.

* It provides control to OpenGL where if you do something that compromises the state of OpenGL, an exception can be thrown with information about what was done wrong.
Remember that you can't control everything, so errors can still arise, keep that in mind.

#### Installation
To use this library in your project, you can clone the repository directly to your local machine. After cloning the repository, you'll be able to reference the project within your own development environment. This method provides flexibility and control over integrating the library into your project, allowing you to easily track updates and contribute to the source code if desired.

Currently, there is no package available in the form of a pre-packaged distribution file (such as a NuGet package), but this may be provided in the future. Referencing the cloned project is a convenient way to start using the library while waiting for a formal distribution.

### Usage Examples

###### Namespaces
```csharp
OpenTK.Utilities
OpenTK.Utilities.Textures
OpenTK.Utilities.Objects
OpenTK.Utilities.BuffersObjects
OpenTK.Utilities.Images
OpenTK.Utilities.Assistants
```



___
###### Vertexs Arrays
```csharp
BufferVertices = new BufferImmutable<Data>(Vertices, StorageUseFlag.ClientStorageBit);
BufferElements = new BufferImmutable<uint>(indices, StorageUseFlag.ClientStorageBit);

VertexArrayObject = new VertexArrayObject();
VertexArrayObject.SetElementBuffer(BufferElements);
VertexArrayObject.AddVertexBuffer(0, BufferVertices);

VertexArrayObject.SetAttribFormat(0, 0, 3, VertexAttribType.Float, (int)Marshal.OffsetOf<Data>("Pos"));
VertexArrayObject.SetAttribFormat(0, 1, 2, VertexAttribType.Float, (int)Marshal.OffsetOf<Data>("TexCoord"));
VertexArrayObject.SetAttribFormat(0, 2, 4, VertexAttribType.Float, (int)Marshal.OffsetOf<Data>("Color"));
```
___
###### Buffers
```csharp
var bufferConstant = new BufferConstant<Vector3>();
bufferConstant.BindBufferBase(BufferRangeTarget.UniformBuffer, 0);
bufferConstant.Data = new Vector3(1, 1, 0);
```
```csharp
var bufferMapped = new BufferMapping<Transform>(5);
bufferMapped.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, bindingIndex: 1);
// Update index value
ref Transform data = ref bufferMapped[2];
data.Position = new Vector3(82.0f, 200f, 10f);
data.Scaling = new Vector3(2.0f);
data.Orientation = Quaternion.Identity;

foreach( var i in bufferMapped)
{
    Console.WriteLine(i);
}
// Position: <0. 0. 0>      Scaling: <0. 0. 0> Orientation: {X:0 Y:0 Z:0 W:0}
// Position: <0. 0. 0>      Scaling: <0. 0. 0> Orientation: {X:0 Y:0 Z:0 W:0}
// Position: <82. 200. 10>  Scaling: <2. 2. 2> Orientation: {X:0 Y:0 Z:0 W:1}
// Position: <0. 0. 0>      Scaling: <0. 0. 0> Orientation: {X:0 Y:0 Z:0 W:0}
// Position: <0. 0. 0>      Scaling: <0. 0. 0> Orientation: {X:0 Y:0 Z:0 W:0}

```

```csharp
var bufferMutable = new BufferMutable<Data>(BufferUsageHint.DynamicDraw);
bufferMutable.Reserve(50);
bufferMutable.ReplaceSubData(new Data[50]);

bufferMutable.Reserve(10);
bufferMutable.ReplaceSubData(new Data[10]);
bufferMutable[5] = new Data();

// Don't keep this, Read the description of this object.
MappedRegion<Data> mappedRegion = bufferMutable.GetMapping(2, 8);
mappedRegion[0].Pos = new Vector3(100f);
mappedRegion[^1].Pos = new Vector3(450f);

foreach (var i in mappedRegion)
{
    Console.WriteLine(i);
}
// Pos: <100. 100. 100> TexCoord: <0. 0> Color: <0. 0. 0. 0>
// Pos: <0. 0. 0>       TexCoord: <0. 0> Color: <0. 0. 0. 0>
// Pos: <0. 0. 0>       TexCoord: <0. 0> Color: <0. 0. 0. 0>
// Pos: <450. 450. 450> TexCoord: <0. 0> Color: <0. 0. 0. 0>

// It is crucial that it is discarded.
//-------------------------
mappedRegion.Dispose();
//-------------------------

foreach (var i in bufferMutable)
{
    Console.WriteLine(i);
}

// Pos: <0. 0. 0>       TexCoord: <0. 0> Color: <0. 0. 0. 0>
// Pos: <0. 0. 0>       TexCoord: <0. 0> Color: <0. 0. 0. 0>
// Pos: <100. 100. 100> TexCoord: <0. 0> Color: <0. 0. 0. 0>
// Pos: <0. 0. 0>       TexCoord: <0. 0> Color: <0. 0. 0. 0>
// Pos: <0. 0. 0>       TexCoord: <0. 0> Color: <0. 0. 0. 0>
// Pos: <0. 0. 0>       TexCoord: <0. 0> Color: <0. 0. 0. 0>
// Pos: <0. 0. 0>       TexCoord: <0. 0> Color: <0. 0. 0. 0>
// Pos: <0. 0. 0>       TexCoord: <0. 0> Color: <0. 0. 0. 0>
// Pos: <450. 450. 450> TexCoord: <0. 0> Color: <0. 0. 0. 0>
// Pos: <0. 0. 0>       TexCoord: <0. 0> Color: <0. 0. 0. 0>

bufferMutable.Dispose();
```
___
###### Shaders
**warning:** Just to remember, I decided to call 'GLShaderProgram' just ShaderObject.

Using the Traditional way
```csharp
ShaderSample = ShaderObject.CreateProgram(
    ShaderLoader.FromFile(ShaderType.VertexShader, "Resources/Vertex.vert"),
    ShaderLoader.FromFile(ShaderType.FragmentShader, "Resources/Fragment.frag"))
{
    EnableExceptions = true,
};

ShaderSample.Uniform("Model", transform.ModelMatrix);
//Exception: OpenTK.Utilities.UniformNotFoundException: 'Uniform: 'Model' not found.'

VertexArrayObject.SetAttribFormat(0, ShaderSample.GetAttribute("insPos"), 3, VertexAttribType.Float, (int)Marshal.OffsetOf<Data>("Pos"));
// Exception: OpenTK.Utilities.AttributeNotFoundException: 'Attribute: 'insPos' not found.'
```
Combined with pipeline object
```csharp
// Note that it must be created in a separable way.
ShaderProgVert = ShaderObject.CreateProgramSeparable(
    ShaderSource.FromFile(ShaderType.VertexShader, "Resources/Vertex.vert"));

ShaderProgFrag = ShaderObject.CreateProgramSeparable(
    ShaderSource.FromFile(ShaderType.FragmentShader, "Resources/Fragment.frag"));

Pipeline = new Pipeline();
Pipeline.SetShader(ShaderProgVert, ShaderProgFrag);
```
It could be
```csharp
ShaderSample = ShaderObject.CreateProgram(
    separable: true,
    ShaderSource.FromFile(ShaderType.VertexShader, "Resources/Vertex.vert"),
    ShaderSource.FromFile(ShaderType.FragmentShader, "Resources/Fragment.frag"));

Pipeline = new PipelineObject();

Pipeline.SetShader(ShaderSample);
// or
Pipeline.SetShaderAllStages(ShaderSample);

```
##### Drawing
```csharp
Pipeline.Bind();
VertexArrayObject.Bind();
Drawing.DrawElements(DrawElementsType.UnsignedInt, BufferElements.Count);
Drawing.ResetDrawingContext();

// Or 

using var BufferIndiretCmd = new BufferConstant<DrawElementsIndirectCommand>();

BufferIndiretCmd.Data = new DrawElementsIndirectCommand
{
    Base = 0,
    Count = BufferElements.Count,
    BaseInstance = 0,
    InstanceCount = 100,
    BaseVertex = 0,
};

Pipeline.Bind();
VertexArrayObject.Bind();
Drawing.DrawElementsIndirect(BufferIndiretCmd, DrawElementsType.UnsignedInt);
// It is advisable to use this depending on the way you choose to render your objects. 
// Even more so if you are using both the pipeline object and the shader object in your application.
Drawing.ResetDrawingContext();

// For the most inexperienced ex:
// If you render using a pipeline object and then try to render 
// with a shader object and have not restored the pipeline context 
// it will not render anything, at render time it must either have a 
// shader program linked, or a pipeline program.
```

___
##### Texture 
Load
```csharp
var img = Image.FromFile("Resources/Goku Ultra Instinct 4K.jpg");
Texture = new Texture2D(TextureFormat.Srgb8, img.Width, img.Height);
Texture.Update(img.Width, img.Height, PixelFormat.Rgb, PixelType.UnsignedByte, img.Data);
```

**For more information see:** [khronos docs](https://www.khronos.org/opengl/wiki/Bindless_Texture)
```csharp
// Use of extension 'GL_ARB_bindless_texture'.
ShaderSample.Uniform(0, Texture.BindlessHandler);
```

Save
```csharp
if(ImGui.Button("SaveScreen"))
{
    using Texture2D screenTex = Context.Binding.FramebufferBinding.Default.ExtractTextureColor<Texture2D>(WinSize);
    TextureManager.SaveJpg(screenTex, filePath: "Resources", fileName: "ScreenShoot", 100)
}
```
<image src="Test/Resources/ScreenShoot.jpg" alt="Screen capture">

___
###### Device
```csharp
Console.WriteLine($"Renderer: {Context.Device.Renderer}");
Console.WriteLine($"Vendor: {Context.Device.Vendor}");
Console.WriteLine($"Version: {Context.Device.Version}");
Console.WriteLine($"Major Minor Version: {Context.Device.MajorMinorVersion}");
Console.WriteLine($"Shading Language Version: {Context.Device.ShadingLanguageVersion}");
Console.WriteLine($"Memory Total: {Context.Device.GpuMemory.MemoryTotal}");
Console.WriteLine($"Memory Available: {Context.Device.GpuMemory.MemoryAvailable}");
Console.WriteLine($"Memory Usage: {Context.Device.GpuMemory.MemoryUsage}");
```
Renderer: NVIDIA GeForce GTX 660/PCIe/SSE2
Vendor: NVIDIA Corporation
Version: 4.6.0 NVIDIA 474.44
Major Minor Version: 4,6
Shading Language Version: 4.60 NVIDIA
Memory Total: 2048
Memory Available: 1156
Memory Usage: 892
___

#### Contribution
Contributions are welcome! There's still a lot to do and review, I've only tested a few features, feel free to report issues, suggest improvements, or send pull requests for this project.

##### Credits
* https://github.com/opentk/opentk
* https://github.com/StbSharp/StbImageSharp
* https://github.com/StbSharp/StbImageWriteSharp