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
        public List<FamilyTree<T>> Children => children;
        public FamilyTree<T> Parent { get; private set; }
        public T value;

        public int Depth { get; private set; }
        private bool HasChildren => children.Count > 0;
        private bool HasVisitedAllChildren => timesVisited >= children.Count;
        private int timesVisited = 0;

        public FamilyTree()
        {
        }

        public List<List<FamilyTree<T>>> GetPaths()
        {
            var paths = new List<List<FamilyTree<T>>>();
            
            if (children.Count == 0)
                return null;

            FamilyTree<T> child = this;
            paths.Add(new List<FamilyTree<T>>());

            while (true)
            {
                while (child != null)
                {
                    if (!child.HasChildren)
                        break;

                    child = child.Children[child.timesVisited];
                    child.Parent.timesVisited++;
                    paths.Last().Add(child);
                }

                child = child.Parent;
                while (child.HasVisitedAllChildren)
                {
                    child.timesVisited = 0;
                    child = child.Parent;
                    if (child == null)
                        break;
                }

                if (child == null)
                    break;

                var newPath = new List<FamilyTree<T>>();
                foreach (var node in paths.Last())
                {
                    if (node.Parent == child)
                        break;

                    newPath.Add(node);
                }

                paths.Add(newPath);
            }

            return paths;
        }

        public FamilyTree<T> AddChild()
        {
            var child = new FamilyTree<T>();
            child.Parent = this;
            child.Depth = Depth + 1;
            children.Add(child);
            return child;
        }
    }
}
