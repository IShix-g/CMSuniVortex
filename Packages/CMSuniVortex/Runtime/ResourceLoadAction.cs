
using System;

namespace CMSuniVortex
{
    public readonly struct ResourceLoadAction : IEquatable<ResourceLoadAction>
    {
        public readonly string ImagePath;
        public readonly Action<string> SuccessAction;
        
        public ResourceLoadAction(string imagePath, Action<string> successAction)
        {
            ImagePath = imagePath;
            SuccessAction = successAction;
        }

        public bool IsValid() => !string.IsNullOrEmpty(ImagePath) && SuccessAction != default;
        
        public bool Equals(ResourceLoadAction other)
            => ImagePath == other.ImagePath && Equals(SuccessAction, other.SuccessAction);

        public override bool Equals(object obj)
            => obj is ResourceLoadAction other && Equals(other);

        public override int GetHashCode()
            => HashCode.Combine(ImagePath, SuccessAction);

        public static bool operator ==(ResourceLoadAction left, ResourceLoadAction right)
            => left.Equals(right);

        public static bool operator !=(ResourceLoadAction left, ResourceLoadAction right)
            => !left.Equals(right);
    }
}