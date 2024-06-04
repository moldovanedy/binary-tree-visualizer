using System;
using System.Collections.Generic;

namespace DataStructuresProject.Core
{
    public class BinarySearchTree : IDisposable
    {
        public int Depth
        {
            get
            {
                if (_cachedDepth >= 0)
                {
                    return _cachedDepth;
                }
                else
                {
                    int depth = GetSubTreeDepth(_root);
                    _cachedDepth = depth;
                    return depth;
                }
            }
        }
        private int _cachedDepth = -1;

        public int Size { get; private set; }

        private BinarySearchTreeNode? _root;

        public void Dispose()
        {
            List<int> nodeKeys = Traverse(TreeTraversalMode.Inorder);
            foreach (int nodeKey in nodeKeys)
            {
                Delete(nodeKey);
            }

            _cachedDepth = -1;
            Size = 0;
            _root = null;
            GC.SuppressFinalize(this);
        }

        #region Structure
        public void Insert(int newKey, Dictionary<string, object>? data = null)
        {
            if (_root == null)
            {
                _root = new BinarySearchTreeNode(newKey)
                {
                    Data = data ?? []
                };
            }
            else
            {
                _root.Insert(newKey, data);
            }

            //recalculate depth
            _cachedDepth = GetSubTreeDepth(_root);
            Size++;
        }

        public BinarySearchTreeNode? Search(int keyToSearch)
        {
            return _root?.Search(keyToSearch);
        }

