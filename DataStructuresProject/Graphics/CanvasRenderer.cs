using System;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using DataStructuresProject.Core;

namespace DataStructuresProject.Graphics
{
    public static class CanvasRenderer
    {
        public const int VIRTUAL_SAFE_MARGIN_X = 1;
        public const int VIRTUAL_SAFE_MARGIN_Y = 2;

        public static void RedrawAllChildren(BinarySearchTree renderTree)
        {
            RedrawChildrenFromSubtree(renderTree.GetRoot(), null);
        }

        public static void RedrawChildrenFromSubtree(BinarySearchTreeNode? subtree, BinarySearchTreeNode? parentNode)
        {
            if (subtree == null)
            {
                return;
            }

            double nodePhysicalWidth = 0;
            Vector2 nodePhysicalPosition = Vector2.Zero;
            bool isNodeDrawn = true;

            if (subtree.Data.TryGetValue(CanvasController.DataProps[DataNodeProperties.PhysicalNode], out object? physicalNodeData))
            {
                if (physicalNodeData is CanvasItem canvasItem)
                {
                    isNodeDrawn = TransformItemToDeviceCoordinates(canvasItem);
                    nodePhysicalWidth = canvasItem.PhysicalInstance.Width;
                    nodePhysicalPosition = new Vector2(
                        Canvas.GetLeft(canvasItem.PhysicalInstance),
                        Canvas.GetTop(canvasItem.PhysicalInstance));

                    if (canvasItem.PhysicalInstance is Ellipse physicalEllipse)
                    {
                        physicalEllipse.StrokeThickness = CalculateFinalStrokeThickness(5);
                    }
                }
            }
            if (subtree.Data.TryGetValue(CanvasController.DataProps[DataNodeProperties.Text], out object? keyText))
            {
                if (keyText is Label uiLabel)
                {
                    if (isNodeDrawn)
                    {
                        uiLabel.IsVisible = true;
                    }
                    else
                    {
                        uiLabel.IsVisible = false;
                    }

                    uiLabel.FontSize = FitFontToNodeSize(subtree.Key.ToString(), nodePhysicalWidth);
                    //will be a square
                    uiLabel.Width = nodePhysicalWidth;
                    uiLabel.Height = nodePhysicalWidth;

                    Canvas.SetLeft(uiLabel, nodePhysicalPosition.X);
                    Canvas.SetTop(uiLabel, nodePhysicalPosition.Y);
                }
            }
            //if the node is the root, it won't have this
            if (subtree.Data.TryGetValue(CanvasController.DataProps[DataNodeProperties.UpperArrow], out object? upperArrow))
            {
                if (upperArrow is Arrow arrow)
                {
                    //TransformItemToDeviceCoordinates(arrow);

                    Vector2 arrowStartingPoint = Vector2.Zero;
                    if (physicalNodeData is CanvasItem thisNode)
                    {
                        arrowStartingPoint = new Vector2(
                            Canvas.GetLeft(thisNode.PhysicalInstance) + (thisNode.PhysicalInstance.Width / 2),
                            Canvas.GetTop(thisNode.PhysicalInstance) + (thisNode.PhysicalInstance.Height / 2));
                    }
                    //by default it will be the same as starting point to prevent drawing an arrow
                    //if the node doesn't have a parent (should only apply to the root node)
                    Vector2 arrowEndingPoint = arrowStartingPoint;
                    if (parentNode?.Data.TryGetValue(CanvasController.DataProps[DataNodeProperties.PhysicalNode], out object? parentInstance) ?? false)
                    {
                        if (parentInstance is CanvasItem parentItem)
                        {
                            arrowEndingPoint = new Vector2(
                                Canvas.GetLeft(parentItem.PhysicalInstance) + (parentItem.PhysicalInstance.Width / 2),
                                Canvas.GetTop(parentItem.PhysicalInstance) + (parentItem.PhysicalInstance.Height / 2));
                        }
                    }

                    if (arrow.PhysicalInstance is Line physicalLine)
                    {
                        Canvas.SetLeft(physicalLine, 0);
                        Canvas.SetTop(physicalLine, 0);

                        physicalLine.StartPoint = new Avalonia.Point(arrowStartingPoint.X, arrowStartingPoint.Y);
                        physicalLine.EndPoint = new Avalonia.Point(arrowEndingPoint.X, arrowEndingPoint.Y);

                        physicalLine.StrokeThickness = CalculateFinalStrokeThickness(5);
                        /*if (arrow.IsOrientedNorthEast)
                        {
                            physicalLine.StartPoint = new Avalonia.Point(0, 0);
                            physicalLine.EndPoint = new Avalonia.Point(physicalLine.Width, physicalLine.Height);
                        }
                        else
                        {
                            physicalLine.StartPoint = new Avalonia.Point(physicalLine.Width, 0);
                            physicalLine.EndPoint = new Avalonia.Point(0, physicalLine.Height);
                        }*/
                    }
                }
            }

            RedrawChildrenFromSubtree(subtree.LeftNode, subtree);
            RedrawChildrenFromSubtree(subtree.RightNode, subtree);
        }

