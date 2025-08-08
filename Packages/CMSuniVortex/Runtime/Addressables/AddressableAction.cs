#if ENABLE_ADDRESSABLES
using System;

namespace CMSuniVortex.Addressable
{
    public readonly struct AddressableAction : IEquatable<AddressableAction>
    {
        public readonly string Guid;
        public readonly Action<string> CompletedAction;
        
        public AddressableAction(string guid, Action<string> completedAction)
        {
            Guid = guid;
            CompletedAction = completedAction;
        }

        public bool IsValid() => !string.IsNullOrEmpty(Guid) && CompletedAction != default;
        
        public static bool operator ==(AddressableAction lhs, AddressableAction rhs) => lhs.Equals(rhs);
        public static bool operator !=(AddressableAction lhs, AddressableAction rhs) => !(lhs == rhs);
        public bool Equals(AddressableAction other) => Guid == other.Guid;
        public override bool Equals(object obj) => obj is AddressableAction other && Equals(other);
        public override int GetHashCode() => (Guid != null ? Guid.GetHashCode() : 0);
    }
}
#endif