        public bool Delete(int key)
        {
            if (_root == null)
            {
                return false;
            }

            BinarySearchTreeNode? parentNode = null;
            BinarySearchTreeNode? nodeToDelete = _root.Search(key, ref parentNode);
            if (nodeToDelete == null)
            {
                return false;
            }

            bool success = nodeToDelete.Delete(parentNode);
            if (!success)
            {
                if (nodeToDelete.Key == _root.Key)
                {
                    if (nodeToDelete.LeftNode == null && nodeToDelete.RightNode == null)
                    {
                        _root = null;
                    }
                    else if (nodeToDelete.LeftNode == null || nodeToDelete.RightNode == null)
                    {
                        if (nodeToDelete.LeftNode != null)
                        {
                            _root = nodeToDelete.LeftNode;
                        }
                        else
                        {
                            _root = nodeToDelete.RightNode;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            //recalculate depth
            _cachedDepth = GetSubTreeDepth(_root);
            Size--;
            return true;
        }

        public bool Modify(int oldKey, int newKey)
        {
            BinarySearchTreeNode? parentNode = null;
            BinarySearchTreeNode? nodeToDelete = _root?.Search(oldKey, ref parentNode);
            Dictionary<string, object> dataToCopy;
            if (nodeToDelete != null && parentNode != null)
            {
                dataToCopy = nodeToDelete.Data;
                nodeToDelete.Delete(parentNode);
            }
            else
            {
                return false;
            }

            //depth is recalculated here
            Insert(newKey, dataToCopy);
            return true;
        }

        public List<int> Traverse(TreeTraversalMode traversalMode)
        {
            List<int> results = [];
            if (_root == null)
            {
                return [];
            }

            Traverse(traversalMode, _root, results);
            return results;
        }

        private static void Traverse(TreeTraversalMode traversalMode, BinarySearchTreeNode currentNode, List<int> results)
        {
            if (traversalMode == TreeTraversalMode.Preorder)
            {
                results.Add(currentNode.Key);

                if (currentNode.LeftNode != null)
                {
                    Traverse(TreeTraversalMode.Preorder, currentNode.LeftNode, results);
                }

                if (currentNode.RightNode != null)
                {
                    Traverse(TreeTraversalMode.Preorder, currentNode.RightNode, results);
                }
            }
            else if (traversalMode == TreeTraversalMode.Inorder)
            {
                if (currentNode.LeftNode != null)
                {
                    Traverse(TreeTraversalMode.Inorder, currentNode.LeftNode, results);
                }

                results.Add(currentNode.Key);

                if (currentNode.RightNode != null)
                {
                    Traverse(TreeTraversalMode.Inorder, currentNode.RightNode, results);
                }
            }
            else if (traversalMode == TreeTraversalMode.Postorder)
            {
                if (currentNode.LeftNode != null)
                {
                    Traverse(TreeTraversalMode.Postorder, currentNode.LeftNode, results);
                }

                if (currentNode.RightNode != null)
                {
                    Traverse(TreeTraversalMode.Postorder, currentNode.RightNode, results);
                }

                results.Add(currentNode.Key);
            }
        }
        #endregion

        #region Utils
        public List<int> GetLeafNodes()
        {
            if (_root != null)
            {
                return GetSubtreeLeafNodes(_root);
            }
            else
            {
                return [];
            }
        }

        private static List<int> GetSubtreeLeafNodes(BinarySearchTreeNode node)
        {
            List<int> nodes = [];
            if (node.IsLeafNode)
            {
                nodes.Add(node.Key);
            }

            if (node.LeftNode != null)
            {
                nodes.AddRange(GetSubtreeLeafNodes(node.LeftNode));
            }
            if (node.RightNode != null)
            {
                nodes.AddRange(GetSubtreeLeafNodes(node.RightNode));
            }
            return nodes;
        }

        public BinarySearchTreeNode? GetRoot()
        {
            return _root;
        }

        public int GetNodeLevel(int key)
        {
            int level = 0;
            BinarySearchTreeNode? currentNode = _root;
            while (currentNode != null && currentNode.Key != key)
            {
                if (currentNode.Key > key)
                {
                    currentNode = currentNode.LeftNode;
                }
                else
                {
                    currentNode = currentNode.RightNode;
                }
                level++;
            }

            if (currentNode == null)
            {
                return -1;
            }
            else
            {
                return level;
            }
        }
        #endregion

        private static int GetSubTreeDepth(BinarySearchTreeNode? node)
        {
            if (node == null)
            {
                return 0;
            }
            else
            {
                bool hasChildren = false;
                int leftDepth = 0, rightDepth = 0;
                if (node.LeftNode != null)
                {
                    leftDepth = GetSubTreeDepth(node.LeftNode);
                    hasChildren = true;
                }
                if (node.RightNode != null)
                {
                    rightDepth = GetSubTreeDepth(node.RightNode);
                    hasChildren = true;
                }
                return hasChildren ? Math.Max(leftDepth, rightDepth) + 1 : 0;
            }
        }

        private static void GetSubTreeDepth(BinarySearchTreeNode node, ref int left, ref int right)
        {
            if (node.LeftNode != null)
            {
                left++;
                GetSubTreeDepth(node.LeftNode, ref left, ref right);
            }
            if (node.RightNode != null)
            {
                right++;
                GetSubTreeDepth(node.RightNode, ref left, ref right);
            }
        }
    }

    public class BinarySearchTreeNode
    {
        public int Key { get; set; } = 0;
        public Dictionary<string, object> Data { get; set; } = [];
        public BinarySearchTreeNode? LeftNode { get; set; } = null;
        public BinarySearchTreeNode? RightNode { get; set; } = null;

        public BinarySearchTreeNode() { }
        public BinarySearchTreeNode(int key)
        {
            Key = key;
        }

        #region Structure
        public void Insert(int newKey, Dictionary<string, object>? data = null)
        {
            if (newKey == Key)
            {
                throw new DuplicateKeyException(Key);
            }
            else if (newKey < Key)
            {
                if (LeftNode == null)
                {
                    LeftNode = new BinarySearchTreeNode(newKey)
                    {
                        Data = data ?? []
                    };
                }
                else
                {
                    LeftNode.Insert(newKey, data);
                }
            }
            else
            {
                if (RightNode == null)
                {
                    RightNode = new BinarySearchTreeNode(newKey)
                    {
                        Data = data ?? []
                    };
                }
                else
                {
                    RightNode.Insert(newKey, data);
                }
            }
        }

        public BinarySearchTreeNode? Search(int key)
        {
            if (key == Key)
            {
                return this;
            }
            else if (key < Key)
            {
                return this.LeftNode?.Search(key);
            }
            else
            {
                return this.RightNode?.Search(key);
            }
        }

        public BinarySearchTreeNode? Search(int key, ref BinarySearchTreeNode? parentNode)
        {
            if (key == Key)
            {
                return this;
            }
            else if (key < Key)
            {
                parentNode = this;
                return this.LeftNode?.Search(key, ref parentNode);
            }
            else
            {
                parentNode = this;
                return this.RightNode?.Search(key, ref parentNode);
            }
        }

        /// <summary>
        /// Deletes the current node and returns true if the deletion was successful,
        /// if the parent node is null it returns false as it requires special treatment (like setting the root to null or another node).
        /// </summary>
        /// <param name="parentNode"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(BinarySearchTreeNode? parentNode)
        {
            //leaf node
            if (this.LeftNode == null && this.RightNode == null)
            {
                if (parentNode != null)
                {
                    if (parentNode.LeftNode == this)
                    {
                        parentNode.LeftNode = null;
                    }
                    else
                    {
                        parentNode.RightNode = null;
                    }
                }
                else
                {
                    return false;
                }
            }
            //one child
            else if (this.LeftNode == null || this.RightNode == null)
            {
                //get the non-null one
                BinarySearchTreeNode? replacingChild = this.LeftNode ?? this.RightNode;
                if (parentNode != null)
                {
                    if (parentNode.LeftNode == this)
                    {
                        parentNode.LeftNode = replacingChild;
                    }
                    else
                    {
                        parentNode.RightNode = replacingChild;
                    }
                }
                else
                {
                    return false;
                }
            }
            //two children
            else
            {
                BinarySearchTreeNode localParent = this;
                //it shouldn't be possible to have null here but this will ensure this is the case
                BinarySearchTreeNode replacingNode = this.LeftNode ?? throw new Exception("An unknown internal error occurred");
                while (replacingNode.RightNode != null)
                {
                    localParent = replacingNode;
                    replacingNode = replacingNode.RightNode;
                }

                //we don't actually delete the node, but rather copy the data from a replacing node and delete that node
                this.Key = replacingNode.Key;
                //this will only get a reference to the dictionary, but it's ok because we never clear the dictionary,
                //just let the GC collect this node when no other variable will have any reference to it
                this.Data = replacingNode.Data;

                //this is a fix for the edge case when the replacing node will actually be this.LeftNode
                if (localParent != this)
                {
                    localParent!.RightNode = replacingNode.LeftNode;
                }
                else
                {
                    localParent!.LeftNode = replacingNode.LeftNode;
                }
            }

            return true;
        }
        #endregion

        #region Utils
        public bool IsLeafNode
        {
            get
            {
                return this.LeftNode == null && this.RightNode == null;
            }
        }

        public int GetMaxKeyFromSubtree()
        {
            BinarySearchTreeNode? current = this;
            int max = this.Key;
            while (current != null)
            {
                if (current.Key > max)
                {
                    max = current.Key;
                }
                current = current.RightNode;
            }

            return max;
        }

        public int GetMinKeyFromSubtree()
        {
            BinarySearchTreeNode? current = this;
            int min = this.Key;
            while (current != null)
            {
                if (current.Key < min)
                {
                    min = current.Key;
                }
                current = current.LeftNode;
            }

            return min;
        }


        #endregion

        #region Data
        public bool AddRecord(string key, object data)
        {
            return Data.TryAdd(key, data);
        }

        public bool UpdateRecord(string key, object newData)
        {
            if (Data.ContainsKey(key))
            {
                Data[key] = newData;
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool DeleteRecord(string key)
        {
            return Data.Remove(key);
        }
        #endregion
    }

    public enum TreeTraversalMode
    {
        None = 0,
        Preorder = 1,
        Inorder = 2,
        Postorder = 3,
    }
}
