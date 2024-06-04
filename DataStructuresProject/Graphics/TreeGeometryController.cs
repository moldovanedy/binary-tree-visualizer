using System.Collections.Generic;
using DataStructuresProject.Core;

namespace DataStructuresProject.Graphics
{
    public static class TreeGeometryController
    {
        public static void DetermineCorrectNodePositions(BinarySearchTree tree)
        {
            BinarySearchTreeNode? root = tree.GetRoot();
            if (root != null)
            {
                DetermineCorrectNodePosition(root);
            }
        }

        private static void DetermineCorrectNodePosition(BinarySearchTreeNode currentNode)
        {
            if (currentNode.LeftNode != null)
            {
                DetermineCorrectNodePosition(currentNode.LeftNode);
            }
            if (currentNode.RightNode != null)
            {
                DetermineCorrectNodePosition(currentNode.RightNode);
            }

            int leftBias = 0, rightBias = 0;
            if (currentNode.LeftNode != null)
            {
                if (currentNode.LeftNode.Data.TryGetValue("LeftBias", out object? childLeftBias))
                {
                    leftBias += (int)childLeftBias;
                }
                if (currentNode.LeftNode.Data.TryGetValue("RightBias", out object? childRightBias))
                {
                    leftBias += (int)childRightBias;
                }
                leftBias++;
            }
            if (currentNode.RightNode != null)
            {
                if (currentNode.RightNode.Data.TryGetValue("LeftBias", out object? childLeftBias))
                {
                    rightBias += (int)childLeftBias;
                }
                if (currentNode.RightNode.Data.TryGetValue("RightBias", out object? childRightBias))
                {
                    rightBias += (int)childRightBias;
                }
                rightBias++;
            }
            currentNode.Data["LeftBias"] = leftBias;
            currentNode.Data["RightBias"] = rightBias;
        }

        public static void SetCorrectNodePositions(BinarySearchTree tree)
        {
            BinarySearchTreeNode? root = tree.GetRoot();
            if (root != null)
            {
                int leftBias = 0, rightBias = 0;
                if (root.Data.TryGetValue("LeftBias", out object? rootLeftBias))
                {
                    leftBias = (int)rootLeftBias;
                }
                if (root.Data.TryGetValue("RightBias", out object? rootRightBias))
                {
                    rightBias = (int)rootRightBias;
                }

                if (root.LeftNode != null)
                {
                    SetCorrectNodePosition(root.LeftNode, new Vector2(), leftBias, rightBias, true);
                }
                if (root.RightNode != null)
                {
                    SetCorrectNodePosition(root.RightNode, new Vector2(), leftBias, rightBias, false);
                }

                if (root.Data[CanvasController.DataProps[DataNodeProperties.PhysicalNode]] is CanvasItem canvasItem)
                {
                    canvasItem.Position = new Vector2();
                }
                root.DeleteRecord("LeftBias");
                root.DeleteRecord("RightBias");
            }
        }

        private static void SetCorrectNodePosition(BinarySearchTreeNode node, Vector2 parentPosition, int parentLeftBias, int parentRightBias, bool isLeftChild)
        {
            if (node.Data.TryGetValue(CanvasController.DataProps[DataNodeProperties.PhysicalNode], out object? physicalNodeData))
            {
                if (physicalNodeData is CanvasItem canvasItem)
                {
                    object leftBias = node.Data.GetValueOrDefault("LeftBias", 0);
                    object rightBias = node.Data.GetValueOrDefault("RightBias", 0);

                    if (isLeftChild)
                    {
                        canvasItem.Position.X = parentPosition.X - (parentLeftBias - (int)leftBias);
                    }
                    else
                    {
                        canvasItem.Position.X = parentPosition.X + (parentRightBias - (int)rightBias);
                    }

                    canvasItem.Position.Y = parentPosition.Y - 2;
                    if (node.LeftNode != null)
                    {
                        SetCorrectNodePosition(node.LeftNode, canvasItem.Position, (int)(leftBias ?? 0), (int)(rightBias ?? 0), true);
                    }
                    if (node.RightNode != null)
                    {
                        SetCorrectNodePosition(node.RightNode, canvasItem.Position, (int)(leftBias ?? 0), (int)(rightBias ?? 0), false);
                    }

                    node.DeleteRecord("LeftBias");
                    node.DeleteRecord("RightBias");
                }
            }
        }
    }
}
