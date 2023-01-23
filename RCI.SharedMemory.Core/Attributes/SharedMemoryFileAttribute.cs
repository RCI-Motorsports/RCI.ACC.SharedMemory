namespace RCI.SharedMemory.Core.Attributes;

[AttributeUsage(AttributeTargets.Struct)]
public class SharedMemoryFileAttribute : Attribute
{
  public string Name { get; }

  public SharedMemoryFileAttribute(string name)
  {
    Name = name;
  }
}