// ******************************************************************************
//  © 2016 Sebastiaan Dammann - damsteen.nl
// 
//  File:           : DependencySorter.cs
//  Project         : NUnitTestOrdering
// ******************************************************************************
namespace NUnitTestOrdering.Common {
    using System.Collections.Generic;

    internal static class DependencySorter {
        /// <summary>
        ///     Sorts the specified list based on dependency order
        /// </summary>
        public static IEnumerable<T> Sort<T>(IEnumerable<T> enumerable) where T : class, IDependencyIndicator<T> {
            ICollection<T> items = (enumerable as ICollection<T>) ?? new List<T>(enumerable);

            DependencyChainValidator.Validate(items);

            // For each node, either find the previous or the next node in 
            // the list of dependencies. 
            LinkedList<T> listOfDependencies = new LinkedList<T>();
            foreach (T item in items) {
                // Find the dependency for the current node
                bool dependencyOrDependantFound = false;
                LinkedListNode<T> current = listOfDependencies.First;
                do {
                    if (current == null) continue;

                    if (item.IsDependencyOf(current.Value)) {
                        listOfDependencies.AddBefore(current, item);
                        dependencyOrDependantFound = true;
                        break;
                    }

                    if (current.Value.IsDependencyOf(item)) {
                        listOfDependencies.AddAfter(current, item);
                        dependencyOrDependantFound = true;
                        break;
                    }
                } while ((current = current?.Next) != null);

                if (!dependencyOrDependantFound) {
                    listOfDependencies.AddLast(item);
                }
            }

            // At this point we have sorted the list of dependencies 
            // but dependency chains themselves have an undefined order, so:
            // A -> B \-> C    0 -> 1 -> 2
            //         -> D    
            //
            // May be sorted as
            // 0 -> 1 -> 2     A -> B \-> D    
            //                         -> C    
            //
            // Also, items without any dependencies have a undefined position
            // although the will most likely be at the end of the list.
            return listOfDependencies;
        }
    }
}