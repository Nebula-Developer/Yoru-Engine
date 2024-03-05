#nullable disable
namespace Yume.Graphics.Elements;

public class CircularElementReferenceException()
    : Exception("Circular reference detected. Setting this element as its own ancestor or descendant is not allowed.");