        #region Graphics calculations
        /// <summary>
        /// Given a canvas item, will convert its virtual units to physical units and will apply them to the item.
        /// </summary>
        /// <param name="canvasItem"></param>
        /// <returns>True if the item would be inside the visible area, false otherwise. If false, the item will automatically be hidden.</returns>
        public static bool TransformItemToDeviceCoordinates(CanvasItem canvasItem)
        {
            //represents the size (width or height) that can be seen on the screen
            double viewWidth = Math.Abs(UnitConverter.ConvertPixelsToVirtualUnits(CanvasController.PhysicalWidth));
            double viewHeight = Math.Abs(UnitConverter.ConvertPixelsToVirtualUnits(CanvasController.PhysicalHeight));

            //defines the clip space (the total size in virtual units that can be seen on the screen)
            //1 and 2 are safe margins so that arrows work correctly even when a node is outside the clip space
            Vector2 clipSpaceTopLeftCorner = new Vector2(
                CanvasController.VirtualOffset.X - VIRTUAL_SAFE_MARGIN_X - (viewWidth / 2),
                CanvasController.VirtualOffset.Y + VIRTUAL_SAFE_MARGIN_Y + (viewHeight / 2));
            Vector2 clipSpaceBottomRightCorner = new Vector2(
                CanvasController.VirtualOffset.X + VIRTUAL_SAFE_MARGIN_X + (viewWidth / 2),
                CanvasController.VirtualOffset.Y - VIRTUAL_SAFE_MARGIN_Y - (viewHeight / 2));

            if (canvasItem.Position.X > clipSpaceBottomRightCorner.X ||
                canvasItem.Position.Y < clipSpaceBottomRightCorner.Y ||
                (canvasItem.Position.X + canvasItem.Width < clipSpaceTopLeftCorner.X) ||
                //Y would be positive here
                (canvasItem.Position.Y - canvasItem.Height > clipSpaceTopLeftCorner.Y))
            {
                //TODO: check why this puts arrows and nodes to random points
                return false;
            }

            double physicalXDistanceFromTopLeftCorner = UnitConverter.ConvertVirtualUnitsToPixels(
                canvasItem.Position.X - (clipSpaceTopLeftCorner.X + VIRTUAL_SAFE_MARGIN_X));
            double physicalYDistanceFromTopLeftCorner = UnitConverter.ConvertVirtualUnitsToPixels(
                clipSpaceTopLeftCorner.Y - VIRTUAL_SAFE_MARGIN_Y - canvasItem.Position.Y);

            Canvas.SetLeft(canvasItem.PhysicalInstance, physicalXDistanceFromTopLeftCorner);
            Canvas.SetTop(canvasItem.PhysicalInstance, physicalYDistanceFromTopLeftCorner);
            canvasItem.PhysicalInstance.Width = UnitConverter.ConvertVirtualUnitsToPixels(canvasItem.Width);
            canvasItem.PhysicalInstance.Height = UnitConverter.ConvertVirtualUnitsToPixels(canvasItem.Height);

            return true;
        }

        /// <summary>
        /// Given the text that needs to be drawn and the node width (being an circle, it's equivalent to its diameter),
        /// will approximate the correct font size that needs to be applied to the text in order to fit within the node.
        /// </summary>
        /// <param name="textToRender">The text to be drawn.</param>
        /// <param name="nodeDiameter">The node's width/diameter.</param>
        /// <returns>The font size that should be applied to the node text.</returns>
        public static double FitFontToNodeSize(string textToRender, double nodeDiameter)
        {
            double widthPerChar = nodeDiameter / (textToRender.Length > 0 ? textToRender.Length : 1);

            if (textToRender.Length <= 2)
            {
                widthPerChar *= 0.35 * textToRender.Length;
            }
            else
            {
                //make text 80% from the node width
                widthPerChar *= 0.8;
            }

            //clamp the value to avoid aberrant values
            widthPerChar = Math.Clamp(widthPerChar, 0, 128);
            //approximate height as being 1.5x the width (will depend on the font and is too difficult to calculate precisely)
            return widthPerChar * 1.5;
        }

        /// <summary>
        /// Given the thickness on 100% zoom, will calculate the new thickness for the current zoom level.
        /// </summary>
        /// <param name="normalThickness"></param>
        /// <returns>The thickness that should be implemented.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double CalculateFinalStrokeThickness(double normalThickness)
        {
            return normalThickness * CanvasController.Zoom;
        }
        #endregion
    }
}
