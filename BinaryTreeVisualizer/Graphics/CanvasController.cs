using System;
using System.Collections.Generic;
using Avalonia.Controls;
using BinaryTreeVisualizer.Core;
using BinaryTreeVisualizer.Views;

namespace BinaryTreeVisualizer.Graphics
{
    public static class CanvasController
    {
        public static Dictionary<DataNodeProperties, string> DataProps { get; } = new()
        {
            { DataNodeProperties.PhysicalNode, "PhysicalNode" },
            { DataNodeProperties.Text, "Text" },
            { DataNodeProperties.UpperArrow, "UpperArrow" }
        };

        /// <summary>
        /// Represents the offset from the origin in virtual units.
        /// </summary>
        public static Vector2 VirtualOffset { get; set; } = new();
        public static double PhysicalWidth { get; private set; }
        public static double PhysicalHeight { get; private set; }
        public static double Zoom { get; private set; } = 1.0f;

        /// <summary>
        /// Contains all the relevant canvas items as a binary tree.
        /// </summary>
        public static BinarySearchTree PhysicalItems { get; private set; } = new BinarySearchTree();

        public static void UpdateCanvasSize(double width, double height)
        {
            PhysicalWidth = width;
            PhysicalHeight = height;
            CanvasRenderer.RedrawAllChildren(PhysicalItems);
        }

        public static void UpdateZoomLevel(double zoom)
        {
            Zoom = Math.Clamp(zoom, 0.1, 5);
            CanvasRenderer.RedrawAllChildren(PhysicalItems);
        }

        public static void UpdateOffset(double deltaX, double deltaY)
        {
            VirtualOffset.X += UnitConverter.ConvertPixelsToVirtualUnits(deltaX);
            VirtualOffset.Y += UnitConverter.ConvertPixelsToVirtualUnits(deltaY);
            CanvasRenderer.RedrawAllChildren(PhysicalItems);
        }

        public static bool AddNode(int key)
        {
            BinarySearchTreeNode? duplicateNode = PhysicalItems.Search(key);
            if (duplicateNode != null)
            {
                return false;
            }

            //Vector2 nodePosition = new Vector2(-0.5, 0.5);

            BinarySearchTreeNode? parentNode = PhysicalItems.GetRoot();
            if (parentNode != null)
            {
                while (true)
                {
                    if (key < parentNode!.Key)
                    {
                        //nodePosition = new Vector2(nodePosition.X - 1, nodePosition.Y - 2);

                        if (parentNode.LeftNode == null)
                        {
                            break;
                        }
                        else
                        {
                            parentNode = parentNode.LeftNode;
                        }
                    }
                    else
                    {
                        //nodePosition = new Vector2(nodePosition.X + 1, nodePosition.Y - 2);

                        if (parentNode.RightNode == null)
                        {
                            break;
                        }
                        else
                        {
                            parentNode = parentNode.RightNode;
                        }
                    }
                }
            }

            BinarySearchTreeNode dataNode = new BinarySearchTreeNode
            {
                Key = key,
            };

            TreeNode node = new TreeNode()
            {
                NodeKey = key,
            };
            MainView.AddItemToCanvas(node.PhysicalInstance);
            dataNode.Data.Add(DataProps[DataNodeProperties.PhysicalNode], node);

            Label uiLabel = new Label()
            {
                Content = dataNode.Key.ToString(),

                VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center,
                HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                Target = node.PhysicalInstance,
                ZIndex = 3,
            };
            MainView.AddItemToCanvas(uiLabel);
            dataNode.Data.Add(DataProps[DataNodeProperties.Text], uiLabel);

            //for the root node we won't have arrows
            if (parentNode != null)
            {
                Arrow arrow = new Arrow();
                MainView.AddItemToCanvas(arrow.PhysicalInstance);
                dataNode.Data.Add(DataProps[DataNodeProperties.UpperArrow], arrow);
            }

            PhysicalItems.Insert(key, dataNode.Data);
            TreeGeometryController.DetermineCorrectNodePositions(PhysicalItems);
            TreeGeometryController.SetCorrectNodePositions(PhysicalItems);
            CanvasRenderer.RedrawAllChildren(PhysicalItems);
            MainView.UpdateTreeInfo(PhysicalItems.Depth, PhysicalItems.Size);
            return true;
        }

        public static bool DeleteNode(int key)
        {
            BinarySearchTreeNode? nodeToDelete = PhysicalItems.Search(key);
            if (nodeToDelete == null)
            {
                return false;
            }

            TreeNode physicalTreeNode = (TreeNode)nodeToDelete.Data[DataProps[DataNodeProperties.PhysicalNode]];
            MainView.RemoveItemFromCanvas(physicalTreeNode.PhysicalInstance);
            Label physicalLabel = (Label)nodeToDelete.Data[DataProps[DataNodeProperties.Text]];
            MainView.RemoveItemFromCanvas(physicalLabel);
            //it might be the root node, don't try to delete the arrow as it's null
            if (nodeToDelete.Data.TryGetValue(DataProps[DataNodeProperties.UpperArrow], out object? physicalArrow))
            {
                MainView.RemoveItemFromCanvas(((Arrow)physicalArrow).PhysicalInstance);
            }

            bool success = PhysicalItems.Delete(key);
            TreeGeometryController.DetermineCorrectNodePositions(PhysicalItems);
            TreeGeometryController.SetCorrectNodePositions(PhysicalItems);
            CanvasRenderer.RedrawAllChildren(PhysicalItems);
            MainView.UpdateTreeInfo(PhysicalItems.Depth, PhysicalItems.Size);
            return success;
        }
    }

    public enum DataNodeProperties
    {
        None = 0,
        PhysicalNode = 1,
        Text = 2,
        UpperArrow = 3
    }
}
