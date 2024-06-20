#if ENABLE_ADDRESSABLES
using System;

namespace CMSuniVortex.Addressable
{
    public readonly struct AddressableAction : IEquatable<AddressableAction>
    {
        public readonly string Guid;
        public readonly Action<string> Completed;

        public bool IsValid() => !string.IsNullOrEmpty(Guid) && Completed != default;
        
        public AddressableAction(string guid, Action<string> completed)
        {
            Guid = guid;
            Completed = completed;
        }

        public static bool operator ==(AddressableAction lhs, AddressableAction rhs) => lhs.Equals(rhs);
        public static bool operator !=(AddressableAction lhs, AddressableAction rhs) => !(lhs == rhs);
        public bool Equals(AddressableAction other) => Guid == other.Guid;
        public override bool Equals(object obj) => obj is AddressableAction other && Equals(other);
        public override int GetHashCode() => (Guid != null ? Guid.GetHashCode() : 0);
    }
}
#endif