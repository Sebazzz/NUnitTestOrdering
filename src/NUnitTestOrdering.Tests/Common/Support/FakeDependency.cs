// ******************************************************************************
//  © 2016 Sebastiaan Dammann - damsteen.nl
// 
//  File:           : DependencySorter.cs
//  Project         : NUnitTestOrdering.Tests
// ******************************************************************************

namespace NUnitTestOrdering.Tests.Common.Support {
    using NUnitTestOrdering.Common;


    public sealed class FakeDependency : IDependencyIndicator<FakeDependency> {
        public int Id { get; }

        public int DependendOn { get; set; }

        public FakeDependency(int id, int dependendOn) {
            this.DependendOn = dependendOn;
            this.Id = id;
        }

        public bool Equals(FakeDependency other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return this.Id == other.Id;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            FakeDependency other = obj as FakeDependency;
            return other != null && this.Equals(other);
        }

        public override int GetHashCode() {
            return this.Id;
        }

        public bool HasDependencies => this.DependendOn != 0;

        public bool IsDependencyOf(FakeDependency other) {
            if (other.DependendOn == 0) {
                return false;
            }

            return this.Id == other.DependendOn;
        }

        public override string ToString() => "#" + this.Id.ToString();
    }
}
