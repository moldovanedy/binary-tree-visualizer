using System;
using System.Collections.Generic;
using Avalonia.Controls;
using DataStructuresProject.Core;
using DataStructuresProject.Views;

namespace DataStructuresProject.Graphics
{
    public static class CanvasController
    {
        public static Dictionary<DataNodeProperties, string> DataProps { get; } = new Dictionary<DataNodeProperties, string>()
        {
            { DataNodeProperties.PhysicalNode, "PhysicalNode" },
            { DataNodeProperties.Text, "Text" },
            { DataNodeProperties.UpperArrow, "UpperArrow" }
        };

        /// <summary>
        /// Represents the offset from the origin in virtual units.
        /// </summary>
        public static Vector2 VirtualOffset { get; private set; } = Vector2.Zero;
        public static double PhysicalWidth { get; private set; } = 0;
        public static double PhysicalHeight { get; private set; } = 0;
        public static double Zoom { get; private set; } = 1.0f;

        /// <summary>
        /// Contains all the relevant canvas items as a binary tree.
        /// </summary>
        private static BinarySearchTree _physicalItems = new BinarySearchTree();

        public static void UpdateCanvasSize(double width, double height)
        {
            PhysicalWidth = width;
            PhysicalHeight = height;
            CanvasRenderer.RedrawAllChildren(_physicalItems);
        }

        public static void UpdateZoomLevel(double zoom)
        {
            Zoom = Math.Clamp(zoom, 0.1, 5);
            CanvasRenderer.RedrawAllChildren(_physicalItems);
        }

        public static void UpdateOffset(double deltaX, double deltaY)
        {
            //Debug.WriteLine($"deltaX: {deltaX}, deltaY: {deltaY}");
            VirtualOffset.X += UnitConverter.ConvertPixelsToVirtualUnits(deltaX);
            VirtualOffset.Y += UnitConverter.ConvertPixelsToVirtualUnits(deltaY);
            CanvasRenderer.RedrawAllChildren(_physicalItems);
        }

        public static bool AddNode(int key)
        {
            BinarySearchTreeNode? duplicateNode = _physicalItems.Search(key);
            if (duplicateNode != null)
            {
                return false;
            }

            Vector2 nodePosition = new Vector2(-0.5, 0.5);

            BinarySearchTreeNode? parentNode = _physicalItems.GetRoot();
            if (parentNode != null)
            {
                while (true)
                {
                    if (key < parentNode!.Key)
                    {
                        nodePosition = new Vector2(nodePosition.X - 1, nodePosition.Y - 2);

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
                        nodePosition = new Vector2(nodePosition.X + 1, nodePosition.Y - 2);

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
                //ParentNode = parentNode,
            };

            TreeNode node = new TreeNode()
            {
                Position = nodePosition,
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
                Arrow arrow = new Arrow()
                {
                    //up 2 units and left one unit, regardless of the orientation
                    Position = new Vector2(nodePosition.X - 1, nodePosition.Y + 2)
                };
                MainView.AddItemToCanvas(arrow.PhysicalInstance);
                dataNode.Data.Add(DataProps[DataNodeProperties.UpperArrow], arrow);
            }

            _physicalItems.Insert(key, dataNode.Data);
            CanvasRenderer.RedrawAllChildren(_physicalItems);
            return true;
        }

        public static bool DeleteNode(int key)
        {
            BinarySearchTreeNode? nodeToDelete = _physicalItems.Search(key);
            if (nodeToDelete == null)
            {
                return false;
            }

            TreeNode physicalTreeNode = (TreeNode)nodeToDelete.Data[DataProps[DataNodeProperties.PhysicalNode]];
            MainView.RemoveItemFromCanvas(physicalTreeNode.PhysicalInstance);
            Label physicalLabel = (Label)nodeToDelete.Data[DataProps[DataNodeProperties.Text]];
            MainView.RemoveItemFromCanvas(physicalLabel);
            Arrow physicalArrow = (Arrow)nodeToDelete.Data[DataProps[DataNodeProperties.UpperArrow]];
            MainView.RemoveItemFromCanvas(physicalArrow.PhysicalInstance);

            _physicalItems.Delete(key);
            return true;
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
