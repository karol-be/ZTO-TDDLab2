using System;

namespace BasicUtils
{
    public class BusinessRule<T> : IBusinessRule<T>, IRule where T : IValidatedObject
    {
        // Fields
        private string description;
        private Predicate<T> matchPredicate;
        private string name;

        // Methods
        public BusinessRule(string name, string description, Predicate<T> matchPredicate)
        {
            this.name = name;
            this.description = description;
            this.matchPredicate = matchPredicate;
        }

        public override bool Equals(object obj)
        {
            IBusinessRule<T> rule = obj as IBusinessRule<T>;
            if (rule != null)
            {
                return this.Name.Equals(rule.Name);
            }
            return true;
        }

        public override int GetHashCode()
        {
            return (this.matchPredicate.GetHashCode() + (0x1d * this.name.GetHashCode()));
        }

        public bool IsSatisfiedBy(T item)
        {
            return this.matchPredicate(item);
        }

        // Properties
        public string Description
        {
            get
            {
                return this.description;
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }
        }
    }
}