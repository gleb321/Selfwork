using NewVariant.Interfaces;

namespace Tests {
    public record FakeEntity : IEntity {
        public int Id => -1;
    }
}