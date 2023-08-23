using Moco.Rendering.Display;

namespace Moco.SWF.Tags.Control;

/// <summary>
/// A tag that affects the display list.
/// </summary>
public interface IDisplayListAffectingTag : IControlTag
{
	/// <summary>
	/// Executes this tag on the display list.
	/// </summary>
	/// <param name="displayList">The display list.</param>
	void Execute(DisplayList displayList);
}
