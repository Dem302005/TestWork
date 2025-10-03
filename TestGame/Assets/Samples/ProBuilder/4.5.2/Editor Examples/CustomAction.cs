// This script demonstrates how to create a new action that can be accessed from the ProBuilder toolbar.
// A new menu item is registered under "Geometry" actions called "Make Double-Sided".
// To enable, remove the #if PROBUILDER_API_EXAMPLE and #endif directives.

using System.Linq;
using UnityEditor;
using UnityEditor.ProBuilder;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

namespace ProBuilder.ExampleActions
{
	/// <summary>
	///     This is the actual action that will be executed.
	/// </summary>
	public class MakeFacesDoubleSided : MenuAction
    {
	    /// <summary>
	    ///     What to show in the hover tooltip window.
	    ///     TooltipContent is similar to GUIContent, with the exception that it also includes an optional params[]
	    ///     char list in the constructor to define shortcut keys (ex, CMD_CONTROL, K).
	    /// </summary>
	    private static readonly TooltipContent k_Tooltip = new TooltipContent
        (
            "Set Double-Sided",
            "Adds another face to the back of the selected faces."
        );

        public override ToolbarGroup group => ToolbarGroup.Geometry;
        public override Texture2D icon => null;
        public override TooltipContent tooltip => k_Tooltip;

        /// <summary>
        ///     Determines if the action should be enabled or grayed out.
        /// </summary>
        /// <returns></returns>
        public override bool enabled => MeshSelection.selectedFaceCount > 0;

        /// <summary>
        ///     This action is applicable in Face selection modes.
        /// </summary>
        public override SelectMode validSelectModes => SelectMode.Face | SelectMode.TextureFace;

        /// <summary>
        ///     Return a pb_ActionResult indicating the success/failure of action.
        /// </summary>
        /// <returns></returns>
        public override ActionResult DoAction()
        {
            var selection = MeshSelection.top.ToArray();
            Undo.RecordObjects(selection, "Make Double-Sided Faces");

            foreach (var mesh in selection)
            {
                mesh.DuplicateAndFlip(mesh.GetSelectedFaces());

                mesh.ToMesh();
                mesh.Refresh();
                mesh.Optimize();
            }

            // Rebuild the pb_Editor caches
            ProBuilderEditor.Refresh();

            return new ActionResult(ActionResult.Status.Success, "Make Faces Double-Sided");
        }
    }
}