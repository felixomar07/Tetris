
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Threading.Tasks;



public class Shader
{
    private readonly int handle;

    public Shader(string vertexPath, string fragmentPath)
    {
        string vertexShaderSource = File.ReadAllText(vertexPath);
        string fragmentShaderSource = File.ReadAllText(fragmentPath);

        int vertexShader = GL.CreateShader(ShaderType.VertexShader);
        int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);

        GL.ShaderSource(vertexShader, vertexShaderSource);
        GL.CompileShader(vertexShader);

        GL.ShaderSource(fragmentShader, fragmentShaderSource);
        GL.CompileShader(fragmentShader);

        handle = GL.CreateProgram();
        GL.AttachShader(handle, vertexShader);
        GL.AttachShader(handle, fragmentShader);
        GL.LinkProgram(handle);

        GL.DetachShader(handle, vertexShader);
        GL.DetachShader(handle, fragmentShader);
        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);

    }

    public void Use()
    {
        GL.UseProgram(handle);
    }

    public void SetMatrix4(string name, Matrix4 matrix)
    {
        int location = GL.GetUniformLocation(handle, name);
        GL.UniformMatrix4(location, false, ref matrix);
    }

    public void SetVector4(string name, Vector4 vector)
    {
        int location = GL.GetUniformLocation(handle, name);
        GL.Uniform4(location, vector);
    }

    public void Dispose()
    {
        GL.DeleteProgram(handle);
    }
}
