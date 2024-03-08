#nullable disable
namespace Yoru.Graphics;

public class CircularElementReferenceException()
    : Exception("Circular reference detected. Setting this element as its own ancestor or descendant is not allowed.");