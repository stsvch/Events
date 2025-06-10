using Events.Domain.Exceptions;

namespace Events.Domain.Entities
{
    public class Category
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }

        public Category(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new InvariantViolationException("Category name is required.");
            }
            Name = name.Trim();
        }

        private Category() { }
    }
}