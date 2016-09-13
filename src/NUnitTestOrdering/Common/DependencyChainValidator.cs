// ******************************************************************************
//  © 2016 Sebastiaan Dammann - damsteen.nl
// 
//  File:           : DependencyChainValidator.cs
//  Project         : NUnitTestOrdering
// ******************************************************************************
namespace NUnitTestOrdering.Common {
    using System;
    using System.Collections.Generic;
    using System.Linq;


    internal static class DependencyChainValidator {
        public static void Validate<T>(ICollection<T> listOfDependencies) where T : class, IDependencyIndicator<T> {
            foreach (T item in listOfDependencies) {
                DetectCyclicDependency(item, listOfDependencies);
            }
        }

        private static void DetectCyclicDependency<T>(T item, ICollection<T> listOfDependencies) where T : class, IDependencyIndicator<T> {
            HashSet<T> items = new HashSet<T>();

            // For each item in the list, we validate the dependency chain
            T currentDependency = item;
            do {
                if (!items.Add(currentDependency)) {
                    throw new TestOrderingException("Cyclic dependency detected between these dependencies: " + String.Join(" -> ", items));
                }
            } while ((currentDependency = FindDependency(currentDependency, listOfDependencies)) != null);
        }

        private static T FindDependency<T>(T item, IEnumerable<T> listOfDependencies) where T : class, IDependencyIndicator<T> {
            try {
                return item.HasDependencies ? listOfDependencies.First(x => x.IsDependencyOf(item)) : null;
            }
            catch (InvalidOperationException ex) {
                throw new TestOrderingException($"Broken dependency: Unable to find dependency of {item}", ex);
            }
        }
    }
}
