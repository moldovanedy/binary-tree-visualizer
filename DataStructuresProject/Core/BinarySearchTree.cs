using System;
using System.Collections.Generic;

namespace DataStructuresProject.Core
{
    public class BinarySearchTree
    {
        public int Depth
        {
            get
            {
                if (_cachedDepth != null)
                {
                    return _cachedDepth.Value;
                }
                else
                {
                    int depth = GetSubTreeDepth(_root);
                    _cachedDepth = depth;
                    return depth;
                }
            }
        }
        private int? _cachedDepth;

        private BinarySearchTreeNode? _root;

        /*public BinarySearchTree()
        {
            _root = new BinarySearchTreeNode();
        }*/

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
        }

        public BinarySearchTreeNode? Search(int keyToSearch)
        {
            return _root?.Search(keyToSearch);
        }

        public bool Delete(int key)
        {
            BinarySearchTreeNode? parentNode = null;
            BinarySearchTreeNode? nodeToDelete = _root?.Search(key, ref parentNode);
            if (nodeToDelete == null || parentNode == null)
            {
                return false;
            }

            nodeToDelete.Delete(parentNode);
            /*//leaf node
            if (nodeToDelete.LeftNode == null && nodeToDelete.RightNode == null)
            {
                if (parentNode != null)
                {
                    if (parentNode.LeftNode == nodeToDelete)
                    {
                        parentNode.LeftNode = null;
                    }
                    else
                    {
                        parentNode.RightNode = null;
                    }
                }
                //TODO: can reach here if we delete the root node, implement this case
                else
                {
                    return false;
                }
            }
            //one child
            else if (nodeToDelete.LeftNode == null || nodeToDelete.RightNode == null)
            {
                //get the non-null one
                BinarySearchTreeNode? replacingChild = nodeToDelete.LeftNode ?? nodeToDelete.RightNode;
                if (parentNode != null)
                {
                    if (parentNode.LeftNode == nodeToDelete)
                    {
                        parentNode.LeftNode = replacingChild;
                    }
                    else
                    {
                        parentNode.RightNode = replacingChild;
                    }
                }
                //shouldn't be possible to reach here
                else
                {
                    return false;
                }
            }
            //two children
            else
            {
                //it shouldn't be possible to have null here but this will ensure this is the case
                BinarySearchTreeNode replacingNode = nodeToDelete.LeftNode ?? throw new Exception("An unknown internal error occurred");
                while (replacingNode.RightNode != null)
                {
                    replacingNode = replacingNode.RightNode;
                }

                //we don't actually delete the node, but rather copy the data from a replacing node and delete that node
                nodeToDelete.Key = replacingNode.Key;
                nodeToDelete.Data = replacingNode.Data;
                //?!?! needs testing
                parentNode!.RightNode = replacingNode.LeftNode;
            }*/

            //recalculate depth
            _cachedDepth = GetSubTreeDepth(_root);
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

            Insert(newKey, dataToCopy);

            //recalculate depth
            _cachedDepth = GetSubTreeDepth(_root);
            return true;
        }

        public int[] Traverse(TreeTraversalMode traversalMode)
        {
            //TODO
            return [];
        }
        #endregion

        public BinarySearchTreeNode? GetRoot()
        {
            return _root;
        }

        private static int GetSubTreeDepth(BinarySearchTreeNode? node)
        {
            if (node == null)
            {
                return 0;
            }
            else
            {
                int leftDepth = 0, rightDepth = 0;

                if (node.LeftNode != null)
                {
                    leftDepth += GetSubTreeDepth(node.LeftNode);
                }
                else
                {
                    return 1;
                }

                if (node.RightNode != null)
                {
                    rightDepth += GetSubTreeDepth(node.RightNode);
                }
                else
                {
                    return 1;
                }

                return Math.Max(leftDepth, rightDepth);
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

        public bool Delete(BinarySearchTreeNode parentNode)
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
                //TODO: can reach here if we delete the root node, implement this case
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
                //TODO: can reach here if we delete the root node, implement this case
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
                //should work now
                localParent!.RightNode = replacingNode.LeftNode;
            }

            return true;
        }
        #endregion

        #region Utils
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
