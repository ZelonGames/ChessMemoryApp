using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessMemoryApp.Model.Threat_Finder
{
    public class FamilyTree<T>
    {
        private readonly List<FamilyTree<T>> children = new();
        private readonly List<FamilyTree<T>> leafNodes = new();
        public List<FamilyTree<T>> LeafNodes => Root.leafNodes;
        public List<FamilyTree<T>> Children => children;
        public FamilyTree<T> Parent { get; private set; }
        public FamilyTree<T> Root { get; private set; }
        public T value;

        public int Depth { get; private set; }
        public bool HasChildren => children.Count > 0;

        public FamilyTree()
        {
            Root = this;
        }

        public List<Stack<FamilyTree<T>>> GetPaths()
        {
            var paths = new List<Stack<FamilyTree<T>>>();

            foreach (var node in leafNodes)
            {
                var path = GetPathFromChildToRoot(node);
                paths.Add(path);
            }

            return paths;
        }

        public static Stack<FamilyTree<T>> GetPathFromChildToRoot(FamilyTree<T> child)
        {
            var path = new Stack<FamilyTree<T>>(child.Depth);
            FamilyTree<T> parent = child;

            do
            {
                path.Push(parent);
                parent = parent.Parent;
            }
            while (parent != null && parent.value != null);

            return path;
        }

        public FamilyTree<T> AddChild(T value)
        {
            var child = new FamilyTree<T>();
            child.Parent = this;
            child.Depth = child.Parent.Depth + 1;
            child.Root = child.Parent.Root;
            child.value = value;
            children.Add(child);
            UpdateLeafNodes(child);
            return child;
        }

        private void UpdateLeafNodes(FamilyTree<T> addedChild)
        {
            Root.leafNodes.Remove(addedChild.Parent);
            Root.leafNodes.Add(addedChild);
        }
    }
}
